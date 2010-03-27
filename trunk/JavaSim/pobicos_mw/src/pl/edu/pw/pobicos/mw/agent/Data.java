package pl.edu.pw.pobicos.mw.agent;
import pl.edu.pw.pobicos.mw.Conversions;

public class Data {	
	public byte[] data;
	
	public Data(byte[] str, long size)
	{
		data = new byte[(int)size];
		for(int i = 0; i < str.length; i++)
			data[i] = str[i];	
	}
	
	public int get8(int addr)
	{
		return (int)Conversions.byteToInt(addr, 1, data, true);
	}
	
	public char get16(int addr)
	{
		return (char)Conversions.byteToInt(addr, 2, data, true);
	}
	
	public long get32(int addr)
	{
		return (long)Conversions.byteToInt(addr, 4, data, true);
	}
	
    public String put8(int addr, int data)
    {
    	this.data[addr] = (new Integer(data)).byteValue();
    	return "SUCCESS";
    }
    
    public String put16(int addr, char data)
    {
    	this.data[addr] = (new Integer(data % 256)).byteValue();
    	this.data[addr + 1] = (new Integer(data >> 8)).byteValue();
    	return "SUCCESS";
    }
    
    public String put32(int addr, long data)
    {
    	this.data[addr] = (new Integer((short)(data % Math.pow(256, 3)))).byteValue();
    	this.data[addr + 1] = (new Integer((short)((data >> 8) % Math.pow(256, 2)))).byteValue();
    	this.data[addr + 2] = (new Integer((short)((data >> 16) % Math.pow(256, 1)))).byteValue();
    	this.data[addr + 3] = (new Integer((short)((data >> 24)))).byteValue();
    	return "SUCCESS";
    }
    
    public String toString()
    {
    	String result = "";
    	for(int i = 0; i < data.length; i++)
    		result += Byte.toString(data[i])+" ";
    	return result;
    }
}
