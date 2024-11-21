using System.Text;
using Win32.Graphics.Direct3D.Dxc;
using static ImGuiNET.Dx11;
using Vortice.D3DCompiler;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace ImGuiNET;
class Program
{

    public static unsafe void Main(string[] args)
    {
        ImGui.CreateContext();
        var hInstance = Win32ImGui.NativeMethods.GetModuleHandleW(null);
        Win32ImGui.Utils.CreateAndRegisterWindowClass("hello", hInstance);
        var hwnd = Win32ImGui.Utils.CreateWindow("hello", hInstance);
        Win32ImGui.NativeMethods.ShowWindow(hwnd, (int)Win32ImGui.SW.ShowDefault);
        Win32ImGui.NativeMethods.UpdateWindow(hwnd);

        ImGui.CreateContext();
        ImGuiIOPtr io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        
        Win32ImGui.ImGui_ImplWin32_Init(hwnd);

        bool done = false;
        while (!done)
        {


            while (Win32ImGui.NativeMethods.PeekMessage(out Win32ImGui.WindowMessage msg, IntPtr.Zero, 0U, 0U, 0x0001))
            {
                Win32ImGui.NativeMethods.TranslateMessage(ref msg);
                Win32ImGui.NativeMethods.DispatchMessageW(ref msg);
                if (msg.message == 0x0012)
                    done = true;
            }
            if (done)
                break;

            Win32ImGui.ImGui_ImplWin32_NewFrame();
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
       

        }

    }
}
