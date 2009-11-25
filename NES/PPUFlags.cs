using System;

namespace NES
{
	public class PPUFlags
	{
		public byte Control2, SprAddr;
	
		// $2001 - PPU Control 1
		private byte control1;
		public byte Control1
		{
			set
			{
				control1 = value;
				
				/* 2000 write:
			       t:xxxxABxxxxxxxxxx=d:xxxxxxAB
				   t is Loopy_T in this case */
				   
				Loopy_T &= 0x73FF;
				Loopy_T |= (ushort)((value & 0x03) << 10);
			}
			get { return control1; }
		}		
	
		// $2005 - VRAM Address 1 - Scroll Register (Write)
		private bool writeLatch = true; // used for 1st/2nd writes to VRAMAddr1/VRAMAddr2
		public ushort Loopy_T = 0x2000;
		private ushort loopy_v = 0x2000;
		public ushort Loopy_V
		{
			get 
			{
				return loopy_v;
			}
			set
			{
				loopy_v = value;
			}
		}
		public byte Loopy_X;
		public byte VramAddrReg1
		{
			set 
			{
				/* from loopy's docs [I used nes_emu.txt]
		       	t is Loopy_T while x is the fine x (Loopy_X)
		
		 	   	2005 first write:
		        	t:xxxxxxxxxxxABCDE=d:ABCDExxx
		        	x=d:xxxxxABC 
			   	2005 second write:
		        	t:xxxxxxABCDExxxxx=d:ABCDExxx
		        	t:xABCxxxxxxxxxxxx=d:xxxxxABC */
		        	
		        if (writeLatch)
		        {
		        	Loopy_T &= 0x7FE0;
		        	Loopy_T |= (ushort)((value & 0xF8) >> 3);
		        	Loopy_X = (byte)(value & 0x07);
		        }
		        else
		        {
		        	Loopy_T &= 0xC1F;
		        	Loopy_T |= (ushort)(((value & 0xF8) << 2) | ((value & 0x07) << 12));
		        }
		        
		        writeLatch = !writeLatch;
			}
		}
	
		// $2006 - VRAM Address 2 (Write)
		public ushort VRAMAddress
		{
			get { return Loopy_V; }
		}
		public byte VramAddrReg2
		{
			set
			{
				/* from loopy's docs [I used nes_emu.txt]
       			t is Loopy_V while v is Loopy_T

				2006 first write:
					t:xxABCDEFxxxxxxxx=d:xxABCDEF
			        t:ABxxxxxxxxxxxxxx=0 (bits 14,15 cleared)
			   	2006 second write:
			        t:xxxxxxxxABCDEFGH=d:ABCDEFGH
			        v=t */
			        
			    if (writeLatch) //0x2005 and 0x2006 share the same latch
				{
					Loopy_T &= 0xFF;
					Loopy_T |= (ushort)((value & 0x3F) << 8);
				}
				else
				{
					Loopy_T &= 0x7F00;
					Loopy_T |= value;
					Loopy_V = Loopy_T;
					//Console.WriteLine("WRITE: {0:x}", Loopy_T);
				}
			
				writeLatch = !writeLatch;
			}
		}
	
		// $2002 - PPU Status (Read)
		// Notes: Reading will clear bit 7 and also the $2005/2006 latch (reset)
		private byte status;
		public byte Status
		{
			get 
			{ 
				return status;
			}
			set
			{
				status = value;
			}
		}
		public void OnStatusRead()
		{
			status = (byte)(status & 127); // unset bit 7
			writeLatch = true; // true = 1st write
		}
	
		// Control 1
		public bool RenderBackground = false;
		public bool RenderSprites = false;
		public bool TallSprites 
		{	
			get { return ((Control1 & 32) > 0); }
		}
		public byte NameTableAddress // Bits 0-1, Name Table Address
		{
			get { return (byte)(Control1 & 3); }
			set { Control1 = (byte)((Control1 & 0xFC) | (value)); } // 0xFC - 11111100
		}
		public byte IncrementAddress // Bit 2, Number to increment addr by each read
		{
			get { return (byte)(((Control1 & 4) > 0) ? 32 : 1); }
			set { Control1 = (byte)((Control1 & 0xFB) | (value << 2)); } // 0xFB - 11111011
		}
		public byte BGTable // Bit 4, the Pattern Table BG is stored in [returns 0 or 0x1]
		{
			get { return (byte)((Control1 & 16) >> 4); }
		}
		public byte SprTable
		{
			get { return (byte)((Control1 & 4) >> 3); }
		}
		
		public PPUFlags()
		{
		}
	}
}
