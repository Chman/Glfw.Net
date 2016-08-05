namespace Glfw3.Tests
{
    using OpenGL;
    using System;

    /// <summary>
    /// This test provides an interface to the cursor image and cursor mode parts of the API. Custom
    /// cursor image generation by urraka.
    /// </summary>
    /// <remarks>
    /// Ported from <c>cursor.c</c>.
    /// </remarks>
    class TestCursor : TestBase
    {
        const int CURSOR_FRAME_COUNT = 60;
        
        static double cursorX;
        static double cursorY;
        static int swapInterval = 1;
        static bool waitEvents = true;
        static bool animateCursor = false;
        static bool trackCursor = false;
        static Glfw.Cursor[] standardCursors;

        static float Star(int x, int y, float t)
        {
            const float c = 64 / 2f;

            float i = (0.25f * (float)Math.Sin(2f * Math.PI * t) + 0.75f);
            float k = 64 * 0.046875f * i;

            float dist = (float)Math.Sqrt((x - c) * (x - c) + (y - c) * (y - c));

            float salpha = 1f - dist / c;
            float xalpha = (float)x == c ? c : k / Math.Abs(x - c);
            float yalpha = (float)y == c ? c : k / Math.Abs(y - c);

            return Math.Max(0f, Math.Min(1f, i * salpha * 0.2f + salpha * xalpha * yalpha));
        }

        static Glfw.Cursor CreateCursorFrame(float t)
        {
            int i = 0, x, y;
            var buffer = new byte[64 * 64 * 4];

            for (y = 0;  y < 64;  y++)
            {
                for (x = 0;  x < 64;  x++)
                {
                    buffer[i++] = 255;
                    buffer[i++] = 255;
                    buffer[i++] = 255;
                    buffer[i++] = (byte)(255 * Star(x, y, t));
                }
            }

            var image = new Glfw.Image
            {
                Width = 64,
                Height = 64,
                Pixels = buffer
            };

            return Glfw.CreateCursor(image, image.Width / 2, image.Height / 2);
        }

        static void CursorPositionCallback(Glfw.Window window, double x, double y)
        {
            Log("{0}: Cursor position: {1} {2} ({3} {4})",
                   Glfw.GetTime(),
                   x, y, x - cursorX, y - cursorY);

            cursorX = x;
            cursorY = y;
        }

        static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
        {
            if (state != Glfw.InputState.Press)
                return;

            switch (key)
            {
                case Glfw.KeyCode.A:
                {
                    animateCursor = !animateCursor;
                    if (!animateCursor)
                        Glfw.SetCursor(window, Glfw.Cursor.None);

                    break;
                }

                case Glfw.KeyCode.Escape:
                {
                    Glfw.SetWindowShouldClose(window, true);
                    break;
                }

                case Glfw.KeyCode.N:
                    Glfw.SetInputMode(window, Glfw.InputMode.Cursor, Glfw.CursorMode.Normal);
                    Log("(( cursor is normal ))");
                    break;

                case Glfw.KeyCode.D:
                    Glfw.SetInputMode(window, Glfw.InputMode.Cursor, Glfw.CursorMode.Disabled);
                    Log("(( cursor is disabled ))");
                    break;

                case Glfw.KeyCode.H:
                    Glfw.SetInputMode(window, Glfw.InputMode.Cursor, Glfw.CursorMode.Hidden);
                    Log("(( cursor is hidden ))");
                    break;

                case Glfw.KeyCode.Space:
                    swapInterval = 1 - swapInterval;
                    Log("(( swap interval: {0} ))", swapInterval);
                    Glfw.SwapInterval(swapInterval);
                    break;

                case Glfw.KeyCode.W:
                    waitEvents = !waitEvents;
                    Log("(( {0}ing for events ))", waitEvents ? "wait" : "poll");
                    break;

                case Glfw.KeyCode.T:
                    trackCursor = !trackCursor;
                    break;

                case Glfw.KeyCode.Alpha0:
                    Glfw.SetCursor(window, Glfw.Cursor.None);
                    break;

                case Glfw.KeyCode.Alpha1:
                    Glfw.SetCursor(window, standardCursors[0]);
                    break;

                case Glfw.KeyCode.Alpha2:
                    Glfw.SetCursor(window, standardCursors[1]);
                    break;

                case Glfw.KeyCode.Alpha3:
                    Glfw.SetCursor(window, standardCursors[2]);
                    break;

                case Glfw.KeyCode.Alpha4:
                    Glfw.SetCursor(window, standardCursors[3]);
                    break;

                case Glfw.KeyCode.Alpha5:
                    Glfw.SetCursor(window, standardCursors[4]);
                    break;

                case Glfw.KeyCode.Alpha6:
                    Glfw.SetCursor(window, standardCursors[5]);
                    break;
            }
        }

        static void Main(string[] args)
        {
            Init();

            Glfw.Window window;
            var starCursors = new Glfw.Cursor[CURSOR_FRAME_COUNT];
            Glfw.Cursor currentFrame = Glfw.Cursor.None;

            if (!Glfw.Init())
                Environment.Exit(1);

            for (int i = 0; i < CURSOR_FRAME_COUNT; i++)
            {
                starCursors[i] = CreateCursorFrame(i / (float)CURSOR_FRAME_COUNT);
                if (!starCursors[i])
                {
                    Glfw.Terminate();
                    Environment.Exit(1);
                }
            }

            Glfw.CursorType[] shapes =
            {
                Glfw.CursorType.Arrow,
                Glfw.CursorType.Beam,
                Glfw.CursorType.Crosshair,
                Glfw.CursorType.Hand,
                Glfw.CursorType.ResizeX,
                Glfw.CursorType.ResizeY
            };

            standardCursors = new Glfw.Cursor[6];

            for (int i = 0; i < standardCursors.Length; i++)
            {
                standardCursors[i] = Glfw.CreateStandardCursor(shapes[i]);

                if (!standardCursors[i])
                {
                    Glfw.Terminate();
                    Environment.Exit(1);
                }
            }

            window = Glfw.CreateWindow(640, 480, "Cursor Test");

            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.MakeContextCurrent(window);

            Glfw.GetCursorPos(window, out cursorX, out cursorY);
            Log("Cursor position: {0} {1}", cursorX, cursorY);

            Glfw.SetCursorPosCallback(window, CursorPositionCallback);
            Glfw.SetKeyCallback(window, KeyCallback);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                if (trackCursor)
                {
                    int wnd_width, wnd_height, fb_width, fb_height;
                    float scale;

                    Glfw.GetWindowSize(window, out wnd_width, out wnd_height);
                    Glfw.GetFramebufferSize(window, out fb_width, out fb_height);

                    scale = (float)fb_width / (float)wnd_width;

                    Gl.Viewport(0, 0, fb_width, fb_height);

                    Gl.MatrixMode(MatrixMode.Projection);
                    Gl.LoadIdentity();
                    Gl.Ortho(0f, fb_width, 0f, fb_height, 0f, 1f);

                    Gl.Begin(PrimitiveType.Lines);
                    Gl.Vertex2(0f, (float)(fb_height - cursorY * scale));
                    Gl.Vertex2((float)fb_width, (float)(fb_height - cursorY * scale));
                    Gl.Vertex2((float)cursorX * scale, 0f);
                    Gl.Vertex2((float)cursorX * scale, (float)fb_height);
                    Gl.End();
                }

                Glfw.SwapBuffers(window);

                if (animateCursor)
                {
                    int i = (int)(Glfw.GetTime() * 30.0) % CURSOR_FRAME_COUNT;

                    if (currentFrame != starCursors[i])
                    {
                        Glfw.SetCursor(window, starCursors[i]);
                        currentFrame = starCursors[i];
                    }
                }
                else currentFrame = Glfw.Cursor.None;

                if (waitEvents)
                {
                    if (animateCursor)
                        Glfw.WaitEventsTimeout(1.0 / 30.0);
                    else
                        Glfw.WaitEvents();
                }
                else Glfw.PollEvents();
            }

            Glfw.DestroyWindow(window);

            for (int i = 0; i < CURSOR_FRAME_COUNT; i++)
                Glfw.DestroyCursor(starCursors[i]);

            for (int i = 0; i < standardCursors.Length; i++)
                Glfw.DestroyCursor(standardCursors[i]);

            Glfw.Terminate();
        }
    }
}
