package pl.edu.pw.pobicos.ng.product;

public class SensorValue {

	private String instruction;
	private String type;
	private Object value;
	
	public SensorValue(String instruction, String type, Object value)
	{
		this.instruction = instruction;
		this.type = type;
		this.value = value;
	}
	
	public String getInstruction()
	{
		return instruction;
	}
	
	public String getType()
	{
		return type;
	}
	
	public Object getValue()
	{
		return value;
	}
	
	public void setValue(Object value)
	{
		this.value = value;
	}
}
