using System;

namespace NES
{
	public class APU
	{
		public byte Register4017 = 0;	
		public byte Register4016 = 0;
		private Engine Engine;
		public int Cycles = 0;
		public int Step = 0;
		public int Delay = 8315; // PAL
		public int EndStep
		{
			get 
			{
				return ((Register4017 & 128) > 0) ? 5 : 4;
			}
		}
		public APU(Engine engine)
		{
			Engine = engine;
		}
		
		public void Run(int cpuCycles)
		{
			Cycles += cpuCycles;
			if (Cycles >= Delay)
			{	
				Step++;
				Cycles = 0;
			}
			if (Step == EndStep)
			{
				if (Step == 4)
					if (!Engine.CPU.Flags.InterruptDisable)
					{
						Engine.CPU.IRQ = true;
						
					}
				Step = 0;
			}
		}
	}
}
