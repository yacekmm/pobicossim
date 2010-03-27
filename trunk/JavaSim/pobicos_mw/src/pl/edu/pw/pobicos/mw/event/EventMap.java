package pl.edu.pw.pobicos.mw.event;

public class EventMap {
	
	public static String getName(long code)
	{
		return EventTree.getName(code);
	}
	
	public static long getCode(String name)
	{
		return EventTree.getCode(name);
	}
}
