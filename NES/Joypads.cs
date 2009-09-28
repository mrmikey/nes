using System;

namespace NES
{
	public class Joypads
	{
		private int[] readCount = new int[2];
	
		public Joypads()
		{
		}
		
		// Read and Writes to $4016/$4017
		
		public byte Read(int joyNo)
		{
			readCount[joyNo]++;
			if (readCount[joyNo] <= 8)
				return (byte)0; // return status of controller for the first 8 reads
			else
				return (byte)1; // then return 1!
		}
		
		public void Write(int joyNo, int val)
		{
			if ((joyNo == 0) && (val == 1))
				readCount = new int[2];
		}
		
	}
}
