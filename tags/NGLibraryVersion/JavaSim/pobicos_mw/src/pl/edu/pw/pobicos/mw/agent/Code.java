package pl.edu.pw.pobicos.mw.agent;
import pl.edu.pw.pobicos.mw.Conversions;
public class Code {	
	byte[] code;
	
	public Code(byte[] str)
	{
		code = str;
	}
	
	public int get8(int addr)
	{
		return (int)Conversions.byteToInt(addr, 1, code, true);
	}
	
	public char get16(int addr)
	{
		return (char)Conversions.byteToInt(addr, 2, code, true);
	}
	
	public long get32(int addr)
	{
		return (long)Conversions.byteToInt(addr, 4, code, true);
	}
}
