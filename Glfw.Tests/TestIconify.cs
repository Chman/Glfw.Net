namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;

    /// <summary>
    /// This program is used to test the iconify/restore functionality for both full screen and
    /// windowed mode windows
    /// </summary>
    /// <remarks>
    /// Ported from <c>iconify.c</c>.
    /// </remarks>
    class TestIconify : TestBase
    {
        static int m_WindowedXPos, m_WindowedYPos, m_WindowedWidth, m_WindowedHeight;
        
        class Options
        {
            [Option('a', HelpText = "Create windows for all monitors")]
            public bool AllMonitors { get; set; }

            [Option('f', HelpText = "Create full screen window(s)")]
            public bool Fullscreen { get; set; }

            [Option('n', HelpText = "No automatic iconification of full screen windows")]
            public bool AutoIconify { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            Log("{0} Key %s",
                   Glfw.GetTime(),
                   state == Glfw.InputState.Press ? "pressed" : "released");

            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.I:
                    Glfw.IconifyWindow(window);
                    break;
                case Glfw.KeyCode.M:
                    Glfw.MaximizeWindow(window);
                    break;
                case Glfw.KeyCode.R:
                    Glfw.RestoreWindow(window);
                    break;
                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;
                case Glfw.KeyCode.F11:
                case Glfw.KeyCode.Enter:
                {
                    if (mods != Glfw.KeyMods.Alt)
                        return;

                    if (Glfw.GetWindowMonitor(window))
                    {
                        Glfw.SetWindowMonitor(window, Glfw.Monitor.None,
                                                m_WindowedXPos, m_WindowedYPos,
                                                m_WindowedWidth, m_WindowedHeight,
                                                0);
                    }
                    else
                    {
                        var monitor = Glfw.GetPrimaryMonitor();
                        if (monitor)
                        {
                            var mode = Glfw.GetVideoMode(monitor);
                            Glfw.GetWindowPos(window, out m_WindowedXPos, out m_WindowedYPos);
                            Glfw.GetWindowSize(window, out m_WindowedWidth, out m_WindowedHeight);
                            Glfw.SetWindowMonitor(window, monitor,
                                                    0, 0, mode.Width, mode.Height,
                                                    mode.RefreshRate);
                        }
                    }

                    break;
                }
            }
        }

        static void WindowSizeCallback(Glfw.Window window, int width, int height)
        {
            Log("{0} Window resized to {1}x{2}", Glfw.GetTime(), width, height);
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Log("{0} Framebuffer resized to {1}x{2}\n", Glfw.GetTime(), width, height);

            Gl.Viewport(0, 0, width, height);
        }

        static void WindowFocusCallback(Glfw.Window window, bool focused)
        {
            Log("{0} Window {1}\n",
                   Glfw.GetTime(),
                   focused ? "focused" : "defocused");
        }

        static void WindowIconifyCallback(Glfw.Window window, bool iconified)
        {
            Log("%0.2f Window %s\n",
                   Glfw.GetTime(),
                   iconified ? "iconified" : "restored");
        }

        static void WindowRefreshCallback(Glfw.Window window)
        {
            int width, height;

            Log("{0} Window refresh\n", Glfw.GetTime());

            Glfw.GetFramebufferSize(window, out width, out height);

            Glfw.MakeContextCurrent(window);

            Gl.Enable(EnableCap.ScissorTest);

            Gl.Scissor(0, 0, width, height);
            Gl.ClearColor(0, 0, 0, 0);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            Gl.Scissor(0, 0, 640, 480);
            Gl.ClearColor(1, 1, 1, 0);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            Glfw.SwapBuffers(window);
        }

        static Glfw.Window CreateWindow(Glfw.Monitor monitor)
        {
            int width, height;
            Glfw.Window window;

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

            window = Glfw.CreateWindow(width, height, "Iconify", monitor, Glfw.Window.None);
            if (!window)
            {
                Glfw.Terminate();
                System.Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);

            return window;
        }

        static void Main(string[] args)
        {
            Init();
            
            bool autoIconify = true, fullscreen = false, allMonitors = false;
            Glfw.Window[] windows;

            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                autoIconify = options.AutoIconify;
                fullscreen = options.Fullscreen;
                allMonitors = options.AllMonitors;
            }

            if (!Glfw.Init())
                Environment.Exit(1);

            Glfw.WindowHint(Glfw.Hint.AutoIconify, autoIconify);

            if (fullscreen && allMonitors)
            {
                var monitors = Glfw.GetMonitors();
                windows = new Glfw.Window[monitors.Length];

                for (int i = 0; i < monitors.Length; i++)
                {
                    windows[i] = CreateWindow(monitors[i]);
                    if (!windows[i])
                        break;
                }
            }
            else
            {
                var monitor = Glfw.Monitor.None;

                if (fullscreen)
                    monitor = Glfw.GetPrimaryMonitor();
                
                windows = new Glfw.Window[1];
                windows[0] = CreateWindow(monitor);
            }

            for (int i = 0; i < windows.Length; i++)
            {
                Glfw.SetKeyCallback(windows[i], KeyCallback);
                Glfw.SetFramebufferSizeCallback(windows[i], FramebufferSizeCallback);
                Glfw.SetWindowSizeCallback(windows[i], WindowSizeCallback);
                Glfw.SetWindowFocusCallback(windows[i], WindowFocusCallback);
                Glfw.SetWindowIconifyCallback(windows[i], WindowIconifyCallback);
                Glfw.SetWindowRefreshCallback(windows[i], WindowRefreshCallback);

                WindowRefreshCallback(windows[i]);

                Log("Window is {0} and {1}\n",
                    Glfw.GetWindowAttrib(windows[i], Glfw.WindowAttrib.Iconified) ? "iconified" : "restored",
                    Glfw.GetWindowAttrib(windows[i], Glfw.WindowAttrib.Focused) ? "focused" : "defocused");
            }

            for (;;)
            {
                Glfw.WaitEvents();

                int i;
                for (i = 0; i < windows.Length; i++)
                {
                    if (Glfw.WindowShouldClose(windows[i]))
                        break;
                }

                if (i < windows.Length)
                    break;
            }

            Glfw.Terminate();
        }
    }
}
