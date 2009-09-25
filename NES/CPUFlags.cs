using System;

namespace NES
{
	public class CPUFlags
	{ 
		// Hmm.. Should the Byte be the 'real' var and the bools be properties? hmm hmm.
	
		public bool Carry, Zero, InterruptDisable, DecimalMode, Break, Overflow, Negative;
		public byte Byte
		{
			get 
			{
				byte	b =  toBit(Carry);
						b |= (byte)(toBit(Zero) << 1);
						b |= (byte)(toBit(InterruptDisable) << 2);
						b |= (byte)(toBit(DecimalMode) << 3);
						b |= (byte)(toBit(Break) << 4);
						b |= (byte)(1 << 5); // convention
						b |= (byte)(toBit(Overflow) << 6);
						b |= (byte)(toBit(Negative) << 7);
				return b;
			}
			set
			{
				Carry = (value & 1) > 0;
				Zero = (value & 2) > 0;
				InterruptDisable = (value & 4) > 0;
				DecimalMode = (value & 8) > 0;
				Break = (value & 16) > 0;
				Overflow = (value & 64) > 0;
				Negative = (value & 128) > 0;
			}
		}
		
		private byte toBit(bool val)
		{
			return (byte)((val) ? 1 : 0);
		}

	}

}
