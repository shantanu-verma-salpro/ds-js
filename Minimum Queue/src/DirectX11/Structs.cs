using System.Numerics;
using System.Runtime.InteropServices;
using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi;

namespace ImGuiNET
{
    public static class DxStucts
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ImGuiDxData
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

        };

        [StructLayout(LayoutKind.Sequential)]
        public struct VERTEX_CONSTANT_BUFFER_DX11
        {
            public Matrix4x4 mvp;
        }
    }
}