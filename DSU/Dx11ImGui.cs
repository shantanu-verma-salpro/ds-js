using System.Numerics;
using System.Runtime.InteropServices;
using Win32;
using Win32.Graphics.Direct3D;
using Win32.Graphics.Direct3D.Dxc;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi;
using Win32.Graphics.Dxgi.Common;
using Win32.Numerics;
using static Win32.Graphics.Direct3D.Dxc.Apis;
using static Win32.Graphics.Direct3D11.Apis;
using static Win32.Graphics.Dxgi.Apis;
using static Win32.Apis;
using MapFlags = Win32.Graphics.Direct3D11.MapFlags;
using Vortice.D3DCompiler;
using System.Text;

namespace ImGuiNET
{
    public class Dx11
    {


        public struct ImGui_ImplDX11_Data
        {
            public ComPtr<ID3D11Device> _device;
            public ComPtr<ID3D11DeviceContext> _deviceContext;
            public ComPtr<IDXGIFactory> _factory;
            public ComPtr<ID3D11Buffer> _vertexBuffer;
            public ComPtr<ID3D11Buffer> _indexBuffer;
            public ComPtr<ID3D11VertexShader> _vertexShader;
            public ComPtr<ID3D11InputLayout> _inputLayout;
            public ComPtr<ID3D11Buffer> _vertexConstantBuffer;
            public ComPtr<ID3D11PixelShader> _pixelShader;
            public ComPtr<ID3D11SamplerState> _fontSampler;
            public ComPtr<ID3D11ShaderResourceView> _fontTextureView;
            public ComPtr<ID3D11RasterizerState> _rasterizerState;
            public ComPtr<ID3D11BlendState> _blendState;
            public ComPtr<ID3D11DepthStencilState> _depthStencilState;
            public ComPtr<IDXGISwapChain1> _swapchain;
            public int VertexBufferSize;
            public int IndexBufferSize;
            public ImGui_ImplDX11_Data() { VertexBufferSize = 5000; IndexBufferSize = 1000; }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct VERTEX_CONSTANT_BUFFER_DX11
        {
            public Matrix4x4 mvp;
        }


        private static unsafe ImGui_ImplDX11_Data* ImGui_ImplDX11_GetBackendData()
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                return null;

            return (ImGui_ImplDX11_Data*)ImGui.GetIO().BackendRendererUserData;
        }

