using System;

namespace NES
{
	public class Cartridge
	{
		public const int PRGLength = 16384;
		public const int RAMLength = 8192;
		public const int TrainerLength = 512;
		public const int CHRLength = 8192;
		
		private byte[][] chrbanks; // All CHR banks loaded.
		public byte[][] CHRBanks; // The two mapped CHR banks (one might be RAM)
		public bool[] CHRBanksCanWrite; // Whether we can write to each chr bank
		private int numCHR, numRAM;
		
		private Engine Engine;
		public byte[][] PRGROMBanks;
		public byte[] Trainer;
		public bool HasTrainer; 
		public bool SRAM;
		public int MapperNumer;
		
		public Cartridge(Engine engine, int prg, int chr, int ram)
		{
			this.Engine = engine;
			PRGROMBanks = new byte[prg][];
			chrbanks = new byte[chr + ram][];	
			numCHR = chr;
			numRAM = ram;
		}
		
		public void CHRWrite(ushort addr, byte val)
		{
			int bankNo = (addr & 0x3000) >> 12;
			if (CHRBanksCanWrite[bankNo])
				CHRBanks[bankNo][addr & 0xFFF] = val; // if we can write to it, do so. [ie. if it is RAM not ROM]
			// otherwise, do nothing!
		}
		
		public void LoadCHR(System.IO.FileStream fs)
		{
			// CHR-ROM
			for (int i = 0; i < numCHR; i++)
			{
				chrbanks[i] = new byte[Cartridge.CHRLength];
				fs.Read(chrbanks[i], 0, Cartridge.CHRLength);
			}
			// CHR-RAM
			for (int i = numCHR; i < (numCHR + numRAM); i++)
			{
				chrbanks[i] = new byte[Cartridge.CHRLength];
				for (int j = 0; j < Cartridge.CHRLength; j++)
					chrbanks[i][j] = (byte)0; // write 0's
			}
				
			MapCHR();
		}
		
		public void MapCHR()
		{
			// Work out which to map! just do the first two for now :)
			CHRBanks = new byte[2][];
			CHRBanksCanWrite = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				CHRBanks[i] = chrbanks[i];
				CHRBanksCanWrite[i] = (i < numCHR) ? false : true;
			}
			
			// Initialise CHR Cache
			Engine.PPU.InitialiseCHRCache(CHRBanks);
		}
	}
}
