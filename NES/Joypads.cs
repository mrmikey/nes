using System;
using Tao.Glfw;

namespace NES
{
	public class Joypads
	{
		private int[] readCount = new int[2];
	
	    public enum Key
	    {
	        Up = Glfw.GLFW_KEY_UP,
	        Left = Glfw.GLFW_KEY_LEFT,
	        Down = Glfw.GLFW_KEY_DOWN,
	        Right = Glfw.GLFW_KEY_RIGHT,
	       	A = 97, // ascii a
	       	B = 115,
	       	Select = Glfw.GLFW_KEY_LALT,
	       	Start = Glfw.GLFW_KEY_SPACE,
	        Quit = 81 /* ascii value q */
	    }
	
	    public delegate void KeyEventDelegate(Key Key);
	
		// A, B, Select, Start, Up, Down, Left, Right
		public int[] Keys = new int[] { 97, 115, Glfw.GLFW_KEY_LALT, Glfw.GLFW_KEY_SPACE, Glfw.GLFW_KEY_UP, Glfw.GLFW_KEY_DOWN, Glfw.GLFW_KEY_LEFT, Glfw.GLFW_KEY_RIGHT };
	
		public Joypads()
		{
		}
		
		public event KeyEventDelegate KeyDown;
        public event KeyEventDelegate KeyUp;

        // Not sure how big the array should be.
        private bool[] keys = new bool[65536];
        private Glfw.GLFWkeyfun gkf;

        public bool IsPressed(Key Key)
        {
            return keys[(int)Key];
        }

        public void Initialise()
        {
            gkf = new Glfw.GLFWkeyfun(glfwKeyFun);
            Glfw.glfwSetKeyCallback(gkf);
        }

        public void Deinitialise()
        {
            GC.KeepAlive(gkf);
            gkf = null;
        }

        private void glfwKeyFun(int keyValue, int down)
        {
            keys[keyValue] = (down == 1);

            if (down == 1 && KeyDown != null)
                KeyDown((Key)keyValue);
            else if (down == 0 && KeyUp != null)
                KeyUp((Key)keyValue);

            //Log.i("Key" + (down == 1 ? "Down" : "Up") + ": " + ((Key)keyValue).ToString());
        }
		
		// Read and Writes to $4016/$4017
		public byte Read(int joyNo)
		{
			readCount[joyNo]++;
			if (readCount[joyNo] <= 8)
				return (byte)((keys[Keys[readCount[joyNo]-1]]  ) ? 1 : 0); 
			else if (readCount[joyNo] >= 23) // needed?
			{
				readCount[joyNo] = 0;
				return 0;
			}
			else
				return (byte)0; // then return 1!
		}
		
		public void Write(int joyNo, int val)
		{
			if ((joyNo == 0) && ((val & 1) == 1))
				readCount = new int[2];
		}
		
	}
}
