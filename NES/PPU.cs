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
		public decimal CPUScaling = 3m; // Number of PPU Cycles per CPU cycle [PAL; NTSC is 3]
		public int Cycle = 0;
		public float CycleCarry = 0;
		public int VBlankTime = 70; // Number of Scanlines we're in VBlank for [PAL; NTSC is 20]
		public int CurrentScaline = -1; // Pre-draw
		public int VBlankAt = 240; // Scanline to start VBlank at
		public int EndScanline = 500; // PAL... hardcode for now, should use VBlankTime todo
		public int CurrentSprite = 0;
	//	private byte[,] PatternTables = new byte[2, 0x1000];
		private byte[] SpriteRAM = new byte[256];
		private byte[] Palettes = new byte[0x20];
		private Nametables Nametables = new Nametables();
		public byte[] CHRCache;
		public byte[] AttributeShiftTable = new byte[0x400];
		public byte[] AttributeLocationTable = new byte[0x400];
		public int[] Palette = new int[] { 
			0x757575, 0x271B8F, 0x0000AB, 0x47009F, 0x8F0077, 0xAB0013, 0xA70000, 0x7F0B00,
			0x432F00, 0x004700, 0x005100, 0x003F17, 0x1B3F5F, 0x000000, 0x000000, 0x000000,
			0xBCBCBC, 0x0073EF, 0x233BEF, 0x8300F3, 0xBF00BF, 0xE7005B, 0xDB2B00, 0xCB4F0F,
			0x8B7300, 0x009700, 0x00AB00, 0x00933B, 0x00838B, 0x000000, 0x000000, 0x000000,
			0xFFFFFF, 0x3FBFFF, 0x5F97FF, 0xA78BFD, 0xF77BFF, 0xFF77B7, 0xFF7763, 0xFF9B3B,
			0xF3BF3F, 0x83D313, 0x4FDF4B, 0x58F898, 0x00EBDB, 0x000000, 0x000000, 0x000000,
			0xFFFFFF, 0xABE7FF, 0xC7D7FF, 0xD7CBFF, 0xFFC7FF, 0xFFC7DB, 0xFFBFB3, 0xFFDBAB,
			0xFFE7A3, 0xE3FFA3, 0xABF3BF, 0xB3FFCF, 0x9FFFF3, 0x000000, 0x000000, 0x000000 };
			
		private Mirroring mirroring = Mirroring.Horizontal;
		public Mirroring Mirroring
		{
			get { return mirroring; }
			set 
			{
				mirroring = value;
				Nametables.SetupNametables(mirroring);
			}
		}
		
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
			this.dumpSprite = new SpriteDumpFunc(dumpSpriteSmall);
			// Attribute Lookup Tables
		 	for (int i = 0; i != 0x400; ++i)
		    {
				AttributeShiftTable[i] = (byte)(((i >> 4) & 0x04) | (i & 0x02));
				AttributeLocationTable[i] = (byte)(((i >> 4) & 0x38) | ((i >> 2))  & 7);
		   }
			
			// Setup nametables
			Nametables.SetupNametables(this.Mirroring);
		}
		
		public void dumpNametable()
		{
			int addr = 0x2000; int attrib_addr = 0x23C0;
			for (int i = 0x2000; i < attrib_addr; i++)
			{
				byte tile = Nametables[i];

				int pattern_addr = (tile << 6) | (Flags.BGTable << 14);
				
				// Get attribute
				int attribute_addr = 0x23C0 | (i & 0xC00) | (AttributeLocationTable[i & 0x3FF]);
				int attribute_shift = AttributeShiftTable[i & 0x3FF];
				int attribute = ((Nametables[attribute_addr] >> attribute_shift) & 3) << 2; // Remember attribute is upper 2 bits of color.
				
				// Now we have the pattern_addr and the attribute, we can render! :)
				int x = i & 31; // bits 0-4 are x-scroll
				int y = (i & 0x3E0) >> 5; // bits 5-9 are y-scroll.
				y = (y > 29) ? 29 : y;
				Engine.Graphics.DrawTile(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprHighBuffer, 0, (256 * Flags.BGTable) + tile, x*8, y*8, attribute);
				if (tile > 0)
					Console.ReadKey();
			}
		}

		private delegate void SpriteDumpFunc(int i);
		private SpriteDumpFunc dumpSprite;
		
		public void dumpSpriteSmall(int i)
		{
				int y = SpriteRAM[i]+1;
				
				if (y >= 0xF0)
					return;
					
				byte tile = SpriteRAM[i+1];
				byte table = Flags.SprTable;
				int palette = (SpriteRAM[i+2] & 3) << 2;
				
				// ignore priority
				// ignore flipping horiz/vert
				byte x = SpriteRAM[i+3];
				
				// Handle Flipping and Render! [Upper two bits of byte 2; MSB is vert flip]
				int flipping = SpriteRAM[i+2] & 0xC0;
				switch (flipping)
				{
					case 0xC0: // 11 - Both
						Engine.Graphics.DrawTileBothFlip(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprHighBuffer, 0x10, (256 * table) + tile, x, y, palette);
						break;
					case 0x80: // 10 - Vert
						Engine.Graphics.DrawTileVertFlip(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprHighBuffer, 0x10, (256 * table) + tile, x, y, palette);
						break;
					case 0x40: // 01 - Horiz
						Engine.Graphics.DrawTileHorizFlip(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprHighBuffer, 0x10, (256 * table) + tile, x, y, palette);
						break;
					case 0:
						Engine.Graphics.DrawTile(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprHighBuffer, 0x10, (256 * table) + tile, x, y, palette);
						break;
				} // yum, efficient.
		}
		
		public void dumpSpriteTall(int i)
		{	
				throw new NotImplementedException();
		
				byte y = (byte)(SpriteRAM[i]+1);
				
				if (y >= 0xF0)
					return;
				
				// Get tile number -- eventually move this if out of the loop!


				byte tile = (byte)(SpriteRAM[i+1] >> 1);
				byte table = (byte)(SpriteRAM[i+1] & 1);
				
				byte palette = (byte)(SpriteRAM[i+2] & 3);
				// ignore priority
				// ignore flipping horiz/vert
				byte x = SpriteRAM[i+3];
				
				// DRAW BIZNITCH
				Engine.Graphics.DrawTile(Engine.Cartridge.CHRBanks[0], Engine.Graphics.SprLowBuffer, 0x10, (256 * table) + tile, x, y, palette);
		}

		public void Run(int cpuCycles)
		{
			int ppuCycles = (int)((decimal)(cpuCycles + CycleCarry) * CPUScaling); // Number of ppu cycles to do
			int endCycle = 341; // 0-340 cycles inclusive for 341 cycles total.
			
			// Timing!
			for (int i = 0; i < ppuCycles; ++i)
			{
			//	if ((Cycle == 0) && (CurrentScaline == 0))
			//		Flags.Loopy_V = (ushort)(0x2000 | (Flags.NameTableAddress << 10));
			
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
						//dumpNametable();
						//dumpSprites();
						Engine.Graphics.Render();
						Flags.Status |= 0x80; // Set VBlank flag in $2002
						Engine.CPU.NMI = ((Flags.Control1 & 0x80) > 1) ? true : false; // Only trigger NMI if they want us to in $2000
						Flags.SprAddr = 0; // Gets reset at end of frame
						Engine.CPU.Flags.InterruptDisable = true; // Set NMI
						Engine.CPU.SinceLastVBlank = 0;
					} else if (CurrentScaline == EndScanline)
					{
						CurrentScaline = -1;
						CurrentSprite = 0;
						if (Flags.TallSprites)
							dumpSprite = new SpriteDumpFunc(dumpSpriteTall);
						else
							dumpSprite = new SpriteDumpFunc(dumpSpriteSmall);
						// todo: this.Rendering, and short scanlines
					}
				} else if ((CurrentScaline < 0) && (Cycle == 1))
				{
					// VBlank gets cleared at cycle 1 of scanline -1
					Flags.Status = 0; // ALL 0?!
				}
				
				// Sprites!
				if ((CurrentSprite < 32) && (CurrentScaline > 0))
				{
					dumpSprite(CurrentSprite * 4);
					CurrentSprite++;
				}
				
				// :D
				if ((CurrentScaline < 240) && (CurrentScaline > 0))
				{
					// Datas! 
					switch(Cycle)
					{	
						// Cycle 0 - Get tile
						// Cycle 1 - Gets the pattern address for the tile
						// Cycle 2 - Gets attribute addr[/shift]
						// Cycle 3 - Apply attribute
						// Cycle 4 - Nothing
						// Cycle 5 - Get lower bit from nt [We do upper too]
						// Cycle 6 - Get pattern address for upper two bits [We don't need this]
						// Cycle 7 - Get upper bit from nt [Did this in Cycle 5]
						// Rinse and repeat.
						// Buuut we're not gonna do that -- Just make sure we update Loopy_V at cycle 251
						
						// Cycle 0 - We'll do all of the above! :)
						case   0:	case   8:	case  16:	case  24:	
						case  32:	case  40:	case  48:	case  56:
						case  64:	case  72:	case  80:	case  88:	
						case  96:	case 104:	case 112:	case 120:
						case 128:	case 136:	case 144:	case 152:	
						case 160:	case 168:	case 176:	case 184:
						case 192:	case 200:	case 208:	case 216:	
						case 224:	case 232:	case 240:	case 248:
							// Find out which pattern to use
							byte tile = Nametables[Flags.Loopy_V];
	
							int pattern_addr = (tile << 6) | (Flags.BGTable << 14);
							
							// Get attribute
							int attribute_addr = 0x23C0 | (Flags.Loopy_V & 0xC00) | (AttributeLocationTable[Flags.Loopy_V & 0x3FF]);
							int attribute_shift = AttributeShiftTable[Flags.Loopy_V & 0x3FF];
							int attribute = ((Nametables[attribute_addr] >> attribute_shift) & 3) << 2; // Remember attribute is upper 2 bits of color.
							
							// Now we have the pattern_addr and the attribute, we can render! :)
							int x = Flags.Loopy_V & 31; // bits 0-4 are x-scroll
							int y = (Flags.Loopy_V & 0x3E0) >> 5; // bits 5-9 are y-scroll.
							y = (y > 29) ? 29 : y;
							Engine.Graphics.DrawTile(Engine.Cartridge.CHRBanks[0], Engine.Graphics.BGBuffer, 0, (256 * Flags.BGTable) + tile, x*8, y*8, attribute);
							
							
							break;
							
						case   3:	case  11:	case  19:	case  27:	
						case  35:	case  43:	case  51:	case  59:
						case  67:	case  75:	case  83:	case  91:	
						case  99:	case 107:	case 115:	case 123:
						case 131:	case 139:	case 147:	case 155:	
						case 163:	case 171:	case 179:	case 187:
						case 195:	case 203:	case 211:	case 219:	
						case 227:	case 235:	case 243:
						
							if ((Flags.Loopy_V & 0x1F) == 0x1F)
							 	 Flags.Loopy_V ^= 0x41F;
							else	
								++Flags.Loopy_V;
								
							break;
							
						case 323:	case 331:
								
							if ((Flags.Loopy_V & 0x1F) == 0x1F)
							 	 Flags.Loopy_V ^= 0x41F;
							else	
								++Flags.Loopy_V;
								
							
							break;
							
							
						case 251:
						
							if ((Flags.Loopy_V & 0x1F) == 0x1F)
							 	 Flags.Loopy_V ^= 0x41F;
							else	
								++Flags.Loopy_V;
		
							
		
							if ((Flags.Loopy_V & 0x7000) == 0x7000)
							{
								int tmp = Flags.Loopy_V & 0x3E0;
								//reset tile y offset 12 - 14 in addr
								Flags.Loopy_V &= 0xFFF;
								switch (tmp)
								{
									//29, flip bit 11
									case 0x3A0:
										Flags.Loopy_V ^= 0xBA0;
										break;
									case 0x3E0: //31, back to 0
										Flags.Loopy_V ^= 0x3E0;
										break;
									default: //inc y scroll if not reached
										Flags.Loopy_V += 0x20;
										break;
								}
							}
							else //inc fine y
								Flags.Loopy_V += 0x1000;
								
							break;
					} 
				}
			}
		}
	/*
		public void InitialiseCHRCache(byte[][] CHRROMs)
		{
			CHRCache = new byte[CHRROMs.Length * 64 * 0x100];
			int count = 0;
			for (int i = 0; i < CHRROMs.Length; i++)
			{
				for (int j = 0; j < 0x1000; j += 16)
				{
					for (int x = 0; x < 8; x++)
					{
						for (int y = 0; y < 8; y++)
						{
							byte lower =  (byte)((CHRROMs[i][j+y+8] >> (x ^ 7)) & 1);
							byte upper = (byte)(((CHRROMs[i][j+y] >> (x ^ 7)) & 1) << 1);
							if (((lower != 0) && (lower != 1)) || ((upper != 0) && (upper != 2)))
								throw new Exception();
							
							CHRCache[count] = lower;
							CHRCache[count] |= upper;
							count++;
						}
					}
				}
			}
		}
		*/
		# region Read/Write
		
		// VRAMRead is for $2007 read, with first garbage data (hence le buffer)
		private byte bufferedRead = 0;
		private bool hasRead = false; // so we don't increase Loopy_V for first (buffered) read
		public byte VRAMRead()
		{ // Todo: implement pallette quirks
			byte ret = bufferedRead;
			bufferedRead = ReadMemory8(Flags.Loopy_V);
			
			// Inc Loopy_V?
			if (!hasRead)
				hasRead = true;
			else 
				Flags.Loopy_V += Flags.IncrementAddress; // 1 or 32 (ie. x or y direction)
			
			return ret;
		}
	
		public byte ReadMemory8(ushort addr)
		{
			addr &= 0x3FFF; // mirroring.
			if (addr < 0x2000)	// Pattern table
			{
				if (addr < 0x1000)
					return Engine.Cartridge.CHRBanks[0][addr];
				else
					return Engine.Cartridge.CHRBanks[1][addr - 0x1000];
			}
			else if ((addr >= 0x2000) && (addr < 0x3F00)) // Nametables
				return Nametables[addr];
			else if ((addr >= 0x3F00) && (addr < 0x3F20)) // Palettes Todo: Mirroring
				return Palettes[addr - 0x3F00];
			else
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
			addr &= 0x3FFF; // mirroring.
			if (addr < 0x2000)	// Pattern table
				return; // Todo: support chr ram
			else if ((addr >= 0x2000) && (addr < 0x3F00)) // Nametables
				Nametables[addr] = val;
			else if ((addr >= 0x3F00) && (addr < 0x4000)) // Palettes Todo: Mirroring
				Palettes[addr & 0x1F] = val;
			else
				throw new NotImplementedException();
		}
		
		public void SpriteDMA(byte val)
		{
			ushort cpuAddr = (ushort)(val * 0x100); // Value written to $4014 * $100 is the CPU value to copy from
			for (int i = cpuAddr; i < (cpuAddr + SpriteRAM.Length); i++) // Todo: INEFFICENT
				SpriteRAM[i - cpuAddr] = Engine.ReadMemory8((ushort)i);
		}
		
		#endregion
		
	}
}
