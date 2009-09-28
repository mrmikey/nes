using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.Glfw;

namespace Clandestine
{
    public enum Key
    {
        Up = Glfw.GLFW_KEY_UP,
        Left = Glfw.GLFW_KEY_LEFT,
        Down = Glfw.GLFW_KEY_DOWN,
        Right = Glfw.GLFW_KEY_RIGHT,
        Select = Glfw.GLFW_KEY_ENTER, 
        Menu = Glfw.GLFW_KEY_ESC,
        Cancel = Glfw.GLFW_KEY_SPACE,
        Quit = 81 /* ascii value q */
    }

    public delegate void KeyEventDelegate(Key Key);

    public static class Keyboard
    {
        public static event KeyEventDelegate KeyDown;
        public static event KeyEventDelegate KeyUp;

        // Not sure how big the array should be.
        private static bool[] keys = new bool[65536];
        private static Glfw.GLFWkeyfun gkf;

        public static bool IsPressed(Key Key)
        {
            return keys[(int)Key];
        }

        /// <summary>
        /// Call Graphics.Initialise() before this.
        /// </summary>
        public static void Initialise()
        {
            gkf = new Glfw.GLFWkeyfun(glfwKeyFun);
            Glfw.glfwSetKeyCallback(gkf);
        }

        public static void Deinitialise()
        {
            GC.KeepAlive(gkf);
            gkf = null;
        }

        private static void glfwKeyFun(int keyValue, int down)
        {
            keys[keyValue] = (down == 1);

            if (down == 1 && KeyDown != null)
                KeyDown((Key)keyValue);
            else if (down == 0 && KeyUp != null)
                KeyUp((Key)keyValue);

            //Log.i("Key" + (down == 1 ? "Down" : "Up") + ": " + ((Key)keyValue).ToString());
        }
    }
}
