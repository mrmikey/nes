using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.Glfw;
using System.Drawing;
using Tao.OpenGl;
using System.Threading;
using System.Runtime.Remoting.Contexts;

namespace Clandestine
{
    public static class Graphics
    {
        public static int ScreenWidth = 640;
        public static int ScreenHeight = 400;

        public static bool EatCPU = false;

        public static bool PerformSlowDebugDepthChecking = true;

        /// <summary>
        /// Dictates whether textures with sizes that aren't a power of two are
        /// allowed. If this is set to false, some quick hax will be applied to
        /// work around the lack of capability in the hardware.
        /// (Logic for setting this is in Initialise()).
        /// </summary>
        public static bool NPOTAllowed = true;

        private static Glfw.GLFWwindowclosefun windowCloseFunc;

        public static List<Renderable> Renderables = new List<Renderable>();
        private static bool renderableListNeedsSorting = false;

        // filthy hack
        public static object RenderLockBlob = new object();

        private static Color backgroundColor = Color.CornflowerBlue;

        public static Thread GraphicsThread;

        public static Color BackgroundColor
        {
            get
            {
                lock (RenderLockBlob)
                    return backgroundColor;
            }
            set
            {
                lock (RenderLockBlob)
                    backgroundColor = value;
            }
        }

        public static GLColor GlobalColor = new GLColor(1f, 1f, 1f, 1f);

        internal static void NotifyRenderablesDepthChange()
        {
            renderableListNeedsSorting = true;
        }

        public static bool WindowMinimized
        {
            get { return Glfw.glfwGetWindowParam(Glfw.GLFW_ICONIFIED) == 1; }
        }

        public static void RestoreWindow()
        {
            Glfw.glfwRestoreWindow();
        }

		public static void Initialise(int width, int height)
		{ Initialise(width, height, 1); }
        public static void Initialise(int width, int height, int zoomOverride)
        {
        	Graphics.ScreenWidth = width;
        	Graphics.ScreenHeight = height;
        
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

                    if (vidDesktop.Width < 1280 || vidDesktop.Height < 800)
                    {
                        /* MessageBox.Show("Your screen resolution is only " + vidDesktop.Width.ToString()
                             + "x" + vidDesktop.Height.ToString() + "! The window will be displayed at 1x zoom level, whereas "
                             + "the beautiful and glorious game designers envisioned it for 2x.\n\nAh well. Who cares, right?",
                             "Small Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning);*/
                    }

                    while ((ScreenWidth * (zoomLevel * 2)) <= vidDesktop.Width
                        && (ScreenHeight * (zoomLevel * 2)) <= vidDesktop.Height)
                    {
                        zoomLevel *= 2;
                    }

                    if (Glfw.glfwOpenWindow(zoomLevel * ScreenWidth, zoomLevel * ScreenHeight, 0, 0, 0, 8, 16, 0, Glfw.GLFW_WINDOW) != 1)
                        throw new Exception("Failed to create GLFW window, for whatever reason.");

                }
                else
                {
                    if (Glfw.glfwOpenWindow(zoomOverride * ScreenWidth, zoomOverride * ScreenHeight, 0, 0, 0, 8, 16, 0, Glfw.GLFW_WINDOW) != 1)
                        throw new Exception("Failed to create GLFW window, for whatever reason.");
                }

                windowCloseFunc = new Glfw.GLFWwindowclosefun(OnWindowClose);
                Glfw.glfwSetWindowCloseCallback(windowCloseFunc);

                NPOTAllowed = Gl.glGetString(Gl.GL_EXTENSIONS).ToLower().Split(' ')
                    .Contains("gl_arb_texture_non_power_of_two");
                // Future: Fallback to gl_*_texture_rectangle where possible?

                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glLoadIdentity();
                //Glu.gluOrtho2D(0, ScreenWidth, ScreenHeight, 0);
                Gl.glOrtho(0, ScreenWidth, ScreenHeight, 0, -1000, 1000);
                
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                Gl.glEnable(Gl.GL_BLEND);
            }

