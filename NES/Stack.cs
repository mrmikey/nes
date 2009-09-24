using System;

namespace NES
{
	public class Stack
	{
		private Engine Engine;
		public byte SP = 0xFF;
		
		public Stack(Engine engine)
		{
			this.Engine = engine;
		}
		
		public void Push8(byte val)
		{
			Engine.WriteMemory8((ushort)(0x0100 + SP), val);
			SP--;
			
			if (SP < 0)
				Console.WriteLine("Warning: Stack overflow!");
		}
		public void Push16(ushort val)
		{
			byte upper = (byte)((val & 0xFF00) >> 8);
			byte lower = (byte)(val & 0xFF);

			// Little Endian :')
			Push8(upper);
			Push8(lower);
		}
		
		public byte Pop8()
		{
			SP++;
			return Engine.ReadMemory8((ushort)(0x0100 + SP));
		}
		public ushort Pop16()
		{
			byte lower = Pop8();
			byte upper = Pop8();
			return (ushort)(lower | (upper << 8));
		}
	}
}
