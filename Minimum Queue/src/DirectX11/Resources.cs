using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi;
using static ImGuiNET.DxStucts;

namespace ImGuiNET
{
    public static class DxResources
    {
        public static ComPtr<ID3D11Device> Device;
        public static ComPtr<ID3D11DeviceContext> DeviceContext;
        public static ComPtr<IDXGISwapChain> SwapChain;
        public static ComPtr<ID3D11RenderTargetView> RenderTargetView;
        public static  IntPtr imguiDxData = Marshal.AllocHGlobal((int)Marshal.SizeOf<ImGuiDxData>());
        unsafe static DxResources(){
            Unsafe.InitBlockUnaligned((byte*)imguiDxData.ToPointer(),0, (uint)Marshal.SizeOf<ImGuiDxData>());
        }
        public static readonly unsafe IntPtr vertexShaderCode = Marshal.StringToHGlobalAuto(@"cbuffer vertexBuffer : register(b0)
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
                ");
        
         public static readonly int vertexShaderCodeLength = Encoding.Unicode.GetByteCount(@"cbuffer vertexBuffer : register(b0)
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
            ");
       
        public static readonly unsafe IntPtr vsTarget = Marshal.StringToHGlobalAuto("vs_4_0");
        public static readonly unsafe IntPtr idxTarget = Marshal.StringToHGlobalAuto("ps_4_0");
        public static readonly unsafe IntPtr pixelShaderCode = Marshal.StringToHGlobalAuto(
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
                float4 out_col = input.col * texture0.Sample(sampler0, input.uv);
                return out_col;
            }");
        
        public static readonly int pixelShaderCodeLength = Encoding.Unicode.GetByteCount(@"struct PS_INPUT
            {
                float4 pos : SV_POSITION;
                float4 col : COLOR0;
                float2 uv  : TEXCOORD0;
            };
            sampler sampler0;
            Texture2D texture0;

            float4 main(PS_INPUT input) : SV_Target
            {
                float4 out_col = input.col * texture0.Sample(sampler0, input.uv);
                return out_col;
            }");

        public static readonly unsafe IntPtr entrypoint = Marshal.StringToHGlobalAuto("main");
        public static readonly unsafe IntPtr position = Marshal.StringToHGlobalAuto("POSITION");
        public static readonly unsafe IntPtr texcoord = Marshal.StringToHGlobalAuto("TEXCOORD");
        public static readonly unsafe IntPtr color = Marshal.StringToHGlobalAuto("COLOR");
        public static unsafe void ReleaseComPtr<T>(ref ComPtr<T> resource) where T : unmanaged
        {
            if (resource.Get() != null)
            {
                resource.Dispose();
            }
        }

        public static unsafe void CleanupShaderProps()
        {
            Marshal.FreeHGlobal((nint)position);
            Marshal.FreeHGlobal((nint)texcoord);
            Marshal.FreeHGlobal((nint)color);
            Marshal.FreeHGlobal((nint)vertexShaderCode);
            Marshal.FreeHGlobal((nint)pixelShaderCode);
            Marshal.FreeHGlobal((nint)vsTarget);
            Marshal.FreeHGlobal((nint)idxTarget);
            Marshal.FreeHGlobal((nint)entrypoint);
        }

        public static void DisposeDxResouces()
        {
            ReleaseComPtr(ref RenderTargetView);
            ReleaseComPtr(ref SwapChain);
            ReleaseComPtr(ref DeviceContext);
            ReleaseComPtr(ref Device);

            if (imguiDxData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(imguiDxData);
            }
        }
    }
}