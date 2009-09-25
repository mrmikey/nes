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
		private Engine Engine;
		public PPUFlags Flags = new PPUFlags();
		public Mirroring Mirroring = Mirroring.Horizontal;
		public decimal CPUScaling = 3.2m; // Number of PPU Cycles per CPU cycle [PAL; NTSC is 3]
		public int Cycle = 0;
		public float CycleCarry = 0;
		public int VBlankTime = 70; // Number of Scanlines we're in VBlank for [PAL; NTSC is 20]
		public int CurrentScaline = -1; // Pre-draw
		public int VBlankAt = 240; // Scanline to start VBlank at
		public int EndScanline = 240 + 1 + 1 + 70; // PAL... hardcode for now, should use VBlankTime todo
		
		private byte[,] PatternTables = new byte[2, 0x1000];
		private byte[,] NameTables = new byte[4,0x3C0];
		private byte[,] AttributeTables = new byte[4, 0x40];
		private byte[] SpriteRAM = new byte[256];
		private byte[] ImagePalette = new byte[16];
		private byte[] SpritePalette = new byte[16];
		
		public bool Rendering
		{
			get 
			{ 
			return true; //Todo: Check scanline# and $2002 
			}
		}
		
		public PPU(Engine engine)
		{
			this.Engine = engine;
		}
	
		public void Run(int cpuCycles)
		{
			int ppuCycles = (int)((decimal)(cpuCycles + CycleCarry) * CPUScaling); // Number of ppu cycles to do
			int endCycle = 341; // 0-340 cycles inclusive for 341 cycles total.
			
			// Timing!
			for (int i = 0; i != ppuCycles; ++i)
			{
				++Cycle;
				if (Cycle == 256) // Significance?
				{
					// Todo: handle short scanlines, see nes_emu.txt
					endCycle = 341;
				} else if (Cycle == 304)
				{
					//frame start, $2006 gets reloaded with the tmp addr
					//this happens in the dummy scanline, and the PPU
					//is rendering. The reason for the reload because
					//Loopy_V is changed as the PPU is rendering.
					//Loopy_V is the "program counter" for the PPU.
					if ((CurrentScaline < 0) && Rendering)
						Flags.Loopy_V = Flags.Loopy_T;
				} else if (Cycle == endCycle)
				{
					++CurrentScaline;
					Cycle = 0;
					
					// Start of VBlank (240 is idle)
					if (CurrentScaline == 241)
					{
						// Todo: Render
						Flags.Status |= 0x80; // Set VBlank flag in $2002
						Flags.SprAddr = 0; // Gets reset at end of frame
						Engine.CPU.Flags.InterruptDisable = true; // Set NMI
					} else if (CurrentScaline == EndScanline)
					{
						CurrentScaline = -1;
						// todo: this.Rendering, and short scanlines
					}
				} else if ((CurrentScaline < 0) && (Cycle == 1))
				{
					// VBlank gets cleared at cycle 1 of scanline -1
					Flags.Status = 0; // ALL 0?!
				}
			}
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