            //Texture.SetTexture("Special Tiles", new Texture("specialtiles.png"), false);
        }

        public static void Deinitialise()
        {
            // God, I do despise this bit.
            GC.KeepAlive(windowCloseFunc);
            windowCloseFunc = null;
            Texture.ForceDestroyAllTextures();
            Glfw.glfwCloseWindow();
            Glfw.glfwTerminate();
        }

        private static int OnWindowClose()
        {
            return 1; // Don't close (yet)!
        }

        public static void SetWindowTitle(string windowTitle)
        {
            Glfw.glfwSetWindowTitle(windowTitle);
        }

        private static int chkt = 0; // used so we don't update the FPS reading constantly

        public static void Render()
        {
            DateTime dtStart = DateTime.Now;

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
                
                Gl.glScalef(Camera.Zoom, Camera.Zoom, 1f);
                Gl.glTranslatef(-Camera.Position.X, -Camera.Position.Y, 0f);
                Gl.glRotatef(-Camera.Angle, 0f, 0f, 1f);

                lock (Renderables)
                {
                    Texture.DoDeferredTextureOperations();

                    // ...Because depth testing can't cut it with alpha blending. :(
                    if (renderableListNeedsSorting)
                    {
                        Log.v("SORTING.");
                        renderableListNeedsSorting = false;
                        DateTime dtSortStart = DateTime.Now;
                        Renderables.Sort(new Comparison<Renderable>(compareRenderablesDepths));
                        Log.v("R.Sort took " + (DateTime.Now - dtSortStart).TotalMilliseconds.ToString()
                            + "ms.");
                        
                        // Unsurprisingly, this operation can get slow. :p
                        // Could write an O(n) version, cba!
                        if (PerformSlowDebugDepthChecking)
                        {
                            DateTime dtCheckStart = DateTime.Now;
                            foreach (Renderable r1 in Renderables)
                                foreach (Renderable r2 in Renderables)
                                    checkForDepthProblems(r1, r2);
                            Log.v("checkForDepthProblems took " + (DateTime.Now - dtCheckStart)
                            .TotalMilliseconds.ToString() + "ms.");
                        }
                    }

                    foreach (Renderable r in Renderables)
                        r.Render();
                }

                Glfw.glfwSwapBuffers();
                Glfw.glfwPollEvents();

                DateTime dtEnd = DateTime.Now;

                chkt++;

                if (chkt >= 60)
                {
                    chkt = 0;
                    //Log.i((dtEnd - dtStart).TotalMilliseconds.ToString() + "ms to render");
                }

                //Thread.Sleep(EatCPU ? 0 : 30);
            }

            // This is here, as we don't want a callback and the loop trying to lock stuff.
            // Run stuff that needs to be in graphics thread
            //handleCallbacks();
        }

        struct SpriteWarnPair
        {
            public Sprite s1;
            public Sprite s2;
        }

        private static List<SpriteWarnPair> swp = new List<SpriteWarnPair>();

        private static void checkForDepthProblems(Renderable r1, Renderable r2)
        {
            if (r1 != r2)
                if (r1 is Sprite && r2 is Sprite)
                    if (r1.Layer == r2.Layer)
                    {
                        Sprite s1 = ((Sprite)r1);
                        Sprite s2 = ((Sprite)r2);

                        bool found = false;
                        foreach (SpriteWarnPair sp in swp)
                            if ((sp.s1 == s1 && sp.s2 == s2)
                                || (sp.s2 == s1 && sp.s1 == s2))
                            {
                                found = true;
                            }

                        if (!found)
                        {
                            if (Intersects(new Rectangle(s1.X, s1.Y, s1.Width, s1.Height),
                                new Rectangle(s2.X, s2.Y, s2.Width, s2.Height)))
                            {
                                Log.w("Sprites w/tex: " + s1.GetTexture().Filename + ", "
                                    + s2.GetTexture().Filename);
                                Log.e("Two sprites with depth '" + r1.Layer.ToString() + "' overlap.");
                                Log.e("The sort results are UNDEFINED, and will probably fluctuate!");
                                Log.i("You will not be warned about this sprite overlap pair again.");

                                SpriteWarnPair sw = new SpriteWarnPair();
                                sw.s1 = s1;
                                sw.s2 = s2;
                                swp.Add(sw);
                            }
                        }
                    }
        }

        private static int compareRenderablesDepths(Renderable r1, Renderable r2)
        {
            return r1.Layer.CompareTo(r2.Layer);
        }

        // (stolen & adapted from my falsexna stuff, heh)
        private static bool Intersects(Rectangle tthis, Rectangle value)
        {
            return (tthis.Bottom > value.Top && tthis.Top < value.Bottom && tthis.Left < value.Right
                && tthis.Right > value.Left);
        }
    }
}
