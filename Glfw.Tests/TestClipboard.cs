namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This program is used to test the clipboard functionality.
    /// </summary>
    /// <remarks>
    /// Ported from <c>clipboard.c</c>.
    /// </remarks>
    class TestClipboard : TestBase
    {
        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.Escape:
                    Glfw.SetWindowShouldClose(window, true);
                    break;

                case Glfw.KeyCode.V:
                    if (mods == Glfw.KeyMods.Control)
                    {
                        string str = Glfw.GetClipboardString(window);
                        if (!string.IsNullOrEmpty(str))
                            Log("Clipboard contains \"{0}\"", str);
                        else
                            Log("Clipboard does not contain a string");
                    }
                    break;

                case Glfw.KeyCode.C:
                    if (mods == Glfw.KeyMods.Control)
                    {
                        const string str = "Hello GLFW World!";
                        Glfw.SetClipboardString(window, str);
                        Log("Setting clipboard to \"{0}\"", str);
                    }
                    break;
            }
        }

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void Main(string[] args)
        {
            Init();

            Glfw.Window window;

            if (!Glfw.Init())
                Environment.Exit(1);

            window = Glfw.CreateWindow(200, 200, "Clipboard Test");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            Glfw.SetKeyCallback(window, KeyCallback);
            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.Ortho(-1f, 1f, -1f, 1f, 1f, -1f);
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
