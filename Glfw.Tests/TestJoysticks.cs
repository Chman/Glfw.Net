namespace Glfw3.Tests
{
    using OpenGL;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This test displays the state of every button and axis of every connected joystick and/or
    /// gamepad.
    /// </summary>
    class TestJoysticks : TestBase
    {
        static List<Glfw.Joystick> m_Joysticks = new List<Glfw.Joystick>();

        static void FramebufferSizeCallback(Glfw.Window window, int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        static void DrawJoystick(int index, int x, int y, int width, int height)
        {
            int axisHeight = 3 * height / 4;
            int buttonHeight = height / 4;

            var axes = Glfw.GetJoystickAxes(m_Joysticks[index]);
            if (axes != null)
            {
                int axis_width = width / axes.Length;

                for (int i = 0; i < axes.Length; i++)
                {
                    float value = axes[i] / 2f + 0.5f;

                    Gl.Color3(0.3f, 0.3f, 0.3f);
                    Gl.Rect(x + i * axis_width,
                            y,
                            x + (i + 1) * axis_width,
                            y + axisHeight);

                    Gl.Color3(1f, 1f, 1f);
                    Gl.Rect(x + i * axis_width,
                            y + (int)(value * (axisHeight - 5)),
                            x + (i + 1) * axis_width,
                            y + 5 + (int)(value * (axisHeight - 5)));
                }
            }

            var buttons = Glfw.GetJoystickButtons(m_Joysticks[index]);
            if (buttons != null)
            {
                int button_width = width / buttons.Length;

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i])
                        Gl.Color3(1f, 1f, 1f);
                    else
                        Gl.Color3(0.3f, 0.3f, 0.3f);

                    Gl.Rect(x + i * button_width,
                            y + axisHeight,
                            x + (i + 1) * button_width,
                            y + axisHeight + buttonHeight);
                }
            }
        }

        static void DrawJoysticks(Glfw.Window window)
        {
            int width, height;
            Glfw.GetFramebufferSize(window, out width, out height);

            Gl.MatrixMode(MatrixMode.Projection);
            Gl.LoadIdentity();
            Gl.Ortho(0f, width, height, 0f, 1f, -1f);
            Gl.MatrixMode(MatrixMode.Modelview);

            for (int i = 0; i < m_Joysticks.Count; i++)
            {
                DrawJoystick(i,
                             0, i * height / m_Joysticks.Count,
                             width, height / m_Joysticks.Count);
            }
        }

        static void JoystickCallback(Glfw.Joystick joy, Glfw.ConnectionEvent evt)
        {
            if (evt == Glfw.ConnectionEvent.Connected)
            {
                var axisCount = Glfw.GetJoystickAxes(joy).Length;
                var buttonCount = Glfw.GetJoystickButtons(joy).Length;

                Log("Found joystick {0} named \'{1}\' with {2} axes, {3} buttons\n",
                    (int)joy + 1,
                    Glfw.GetJoystickName(joy),
                    axisCount,
                    buttonCount);

                m_Joysticks.Add(joy);
            }
            else if (evt == Glfw.ConnectionEvent.Disconnected)
            {
                m_Joysticks.Remove(joy);
                Log("Lost joystick {0}", (int)joy + 1);
            }
        }

        static void FindJoysticks()
        {
            for (int i = (int)Glfw.Joystick.Joystick1; i <= (int)Glfw.Joystick.JoystickLast; i++)
            {
                var joy = (Glfw.Joystick)i;

                if (Glfw.JoystickPresent(joy))
                    JoystickCallback(joy, Glfw.ConnectionEvent.Connected);
            }
        }

        static void Main(string[] args)
        {
            Init();

            Glfw.Window window;

            if (!Glfw.Init())
                Environment.Exit(1);

            FindJoysticks();
            Glfw.SetJoystickCallback(JoystickCallback);

            window = Glfw.CreateWindow(640, 480, "Joystick Test");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(1);
            }

            Glfw.SetFramebufferSizeCallback(window, FramebufferSizeCallback);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            while (!Glfw.WindowShouldClose(window))
            {
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                DrawJoysticks(window);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            Glfw.Terminate();
        }
    }
}
