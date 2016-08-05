namespace Glfw3.Tests
{
    using CommandLine;
    using CommandLine.Text;
    using OpenGL;
    using System;

    /// <summary>
    /// This program is used to test the gamma correction functionality for both full screen and
    /// windowed mode windows using <see cref="Glfw.SetGammaRamp(Glfw.Monitor, Glfw.GammaRamp)"/>.
    /// </summary>
    class TestGammaRamp : TestBase
    {
        const float kStepSize = 0.1f;
        static float m_GammaValue = 1f;

        class Options
        {
            [Option('f', HelpText = "Create full screen window")]
            public bool Fullscreen { get; set; }

            [HelpOption(HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void SetGamma(Glfw.Window window, float gamma)
        {
            var monitor = Glfw.GetWindowMonitor(window);
            if (!monitor)
                monitor = Glfw.GetPrimaryMonitor();

            if (gamma < 0f)
                return;

            // It is recommended to use gamma ramps of size 256, as that is the size supported by
            // all graphics cards on all platforms.
            const int kSize = 256;
            var ramp = new Glfw.GammaRamp
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            for (int i = 0; i < kSize; i++)
            {
                float value;

                // Calculate intensity
                value = (float)i / (float)(kSize - 1);
                // Apply gamma curve
                value = (float)Math.Pow(value, 1f / gamma) * 65535f + 0.5f;

                // Clamp to value range
                if (value < 0f)
                    value = 0f;
                else if (value > 65535f)
                    value = 65535f;

                ramp.Red[i] = (ushort)value;
                ramp.Green[i] = (ushort)value;
                ramp.Blue[i] = (ushort)value;
            }

            m_GammaValue = gamma;
            Log("Set Gamma: {0}", m_GammaValue);
            Glfw.SetGammaRamp(monitor, ramp);

            var r = Glfw.GetGammaRamp(monitor);
            for (int i = 0; i < r.Size; i++)
                Log("Ramp {0}: {1},{2},{3}", i, r.Red[i], r.Green[i], r.Blue[i]);
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Escape:
                {
                    Glfw.SetWindowShouldClose(window, true);
                    break;
                }

                case Glfw.KeyCode.NumpadAdd:
                case Glfw.KeyCode.Up:
                case Glfw.KeyCode.Q:
                {
                    SetGamma(window, m_GammaValue + kStepSize);
                    break;
                }

                case Glfw.KeyCode.NumpadSubtract:
                case Glfw.KeyCode.Down:
                case Glfw.KeyCode.W:
                {
                    if (m_GammaValue - kStepSize > 0f)
                        SetGamma(window, m_GammaValue - kStepSize);

                    break;
                }
            }
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        public static void Main(string[] args)
        {
            Init();

            int width, height;
            Glfw.Window window;
            Glfw.Monitor monitor = Glfw.Monitor.None;

            var options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.Fullscreen)
                    monitor = Glfw.GetPrimaryMonitor();
            }

            if (!Glfw.Init())
                Environment.Exit(1);

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
                width = 200;
                height = 200;
            }

            window = Glfw.CreateWindow(width, height, "Gamma Test", monitor, Glfw.Window.None);
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            SetGamma(window, 1f);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            Glfw.SetKeyCallback(window, KeyCallback);
            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.Ortho(-1f, 1f, -1f, 1f, -1f, 1f);
            Gl.MatrixMode(MatrixMode.Modelview);

            Gl.ClearColor(0.5f, 0.5f, 0.5f, 0);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                Gl.Color3(0.8f, 0.2f, 0.4f);
                Gl.Rect(-0.5f, -0.5f, 0.5f, 0.5f);

                Glfw.SwapBuffers(window);
                Glfw.WaitEvents();
            }

            Glfw.Terminate();
        }
    }
}
