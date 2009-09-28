using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Tao.OpenGl;
using System.Drawing.Imaging;
using Tao.Glfw;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Clandestine
{
    public struct GLColor
    {
        /// <summary>
        /// Range: 0.0 to 1.0
        /// </summary>
        public float R;
        /// <summary>
        /// Range: 0.0 to 1.0
        /// </summary>
        public float G;
        /// <summary>
        /// Range: 0.0 to 1.0
        /// </summary>
        public float B;
        /// <summary>
        /// Range: 0.0 to 1.0
        /// </summary>
        public float A;

        /// <summary>
        /// Creates a GLColor struct, filling it with the supplied values. Each parameter takes a value within the range 0.0 - 1.0.
        /// </summary>
        /// <param name="r">Range: 0.0 to 1.0</param>
        /// <param name="g">Range: 0.0 to 1.0</param>
        /// <param name="b">Range: 0.0 to 1.0</param>
        /// <param name="a">Range: 0.0 to 1.0</param>
        public GLColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    public class Sprite : Renderable
    {
        private object lockBodge = new object(); // splodge bodge dodge wedge dinge

        public float Scale = 1f;
        public float ScaleX = 1f;
        public float ScaleY = 1f;

        public int X = 0;

        public int Y = 0;

        public bool Superbright = false;

        public bool GluedToCamera = false;

        public GLColor Color = new GLColor(1f, 1f, 1f, 1f);

        public bool IgnoreGlobalColor = false;

        public float Rotation = 0f;

        public bool FlipHorizontal = false;

        public Point Origin = new Point(0, 0);

        /// <summary>
        /// Reference the 'Texture' property, and *NOT* this variable!
        /// </summary>
        private Texture texturePropertyStorage;

        protected virtual Texture Texture
        {
            get { return texturePropertyStorage; }
            set { texturePropertyStorage = value; }
        }

        public virtual Point Position
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size UnscaledSize
        {
            get
            { 
                lock (Texture)
                    return Texture.Bitmap.Size; 
            }
        }

        public int UnscaledWidth
        {
            get
            { 
                lock (Texture)
                    return Texture.Bitmap.Width;
            }
        }

        public int UnscaledHeight
        {
            get 
            { 
                lock (Texture)
                    return Texture.Bitmap.Height; 
            }
        }

        public Size Size
        {
            get
            {
                Texture t = Texture;
                lock (t)
                    return new Size((int)(t.Bitmap.Width * ScaleX * Scale),
                        (int)(t.Bitmap.Height * ScaleY * Scale));
            }
        }

        public int Width
        {
            get { return (int)(UnscaledWidth * ScaleX * Scale); }
        }

        public int Height
        {
            get { return (int)(UnscaledHeight * ScaleY * Scale); }
        }

        public static Sprite FromBitmap(Bitmap bmp)
        {
            Sprite sp = new Sprite();
            sp.SetTexture(new Texture(bmp));
            return sp;
        }

        public Texture GetTexture()
        {
            return Texture;
        }

        public void SetTexture(Texture texture)
        {
            this.Texture = texture;
        }

        public Sprite()
            : this("error.png")
        {
        }

        public Sprite(string filename)
        {
            Texture = Texture.GetTexture(filename);
        }

        public Sprite(Texture texture)
        {
            Texture = texture;
        }

        internal override void Render()
        {
            if (!Visible)
                return;

            /*if (!GluedToCamera && (Camera.Position.X + this.Position.X > Graphics.ScreenWidth
                || Camera.Position.X + this.Position.X < -Width
                || Camera.Position.Y + this.Position.Y > Graphics.ScreenHeight
                || Camera.Position.Y + this.Position.Y < -Height))
                return;*/

            lock (lockBodge)
            {
                Texture texture = this.Texture;

                lock (texture)
                {

                    // save state...
                    Gl.glPushMatrix();

                    if (GluedToCamera)
                        Gl.glLoadIdentity();

                    float width = (texture.Bitmap.Width);
                    float height = (texture.Bitmap.Height);

                    //Gl.glTranslatef(X, Y, 0f);
                    Gl.glTranslatef(X - Origin.X, Y - Origin.Y, 0f);
                    Gl.glScalef(Scale * ScaleX, Scale * ScaleY, 1f);
                    Gl.glRotatef(Rotation, 0f, 0f, 1f);

                    //Gl.glEnable(Gl.GL_TEXTURE_2D);

                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.TexPointer);

                    if (!IgnoreGlobalColor)
                    {
                        Gl.glColor4f(Graphics.GlobalColor.R * Color.R,
                            Graphics.GlobalColor.G * Color.G,
                            Graphics.GlobalColor.B * Color.B,
                            Graphics.GlobalColor.A * Color.A);
                    }
                    else
                        Gl.glColor4f(Color.R, Color.G, Color.B, Color.A);

                    if (!Superbright)
                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                    else
                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);

                    float maxU = (texture.PotTextureSize == -1 ? 1f
                        : (((float)texture.Bitmap.Width) / texture.PotTextureSize));
                    float maxV = (texture.PotTextureSize == -1 ? 1f
                        : (((float)texture.Bitmap.Height) / texture.PotTextureSize));

                    // remember chaps, culling order is clockwise.
                    Gl.glBegin(Gl.GL_QUADS);
                    if (FlipHorizontal)
                    {
                        // FLIPPING HORIZONTALLY!
                        // Top-left!
                        Gl.glTexCoord2f(maxU, 0f);
                        Gl.glVertex3f(0, 0, 0f);
                        // Top-right!
                        Gl.glTexCoord2f(0f, 0f);
                        Gl.glVertex3f(width, 0, 0f);
                        // Bottom-right!
                        Gl.glTexCoord2f(0f, maxV);
                        Gl.glVertex3f(width, height, 0f);
                        // Bottom-left!
                        Gl.glTexCoord2f(maxU, maxV);
                        Gl.glVertex3f(0, height, 0f);
                    }
                    else
                    {
                        // DRAWING NORMALLY!
                        // Top-left!
                        Gl.glTexCoord2f(0f, 0f);
                        Gl.glVertex3f(0, 0, 0f);
                        // Top-right!
                        Gl.glTexCoord2f(maxU, 0f);
                        Gl.glVertex3f(width, 0, 0f);
                        // Bottom-right!
                        Gl.glTexCoord2f(maxU, maxV);
                        Gl.glVertex3f(width, height, 0f);
                        // Bottom-left!
                        Gl.glTexCoord2f(0f, maxV);
                        Gl.glVertex3f(0, height, 0f);
                    }
                    // Aaah, done.
                    Gl.glEnd();

                    //Gl.glDisable(Gl.GL_TEXTURE_2D);

                    // ...aaand restore it. Lovely.
                    Gl.glPopMatrix();
                }
            }
        }

    }
}