        public static unsafe void ImGui_ImplDX11_SetupRenderState(ref ImDrawData drawData, ID3D11DeviceContext* context)
        {
            var bd = ImGui_ImplDX11_GetBackendData();
            if (bd == null)
                return;

            var viewport = new Viewport(0, 0, drawData.DisplaySize.X, drawData.DisplaySize.Y, 0, 1);
            context->RSSetViewports(1, &viewport);

            uint stride = (uint)sizeof(ImDrawVert);
            uint offset = 0;
            context->IASetInputLayout(bd->_inputLayout.Get());
            context->IASetVertexBuffers(0, 1, bd->_vertexBuffer.GetAddressOf(), &stride, &offset);
            context->IASetIndexBuffer(bd->_indexBuffer.Get(), sizeof(ushort) == 2 ? Format.R16Uint : Format.R32Uint, 0);
            context->IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

            context->VSSetShader(bd->_vertexShader.Get(), null, 0);
            context->VSSetConstantBuffers(0, 1, bd->_vertexConstantBuffer.GetAddressOf());
            context->PSSetShader(bd->_pixelShader.Get(), null, 0);
            context->PSSetSamplers(0, 1, bd->_fontSampler.GetAddressOf());

            context->GSSetShader(null, null, 0);
            context->HSSetShader(null, null, 0);
            context->DSSetShader(null, null, 0);
            context->CSSetShader(null, null, 0);

            var blendFactor = Vector4.Zero;
            context->OMSetBlendState(bd->_blendState.Get(), (float*)&blendFactor, 0xffffffff);
            context->OMSetDepthStencilState(bd->_depthStencilState.Get(), 0);
            context->RSSetState(bd->_rasterizerState.Get());

        }
        public unsafe static void ImGui_ImplDX11_RenderDrawData(ref ImDrawData drawData)
        {
            if (drawData.DisplaySize.X <= 0.0f || drawData.DisplaySize.Y <= 0.0f)
                return;

            var bd = ImGui_ImplDX11_GetBackendData();
            var deviceContext = bd->_deviceContext.Get();

            if (bd->_vertexBuffer.Get() == null || bd->VertexBufferSize < drawData.TotalVtxCount)
            {
                if (bd->_vertexBuffer.Get() != null)
                {
                    bd->_vertexBuffer.Get()->Release();
                    bd->_vertexBuffer = default;
                }

                bd->VertexBufferSize = drawData.TotalVtxCount + 5000;

                BufferDescription desc = new()
                {
                    Usage = Win32.Graphics.Direct3D11.Usage.Dynamic,
                    ByteWidth = (uint)(bd->VertexBufferSize * sizeof(ImDrawVert)),
                    BindFlags = D3D11_BIND_VERTEX_BUFFER,
                    CPUAccessFlags = CpuAccessFlags.Write,
                    MiscFlags = 0
                };


                var result = bd->_device.Get()->CreateBuffer(&desc, null, bd->_vertexBuffer.GetAddressOf());
                if (result.Failure)
                {
                    return;
                }
            }
            if (bd->_indexBuffer.Get() == null || bd->IndexBufferSize < drawData.TotalIdxCount)
            {
                if (bd->_indexBuffer.Get() != null)
                {
                    bd->_indexBuffer.Get()->Release();
                    bd->_indexBuffer = default;
                }

                bd->IndexBufferSize = drawData.TotalIdxCount + 10000;

                BufferDescription desc = new()
                {
                    Usage = Win32.Graphics.Direct3D11.Usage.Dynamic,
                    ByteWidth = (uint)(bd->IndexBufferSize * sizeof(ushort)),
                    BindFlags = D3D11_BIND_INDEX_BUFFER,
                    CPUAccessFlags = CpuAccessFlags.Write,
                    MiscFlags = 0
                };

                var result = bd->_device.Get()->CreateBuffer(&desc, null, bd->_indexBuffer.GetAddressOf());
                if (result.Failure)
                {
                    return;
                }
            }

            var vtxResource = new MappedSubresource();
            var idxResource = new MappedSubresource();


            var mapResultVtx = deviceContext->Map((ID3D11Resource*)bd->_vertexBuffer.Get(), 0, MapMode.WriteDiscard, Win32.Graphics.Direct3D11.MapFlags.None, &vtxResource);
            if (mapResultVtx.Failure)
                return;

            var mapResultIdx = deviceContext->Map((ID3D11Resource*)bd->_indexBuffer.Get(), 0, MapMode.WriteDiscard, Win32.Graphics.Direct3D11.MapFlags.None, &idxResource);
            if (mapResultIdx.Failure)
                return;

            ImDrawVert* vtxDst = (ImDrawVert*)vtxResource.pData;

            ushort* idxDst = (ushort*)idxResource.pData;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawList* drawList = (ImDrawList*)drawData.CmdLists.Address<ImDrawList>(n);
                System.Buffer.MemoryCopy(drawList->VtxBuffer.Data.ToPointer(), vtxDst, drawList->VtxBuffer.Size * sizeof(ImDrawVert), drawList->VtxBuffer.Size * sizeof(ImDrawVert));
                System.Buffer.MemoryCopy(drawList->IdxBuffer.Data.ToPointer(), idxDst, drawList->IdxBuffer.Size * sizeof(ushort), drawList->IdxBuffer.Size * sizeof(ushort));

                vtxDst += drawList->VtxBuffer.Size;
                idxDst += drawList->IdxBuffer.Size;
            }


            deviceContext->Unmap((ID3D11Resource*)bd->_vertexBuffer.Get(), 0);
            deviceContext->Unmap((ID3D11Resource*)bd->_indexBuffer.Get(), 0);

