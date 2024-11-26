using static ImGuiNET.WinConstants;
using static ImGuiNET.WinStructs;
using static ImGuiNET.Pinvoke;
using static ImGuiNET.WinUtils;
using static ImGuiNET.WinResources;
using static ImGuiNET.WinMacros;

namespace ImGuiNET
{
    class WinController
    {
        public unsafe static ImGuiData* GetBackendData()
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero) return null;
            return (ImGuiData*)ImGui.GetIO().BackendPlatformUserData.ToPointer();
        }

        public unsafe static ImGuiData* GetBackendData(ImGuiIOPtr io)
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero) return null;
            return (ImGuiData*)io.BackendPlatformUserData.ToPointer();
        }
        public static void SetImGuiCursor(ImGuiMouseCursor imguiCursor)
        {
            SystemCursors cursorId = imguiCursor switch
            {
                ImGuiMouseCursor.Arrow => SystemCursors.IDC_ARROW,
                ImGuiMouseCursor.TextInput => SystemCursors.IDC_IBEAM,
                ImGuiMouseCursor.ResizeAll => SystemCursors.IDC_SIZEALL,
                ImGuiMouseCursor.ResizeEW => SystemCursors.IDC_SIZEWE,
                ImGuiMouseCursor.ResizeNS => SystemCursors.IDC_SIZENS,
                ImGuiMouseCursor.ResizeNESW => SystemCursors.IDC_SIZENESW,
                ImGuiMouseCursor.ResizeNWSE => SystemCursors.IDC_SIZENWSE,
                ImGuiMouseCursor.Hand => SystemCursors.IDC_HAND,
                ImGuiMouseCursor.NotAllowed => SystemCursors.IDC_NO,
                _ => SystemCursors.IDC_ARROW,
            };

            SetCursor(LoadCursor(IntPtr.Zero, (int)cursorId));
        }
        public static unsafe void UpdateKeyboardCodePage(ImGuiIOPtr io) => GetBackendData(io)->KeyboardCodePage = (uint)GetCurrentKeyboardCodePage();
        public static unsafe bool InitWin32Controller(bool platform_has_own_dc = false)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            System.Diagnostics.Debug.Assert(io.NativePtr->BackendPlatformUserData == null, "Already initialized a platform backend!");
            if (!QueryPerformanceFrequency(out long perf_frequency) || !QueryPerformanceCounter(out long perf_counter))
                return false;

            ImGuiData* bd = (ImGuiData*)imguiData;

            try
            {
                bd->hWnd = windowHandle;
                bd->TicksPerSecond = perf_frequency;
                bd->Time = perf_counter;
                bd->LastMouseCursor = (int)ImGuiMouseCursor.COUNT;

                io.NativePtr->BackendPlatformUserData = bd;
                io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors | ImGuiBackendFlags.HasSetMousePos;
                var mainViewport = ImGui.GetMainViewport();
                mainViewport.PlatformHandle = mainViewport.PlatformHandleRaw = windowHandle;
                UpdateKeyboardCodePage(io);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during initialization: {ex.Message}");
                ReleaseAllocHGlobal(imguiData);
                return false;
            }
        }
        public unsafe static void Shutdown()
        {
            var bd = GetBackendData();
            System.Diagnostics.Debug.Assert(bd != null, "No platform backend to shutdown");
            ImGuiIOPtr io = ImGui.GetIO();
            if (io.BackendPlatformUserData != IntPtr.Zero)
            {
                ReleaseAllocHGlobal(imguiData);
                io.BackendPlatformUserData = IntPtr.Zero;
                io.BackendFlags &= ~(ImGuiBackendFlags.HasMouseCursors | ImGuiBackendFlags.HasSetMousePos | ImGuiBackendFlags.HasGamepad);
            }
        }
        public static bool UpdateMouseCursor(ImGuiIOPtr io, ImGuiMouseCursor imguiCursor)
        {
            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
                return false;

            if (imguiCursor == ImGuiMouseCursor.None || io.MouseDrawCursor) SetCursor(IntPtr.Zero);
            else SetImGuiCursor(imguiCursor);

            return true;
        }
        static void AddKeyEvent(ImGuiIOPtr io, ImGuiKey key, bool down, int native_keycode, int native_scancode = -1)
        {
            io.AddKeyEvent(key, down);
            io.SetKeyEventNativeData(key, native_keycode, native_scancode);
            _ = native_scancode;
        }

        static void ProcessKeyEventsWorkarounds(ImGuiIOPtr io)
        {
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift) && !IsKeyPressed((int)VK.VK_LSHIFT))
                AddKeyEvent(io, ImGuiKey.LeftShift, false, (int)VK.VK_LSHIFT);
            if (ImGui.IsKeyDown(ImGuiKey.RightShift) && !IsKeyPressed((int)VK.VK_RSHIFT))
                AddKeyEvent(io, ImGuiKey.RightShift, false, (int)VK.VK_RSHIFT);

            if (ImGui.IsKeyDown(ImGuiKey.LeftSuper) && !IsKeyPressed((int)VK.VK_LWIN))
                AddKeyEvent(io, ImGuiKey.LeftSuper, false, (int)VK.VK_LWIN);
            if (ImGui.IsKeyDown(ImGuiKey.RightSuper) && !IsKeyPressed((int)VK.VK_RWIN))
                AddKeyEvent(io, ImGuiKey.RightSuper, false, (int)VK.VK_RWIN);
        }

        static void UpdateKeyModifiers(ImGuiIOPtr io)
        {
            io.AddKeyEvent(ImGuiKey.ModCtrl, IsKeyPressed((int)VK.VK_CONTROL));
            io.AddKeyEvent(ImGuiKey.ModShift, IsKeyPressed((int)VK.VK_SHIFT));
            io.AddKeyEvent(ImGuiKey.ModAlt, IsKeyPressed((int)VK.VK_MENU));
            io.AddKeyEvent(ImGuiKey.ModSuper, IsKeyPressed((int)VK.VK_LWIN) || IsKeyPressed((int)VK.VK_RWIN));
        }
        static unsafe void UpdateMouseData(ImGuiIOPtr io)
        {
            var bd = GetBackendData();
            System.Diagnostics.Debug.Assert(bd->hWnd != IntPtr.Zero, "No platform backend to shutdown, or already shutdown?");

            IntPtr focusedWindow = GetForegroundWindow();
            bool isAppFocused = focusedWindow == bd->hWnd;

            if (isAppFocused)
            {
                if (io.WantSetMousePos)
                {
                    POINT point;
                    point.X = (int)io.MousePos.X;
                    point.Y = (int)io.MousePos.Y;
                    if (ClientToScreen(bd->hWnd, ref point))
                    {
                        SetCursorPos(point.X, point.Y);
                    }
                }

                if (!io.WantSetMousePos && bd->MouseTrackedArea == 0)
                {
                    if (GetCursorPos(out POINT point) && ScreenToClient(bd->hWnd, ref point))
                    {
                        io.AddMousePosEvent((float)point.X, (float)point.Y);
                    }
                }
            }
        }
        public unsafe static void WindowNewFrame()
        {
            var bd = GetBackendData();
            System.Diagnostics.Debug.Assert(bd->hWnd != IntPtr.Zero, "Context or backend not initialized? Did you call ImGui_ImplWin32_Init()?");

            ImGuiIOPtr io = ImGui.GetIO();
            GetClientRect(bd->hWnd, out RECT rect1);
            io.DisplaySize.X = rect1.Right - rect1.Left;
            io.DisplaySize.Y = rect1.Bottom - rect1.Top;

            QueryPerformanceCounter(out long current_time);
            io.DeltaTime = (float)(current_time - bd->Time) / bd->TicksPerSecond;
            bd->Time = current_time;

            UpdateMouseData(io);
            ProcessKeyEventsWorkarounds(io);

            ImGuiMouseCursor mouse_cursor = io.MouseDrawCursor ? ImGuiMouseCursor.None : ImGui.GetMouseCursor();
            if (bd->LastMouseCursor != (int)mouse_cursor)
            {
                bd->LastMouseCursor = (int)mouse_cursor;
                UpdateMouseCursor(io, mouse_cursor);
            }
        }

        public unsafe static ImGuiKey KeyEventToImGuiKey(UIntPtr wParam, IntPtr lParam)
        {
            var wp = (VK)wParam.ToUInt32();
            if (wp == VK.VK_RETURN && (HIWORD(lParam) & 0x0100) != 0) return ImGuiKey.KeypadEnter;

            return wp switch

            {
                VK.VK_TAB => ImGuiKey.Tab,
                VK.VK_LEFT => ImGuiKey.LeftArrow,
                VK.VK_RIGHT => ImGuiKey.RightArrow,
                VK.VK_UP => ImGuiKey.UpArrow,
                VK.VK_DOWN => ImGuiKey.DownArrow,
                VK.VK_HOME => ImGuiKey.Home,
                VK.VK_END => ImGuiKey.End,
                VK.VK_INSERT => ImGuiKey.Insert,
                VK.VK_DELETE => ImGuiKey.Delete,
                VK.VK_BACK => ImGuiKey.Backspace,
                VK.VK_SPACE => ImGuiKey.Space,
                VK.VK_RETURN => ImGuiKey.Enter,
                VK.VK_ESCAPE => ImGuiKey.Escape,
                VK.VK_OEM_7 => ImGuiKey.Apostrophe,
                VK.VK_OEM_COMMA => ImGuiKey.Comma,
                VK.VK_OEM_MINUS => ImGuiKey.Minus,
                VK.VK_OEM_PERIOD => ImGuiKey.Period,
                VK.VK_OEM_2 => ImGuiKey.Slash,
                VK.VK_OEM_1 => ImGuiKey.Semicolon,
                VK.VK_OEM_PLUS => ImGuiKey.Equal,
                VK.VK_OEM_4 => ImGuiKey.LeftBracket,
                VK.VK_OEM_5 => ImGuiKey.Backslash,
                VK.VK_OEM_6 => ImGuiKey.RightBracket,
                VK.VK_OEM_3 => ImGuiKey.GraveAccent,
                VK.VK_CAPITAL => ImGuiKey.CapsLock,
                VK.VK_SCROLL => ImGuiKey.ScrollLock,
                VK.VK_PAUSE => ImGuiKey.Pause,
                VK.VK_0 => ImGuiKey._0,
                VK.VK_1 => ImGuiKey._1,
                VK.VK_2 => ImGuiKey._2,
                VK.VK_3 => ImGuiKey._3,
                VK.VK_4 => ImGuiKey._4,
                VK.VK_5 => ImGuiKey._5,
                VK.VK_6 => ImGuiKey._6,
                VK.VK_7 => ImGuiKey._7,
                VK.VK_8 => ImGuiKey._8,
                VK.VK_9 => ImGuiKey._9,
                VK.VK_F1 => ImGuiKey.F1,
                VK.VK_F2 => ImGuiKey.F2,
                VK.VK_F3 => ImGuiKey.F3,
                VK.VK_F4 => ImGuiKey.F4,
                VK.VK_F5 => ImGuiKey.F5,
                VK.VK_F6 => ImGuiKey.F6,
                VK.VK_F7 => ImGuiKey.F7,
                VK.VK_F8 => ImGuiKey.F8,
                VK.VK_F9 => ImGuiKey.F9,
                VK.VK_F10 => ImGuiKey.F10,
                VK.VK_F11 => ImGuiKey.F11,
                VK.VK_F12 => ImGuiKey.F12,
                VK.VK_A => ImGuiKey.A,
                VK.VK_B => ImGuiKey.B,
                VK.VK_C => ImGuiKey.C,
                VK.VK_D => ImGuiKey.D,
                VK.VK_E => ImGuiKey.E,
                VK.VK_F => ImGuiKey.F,
                VK.VK_G => ImGuiKey.G,
                VK.VK_H => ImGuiKey.H,
                VK.VK_I => ImGuiKey.I,
                VK.VK_J => ImGuiKey.J,
                VK.VK_K => ImGuiKey.K,
                VK.VK_L => ImGuiKey.L,
                VK.VK_M => ImGuiKey.M,
                VK.VK_N => ImGuiKey.N,
                VK.VK_O => ImGuiKey.O,
                VK.VK_P => ImGuiKey.P,
                VK.VK_Q => ImGuiKey.Q,
                VK.VK_R => ImGuiKey.R,
                VK.VK_S => ImGuiKey.S,
                VK.VK_T => ImGuiKey.T,
                VK.VK_U => ImGuiKey.U,
                VK.VK_V => ImGuiKey.V,
                VK.VK_W => ImGuiKey.W,
                VK.VK_X => ImGuiKey.X,
                VK.VK_Y => ImGuiKey.Y,
                VK.VK_Z => ImGuiKey.Z,

                _ => ImGuiKey.None,
            };
        }
        static public ImGuiMouseSource GetMouseSourceFromMessageExtraInfo()
        {
            IntPtr extra_info = GetMessageExtraInfo();
            long extraInfoValue = extra_info.ToInt64();

            if ((extraInfoValue & 0xFFFFFF80) == 0xFF515700)
                return ImGuiMouseSource.Pen;

            if ((extraInfoValue & 0xFFFFFF80) == 0xFF515780)
                return ImGuiMouseSource.TouchScreen;

            return ImGuiMouseSource.Mouse;
        }
        public static int HandleInputs(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                return 0;
            return WndProcHandlerEx(hwnd, msg, wParam, lParam, ImGui.GetIO());
        }

        public unsafe static int WndProcHandlerEx(IntPtr hwnd, uint _msg, UIntPtr wParam, IntPtr lParam, ImGuiIOPtr io)
        {
            TRACKMOUSEEVENT tmeCancel = default;
            TRACKMOUSEEVENT tmeTrack = default;

            WM msg = (WM)_msg;
            if (io.BackendPlatformUserData == IntPtr.Zero)
                return 0;

            var bd = GetBackendData();

            switch (msg)
            {
                case WM.WM_MOUSEMOVE:
                case WM.WM_NCMOUSEMOVE:
                    {
                        ImGuiMouseSource mouseSource = GetMouseSourceFromMessageExtraInfo();
                        int area = (msg == WM.WM_MOUSEMOVE) ? 1 : 2;
                        bd->MouseHwnd = hwnd;
                        if (bd->MouseTrackedArea != area)
                        {
                            tmeCancel.cbSize = TRACKMOUSEEVENT.Size;
                            tmeCancel.dwFlags = (uint)TME.TME_CANCEL;
                            tmeCancel.hwndTrack = hwnd;
                            tmeCancel.dwHoverTime = default;

                            tmeTrack.cbSize = TRACKMOUSEEVENT.Size;
                            tmeTrack.dwFlags = (uint)((area == 2) ? (TME.TME_LEAVE | TME.TME_NONCLIENT) : TME.TME_LEAVE);
                            tmeTrack.hwndTrack = hwnd;
                            tmeTrack.dwHoverTime = default;

                            if (bd->MouseTrackedArea != 0)
                            {
                                TrackMouseEvent(ref tmeCancel);
                            }

                            TrackMouseEvent(ref tmeTrack);
                            bd->MouseTrackedArea = area;
                        }

                        POINT point;
                        point.X = GET_X_LPARAM(lParam);
                        point.Y = GET_Y_LPARAM(lParam);

                        if (msg == WM.WM_NCMOUSEMOVE && !ScreenToClient(hwnd, ref point))
                        {
                            return 0;
                        }

                        io.AddMouseSourceEvent(mouseSource);
                        io.AddMousePosEvent(point.X, point.Y);

                        return 0;
                    }

                case WM.WM_MOUSELEAVE:
                case WM.WM_NCMOUSELEAVE:
                    {
                        int area = (msg == WM.WM_MOUSELEAVE) ? 1 : 2;
                        if (bd->MouseTrackedArea == area)
                        {
                            if (bd->MouseHwnd == hwnd)
                                bd->MouseHwnd = IntPtr.Zero;
                            bd->MouseTrackedArea = 0;
                            io.AddMousePosEvent(-float.MaxValue, -float.MaxValue);
                        }
                        return 0;
                    }
                case WM.WM_DESTROY:
                    if (bd->MouseHwnd == hwnd && bd->MouseTrackedArea != 0)
                    {

                        tmeCancel.cbSize = (uint)TRACKMOUSEEVENT.Size;
                        tmeCancel.dwFlags = (uint)TME.TME_CANCEL;
                        tmeCancel.hwndTrack = hwnd;
                        tmeCancel.dwHoverTime = default;

                        TrackMouseEvent(ref tmeCancel);
                        bd->MouseHwnd = IntPtr.Zero;
                        bd->MouseTrackedArea = 0;
                        io.AddMousePosEvent(-float.MaxValue, -float.MaxValue);
                    }
                    return 0;
                case WM.WM_LBUTTONDOWN:
                case WM.WM_LBUTTONDBLCLK:
                case WM.WM_RBUTTONDOWN:
                case WM.WM_RBUTTONDBLCLK:
                case WM.WM_MBUTTONDOWN:
                case WM.WM_MBUTTONDBLCLK:
                case WM.WM_XBUTTONDOWN:
                case WM.WM_XBUTTONDBLCLK:
                    {
                        ImGuiMouseSource mouse_source = GetMouseSourceFromMessageExtraInfo();
                        int button = 0;
                        if (msg == WM.WM_LBUTTONDOWN || msg == WM.WM_LBUTTONDBLCLK) { button = 0; }
                        if (msg == WM.WM_RBUTTONDOWN || msg == WM.WM_RBUTTONDBLCLK) { button = 1; }
                        if (msg == WM.WM_MBUTTONDOWN || msg == WM.WM_MBUTTONDBLCLK) { button = 2; }
                        if (bd->MouseButtonsDown == 0 && GetCapture() == IntPtr.Zero)
                            SetCapture(hwnd);
                        bd->MouseButtonsDown |= 1 << button;
                        io.AddMouseSourceEvent(mouse_source);
                        io.AddMouseButtonEvent(button, true);
                        return 0;
                    }
                case WM.WM_LBUTTONUP:
                case WM.WM_RBUTTONUP:
                case WM.WM_MBUTTONUP:
                case WM.WM_XBUTTONUP:
                    {
                        ImGuiMouseSource mouse_source = GetMouseSourceFromMessageExtraInfo();
                        int button = 0;
                        if (msg == WM.WM_LBUTTONUP) { button = 0; }
                        if (msg == WM.WM_RBUTTONUP) { button = 1; }
                        if (msg == WM.WM_MBUTTONUP) { button = 2; }

                        bd->MouseButtonsDown &= ~(1 << button);
                        if (bd->MouseButtonsDown == 0 && GetCapture() == hwnd)
                            ReleaseCapture();

                        io.AddMouseSourceEvent(mouse_source);
                        io.AddMouseButtonEvent(button, false);
                        return 0;
                    }
                case WM.WM_MOUSEWHEEL:
                    io.AddMouseWheelEvent(0.0f, (float)GET_WHEEL_DELTA_WPARAM(wParam.ToUInt32()) / (float)120);
                    return 0;
                case WM.WM_MOUSEHWHEEL:
                    io.AddMouseWheelEvent(-(float)GET_WHEEL_DELTA_WPARAM(wParam.ToUInt32()) / (float)120, 0.0f);
                    return 0;
                case WM.WM_KEYDOWN:
                case WM.WM_KEYUP:
                case WM.WM_SYSKEYDOWN:
                case WM.WM_SYSKEYUP:
                    {
                        bool is_key_down = msg == WM.WM_KEYDOWN || msg == WM.WM_SYSKEYDOWN;
                        if (wParam < 256)
                        {
                            UpdateKeyModifiers(io);

                            ImGuiKey key = KeyEventToImGuiKey(wParam, lParam);
                            int vk = (int)wParam.ToUInt32();
                            int scancode = LOBYTE(HIWORD(lParam));

                            if (key == ImGuiKey.PrintScreen && !is_key_down)
                                AddKeyEvent(io, key, true, vk, scancode);

                            if (key != ImGuiKey.None)
                                AddKeyEvent(io, key, is_key_down, vk, scancode);

                            if (vk == (int)VK.VK_SHIFT)
                            {
                                if (IsKeyPressed((int)VK.VK_LSHIFT) == is_key_down) { AddKeyEvent(io, ImGuiKey.LeftShift, is_key_down, (int)VK.VK_LSHIFT, scancode); }
                                if (IsKeyPressed((int)VK.VK_RSHIFT) == is_key_down) { AddKeyEvent(io, ImGuiKey.RightShift, is_key_down, (int)VK.VK_RSHIFT, scancode); }
                            }
                            else if (vk == (int)VK.VK_CONTROL)
                            {
                                if (IsKeyPressed((int)VK.VK_LCONTROL) == is_key_down) { AddKeyEvent(io, ImGuiKey.LeftCtrl, is_key_down, (int)VK.VK_LCONTROL, scancode); }
                                if (IsKeyPressed((int)VK.VK_RCONTROL) == is_key_down) { AddKeyEvent(io, ImGuiKey.RightCtrl, is_key_down, (int)VK.VK_RCONTROL, scancode); }
                            }
                            else if (vk == (int)VK.VK_MENU)
                            {
                                if (IsKeyPressed((int)VK.VK_LMENU) == is_key_down) { AddKeyEvent(io, ImGuiKey.LeftAlt, is_key_down, (int)VK.VK_LMENU, scancode); }
                                if (IsKeyPressed((int)VK.VK_RMENU) == is_key_down) { AddKeyEvent(io, ImGuiKey.RightAlt, is_key_down, (int)VK.VK_RMENU, scancode); }
                            }
                        }
                        return 0;
                    }
                case WM.WM_SETFOCUS:
                case WM.WM_KILLFOCUS:
                    io.AddFocusEvent(msg == WM.WM_SETFOCUS);
                    return 0;
                case WM.WM_INPUTLANGCHANGE:
                    UpdateKeyboardCodePage(io);
                    return 0;
                case WM.WM_CHAR:
                    if (IsWindowUnicode(hwnd))
                    {

                        if (wParam.ToUInt32() > 0 && wParam.ToUInt32() < 0x10000)
                            io.AddInputCharacterUTF16((ushort)wParam.ToUInt32());
                    }
                    else
                    {
                        uint codePage = bd->KeyboardCodePage;
                        ReadOnlySpan<byte> multiByteArray = [(byte)wParam.ToUInt32()];
                        string convertedStr = MultiByteToWC(multiByteArray, (int)codePage);
                        if (convertedStr.Length > 0)
                        {
                            ushort wch = convertedStr[0];
                            io.AddInputCharacter(wch);
                        }
                    }
                    return 0;

                case WM.WM_SETCURSOR:
                    if (LOWORD(lParam.ToInt32()) == 1 && UpdateMouseCursor(io, (ImGuiMouseCursor)bd->LastMouseCursor))
                        return 1;
                    return 0;
                case WM.WM_DEVICECHANGE:
                    return 0;
            }
            return 0;
        }
    }

}