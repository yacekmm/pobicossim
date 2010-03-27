package pl.edu.pw.pobicos.ng.taxonomy;


public class ProductMap {
	public static String getName(long code)
	{
		return ProductTree.getName(code);
	}
	
	public static long getCode(String name)
	{
		return ProductTree.getCode(name);
	}
}
