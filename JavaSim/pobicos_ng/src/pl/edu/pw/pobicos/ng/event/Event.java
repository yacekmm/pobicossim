package pl.edu.pw.pobicos.ng.event;

public class Event {
	private long code;
	private String name, source = "";
	private long virtualTime;
	
	public Event(long code)
	{
		this.code = code;
		this.name = EventMap.getName(code);
	}
	
	public Event(long code, String source, long time)
	{
		this.code = code;
		this.name = EventMap.getName(code);
		this.source = source;
		this.virtualTime = time;
	}
	
	public long getCode()
	{
		return code;
	}
	
	public String getName()
	{
		return name;
	}
	
	public String getSource()
	{
		return source;
	}
	
	public long getVirtualTime()
	{
		return virtualTime;
	}
	
	public boolean getIsGeneric()
	{
		if(code < 256)//code.startsWith("0"))
			return true;
		return false;
	}
}
