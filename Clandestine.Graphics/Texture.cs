using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using System.IO;

namespace Clandestine
{
    public class Texture
    {
        private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        public Bitmap Bitmap
        {
            get
            {
                lock (bitmap)
                    return bitmap;
            }
            set
            {
                lock (bitmap)
                    bitmap = value;
            }
        }

        public Bitmap bitmap = null;
        //public int RefCount = 0; // Disabling ref-counting -- more trouble than it's worth!
        public int TexPointer = 0;
        public string Filename = "";

        private int potTextureSize = -1;

        public int PotTextureSize
        {
            get { return potTextureSize; }
        }

        private bool needsDestroy = false;

        // gah, can't do static this[] properties. very annoying...

        public static bool TextureExists(string name)
        {
            name = name.ToLower();

            lock (textures)
                return (textures.Keys.Contains(name));
        }

        public static Texture GetTexture(string name)
        {
            string lname = name.ToLower();

            lock (textures)
            {
                if (textures.Keys.Contains(lname))
                    return textures[lname];
                else
                {
                    if (File.Exists(name))
                        return new Texture(name);
                    else
                    {
                        Log.e("Texture.GetTexture(): No file with name '" + name
                            + "' exists on disk!");
                        return Texture.GetTexture("error.png");
                    }
                }
            }
        }

        /// <summary>
        /// Either changes the name of an existing texture (and it's entry in the global
        /// texture array), or creates a new texture sharing the data of the existing one
        /// but with a different name. (Bitmap object and GL graphics memory is shared.)
        /// 
        /// This call is NOT necessary in ordinary usage! EVERY Texture gets put in the global
        /// texture array once created, even if the name assigned is just a Guid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        /// <param name="makeCopy"></param>
        /// <remarks>ALL Textures already exist in the global texture array!
        /// This call is NOT necessary in ordinary Texture usage!
        /// Textures created using the new Texture(Bitmap) constructor have a GUID assigned.</remarks>
        public static void SetTexture(string name, Texture texture, bool makeCopy)
        {
            name = name.ToLower();

            lock (textures)
            {
                Texture t;

                if (makeCopy)
                {
                    // Copy the bastard, and change the name.
                    t = new Texture();
                    t.bitmap = texture.bitmap;
                    t.TexPointer = texture.TexPointer;
                    t.Filename = name;

                    if (textures.Keys.Contains(name))
                    {
                        Log.w("A texture with name '" + name + "' is already present - replacing.");
                        textures.Remove(name);
                    }
                }
                else
                {
                    textures.Remove(texture.Filename);
                    texture.Filename = name;
                    t = texture;
                }

                if (textures.ContainsKey(name))
                {
                    textures.Remove(name);
                    Log.v("Replacing texture '" + name + "' with new one. This is rarely"
                        + " a good thing.");
                }
                textures.Add(name, t);
            }
        }

        public static void ForceDestroyAllTextures()
        {
            lock (textures)
            {
                foreach (Texture t in textures.Values.ToArray())
                    t.Destroy();
            }
        }

        private Texture() { }

        public Texture(string bitmapFilename)
        {
            lock (textures)
            {
                this.Filename = bitmapFilename.ToLower();

                if (textures.Keys.Contains(Filename))
                {
                    // Already exists, hm.
                    Log.w("Internal Texture constructor called with the purpose of creating a new texture only, but a texture"
                        + " with this filename exists in the textures list!");
                    Log.w("I will copy it's texture pointer and bitmap, grudgingly.");
                    this.bitmap = textures[Filename].bitmap;
                    this.TexPointer = textures[Filename].TexPointer;

                    // in case it didn't have a texture pointer -- in which case, we would be the more desirable
                    // option. ('this' is added to the textures array below)
                    textures.Remove(Filename);
                }
                else
                {
                    this.bitmap = ChromaKeyIfNecessary(new Bitmap(bitmapFilename), false);
                    this.TexPointer = 0;
                }

                if (this.TexPointer == 0)
                {
                    // createNewGlTexture() sets .TexPointer
                    if (Thread.CurrentThread == Graphics.GraphicsThread)
                        lock (Graphics.RenderLockBlob)
                            createNewGlTexture();

                    // Otherwise, it'll be caught and sorted out by DoDeferredOperations()
                    // and all that.
                }

                textures.Add(Filename, this);
            }
        }

        public Texture(Bitmap bitmap) : this(bitmap, false) { }

        public Texture(Bitmap bitmap, bool treatAsIndexed) : this(System.Guid.NewGuid().ToString(), bitmap, treatAsIndexed) { }

        public Texture(string identifier, Bitmap bitmap) : this(identifier, bitmap, false) { }

        public Texture(string identifier, Bitmap bitmap, bool treatAsIndexed)
        {
            identifier = identifier.ToLower();
            
            lock (textures)
            {
                if (textures.Keys.Contains(identifier))
                {
                    Log.w("Textures list already contains a texture called '" + identifier + "' -- displacing.");
                    textures.Remove(identifier);
                }

                this.bitmap = ChromaKeyIfNecessary(bitmap, treatAsIndexed);
                this.Filename = identifier;

                // createNewGlTexture() sets .TexPointer
            }
           
           	if (Thread.CurrentThread == Graphics.GraphicsThread)
            	lock (Graphics.RenderLockBlob)
            		createNewGlTexture();

            lock (textures)
                textures.Add(identifier, this);
            
        }

        private static Dictionary<Bitmap, Bitmap> resizeResultDictionary
            = new Dictionary<Bitmap, Bitmap>();

