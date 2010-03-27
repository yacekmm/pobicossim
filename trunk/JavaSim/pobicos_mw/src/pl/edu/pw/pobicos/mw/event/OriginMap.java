package pl.edu.pw.pobicos.mw.event;

public class OriginMap {
	
	public static String getName(long code)
	{
		return EventTree.getOriginName(code);
	}
	
	public static long getCode(String name)
	{
		return EventTree.getOriginCode(name);
	}
}
