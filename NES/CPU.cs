using System;

namespace NES
{
	public class CPU
	{
		private Engine Engine;
		public byte X,Y,A,F; // X,Y registers, F processor flags, A accumulator
		public ushort PC;
		public int Cycles = 0;
		public bool ChangedPC = false;
		public int CurrentOpCodeLength = 0;
		public byte CurrentOpCode = 0;
		public CPUFlags Flags = new CPUFlags();
		public Stack Stack;
		
		public CPU(Engine engine)
		{
			this.Engine = engine;
			this.Stack = new Stack(this.Engine);
		}
		
		#region Helper Functions
		
		private void getValues(out byte val1, out byte val2)
		{
			val1 = Engine.ReadMemory8((ushort)(PC+1));
			val2 = Engine.ReadMemory8((ushort)(PC+2));
		}
		
		private ushort sortEndian(byte lower, byte upper)
		{
			return (ushort)(lower | (upper << 8));
		}
		
		private void setZeroNegFlags(byte val)
		{
			if (val == 0)
				this.Flags.Zero = true;
			if (isNegative(val)) // MSB set
				this.Flags.Negative = true;
		}
		
		private bool isNegative(byte val)
		{
			return ((val & 128) > 0) ? true : false;
		}
		
		private void branch(bool test)
		{
			if (test)
			{
				PC = (ushort)(PC + (sbyte)Engine.ReadMemory8((ushort)(PC + 1)));
				Cycles += 3;
			}
			else
				Cycles += 2;
			CurrentOpCodeLength = 2;
		}
		
		#endregion
		
		#region Addressing Modes
		
		public byte ZeroPage(byte addr)
		{
			return Engine.ReadMemory8((ushort)addr);
		}
		public byte ZeroPageX(byte addr)
		{
			addr += X;
			return Engine.ReadMemory8((ushort)addr);
		}
		public byte ZeroPageY(byte addr)
		{
			addr += Y;
			return Engine.ReadMemory8((ushort)addr);
		}
		
