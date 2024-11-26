using static ImGuiNET.WinConstants;
using static ImGuiNET.Pinvoke;
using static ImGuiNET.WinUtils;
using static ImGuiNET.WinResources;

namespace ImGuiNET
{
    public static class Window
    {
        public unsafe static bool CreateWindow(IntPtr title, int width, int height, IntPtr proc)
        {
            InitWindowClass(WindowClassStyles.CS_CLASSDC, proc);
            if (!RegisterClass()) return false;

            windowHandle = CreateWindowEx(
                (uint)WindowStylesEx.WS_EX_APPWINDOW,
                ClassNamePtr,
                title,
                (uint)WindowStyles.WS_OVERLAPPEDWINDOW,
                100, 100, width, height,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero
            );

            if (windowHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create Window.");
                return false;
            }
            return true;
        }
    }
}