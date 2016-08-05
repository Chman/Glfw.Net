namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;

    /// <summary>
    /// This test renders a high contrast, horizontally moving bar, allowing for visual verification
    /// of whether the set swap interval is indeed obeyed.
    /// </summary>
    /// <remarks>
    /// Ported from <c>tearing.c</c>.
    /// </remarks>
    class TestTearing : TestBase
    {
        static bool swapTear;
        static int swapInterval;
        static double frameRate;

        class Options
        {
            [Option('f', HelpText = "create full screen window")]
            public bool Fullscreen { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void UpdateWindowTitle(Glfw.Window window)
        {
            string title = string.Format("Tearing detector (interval {0}{1}, {2} Hz)",
                swapInterval,
                (swapTear && swapInterval < 0) ? " (swap tear)" : "",
                frameRate);

            Glfw.SetWindowTitle(window, title);
        }

        static void SetSwapInterval(Glfw.Window window, int interval)
        {
            swapInterval = interval;
            Glfw.SwapInterval(swapInterval);
            UpdateWindowTitle(window);
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Up:
                {
                    if (swapInterval + 1 > swapInterval)
                        SetSwapInterval(window, swapInterval + 1);
                    break;
                }

                case Glfw.KeyCode.Down:
                {
                    if (swapTear)
                    {
                        if (swapInterval - 1 < swapInterval)
                            SetSwapInterval(window, swapInterval - 1);
                    }
                    else
                    {
                        if (swapInterval - 1 >= 0)
                            SetSwapInterval(window, swapInterval - 1);
                    }
                    break;
                }

                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;
            }
        }

        static void Main(string[] args)
        {
            Init();

            var options = new Options();

            int width, height;
            float position;
            ulong frameCount = 0;
            double lastTime, currentTime;
            bool fullscreen = false;
            var monitor = Glfw.Monitor.None;
            Glfw.Window window;
            
            if (Parser.Default.ParseArguments(args, options))
                fullscreen = options.Fullscreen;

            if (!Glfw.Init())
                Environment.Exit(1);

            if (fullscreen)
            {
                monitor = Glfw.GetPrimaryMonitor();
                var mode = Glfw.GetVideoMode(monitor);

                Glfw.WindowHint(Glfw.Hint.RedBits, mode.RedBits);
                Glfw.WindowHint(Glfw.Hint.GreenBits, mode.GreenBits);
                Glfw.WindowHint(Glfw.Hint.BlueBits, mode.BlueBits);
                Glfw.WindowHint(Glfw.Hint.RefreshRate, mode.RefreshRate);

                width = mode.Width;
                height = mode.Height;
            }
            else
            {
                width = 640;
                height = 480;
            }

            window = Glfw.CreateWindow(width, height, "", monitor, Glfw.Window.None);
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);
            SetSwapInterval(window, 0);

            lastTime = Glfw.GetTime();
            frameRate = 0.0;
            swapTear = (Glfw.ExtensionSupported("WGL_EXT_swap_control_tear") ||
                         Glfw.ExtensionSupported("GLX_EXT_swap_control_tear"));

            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);
            Glfw.SetKeyCallback(window, KeyCallback);
            
            Gl.MatrixMode(MatrixMode.Projection);
            Gl.Ortho(-1f, 1f, -1f, 1f, 1f, -1f);
            Gl.MatrixMode(MatrixMode.Modelview);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                position = (float)System.Math.Cos(Glfw.GetTime() * 4f) * 0.75f;
                Gl.Rect(position - 0.25f, -1f, position + 0.25f, 1f);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();

                frameCount++;

                currentTime = Glfw.GetTime();
                if (currentTime - lastTime > 1.0)
                {
                    frameRate = frameCount / (currentTime - lastTime);
                    frameCount = 0;
                    lastTime = currentTime;
                    UpdateWindowTitle(window);
                }
            }

            Glfw.Terminate();
        }
    }
}
