using System;

namespace NES
{
	public class Engine
	{
		private byte[] RAM = new byte[2048];
	
		public CPU CPU;
		public PPU PPU;
		public IORegisters IORegisters;
		
		public Engine()
		{
		}
		
		public byte ReadMemory8(ushort addr)
		{
			// Depends on where!
			if (addr < 0x2000)
			{
				// Ram w/ mirroring
				return RAM[addr % 2048]; 
			} else if ((addr >= 0x2000) && (addr < 0x4020))
			{
				// Registers
				if (addr < 0x4000)
					addr = (ushort)(((addr - 0x2000) % 0x8) + 0x2000);
				
				return IORegisters.ReadMemory8(addr);
			} else if (addr >= 0x4020)
			{
				// Mapper
				throw new NotImplementedException();
			} else
				throw new ArgumentException();
		}
		
		public ushort ReadMemory16(ushort addr)
		{
			byte lower = ReadMemory8(addr);
			byte upper = ReadMemory8((ushort)(addr + 1));
			return (ushort)(lower | (upper << 8));
		}
		
		public void WriteMemory8(ushort addr, byte val)
		{
			// Depends on where!
			if (addr < 0x2000)
			{
				// Ram w/ mirroring
				RAM[addr % 2048] = val;
			} else if ((addr >= 0x2000) && (addr < 0x4020))
			{
				// Registers
				if (addr < 0x4000)
					addr = (ushort)(((addr - 0x2000) % 0x8) + 0x2000);
				
				IORegisters.WriteMemory8(addr, val);
			} else if (addr >= 0x4020)
			{
				// Mapper
				throw new NotImplementedException();
			} else
				throw new ArgumentException();
		}
		
	}
}
