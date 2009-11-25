using System;
using System.Runtime.InteropServices;
using Tao.Glfw;
using Tao.OpenGl;
using System.Drawing;
using System.Threading;	
using System.Linq;
using System.Text;

namespace NES
{
	public class Graphics
	{
		private Engine Engine;
		public int Width, Height;
		public Thread GraphicsThread;
		public object RenderLockBlob = new object();
		private Glfw.GLFWwindowclosefun windowCloseFunc;
		private bool NPOTAllowed;
		private int debugCount;
		private Color backgroundColor = Color.Black;
		public int[] ScreenBuffer;
		
		public int[] BGBuffer;
		public int BGPointer;
		public int[] SprHighBuffer;
		public int SprHighPointer;
		public int[] SprLowBuffer;
		public int SprLowPointer;
		
		//public int TexturePointer; // ptr to our screen texture
		public Color[] TempPalette = new Color[] { Color.Red, Color.Green, Color.Blue, Color.White };

		public Graphics(Engine engine, int width, int height)
		{
			Engine = engine;
			Width = width;
			Height = height;
			//Screen = new int[Width * Height];
			BGBuffer =  new int[(Width)*(Height)];
			SprHighBuffer = new int[(Width)*(Height)];
			SprLowBuffer = new int[(Width)*(Height)];
			// Debug
			/*int offset = width * (height/2);
			for (int x = 0; x < width; x++)
				Screen[offset + x] = Color.Blue.ToArgb();*/
		
			Initialise(1);

			Render();
		}		
		
		public void Initialise(int zoomOverride)
		{
            lock (RenderLockBlob)
            {
                GraphicsThread = Thread.CurrentThread;

                if (Glfw.glfwInit() != 1)
                    throw new Exception("GLFW initialisation failed. No, I have no idea why either.");

                Glfw.GLFWvidmode vidDesktop = new Glfw.GLFWvidmode(); // not necessary b/c struct but whatever, man!
                Glfw.glfwGetDesktopMode(out vidDesktop);

                if (zoomOverride == 0)
                {
                    int zoomLevel = 1;

                    while ((Width * (zoomLevel * 2)) <= vidDesktop.Width
                        && (Height * (zoomLevel * 2)) <= vidDesktop.Height)
                    {
                        zoomLevel *= 2;
                    }

                    if (Glfw.glfwOpenWindow(zoomLevel * Width, zoomLevel * Height, 0, 0, 0, 8, 16, 0, Glfw.GLFW_WINDOW) != 1)
                        throw new Exception("Failed to create GLFW window, for whatever reason.");

                }
                else
                {
                    if (Glfw.glfwOpenWindow(zoomOverride * Width, zoomOverride * Height, 0, 0, 0, 8, 16, 0, Glfw.GLFW_WINDOW) != 1)
                        throw new Exception("Failed to create GLFW window, for whatever reason.");
                }

                windowCloseFunc = new Glfw.GLFWwindowclosefun(OnWindowClose);
                Glfw.glfwSetWindowCloseCallback(windowCloseFunc);

                NPOTAllowed = Gl.glGetString(Gl.GL_EXTENSIONS).ToLower().Split(' ')
                    .Contains("gl_arb_texture_non_power_of_two");
                // Future: Fallback to gl_*_texture_rectangle where possible?

                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                //Glu.gluOrtho2D(0, Width, Height, 0);
                Gl.glOrtho(0, Width, Height, 0, -1000, 1000);
                
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                Gl.glEnable(Gl.GL_BLEND);
            }
		}
		
		public void Deinitialise()
        {
            // God, I do despise this bit.
            GC.KeepAlive(windowCloseFunc);
            windowCloseFunc = null;
        	// Todo: Destroy textures!
            Glfw.glfwCloseWindow();
            Glfw.glfwTerminate();
        }
        
        public void DrawTile(byte[] bank, int[] buffer, int tileNo, int X, int Y, int attribute)
        {
        	int offset = tileNo * 16;
        	for (int y = 0; y < 8; y++)
        	{
        		byte scanLower = bank[offset + y + 8];
        		byte scanUpper = bank[offset + y];
        		for (int x = 0; x < 8; x++)
        		{
        			byte lower = (byte)((scanLower >> (7 ^ x)) & 1);
        			byte upper = (byte)(((scanUpper >> (7 ^ x)) & 1) << 1);
        			int palIndex = lower | upper | attribute;
        			int color = Engine.PPU.Palette[Engine.PPU.ReadMemory8((ushort)(0x3F00 | palIndex))];
	        		
	        		// Can draw outside range
	        		try
	        		{
	        			buffer[((y+Y)*Width)+(x+X)] = ((lower|upper) == 0) ? 0 : (int)(color|0xFF000000);
	        		}
	        		catch (IndexOutOfRangeException e)
	        		{
	        			Log.w("Ignoring drawing done out of bounds.");
	        		}
        		}
        	}

        }
        
        
        private DateTime lastRender;
        
