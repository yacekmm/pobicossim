package pl.edu.pw.pobicos.mw.instruction;

import java.util.Vector;

public class InstructionMap {
	
	public static String UINT8 = "uint8";
	public static String UINT16 = "uint16";
	public static String UINT32 = "uint32";
	public static String UINT8P = "uint8*";
	
	public static String getName(long code)
	{
		return InstructionTree.getName(code);
	}
	
	public static long getCode(String name)
	{
		return InstructionTree.getCode(name);
	}
	
	public static Vector<String> getParams(long code)
	{
		return InstructionTree.getParams(code);
	}
	
	public static Object getReturn(long code) {
		return InstructionTree.getReturn(code);
	}
}
