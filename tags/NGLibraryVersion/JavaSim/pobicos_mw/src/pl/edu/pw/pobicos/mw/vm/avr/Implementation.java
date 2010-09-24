package pl.edu.pw.pobicos.mw.vm.avr;

import pl.edu.pw.pobicos.mw.agent.NonGenericAgent;
import pl.edu.pw.pobicos.mw.instruction.InstructionMap;
import pl.edu.pw.pobicos.mw.instruction.ReturnValue;

/**
 * Implements behavior on each opcode.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Implementation {
	
	private State state;
	
	private AvrVM parent;

	/**
	 * Constructor.
	 * @param state virtual machine's registers
	 * @param parent virtual machine
	 */
	public Implementation(State state, AvrVM parent)
	{
		this.state = state;
		this.parent = parent;
	}

	/************************************ OPCODEs_generic.c *******************************************/
	private void COPY_INSTR_NAME(String str)
	{
		//parent.log.copyInstrName(str);
		//parent.log.print_regs();
		//parent.log.print_data(parent.getCurrAgent());
	}

	protected void OP_MOV()
	{
	    COPY_INSTR_NAME("MOV\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x1f0) >> 4] = state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)];
	    state._PC++;
	}

	protected void OP_MOVW()
	{
	    COPY_INSTR_NAME("MOVW\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x0f0) >> 3] = state.regs[(OPCODE & 0x0f) << 1];
	    state.regs[((OPCODE & 0x0f0) >> 3) + 1] = state.regs[((OPCODE & 0x0f) << 1) + 1];
	    state._PC++;
	}

	protected void OP_LDI()
	{
	    COPY_INSTR_NAME("LDI\n");
	    
		int OPCODE = state.opcode;
	    short reg_num = (short)(((OPCODE & 0x00f0) >> 4) + 16);
	    state.regs[reg_num] = (byte)((OPCODE & 0x0f) | ((OPCODE & 0x0f00) >> 4));
	    state._PC++;
	}

	protected void OP_SER()
	{
	    COPY_INSTR_NAME("SER\n");
	    
		int OPCODE = state.opcode;
	    short reg_num = (short)(((OPCODE & 0x00f0) >> 4) + 16);
	    state.regs[reg_num] = (byte)0x0ff;
	    state._PC++;
	}

	protected void OP_LDS()
	{
	    COPY_INSTR_NAME("LDS\n");
	    
		int OPCODE = state.opcode;
	    int address;
	    short destReg = (short)((OPCODE & 0x01f0) >> 4);
	    state._PC++;
	    address = code_get_opcode_extension();
	    state.regs[destReg] = data_ld(address);
	    state._PC++;
	}

	private void LD(short r)
	{
	    switch (r) {
	        case 26:
	            COPY_INSTR_NAME("LDX\n");
	            break;
	        case 28:
	            COPY_INSTR_NAME("LDY\n");
	            break;
	        case 30:
	            COPY_INSTR_NAME("LDZ\n");
	            break;
	        }
	    int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[r]) | (((0x000000ff & (int) state.regs[r + 1]) << 8));
	    short destReg = (short)((OPCODE & 0x01f0) >> 4);

	    state.regs[destReg] = data_ld(address);
	    state._PC++;
	}

	private void LD_ADD(short r)
	{
	    LD(r);

	    // post-increment address
	    if (state.regs[r] == (byte)0x0ff) {
	    	state.regs[r] = 0x00;
	    	state.regs[r + 1]++;
	    } else
	        state.regs[r]++;
	}

	private void SUB_LD(short r)
	{
	    // pre-decrement address
	    if (state.regs[r] == 0x00) {
	    	state.regs[r + 1]--;
	    	state.regs[r] = (byte)0x0ff;
	    } else
	    	state.regs[r]--;

	    LD(r);
	}

	protected void OP_LDX()
	{
		LD((short)26);
	}
	protected void OP_LDX_ADD() 
	{
		LD_ADD((short)26);
	}
	protected void OP_SUB_LDX()
	{
		SUB_LD((short)26);
	}

	protected void OP_LDY()
	{
		LD((short)28);
	}
	protected void OP_LDY_ADD()
	{
		LD_ADD((short)28);
	}
	protected void OP_SUB_LDY()
	{
		SUB_LD((short)28);
	}

	protected void OP_LDD_Y()
	{
	    COPY_INSTR_NAME("LDD_Y\n");
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[28]) | (((0x000000ff & (int) state.regs[29]) << 8));
	    short destReg;

	    // add displacement
	    address += (OPCODE & 0x07);
	    address += ((OPCODE >> 7) & 0x018);
	    address += ((OPCODE >> 8) & 0x020);

	    destReg = (short)((OPCODE & 0x01f0) >> 4);
	    state.regs[destReg] = data_ld(address);
	    state._PC++;
	}

	protected void OP_LDZ()
	{
		LD((short)30);
	}
	protected void OP_LDZ_ADD()
	{
		LD_ADD((short)30);
	}
	protected void OP_SUB_LDZ()
	{
		SUB_LD((short)30);
	}

	protected void OP_LDD_Z()
	{
	    COPY_INSTR_NAME("LDD_Z\n");
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[30]) | (((0x000000ff & (int) state.regs[31]) << 8));
	    short destReg;

	    // add displacement
	    address += (OPCODE & 0x07);
	    address += ((OPCODE >> 7) & 0x018);
	    address += ((OPCODE >> 8) & 0x020);

	    destReg = (short)((OPCODE & 0x01f0) >> 4);

	    state.regs[destReg] = data_ld(address);
	    state._PC++;
	}

	protected void OP_STS()
	{
	    COPY_INSTR_NAME("STS\n");
	    
		int OPCODE = state.opcode;
	    short srcReg = (short)((OPCODE & 0x01f0) >> 4);
	    int address;

	    state._PC++;
	    address = code_get_opcode_extension();
	    data_st(address, state.regs[srcReg]);
	    state._PC++;
	}

	private void ST(short r)
	{
	    switch (r) {
	        case 26:
	            COPY_INSTR_NAME("STX\n");
	            break;
	        case 28:
	            COPY_INSTR_NAME("STY\n");
	            break;
	        case 30:
	            COPY_INSTR_NAME("STZ\n");
	            break;
	        }
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[r]) | ((0x000000ff & (int) state.regs[r + 1]) << 8);
	    short srcReg = (short)((OPCODE & 0x01f0) >> 4);

	    data_st(address, state.regs[srcReg]);
	    state._PC++;
	}

	private void ST_ADD(short r)
	{
	    ST(r);

	    // post-increment address
	    if (state.regs[r] == (byte)0x0ff) {
	    	state.regs[r] = 0x00;
	    	state.regs[r + 1]++;
	    } else
	    	state.regs[r]++;
	}

	private void SUB_ST(short r)
	{
	    // pre-decrement address
	    if (state.regs[r] == 0x00) {
	    	state.regs[r + 1]--;
	    	state.regs[r] = (byte)0x0ff;
	    } else
	    	state.regs[r]--;

	    ST(r);
	}

	protected void OP_STX()
	{
		ST((short)26);
	}
	protected void OP_STX_ADD()
	{
		ST_ADD((short)26);
	}
	protected void OP_SUB_STX()
	{
		SUB_ST((short)26);
	}

	protected void OP_STY()
	{
		ST((short)28);
	}
	protected void OP_STY_ADD()
	{
		ST_ADD((short)28);
	}
	protected void OP_SUB_STY()
	{
		SUB_ST((short)28);
	}

	protected void OP_STD_Y()
	{
	    COPY_INSTR_NAME("STD_Y\n");
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[28]) | ((0x000000ff & (int) state.regs[29]) << 8);
	    short srcReg;

	    // add displacement
	    address += (OPCODE & 0x07);
	    address += ((OPCODE >> 7) & 0x018);
	    address += ((OPCODE >> 8) & 0x020);

	    srcReg = (short)((OPCODE & 0x01f0) >> 4);

	    data_st(address, state.regs[srcReg]);
	    state._PC++;
	}

	protected void OP_STZ()
	{
		ST((short)30);
	}
	protected void OP_STZ_ADD()
	{
		ST_ADD((short)30);
	}
	protected void OP_SUB_STZ()
	{
		SUB_ST((short)30);
	}

	protected void OP_STD_Z()
	{
	    COPY_INSTR_NAME("STD_Z\n");
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[30]) + ((0x000000ff & (int) state.regs[31]) << 8);
	    short srcReg;

	    // add displacement
	    address += (OPCODE & 0x07);
	    address += ((OPCODE >> 7) & 0x018);
	    address += ((OPCODE >> 8) & 0x020);

	    srcReg = (short)((OPCODE & 0x01f0) >> 4);

	    data_st(address, state.regs[srcReg]);
	    state._PC++;
	}

	protected void OP_LPM()
	{
	    COPY_INSTR_NAME("LPM\n");
	    
		int OPCODE = state.opcode;
	    int address = (0x000000ff & (int) state.regs[30]) + ((0x000000ff & (int) state.regs[31]) << 8);
	    byte data = code_lpm(address);

	    if (OPCODE == 0x095c8) {
	        // implicit destination register is R0
	        state.regs[0] = data;
	    } else {
	    	state.regs[(OPCODE & 0x01f0) >> 4] = data;
	    }

	    if ((OPCODE & 0x0f) == 0x05) {
	        // this is a post-increment LPM variant
	        if (state.regs[30] == (byte)0x0ff) {
	        	state.regs[30] = 0x00;
	        	state.regs[31]++;
	        } else
	        	state.regs[30]++;
	    }
	    state._PC++;
	}

	protected void OP_IN()
	{
	    COPY_INSTR_NAME("IN\n");
	    
		int OPCODE = state.opcode;
	    short io_addr = (short)(((OPCODE & 0x0f) | ((OPCODE >> 5) & 0x30)));
	    state.regs[(OPCODE & 0x01f0) >> 4] = data_ld(io_addr + 0x20);
	    state._PC++;
	}

	protected void OP_OUT()
	{
	    COPY_INSTR_NAME("OUT\n");
	    
		int OPCODE = state.opcode;
	    short io_addr = (short)(((OPCODE & 0x0f) | ((OPCODE >> 5) & 0x30)));
	    data_st(io_addr + 0x20, state.regs[(OPCODE & 0x01f0) >> 4]);
	    state._PC++;
	}

	protected void OP_PUSH()
	{
	    COPY_INSTR_NAME("PUSH\n");
		int OPCODE = state.opcode;
	    stack_push((short)(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]));
	    state._PC++;
	}

	protected void OP_POP()
	{
	    COPY_INSTR_NAME("POP\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] = stack_pop();
	    state._PC++;
	}

	protected void OP_JUDGE_C_H_ADD(int p, int q, short c, short s)
	{
	    if (s == 8) {
	        if (p + q + c > 255)
	            state._SREG |= 0x01;      // set C bit
	        else
	            state._SREG &= 0x0fe;     // clear C bit
	        if ((p & 0x0f) + (q & 0x0f) + c > 15)
	            state._SREG |= 0x20;     // set H bit
	        else
	            state._SREG &= 0x0df;     // clear H bit
	    } else if (s == 16) {
	        // if(p+q+c>65535)
	        if ((long) p + (long) q + (long) c > 65535)
	            state._SREG |= 0x01;      // set C bit
	        else
	            state._SREG &= 0x0fe;     // clear C bit
	    }
	}

//	void OP_JUDGE_C_H_SUB(int p,int q,int c,int s)
	protected void OP_JUDGE_C_H_SUB(int p, int q, short c, short s)
//	 end
	{
	    if (p >= (q + c))
	        state._SREG &= 0x0fe;         // clear C bit
	    else if (p < (q + c))
	        state._SREG |= 0x01;            // set C bit
	    if (s == 8) {
	        if ((p & 0x0f) >= ((q & 0x0f) + c))
	            state._SREG &= 0x0df;     // clear H bit
	        else if ((p & 0x0f) < ((q & 0x0f) + c))
	            state._SREG |= 0x20;     // set H bit
	    }
	}

//	void OP_JUDGE_V_ADD(int p, int q, int c, int s)
	protected void OP_JUDGE_V_ADD(int p, int q, short c, short s)
//	 end
	{
	    if (s == 8) {   // ADD, ADC, INC
	        short result8 = 0;

	    /*
	    if ((((p & 0x7f) + (q & 0x7f) + c > 127) && (p + q + c > 255))
	                || (((p & 0x7f) + (q & 0x7f) + c < 127) && (p + q + c < 255)))
	        state._SREG = state._SREG & 0x0f7;
	    else
	        state._SREG = state._SREG | 0x08;
	     */
	        result8 = (short)(p + q + c);
	        //FIXME: wszystkie operacje logiczne zamieniam na > 0
	        if ((((p & q & ~result8) | (~p & ~q & result8)) & 0x080) != 0)
	            state._SREG |= 0x08;  // set V bit
	        else
	            state._SREG &= 0x0f7;  // clear V bit
	    } else if (s == 16) {   // ADIW
	        int result16 = 0;

	    /*
	    if ((((p & 0x7FFF) + (q & 0x7FFF) + c > 32767) && (p + q + c > 65535))
	        || (((p & 0x7FFF) + (q & 0x7FFF) + c < 32767) && (p + q + c < 65535)))
	        state._SREG = state._SREG & 0x0f7;
	    else
	        state._SREG = state._SREG | 0x08;
	    */
	        result16 = p + q + c;
	        if ((((~p) & result16) & 0x08000) != 0)
	            state._SREG |= 0x08;  // set V bit
	        else
	            state._SREG &= 0x0f7;  // clear V bit
	    }
	}

//	void OP_JUDGE_V_SUB(int p,int q,int c,int s)
	protected void OP_JUDGE_V_SUB(int p, int q, short c, short s)
//	 end
	{
	        /*
	           int c7,c6,v;
	           if(p>(q+c))
	           c7=0;
	           else
	           c7=1;
	           if(s==8)
	           {
	           if((p&0x7F)>((q&0x7F)+c))
	           c6=0;
	           else
	           c6=1;
	           }
	           else if(s==16)
	           {
	           if((p&0x7FFF)>((q&0x7FFF)+c))
	           c6=0;
	           else
	           c6=1;
	           }
	           v=c7^c6;
	           if(v==1)
	           state._SREG|=0x08;
	           else if(v==0)
	           state._SREG&=0x0f7;
	         */
	    if (s == 8) {
	        /* SUB, SUBI, SBC, SBCI, CP, CPC, CPI, DEC, NEG */
	        short result8 = (short)(p - q - c);
	        if ((((p & ~q & ~result8) | (~p & q & result8)) & 0x080) != 0)
	            state._SREG |= 0x08;      // set V bit
	        else
	            state._SREG &= 0x0f7;      // clear V bit
	    } else if (s == 16) {
	        /* SBIW */
	        int result16 = p - q - c;
	        if (((p & ~result16) & 0x08000) != 0)
	            state._SREG |= 0x08;      // set V bit
	        else
	            state._SREG &= 0x0f7;      // clear V bit
	    }
	        // end
	}

//	void OP_JUDGE_N(int p,int s)
	protected void OP_JUDGE_N(int p, short s)
//	 end
	{
	    //if((p>>(s-1))==1)
	    if (((p >> (s - 1)) & 0x01) != 0)
	    // end
	        state._SREG |= 4;
	    else
	        state._SREG &= 0x0fb;
	}

	protected void OP_JUDGE_S()
	{
	    short n, v;

	    n = (short)((state._SREG & 0x04) >> 2);
	    v = (short)((state._SREG & 0x08) >> 3);
	    n ^= v;
	    if (n == 1)
	        state._SREG |= 0x10;
	    else
	        state._SREG &= 0x0ef;
	}

	protected void OP_JUDGE_Z(int p)
	{
	    if (p == 0)
	        state._SREG |= 0x02;
	    else
	        state._SREG &= 0x0fd;
	}

	protected void OP_ADD()
	{
	    COPY_INSTR_NAME("ADD\n");
	    
		int OPCODE = state.opcode;
	    OP_JUDGE_C_H_ADD(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    OP_JUDGE_V_ADD(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)((0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) + (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)]));
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_ADC()
	{
	    COPY_INSTR_NAME("ADC\n");
	    
		int OPCODE = state.opcode;
	    byte c = (byte)(state._SREG & 1);
	    OP_JUDGE_C_H_ADD(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    OP_JUDGE_V_ADD(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)((0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) + (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)] + c));
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

//	 must be changed!!!!
	protected void OP_ADIW()
	{
	    COPY_INSTR_NAME("ADIW\n");
	    
		int OPCODE = state.opcode;
	    short d = (short)((OPCODE & 0x30) >> 4);

	        //int p=REG(2*d+24);
	        //int q=REG(2*d+25)<<8;
	    int p = 0x000000ff & (int)state.regs[2 * d + 24];
	    int q = (0x000000ff & (int) state.regs[2 * d + 25]) << 8;

	        // end
	    OP_JUDGE_C_H_ADD(p + q, (OPCODE & 0x0f) + ((OPCODE & 0x00c0) >> 2), (short)0, (short)16);
	    OP_JUDGE_V_ADD(p + q, (OPCODE & 0x0f) + ((OPCODE & 0x00c0) >> 2), (short)0, (short)16);
	    state.regs[2 * d + 24] = (byte)((p + q + (OPCODE & 0x0f) + ((OPCODE & 0x00c0) >> 2)) & 0x00ff);
	    state.regs[2 * d + 25] = (byte)(((p + q + (OPCODE & 0x0f) + ((OPCODE & 0x00c0) >> 2)) & 0x0ff00) >> 8);
	    OP_JUDGE_N(p + q + (OPCODE & 0x0f) + ((OPCODE & 0x00c0) >> 2), (short)16);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_INC()
	{
	    COPY_INSTR_NAME("INC\n");
	    
		int OPCODE = state.opcode;
	    OP_JUDGE_V_ADD(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)], 1, (short)0, (short)8);
	    state.regs[((OPCODE & 0x01f0) >> 4)] += 0x01;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_SUB()
	{
	    COPY_INSTR_NAME("SUB\n");
	    
		int OPCODE = state.opcode;
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)((0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) - (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)]));
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_SUBI()
	{
	    COPY_INSTR_NAME("SUBI\n");
	    
		int OPCODE = state.opcode;
	        //OP_JUDGE_C_H_SUB(REG(((OPCODE&0x0f0)>>4)+16),(OPCODE&0x0f)+((OPCODE&0x0f00)>>8),0,8);
	        //OP_JUDGE_V_SUB(REG(((OPCODE&0x0f0)>>4)+16),(OPCODE&0x0f)+((OPCODE&0x0f00)>>8),0,8);
	        //REG(((OPCODE&0x0f0)>>4)+16)-=(OPCODE&0x0f)+((OPCODE&0x0f00)>>8);
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4), (short)0, (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)), (short)0, (short)8);
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] = (byte)((0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]) - ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)));

	        // end
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_SBC()
	{
	    COPY_INSTR_NAME("SBC\n");
	    
		int OPCODE = state.opcode;
	    short c = (short)(state._SREG & 1);
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)((0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) - (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)]) - c);
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_SBCI()
	{
	    COPY_INSTR_NAME("SBCI\n");
	    
		int OPCODE = state.opcode;
	    byte c = (byte)(state._SREG & 0x01);

	        //OP_JUDGE_C_H_SUB(REG(((OPCODE&0x0f0)>>4)+16),(OPCODE&0x0f)+((OPCODE&0x0f00)>>8),state._SREG&0x1,8);
	        //OP_JUDGE_V_SUB(REG(((OPCODE&0x0f0)>>4)+16),(OPCODE&0x0f)+((OPCODE&0x0f00)>>8),state._SREG&0x1,8);
	        //REG(((OPCODE&0x0f0)>>4)+16)=REG(((OPCODE&0x0f0)>>4)+16)-((OPCODE&0x0f)+((OPCODE&0x0f00)>>8))-c;
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4), (short)(state._SREG & 0x01), (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)), (short)(state._SREG & 0x01), (short)8);
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] = (byte)((0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]) - ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)) - c);

	        // end
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_SBIW()
	{
	    COPY_INSTR_NAME("SBIW\n");
	    
		int OPCODE = state.opcode;
	    short d = (short)((OPCODE & 0x30) >> 4);
	    int p = 0x000000ff & (int)state.regs[2 * d + 24];
	    int q = (0x000000ff & (int)state.regs[2 * d + 25]) << 8;
	    OP_JUDGE_C_H_SUB(p + q, (OPCODE & 0x0f) + ((OPCODE & 0x0c0) >> 2), (short)0, (short)16);
	    OP_JUDGE_V_SUB(p + q, (OPCODE & 0x0f) + ((OPCODE & 0x0c0) >> 2), (short)0, (short)16);
	    state.regs[2 * d + 24] = (byte)((p + q - (OPCODE & 0x0f) - ((OPCODE & 0x0c0) >> 2)) & 0x0ff);
	    state.regs[2 * d + 25] = (byte)(((p + q - (OPCODE & 0x0f) - ((OPCODE & 0x0c0) >> 2)) & 0x0ff00) >> 8);
	    OP_JUDGE_N(p + q - (OPCODE & 0x0f) - ((OPCODE & 0x0c0) >> 2), (short)16);
	    OP_JUDGE_Z(p + q - (OPCODE & 0x0f) - ((OPCODE & 0x0c0) >> 2));
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_DEC()
	{
	    COPY_INSTR_NAME("DEC\n");
	    
		int OPCODE = state.opcode;
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)], (short)1, (short)0, (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] -= 0x01;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x01f0) >> 4)]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_COM()
	{
	    COPY_INSTR_NAME("COM\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)(0x0ff - (0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]));
	    state._SREG &= 0x0f7;
	    state._SREG |= 1;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_NEG()
	{
	    COPY_INSTR_NAME("NEG\n");
	    
		int OPCODE = state.opcode;
	    OP_JUDGE_C_H_SUB(0, 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)0, (short)8);
	    OP_JUDGE_V_SUB(0, 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)0, (short)8);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)(0x00 - 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	protected void OP_CP()
	{
	    COPY_INSTR_NAME("CP\n");
	    
		int OPCODE = state.opcode;
	    int t;
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)0, (short)8);
	    t = (0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) - (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)]);
	    OP_JUDGE_N(t, (short)8);
	    OP_JUDGE_Z(t);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_C_PC()
	{
	    COPY_INSTR_NAME("C_PC\n");
	    
		int OPCODE = state.opcode;
	    int t;
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], 0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)], (short)(state._SREG & 0x01), (short)8);
	    t = (0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) - (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)]) - (state._SREG & 0x01);
	    OP_JUDGE_N(t, (short)8);
	    OP_JUDGE_S();
	    if (((state._SREG & 0x02) >> 1 == 1) && t == 0)
	        state._SREG |= 0x02;
	    else
	        state._SREG &= 0x0fd;
	    state._PC++;
	}

	void OP_CPI()
	{
	    COPY_INSTR_NAME("CPI\n");
	    
		int OPCODE = state.opcode;
	    int t;
	    OP_JUDGE_C_H_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)), (short)0, (short)8);
	    OP_JUDGE_V_SUB(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4)), (short)0, (short)8);
	    t = (0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]) - ((OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4));
	    OP_JUDGE_N(t, (short)8);
	    OP_JUDGE_Z(t);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_AND()
	{
	    COPY_INSTR_NAME("AND\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] &= state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)];
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_ANDI()
	{
	    COPY_INSTR_NAME("ANDI\n");
	    
		int OPCODE = state.opcode;
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] &=  (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4);
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_CBR()
	{
	    COPY_INSTR_NAME("CBR\n");
	    
		int OPCODE = state.opcode;
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] &= (0x0ff - (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4));
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_TST()
	{
	    COPY_INSTR_NAME("TST\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] &= state.regs[(OPCODE & 0x01f0) >> 4];
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_OR()
	{
	    COPY_INSTR_NAME("OR\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] |= state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)];
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_ORI()
	{
	    COPY_INSTR_NAME("ORI\n");
	    
		int OPCODE = state.opcode;
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] |= (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4);
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_SBR()
	{
	    COPY_INSTR_NAME("SBR\n");
	    
		int OPCODE = state.opcode;
	    state.regs[((OPCODE & 0x0f0) >> 4) + 16] |= (OPCODE & 0x0f) + ((OPCODE & 0x0f00) >> 4);
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_EOR()
	{
	    COPY_INSTR_NAME("EOR\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] ^= state.regs[(OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5)];
	    state._SREG &= 0x0f7;
	    OP_JUDGE_N(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4], (short)8);
	    OP_JUDGE_Z(0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]);
	    OP_JUDGE_S();
	    state._PC++;
	}

	void OP_CLR()
	{
	    COPY_INSTR_NAME("CLR\n");
	    
		int OPCODE = state.opcode;
	    state.regs[(OPCODE & 0x01f0) >> 4] = 0;
	    state._SREG &= 0x0e3;
	    state._SREG |= 0x02;
	}
//	 end of arithmetic instructions

	void OP_RJMP()
	{
	    COPY_INSTR_NAME("RJMP\n");
	    
		int OPCODE = state.opcode;
	    int k;

	    k = OPCODE & 0x0fff;
	    if ((k & 0x0800) != 0)
	    {
	        k |= 0x0fffff800;    // sign-extend
	    }
	    state._PC = state._PC + k + 1;
	}

	void OP_IJMP()
	{
	    COPY_INSTR_NAME("IJMP\n");
	    
	    state._PC = (0x000000ff & (int) state.regs[30]) + (((0x000000ff & (int) state.regs[31]) << 8));
	}

	void OP_JMP()
	{
	    COPY_INSTR_NAME("JMP\n");
	    
	    state._PC++;                     // now state._PC points to call address
	    state._PC = code_get_opcode_extension();
	}

	void OP_RCALL()
	{
	    COPY_INSTR_NAME("RCALL\n");
	    
		int OPCODE = state.opcode;
	    int addr;

	    addr = state._PC + 1;
	    stack_push((short)((addr >> 8) & 0x00ff));
	    stack_push((short)(addr & 0x00ff));

	    addr = OPCODE & 0x0fff;
	    if ((addr & 0x0800) != 0)
	    {
	        addr |= 0x0fffff800;    // sign-extend
	    }
	    //FIXME
	    state._PC = state._PC + addr + 1;
	    state.depth++;
	}

	void OP_ICALL()
	{
	    COPY_INSTR_NAME("ICALL\n");
	    
	    int addr;

	    addr = state._PC + 1;
	    stack_push((short)((addr >> 8) & 0x00ff));
	    stack_push((short)(addr & 0x00ff));

	    state._PC = 0x000000ff & (int) state.regs[30] + ((0x000000ff & (int) state.regs[31]) << 8);
	    state.depth++;
	}

	void OP_CALL()
	{
        COPY_INSTR_NAME("CALL... ");
        
	    int address;

	    state._PC++;
	    address = code_get_opcode_extension();
	    //FIXME: by³o U
	    if ((address & 0x00008000/*U*/) != 0) {
	        COPY_INSTR_NAME("CALL_PRIMITIVE\n");
	        execute_primitive(address & 0x7fff);
	        state._PC++;
	    } else {
	        COPY_INSTR_NAME("CALL\n");
	        
	        int returnAddress = state._PC + 1;
	        stack_push((short)((returnAddress >> 8) & 0x00ff));
	        stack_push((short)(returnAddress & 0x00ff));
	        state._PC = address;
	        state.depth++;
	    }
	}

	void OP_RET()
	{
	    COPY_INSTR_NAME("RET\n");
	    
	    if (state.depth == 0) {
	        // end of event handler!
	        state.running = false;
	    } else {
	        int lo, hi;
	        state.depth--;

	        lo = (int)stack_pop() & 0x000000ff;
	        hi = (int)stack_pop() & 0x000000ff;
	        state._PC = ( hi << 8) | lo;
	    }
	}

	void OP_RETI()
	{
	    COPY_INSTR_NAME("RETI\n");
	    
	    if (state.depth == 0) {
	        // end of event handler!
	        state.running = false;
	    } else {
	        int lo, hi;
	        state.depth--;

	        lo = (int)stack_pop() & 0x000000ff;
	        hi = (int)stack_pop() & 0x000000ff;
	        state._PC = (hi << 8) | lo;
	        state._SREG |= 0x080;  // enable interrupts
	    }
	}

	//FIXME: by³o "static inline"
	/*static inline*/ void SKIP_CONT()
	{
		int OPCODE = state.opcode;
	    if (((OPCODE & 0x0fe0e) == 0x0940e)   // CALL
	                || ((OPCODE & 0x0fe0e) == 0x0940c)    // JMP
	                || ((OPCODE & 0x0fe0f) == 0x09000)    // LDS
	                || ((OPCODE & 0x0fe0f) == 0x09200)) { // STS
	        state._PC += 2;   // skip 4 byte instruction
	    } else {
	        state._PC++;      // skip 2 byte instruction
	    }
	}

	void OP_CPSE()
	{
	    COPY_INSTR_NAME("CPSE\n");
	    
		int OPCODE = state.opcode;
	    short reg_num = (short)((OPCODE & 0x0f) + ((OPCODE & 0x0200) >> 5));
	    if (state.regs[(OPCODE & 0x01f0) >> 4] == state.regs[reg_num])
	        SKIP_CONT();
	    state._PC++;
	}

	void OP_SBRC()
	{
	    COPY_INSTR_NAME("SBRC\n");
	    
		int OPCODE = state.opcode;
	    if ((state.regs[(OPCODE & 0x01f0) >> 4] & (1 << (OPCODE & 0x07))) == 0)
	        SKIP_CONT();
	    state._PC++;
	}

	void OP_SBRS()
	{
	    COPY_INSTR_NAME("SBRS\n");
	    
		int OPCODE = state.opcode;
	    if ((state.regs[(OPCODE & 0x01f0) >> 4] & (1 << (OPCODE & 0x07))) != 0)
	        SKIP_CONT();
	    state._PC++;
	}

	void OP_SBIC()
	{
	    COPY_INSTR_NAME("SBIC\n");
	    
	    //FIMXE: this instruction operates on lower 32 I/O registers
	    SKIP_CONT();
	    state._PC++;
	}

	void OP_SBIS()
	{
	    COPY_INSTR_NAME("SBIS\n");
	    
	    // FIMXE: this instruction operates on lower 32 I/O registers
	    state._PC++;
	}


	void OP_BRBC()
	{
	    COPY_INSTR_NAME("BRBC\n");
	    
		int OPCODE = state.opcode;
	    long k;
	    if ((((0x01 << (OPCODE & 0x07)) & state._SREG) >> (OPCODE & 0x07)) == 0) {

	            // state._PC=state._PC+((OPCODE&0x3F8)>>3)+1;
	            k = (OPCODE & 0x03f8) >> 3;
	        k = (k & 0x00000040) != 0 ? (k | 0xffffff80L) : (k & 0x003f);
	        state._PC = (int)(state._PC + k + 1);

	            // end
	    } else
	        state._PC++;
	}

	void OP_BRBS()
	{
	    COPY_INSTR_NAME("BRBS\n");
	    
		int OPCODE = state.opcode;
	    long k;
	    if ((((0x01 << (OPCODE & 0x07)) & state._SREG) >> (OPCODE & 0x07)) == 0)
	        state._PC++;
	    else {

	            // state._PC=state._PC+((OPCODE&0x3F8)>>3)+1;
	            k = (OPCODE & 0x03f8) >> 3;
	        k = (k & 0x00000040) != 0 ? (k | 0xffffff80L) : (k & 0x003f);
	        state._PC = (int) (state._PC + k + 1);

	            // end
	    }
	}

	void OP_SBI()
	{
	    COPY_INSTR_NAME("SBI\n");
	    
	    // FIMXE: this instruction operates on lower 32 I/O registers
	    state._PC++;
	}

	void OP_CBI()
	{
	    COPY_INSTR_NAME("CBI\n");
	    
	    // FIMXE: this instruction operates on lower 32 I/O registers
	    state._PC++;
	}

	void OP_BSET()
	{
	    COPY_INSTR_NAME("BSET\n");
	    
		int OPCODE = state.opcode;
	    state._SREG |= (1 << ((OPCODE & 0x70) >> 4));
	    state._PC++;
	}

	void OP_BCLR()
	{
	    COPY_INSTR_NAME("BCLR\n");
	    
		int OPCODE = state.opcode;
	    state._SREG &= (~(1 << ((OPCODE & 0x70) >> 4)));
	    state._PC++;
	}

	void OP_BST()
	{
	    COPY_INSTR_NAME("BST\n");
	    
		int OPCODE = state.opcode;
	    if ((state.regs[(OPCODE & 0x01f0) >> 4] & (1 << (OPCODE & 0x07))) != 0)
	        state._SREG |= 0x040;
	    else
	        state._SREG &= 0x0bf;
	    state._PC++;
	}

	void OP_BLD()
	{
	    COPY_INSTR_NAME("BLD\n");
	    
		int OPCODE = state.opcode;
	    if ((state._SREG & 0x40) != 0)
	        state.regs[(OPCODE & 0x01f0) >> 4] |= (1 << (OPCODE & 0x07));
	    else
	        state.regs[(OPCODE & 0x01f0) >> 4] &= ~(1 << (OPCODE & 0x07));
	    state._PC++;
	}

	void OP_LSL()
	{
	    COPY_INSTR_NAME("LSL\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];
	    if ((r & 0x080) != 0)
	        state._SREG |= 0x01;
	    else
	        state._SREG &= 0x0fe;
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)(r << 1);
	    state._PC++;
	}

	void OP_LSR()
	{
	    COPY_INSTR_NAME("LSR\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];
	    if ((r & 0x01) != 0)
	        state._SREG |= 0x01;
	    else
	        state._SREG &= 0x0fe;
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)(r >> 1);
	    state._PC++;
	}

	void OP_ASR()
	{
	    COPY_INSTR_NAME("ASR\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];
	    if ((r & 0x01) != 0)
	        state._SREG |= 0x01;
	    else
	        state._SREG &= 0x0fe;
	    r = ((r & 0x080) != 0) ? ((r >> 1) | 0x080) : (r >> 1);
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)r;

	    state._PC++;
	}

	void OP_ROL()
	{
	    COPY_INSTR_NAME("ROL\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];
	    short old_cf = (short)(state._SREG & 0x01);

	    if ((r & 0x080) != 0)
	        state._SREG |= 0x01;
	    else
	        state._SREG &= 0x0fe;
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)((r << 1) | old_cf);
	    state._PC++;

	}

	void OP_ROR()
	{
	    COPY_INSTR_NAME("ROR\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];
	    int old_cf = state._SREG & 0x01;

	    if ((r & 0x01) != 0)
	        state._SREG |= 0x01;
	    else
	        state._SREG &= 0x0fe;
	    if (old_cf == 0)
	        r >>= 1;
	    else
	        r = (r >> 1) | 0x080;
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)r;
	    state._PC++;
	}

	void OP_SWAP()
	{
	    COPY_INSTR_NAME("SWAP\n");
	    
		int OPCODE = state.opcode;
	    int r = 0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4];

	    r = (((r >> 4) & 0x0f) | ((r << 4) & 0x0f0));
	    state.regs[(OPCODE & 0x01f0) >> 4] = (byte)r;
	    state._PC++;
	}

	void OP_MUL()
	{
	    COPY_INSTR_NAME("MUL\n");
	    
		int OPCODE = state.opcode;
	    int result = (0x000000ff & (int)state.regs[(OPCODE & 0x01f0) >> 4]) * (0x000000ff & (int)state.regs[(OPCODE & 0x0f) | ((OPCODE & 0x200) >> 5)]);
	    state.regs[0] = (byte)(result & 0x0ff);
	    state.regs[1] = (byte)((result >> 8) & 0x0ff);
	    if (result == 0)
	        state._SREG |= 0x02;      // set Z bit
	    else
	        state._SREG &= 0x0fd;      // clear Z bit
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x01;      // set C bit
	    else
	        state._SREG &= 0x0fe;      // clear C bit
	    state._PC++;
	}

	void OP_MULS()
	{
	    COPY_INSTR_NAME("MULS\n");
	    
		int OPCODE = state.opcode;
	    int result = (0x000000ff & (int)state.regs[((OPCODE & 0x0f0) >> 4) + 16]) * (0x000000ff & (int)state.regs[(OPCODE & 0x0f) + 16]);
	    state.regs[0] = (byte)((int) result & 0x0ff);
	    state.regs[1] = (byte)(((int) result >> 8) & 0x0ff);
	    if (result == 0)
	        state._SREG |= 0x02;      // set Z bit
	    else
	        state._SREG &= 0x0fd;      // clear Z bit
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x01;      // set C bit
	    else
	        state._SREG &= 0x0fe;      // clear C bit
	    state._PC++;
	}

	void OP_MULSU()
	{
	    COPY_INSTR_NAME("MULSU\n");
	    
		int OPCODE = state.opcode;
	    int result = (0x000000ff & (int)state.regs[((OPCODE & 0x70) >> 4) + 16]) * (0x000000ff & (int) state.regs[(OPCODE & 0x07) + 16]);
	    state.regs[0] = (byte)(result & 0x0ff);
	    state.regs[1] = (byte)((result >> 8) & 0x0ff);
	    if (result == 0)
	        state._SREG |= 0x02;      // set Z bit
	    else
	        state._SREG &= 0x0fd;      // clear Z bit
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x01;      // set C bit
	    else
	        state._SREG &= 0x0fe;      // clear C bit
	    state._PC++;
	}

	void OP_FMUL()
	{
	    COPY_INSTR_NAME("FMUL\n");
	    
		int OPCODE = state.opcode;
	    int result;
	    result = (0x000000ff & (int)state.regs[((OPCODE & 0x70) >> 4) + 16]) * (0x000000ff & (int)state.regs[(OPCODE & 0x7) + 16]);
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x1;          // set C bit
	    else
	        state._SREG &= 0x0fE;         // clear C bit
	    result <<= 1;
	    state.regs[0] = (byte) (result & 0x0fF);
	    state.regs[1] = (byte) ((result >> 8) & 0x0fF);
	    if (result == 0)
	        state._SREG |= 0x2;          // set Z bit
	    else
	        state._SREG &= 0x0fD;         // clear Z bit
	    state._PC++;
	}

	void OP_FMULS()
	{
	    COPY_INSTR_NAME("FMULS\n");
	    
		int OPCODE = state.opcode;
	    int result;
	    result =
	    	(0x000000ff & (int)state.regs[((OPCODE & 0x70) >> 4) + 16]) * (0x000000ff & (int)state.regs[(OPCODE & 0x7) + 16]);
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x1;          // set C bit
	    else
	        state._SREG &= 0x0fE;         // clear C bit
	    result <<= 1;
	    state.regs[0] = (byte) (result & 0x0fF);
	    state.regs[1] = (byte) ((result >> 8) & 0x0fF);
	    if (result == 0)
	        state._SREG |= 0x2;          // set Z bit
	    else
	        state._SREG &= 0x0fD;         // clear Z bit
	    state._PC++;
	}

	void OP_FMULSU()
	{
	    COPY_INSTR_NAME("FMULSU\n");
	    
		int OPCODE = state.opcode;
	    int result;
	    result =
	    	(0x000000ff & (int)state.regs[((OPCODE & 0x70) >> 4) + 16]) * 0x000000ff & (0x000000ff & (int)state.regs[(OPCODE & 0x7) + 16]);
	    if ((result & 0x08000) != 0)
	        state._SREG |= 0x1;          // set C bit
	    else
	        state._SREG &= 0x0fE;         // clear C bit
	    result <<= 1;
	    state.regs[0] = (byte) (result & 0x0fF);
	    state.regs[1] = (byte) ((result >> 8) & 0x0fF);
	    if (result == 0)
	        state._SREG |= 0x2;          // set Z bit
	    else
	        state._SREG &= 0x0fD;         // clear Z bit
	    state._PC++;
	}


//	 end
	void OP_NOP()
	{
	    COPY_INSTR_NAME("NOP\n");
	    
	    state._PC++;
	}

	void OP_SLEEP()
	{
	    COPY_INSTR_NAME("SLEEP\n");
	    
	    state._PC++;
	}

	void OP_WDR()
	{
	    COPY_INSTR_NAME("WDR\n");
	    
	    state._PC++;
	}
    
	/************************************ ARGUMENTS *******************************************/
    /*public short RvVMArgsI_get8(String str, short pos) {
        short regs_num = countRegs(str, pos);
        if (regs_num > 16)
            return 0;
        return state.regs[24 - regs_num];
    }*/

    /*public int RvVMArgsI_get16(String str, short pos) {
        int value = 0;
        short regs_num;

        regs_num = countRegs(str, pos);
        if (regs_num > 16)
            return 0;

        value = (0x000000ff & (int) state.regs[25 - regs_num]) << 8;
        value |= 0x000000ff & (int) state.regs[24 - regs_num];
        return value;
    }*/

    /*public long RvVMArgsI_get32(String str, short pos) {
        long value = 0;
        short regs_num;

        regs_num = countRegs(str, pos);
        if (regs_num > 14)
            return 0;

        value = state.regs[25 - regs_num] << 24;
        value |= state.regs[24 - regs_num] << 16;
        value |= state.regs[23 - regs_num] << 8;
        value |= state.regs[22 - regs_num];
        return value;
    }*/

    /*public RvFixedP_t RvVMArgsI_getFixedP(String str, short pos) {
        /*register/ int tmp;
        RvFixedP_t value = new RvFixedP_t();
        short regs_num;

        regs_num = countRegs(str, pos);
        if (regs_num > 14) {
            value.i = value.f = 0;
            return value;
        }

        tmp = (0x000000ff & (int) state.regs[25 - regs_num]) << 8;
        tmp |= 0x000000ff & (int) state.regs[24 - regs_num];
        value.f = tmp;
        tmp = (0x000000ff & (int) state.regs[23 - regs_num]) << 8;
        tmp |= 0x000000ff & (int) state.regs[22 - regs_num];
        value.i = (int) tmp;
        return value;
    }*/

    /*private String RvVMArgsI_getAtPointer(int addr, short *data) {
        data = parent.getCurrAgent().getData().get8(addr - Properties.AVR_REG_IO_SIZE);
        System.out.println("getAtPointer:"+data);
        return state.running ? "SUCCESS" : "FAIL";
    }*/
    /*FIXME
     * public String RvVMArgsI_getAtPointer(int addr, short *data) {
        *data = data.get8(addr - AVR_REG_IO_SIZE);
        return state.running ? "SUCCESS" : "FAIL";
    }*/

    /*private String RvVMArgsI_setAtPointer(int addr, short data) {
        if (parent.getCurrAgent().getData().put8(addr - Properties.AVR_REG_IO_SIZE, data) != "SUCCESS") {
            state.status = "ST_DATA_ADDR";
            parent.log.logError(addr);
            state.running = false;
        }

        return state.running ? "SUCCESS" : "FAIL";
    }*/

    /*public void RvVMArgsI_returnValue(short size, long value) {
    	returnedValue.size = size;
    	returnedValue.value = value;
    }*/

    /*public void RvVMArgsI_returnFPValue(RvFixedP_t value) {
        /*register/ int tmp = (int) value.i;

        returnedValue.size = 32;
        returnedValue.value = (((long) value.f) << 16) + tmp;
    }*/
	
	/************************************ helps *******************************************/
    private byte data_ld(int address) {
    	//System.out.println("load@"+(address - Properties.AVR_REG_IO_SIZE));
        if (address < Properties.AVR_NUM_REGS) {
            return state.regs[address];
        } else if (address < Properties.AVR_REG_IO_SIZE) {
            if (address == (Properties.AVR_IO_SP + 1)) {
                return (byte)((state._SP >> 8) & 0x00ff);
            } else if (address == Properties.AVR_IO_SP) {
                return (byte)(state._SP & 0x00ff);
            } else if (address == Properties.AVR_IO_SREG) {
                return state._SREG;
            } else {
                state.status = "ST_DATA_ADDR";
                parent.log.logError(address);
                state.running = false;
                return 0;
            }
        }
        return (byte)parent.getCurrAgent().getData().get8(address - Properties.AVR_REG_IO_SIZE);
    }

    private void data_st(int address, byte value) {
    	//System.out.println("set@"+(address - Properties.AVR_REG_IO_SIZE)+": "+value);
        if (address < Properties.AVR_NUM_REGS) {
            state.regs[address] = value;
        } else if (address < Properties.AVR_REG_IO_SIZE) {
            /* FIXME: maybe we should track SP modifications to be able
             * to detect stack frame unwinding and fill it with zeros?
             */
            if (address == (Properties.AVR_IO_SP + 1)) {
                state._SP = 0x0000ffff & ((state._SP & 0x00ff) | ( (0x000000ff & (int)value) << 8));
            } else if (address == Properties.AVR_IO_SP) {
                state._SP = 0x0000ffff & ((state._SP & 0x0000ff00) | ( (0x000000ff & (int)value)));
            } else if (address == Properties.AVR_IO_SREG) {
                state._SREG = value;
            } else {
                state.status = "ST_DATA_ADDR";
                parent.log.logError(address);
                state.running = false;
            }
        } else if (parent.getCurrAgent().getData().put8(address - Properties.AVR_REG_IO_SIZE, value) != "SUCCESS") {
            state.status = "ST_DATA_ADDR";
            parent.log.logError(address);
            state.running = false;
        }
    }

    private void stack_push(short value) {
    	//System.out.println("push@"+(state._SP - Properties.AVR_REG_IO_SIZE)+": "+value);
        if (state._SP > Properties.AVR_REG_IO_SIZE) {
            if (parent.getCurrAgent().getData().put8(state._SP - Properties.AVR_REG_IO_SIZE, value) == "SUCCESS") {
                state._SP--;
                return;
            }
        }

        state.status = "ST_DATA_ADDR";
        parent.log.logError(state._SP);
        state.running = false;
    }

    public byte stack_pop() {
    	//System.out.println("pop@"+(state._SP + 1 - Properties.AVR_REG_IO_SIZE));
        byte value = 0;

        if (state._SP == (Properties.DATA_MEM_SIZE + Properties.AVR_REG_IO_SIZE - 1)) {
            state.status = "ST_DATA_ADDR";
            parent.log.logError(state._SP);
            state.running = false;
        } else {
            state._SP++;
            value = (byte)parent.getCurrAgent().getData().get8(state._SP - Properties.AVR_REG_IO_SIZE);

            // zero stack after POP
            /*if (parent.getCurrAgent().getData().put8(state._SP - Properties.AVR_REG_IO_SIZE, (short)0) != "SUCCESS") {
                state.status = "ST_DATA_ADDR";
                parent.log.logError(state._SP);
                state.running = false;
            }*/
        }

        return value;
    }
	
    private int code_get_opcode_extension() {
        return parent.getCurrAgent().getCode().get16(state._PC << 1);
    }
    
    class RvFixedP_t
    {
    	public int f;
    	public int i;
    }
    
    private byte code_lpm(int address) {
    	return '0';//(byte)parent.getCurrAgent().getCode().get8(address);
	}

    /**
     * Gets currently processed code address.
     * @return address
     */
    public int getCurrPosition()
    {
    	return state._PC;
    }
	
    private class RV 
    {
        short size;
        long value;
    };
    
    private RV returnedValue = new RV();

    private void execute_primitive(int idx) {        
        returnedValue.size = 32;
        
        Object retVal = callPrimitive(idx);
        try{
        	if(retVal.getClass() == ReturnValue.class)
        	{
        		if(((ReturnValue)retVal).getReturnValue() == -1)
        		{
                    System.out.print("RvAvrVM: invalid primitive called!\n");
                    state.status = "ST_PRIMITIVE";
                    parent.log.logError(idx + "no. " + ((ReturnValue)retVal).getReturnValue());
                    state.running = false;
        		}
                returnedValue.value = ((ReturnValue)retVal).getReturnValue();   
                returnedValue.size = 8;
        	}
        	else
        	{//agentId
        		returnedValue.value = (Long)retVal;
        		System.out.println("RvAvrVM: got:" + returnedValue.value);
        	}
        }catch(Exception ex){}

        if (returnedValue.size == 0) {
            // void
        } else if (returnedValue.size == 8) {
            // int8_t or uint8_t
            state.regs[25] = 0;
            state.regs[24] = (byte)(returnedValue.value & 0x0ff);
        } else if (returnedValue.size == 16) {
            // int16_t or uint16_t
        	state.regs[24] = (byte)(returnedValue.value & 0x0ff);
            returnedValue.value >>= 8;
                state.regs[25] = (byte)(returnedValue.value & 0x0ff);
        } else if (returnedValue.size == 32) {
            // int32_t or uint32_t
        	state.regs[22] = (byte)(returnedValue.value & 0x0ff);
            returnedValue.value >>= 8;
        	state.regs[23] = (byte)(returnedValue.value & 0x0ff);
            returnedValue.value >>= 8;
        	state.regs[24] = (byte)(returnedValue.value & 0x0ff);
            returnedValue.value >>= 8;
            state.regs[25] = (byte)(returnedValue.value & 0x0ff);
        } else {
            // invalid returned value size
            state.status = "ST_PRIMITIVE";
            parent.log.logError(0x0ffff);
            state.running = false;
        }
    }

    private Object callPrimitive(int idx)
    {
    	//System.out.print("Prymitive " + idx + " called.");
        if (idx <= 0x0ff) {
            short genID = (short) idx;
            return PrimitivesAdapter.primitive_execute(InstructionMap.getName(genID), parent.getCurrAgent(), state.regs);
        } else {
            long globalID;

            idx = (idx - 0x100) & 0x0ff;
            globalID = ((NonGenericAgent)parent.getCurrAgent()).getNonGenericInstruction((short) idx);

            return PrimitivesAdapter.primitive_execute(InstructionMap.getName(globalID), parent.getCurrAgent(), state.regs);
        }
    }
}