using System;

namespace NES
{	
	public class Nametables
	{
		private byte[][] physicalNametables = new byte[4][];
		private byte[][] logicalNametables = new byte[4][];
		public byte[][] LogicalNametables
		{
			get { return logicalNametables; }
		}	
		public byte this[int addr]
		{
			get { Console.WriteLine("{0:x}", addr); return logicalNametables[(addr & 0xC00) >> 10][addr & 0x3FF]; }
			set 
			{ 
			logicalNametables[(addr & 0xC00) >> 10][addr & 0x3FF] = value; 
			if (value > 0)
				Console.ReadKey();
			}
		}
		
		public Nametables()
		{
			for (int i = 0; i < 4; i++)
				physicalNametables[i] = new byte[0x400];
		}
		
		// Read AND Write func.
		private byte doReadWrite(ushort addr, bool read, byte val)
		{
			// Quick check
			if (addr < 0x2000)
				throw new ArgumentException();
				
			// Find nametable
			byte[] nt = null;
			if (addr < 0x2400) {
				nt = logicalNametables[0];
				addr -= 0x2000;
			} else if (addr < 0x2800) {
				nt = logicalNametables[1];
				addr -= 0x2400;
			} else if (addr < 0x2C00) {
				nt = logicalNametables[2];
				addr -= 0x2800;
			} else if (addr < 0x3000) {
				nt = logicalNametables[3];
				addr -= 0x3C00;
			} else if (addr >= 0x3000)
				return doReadWrite((ushort)(addr - 0x1000), read, val);
	
			// Read or write?
			if (read) 
				return nt[addr]; // read
			else
				nt[addr] = val; // write
			return 0;
		}
					
		public void SetupNametables(Mirroring mirroring)
		{
			// Sorts nametable mirroring! :)
			switch (mirroring)
			{
				case Mirroring.Horizontal:
					logicalNametables[0] = physicalNametables[0];
					logicalNametables[1] = physicalNametables[0];
					logicalNametables[2] = physicalNametables[1];
					logicalNametables[3] = physicalNametables[2];
					break;
				case Mirroring.Vertical:
					logicalNametables[0] = physicalNametables[0];
					logicalNametables[1] = physicalNametables[1];
					logicalNametables[2] = physicalNametables[0];
					logicalNametables[3] = physicalNametables[1];
					break;
				case Mirroring.None:
					logicalNametables[0] = physicalNametables[0];
					logicalNametables[1] = physicalNametables[1];
					logicalNametables[2] = physicalNametables[2];
					logicalNametables[3] = physicalNametables[3];
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
