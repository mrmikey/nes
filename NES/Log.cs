using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES
{
    /// <summary>
    /// In spite of all C#'s wonderful debugging functionality, sometimes you really, really
    /// need some logging goodness. e.g. if you don't want to have to investigate every
    /// single thing yourself constantly/checking performance of certain operations.
    /// </summary>
    public static class Log
    {
        private static object logLock = new object();
		public static bool Silence = true;
        public static bool Verbose = false;

        private static void writeLog(ConsoleColor cc, string message)
        {
        	if (!Silence)
	            lock (logLock)
	            {
	                Console.ForegroundColor = cc;
	                Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + message);
	            }
        }

        /// <summary>
        /// The information, do you have it?!
        /// </summary>
        /// <param name="info">Something incredibly interesting.</param>
        /// <param name="format">Formatting shizz.</param>
        public static void i(string info, params object[] format)
        {
            Log.i(String.Format(info, format));
        }
        /// <summary>
        /// The information, do you have it!?
        /// </summary>
        /// <param name="info">Something incredibly interesting</param>
        public static void i(string info)
        {
            writeLog(ConsoleColor.Green, info);
        }

        /// <summary>
        /// Some message that's only useful in debugging.
        /// </summary>
        /// <param name="info">Something incredibly interesting.</param>
        /// <param name="format">Formatting shizz.</param>
        public static void v(string info, params object[] format)
        {
            if (Verbose)
                Log.v(String.Format(info, format));
        }
        /// <summary>
        /// Some message that's only useful in debugging.
        /// </summary>
        /// <param name="info">Something incredibly interesting</param>
        public static void v(string info)
        {
            if (Verbose)
                writeLog(ConsoleColor.Magenta, info);
        }

        /// <summary>
        /// A warning to all those who cross me!
        /// </summary>
        /// <param name="warning">What it is that's got you so worked up</param>
        /// <param name="format">Formatting shizz.</param>
        public static void w(string warning, params object[] format)
        {
            Log.w(String.Format(warning, format));
        }
        /// <summary>
        /// A warning to all those who cross me!
        /// </summary>
        /// <param name="warning">What it is that's got you so worked up</param>
        public static void w(string warning)
        {
            writeLog(ConsoleColor.Yellow, warning);
        }

        /// <summary>
        /// DOES. NOT. COMPUTE.
        /// </summary>
        /// <param name="error">Why we're potentially screwed</param>
        /// /// <param name="format">Formatting shizz.</param>
        public static void e(string error, params object[] format)
        {
            Log.e(String.Format(error, format));
        }
        /// <summary>
        /// DOES. NOT. COMPUTE.
        /// </summary>
        /// <param name="error">Why we're potentially screwed</param>
        public static void e(string error)
        {
            writeLog(ConsoleColor.Red, error);
        }
    }
}
