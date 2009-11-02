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
		public bool NMI = false;
		public bool IRQ = false;
		public Tracelogger Debug = new Tracelogger("trace.txt");
		
		public CPU(Engine engine)
		{
			this.Engine = engine;
			this.Stack = new Stack(this.Engine);
		}
		
		public void Reset()
		{
			PC = Engine.ReadMemory16(0xFFFC);
		}
		
		public int Run()
		{
			Cycles = 0;
			CurrentOpCodeLength = 0;
			ChangedPC = false;
			
			// Interrupts
			if (NMI)
			{
				// Handle the NMI (VBlank!)
				Flags.Break = false;
				Stack.Push16(PC);
				Stack.Push8(Flags.Byte);
				Flags.InterruptDisable = true;
				PC = Engine.ReadMemory16(0xFFFA);
				NMI = false;
			} else if (IRQ)
			{
				// Handle the IRQ (APU?)
				Flags.Break = false;
				Stack.Push16(PC);
				Stack.Push8(Flags.Byte);
				Flags.InterruptDisable = true;
				PC = Engine.ReadMemory16(0xFFFE);
			}
			
			// Debug
			Debug.StartLine();
			Debug.SetRegisters(A, X, Y, Flags);
			Debug.PC = PC;
			
			// Call relevant instruction
			CurrentOpCode = Engine.ReadMemory8(PC);
			switch (CurrentOpCode)
			{
				case 0xa9: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xa5: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xb5: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xad: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xbd: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xb9: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xa1: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xb1: Debug.Instruction = "LDA"; opLDA(); break;
				case 0xa2: Debug.Instruction = "LDX"; opLDX(); break;
				case 0xa6: Debug.Instruction = "LDX"; opLDX(); break;
				case 0xb6: Debug.Instruction = "LDX"; opLDX(); break;
				case 0xae: Debug.Instruction = "LDX"; opLDX(); break;
				case 0xbe: Debug.Instruction = "LDX"; opLDX(); break;
				case 0xa0: Debug.Instruction = "LDY"; opLDY(); break;
				case 0xa4: Debug.Instruction = "LDY"; opLDY(); break;
				case 0xb4: Debug.Instruction = "LDY"; opLDY(); break;
				case 0xac: Debug.Instruction = "LDY"; opLDY(); break;
				case 0xbc: Debug.Instruction = "LDY"; opLDY(); break;
				case 0x85: Debug.Instruction = "STA"; opSTA(); break;
				case 0x95: Debug.Instruction = "STA"; opSTA(); break;
				case 0x8d: Debug.Instruction = "STA"; opSTA(); break;
				case 0x9d: Debug.Instruction = "STA"; opSTA(); break;
				case 0x99: Debug.Instruction = "STA"; opSTA(); break;
				case 0x81: Debug.Instruction = "STA"; opSTA(); break;
				case 0x91: Debug.Instruction = "STA"; opSTA(); break;
				case 0x86: Debug.Instruction = "STX"; opSTX(); break;
				case 0x96: Debug.Instruction = "STX"; opSTX(); break;
				case 0x8e: Debug.Instruction = "STX"; opSTX(); break;
				case 0x84: Debug.Instruction = "STY"; opSTY(); break;
				case 0x94: Debug.Instruction = "STY"; opSTY(); break;
				case 0x8c: Debug.Instruction = "STY"; opSTY(); break;
				case 0xaa: Debug.Instruction = "TAX"; opTAX(); break;
				case 0xa8: Debug.Instruction = "TAY"; opTAY(); break;
				case 0x8a: Debug.Instruction = "TXA"; opTXA(); break;
				case 0x98: Debug.Instruction = "TYA"; opTYA(); break;
				case 0xba: Debug.Instruction = "TSX"; opTSX(); break;
				case 0x9a: Debug.Instruction = "TXS"; opTXS(); break;
				case 0x48: Debug.Instruction = "PHA"; opPHA(); break;
				case 0x08: Debug.Instruction = "PHP"; opPHP(); break;
				case 0x68: Debug.Instruction = "PLA"; opPLA(); break;
				case 0x28: Debug.Instruction = "PLP"; opPLP(); break;
				case 0x29: Debug.Instruction = "AND"; opAND(); break;
				case 0x25: Debug.Instruction = "AND"; opAND(); break;
				case 0x35: Debug.Instruction = "AND"; opAND(); break;
				case 0x2d: Debug.Instruction = "AND"; opAND(); break;
				case 0x3d: Debug.Instruction = "AND"; opAND(); break;
				case 0x39: Debug.Instruction = "AND"; opAND(); break;
				case 0x21: Debug.Instruction = "AND"; opAND(); break;
				case 0x31: Debug.Instruction = "AND"; opAND(); break;
				case 0x49: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x45: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x55: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x4d: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x5d: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x59: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x41: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x51: Debug.Instruction = "EOR"; opEOR(); break;
				case 0x09: Debug.Instruction = "ORA"; opORA(); break;
				case 0x05: Debug.Instruction = "ORA"; opORA(); break;
				case 0x15: Debug.Instruction = "ORA"; opORA(); break;
				case 0x0d: Debug.Instruction = "ORA"; opORA(); break;
				case 0x1d: Debug.Instruction = "ORA"; opORA(); break;
				case 0x19: Debug.Instruction = "ORA"; opORA(); break;
				case 0x01: Debug.Instruction = "ORA"; opORA(); break;
				case 0x11: Debug.Instruction = "ORA"; opORA(); break;
				case 0x24: Debug.Instruction = "BIT"; opBIT(); break;
				case 0x2c: Debug.Instruction = "BIT"; opBIT(); break;
				case 0x69: Debug.Instruction = "ADC"; opADC(); break;
				case 0x65: Debug.Instruction = "ADC"; opADC(); break;
				case 0x75: Debug.Instruction = "ADC"; opADC(); break;
				case 0x6d: Debug.Instruction = "ADC"; opADC(); break;
				case 0x7d: Debug.Instruction = "ADC"; opADC(); break;
				case 0x79: Debug.Instruction = "ADC"; opADC(); break;
				case 0x61: Debug.Instruction = "ADC"; opADC(); break;
				case 0x71: Debug.Instruction = "ADC"; opADC(); break;
				case 0xe9: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xe5: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xf5: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xed: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xfd: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xf9: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xe1: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xf1: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xeb: Debug.Instruction = "SBC"; opSBC(); break;
				case 0xc9: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xc5: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xd5: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xcd: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xdd: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xd9: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xc1: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xd1: Debug.Instruction = "CMP"; opCMP(); break;
				case 0xe0: Debug.Instruction = "CPX"; opCPX(); break;
				case 0xe4: Debug.Instruction = "CPX"; opCPX(); break;
				case 0xec: Debug.Instruction = "CPX"; opCPX(); break;
				case 0xc0: Debug.Instruction = "CPY"; opCPY(); break;
				case 0xc4: Debug.Instruction = "CPY"; opCPY(); break;
				case 0xcc: Debug.Instruction = "CPY"; opCPY(); break;
				case 0xe6: Debug.Instruction = "INC"; opINC(); break;
				case 0xf6: Debug.Instruction = "INC"; opINC(); break;
				case 0xee: Debug.Instruction = "INC"; opINC(); break;
				case 0xfe: Debug.Instruction = "INC"; opINC(); break;
				case 0xe8: Debug.Instruction = "INX"; opINX(); break;
				case 0xc8: Debug.Instruction = "INY"; opINY(); break;
				case 0xc6: Debug.Instruction = "DEC"; opDEC(); break;
				case 0xd6: Debug.Instruction = "DEC"; opDEC(); break;
				case 0xce: Debug.Instruction = "DEC"; opDEC(); break;
				case 0xde: Debug.Instruction = "DEC"; opDEC(); break;
				case 0xca: Debug.Instruction = "DEX"; opDEX(); break;
				case 0x88: Debug.Instruction = "DEY"; opDEY(); break;
				case 0x0a: Debug.Instruction = "ASL"; opASL(); break;
				case 0x06: Debug.Instruction = "ASL"; opASL(); break;
				case 0x16: Debug.Instruction = "ASL"; opASL(); break;
				case 0x0e: Debug.Instruction = "ASL"; opASL(); break;
				case 0x1e: Debug.Instruction = "ASL"; opASL(); break;
				case 0x4a: Debug.Instruction = "LSR"; opLSR(); break;
				case 0x46: Debug.Instruction = "LSR"; opLSR(); break;
				case 0x56: Debug.Instruction = "LSR"; opLSR(); break;
				case 0x4e: Debug.Instruction = "LSR"; opLSR(); break;
				case 0x5e: Debug.Instruction = "LSR"; opLSR(); break;
				case 0x2a: Debug.Instruction = "ROL"; opROL(); break;
				case 0x26: Debug.Instruction = "ROL"; opROL(); break;
				case 0x36: Debug.Instruction = "ROL"; opROL(); break;
				case 0x2e: Debug.Instruction = "ROL"; opROL(); break;
				case 0x3e: Debug.Instruction = "ROL"; opROL(); break;
				case 0x6a: Debug.Instruction = "ROR"; opROR(); break;
				case 0x66: Debug.Instruction = "ROR"; opROR(); break;
				case 0x76: Debug.Instruction = "ROR"; opROR(); break;
				case 0x6e: Debug.Instruction = "ROR"; opROR(); break;
				case 0x7e: Debug.Instruction = "ROR"; opROR(); break;
				case 0x4c: Debug.Instruction = "JMP"; opJMP(); break;
				case 0x6c: Debug.Instruction = "JMP"; opJMP(); break;
				case 0x20: Debug.Instruction = "JSR"; opJSR(); break;
				case 0x60: Debug.Instruction = "RTS"; opRTS(); break;
				case 0x90: Debug.Instruction = "BCC"; opBCC(); break;
				case 0xb0: Debug.Instruction = "BCS"; opBCS(); break;
				case 0xf0: Debug.Instruction = "BEQ"; opBEQ(); break;
				case 0x30: Debug.Instruction = "BMI"; opBMI(); break;
				case 0xd0: Debug.Instruction = "BNE"; opBNE(); break;
				case 0x10: Debug.Instruction = "BPL"; opBPL(); break;
				case 0x50: Debug.Instruction = "BVC"; opBVC(); break;
				case 0x70: Debug.Instruction = "BVS"; opBVS(); break;
				case 0x18: Debug.Instruction = "CLC"; opCLC(); break;
				case 0xd8: Debug.Instruction = "CLD"; opCLD(); break;
				case 0x58: Debug.Instruction = "CLI"; opCLI(); break;
				case 0xB8: Debug.Instruction = "CLV"; opCLV(); break;
				case 0x38: Debug.Instruction = "SEC"; opSEC(); break;
				case 0xF8: Debug.Instruction = "SED"; opSED(); break;
				case 0x78: Debug.Instruction = "SEI"; opSEI(); break;
				case 0x00: Debug.Instruction = "BRK"; opBRK(); break;
				case 0xea: Debug.Instruction = "NOP"; opNOP(); break;
				case 0x40: Debug.Instruction = "RTI"; opRTI(); break;
				case 0x82: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x89: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xC2: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xE2: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x1a: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x3a: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x5a: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x7a: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xda: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xfa: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x80: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x04: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x44: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x64: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x14: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x34: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x54: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x74: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xd4: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xf4: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x0c: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x1c: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x3c: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x5c: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0x7c: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xdc: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xfc: Debug.Instruction = "NOPu"; opNOPu(); break;
				case 0xa7: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0xb7: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0xaf: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0xbf: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0xa3: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0xb3: Debug.Instruction = "LAXu"; opLAXu(); break;
				case 0x87: Debug.Instruction = "ASXu"; opASXu(); break;
				case 0x97: Debug.Instruction = "ASXu"; opASXu(); break;
				case 0x8F: Debug.Instruction = "ASXu"; opASXu(); break;
				case 0x83: Debug.Instruction = "ASXu"; opASXu(); break;
				case 0xC7: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xd7: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xcf: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xdf: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xdb: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xc3: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xd3: Debug.Instruction = "DCPu"; opDCPu(); break;
				case 0xe7: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xf7: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xef: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xff: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xfb: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xe3: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0xf3: Debug.Instruction = "ISBu"; opISBu(); break;
				case 0x07: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x17: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x0f: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x1f: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x1b: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x03: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x13: Debug.Instruction = "SLOu"; opSLOu(); break;
				case 0x27: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x37: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x2F: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x3F: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x3b: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x23: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x33: Debug.Instruction = "RLAu"; opRLAu(); break;
				case 0x47: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x57: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x4F: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x5f: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x5b: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x43: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x53: Debug.Instruction = "SREu"; opSREu(); break;
				case 0x67: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x77: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x6f: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x7f: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x7b: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x63: Debug.Instruction = "RRAu"; opRRAu(); break;
				case 0x73: Debug.Instruction = "RRAu"; opRRAu(); break;
				default:
					throw new ArgumentException(String.Format("Unknown opcode: {0:X}", CurrentOpCode));
			}

			// Check we were given a length
			if (CurrentOpCodeLength == 0)
					throw new Exception("0 length opcode: " + Debug.Instruction);
					
					
			// Get the opcode and params for debug
			byte[] bytes = new byte[CurrentOpCodeLength];
			for (int i = 0; i < CurrentOpCodeLength; i++)
				bytes[i] = Engine.ReadMemory8((ushort)(PC+i));
			Debug.OpCodes = bytes;
			
			// Update PC unless JMP etc.
			if (!ChangedPC)
				PC += (ushort)CurrentOpCodeLength;

			// Check cycles were updated
			if (Cycles == 0)
				throw new Exception("No cycles reported");
			
			Debug.EndLine();
			return Cycles;
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
			else
				this.Flags.Zero = false;
			if (isNegative(val)) // MSB set
				this.Flags.Negative = true;
			else
				this.Flags.Negative = false;
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
			byte val = Engine.ReadMemory8((ushort)addr);
			Debug.AddressingMode = String.Format("{0}{1:X} = {2:X}", "$", addr, val);

			CurrentOpCodeLength = 2;			
			return val;
		}
		public byte ZeroPageX(byte addr)
		{
			byte addr2 = (byte)(addr + X);
			byte val = Engine.ReadMemory8((ushort)addr2);
			Debug.AddressingMode = String.Format("{0}{1:X}, X @ {2:X} = {3:X}", "$", addr, addr2, val);
			
			CurrentOpCodeLength = 2;
			return val;
		}
		public byte ZeroPageY(byte addr)
		{
			byte addr2 = (byte)(addr + Y);
			byte val = Engine.ReadMemory8((ushort)addr2);
			Debug.AddressingMode = String.Format("{0}{1:X}, Y @ {2:X} = {3:X}", "$", addr, addr2, val);
			
			CurrentOpCodeLength = 2;
			return val;
		}
		
		public byte Absolute(byte lower, byte upper)
		{
			byte val = Engine.ReadMemory8(sortEndian(lower, upper));
			Debug.AddressingMode = String.Format("{0}{1:X} = {2:X}", "$", sortEndian(lower, upper), val);
		
			CurrentOpCodeLength = 3;
			return val;
		}	
		public byte AbsoluteX(byte lower, byte upper)
		{
			byte val = Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + X));
			Debug.AddressingMode = String.Format("{0}{1:X}, X @ {2:X} = {3:X}", "$", sortEndian(lower, upper), sortEndian(lower, upper) + X, val);
		
			CurrentOpCodeLength = 3;
			return val;
		}
		public byte AbsoluteY(byte lower, byte upper)
		{
			byte val = Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + Y));
			Debug.AddressingMode = String.Format("{0}{1:X}, Y @ {2:X} = {3:X}", "$", sortEndian(lower, upper), sortEndian(lower, upper) + Y, val);
			
			CurrentOpCodeLength = 3;
			return val;
		}
		
		public ushort Indirect(byte lower, byte upper)
		{
			CurrentOpCodeLength = 3;
			
			// Page-boundary bug [Yes, this is me IMPLEMENTING a bug, lolz, silly 6502]
			ushort addr;
			if (lower == 0xFF)
				addr = sortEndian(Engine.ReadMemory8(sortEndian(lower, upper)), Engine.ReadMemory8(sortEndian(0, upper)));
			else
				addr = Engine.ReadMemory16(sortEndian(lower, upper));
			
			Debug.AddressingMode = String.Format("({0}{1:X}) = {2:X}", "$", sortEndian(lower, upper), addr);
		
			return addr;
		}
		public byte IndirectX(byte addr)
		{
			addr += X;
			CurrentOpCodeLength = 2;
			byte addr2 = (byte)(addr + 1); // for second byte (wraps round $FF -> $00 for page-boundary bug)		
			ushort actualAddr = sortEndian(Engine.ReadMemory8((ushort)addr), Engine.ReadMemory8((ushort)addr2));
			byte val = Engine.ReadMemory8(actualAddr);
			Debug.AddressingMode = String.Format("({0:X},X) = {1:X} = {2:X} = {3:X}", addr-X, addr, actualAddr, val);
			return val;
		}
		public byte IndirectY(byte addr)
		{
			byte lower = Engine.ReadMemory8(addr);
			byte upper = Engine.ReadMemory8((byte)(addr+1)); // page bug!
			ushort addr2 = sortEndian(lower, upper);
			byte val = Engine.ReadMemory8((ushort)(addr2 + Y));
			Debug.AddressingMode = String.Format("({0:X}),Y = {1:X} @ {2:X} = {3:X}", addr, addr2, addr2 + Y, val);
			CurrentOpCodeLength = 2;
			return val;	
		}
		
		public ushort IndirectXAddr(byte addr)
		{
			addr += X;
			byte addr2 = (byte)(addr + 1); // for second byte (wraps round $FF -> $00 for page-boundary bug)		
			ushort actualAddr = sortEndian(Engine.ReadMemory8((ushort)addr), Engine.ReadMemory8((ushort)addr2));
			return actualAddr;
		}
		public ushort IndirectYAddr(byte addr)
		{
			byte lower = Engine.ReadMemory8(addr);
			byte upper = Engine.ReadMemory8((byte)(addr+1)); // page bug!
			ushort addr2 = sortEndian(lower, upper);
			return (ushort)(addr2 + Y);
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
				case 0xA0: // Immediate
					Y = val1;
					Cycles += 2;
					CurrentOpCodeLength = 2;
					break;
				case 0xA4: // Zero Page
					Y = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xB4: // Zero Page, X
					Y = ZeroPageX(val1);
					Cycles += 4;
					break;
				case 0xAC: // Absolute
					Y = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xBC: // Absolute, X
					Y = AbsoluteX(val1, val2);
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
					addr = IndirectXAddr(val1);
					Cycles += 6;
					CurrentOpCodeLength = 2;
					break;
				case 0x91: // (Indirect), Y
					addr = IndirectYAddr(val1);
					Cycles += 6;
					CurrentOpCodeLength = 2;
					break; 
				default:
					throw new NotImplementedException();
			}

			Debug.AddressingMode = String.Format("{0}{1:X} (= {2:X}) (ST)", "$", addr, A);
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
			
			Debug.AddressingMode = String.Format("{0}{1:X} (= {2:X}) (ST)", "$", addr, X);
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
				case 0x94: // Zero Page, X
					val1 += X;
					addr = (ushort)val1;
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				case 0x8C: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 3;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Debug.AddressingMode = String.Format("{0}{1:X} (= {2:X}) (ST)", "$", addr, Y);
			Engine.WriteMemory8(addr, Y);
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
			Flags.Byte = (byte)(Stack.Pop8() & 0xEF); // mask out the break bit
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
			Flags.Overflow = ((val & 64) > 0);
			Flags.Negative = ((val & 128) > 0);
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
			Flags.Overflow = (isNegative(A) != (((sbyte)val + ((Flags.Carry) ? 1 : 0) + (sbyte)oldA) < 0)) ? true : false;
			Flags.Carry = ((oldA + toAdd) > 255) ? true : false;
			//Flags.Overflow = ((isNegative(val) ^ isNegative(oldA)) !=  isNegative(A)) ? true : false;
			//Flags.Overflow = (A == 0) ? false : Flags.Overflow; // Signs cannot be wrong if it is 0!
			setZeroNegFlags(A);
		}
		private void opSBC()
		{
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Mode
			switch (CurrentOpCode)
			{
				case 0xEB:
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
			Flags.Overflow = (isNegative(A) != (((sbyte)oldA - ((sbyte)val) - ((!Flags.Carry) ? 1 : 0)) < 0)) ? true : false;
			Flags.Carry = ((oldA - toSub) < 0) ? false : true; // Todo: Check this
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
				case 0xE6: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0xF6: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0xEE: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0xFE: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Debug.AddressingMode += String.Format("(= {0:X2}) (ST)", (byte)(val+1));
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
			
			Debug.AddressingMode += String.Format("(= {0:X2}) (ST)", (byte)(val-1));
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
		{ // OpCode: 0xC8 (Implied) -- Decrement Y Register
			Y--;
			CurrentOpCodeLength = 1;
			Cycles += 2;
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

			CurrentOpCodeLength = 3;
			ChangedPC = true; // So it doesn't change PC
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
			CurrentOpCodeLength = 3;
			ChangedPC = true; // So it doesn't change PC
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
	 		//Console.WriteLine("Warning: BRK used, unsure of implementation.");
	 		
	 		// Save status
	 		Stack.Push16(PC);
	 		Stack.Push8(Flags.Byte);
	 		
	 		// Handle interrupt
	 		Flags.Break = true;
	 		CurrentOpCodeLength = 1;
	 		PC = Engine.ReadMemory16(0xFFFE);
	 		ChangedPC = true; 
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
	 	
	 		CurrentOpCodeLength = 1;
	 		ChangedPC = true; // So it doesn't bug us about CurrentOpCodeLength being 0.
	 		Cycles += 6;	
	 	}
	 	
	 	// Undocumented
	 	private void opNOPu()
	 	{ 
	 		switch (CurrentOpCode)
	 		{
	 			case 0x82: case 0x89: 			// Implied
	 			case 0xC2: case 0xE2: case 0x1A:
	 			case 0x3A: case 0x5A: case 0x7A:
	 			case 0xDA: case 0xFA:
	 				CurrentOpCodeLength = 1;
	 				Cycles += 2;
	 				break;
	 			case 0x80: // Immediate
	 				CurrentOpCodeLength = 2;
	 				Cycles += 2;
	 				break;
			 	case 0x04: case 0x44: case 0x64: // Zero Page
		 			CurrentOpCodeLength = 2;
		 			Cycles += 3;
					break;
				case 0x14: case 0x34: case 0x54:
				case 0x74: case 0xD4: case 0xF4: // Zero Page X
					CurrentOpCodeLength = 2;
					Cycles += 4;
					break;
				case 0x0C: 						 // Absolute
					CurrentOpCodeLength = 3;
					Cycles += 4;
					break;
				case 0x1C: case 0x3C: case 0x5C:
				case 0x7C: case 0xDC: case 0xFC: // Absolute, X
					CurrentOpCodeLength = 3;
					Cycles += 5;
					break;
				default:
					throw new NotImplementedException();
	 		}
	 	}
	 	private void opLAXu()
	 	{ // LDA + LAX
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode?
			switch (CurrentOpCode)
			{
				case 0xA7: // Zero Page
					A = X = ZeroPage(val1);
					Cycles += 3;
					break;
				case 0xB7: // Zero Page, Y
					A = X = ZeroPageY(val1);
					Cycles += 4;
					break;
				case 0xAF: // Absolute
					A = X = Absolute(val1, val2);
					Cycles += 4;
					break;
				case 0xBF: // Absolute, Y
					A = X = AbsoluteY(val1, val2);
					Cycles += 4;
					break;
				case 0xA3: // (Indirect, X)
					A = X = IndirectX(val1);
					Cycles += 6;
					break;
				case 0xB3: // (Indirect), Y
					A = X = IndirectY(val1);
					Cycles += 6;
					break;
				default:
					throw new NotImplementedException();
			}
			
			setZeroNegFlags(A);
	 	}
		private void opASXu()
		{	// Stores (A & X)
			ushort addr = 0;
			byte val1, val2;
			getValues(out val1, out val2);
			
			// Addressing mode? -- Get the address to Store to.
			switch (CurrentOpCode)
			{
				case 0x87: // Zero Page
					addr = (ushort)val1;
					Cycles += 3;
					CurrentOpCodeLength = 2;
					break;
				case 0x97: // Zero Page, Y
					val1 += Y;
					addr = (ushort)val1;
					Cycles += 4;
					CurrentOpCodeLength = 2;
					break;
				case 0x8F: // Absolute
					addr = sortEndian(val1, val2);
					Cycles += 4;
					CurrentOpCodeLength = 3;
					break;
				case 0x83: // (Indirect, X)
					val1 += X;
					addr = Engine.ReadMemory16((ushort)val1);
					Cycles += 6;
					CurrentOpCodeLength = 2;
					break;
				default:
					throw new NotImplementedException();
			}
			
			Engine.WriteMemory8(addr, (byte)(A & X));
		}
		private void opDCPu()
		{ // DEC then CMP
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0xC7: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0xD7: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0xCF: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0xDF: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0xDB: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0xC3: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0xD3: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
			val--;
			Engine.WriteMemory8(addr, (byte)(val));
			setZeroNegFlags((byte)(val));
			Flags.Carry = (A >= val);
			Flags.Zero = (A == val);
			Flags.Negative = isNegative((byte)(A - val));
		}
		private void opISBu()
		{ // INC memory, then sub it from A
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing Modes
			switch (CurrentOpCode)
			{
				case 0xE7: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0xF7: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0xEF: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0xFF: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0xFB: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0xE3: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0xF3: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
			// Increase 
			val++;
			Engine.WriteMemory8(addr, val);
			
			// Do the sub
			byte oldA = A;
			byte toSub = (byte)(val + ((!Flags.Carry) ? 1 : 0));
			A = (byte)(A - toSub);
			
			// Flags
			Flags.Overflow = (isNegative(A) != (((sbyte)oldA - ((sbyte)val) - ((!Flags.Carry) ? 1 : 0)) < 0)) ? true : false;
			Flags.Carry = ((oldA - toSub) < 0) ? false : true; // Todo: Check this
			setZeroNegFlags(A);
		}
		private void opSLOu()
		{ // ASL + ORA
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x07: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x17: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x0F: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x1F: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0x1B: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0x03: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0x13: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
		
			byte newVal = (byte)(val << 1); // ASL
			Engine.WriteMemory8(addr, newVal); // ASL
			A = (byte)(A | newVal); // ORA
			Flags.Carry = ((val & 128) > 0); // ASL
			setZeroNegFlags(A); // ORA [Could be ASL setZeroNegFlags instead?
		}
		private void opRLAu()
		{ // ROL + AND
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x27: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x37: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x2F: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x3F: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0x3B: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0x23: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0x33: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
			// ROL
			byte oldCarry = (byte)((Flags.Carry) ? 1 : 0);
			Flags.Carry = ((val & 128) > 0);
			val = (byte)((val << 1) | oldCarry); // LSB is filled with old Cary Bit
			//setZeroNegFlags(val);
			Engine.WriteMemory8(addr, val);
			
			// AND
			A = (byte)(A & val);
			setZeroNegFlags(A);
		}
		private void opSREu()
		{ // LSR + EOR
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x47: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x57: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x4F: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x5F: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0x5B: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0x43: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0x53: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
			// LSR
			Flags.Carry = ((val & 1) > 0);
			val = (byte)((val >> 1) & 0x7F); // Mask so MSB is always 0.
			//setZeroNegFlags(val);
			Engine.WriteMemory8(addr, val);
			
			// EOR
			A = (byte)(A ^ val);
			setZeroNegFlags(A);
		}
		private void opRRAu()
		{ // ROR + AND
			ushort addr = 0;
			byte val1, val2, val;
			getValues(out val1, out val2);
			
			// Addressing mode
			switch (CurrentOpCode)
			{
				case 0x67: // Zero Page
					val = ZeroPage(val1);
					addr = (ushort)val1;
					Cycles += 5;
					break;
				case 0x77: // Zero Page, X
					val = ZeroPageX(val1);
					val1 += X;
					addr = (ushort)val1;
					Cycles += 6;
					break;
				case 0x6F: // Absolute
					val = Absolute(val1, val2);
					addr = sortEndian(val1, val2);
					Cycles += 6;
					break;
				case 0x7F: // Absolute, X
					val = AbsoluteX(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + X);
					Cycles += 7;
					break;
				case 0x7B: // Absolute, Y
					val = AbsoluteY(val1, val2);
					addr = (ushort)(sortEndian(val1, val2) + Y);
					Cycles += 7;
					break;
				case 0x63: // (Indirect, X)
					val = IndirectX(val1);
					addr = IndirectXAddr(val1);
					Cycles += 9;
					break;
				case 0x73: // (Indirect), Y
					val = IndirectY(val1);
					addr = IndirectYAddr(val1);
					Cycles += 8;
					break;
				default:
					throw new NotImplementedException();
			}
			
			// ROR
			byte oldCarry = (byte)((Flags.Carry) ? 1 : 0);
			Flags.Carry = ((val & 1) > 0);
			val = (byte)((val >> 1) | (oldCarry << 7)); // MSB is filled with old Cary Bit
			// setZeroNegFlags(val);
			Engine.WriteMemory8(addr, val);

			// ADC
			byte oldA = A;
			byte toAdd = (byte)(val + ((Flags.Carry) ? 1 : 0));
			A = (byte)(A + toAdd);
			
			// Flags [ADC]
			Flags.Overflow = (isNegative(A) != (((sbyte)val + ((Flags.Carry) ? 1 : 0) + (sbyte)oldA) < 0)) ? true : false;
			Flags.Carry = ((oldA + toAdd) > 255) ? true : false;
			setZeroNegFlags(A);
			
		}
		
		#endregion
		
	}
}