using System;
using System.IO;

namespace NES
{		
	public class Tracelogger
	{
		StreamWriter Writer;

		public int PC;
		public byte[] OpCodes;
		public string OpCodesAsString 
		{
			get 
			{
				string str = string.Empty;
				int count = 0;
				for (int i = 0; i < OpCodes.Length; i++)
				{
					str += String.Format("{0:X2} ", OpCodes[i]);
					count++;
				}
				for (int i = count; i < 3; i++)
					str += "   ";
				return str;
			}
		}
		public string Instruction;
		public string AddressingMode;
		byte X, Y, A;
		CPUFlags Flags;
		public string PFlagsAsString 
		{
			get
			{
				string str = string.Empty;
				str += (Flags.Negative) ? "N" : "n";
				str += (Flags.Overflow) ? "V" : "v";
				str += "u"; // this is actually set.
				str += (Flags.Break) ? "B" : "b";
				str += (Flags.DecimalMode) ? "D" : "d";
				str += (Flags.InterruptDisable) ? "I" : "i";
				str += (Flags.Zero) ? "Z" : "z";
				str += (Flags.Carry) ? "C" : "c";
				return str;
			}
		}
		public bool LineActive = false;
		
		public void StartLine()
		{
			if (LineActive)
				EndLine();
			LineActive = true;
		}
		
		public void EndLine()
		{
			Output();
			Reset();
		}
		
		public void Output()
		{
			//Writer.WriteLine("{0}{1:X}:{2} {3} {4} {5} A:{6:X2} X:{7:X2} Y:{8:X2} P:{9:X2}", "$", PC, OpCodesAsString, Instruction, AddressingMode, GetPadding(AddressingMode, 25), A, X, Y, PFlagsAsString);
		}
		
		public string GetPadding(string str, int width)
		{
			int padding = width - str.Length;
			padding = (padding > 0) ? padding : 0;
			string strpad = string.Empty;
			for (int i = 0; i < padding; i++)
				strpad += " ";
			return strpad;
		}
		
		public void Reset()
		{
			PC = 0;
			OpCodes = null;
			Instruction = string.Empty;
			AddressingMode = string.Empty;
			X = 0; Y = 0; A = 0;
			Flags = new CPUFlags();
			LineActive = false;
		}
		
		public void SetRegisters(byte a, byte x, byte y, CPUFlags flags)
		{
			A = a;
			X = x;
			Y = y;
			Flags = flags;
		}
		
		public Tracelogger(string filename)
		{
			//File.Delete(filename);
			//Writer = new StreamWriter(File.OpenWrite(filename));
			Reset();
		}
		
		~Tracelogger()
		{
			//Writer.Close();
		}
		
	}
}
