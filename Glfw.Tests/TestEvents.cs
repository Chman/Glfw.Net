namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This test hooks every available callback and outputs their arguments. Every event also gets
    /// a (sequential) number to aid discussion of logs.
    /// </summary>
    /// <remarks>
    /// Ported from <c>events.c</c>.
    /// </remarks>
    class TestEvents : TestBase
    {
        // Event index
        static uint m_Counter = 0;

        static Dictionary<Glfw.Window, Slot> m_Slots;

        struct Slot
        {
            public Glfw.Window window;
            public int Number;
            public bool Closeable;
        }

        class Options
        {
            [Option('f', HelpText = "Use full screen")]
            public bool Fullscreen { get; set; }

            [Option('n', HelpText = "The number of windows to create", DefaultValue = 1)]
            public int WindowCount { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static string GetKeyName(Glfw.KeyCode key)
        {
            switch (key)
            {
                // Printable keys
                case Glfw.KeyCode.A:                       return "A";
                case Glfw.KeyCode.B:                       return "B";
                case Glfw.KeyCode.C:                       return "C";
                case Glfw.KeyCode.D:                       return "D";
                case Glfw.KeyCode.E:                       return "E";
                case Glfw.KeyCode.F:                       return "F";
                case Glfw.KeyCode.G:                       return "G";
                case Glfw.KeyCode.H:                       return "H";
                case Glfw.KeyCode.I:                       return "I";
                case Glfw.KeyCode.J:                       return "J";
                case Glfw.KeyCode.K:                       return "K";
                case Glfw.KeyCode.L:                       return "L";
                case Glfw.KeyCode.M:                       return "M";
                case Glfw.KeyCode.N:                       return "N";
                case Glfw.KeyCode.O:                       return "O";
                case Glfw.KeyCode.P:                       return "P";
                case Glfw.KeyCode.Q:                       return "Q";
                case Glfw.KeyCode.R:                       return "R";
                case Glfw.KeyCode.S:                       return "S";
                case Glfw.KeyCode.T:                       return "T";
                case Glfw.KeyCode.U:                       return "U";
                case Glfw.KeyCode.V:                       return "V";
                case Glfw.KeyCode.W:                       return "W";
                case Glfw.KeyCode.X:                       return "X";
                case Glfw.KeyCode.Y:                       return "Y";
                case Glfw.KeyCode.Z:                       return "Z";
                case Glfw.KeyCode.Alpha1:                  return "1";
                case Glfw.KeyCode.Alpha2:                  return "2";
                case Glfw.KeyCode.Alpha3:                  return "3";
                case Glfw.KeyCode.Alpha4:                  return "4";
                case Glfw.KeyCode.Alpha5:                  return "5";
                case Glfw.KeyCode.Alpha6:                  return "6";
                case Glfw.KeyCode.Alpha7:                  return "7";
                case Glfw.KeyCode.Alpha8:                  return "8";
                case Glfw.KeyCode.Alpha9:                  return "9";
                case Glfw.KeyCode.Alpha0:                  return "0";
                case Glfw.KeyCode.Space:                   return "SPACE";
                case Glfw.KeyCode.Minus:                   return "MINUS";
                case Glfw.KeyCode.Equal:                   return "EQUAL";
                case Glfw.KeyCode.LeftBracket:             return "LEFT BRACKET";
                case Glfw.KeyCode.RightBracket:            return "RIGHT BRACKET";
                case Glfw.KeyCode.Backslash:               return "BACKSLASH";
                case Glfw.KeyCode.SemiColon:               return "SEMICOLON";
                case Glfw.KeyCode.Apostrophe:              return "APOSTROPHE";
                case Glfw.KeyCode.GraveAccent:             return "GRAVE ACCENT";
                case Glfw.KeyCode.Comma:                   return "COMMA";
                case Glfw.KeyCode.Period:                  return "PERIOD";
                case Glfw.KeyCode.Slash:                   return "SLASH";
                case Glfw.KeyCode.World1:                  return "WORLD 1";
                case Glfw.KeyCode.World2:                  return "WORLD 2";

                // Function keys
                case Glfw.KeyCode.Escape:                  return "ESCAPE";
                case Glfw.KeyCode.F1:                      return "F1";
                case Glfw.KeyCode.F2:                      return "F2";
                case Glfw.KeyCode.F3:                      return "F3";
                case Glfw.KeyCode.F4:                      return "F4";
                case Glfw.KeyCode.F5:                      return "F5";
                case Glfw.KeyCode.F6:                      return "F6";
                case Glfw.KeyCode.F7:                      return "F7";
                case Glfw.KeyCode.F8:                      return "F8";
                case Glfw.KeyCode.F9:                      return "F9";
                case Glfw.KeyCode.F10:                     return "F10";
                case Glfw.KeyCode.F11:                     return "F11";
                case Glfw.KeyCode.F12:                     return "F12";
                case Glfw.KeyCode.F13:                     return "F13";
                case Glfw.KeyCode.F14:                     return "F14";
                case Glfw.KeyCode.F15:                     return "F15";
                case Glfw.KeyCode.F16:                     return "F16";
                case Glfw.KeyCode.F17:                     return "F17";
                case Glfw.KeyCode.F18:                     return "F18";
                case Glfw.KeyCode.F19:                     return "F19";
                case Glfw.KeyCode.F20:                     return "F20";
                case Glfw.KeyCode.F21:                     return "F21";
                case Glfw.KeyCode.F22:                     return "F22";
                case Glfw.KeyCode.F23:                     return "F23";
                case Glfw.KeyCode.F24:                     return "F24";
                case Glfw.KeyCode.F25:                     return "F25";
                case Glfw.KeyCode.Up:                      return "UP";
                case Glfw.KeyCode.Down:                    return "DOWN";
                case Glfw.KeyCode.Left:                    return "LEFT";
                case Glfw.KeyCode.Right:                   return "RIGHT";
                case Glfw.KeyCode.LeftShift:               return "LEFT SHIFT";
                case Glfw.KeyCode.RightShift:              return "RIGHT SHIFT";
                case Glfw.KeyCode.LeftControl:             return "LEFT CONTROL";
                case Glfw.KeyCode.RightControl:            return "RIGHT CONTROL";
                case Glfw.KeyCode.LeftAlt:                 return "LEFT ALT";
                case Glfw.KeyCode.RightAlt:                return "RIGHT ALT";
                case Glfw.KeyCode.Tab:                     return "TAB";
                case Glfw.KeyCode.Enter:                   return "ENTER";
                case Glfw.KeyCode.Backspace:               return "BACKSPACE";
                case Glfw.KeyCode.Insert:                  return "INSERT";
                case Glfw.KeyCode.Delete:                  return "DELETE";
                case Glfw.KeyCode.PageUp:                  return "PAGE UP";
                case Glfw.KeyCode.PageDown:                return "PAGE DOWN";
                case Glfw.KeyCode.Home:                    return "HOME";
                case Glfw.KeyCode.End:                     return "END";
                case Glfw.KeyCode.Numpad0:                 return "KEYPAD 0";
                case Glfw.KeyCode.Numpad1:                 return "KEYPAD 1";
                case Glfw.KeyCode.Numpad2:                 return "KEYPAD 2";
                case Glfw.KeyCode.Numpad3:                 return "KEYPAD 3";
                case Glfw.KeyCode.Numpad4:                 return "KEYPAD 4";
                case Glfw.KeyCode.Numpad5:                 return "KEYPAD 5";
                case Glfw.KeyCode.Numpad6:                 return "KEYPAD 6";
                case Glfw.KeyCode.Numpad7:                 return "KEYPAD 7";
                case Glfw.KeyCode.Numpad8:                 return "KEYPAD 8";
                case Glfw.KeyCode.Numpad9:                 return "KEYPAD 9";
                case Glfw.KeyCode.NumpadDivide:            return "KEYPAD DIVIDE";
                case Glfw.KeyCode.NumpadMultiply:          return "KEYPAD MULTPLY";
                case Glfw.KeyCode.NumpadSubtract:          return "KEYPAD SUBTRACT";
                case Glfw.KeyCode.NumpadAdd:               return "KEYPAD ADD";
                case Glfw.KeyCode.NumpadDecimal:           return "KEYPAD DECIMAL";
                case Glfw.KeyCode.NumpadEqual:             return "KEYPAD EQUAL";
                case Glfw.KeyCode.NumpadEnter:             return "KEYPAD ENTER";
                case Glfw.KeyCode.PrintScreen:             return "PRINT SCREEN";
                case Glfw.KeyCode.NumLock:                 return "NUM LOCK";
                case Glfw.KeyCode.CapsLock:                return "CAPS LOCK";
                case Glfw.KeyCode.ScrollLock:              return "SCROLL LOCK";
                case Glfw.KeyCode.Pause:                   return "PAUSE";
                case Glfw.KeyCode.LeftSuper:               return "LEFT SUPER";
                case Glfw.KeyCode.RightSuper:              return "RIGHT SUPER";
                case Glfw.KeyCode.Menu:                    return "MENU";

                default:                                   return "UNKNOWN";
            }
        }

        static string GetActioName(Glfw.InputState action)
        {
            switch (action)
            {
                case Glfw.InputState.Press:
                    return "pressed";
                case Glfw.InputState.Release:
                    return "released";
                case Glfw.InputState.Repeat:
                    return "repeated";
            }

            return "caused unknown action";
        }

        static string GetButtonName(Glfw.MouseButton button)
        {
            switch (button)
            {
                case Glfw.MouseButton.ButtonLeft:
                    return "left";
                case Glfw.MouseButton.ButtonRight:
                    return "right";
                case Glfw.MouseButton.ButtonMiddle:
                    return "middle";
                default:
                    return button.ToString();
            }
        }

        static string GetModsName(Glfw.KeyMods mods)
        {
            string name = "";

            if ((mods & Glfw.KeyMods.Shift) > 0)
                name += " shift";
            if ((mods & Glfw.KeyMods.Control) > 0)
                name += " control";
            if ((mods & Glfw.KeyMods.Alt) > 0)
                name += " alt";
            if ((mods & Glfw.KeyMods.Super) > 0)
                name += " super";

            return name;
        }
        
        static string GetCharacterString(uint codepoint)
        {
            return Convert.ToChar(codepoint).ToString();
        }

        static void WindowPosCallback(Glfw.Window window, int x, int y)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window position: {3} {4}",
                m_Counter++, slot.Number, Glfw.GetTime(), x, y);
        }

        static void WindowSizeCallback(Glfw.Window window, int width, int height)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window size: {3} {4}",
                m_Counter++, slot.Number, Glfw.GetTime(), width, height);
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Framebuffer size: {3} {4}",
                m_Counter++, slot.Number, Glfw.GetTime(), width, height);

            Gl.Viewport(0, 0, width, height);
        }

        static void WindowCloseCallback(Glfw.Window window)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window close",
                m_Counter++, slot.Number, Glfw.GetTime());

            Glfw.SetWindowShouldClose(window, slot.Closeable);
        }

        static void WindowRefreshCallback(Glfw.Window window)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window refresh",
                m_Counter++, slot.Number, Glfw.GetTime());

            Glfw.MakeContextCurrent(window);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Glfw.SwapBuffers(window);
        }

        static void WindowFocusCallback(Glfw.Window window, bool focused)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window {3}",
                m_Counter++, slot.Number, Glfw.GetTime(),
                focused ? "focused" : "defocused");
        }

        static void WindowIconifyCallback(Glfw.Window window, bool iconified)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Window was {3}",
                m_Counter++, slot.Number, Glfw.GetTime(),
                iconified ? "iconified" : "restored");
        }

        static void MouseButtonCallback(Glfw.Window window, Glfw.MouseButton button, Glfw.InputState state, Glfw.KeyMods mods)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Mouse button {3} ({4}) ({5}) was {6}",
                m_Counter++, slot.Number, Glfw.GetTime(), button,
                GetButtonName(button),
                GetModsName(mods),
                GetActioName(state));
        }

        static void CursorPositionCallback(Glfw.Window window, double x, double y)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Cursor position: {3} {4}",
                   m_Counter++, slot.Number, Glfw.GetTime(), x, y);
        }

        static void CursorEnterCallback(Glfw.Window window, bool entered)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Cursor {3} window",
                m_Counter++, slot.Number, Glfw.GetTime(),
                entered ? "entered" : "left");
        }

        static void ScrollCallback(Glfw.Window window, double x, double y)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Scroll: {3} {4}",
                m_Counter++, slot.Number, Glfw.GetTime(), x, y);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            var slot = m_Slots[window];
            string name = Glfw.GetKeyName(key, scancode);

            if (!string.IsNullOrEmpty(name))
            {
                Log("{0} to {1} at {2}: Key 0x{3} Scancode 0x{4} ({5}) ({6}) ({7}) was {8}",
                    m_Counter++, slot.Number, Glfw.GetTime(), (int)key, scancode,
                    GetKeyName(key),
                    name,
                    GetModsName(mods),
                    GetActioName(state));
            }
            else
            {
                Log("{0} to {1} at {2}: Key 0x{3} Scancode 0x{4} ({5}) ({6}) was {7}",
                    m_Counter++, slot.Number, Glfw.GetTime(), (int)key, scancode,
                    GetKeyName(key),
                    GetModsName(mods),
                    GetActioName(state));
            }

            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.C:
                {
                    slot.Closeable = !slot.Closeable;

                    Log("(( closing {0} ))", slot.Closeable ? "enabled" : "disabled");
                    break;
                }
            }
        }

        static void CharCallback(Glfw.Window window, uint codepoint)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Character 0x{3} ({4}) input",
                   m_Counter++, slot.Number, Glfw.GetTime(), codepoint,
                   GetCharacterString(codepoint));
        }

        static void CharModsCallback(Glfw.Window window, uint codepoint, Glfw.KeyMods mods)
        {
            var slot = m_Slots[window];
            Log("{0} to {1} at {2}: Character 0x{3} ({4}) with modifiers ({5}) input",
                m_Counter++, slot.Number, Glfw.GetTime(), codepoint,
                GetCharacterString(codepoint),
                GetModsName(mods));
        }

        static void DropCallback(Glfw.Window window, int count, string[] paths)
        {
            var slot = m_Slots[window];

            Log("{0} to {1} at {2}: Drop input",
                   m_Counter++, slot.Number, Glfw.GetTime());

            for (int i = 0;  i < paths.Length; i++)
                Log("  {0}: \"{1}\"", i, paths[i]);
        }

        static void MonitorCallback(Glfw.Monitor monitor, Glfw.ConnectionEvent evt)
        {
            if (evt == Glfw.ConnectionEvent.Connected)
            {
                int x, y, widthMM, heightMM;
                var mode = Glfw.GetVideoMode(monitor);

                Glfw.GetMonitorPos(monitor, out x, out y);
                Glfw.GetMonitorPhysicalSize(monitor, out widthMM, out heightMM);

                Log("{0} at {1}: Monitor {2} ({3}x{4} at {5}x{6}, {7}x{8} mm) was connected",
                    m_Counter++,
                    Glfw.GetTime(),
                    Glfw.GetMonitorName(monitor),
                    mode.Width, mode.Height,
                    x, y,
                    widthMM, heightMM);
            }
            else if (evt == Glfw.ConnectionEvent.Disconnected)
            {
                Log("{0} at {1}: Monitor {2} was disconnected",
                    m_Counter++,
                    Glfw.GetTime(),
                    Glfw.GetMonitorName(monitor));
            }
        }

        static void JoystickCallback(Glfw.Joystick joy, Glfw.ConnectionEvent evt)
        {
            if (evt == Glfw.ConnectionEvent.Connected)
            {
                var axes = Glfw.GetJoystickAxes(joy);
                var buttons = Glfw.GetJoystickButtons(joy);

                Log("{0} at {1}: Joystick {2} ({3}) was connected with {4} axes and {5} buttons",
                    m_Counter++, Glfw.GetTime(),
                    joy,
                    Glfw.GetJoystickName(joy),
                    axes.Length,
                    buttons.Length);
            }
            else
            {
                Log("{0} at {1}: Joystick {2} was disconnected",
                    m_Counter++, Glfw.GetTime(), joy);
            }
        }

        static void Main(string[] args)
        {
            Init();
            
            var monitor = Glfw.Monitor.None;
            int width, height, count = 1;

            if (!Glfw.Init())
                Environment.Exit(1);

            Log("Library initialized");

            Glfw.SetMonitorCallback(MonitorCallback);
            Glfw.SetJoystickCallback(JoystickCallback);

            var options = new Options();
            
            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Fullscreen)
                    Glfw.GetPrimaryMonitor();
                else
                    count = options.WindowCount;
            }

            if (monitor)
            {
                var mode = Glfw.GetVideoMode(monitor);

                Glfw.WindowHint(Glfw.Hint.RefreshRate, mode.RefreshRate);
                Glfw.WindowHint(Glfw.Hint.RedBits, mode.RedBits);
                Glfw.WindowHint(Glfw.Hint.GreenBits, mode.GreenBits);
                Glfw.WindowHint(Glfw.Hint.BlueBits, mode.BlueBits);

                width = mode.Width;
                height = mode.Height;
            }
            else
            {
                width = 640;
                height = 480;
            }

            if (count <= 0)
            {
                Log("Invalid user");
                Environment.Exit(1);
            }

            m_Slots = new Dictionary<Glfw.Window, Slot>(count);

            for (int i = 0; i < count; i++)
            {
                var slot = new Slot();

                slot.Closeable = true;
                slot.Number = i + 1;

                string title = string.Format("Event Linter (Window {0})", slot.Number);

                if (monitor)
                {
                    Log("Creating full screen window {0} ({1}x{2} on {3})\n",
                        slot.Number,
                        width, height,
                        Glfw.GetMonitorName(monitor));
                }
                else
                {
                    Log("Creating windowed mode window {0} ({1}x{2})\n",
                        slot.Number,
                        width, height);
                }

                slot.window = Glfw.CreateWindow(width, height, title, monitor, Glfw.Window.None);

                if (!slot.window)
                {
                    Glfw.Terminate();
                    Environment.Exit(1);
                }

                m_Slots.Add(slot.window, slot);

                Glfw.SetWindowPosCallback(slot.window, WindowPosCallback);
                Glfw.SetWindowSizeCallback(slot.window, WindowSizeCallback);
                Glfw.SetFramebufferSizeCallback(slot.window, FramebufferSizeCallback);
                Glfw.SetWindowCloseCallback(slot.window, WindowCloseCallback);
                Glfw.SetWindowRefreshCallback(slot.window, WindowRefreshCallback);
                Glfw.SetWindowFocusCallback(slot.window, WindowFocusCallback);
                Glfw.SetWindowIconifyCallback(slot.window, WindowIconifyCallback);
                Glfw.SetMouseButtonCallback(slot.window, MouseButtonCallback);
                Glfw.SetCursorPosCallback(slot.window, CursorPositionCallback);
                Glfw.SetCursorEnterCallback(slot.window, CursorEnterCallback);
                Glfw.SetScrollCallback(slot.window, ScrollCallback);
                Glfw.SetKeyCallback(slot.window, KeyCallback);
                Glfw.SetCharCallback(slot.window, CharCallback);
                Glfw.SetCharModsCallback(slot.window, CharModsCallback);
                Glfw.SetDropCallback(slot.window, DropCallback);

                Glfw.MakeContextCurrent(slot.window);
                Glfw.SwapInterval(1);
            }

            Log("Main loop starting");

            for (;;)
            {
                int i = 0;
                foreach (var slot in m_Slots.Values)
                {
                    i++;

                    if (Glfw.WindowShouldClose(slot.window))
                        break;
                }

                if (i < count)
                    break;

                Glfw.WaitEvents();
            }
            
            Glfw.Terminate();
        }
    }
}