            {
                var mappedResource = new MappedSubresource();

                if (deviceContext->Map((ID3D11Resource*)bd->_vertexConstantBuffer.Get(), 0, MapMode.WriteDiscard, MapFlags.None, &mappedResource).Failure)
                    return;

                var constantBuffer = (VERTEX_CONSTANT_BUFFER_DX11*)mappedResource.pData;

                float L = drawData.DisplayPos.X;
                float R = drawData.DisplayPos.X + drawData.DisplaySize.X;
                float T = drawData.DisplayPos.Y;
                float B = drawData.DisplayPos.Y + drawData.DisplaySize.Y;

                float[,] mvp = new float[4, 4]
                {
                    { 2.0f / (R - L), 0.0f,           0.0f,       0.0f },
                    { 0.0f,           2.0f / (T - B), 0.0f,       0.0f },
                    { 0.0f,           0.0f,           0.5f,       0.0f },
                    { (R + L) / (L - R), (T + B) / (B - T), 0.5f,  1.0f },
                };

                fixed (float* mvpPtr = &mvp[0, 0])
                {
                    Buffer.MemoryCopy(mvpPtr, &constantBuffer->mvp, sizeof(float) * 16, sizeof(float) * 16);
                }
                deviceContext->Unmap((ID3D11Resource*)bd->_vertexConstantBuffer.Get(), 0);
            }

            ImGui_ImplDX11_SetupRenderState(ref drawData, deviceContext);

            int global_idx_offset = 0;
            int global_vtx_offset = 0;
            Vector2 clip_off = drawData.DisplayPos;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawList* drawList = (ImDrawList*)drawData.CmdLists.Address<ImDrawList>(n);
                for (int i = 0; i < drawList->CmdBuffer.Size; i++)
                {

                    ImDrawCmd* cmd = (ImDrawCmd*)drawList->CmdBuffer.Address<ImDrawCmd>(i);
                    if (cmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException("user callbacks not implemented");
                    }
                    else
                    {
                        Vector2 clip_min = new(cmd->ClipRect.X - clip_off.X, cmd->ClipRect.Y - clip_off.Y);
                        Vector2 clip_max = new(cmd->ClipRect.Z - clip_off.X, cmd->ClipRect.W - clip_off.Y);
                        if (clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                            continue;

                        Rect r = new Rect((int)clip_min.X, (int)clip_min.Y, (int)clip_max.X, (int)clip_max.Y);
                        deviceContext->RSSetScissorRects(1, &r);

                        ID3D11ShaderResourceView* texture_srv = (ID3D11ShaderResourceView*)cmd->TextureId;
                        deviceContext->PSSetShaderResources(0, 1, &texture_srv);
                        deviceContext->DrawIndexed(cmd->ElemCount, (uint)(cmd->IdxOffset + global_idx_offset), (int)(cmd->VtxOffset + global_vtx_offset));
                    }
                }
                global_idx_offset += drawList->IdxBuffer.Size;
                global_vtx_offset += drawList->VtxBuffer.Size;
            }
        }
        static unsafe void ImGui_ImplDX11_CreateFontsTexture()
        {
            var io = ImGui.GetIO();
            ImGui_ImplDX11_Data* bd = ImGui_ImplDX11_GetBackendData();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

            var texDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R8G8B8A8Unorm,
                SampleDesc = new SampleDescription { Count = 1 },
                Usage = Win32.Graphics.Direct3D11.Usage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = (uint)CpuAccessFlags.None
            };

            var subResource = new SubresourceData
            {
                pSysMem = pixels,
                SysMemPitch = texDesc.Width * 4,
                SysMemSlicePitch = 0
            };

            ComPtr<ID3D11Texture2D> texture = default;
            bd->_device.Get()->CreateTexture2D(&texDesc, &subResource, texture.GetAddressOf());

            var resViewDesc = new ShaderResourceViewDescription
            {
                Format = Format.R8G8B8A8Unorm,
                ViewDimension = SrvDimension.Texture2D,
                Texture2D = new() { MipLevels = texDesc.MipLevels, MostDetailedMip = 0 }
            };

