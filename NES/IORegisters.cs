using System;

namespace NES
{
	public class IORegisters
	{
		private Engine Engine;
		public PPUFlags PPUFlags;
		public PPU PPU;
	
		public IORegisters(Engine engine, PPU ppu)
		{
			this.Engine = engine;
			this.PPU = ppu;		
			this.PPUFlags = this.PPU.Flags;
		}
		
		public byte ReadMemory8(ushort addr)
		{
			// sanity check
			if ((addr < 0x2000) || (addr >= 0x4020))
				throw new ArgumentException();
				
			switch (addr)
			{
				case 0x2000: // PPU Control 1
					throw new ArgumentException();			
					break;
				case 0x2001: // PPU Control 2
					throw new ArgumentException();
					break;
				case 0x2002: // PPU Status
					return PPUFlags.Status;
					break;
				case 0x2003: // SPR-RAM Address
					throw new ArgumentException();
					break;
				case 0x2004: // SPR-RAM I/O
					throw new ArgumentException();
					break;
				case 0x2005: // VRAM Address 1
					throw new ArgumentException();
					break;
				case 0x2006: // VRAM Address 2
					throw new ArgumentException();
					break;
				case 0x2007: // VRAM I/O
					return PPU.ReadMemory8((ushort)(PPUFlags.VramAddr1 | (PPUFlags.VramAddr2 << 8)));
					break;
				case 0x4014: // SPR-RAM DMA
					throw new ArgumentException();
					break;
				default:
					throw new NotImplementedException();
			}
		}
		
		public void WriteMemory8(ushort addr, byte val)
		{
			// sanity check
			if ((addr < 0x2000) || (addr >= 0x4020))
				throw new ArgumentException();
				
			switch (addr)
			{
				case 0x2000: // PPU Control 1
					PPUFlags.Control1 = val;		
					break;
				case 0x2001: // PPU Control 2
					PPUFlags.Control2 = val;
					break;
				case 0x2002: // PPU Status
					PPUFlags.Status = val;
					break;
				case 0x2003: // SPR-RAM Address
					PPUFlags.SprAddr = val;
					break;
				case 0x2004: // SPR-RAM I/O
					PPU.WriteSprRam(val);
					break;
				case 0x2005: // VRAM Address 1
					PPUFlags.VramAddr1 = val;
					break;
				case 0x2006: // VRAM Address 2
					PPUFlags.VramAddr2 = val;
					break;
				case 0x2007: // VRAM I/O
					PPU.ReadMemory8((ushort)(PPUFlags.VramAddr1 | (PPUFlags.VramAddr2 << 8)));
					break;
				case 0x4014: // SPR-RAM DMA
					throw new NotImplementedException();
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
