using System.Text;
using Win32.Graphics.Direct3D.Dxc;
using static ImGuiNET.Dx11;
using Vortice.D3DCompiler;

namespace ImGuiNET;
class Program
{
    public static unsafe void Main(string[] args)
    {
        // Create an instance of the DXC Compiler


        // Shader source code
        string shaderSource = @"cbuffer vertexBuffer : register(b0)

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

        // Convert the shader source into a UTF-8 byte array
        var res = Compiler.Compile(shaderSource, "main", "vs", "vs_4_0", out Vortice.Direct3D.Blob vertexShaderBlob, out Vortice.Direct3D.Blob error);

        if (res.Failure || vertexShaderBlob == null)
                throw new Exception("error compiling vertex shader");

    }
}
