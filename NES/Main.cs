using System;

namespace NES
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Engine e = new Engine("nestest.nes");
			e.Run();
		}
	}
}