namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;

    /// <summary>
    /// This test prints monitor and video mode information or verifies video modes.
    /// </summary>
    /// <remarks>
    /// Ported from <c>monitors.c</c>.
    /// </remarks>
    class TestMonitors : TestBase
    {
        enum Mode
        {
            ListMode,
            TestMode
        };

        class Options
        {
            [Option('t', HelpText = "Test mode")]
            public bool Mode { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static string FormatMode(Glfw.VideoMode mode)
        {
            return string.Format("{0} x {1} x {2} ({3} {4} {5}) {6} Hz",
                mode.Width, mode.Height,
                mode.RedBits + mode.GreenBits + mode.BlueBits,
                mode.RedBits, mode.GreenBits, mode.BlueBits,
                mode.RefreshRate
            );
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Log("Framebuffer resized to {0}x{1}", width, height);
            Gl.Viewport(0, 0, width, height);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state == Glfw.InputState.Press && key == Glfw.KeyCode.Escape)
                Glfw.SetWindowShouldClose(window, true);
        }

        static void ListModes(Glfw.Monitor monitor)
        {
            int x, y, widthMM, heightMM;
            var mode = Glfw.GetVideoMode(monitor);
            var modes = Glfw.GetVideoModes(monitor);

            Glfw.GetMonitorPos(monitor, out x, out y);
            Glfw.GetMonitorPhysicalSize(monitor, out widthMM, out heightMM);

            Log("Name: {0} ({1})",
                Glfw.GetMonitorName(monitor),
                Glfw.GetPrimaryMonitor() == monitor ? "primary" : "secondary");
            Log("Current mode: {0}", FormatMode(mode));
            Log("Virtual position: {0} {1}", x, y);

            Log("Physical size: {0} x {1} mm ({2} dpi)",
                   widthMM, heightMM, mode.Width * 25.4f / widthMM);

            Log("Modes:");

            for (int i = 0; i < modes.Length; i++)
            {
                string currentMode = modes[i] == mode
                    ? "(current mode)"
                    : "";
                Log("{0}: {1} {2}", i, FormatMode(modes[i]), currentMode);
            }
        }

        static void TestModes(Glfw.Monitor monitor)
        {
            Glfw.Window window;
            var modes = Glfw.GetVideoModes(monitor);

            for (int i = 0; i < modes.Length; i++)
            {
                var mode = modes[i];
                Glfw.VideoMode current;

                Glfw.WindowHint(Glfw.Hint.RedBits, mode.RedBits);
                Glfw.WindowHint(Glfw.Hint.GreenBits, mode.GreenBits);
                Glfw.WindowHint(Glfw.Hint.BlueBits, mode.BlueBits);
                Glfw.WindowHint(Glfw.Hint.RefreshRate, mode.RefreshRate);

                Log("Testing mode {0} on monitor {1}: {2}", i, Glfw.GetMonitorName(monitor), FormatMode(mode));

                window = Glfw.CreateWindow(mode.Width, mode.Height,
                                          "Video Mode Test",
                                          Glfw.GetPrimaryMonitor(),
                                          Glfw.Window.None);
                if (!window)
                {
                    Log("Failed to enter mode {0}: {1}", i, FormatMode(mode));
                    continue;
                }

                Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);
                Glfw.SetKeyCallback(window, KeyCallback);

                Glfw.MakeContextCurrent(window);
                Glfw.SwapInterval(1);

                Glfw.SetTime(0.0);

                while (Glfw.GetTime() < 5.0)
                {
                    Gl.Clear(ClearBufferMask.ColorBufferBit);
                    Glfw.SwapBuffers(window);
                    Glfw.PollEvents();

                    if (Glfw.WindowShouldClose(window))
                    {
                        Log("User terminated program");

                        Glfw.Terminate();
                        Environment.Exit(0);
                    }
                }

                Gl.Get(GetPName.RedBits, out current.RedBits);
                Gl.Get(GetPName.GreenBits, out current.GreenBits);
                Gl.Get(GetPName.BlueBits, out current.BlueBits);

                Glfw.GetWindowSize(window, out current.Width, out current.Height);

                if (current.RedBits != mode.RedBits ||
                    current.GreenBits != mode.GreenBits ||
                    current.BlueBits != mode.BlueBits)
                {
                    Log("*** Color bit mismatch: ({0} {1} {2}) instead of ({3} {4} {5})\n",
                        current.RedBits, current.GreenBits, current.BlueBits,
                        mode.RedBits, mode.GreenBits, mode.BlueBits);
                }

                if (current.Width != mode.Width || current.Height != mode.Height)
                {
                    Log("*** Size mismatch: %ix%i instead of %ix%i\n",
                        current.Width, current.Height,
                        mode.Width, mode.Height);
                }

                Log("Closing window");

                Glfw.DestroyWindow(window);
                Glfw.PollEvents();
            }
        }

        static void Main(string[] args)
        {
            Init();

            var mode = Mode.ListMode;
            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
                mode = options.Mode ? Mode.TestMode : Mode.ListMode;

            if (!Glfw.Init())
                Environment.Exit(1);

            var monitors = Glfw.GetMonitors();

            for (int i = 0; i < monitors.Length; i++)
            {
                if (mode == Mode.ListMode)
                    ListModes(monitors[i]);
                else if (mode == Mode.TestMode)
                    TestModes(monitors[i]);
            }

            Glfw.Terminate();
        }
    }
}
