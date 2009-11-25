using System;
using System.IO;

namespace NES
{
	public class Engine
	{
		private byte[] RAM = new byte[2048];
	
		public CPU CPU;
		public PPU PPU;
		public APU APU;
		public Cartridge Cartridge;
		public IORegisters IORegisters;
		public Joypads Joypads;
		public Graphics Graphics;
		public bool Running = true;
		
		public Engine(string filename)
		{
			CPU = new CPU(this);
			PPU = new PPU(this);
			APU = new APU(this);
			IORegisters = new IORegisters(this, PPU);
			Joypads = new Joypads();
			
			loadiNes(filename);
			Graphics = new Graphics(this, 256,240);
			Joypads.Initialise();
		}
		
		private void loadiNes(string filename)
		{
			FileStream fs = File.OpenRead(filename);
			StreamReader r = new StreamReader(fs);
			
			// Check header
			byte[] buffer = new byte[4];
			fs.Read(buffer, 0, 4);
			if ((buffer[0] != 0x4E) || (buffer[1] != 0x45) || (buffer[2] != 0x53) || (buffer[3] != 0x1A))
				throw new FileLoadException("Unsupported file format!");
			
			// Datas!
			byte numPRG = (byte)fs.ReadByte();
			byte numCHR = (byte)fs.ReadByte();
			byte control1 = (byte)fs.ReadByte();
			byte control2 = (byte)fs.ReadByte();
			byte numRAM = (byte)fs.ReadByte();
			buffer = new byte[7];
			fs.Read(buffer, 0, 7); // blank.
			
			// Sanity check, 16byte header?
			if (fs.Position != 16)
				throw new FileLoadException("We did it wrong!");
				
			// Process!
			numRAM = (numRAM == 0) ? (byte)1 : numRAM;
			Cartridge = new Cartridge(this, numPRG, numCHR, numRAM);
				// Control 1
				PPU.Mirroring = ((control1 & 1) > 0) ? Mirroring.Vertical : Mirroring.Horizontal; // Bit 0 - Type of mirroring
				Cartridge.SRAM = ((control1 & 2) > 0) ? true : false; // Bit 1 - Presence of SRAM
				Cartridge.HasTrainer = ((control1 & 4) > 0) ? true : false; // Bit 2 - Presence of Trainer
				if ((control1 & 8) > 0) // Bit 3 - If set, overrides bit 0 to indicate 4-screen mirroring
					PPU.Mirroring = Mirroring.None;
				Cartridge.MapperNumer = (control1 & 0xF0) >> 4; // Bit 4-7 - 4 lower bits of mapper number
				// Control 2
				Cartridge.MapperNumer |= (control2 & 0xF0); // Bit 4-7 - 4 upper bits of mapper number
			
			// Trainer
			if (Cartridge.HasTrainer)
				fs.Read(Cartridge.Trainer, 0, Cartridge.TrainerLength);
			
			// Load PRG-ROM
			for (int i = 0; i < numPRG; i++)
			{
				Cartridge.PRGROMBanks[i] = new byte[Cartridge.PRGLength];
				fs.Read(Cartridge.PRGROMBanks[i], 0, Cartridge.PRGLength);
			}
			
			// Load CHR-ROM
			for (int i = 0; i < numCHR; i++)
			{
				Cartridge.CHRBanks[i] = new byte[Cartridge.CHRLength];
				fs.Read(Cartridge.CHRBanks[i], 0, Cartridge.CHRLength);
			}
			
		}
		
		public void Run()
		{
			CPU.Reset();
			while (Running)
			{
				// Accuracy can be up to 6/7 cycles out if they access Registers! HM. Todo
				int cpuCycles = 0;
				for (int i = 0; i < 100; i++)
					cpuCycles += CPU.Run();
				PPU.Run(cpuCycles);
				//APU.Run(cpuCycles);
			}
			
			Graphics.Deinitialise();
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
			} else if (addr < 0x8000)
			{
				Console.WriteLine("$02 = {0:x}", ReadMemory8((ushort)0x02));
				Console.WriteLine("$03 = {0:x}", ReadMemory8((ushort)0x03));
				throw new NotImplementedException();	
			} else if (addr <= 0xFFFF)
			{
				int desiredBank = (addr < 0xC000) ? 1 : 0;
				addr = (ushort)((addr < 0xC000) ? addr - 0x8000 : addr - 0xC000);
				desiredBank = (Cartridge.PRGROMBanks.Length == 1) ? 0 : desiredBank; 
				return Cartridge.PRGROMBanks[desiredBank][addr];
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
			} else if (addr < 0x8000)
			{
				throw new NotImplementedException();	
			} else if (addr <= 0xFFFF)
			{
				int desiredBank = (addr < 0xC000) ? 1 : 0;
				addr = (ushort)((addr < 0xC000) ? addr - 0x8000 : addr - 0xC000);
				desiredBank = (Cartridge.PRGROMBanks.Length == 1) ? 0 : desiredBank; 
				Cartridge.PRGROMBanks[desiredBank][addr] = val;
			} else
				throw new ArgumentException();
		}
		
	}
}