		public byte Absolute(byte lower, byte upper)
		{
			return Engine.ReadMemory8(sortEndian(lower, upper));
		}	
		public byte AbsoluteX(byte lower, byte upper)
		{
			return Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + X));
		}
		public byte AbsoluteY(byte lower, byte upper)
		{
			return Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + Y));
		}
		
		public byte Indirect(byte lower, byte upper)
		{
			ushort addr = Engine.ReadMemory16(sortEndian(lower, upper));
			return Engine.ReadMemory8(addr);
		}
		public byte IndirectX(byte addr)
		{
			addr += X;
			return Engine.ReadMemory8((ushort)addr);
		}
		public byte IndirectY(byte addr)
		{
			Console.WriteLine("Warning: using IndirectY Addressing, this implementation could be wrong. Do we wrap on the Least Sig Byte?");
			ushort addr2 = (ushort)(Engine.ReadMemory16((ushort)addr) + Y);
			return Engine.ReadMemory8(addr2);	
		}
		
		#endregion
	
		#region Opcodes
		
		// Load/Store
		private void opLDA()
		{
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode?
			switch (CurrentOpCode)
			{
				case 0xA9: // Immediate
					A = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xA5: // Zero Page
					A = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xB5: // Zero Page, X
					A = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0xAD: // Absolute
					A = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xBD: // Absolute, X
					A = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0xB9: // Absolute, Y
					A = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0xA1: // (Indirect, X)
					A = IndirectX(val1);
					Cycles += 6;
					break;
				case 0xB1: // (Indirect), Y
					A = IndirectY(val1);
					Cycles += 6;
					break;
				default:
					throw new NotImplementedException();
			}
			
			setZeroNegFlags(A);
		}
		private void opLDX()
		{
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode?
			switch (CurrentOpCode)
			{
				case 0xA2: // Immediate
					X = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xA6: // Zero Page
					X = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xB6: // Zero Page, Y
					X = ZeroPageY(val1);
					Cycles += 4;
					break;
				case 0xAE: // Absolute
					X = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xBE: // Absolute, Y
					X = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				default:
					throw new NotImplementedException();
			}
			
			setZeroNegFlags(X);
		}
		private void opLDY()
		{
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode?
			switch (CurrentOpCode)
			{
				case 0xA2: // Immediate
					Y = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xA6: // Zero Page
					Y = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xB6: // Zero Page, Y
					Y = ZeroPageY(val1);
					Cycles += 4;
					break;
				case 0xAE: // Absolute
					Y = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xBE: // Absolute, Y
					Y = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				default:
					throw new NotImplementedException();
			}
			
			setZeroNegFlags(Y);
		}
		private void opSTA()
		{
			ushort addr = 0;
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode? -- Get the address to STore to.
			switch (CurrentOpCode)
			{
				case 0x85: // Zero Page
					addr = (ushort)val1;
					Cycles += 3;
					CurrentOpCodeLength = 2;
					break;
				case 0x95: // Zero Page, X
					val1 += X;
					addr = (ushort)val1;
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				case 0x8D: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 3;
					break;
				case 0x9D: // Absolute, X
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 5;
					CurrentOpCodeLength = 3;
					break;
				case 0x99: // Absolute, Y
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 5;
					CurrentOpCodeLength = 3;
					break;
				case 0x81: // (Indirect, X)
					val1 += X;
					addr = Engine.ReadMemory16((ushort)val1);
					Cycles += 6;
					CurrentOpCodeLength = 2;
					break;
				case 0x91: // (Indirect), Y
					addr = Engine.ReadMemory16((ushort)(val1 + Y));
					Cycles += 6;
					CurrentOpCodeLength = 2;
					break; 
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, A);
		}
		private void opSTX()
		{
			ushort addr = 0;
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode? -- Get the address to STore to.
			switch (CurrentOpCode)
			{
				case 0x86: // Zero Page
					addr = (ushort)val1;
					Cycles += 3;
					CurrentOpCodeLength = 2;
					break;
				case 0x96: // Zero Page, Y
					val1 += Y;
					addr = (ushort)val1;
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				case 0x8E: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 3;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, X);
		}
		private void opSTY()
		{
			ushort addr = 0;
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode? -- Get the address to STore to.
			switch (CurrentOpCode)
			{
				case 0x84: // Zero Page
					addr = (ushort)val1;
					Cycles += 3;
					CurrentOpCodeLength = 2;
					break;
				case 0x94: // Zero Page, Y
					val1 += Y;
					addr = (ushort)val1;
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				case 0x8C: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, X);
		}
		
		// Register Transfers
		private void opTAX()
		{ // OpCode: 0xAA (Implied)
			X = A;
			setZeroNegFlags(X);
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		private void opTAY()
		{ // OpCode: 0xA8 (Implied)
			Y = A;
			setZeroNegFlags(Y);
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		private void opTXA()
		{ // OpCode: 0x8A (Implied)
			A = X;
			setZeroNegFlags(A);
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		private void opTYA()
		{ // OpCode: 0x98 (Implied)
			A = Y;
			setZeroNegFlags(A);
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		
		// Stack Operations
		private void opTSX()
		{ // OpCode: 0xBA (Implied) -- Transfer SP to X
			X = Stack.SP;
			setZeroNegFlags(X);
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		private void opTXS()
		{ // OpCode: 0x9A (Implied) -- Transfer X to SP
			Stack.SP = X;
			Cycles += 2;
			CurrentOpCodeLength++;
		}
		private void opPHA()
		{ // OpCode: 0x48 (Implied) -- Push Accumulator to Stack
			Stack.Push8(A);
			Cycles += 3;
			CurrentOpCodeLength++;
		}
		private void opPHP()
		{ // OpCode: 0x08 (Implied) -- Push Processor Status to Stack
			Stack.Push8(Flags.Byte);
			Cycles += 3;
			CurrentOpCodeLength++;
		}
		private void opPLA()
		{ // OpCode: 0x68 (Implied) -- Pull Accumulator from Stack
			A = Stack.Pop8();
			Cycles += 4;
			CurrentOpCodeLength++;
			setZeroNegFlags(A);
		}
		private void opPLP()
		{ // OpCode: 0x28 (Implied) -- Pull Processor Status from Stack
			Flags.Byte = Stack.Pop8();
			Cycles += 4;
			CurrentOpCodeLength++;
		}
		
		// Logical
		private void opAND()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0x29: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0x25: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0x35: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0x2D: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0x3D: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0x39: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0x21: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0x31: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			A = (byte)(A & val);
			setZeroNegFlags(A);
		}
		private void opEOR()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0x49: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0x45: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0x55: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0x4D: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0x5D: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0x59: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0x41: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0x51: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			A = (byte)(A ^ val);
			setZeroNegFlags(A);
		}
		private void opORA()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0x09: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0x05: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0x15: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0x0D: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0x1D: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0x19: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0x01: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0x11: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			A = (byte)(A | val);
			setZeroNegFlags(A);
		}
		private void opBIT()
		{
			// Description: The mask pattern in A is ANDed with the value in memory to set or clear the zero flag, 
			// but the result is not kept. Bits 7 and 6 of the value from memory are copied into the N and V flags.
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Two Addressing Modes
			switch (CurrentOpCode)
			{
				case 0x24:
					val = ZeroPage(val1);
					Cycles += 3;
					CurrentOpCodeLength = 2;
					break;
				case 0x2C:
					val = Absolute(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 3;
					break;
				default:
					throw new NotImplementedException();
			}
			
			byte temp = (byte)(A & val);
			setZeroNegFlags(temp);
			Flags.Overflow = ((temp & 64) > 0);
		}
		
		// Arithmetic
		private void opADC()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0x69: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0x65: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0x75: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0x6D: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0x7D: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0x79: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0x61: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0x71: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			// Do the add
			byte oldA = A;
			byte toAdd = (byte)(val + ((Flags.Carry) ? 1 : 0));
			A = (byte)(A + toAdd);
			
			// Flags
			Flags.Carry = ((oldA + toAdd) > 255) ? true : false;
			Flags.Overflow = ((isNegative(toAdd) && isNegative(oldA)) !=  isNegative(A)) ? true : false;
			setZeroNegFlags(A);
		}
		private void opSBC()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0xE9: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xE5: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xF5: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0xED: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xFD: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0xF9: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0xE1: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0xF1: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			// Do the sub
			byte oldA = A;
			byte toSub = (byte)(val + ((!Flags.Carry) ? 1 : 0));
			A = (byte)(A - toSub);
			
			// Flags
			Flags.Carry = ((oldA - toSub) < 0) ? false : true; // Todo: Check this.
			Flags.Overflow = ((isNegative(toSub) && isNegative(oldA)) !=  isNegative(A)) ? true : false;
			setZeroNegFlags(A);
		}
		private void opCMP()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0xC9: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xC5: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xD5: // Zero Page, X
					val = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0xCD: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xDD: // Absolute, X
					val = AbsoluteX(val1, val2);
					Cycles += 4;
					break;
				case 0xD9: // Absolute, Y
					val = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0xC1: // (Indirect, X)
					val = IndirectX(val1);
					Cycles += 6;
					break;
				case 0xD1: // (Indirect), Y
					val = IndirectY(val1);
					Cycles += 5;
					break;  
				default:
					throw new NotImplementedException();
			}
			
			Flags.Carry = (A >= val);
			Flags.Zero = (A == val);
			Flags.Negative = isNegative((byte)(A - val));
		}
		private void opCPX()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0xE0: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xE4: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;

				case 0xEC: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Flags.Carry = (X >= val);
			Flags.Zero = (X == val);
			Flags.Negative = isNegative((byte)(X - val));
		}
		private void opCPY()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0xC0: // Immediate
					val = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xC4: // Zero Page
					val = ZeroPage(val1);
					Cycles += 3;
					break;

				case 0xCC: // Absolute
					val = Absolute(val1, val2);
					Cycles += 4;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Flags.Carry = (Y >= val);
			Flags.Zero = (Y == val);
			Flags.Negative = isNegative((byte)(Y - val));
		}
		
		// Increments and Decrements
		private void opINC()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0xA5: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0xB5: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0xAD: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0xBD: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, (byte)(val + 1));
			setZeroNegFlags((byte)(val + 1));
		}
		private void opINX()
		{ // OpCode: 0xE8 (Implied) -- Increment X Register
			X++;
			Cycles += 2;
			CurrentOpCodeLength = 1;
			setZeroNegFlags(X);
		}
		private void opINY()
		{ // OpCode: 0xC8 (Implied) -- Increment Y Register
			Y++;
			Cycles += 2;
			CurrentOpCodeLength = 1;
			setZeroNegFlags(Y);
		}
		private void opDEC()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0xC6: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0xD6: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0xCE: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0xDE: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, (byte)(val - 1));
			setZeroNegFlags((byte)(val - 1));
		}
		private void opDEX()
		{ // OpCode: 0xE8 (Implied) -- Decrement X Register
			X--;
			Cycles += 2;
			CurrentOpCodeLength = 1;
			setZeroNegFlags(X);
		}
		private void opDEY()
		{ // OpCode: 0xC8 (Implied) -- Decrement Cycles += 2;
			CurrentOpCodeLength = 1;
			setZeroNegFlags(Y);
		}
		
		// Shifts
		private void opASL()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x0A: // Implied (Accumulator)
					val = A;
					Cycles += 2;
					CurrentOpCodeLength = 1;
					break;
				case 0x06: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x16: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x0E: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x1E: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			// For all but Implied modes, we must set the memory!
			if (CurrentOpCode != 0x0A)
				Engine.WriteMemory8(addr, (byte)(val << 1));
			else
				A = (byte)(A << 1);
				
			Flags.Carry = ((val & 128) > 0);
			setZeroNegFlags((byte)(val << 1));
		}
		private void opLSR()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x4A: // Implied (Accumulator)
					val = A;
					Cycles += 2;
					CurrentOpCodeLength = 1;
					break;
				case 0x46: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x56: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x4E: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x5E: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Flags.Carry = ((val & 1) > 0);
			val = (byte)((val >> 1) & 0x7F); // Mask so MSB is always 0.
			setZeroNegFlags(val);
			
			// For all but Implied modes, we must set the memory!
			if (CurrentOpCode != 0x4A)
				Engine.WriteMemory8(addr, val);
			else
				A = (byte)(val);
		}
		private void opROL()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x2A: // Implied (Accumulator)
					val = A;
					Cycles += 2;
					CurrentOpCodeLength = 1;
					break;
				case 0x26: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x36: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x2E: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x3E: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			byte oldCarry = (byte)((Flags.Carry) ? 1 : 0);
			Flags.Carry = ((val & 128) > 0);
			val = (byte)((val << 1) | oldCarry); // LSB is filled with old Cary Bit
			setZeroNegFlags(val);
			
			// For all but Implied modes, we must set the memory!
			if (CurrentOpCode != 0x2A)
				Engine.WriteMemory8(addr, val);
			else
				A = (byte)(val);
		}
		private void opROR()
		{
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x6A: // Implied (Accumulator)
					val = A;
					Cycles += 2;
					CurrentOpCodeLength = 1;
					break;
				case 0x66: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x76: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x6E: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x7E: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			byte oldCarry = (byte)((Flags.Carry) ? 1 : 0);
			Flags.Carry = ((val & 1) > 0);
			val = (byte)((val >> 1) | (oldCarry << 7)); // MSB is filled with old Cary Bit
			setZeroNegFlags(val);
			
			// For all but Implied modes, we must set the memory!
			if (CurrentOpCode != 0x6A)
				Engine.WriteMemory8(addr, val);
			else
				A = (byte)(val);
		}
	
		// Jumps and Calls
		private void opJMP()
		{
			// Todo; original 6502 had a glitch about page boundaries, not implementing this.
			ushort addr = 0;
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0x4C: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 3;
					break;
				case 0x6C: // Indirect
					addr = Indirect(val1, val2); 
					Cycles += 5;
					break;
				default:
					throw new NotImplementedException();
			}
			
			CurrentOpCodeLength = 0; // It'll mess up the PC! ):
			ChangedPC = true; // So it doesn't complain CurrentOpCodeLength is 0
			PC = addr;
		}
		private void opJSR()
		{ // OpCode: 0x20 (Absolute) -- Jump to Subroutine
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Push to stack
			Stack.Push16((ushort)(PC + 2)); // Address of next instruction -1 (JSR is 3 bytes, +3 - 1 = +2)
			
			// Go to the address
			Cycles += 6;
			CurrentOpCodeLength = 0; // It'll mess up the PC! ):
			ChangedPC = true; // So it doesn't complain CurrentOpCodeLength is 0
			PC = sortEndian(val1, val2);
		}
		private void opRTS()
		{ // OpCode: 0x60 (Implied) -- Return from Subroutine
			PC = Stack.Pop16();
			CurrentOpCodeLength = 1;
			Cycles += 6;
		}
	
		// Branches
		private void opBCC()
		{ // OpCode: 0x90 (Relative) -- Branch if Carry Clear
			branch(!Flags.Carry);
		}
		private void opBCS()
		{ // OpCode: 0xB0 (Relative) -- Branch if Carry Set
			branch(Flags.Carry);
		}
		private void opBEQ()
		{ // OpCode: 0xF0 (Relative) -- Branch if Equal
			branch(Flags.Zero);
		}
		private void opBMI()
		{ // OpCode: 0x30 (Relative) -- Branch if Minus
			branch(Flags.Negative);
		}
		private void opBNE()
		{ // OpCode: 0xD0 (Relative) -- Branch if Not Equal
			branch(!Flags.Zero);
		}
		private void opBPL()
		{ // OpCode: 0x10 (Relative) -- Branch if Positive
			branch(!Flags.Negative);
		}
		private void opBVC()
		{ // OpCode: 0x50 (Relative) -- Branch if Overflow Clear
			branch(!Flags.Overflow);
		}
		private void opBVS()
		{ // OpCode: 0x70 (Relative) -- Branch if Overflow Set
			branch(Flags.Overflow);
		}
		
		// Status Flag Changes
		private void opCLC()
		{ // OpCode: 0x18 (Implied) -- Clear Carry Flag.
			Flags.Carry = false;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opCLD()
		{ // OpCode: 0xD8 (Implied) -- Clear Decimal Mode.
			Flags.DecimalMode = false;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opCLI()
		{ // OpCode: 0x58 (Implied) -- Clear Interrupt Disable.
			Flags.InterruptDisable = false;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opCLV()
		{ // OpCode: 0xB8 (Implied) -- Clear Overflow Flag.
			Flags.Overflow = false;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opSEC()
		{ // OpCode: 0x38 (Implied) -- Set Carry Flag.
			Flags.Carry = true;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opSED()
		{ // OpCode: 0xF8 (Implied) -- Set Decimal Mode.
			Flags.DecimalMode = true;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
		private void opSEI()
		{ // OpCode: 0x78 (Implied) -- Set Interrupt Disable.
			Flags.InterruptDisable = true;
			CurrentOpCodeLength = 1;
			Cycles += 2;
		}
	
	 	// System Functions
	 	private void opBRK()
	 	{ // OpCode: 0x00 (Implied) -- Force Interrupt
	 		Console.WriteLine("Warning: BRK used, unsure of implementation.");
	 		
	 		// Save status
	 		Stack.Push16(PC);
	 		Stack.Push8(Flags.Byte);
	 		
	 		// Handle interrupt
	 		Flags.Break = true;
	 		PC = Engine.ReadMemory16(0xFFFE);
	 		CurrentOpCodeLength = 0;
	 		ChangedPC = true; // So it doesn't bug us about CurrentOpCodeLength being 0.
	 		Cycles += 7;
	 	}
	 	private void opNOP()
	 	{ // OpCode: 0xEA (Implied) -- No Operation
	 		Cycles += 2;
	 		CurrentOpCodeLength = 1;
	 	}
	 	private void opRTI()
	 	{ // OpCode: 0x40 (Implied) -- Return from Interrupt
	 	
	 		// Recall state from stack.
	 		Flags.Byte = Stack.Pop8();
	 		PC = Stack.Pop16();
	 	
	 		CurrentOpCodeLength = 0;
	 		ChangedPC = true; // So it doesn't bug us about CurrentOpCodeLength being 0.
	 		Cycles += 6;	
	 	}
	
		#endregion
		
	}
}