        private Bitmap resizeIfNecessary(Bitmap bmpOriginal)
        {
            if (Graphics.NPOTAllowed)
                return bmpOriginal;
            else
            {
                Log.i("This hardware does not support non power-of-two textures - hax will be applied.");

                lock (resizeResultDictionary)
                {
                    // Aye, we do cache 'em...
                    if (resizeResultDictionary.Keys.Contains(bmpOriginal))
                        return resizeResultDictionary[bmpOriginal];
                    else
                    {
                        int referencesize = (bmpOriginal.Width > bmpOriginal.Height
                            ? bmpOriginal.Width : bmpOriginal.Height);
                        int newsize = 2;

                        // Find next bigger POT size.
                        for (int i = 1; newsize < referencesize; i++)
                            newsize = (int)Math.Round(Math.Pow(2, i));

                        potTextureSize = newsize;

                        // Resize bitmap, and return the result!
                        Bitmap bmpResult = new Bitmap(newsize, newsize);
                        System.Drawing.Graphics gRes = System.Drawing.Graphics.FromImage(bmpResult);
                        gRes.DrawImage(bmpOriginal, 0, 0);
                        gRes.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
                        gRes.Dispose();
                        resizeResultDictionary.Add(bmpOriginal, bmpResult);
                        return bmpResult;
                    }
                }
            }
        }

        public void createNewGlTexture()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glGenTextures(1, out TexPointer);

            if (TexPointer == 0)
                throw new Exception("The OpenGL context failed to create a new texture.");

            lock (this.bitmap)
            {
                Bitmap bmpTex = resizeIfNecessary(this.bitmap);
                BitmapData bd = bmpTex.LockBits(new Rectangle(0, 0, bmpTex.Width, bmpTex.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				Gl.glDeleteTextures(1, ref TexPointer);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, TexPointer);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST); // THE
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST); // PIXELS!

                // Actually upload the data.
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, bmpTex.Width, bmpTex.Height,
                    0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bd.Scan0);

                bmpTex.UnlockBits(bd);
            }
        }

        private static Bitmap ChromaKeyIfNecessary(Bitmap bmp, bool treatAsIndexed)
        {
            if (treatAsIndexed || (bmp.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                if (treatAsIndexed)
                    Log.v("treatAsIndexed=true");
                Log.v("Got an indexed bitmap. Doing chroma-keying with zelda-pink chroma. (225,0,126)");
                Bitmap bmpNew = ConvertTo32bppBitmap(bmp);
                bmp.Dispose();
                bmp = bmpNew;
                ChromaKey32bppBitmap(bmp, Color.FromArgb(225, 0, 126));
                return bmp;
            }

            // If 24bpp, chroma-key based on pixel at coords (0,0) (top-left)
            if (!((bmp.PixelFormat & PixelFormat.Alpha) == PixelFormat.Alpha))
            {
                DateTime dtConvStart = DateTime.Now;

                // ah. chroma-keying time! what fun.
                Bitmap bmpNew = ConvertTo32bppBitmap(bmp);
                bmp.Dispose();

                bmp = bmpNew;

                ChromaKey32bppBitmap(bmp, bmp.GetPixel(0, 0));

                DateTime dtConvEnd = DateTime.Now;
                Log.i("Bitmap chroma-keying of bitmap of size " + bmp.Size.ToString()
                    + " took " + (dtConvEnd - dtConvStart).TotalMilliseconds.ToString()
                    + "ms.");
            }

            return bmp;
        }

        private static Bitmap ConvertTo32bppBitmap(Bitmap bmp)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

            BitmapData bdRead = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size),
                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bdWrite = newBmp.LockBits(new Rectangle(Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Temporary buffer ugh
            byte[] xbuf = new byte[bdRead.Stride * bdRead.Height];
            Marshal.Copy(bdRead.Scan0, xbuf, 0, bdRead.Stride * bdRead.Height);
            Marshal.Copy(xbuf, 0, bdWrite.Scan0, bdRead.Stride * bdRead.Height);

            bmp.UnlockBits(bdRead);
            newBmp.UnlockBits(bdWrite);

            return newBmp;
        }

        private static void ChromaKey32bppBitmap(Bitmap bmp, Color chroma)
        {
            BitmapData bd = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size),
                        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                int len = bd.Stride * bd.Height;

                byte* ptr = (byte*)bd.Scan0.ToPointer();

                byte* ptrStop = ptr + len;

                while (ptr < ptrStop)
                {
                    if (*ptr == chroma.B && *(ptr + 1) == chroma.G
                        && *(ptr + 2) == chroma.R)
                        *(ptr + 3) = 0; // ...then set the alpha to 0

                    ptr += 4;
                }
            }

            bmp.UnlockBits(bd);
        }

        public static void DoDeferredTextureOperations()
        {
            lock (textures)
                foreach (Texture t in textures.Values.ToArray())
                    t.doDeferredOperations();
        }

        private void doDeferredOperations()
        {
            if (this.TexPointer == 0)
                lock (Graphics.RenderLockBlob)
                {
                    Log.v("Doing deferred texture operation (creation) on " + this.Filename);
                    createNewGlTexture();
                }

            if (needsDestroy)
            {
                doDestroy();
                needsDestroy = false;
            }
        }

        public void Destroy()
        {
            if (Thread.CurrentThread == Graphics.GraphicsThread)
                doDestroy();
            else
                needsDestroy = true;
        }

        private void doDestroy()
        {
            lock (Graphics.RenderLockBlob)
            {
                if (TexPointer != 0)
                {
                    Gl.glDeleteTextures(1, ref TexPointer);
                    TexPointer = 0;
                }
            }
            lock (textures)
                textures.Remove(this.Filename);
        }

        ~Texture()
        {
            Destroy();
        }


    }
}
