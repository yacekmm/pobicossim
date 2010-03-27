package pl.edu.pw.pobicos.mw.taxonomy;

/**
 * Maps poruct's name and code to each other.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ProductMap {
	
	/**
	 * Gets the name of product.
	 * @param code code
	 * @return name
	 */
	public static String getName(long code)
	{
		return ProductTree.getName(code);
	}
	
	/**
	 * Gets the node of product.
	 * @param name name
	 * @return code
	 */
	public static long getCode(String name)
	{
		return ProductTree.getCode(name);
	}
}
