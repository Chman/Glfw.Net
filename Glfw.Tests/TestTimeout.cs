namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This test is intended to verify that waiting for events with timeout works.
    /// </summary>
    /// <remarks>
    /// Ported from <c>timeout.c</c>.
    /// </remarks>
    class TestTimeout : TestBase
    {
        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (key == Glfw.KeyCode.Escape && state == Glfw.InputState.Press)
                Glfw.SetWindowShouldClose(window, true);
        }

        static void Main(string[] args)
        {
            Init();

            var rand = new Random();
            Glfw.Window window;

            if (!Glfw.Init())
                Environment.Exit(1);

            window = Glfw.CreateWindow(640, 480, "Event Wait Timeout Test");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);
            Glfw.SetKeyCallback(window, KeyCallback);

            while (!Glfw.WindowShouldClose(window))
            {
                int width, height;
                double r = rand.NextDouble(), g = rand.NextDouble(), b = rand.NextDouble();
                double l = Math.Sqrt(r * r + g * g + b * b);

                Glfw.GetFramebufferSize(window, out width, out height);

                Gl.Viewport(0, 0, width, height);
                Gl.ClearColor((float)(r / l), (float)(g / l), (float)(b / l), 1f);
                Gl.Clear(ClearBufferMask.ColorBufferBit);
                Glfw.SwapBuffers(window);

                Glfw.WaitEventsTimeout(1.0);
            }

            Glfw.DestroyWindow(window);

            Glfw.Terminate();
        }
    }
}
