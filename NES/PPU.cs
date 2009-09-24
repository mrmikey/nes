using System;

namespace NES
{	
	public enum Mirroring
	{
		Horizontal,
		Vertical,
		None
	}

	public class PPU
	{
		public PPUFlags Flags = new PPUFlags();
		public Mirroring Mirroring = Mirroring.Horizontal;
		
		private byte[,] PatternTables = new byte[2, 0x1000];
		private byte[,] NameTables = new byte[4,0x3C0];
		private byte[,] AttributeTables = new byte[4, 0x40];
		private byte[] SpriteRAM = new byte[256];
		private byte[] ImagePalette = new byte[16];
		private byte[] SpritePalette = new byte[16];
		
		public PPU()
		{
		}
	
		public byte ReadMemory8(ushort addr)
		{
			addr = (ushort)(addr % 0x4000); // mirroring.
			
			// Depends on where!
			if (addr < 0x2000)
			{
				// Pattern table
				if (addr < 0x1000)
					return PatternTables[0, addr];
				else
					return PatternTables[1, addr];
			}
			else if ((addr >= 0x2000) && (addr < 0x3F00))
			{
				addr = (ushort)(((addr - 0x2000) % 0x1000) + 0x2000);
				
				// Which logical table are they accessing?
				int logicalNumber = 0; 
				bool accessName = false; // whether we're accessing name or attrib
				if (addr < 0x23C0)
				{
					logicalNumber = 0;
					accessName = true;
					addr -= 0x2000;
				} else if (addr < 0x2400)
				{
					logicalNumber = 0;
					accessName = false;
					addr -= 0x23C0;
				} else if (addr < 0x27C0)
				{
					logicalNumber = 1;
					accessName = true;
					addr -= 0x2400;
				} else if (addr < 0x2800)
				{
					logicalNumber = 1;
					accessName = false;
					addr -= 0x27C0;
				} else if (addr < 0x2BC0)
				{
					logicalNumber = 2;
					accessName = true;
					addr -= 0x2800;
				} else if (addr < 0x2C00)
				{
					logicalNumber = 2;
					accessName = false;
					addr -= 0x2Bc0;
				} else if (addr < 0x2FC0)
				{
					logicalNumber = 3;
					accessName = true;
					addr -= 0x2C00;
				} else if (addr < 0x300)
				{
					logicalNumber = 3;
					accessName = false;
					addr -= 0x2FC0;
				} else
					throw new ArgumentException();
			
				// Mirroring
				switch (Mirroring)
				{
					case Mirroring.Horizontal:
						// Logical 0 and 1 refer to the first Name Table, 2 and 3 the second.
						if (logicalNumber == 1)
							logicalNumber = 0;
						else if (logicalNumber >= 2);
							logicalNumber = 1;
						break;
					case Mirroring.Vertical:
						// Logical 0 and 2 refer to the first Name Table, 1 and 3 the second
						if (logicalNumber == 2)
							logicalNumber = 0;
						else if (logicalNumber == 3)
							logicalNumber = 1;
						break;
					case Mirroring.None:
						throw new NotImplementedException();
						break;
					default:
						throw new ArgumentException();
				}
				
				// Actually do the read
				if (accessName)
					return NameTables[logicalNumber, addr];
				else
					return AttributeTables[logicalNumber, addr];
			}
			else if ((addr >= 0x3F00) && (addr < 0x4000))
			{ 
				// Palettes
				throw new NotImplementedException();
			} else
				throw new NotImplementedException();
		}
		
		public ushort ReadMemory16(ushort addr)
		{
			byte lower = ReadMemory8(addr);
			byte upper = ReadMemory8((ushort)(addr + 1));
			return (ushort)(lower | (upper << 8));
		}
		
		public void WriteMemory8(ushort addr, byte val)
		{
			addr = (ushort)(addr % 0x4000); // mirroring.
			
			// Depends on where!
			if (addr < 0x2000)
			{
				// Pattern table
				if (addr < 0x1000)
					PatternTables[0, addr] = val;
				else
					PatternTables[1, addr] = val;
			}
			else if ((addr >= 0x2000) && (addr < 0x3F00))
			{
				addr = (ushort)(((addr - 0x2000) % 0x1000) + 0x2000);
				
				// Which logical table are they accessing?
				int logicalNumber = 0; 
				bool accessName = false; // whether we're accessing name or attrib
				if (addr < 0x23C0)
				{
					logicalNumber = 0;
					accessName = true;
					addr -= 0x2000;
				} else if (addr < 0x2400)
				{
					logicalNumber = 0;
					accessName = false;
					addr -= 0x23C0;
				} else if (addr < 0x27C0)
				{
					logicalNumber = 1;
					accessName = true;
					addr -= 0x2400;
				} else if (addr < 0x2800)
				{
					logicalNumber = 1;
					accessName = false;
					addr -= 0x27C0;
				} else if (addr < 0x2BC0)
				{
					logicalNumber = 2;
					accessName = true;
					addr -= 0x2800;
				} else if (addr < 0x2C00)
				{
					logicalNumber = 2;
					accessName = false;
					addr -= 0x2Bc0;
				} else if (addr < 0x2FC0)
				{
					logicalNumber = 3;
					accessName = true;
					addr -= 0x2C00;
				} else if (addr < 0x300)
				{
					logicalNumber = 3;
					accessName = false;
					addr -= 0x2FC0;
				} else
					throw new ArgumentException();
			
				// Mirroring
				switch (Mirroring)
				{
					case Mirroring.Horizontal:
						// Logical 0 and 1 refer to the first Name Table, 2 and 3 the second.
						if (logicalNumber == 1)
							logicalNumber = 0;
						else if (logicalNumber >= 2);
							logicalNumber = 1;
						break;
					case Mirroring.Vertical:
						// Logical 0 and 2 refer to the first Name Table, 1 and 3 the second
						if (logicalNumber == 2)
							logicalNumber = 0;
						else if (logicalNumber == 3)
							logicalNumber = 1;
						break;
					case Mirroring.None:
						throw new NotImplementedException();
						break;
					default:
						throw new ArgumentException();
				}
				
				// Actually do the read
				if (accessName)
					NameTables[logicalNumber, addr] = val;
				else
					AttributeTables[logicalNumber, addr] = val;
			}
			else if ((addr >= 0x3F00) && (addr < 0x4000))
			{ 
				// Palettes
				throw new NotImplementedException();
			} else
				throw new NotImplementedException();
		}
		
		public void WriteSprRam(byte val)
		{
		}
	}
}
