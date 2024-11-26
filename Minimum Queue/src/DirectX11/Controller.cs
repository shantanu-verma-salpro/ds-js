using System.Numerics;
using Win32.Graphics.Direct3D;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi.Common;
using Win32.Numerics;
using static ImGuiNET.DxStucts;
using static ImGuiNET.DxResources;
using static Win32.Graphics.Direct3D.Fxc.Apis;
using static Win32.Graphics.Direct3D11.Apis;
using Win32;
using static Win32.Apis;
using System.Runtime.InteropServices;
using Win32.Graphics.Direct3D.Fxc;
using Win32.Graphics.Dxgi;
using Usage = Win32.Graphics.Direct3D11.Usage;
using MapFlags = Win32.Graphics.Direct3D11.MapFlags;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
    public static class DxController
    {
        private static unsafe ImGuiDxData* GetBackendData()
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero) return null;
            return (ImGuiDxData*)ImGui.GetIO().BackendRendererUserData.ToPointer();
        }

        public static unsafe void SetupRenderState(ImDrawDataPtr drawData)
        {
            var bd = GetBackendData();
            ID3D11DeviceContext* context = bd->_deviceContext.Get();

            if (bd == null || context == null)
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
        public unsafe static void RenderDrawData(ImDrawDataPtr drawData)
        {
            if (drawData.DisplaySize.X <= 0.0f || drawData.DisplaySize.Y <= 0.0f || drawData.CmdListsCount == 0)
                return;

            BufferDescription desc = default;
            MappedSubresource vtxResource = default;
            MappedSubresource idxResource = default;
            MappedSubresource cbxResource = default;
            Rect rect = default;
            Vector2 clip_min = default;
            Vector2 clip_max = default;

            var bd = GetBackendData();
            ID3D11DeviceContext* deviceContext = bd->_deviceContext.Get();

            if (bd->_vertexBuffer.Get() == null || bd->VertexBufferSize < drawData.TotalVtxCount)
            {
                bd->_vertexBuffer.Dispose();

                bd->VertexBufferSize = drawData.TotalVtxCount + 5000;
                desc.Usage = Usage.Dynamic;
                desc.ByteWidth = (uint)(bd->VertexBufferSize * sizeof(ImDrawVert));
                desc.BindFlags = BindFlags.VertexBuffer;
                desc.CPUAccessFlags = CpuAccessFlags.Write;
                desc.MiscFlags = 0;

                if (bd->_device.Get()->CreateBuffer(&desc, null, bd->_vertexBuffer.GetAddressOf()).Failure)
                {
                    Console.WriteLine("CreateBuffer _vertexBuffer() fail");
                    return;
                }

            }

            if (bd->_indexBuffer.Get() == null || bd->IndexBufferSize < drawData.TotalIdxCount)
            {
                bd->_indexBuffer.Dispose();

                bd->IndexBufferSize = drawData.TotalIdxCount + 10000;
                desc.Usage = Usage.Dynamic;
                desc.ByteWidth = (uint)(bd->IndexBufferSize * sizeof(ushort));
                desc.BindFlags = BindFlags.IndexBuffer;
                desc.CPUAccessFlags = CpuAccessFlags.Write;
                desc.MiscFlags = 0;

                if (bd->_device.Get()->CreateBuffer(&desc, null, bd->_indexBuffer.GetAddressOf()).Failure)
                {
                    Console.WriteLine("Index buffer createBuffer() fail");
                    return;
                }
            }

            if (deviceContext->Map((ID3D11Resource*)bd->_vertexBuffer.Get(), 0, MapMode.WriteDiscard, MapFlags.None, &vtxResource).Failure)
            {
                Console.WriteLine("Vertex buffer mapping fail");
                return;
            }

            if (deviceContext->Map((ID3D11Resource*)bd->_indexBuffer.Get(), 0, MapMode.WriteDiscard, MapFlags.None, &idxResource).Failure)
            {
                Console.WriteLine("index buffer mapping fail");
                return;
            }

            ImDrawVert* vtxDst = (ImDrawVert*)vtxResource.pData;
            ushort* idxDst = (ushort*)idxResource.pData;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr drawList = drawData.CmdLists[n];
                Buffer.MemoryCopy(drawList.VtxBuffer.Data.ToPointer(), vtxDst, drawList.VtxBuffer.Size * sizeof(ImDrawVert), drawList.VtxBuffer.Size * sizeof(ImDrawVert));
                Buffer.MemoryCopy(drawList.IdxBuffer.Data.ToPointer(), idxDst, drawList.IdxBuffer.Size * sizeof(ushort), drawList.IdxBuffer.Size * sizeof(ushort));

                vtxDst += drawList.VtxBuffer.Size;
                idxDst += drawList.IdxBuffer.Size;
            }

            deviceContext->Unmap((ID3D11Resource*)bd->_vertexBuffer.Get(), 0);
            deviceContext->Unmap((ID3D11Resource*)bd->_indexBuffer.Get(), 0);

            {
                if (deviceContext->Map((ID3D11Resource*)bd->_vertexConstantBuffer.Get(), 0, MapMode.WriteDiscard, MapFlags.None, &cbxResource).Failure)
                {
                    Console.WriteLine("Vertex constant buffer mapping fail");
                    return;
                }

                var constantBuffer = (VERTEX_CONSTANT_BUFFER_DX11*)cbxResource.pData;

                float L = drawData.DisplayPos.X;
                float R = drawData.DisplayPos.X + drawData.DisplaySize.X;
                float T = drawData.DisplayPos.Y;
                float B = drawData.DisplayPos.Y + drawData.DisplaySize.Y;

                constantBuffer->mvp.M11 = 2.0f / (R - L);
                constantBuffer->mvp.M12 = 0.0f;
                constantBuffer->mvp.M13 = 0.0f;
                constantBuffer->mvp.M14 = 0.0f;

                constantBuffer->mvp.M21 = 0.0f;
                constantBuffer->mvp.M22 = 2.0f / (T - B);
                constantBuffer->mvp.M23 = 0.0f;
                constantBuffer->mvp.M24 = 0.0f;

                constantBuffer->mvp.M31 = 0.0f;
                constantBuffer->mvp.M32 = 0.0f;
                constantBuffer->mvp.M33 = 0.5f;
                constantBuffer->mvp.M34 = 0.0f;

                constantBuffer->mvp.M41 = (R + L) / (L - R);
                constantBuffer->mvp.M42 = (T + B) / (B - T);
                constantBuffer->mvp.M43 = 0.5f;
                constantBuffer->mvp.M44 = 1.0f;

                deviceContext->Unmap((ID3D11Resource*)bd->_vertexConstantBuffer.Get(), 0);
            }

            SetupRenderState(drawData);

            int global_idx_offset = 0;
            int global_vtx_offset = 0;
            Vector2 clip_off = drawData.DisplayPos;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr drawList = drawData.CmdLists[n];
                for (int i = 0; i < drawList.CmdBuffer.Size; i++)
                {
                    ImDrawCmdPtr cmd = drawList.CmdBuffer[i];
                    if (cmd.UserCallback != IntPtr.Zero) throw new NotImplementedException("user callbacks not implemented");
                    else
                    {
                        clip_min.X = cmd.ClipRect.X - clip_off.X;
                        clip_min.Y = cmd.ClipRect.Y - clip_off.Y;

                        clip_max.X = cmd.ClipRect.Z - clip_off.X;
                        clip_max.Y = cmd.ClipRect.W - clip_off.Y;

                        if (clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                            continue;

                        rect.Left = (int)clip_min.X;
                        rect.Top = (int)clip_min.Y;
                        rect.Right = (int)clip_max.X;
                        rect.Bottom = (int)clip_max.Y;

                        deviceContext->RSSetScissorRects(1, &rect);
                        ID3D11ShaderResourceView* texture_srv = (ID3D11ShaderResourceView*)cmd.GetTexID();
                        deviceContext->PSSetShaderResources(0, 1, &texture_srv);
                        deviceContext->DrawIndexed(cmd.ElemCount, (uint)(cmd.IdxOffset + global_idx_offset), (int)(cmd.VtxOffset + global_vtx_offset));
                    }
                }

                global_idx_offset += drawList.IdxBuffer.Size;
                global_vtx_offset += drawList.VtxBuffer.Size;
            }


        }
        static unsafe void CreateFontsTexture()
        {
            var io = ImGui.GetIO();
            ImGuiDxData* bd = GetBackendData();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

            Texture2DDescription texDesc = default;
            texDesc.Width = (uint)width;
            texDesc.Height = (uint)height;
            texDesc.MipLevels = 1;
            texDesc.ArraySize = 1;
            texDesc.Format = Format.R8G8B8A8Unorm;
            texDesc.SampleDesc.Count = 1;
            texDesc.Usage = Usage.Default;
            texDesc.BindFlags = BindFlags.ShaderResource;
            texDesc.CPUAccessFlags = (uint)CpuAccessFlags.None;

            SubresourceData subResource = default;
            subResource.pSysMem = pixels;
            subResource.SysMemPitch = texDesc.Width * 4;
            subResource.SysMemSlicePitch = 0;

            ComPtr<ID3D11Texture2D> texture = default;
            if (bd->_device.Get()->CreateTexture2D(&texDesc, &subResource, texture.GetAddressOf()).Failure)
            {
                texture.Dispose();
                Console.WriteLine("CreateTexture2D fail");
                return;
            }

            ShaderResourceViewDescription resViewDesc = default;
            resViewDesc.Format = Format.R8G8B8A8Unorm;
            resViewDesc.ViewDimension = SrvDimension.Texture2D;
            resViewDesc.Texture2D.MipLevels = texDesc.MipLevels;
            resViewDesc.Texture2D.MostDetailedMip = 0;

            if (bd->_device.Get()->CreateShaderResourceView((ID3D11Resource*)texture.Get(), &resViewDesc, bd->_fontTextureView.GetAddressOf()).Failure)
            {
                Console.WriteLine("CreateShaderResourceView fail");
                texture.Dispose();
                return;
            }

            texture.Dispose();
            io.Fonts.SetTexID((nint)bd->_fontTextureView.Get());

            SamplerDescription samplerDesc = default;
            samplerDesc.Filter = Filter.MinMagMipLinear;
            samplerDesc.AddressU = TextureAddressMode.Clamp;
            samplerDesc.AddressV = TextureAddressMode.Clamp;
            samplerDesc.AddressW = TextureAddressMode.Clamp;
            samplerDesc.ComparisonFunc = ComparisonFunction.Always;
            samplerDesc.MaxLOD = 0;
            samplerDesc.MinLOD = 0;
            samplerDesc.MipLODBias = 0;

            if (bd->_device.Get()->CreateSamplerState(&samplerDesc, bd->_fontSampler.GetAddressOf()).Failure)
            {
                bd->_fontSampler.Dispose();
                Console.WriteLine("CreateSamplerState fail");
                return;
            }
        }
        static unsafe bool CreateDeviceObjects()
        {
            ImGuiDxData* bd = GetBackendData();
            if (bd->_device.Get() == null)
                return false;

            if (bd->_fontSampler.Get() != null)
            {
                Console.WriteLine("Invalidating font smapler in ImGui_ImplDX11_CreateDeviceObjects");
                ImGui_ImplDX11_InvalidateDeviceObjects();
            }

            ComPtr<ID3DBlob> byteCodeBlob = default;
            ComPtr<ID3DBlob> errorBlob = default;

            var res = D3DCompile(
                pSrcData: (void*) vertexShaderCode,
                SrcDataSize: (nuint)vertexShaderCodeLength,
                pSourceName: null,
                pDefines: null,
                pInclude: D3D_COMPILE_STANDARD_FILE_INCLUDE,
                pEntrypoint: (byte*)entrypoint.ToPointer(),
                pTarget: (byte*)vsTarget.ToPointer(),
                Flags1: CompileFlags.None,
                Flags2: 0u,
                ppCode: byteCodeBlob.GetAddressOf(),
                ppErrorMsgs: errorBlob.GetAddressOf());

            if (res.Failure) throw new Exception("error compiling vertex shader");

            if (bd->_device.Get()->CreateVertexShader(byteCodeBlob.Get()->GetBufferPointer(), byteCodeBlob.Get()->GetBufferSize(), null, bd->_vertexShader.GetAddressOf()) != HResult.Ok)
            {
                Console.WriteLine("CreateVertexShader fail");
                byteCodeBlob.Dispose();
                errorBlob.Dispose();
                return false;
            }


            var inputElements = stackalloc InputElementDescription[3];

            inputElements[0] = new InputElementDescription
            {
                SemanticName = (byte*)position.ToPointer(),
                SemanticIndex = 0,
                Format = Format.R32G32Float,
                InputSlot = 0,
                AlignedByteOffset = 0,
                InputSlotClass = InputClassification.PerVertexData,
                InstanceDataStepRate = 0
            };

            inputElements[1] = new InputElementDescription
            {
                SemanticName = (byte*)texcoord.ToPointer(),
                SemanticIndex = 0,
                Format = Format.R32G32Float,
                InputSlot = 0,
                AlignedByteOffset = 8,
                InputSlotClass = InputClassification.PerVertexData,
                InstanceDataStepRate = 0
            };

            inputElements[2] = new InputElementDescription
            {
                SemanticName = (byte*)color.ToPointer(),
                SemanticIndex = 0,
                Format = Format.R8G8B8A8Unorm,
                InputSlot = 0,
                AlignedByteOffset = 16,
                InputSlotClass = InputClassification.PerVertexData,
                InstanceDataStepRate = 0
            };

            if (bd->_device.Get()->CreateInputLayout(inputElements, 3,
                byteCodeBlob.Get()->GetBufferPointer(), byteCodeBlob.Get()->GetBufferSize(),
                bd->_inputLayout.GetAddressOf()) != HResult.Ok)
            {
                byteCodeBlob.Dispose();
                return false;
            }

            byteCodeBlob.Dispose();
            errorBlob.Dispose();

            var constBufferDesc = new BufferDescription(
                (uint)Marshal.SizeOf<VERTEX_CONSTANT_BUFFER_DX11>(),
                BindFlags.ConstantBuffer,
                Usage.Dynamic,
                CpuAccessFlags.Write);

            if (bd->_device.Get()->CreateBuffer(&constBufferDesc, null, bd->_vertexConstantBuffer.GetAddressOf()) != HResult.Ok)
            {
                Console.WriteLine("CreateBuffer constBufferDesc fail");
                return false;
            };

            res = D3DCompile(
              pSrcData: pixelShaderCode.ToPointer(),
              SrcDataSize: (nuint)pixelShaderCodeLength,
              pSourceName: null,
              pDefines: null,
              pInclude: D3D_COMPILE_STANDARD_FILE_INCLUDE,
              pEntrypoint: (byte*)entrypoint.ToPointer(),
              pTarget: (byte*)idxTarget.ToPointer(),
              Flags1: CompileFlags.None,
              Flags2: 0u,
              ppCode: byteCodeBlob.GetAddressOf(),
              ppErrorMsgs: errorBlob.GetAddressOf());

            if (res.Failure) throw new Exception("error compiling pixel shader");

            if (bd->_device.Get()->CreatePixelShader(byteCodeBlob.Get()->GetBufferPointer(), byteCodeBlob.Get()->GetBufferSize(), null, bd->_pixelShader.GetAddressOf()) != HResult.Ok)
            {
                Console.WriteLine("CreatePixelShader fail");
                byteCodeBlob.Dispose();
                errorBlob.Dispose();
                return false;
            }
            byteCodeBlob.Dispose();
            errorBlob.Dispose();
            var blendDesc = new BlendDescription
            {
                AlphaToCoverageEnable = false
            };

            blendDesc.RenderTarget[0] = new()
            {
                BlendOpAlpha = BlendOperation.Add,
                BlendEnable = true,
                BlendOp = BlendOperation.Add,
                DestBlendAlpha = Blend.InverseSrcAlpha,
                DestBlend = Blend.InverseSrcAlpha,
                SrcBlend = Blend.SrcAlpha,
                SrcBlendAlpha = Blend.One,
                RenderTargetWriteMask = ColorWriteEnable.All
            };

            bd->_device.Get()->CreateBlendState(&blendDesc, bd->_blendState.GetAddressOf());

            var rasterDesc = new RasterizerDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                ScissorEnable = true,
                DepthClipEnable = true,
            };

            bd->_device.Get()->CreateRasterizerState(&rasterDesc, bd->_rasterizerState.GetAddressOf());

            var stencilOpDesc = new DepthStencilOperationDescription(StencilOperation.Keep, StencilOperation.Keep, StencilOperation.Keep, ComparisonFunction.Always);
            var depthDesc = new DepthStencilDescription
            {
                DepthEnable = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthFunc = ComparisonFunction.Always,
                StencilEnable = false,
                FrontFace = stencilOpDesc,
                BackFace = stencilOpDesc
            };

            bd->_device.Get()->CreateDepthStencilState(&depthDesc, bd->_depthStencilState.GetAddressOf());

            CreateFontsTexture();
            return true;

        }
        static unsafe void ImGui_ImplDX11_InvalidateDeviceObjects()
        {
            ImGuiDxData* bd = GetBackendData();
            if (bd->_device.Get() == null)
                return;

            bd->_fontSampler.Dispose();
            bd->_fontTextureView.Dispose();
            ImGui.GetIO().Fonts.SetTexID(0);
            bd->_indexBuffer.Dispose();
            bd->_vertexBuffer.Dispose();
            bd->_blendState.Dispose();
            bd->_depthStencilState.Dispose();
            bd->_rasterizerState.Dispose();
            bd->_pixelShader.Dispose();
            bd->_vertexConstantBuffer.Dispose();
            bd->_inputLayout.Dispose();
            bd->_vertexShader.Dispose();


        }
        public static unsafe bool InitDxController(ID3D11Device* device, ID3D11DeviceContext* device_context)
        {
            var io = ImGui.GetIO();
            io.BackendRendererUserData = imguiDxData;
            ImGuiDxData* bd = (ImGuiDxData*)imguiDxData.ToPointer();

            bd->VertexBufferSize = 5000;
            bd->IndexBufferSize = 10000;

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            ComPtr<IDXGIDevice> pDXGIDevice = default;
            ComPtr<IDXGIAdapter> pDXGIAdapter = default;
            ComPtr<IDXGIFactory> pFactory = default;

            if (device->QueryInterface(__uuidof<IDXGIDevice>(), (void**)pDXGIDevice.GetAddressOf()) == 0)
            {
                if (pDXGIDevice.Get()->GetParent(__uuidof<IDXGIAdapter>(), (void**)pDXGIAdapter.GetAddressOf()) == 0)
                {
                    if (pDXGIAdapter.Get()->GetParent(__uuidof<IDXGIFactory>(), (void**)pFactory.GetAddressOf()) == 0)
                    {
                        bd->_device = device;
                        bd->_deviceContext = device_context;
                        bd->_factory = pFactory;
                    }
                }

                if (pDXGIAdapter.Get() != null)
                {
                    pDXGIAdapter.Dispose();
                }
            }

            if (pDXGIDevice.Get() != null)
            {
                pDXGIDevice.Dispose();
            }

            bd->_device.Get()->AddRef();
            bd->_deviceContext.Get()->AddRef();

            return true;

        }

        public static unsafe void DxShutdown()
        {
            ImGuiDxData* bd = GetBackendData();
            ImGui_ImplDX11_InvalidateDeviceObjects();
            if (bd->_factory.Get() != null) { bd->_factory.Dispose(); }
            if (bd->_device.Get() != null) { bd->_device.Dispose(); }
            if (bd->_deviceContext.Get() != null) { bd->_deviceContext.Dispose(); }
            CleanupShaderProps();
            Marshal.FreeHGlobal(imguiDxData);
            imguiDxData = IntPtr.Zero;
            ImGui.GetIO().BackendFlags &= ~ImGuiBackendFlags.RendererHasVtxOffset;
        }

        public static unsafe void DxNewFrame()
        {
            ImGuiDxData* bd = GetBackendData();
            if (bd->_fontSampler.Get() == null)
            {
                Console.WriteLine("Creating device object");
                CreateDeviceObjects();
            }
        }

        public static unsafe bool CreateDeviceD3D(IntPtr hWnd)
        {
            var sd = new SwapChainDescription
            {
                BufferCount = 3,
                BufferDesc = new ModeDescription
                {
                    Width = (uint)0,
                    Height = (uint)0,
                    Format = Format.R8G8B8A8Unorm,
                    RefreshRate = new Rational(60, 1),
                    Scaling = ModeScaling.Unspecified
                },
                Flags = SwapChainFlags.AllowModeSwitch,
                BufferUsage = Win32.Graphics.Dxgi.Usage.RenderTargetOutput,
                OutputWindow = hWnd,
                SampleDesc = new SampleDescription(1, 0),
                Windowed = true,
                SwapEffect = SwapEffect.FlipSequential
            };

            CreateDeviceFlags creationFlags = CreateDeviceFlags.BgraSupport;
            ReadOnlySpan<FeatureLevel> featureLevels =
                [
                        FeatureLevel.Level_11_1,
                        FeatureLevel.Level_11_0,
                        FeatureLevel.Level_10_1

                ];

            FeatureLevel featureLevel = 0;

            var res = D3D11CreateDeviceAndSwapChain(
                null,
                DriverType.Hardware,
                IntPtr.Zero,
                creationFlags,
                featureLevels.GetPointer(),
                (uint)featureLevels.Length,
                D3D11_SDK_VERSION,
                &sd,
                SwapChain.GetAddressOf(),
                Device.GetAddressOf(),
                &featureLevel,
                DeviceContext.GetAddressOf()
            );

            if (res.Failure)
            {
                Console.WriteLine("Unable to initialize DirectX device and swap chain.");
                return false;
            }

            ComPtr<ID3D11Texture2D> pBackBuffer = default;
            SwapChain.Get()->GetBuffer(0, __uuidof<ID3D11Texture2D>(), pBackBuffer.GetVoidAddressOf());
            Device.Get()->CreateRenderTargetView((ID3D11Resource*)pBackBuffer.Get(), null, RenderTargetView.GetAddressOf());
            pBackBuffer.Dispose();
            return true;
        }

        public unsafe static void CleanupDeviceD3D()
        {
            CleanupRenderTarget();
            SwapChain.Dispose();
            DeviceContext.Dispose();
            Device.Dispose();
        }

        public static unsafe void CreateRenderTarget(uint g_ResizeWidth, uint g_ResizeHeight)
        {
            ComPtr<ID3D11Texture2D> pBackBuffer = default;
            SwapChain.Get()->ResizeBuffers(0, g_ResizeWidth, g_ResizeHeight, Format.Unknown, 0);
            SwapChain.Get()->GetBuffer(0, __uuidof<ID3D11Texture2D>(), pBackBuffer.GetVoidAddressOf());
            Device.Get()->CreateRenderTargetView((ID3D11Resource*)pBackBuffer.Get(), null, RenderTargetView.GetAddressOf());
            pBackBuffer.Dispose();
        }

        public static unsafe void CleanupRenderTarget()
        {
            RenderTargetView.Dispose();
        }
    }
}