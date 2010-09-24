package pl.edu.pw.pobicos.mw.taxonomy;

/**
 * Maps location's name and code to each other.
 * @author Micha³ Krzysztof Szczerbak
 */
public class LocationMap {
	
	/**
	 * Gets the name of location.
	 * @param code code
	 * @return name
	 */
	public static String getName(long code)
	{
		return LocationTree.getName(code);
	}
	
	/**
	 * Gets the node of location.
	 * @param name name
	 * @return code
	 */
	public static long getCode(String name)
	{
		return LocationTree.getCode(name);
	}
}
