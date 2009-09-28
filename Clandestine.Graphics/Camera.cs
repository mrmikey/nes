using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Clandestine
{
    public static class Camera
    {
        public static PointF Position;
        public static float Zoom = 1f;
        public static float Angle = 0f;

        private static object LockBlob = new object();
    }
}
