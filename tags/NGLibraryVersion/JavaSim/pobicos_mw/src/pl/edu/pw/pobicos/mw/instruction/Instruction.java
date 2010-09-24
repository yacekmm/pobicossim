package pl.edu.pw.pobicos.mw.instruction;

public class Instruction {
	private long code;
	private String name;
	
	public Instruction(long code, String name)
	{
		this.code = code;
		this.name = name;
	}
	
	public long getCode()
	{
		return code;
	}
	
	public String getName()
	{
		return name;
	}
}
