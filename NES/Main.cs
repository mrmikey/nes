using System;

namespace NES
{
	class MainClass
	{
		public static bool Running = true;
	
		public static void Main(string[] args)
		{
			Engine e = new Engine("Donkey Kong.nes");
			e.Run();
			//Graphics g = new Graphics(256, 240);
			//Console.Read();
			//while (true);
		}
	}
}