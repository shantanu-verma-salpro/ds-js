/*using System.Text;
using System.Runtime.CompilerServices;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Numerics;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using System.Reflection.Metadata;

namespace ImGuiNET
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
        public ImGui_ImplDX11_Data() {  }
    };

    class Dx11GUI
    {
        static unsafe ImGui_ImplDX11_Data* ImGui_ImplDX11_GetBackendData()
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero) return null;
            return (ImGui_ImplDX11_Data*)ImGui.GetIO().BackendRendererUserData;
        }

        static unsafe void ImGui_ImplDX11_SetupRenderState(ref ImDrawData draw_data, ComPtr<ID3D11DeviceContext> ctx)
        {
            ImGui_ImplDX11_Data* bd = ImGui_ImplDX11_GetBackendData();
            var viewport = new Viewport(0, 0, draw_data.DisplaySize.X, draw_data.DisplaySize.Y, 0, 1);

            ctx.RSSetViewports(1, &viewport);

            uint stride = (uint)sizeof(ImDrawVert);
            uint offset = 0;

            ctx.IASetInputLayout(bd->_inputLayout);
            ctx.IASetVertexBuffers(0, 1, bd->_vertexBuffer, &stride, &offset);
            ctx.IASetIndexBuffer(bd->_indexBuffer, sizeof(ushort) == 2 ? Format.FormatR16Uint : Format.FormatR32Uint, 0);
            ctx.IASetPrimitiveTopology(D3DPrimitiveTopology.D3DPrimitiveTopologyTrianglelist);
            ctx.VSSetShader(bd->_vertexShader, null, 0);
            ctx.VSSetConstantBuffers(0, 1, bd->_vertexConstantBuffer);
            ctx.PSSetShader(bd->_pixelShader, null, 0);
            ctx.PSSetSamplers(0, 1, bd->_fontSampler);
            ctx.GSSetShader((ID3D11GeometryShader*)null, null, 0);
            ctx.HSSetShader((ID3D11HullShader*)null, null, 0); 
            ctx.DSSetShader((ID3D11DomainShader*)null, null, 0); 
            ctx.CSSetShader((ID3D11ComputeShader*)null, null, 0);

            Vector4 blend_factor = Vector4.Zero;
            ctx.OMSetBlendState(bd->_blendState, (float*)&blend_factor, 0xffffffff);
            ctx.OMSetDepthStencilState(bd->_depthStencilState, 0);
            ctx.RSSetState(bd->_rasterizerState);
        }

        public unsafe static void ImGui_ImplDX11_RenderDrawData(ref ImDrawData draw_data)
        {
            if (draw_data.DisplaySize.X <= 0.0f || draw_data.DisplaySize.Y <= 0.0f)
                return;

            ImGui_ImplDX11_Data* bd = ImGui_ImplDX11_GetBackendData();
        
            if (bd->_vertexBuffer.Handle == null || bd->VertexBufferSize < draw_data.TotalVtxCount)
            {
                if (bd->_vertexBuffer.Handle != null) { bd->_vertexBuffer.Release(); bd->_vertexBuffer.Handle = null; }
                bd->VertexBufferSize = draw_data.TotalVtxCount + 5000;
                BufferDesc desc = new()
                {
                    Usage = Usage.Dynamic,
                    ByteWidth = (uint)(bd->VertexBufferSize * Marshal.SizeOf(typeof(ImDrawVert))),
                    BindFlags = (uint)BindFlag.VertexBuffer,
                    CPUAccessFlags = (uint)CpuAccessFlag.Write,
                    MiscFlags = 0
                };
                if (bd->_device.CreateBuffer(in desc, null, ref bd->_vertexBuffer) < 0)
                    return;
            }

            if (bd->_indexBuffer.Handle == null || bd->IndexBufferSize < draw_data.TotalIdxCount)
            {
                if (bd->_indexBuffer.Handle != null) { bd->_indexBuffer.Release(); bd->_indexBuffer.Handle = null; }
                bd->VertexBufferSize = draw_data.TotalVtxCount + 10000;
                BufferDesc desc = new()
                {
                    Usage = Usage.Dynamic,
                    ByteWidth = (uint)(bd->IndexBufferSize * Marshal.SizeOf(typeof(ushort))),
                    BindFlags = (uint)BindFlag.IndexBuffer,
                    CPUAccessFlags = (uint)CpuAccessFlag.Write,
                    MiscFlags = 0
                };
                if (bd->_device.CreateBuffer(in desc, null, ref bd->_indexBuffer) < 0)
                    return;
            }

            MappedSubresource vtxResource = new();
            if (bd->_deviceContext.Map(bd->_vertexBuffer, 0, Map.WriteDiscard, 0, &vtxResource) != HRESULT.S_OK)
                return;

            MappedSubresource idxResource = new();
            if (bd->_deviceContext.Map(bd->_indexBuffer, 0, Map.WriteDiscard, 0, &idxResource) != HRESULT.S_OK)
            {
                return;
            }

            ImDrawVert* vtxDst = (ImDrawVert*)vtxResource.PData;
            ushort* idxDst = (ushort*)idxResource.PData;

            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawList* drawList = draw_data.CmdLists[n];

                Unsafe.CopyBlock(vtxDst, drawList->VtxBuffer.Data.ToPointer(), (uint)(drawList->VtxBuffer.Size * sizeof(ImDrawVert)));
                vtxDst += drawList->VtxBuffer.Size;

                Unsafe.CopyBlock(idxDst, drawList->IdxBuffer.Data.ToPointer(), (uint)(drawList->IdxBuffer.Size * sizeof(ushort)));
                idxDst += drawList->IdxBuffer.Size;
            }

            deviceCtx.Unmap(bd._vertexBuffer, 0);
            deviceCtx.Unmap(bd._indexBuffer, 0);

            {
                MappedSubresource mappedResource = new();
                if (deviceCtx.Map(bd._vertexConstantBuffer, 0, Map.WriteDiscard, 0, &mappedResource) != HRESULT.S_OK)
                    return;

                VertexConstantBufferDX11* constantBuffer = (VertexConstantBufferDX11*)mappedResource.PData;

                float L = draw_data.DisplayPos.X;
                float R = draw_data.DisplayPos.X + draw_data.DisplaySize.X;
                float T = draw_data.DisplayPos.Y;
                float B = draw_data.DisplayPos.Y + draw_data.DisplaySize.Y;

                float[,] mvp =
                {
                    { 2.0f / (R - L), 0.0f,            0.0f,       0.0f },
                    { 0.0f,           2.0f / (T - B),  0.0f,       0.0f },
                    { 0.0f,           0.0f,            0.5f,       0.0f },
                    { (R + L) / (L - R), (T + B) / (B - T), 0.5f,   1.0f }
                };

                fixed (float* mvpPtr = &mvp[0, 0])
                {
                    Unsafe.CopyBlock(constantBuffer->Mvp, mvpPtr, (uint)(sizeof(float) * 16));
                }

                deviceCtx.Unmap(bd._vertexConstantBuffer, 0);
            }

        }

    }
}
*/