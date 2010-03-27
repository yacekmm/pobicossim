package pl.edu.pw.pobicos.ng.taxonomy;


public class LocationMap {
	public static String getName(long code)
	{
		return LocationTree.getName(code);
	}
	
	public static long getCode(String name)
	{
		return LocationTree.getCode(name);
	}
}
