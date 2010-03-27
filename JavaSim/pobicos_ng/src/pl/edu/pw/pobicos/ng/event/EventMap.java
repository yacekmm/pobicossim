package pl.edu.pw.pobicos.ng.event;


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