            bd->_device.Get()->CreateShaderResourceView((ID3D11Resource*)texture.Get(), &resViewDesc, bd->_fontTextureView.GetAddressOf());
            texture.Get()->Release();

            io.Fonts.SetTexID((nint)bd->_fontTextureView.Get());

            var samplerDesc = new SamplerDescription(
                Filter.MinMagMipLinear,
                TextureAddressMode.Clamp,
                TextureAddressMode.Clamp,
                TextureAddressMode.Clamp,
                0f,
                0,
                ComparisonFunction.Always,
                0f,
                0f
            );

            bd->_device.Get()->CreateSamplerState(&samplerDesc, bd->_fontSampler.GetAddressOf());
        }
        static unsafe bool ImGui_ImplDX11_CreateDeviceObjects()
        {
            ImGui_ImplDX11_Data* bd = ImGui_ImplDX11_GetBackendData();
            if (bd->_device.Get() == null)
                return false;
            if (bd->_fontSampler.Get() != null)
                ImGui_ImplDX11_InvalidateDeviceObjects();
            string shaderSource =
          @"cbuffer vertexBuffer : register(b0)

            {
              float4x4 ProjectionMatrix;
            };
            struct VS_INPUT
            {
              float2 pos : POSITION;
              float2 uv  : TEXCOORD0;
              float4 col : COLOR0;
            };

            struct PS_INPUT
            {
              float4 pos : SV_POSITION;
              float4 col : COLOR0;
              float2 uv  : TEXCOORD0;
            };

            PS_INPUT main(VS_INPUT input)
            {
              PS_INPUT output;
              output.pos = mul(ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));
              output.col = input.col;
              output.uv = input.uv;
              return output;
            }
            ";


            var res = Compiler.Compile(shaderSource, "main", "vs", "vs_4_0", out Vortice.Direct3D.Blob vertexShaderBlob, out Vortice.Direct3D.Blob error);
            if (res.Failure || vertexShaderBlob == null)
                throw new Exception("error compiling vertex shader");

            if (bd->_device.Get()->CreateVertexShader(vertexShaderBlob.BufferPointer.ToPointer(), vertexShaderBlob.BufferSize, null, bd->_vertexShader.GetAddressOf()) != HResult.Ok)
            {
                vertexShaderBlob.Release();
                return false;
            }

            fixed (byte* positionPtr = Encoding.UTF8.GetBytes("POSITION"))
            fixed (byte* texcoordPtr = Encoding.UTF8.GetBytes("TEXCOORD"))
            fixed (byte* colorPtr = Encoding.UTF8.GetBytes("COLOR"))
            {
                var inputElements = new[]
                {
                    new InputElementDescription
                    {
                        SemanticName = positionPtr,
                        SemanticIndex = 0,
                        Format = Format.R32G32Float,
                        InputSlot = 0,
                        AlignedByteOffset = 0,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElementDescription
                    {
                        SemanticName = texcoordPtr,
                        SemanticIndex = 0,
                        Format = Format.R32G32Float,
                        InputSlot = 0,
                        AlignedByteOffset = 8,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElementDescription
                    {
                        SemanticName = colorPtr,
                        SemanticIndex = 0,
                        Format = Format.R8G8B8A8Unorm,
                        InputSlot = 0,
                        AlignedByteOffset = 16,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };

                fixed (InputElementDescription* inputElementPtr = &inputElements[0])
                {
                    if (bd->_device.Get()->CreateInputLayout(inputElementPtr, 3, vertexShaderBlob.BufferPointer.ToPointer(), vertexShaderBlob.BufferSize, bd->_inputLayout.GetAddressOf()) != HResult.Ok)
                    {
                        vertexShaderBlob.Release();
                        return false;
                    }
                    vertexShaderBlob.Release();
                }
            }

            var constBufferDesc = new BufferDescription(
                (uint)Marshal.SizeOf(typeof(VERTEX_CONSTANT_BUFFER_DX11)),
                BindFlags.ConstantBuffer,
                Win32.Graphics.Direct3D11.Usage.Dynamic,
                CpuAccessFlags.Write);

            bd->_device.Get()->CreateBuffer(&constBufferDesc, null, bd->_vertexConstantBuffer.GetAddressOf());

            var pixelShaderCode =
                @"struct PS_INPUT
                    {
                        float4 pos : SV_POSITION;
                        float4 col : COLOR0;
                        float2 uv  : TEXCOORD0;
                    };

                    sampler sampler0;
                    Texture2D texture0;

                    float4 main(PS_INPUT input) : SV_Target
                    {
                        return input.col * texture0.Sample(sampler0, input.uv);
                    }";

            Compiler.Compile(pixelShaderCode, "main", "ps", "ps_4_0", out Vortice.Direct3D.Blob pixelShaderBlob, out _);
            if (pixelShaderBlob == null)
                throw new Exception("error compiling pixel shader");
            if (bd->_device.Get()->CreatePixelShader(pixelShaderBlob.BufferPointer.ToPointer(), pixelShaderBlob.BufferSize, null, bd->_pixelShader.GetAddressOf()) != HResult.Ok)
            {
                pixelShaderBlob.Release();
                return false;
            }
            pixelShaderBlob.Release();

            var blendDesc = new BlendDescription(Blend.SrcAlpha, Blend.InverseSrcAlpha, Blend.One, Blend.InverseSrcAlpha);
            bd->_device.Get()->CreateBlendState(&blendDesc, bd->_blendState.GetAddressOf());
            var rasterDesc = new RasterizerDescription(CullMode.None, FillMode.Solid)
            {
                MultisampleEnable = false,
                ScissorEnable = true
            };

            bd->_device.Get()->CreateRasterizerState(&rasterDesc,bd->_rasterizerState.GetAddressOf());

            
            var depthDesc = new DepthStencilDescription(false, DepthWriteMask.All, ComparisonFunction.Always);
            bd->_device.Get()->CreateDepthStencilState(&depthDesc,bd->_depthStencilState.GetAddressOf());

            ImGui_ImplDX11_CreateFontsTexture();
            return true;

        }
        static unsafe void ImGui_ImplDX11_InvalidateDeviceObjects()
        {
            ImGui_ImplDX11_Data* bd = ImGui_ImplDX11_GetBackendData();
            if (bd->_device.Get() == null)
                return;

            if (bd->_fontSampler.Get() != null)
            {
                bd->_fontSampler.Get()->Release();
                bd->_fontSampler = default;
            }

            if (bd->_fontTextureView.Get() != null)
            {
                bd->_fontTextureView.Get()->Release();
                bd->_fontTextureView = default;
                ImGui.GetIO().Fonts.SetTexID(0);
            }

            if (bd->_indexBuffer.Get() != null)
            {
                bd->_indexBuffer.Get()->Release();
                bd->_indexBuffer = default;
            }

            if (bd->_vertexBuffer.Get() != null)
            {
                bd->_vertexBuffer.Get()->Release();
                bd->_vertexBuffer = default;
            }

            if (bd->_blendState.Get() != null)
            {
                bd->_blendState.Get()->Release();
                bd->_blendState = default;
            }

            if (bd->_depthStencilState.Get() != null)
            {
                bd->_depthStencilState.Get()->Release();
                bd->_depthStencilState = default;
            }

            if (bd->_rasterizerState.Get() != null)
            {
                bd->_rasterizerState.Get()->Release();
                bd->_rasterizerState = default;
            }

            if (bd->_pixelShader.Get() != null)
            {
                bd->_pixelShader.Get()->Release();
                bd->_pixelShader = default;
            }

            if (bd->_vertexConstantBuffer.Get() != null)
            {
                bd->_vertexConstantBuffer.Get()->Release();
                bd->_vertexConstantBuffer = default;
            }

            if (bd->_inputLayout.Get() != null)
            {
                bd->_inputLayout.Get()->Release();
                bd->_inputLayout = default;
            }

            if (bd->_vertexShader.Get() != null)
            {
                bd->_vertexShader.Get()->Release();
                bd->_vertexShader = default;
            }

        }
    }
}
