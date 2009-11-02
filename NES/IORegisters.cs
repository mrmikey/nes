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
					byte status = PPUFlags.Status;
					PPUFlags.OnStatusRead();
					return status;
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
					return PPU.VRAMRead();
					break;
				case 0x4014: // SPR-RAM DMA
					throw new ArgumentException();
					break;
				case 0x4016: // Joypad 1
					return (byte)(Engine.Joypads.Read(0) | Engine.APU.Register4016);
				case 0x4017: // Joypad 2
					return Engine.Joypads.Read(1);
				default:
					throw new NotImplementedException(String.Format("Uh-oh, attempting read to {0:x} and this register is not implemented!", addr));
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
					throw new ArgumentException();
					break;
				case 0x2003: // SPR-RAM Address
					PPUFlags.SprAddr = val;
					break;
				case 0x2004: // SPR-RAM I/O
					throw new NotImplementedException();
					break;
				case 0x2005: // VRAM Address 1
					PPUFlags.VramAddrReg1 = val;
					break;
				case 0x2006: // VRAM Address 2
					PPUFlags.VramAddrReg2 = val;
					break;
				case 0x2007: // VRAM I/O
					PPU.WriteMemory8(PPU.Flags.Loopy_V, val);
					PPUFlags.Loopy_V += PPUFlags.IncrementAddress;
					break;
				case 0x4014: // SPR-RAM DMA
					PPU.SpriteDMA(val);
					break;
				case 0x4016: // Joypad 1
					Engine.Joypads.Write(0, val);
					break;
				case 0x4017: // Joypad 2
					Engine.Joypads.Write(1, val);
					Engine.APU.Register4017 = val;
					break;
				//default:
				//default:
				//	throw new NotImplementedException();
			}
		}
	}
}
