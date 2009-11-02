using System;

namespace NES
{
	public class Cartridge
	{
		public const int PRGLength = 16384;
		public const int RAMLength = 8192;
		public const int TrainerLength = 512;
		public const int CHRLength = 8192;
		
		public byte[][] CHRBanks; // The two mapped CHR banks (one might be RAM)
		
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
			CHRBanks = new byte[chr][];	
		}
	
	}
}
