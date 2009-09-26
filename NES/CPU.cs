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
		
		public void Reset()
		{
			PC = 0xc000; //;Engine.ReadMemory16(0xFFFC);
		}
		
		public int Run()
		{
			Cycles = 0;
			CurrentOpCode = Engine.ReadMemory8(PC);
			CurrentOpCodeLength = 0;
			ChangedPC = false;
			
			// Call relevant instruction
			int oldCycles = Cycles;
			switch (CurrentOpCode)
			{
				case 0xa9: debugOp("LDA (0xa9)"); opLDA(); break;
				case 0xa5: debugOp("LDA (0xa5)"); opLDA(); break;
				case 0xb5: debugOp("LDA (0xb5)"); opLDA(); break;
				case 0xad: debugOp("LDA (0xad)"); opLDA(); break;
				case 0xbd: debugOp("LDA (0xbd)"); opLDA(); break;
				case 0xb9: debugOp("LDA (0xb9)"); opLDA(); break;
				case 0xa1: debugOp("LDA (0xa1)"); opLDA(); break;
				case 0xb1: debugOp("LDA (0xb1)"); opLDA(); break;
				case 0xa2: debugOp("LDX (0xa2)"); opLDX(); break;
				case 0xa6: debugOp("LDX (0xa6)"); opLDX(); break;
				case 0xb6: debugOp("LDX (0xb6)"); opLDX(); break;
				case 0xae: debugOp("LDX (0xae)"); opLDX(); break;
				case 0xbe: debugOp("LDX (0xbe)"); opLDX(); break;
				case 0xa0: debugOp("LDY (0xa0)"); opLDY(); break;
				case 0xa4: debugOp("LDY (0xa4)"); opLDY(); break;
				case 0xb4: debugOp("LDY (0xb4)"); opLDY(); break;
				case 0xac: debugOp("LDY (0xac)"); opLDY(); break;
				case 0xbc: debugOp("LDY (0xbc)"); opLDY(); break;
				case 0x85: debugOp("STA (0x85)"); opSTA(); break;
				case 0x95: debugOp("STA (0x95)"); opSTA(); break;
				case 0x8d: debugOp("STA (0x8d)"); opSTA(); break;
				case 0x9d: debugOp("STA (0x9d)"); opSTA(); break;
				case 0x99: debugOp("STA (0x99)"); opSTA(); break;
				case 0x81: debugOp("STA (0x81)"); opSTA(); break;
				case 0x91: debugOp("STA (0x91)"); opSTA(); break;
				case 0x86: debugOp("STX (0x86)"); opSTX(); break;
				case 0x96: debugOp("STX (0x96)"); opSTX(); break;
				case 0x8e: debugOp("STX (0x8e)"); opSTX(); break;
				case 0x84: debugOp("STY (0x84)"); opSTY(); break;
				case 0x94: debugOp("STY (0x94)"); opSTY(); break;
				case 0x8c: debugOp("STY (0x8c)"); opSTY(); break;
				case 0xaa: debugOp("TAX (0xaa)"); opTAX(); break;
				case 0xa8: debugOp("TAY (0xa8)"); opTAY(); break;
				case 0x8a: debugOp("TXA (0x8a)"); opTXA(); break;
				case 0x98: debugOp("TYA (0x98)"); opTYA(); break;
				case 0xba: debugOp("TSX (0xba)"); opTSX(); break;
				case 0x9a: debugOp("TXS (0x9a)"); opTXS(); break;
				case 0x48: debugOp("PHA (0x48)"); opPHA(); break;
				case 0x08: debugOp("PHP (0x08)"); opPHP(); break;
				case 0x68: debugOp("PLA (0x68)"); opPLA(); break;
				case 0x28: debugOp("PLP (0x28)"); opPLP(); break;
				case 0x29: debugOp("AND (0x29)"); opAND(); break;
				case 0x25: debugOp("AND (0x25)"); opAND(); break;
				case 0x35: debugOp("AND (0x35)"); opAND(); break;
				case 0x2d: debugOp("AND (0x2d)"); opAND(); break;
				case 0x3d: debugOp("AND (0x3d)"); opAND(); break;
				case 0x39: debugOp("AND (0x39)"); opAND(); break;
				case 0x21: debugOp("AND (0x21)"); opAND(); break;
				case 0x31: debugOp("AND (0x31)"); opAND(); break;
				case 0x49: debugOp("EOR (0x49)"); opEOR(); break;
				case 0x45: debugOp("EOR (0x45)"); opEOR(); break;
				case 0x55: debugOp("EOR (0x55)"); opEOR(); break;
				case 0x4d: debugOp("EOR (0x4d)"); opEOR(); break;
				case 0x5d: debugOp("EOR (0x5d)"); opEOR(); break;
				case 0x59: debugOp("EOR (0x59)"); opEOR(); break;
				case 0x41: debugOp("EOR (0x41)"); opEOR(); break;
				case 0x51: debugOp("EOR (0x51)"); opEOR(); break;
				case 0x09: debugOp("ORA (0x09)"); opORA(); break;
				case 0x05: debugOp("ORA (0x05)"); opORA(); break;
				case 0x15: debugOp("ORA (0x15)"); opORA(); break;
				case 0x0d: debugOp("ORA (0x0d)"); opORA(); break;
				case 0x1d: debugOp("ORA (0x1d)"); opORA(); break;
				case 0x19: debugOp("ORA (0x19)"); opORA(); break;
				case 0x01: debugOp("ORA (0x01)"); opORA(); break;
				case 0x11: debugOp("ORA (0x11)"); opORA(); break;
				case 0x24: debugOp("BIT (0x24)"); opBIT(); break;
				case 0x2c: debugOp("BIT (0x2c)"); opBIT(); break;
				case 0x69: debugOp("ADC (0x69)"); opADC(); break;
				case 0x65: debugOp("ADC (0x65)"); opADC(); break;
				case 0x75: debugOp("ADC (0x75)"); opADC(); break;
				case 0x6d: debugOp("ADC (0x6d)"); opADC(); break;
				case 0x7d: debugOp("ADC (0x7d)"); opADC(); break;
				case 0x79: debugOp("ADC (0x79)"); opADC(); break;
				case 0x61: debugOp("ADC (0x61)"); opADC(); break;
				case 0x71: debugOp("ADC (0x71)"); opADC(); break;
				case 0xe9: debugOp("SBC (0xe9)"); opSBC(); break;
				case 0xe5: debugOp("SBC (0xe5)"); opSBC(); break;
				case 0xf5: debugOp("SBC (0xf5)"); opSBC(); break;
				case 0xed: debugOp("SBC (0xed)"); opSBC(); break;
				case 0xfd: debugOp("SBC (0xfd)"); opSBC(); break;
				case 0xf9: debugOp("SBC (0xf9)"); opSBC(); break;
				case 0xe1: debugOp("SBC (0xe1)"); opSBC(); break;
				case 0xf1: debugOp("SBC (0xf1)"); opSBC(); break;
				case 0xeb: debugOp("SBC (0xeb)"); opSBC(); break;
				case 0xc9: debugOp("CMP (0xc9)"); opCMP(); break;
				case 0xc5: debugOp("CMP (0xc5)"); opCMP(); break;
				case 0xd5: debugOp("CMP (0xd5)"); opCMP(); break;
				case 0xcd: debugOp("CMP (0xcd)"); opCMP(); break;
				case 0xdd: debugOp("CMP (0xdd)"); opCMP(); break;
				case 0xd9: debugOp("CMP (0xd9)"); opCMP(); break;
				case 0xc1: debugOp("CMP (0xc1)"); opCMP(); break;
				case 0xd1: debugOp("CMP (0xd1)"); opCMP(); break;
				case 0xe0: debugOp("CPX (0xe0)"); opCPX(); break;
				case 0xe4: debugOp("CPX (0xe4)"); opCPX(); break;
				case 0xec: debugOp("CPX (0xec)"); opCPX(); break;
				case 0xc0: debugOp("CPY (0xc0)"); opCPY(); break;
				case 0xc4: debugOp("CPY (0xc4)"); opCPY(); break;
				case 0xcc: debugOp("CPY (0xcc)"); opCPY(); break;
				case 0xe6: debugOp("INC (0xe6)"); opINC(); break;
				case 0xf6: debugOp("INC (0xf6)"); opINC(); break;
				case 0xee: debugOp("INC (0xee)"); opINC(); break;
				case 0xfe: debugOp("INC (0xfe)"); opINC(); break;
				case 0xe8: debugOp("INX (0xe8)"); opINX(); break;
				case 0xc8: debugOp("INY (0xc8)"); opINY(); break;
				case 0xc6: debugOp("DEC (0xc6)"); opDEC(); break;
				case 0xd6: debugOp("DEC (0xd6)"); opDEC(); break;
				case 0xce: debugOp("DEC (0xce)"); opDEC(); break;
				case 0xde: debugOp("DEC (0xde)"); opDEC(); break;
				case 0xca: debugOp("DEX (0xca)"); opDEX(); break;
				case 0x88: debugOp("DEY (0x88)"); opDEY(); break;
				case 0x0a: debugOp("ASL (0x0a)"); opASL(); break;
				case 0x06: debugOp("ASL (0x06)"); opASL(); break;
				case 0x16: debugOp("ASL (0x16)"); opASL(); break;
				case 0x0e: debugOp("ASL (0x0e)"); opASL(); break;
				case 0x1e: debugOp("ASL (0x1e)"); opASL(); break;
				case 0x4a: debugOp("LSR (0x4a)"); opLSR(); break;
				case 0x46: debugOp("LSR (0x46)"); opLSR(); break;
				case 0x56: debugOp("LSR (0x56)"); opLSR(); break;
				case 0x4e: debugOp("LSR (0x4e)"); opLSR(); break;
				case 0x5e: debugOp("LSR (0x5e)"); opLSR(); break;
				case 0x2a: debugOp("ROL (0x2a)"); opROL(); break;
				case 0x26: debugOp("ROL (0x26)"); opROL(); break;
				case 0x36: debugOp("ROL (0x36)"); opROL(); break;
				case 0x2e: debugOp("ROL (0x2e)"); opROL(); break;
				case 0x3e: debugOp("ROL (0x3e)"); opROL(); break;
				case 0x6a: debugOp("ROR (0x6a)"); opROR(); break;
				case 0x66: debugOp("ROR (0x66)"); opROR(); break;
				case 0x76: debugOp("ROR (0x76)"); opROR(); break;
				case 0x6e: debugOp("ROR (0x6e)"); opROR(); break;
				case 0x7e: debugOp("ROR (0x7e)"); opROR(); break;
				case 0x4c: debugOp("JMP (0x4c)"); opJMP(); break;
				case 0x6c: debugOp("JMP (0x6c)"); opJMP(); break;
				case 0x20: debugOp("JSR (0x20)"); opJSR(); break;
				case 0x60: debugOp("RTS (0x60)"); opRTS(); break;
				case 0x90: debugOp("BCC (0x90)"); opBCC(); break;
				case 0xb0: debugOp("BCS (0xb0)"); opBCS(); break;
				case 0xf0: debugOp("BEQ (0xf0)"); opBEQ(); break;
				case 0x30: debugOp("BMI (0x30)"); opBMI(); break;
				case 0xd0: debugOp("BNE (0xd0)"); opBNE(); break;
				case 0x10: debugOp("BPL (0x10)"); opBPL(); break;
				case 0x50: debugOp("BVC (0x50)"); opBVC(); break;
				case 0x70: debugOp("BVS (0x70)"); opBVS(); break;
				case 0x18: debugOp("CLC (0x18)"); opCLC(); break;
				case 0xd8: debugOp("CLD (0xd8)"); opCLD(); break;
				case 0x58: debugOp("CLI (0x58)"); opCLI(); break;
				case 0xB8: debugOp("CLV (0xB8)"); opCLV(); break;
				case 0x38: debugOp("SEC (0x38)"); opSEC(); break;
				case 0xF8: debugOp("SED (0xF8)"); opSED(); break;
				case 0x78: debugOp("SEI (0x78)"); opSEI(); break;
				case 0x00: debugOp("BRK (0x00)"); opBRK(); break;
				case 0xea: debugOp("NOP (0xea)"); opNOP(); break;
				case 0x40: debugOp("RTI (0x40)"); opRTI(); break;
				case 0x82: debugOp("NOPu (0x82)"); opNOPu(); break;
				case 0x89: debugOp("NOPu (0x89)"); opNOPu(); break;
				case 0xC2: debugOp("NOPu (0xC2)"); opNOPu(); break;
				case 0xE2: debugOp("NOPu (0xE2)"); opNOPu(); break;
				case 0x1a: debugOp("NOPu (0x1a)"); opNOPu(); break;
				case 0x3a: debugOp("NOPu (0x3a)"); opNOPu(); break;
				case 0x5a: debugOp("NOPu (0x5a)"); opNOPu(); break;
				case 0x7a: debugOp("NOPu (0x7a)"); opNOPu(); break;
				case 0xda: debugOp("NOPu (0xda)"); opNOPu(); break;
				case 0xfa: debugOp("NOPu (0xfa)"); opNOPu(); break;
				case 0x80: debugOp("NOPu (0x80)"); opNOPu(); break;
				case 0x04: debugOp("NOPu (0x04)"); opNOPu(); break;
				case 0x44: debugOp("NOPu (0x44)"); opNOPu(); break;
				case 0x64: debugOp("NOPu (0x64)"); opNOPu(); break;
				case 0x14: debugOp("NOPu (0x14)"); opNOPu(); break;
				case 0x34: debugOp("NOPu (0x34)"); opNOPu(); break;
				case 0x54: debugOp("NOPu (0x54)"); opNOPu(); break;
				case 0x74: debugOp("NOPu (0x74)"); opNOPu(); break;
				case 0xd4: debugOp("NOPu (0xd4)"); opNOPu(); break;
				case 0xf4: debugOp("NOPu (0xf4)"); opNOPu(); break;
				case 0x0c: debugOp("NOPu (0x0c)"); opNOPu(); break;
				case 0x1c: debugOp("NOPu (0x1c)"); opNOPu(); break;
				case 0x3c: debugOp("NOPu (0x3c)"); opNOPu(); break;
				case 0x5c: debugOp("NOPu (0x5c)"); opNOPu(); break;
				case 0x7c: debugOp("NOPu (0x7c)"); opNOPu(); break;
				case 0xdc: debugOp("NOPu (0xdc)"); opNOPu(); break;
				case 0xfc: debugOp("NOPu (0xfc)"); opNOPu(); break;
				case 0xa7: debugOp("LAXu (0xa7)"); opLAXu(); break;
				case 0xb7: debugOp("LAXu (0xb7)"); opLAXu(); break;
				case 0xaf: debugOp("LAXu (0xaf)"); opLAXu(); break;
				case 0xbf: debugOp("LAXu (0xbf)"); opLAXu(); break;
				case 0xa3: debugOp("LAXu (0xa3)"); opLAXu(); break;
				case 0xb3: debugOp("LAXu (0xb3)"); opLAXu(); break;
				case 0x87: debugOp("ASXu (0x87)"); opASXu(); break;
				case 0x97: debugOp("ASXu (0x97)"); opASXu(); break;
				case 0x8F: debugOp("ASXu (0x8F)"); opASXu(); break;
				case 0x83: debugOp("ASXu (0x83)"); opASXu(); break;
				case 0xC7: debugOp("DCPu (0xC7)"); opDCPu(); break;
				case 0xd7: debugOp("DCPu (0xd7)"); opDCPu(); break;
				case 0xcf: debugOp("DCPu (0xcf)"); opDCPu(); break;
				case 0xdf: debugOp("DCPu (0xdf)"); opDCPu(); break;
				case 0xdb: debugOp("DCPu (0xdb)"); opDCPu(); break;
				case 0xc3: debugOp("DCPu (0xc3)"); opDCPu(); break;
				case 0xd3: debugOp("DCPu (0xd3)"); opDCPu(); break;
				case 0xe7: debugOp("ISPu (0xe7)"); opISBu(); break;
				case 0xf7: debugOp("ISPu (0xf7)"); opISBu(); break;
				case 0xef: debugOp("ISPu (0xef)"); opISBu(); break;
				case 0xff: debugOp("ISPu (0xff)"); opISBu(); break;
				case 0xfb: debugOp("ISPu (0xfb)"); opISBu(); break;
				case 0xe3: debugOp("ISPu (0xe3)"); opISBu(); break;
				case 0xf3: debugOp("ISPu (0xf3)"); opISBu(); break;
				case 0x07: debugOp("SLOu (0x07)"); opSLOu(); break;
				case 0x17: debugOp("SLOu (0x17)"); opSLOu(); break;
				case 0x0f: debugOp("SLOu (0x0f)"); opSLOu(); break;
				case 0x1f: debugOp("SLOu (0x1f)"); opSLOu(); break;
				case 0x1b: debugOp("SLOu (0x1b)"); opSLOu(); break;
				case 0x03: debugOp("SLOu (0x03)"); opSLOu(); break;
				case 0x13: debugOp("SLOu (0x13)"); opSLOu(); break;
				case 0x27: debugOp("RLAu (0x27)"); opRLAu(); break;
				case 0x37: debugOp("RLAu (0x37)"); opRLAu(); break;
				case 0x2F: debugOp("RLAu (0x2F)"); opRLAu(); break;
				case 0x3F: debugOp("RLAu (0x3F)"); opRLAu(); break;
				case 0x3b: debugOp("RLAu (0x3b)"); opRLAu(); break;
				case 0x23: debugOp("RLAu (0x23)"); opRLAu(); break;
				case 0x33: debugOp("RLAu (0x33)"); opRLAu(); break;
				case 0x47: debugOp("SREu (0x47)"); opSREu(); break;
				case 0x57: debugOp("SREu (0x57)"); opSREu(); break;
				case 0x4F: debugOp("SREu (0x4F)"); opSREu(); break;
				case 0x5f: debugOp("SREu (0x5f)"); opSREu(); break;
				case 0x5b: debugOp("SREu (0x5b)"); opSREu(); break;
				case 0x43: debugOp("SREu (0x43)"); opSREu(); break;
				case 0x53: debugOp("SREu (0x53)"); opSREu(); break;
				case 0x67: debugOp("RRAu (0x67)"); opRRAu(); break;
				case 0x77: debugOp("RRAu (0x77)"); opRRAu(); break;
				case 0x6f: debugOp("RRAu (0x6f)"); opRRAu(); break;
				case 0x7f: debugOp("RRAu (0x7f)"); opRRAu(); break;
				case 0x7b: debugOp("RRAu (0x7b)"); opRRAu(); break;
				case 0x63: debugOp("RRAu (0x63)"); opRRAu(); break;
				case 0x73: debugOp("RRAu (0x73)"); opRRAu(); break;
				default:
					throw new ArgumentException(String.Format("Unknown opcode: {0:x}", CurrentOpCode));
			}
			debugOpRegs();
			
			// Check we were given a length unless it's a jmp/branch etc.
			if (!ChangedPC)
			{
				if (CurrentOpCodeLength == 0)
					throw new Exception();
				else
					PC += (ushort)CurrentOpCodeLength;
			}
			
			// Check cycles were updated
			if (Cycles == 0)
				throw new Exception();
			
			return Cycles;
		}
		
		#region Helper Functions
		
		private bool debugLatch = false;
		private void debugOp(string op)
		{
			if (debugLatch == true)
				Console.WriteLine("");
				
			Console.Write("{0:x}\t\t {1} ", PC, op);
			debugLatch = true;
		}
		private void debugOpRegs()
		{
			Console.WriteLine("\t\t A:{0:x} P:{1:x}", A, Flags.Byte);
			debugLatch = false;
		}
		
		
		
		public void debugOpMem(string mem, params object[] format)
		{
			Console.Write(String.Format(mem, format));
			
		}
		
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
			CurrentOpCodeLength = 2;
			debugOpMem("ZeroPage({0})={1}", addr, Engine.ReadMemory8((ushort)addr));
			return Engine.ReadMemory8((ushort)addr);
		}
		public byte ZeroPageX(byte addr)
		{
			debugOpMem("ZeroPageX({0}, {1})={2}", addr, X, Engine.ReadMemory8((ushort)addr));
			addr += X;
			CurrentOpCodeLength = 2;
			return Engine.ReadMemory8((ushort)addr);
		}
		public byte ZeroPageY(byte addr)
		{
			debugOpMem("ZeroPageY({0}, {1})={2}", addr, Y, Engine.ReadMemory8((ushort)addr));
			addr += Y;
			CurrentOpCodeLength = 2;
			return Engine.ReadMemory8((ushort)addr);
		}
		
		public byte Absolute(byte lower, byte upper)
		{
			CurrentOpCodeLength = 3;
			debugOpMem("Absolute({0})={1}", sortEndian(lower, upper), Engine.ReadMemory8(sortEndian(lower, upper)));
			return Engine.ReadMemory8(sortEndian(lower, upper));
		}	
		public byte AbsoluteX(byte lower, byte upper)
		{
			CurrentOpCodeLength = 3;
			debugOpMem("AbsoluteX({0:x},{2:x})={1:x}", sortEndian(lower, upper), Engine.ReadMemory8(sortEndian(lower, upper)), X);
			return Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + X));
		}
		public byte AbsoluteY(byte lower, byte upper)
		{
			CurrentOpCodeLength = 3;
			debugOpMem("AbsoluteY({0:x},{2:x})={1:x}", sortEndian(lower, upper), Engine.ReadMemory8(sortEndian(lower, upper)), Y);
			return Engine.ReadMemory8((ushort)(sortEndian(lower, upper) + Y));
		}
		
		public ushort Indirect(byte lower, byte upper)
		{
			debugOpMem("Indirect");
			CurrentOpCodeLength = 3;
			
			// Page-boundary bug [Yes, this is me IMPLEMENTING a bug, lolz, silly 6502]
			ushort addr;
			if (lower == 0xFF)
				addr = sortEndian(Engine.ReadMemory8(sortEndian(lower, upper)), Engine.ReadMemory8(sortEndian(0, upper)));
			else
				addr = Engine.ReadMemory16(sortEndian(lower, upper));
		
			return addr;
		}
		public byte IndirectX(byte addr)
		{
		//	debugOpMem("IndirectX");
			addr += X;
			CurrentOpCodeLength = 2;
			byte addr2 = (byte)(addr + 1); // for second byte (wraps round $FF -> $00 for page-boundary bug)		
			ushort actualAddr = sortEndian(Engine.ReadMemory8((ushort)addr), Engine.ReadMemory8((ushort)addr2));
			byte val = Engine.ReadMemory8(actualAddr);
			debugOpMem("({0:x},X) = {1:x} = {2:x} = {3:x}", addr-X, addr, actualAddr, val);
			return val;
		}
		public byte IndirectY(byte addr)
		{
			//Console.WriteLine("Warning: using IndirectY Addressing, this implementation could be wrong. Do we wrap on the Least Sig Byte?");

			byte lower = Engine.ReadMemory8(addr);
			byte upper = Engine.ReadMemory8((byte)(addr+1)); // page bug!
			ushort addr2 = sortEndian(lower, upper);
			byte val = Engine.ReadMemory8((ushort)(addr2 + Y));
			debugOpMem("({0:x}),Y = {1:x} @ {2:x} = {3:x}", addr, addr2, addr2 + Y, val);
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
	 		//Console.WriteLine("Warning: BRK used, unsure of implementation.");
	 		
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