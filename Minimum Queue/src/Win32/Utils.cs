using System;
using System.Runtime.InteropServices;
using static ImGuiNET.WinConstants;
using static ImGuiNET.Pinvoke;
using static ImGuiNET.WinStructs;
using static ImGuiNET.WinResources;
using System.Text;

namespace ImGuiNET
{
    static class WinUtils
    {
        public unsafe static void InitWindowClass(WindowClassStyles style, IntPtr proc)
        {
            WNDCLASSEX* cls = (WNDCLASSEX*)windowClass;
            cls->cbSize = WNDCLASSEX.Size;
            cls->style = (uint)style;
            cls->lpfnWndProc = proc;
            cls->cbClsExtra = 0;
            cls->cbWndExtra = 0;
            cls->hInstance = hInstance;
            cls->hCursor = HandCursorPtr;
            cls->hIcon = IntPtr.Zero;
            cls->hbrBackground = IntPtr.Zero;
            cls->lpszMenuName = IntPtr.Zero;
            cls->lpszClassName = ClassNamePtr;
        }

        public unsafe static bool RegisterClass()
        {
            ushort classAtom = (ushort)RegisterClassEx(windowClass);
            if (classAtom == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != (int)Errors.ERROR_CLASS_ALREADY_EXISTS)
                {
                    Console.WriteLine($"Failed to register window class. Error code: {errorCode}");
                    ReleaseAllocHGlobal(ClassNamePtr);
                    ReleaseAllocHGlobal(windowClass);
                    return false;
                }
            }
            return true;
        }

        public static int GetCurrentKeyboardCodePage()
        {
            return LangIdToCodePageMap.TryGetValue((ushort)(GetKeyboardLayout(0).ToInt64() & 0xFFFF), out var codePage) ? codePage : 0;
        }

        public static bool IsKeyPressed(int vKey)
        {
            return (GetKeyState(vKey) & 0x8000) != 0;
        }

        public static string MultiByteToWC(ReadOnlySpan<byte> multiByteStr, int codePage)
        {
            return Encoding.GetEncoding(codePage).GetString(multiByteStr);
        }

        public static void UnregisterClass()
        {
            UnregisterClassW(ClassNamePtr, hInstance);
            ReleaseAllocHGlobal(windowClass);
            ReleaseAllocHGlobal(ClassNamePtr);
        }
    }
}
