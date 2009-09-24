using System;

namespace NES
{
	public class CPUFlags
	{ 
		// Hmm.. Should the Byte be the 'real' var and the bools be properties? hmm hmm.
	
		public bool Carry 
		{
			get { return ((Byte & 1) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0))); }
		}
		public bool Zero
		{
			get { return ((Byte & 2) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 1)); }
		}
		public bool InterruptDisable
		{
			get { return ((Byte & 4) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 2)); }
		}
		public bool DecimalMode
		{
			get { return ((Byte & 8) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 3)); }
		}
		public bool Break
		{
			get { return ((Byte & 16) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 4)); }
		}
		// 6th bit is emtpy
		public bool Overflow
		{
			get { return ((Byte & 64) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 6)); }
		}
		public bool Negative
		{
			get { return ((Byte & 128) > 0); }
			set { Byte = (byte)(Byte | (((value) ? 1 : 0) << 7)); }
		}
	
		public byte Byte = 0;
	}

}
