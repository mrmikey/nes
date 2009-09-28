private uint rmask, gmask, bmask, amask; // probably not needed
		private Sdl.SDL_Rect screenRect;
		private IntPtr screenSurfacePtr;
		private Sdl.SDL_Surface screenSurface;
		private IntPtr sdlBuffer;
		private int[] pixelBuffer;
		private const int bpp = 32;
		public int Width,Height;
		
		public Graphics(int width, int height)
		{
			Width = width;
			Height = height;
		
		    // Init SDL and check for error.
            if (Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO) != 0)
                throw new Exception();

            // Set video mode and check for error.
            screenRect = new Sdl.SDL_Rect((short)0, (short)0, (short)(width), (short)(height));
            screenSurfacePtr = Sdl.SDL_SetVideoMode(width, height, bpp, Sdl.SDL_HWSURFACE | Sdl.SDL_DOUBLEBUF);
            if (screenSurfacePtr == null)
                throw new Exception(Sdl.SDL_GetError());
			Console.WriteLine(Sdl.SDL_GetError());
			// Neat! Get the ACTUAL surface none of this pointer shite!
			screenSurface = (Sdl.SDL_Surface)Marshal.PtrToStructure(screenSurfacePtr, typeof(Sdl.SDL_Surface));
			sdlBuffer = screenSurface.pixels;
			
			// Off screen buffer
			pixelBuffer = new int[width*height];
			for (int i = 0; i < 100; i++)
				DrawPixel(i, 5, 0, 0, 0);
			
            // Blah, mask shite. Starting to dislike SDL...
            if (Sdl.SDL_BYTEORDER == Sdl.SDL_BIG_ENDIAN)
            {
                rmask = 0xff000000;
                gmask = 0x00ff0000;
                bmask = 0x0000ff00;
                amask = 0x000000ff;
            }
            else {
                rmask = 0x000000ff;
                gmask = 0x0000ff00;
                bmask = 0x00ff0000;
                amask = 0xff000000;
            }
        }
        
    	public void DrawPixel(int x, int y, int r, int g, int b)
    	{
    		//int color = Sdl.SDL_MapRGB(screenSurface.format, (byte)r, (byte)g, (byte)b);
    		int color = 55;
    		if (Sdl.SDL_MUSTLOCK(screenSurfacePtr) == 1)
    			Sdl.SDL_LockSurface(screenSurfacePtr);
    			
    		
    		unsafe {
    			int *pixmem32 = (int *)sdlBuffer + y + x;
    			*pixmem32 = color;
    		}
    		
    		if (Sdl.SDL_MUSTLOCK(screenSurfacePtr) == 1)
    			Sdl.SDL_UnlockSurface(screenSurfacePtr);
    		Sdl.SDL_Flip(screenSurfacePtr);
    	}
		
		public void BlitScreen()
		{
			int j, f;
			
			for (j = 0; j < 1000; j++)
			{
				int myPixelColor;
				unsafe {
					
					int *p = (int *)sdlBuffer;
					int maxlength = Width*Height;
					
					for (f = 0; f < maxlength; f++)
					{
						myPixelColor =  pixelBuffer[f];
						p[f] = myPixelColor;
					}
				}
				Sdl.SDL_Flip(screenSurfacePtr);
			}
		
		}
		
		public void DrawTile(byte[] src, int start_addr)
		{
			//Sdl.SDL_S
		}
		