        public void Render()
        {
            DateTime dtStart = DateTime.Now;
			//Console.WriteLine("{0} milliseconds since last render.", (DateTime.Now - lastRender).TotalMilliseconds);
			lastRender = dtStart;
			
		//	Console.WriteLine(String.Format("{0:x} {1:x}", Engine.ReadMemory8(0x00), Engine.ReadMemory8(0x44)));
			// Draw tile table 1
			/*int count = 0; 
			for (int y = 0; y < 16; y++)
				for (int x = 0; x < 16; x++)
					DrawTile(Engine.Cartridge.CHRBanks[0], count++, x*8, y*8, 0);
					//DrawTileFromCache(Engine.PPU.CHRCache, count++, x, y, 0);
			
			// Draw tile table 2
			for (int y = 0; y < 16; y++)
				for (int x = 0; x < 16; x++)
					DrawTile(Engine.Cartridge.CHRBanks[0], count++, 128 + (x*8), (y*8), 0);
					//DrawTileFromCache(Engine.PPU.CHRCache, count++, x, y, 0);		
			*/
		//	SwapBuffer();
			
            // Gl blah blah
            lock (RenderLockBlob)
            {
                int gerr = Gl.glGetError();
                if (gerr != 0)
                    Log.w("glGetError():: " + Glu.gluErrorString(gerr));

                // How inefficient is this?
                // (not rhetorical question, seriously -- does it matter?)
                int WindowWidth, WindowHeight;
                Glfw.glfwGetWindowSize(out WindowWidth, out WindowHeight);
                Gl.glViewport(0, 0, WindowWidth, WindowHeight);

                // clear background (no, really?)
                Gl.glClearColor(((float) backgroundColor.R) / 255f, 
                    ((float) backgroundColor.G / 255f),
                    ((float) backgroundColor.B / 255f), ((float) backgroundColor.A / 255f));
                Gl.glClearDepth(double.MinValue);
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

                // Set-up the modelview matrix again.
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();
                
                // Think this does nothing.. used to be camera code
                Gl.glScalef(1f, 1f, 1f);
                Gl.glTranslatef(0f, 0f, 0f);
                Gl.glRotatef(0f, 0f, 0f, 1f);

                // DO RENDER HERE :)
				sortTexture(ref SprLowBuffer, ref SprLowPointer);
				sortTexture(ref BGBuffer, ref BGPointer);
				sortTexture(ref SprHighBuffer, ref SprHighPointer);
				renderScreen(SprLowPointer);
				renderScreen(BGPointer);
				renderScreen(SprHighPointer);
				
				// Finished, swap buffers
                Glfw.glfwSwapBuffers();
                Glfw.glfwPollEvents();

				// Debug
                DateTime dtEnd = DateTime.Now;
                 //   Console.WriteLine((dtEnd - dtStart).TotalMilliseconds.ToString() + "ms to render");
            }
        }
                
        // function that renders each screen, sprlow, bg, sprhigh
        private void renderScreen(int pointer)
        {
	        // save state...
	        Gl.glPushMatrix();
	
			// Don't think I needs this...
	        Gl.glTranslatef(0f, 0f, 0f);
	        Gl.glScalef(1f, 1f, 1f);
	        Gl.glRotatef(0f, 0f, 0f, 1f);
	
			// Setups!
	        Gl.glBindTexture(Gl.GL_TEXTURE_2D, pointer);
	        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
	
	        // remember chaps, culling order is clockwise.
	        Gl.glBegin(Gl.GL_QUADS);
	            // DRAWING NORMALLY!
	            // Top-left!
	            Gl.glTexCoord2f(0f, 0f);
	            Gl.glVertex3f(0, 0, 0f);
	            // Top-right!
	            Gl.glTexCoord2f(1f, 0f);
	            Gl.glVertex3f(Width, 0, 0f);
	            // Bottom-right!
	            Gl.glTexCoord2f(1f, 1f);
	            Gl.glVertex3f(Width, Height, 0f);
	            // Bottom-left!
	            Gl.glTexCoord2f(0f, 1f);
	            Gl.glVertex3f(0, Height, 0f);
	        Gl.glEnd();
	
	        // ...aaand restore it. Lovely.
	        Gl.glPopMatrix();
        }
        
        private unsafe void sortTexture(ref int[] buffer, ref int pointer)
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
			
			// Delete the old one!
			Gl.glDeleteTextures(1, ref pointer);
			pointer = -1;
			
			// Try making a texture
            Gl.glGenTextures(1, out pointer);
            if (pointer == 0)
                throw new Exception("The OpenGL context failed to create a new texture.");

			// Setup some Gl shizzle.
	        Gl.glBindTexture(Gl.GL_TEXTURE_2D, pointer);
	        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
	        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
	        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST); // THE
	        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST); // PIXELS!
	
	        // Actually upload the data.
	        fixed (int* ptrScreen = buffer)
		    {
		    	Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, Width, Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, new IntPtr((void*)ptrScreen));
       	    }
       	    buffer = new int[(Width)*(Height)];
        }

        private int OnWindowClose()
        {
        	Engine.Running = false;
            return 1; // Do stuff
        }
		
	}
}


