package pl.edu.pw.pobicos.mw.vm.avr;

/**
 * Global properties for every Avr virtual machine.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Properties {
	//static public char RV_MAX_DATA_SIZE = 256;
	//static public char RV_VM_MAX_INSTRUCTIONS = 0;
	static public int VM_MAX_INSTRUCTIONS = 0;
	
	/////RvVM.nc
	//	 supported architecture
	static public int RV_VM_ARCH = 1;
	
	/////avrmcu.h
	//	 number of general-purpose registers
	static public char AVR_NUM_REGS = 32;
	//	 definitions of IO space
	static public char AVR_IO_SP = 0x5d;
	static public char AVR_IO_SREG = 0x5f;
	static public char AVR_REG_IO_SIZE = 0x60;
	static public boolean AVR_IO_ADDR_VALID(int a)
	{
		return ((a) >= AVR_IO_SP && (a) <= AVR_IO_SREG);
	}

	//PoSIM
	static public long CODE_MEM_SIZE = 0xffff;
	static public long DATA_MEM_SIZE = 0x1000;
	static public int STACK_SIZE = 1024;
	static public int PO_MAX_DATA_SIZE = 512;
}
