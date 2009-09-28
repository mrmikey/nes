using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clandestine
{
    public abstract class Renderable
    {
        public bool Visible = true;

        private float layer = 0f;

        public float Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                Graphics.NotifyRenderablesDepthChange();
            }
        }

        internal abstract void Render();

        public Renderable()
        {
            lock (Graphics.Renderables)
            {
                Graphics.Renderables.Add(this);
                Graphics.NotifyRenderablesDepthChange();
            }
        }

        ~Renderable() 
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            lock (Graphics.Renderables)
                if (Graphics.Renderables.Contains(this))
                    Graphics.Renderables.Remove(this);
        }
    }
}
