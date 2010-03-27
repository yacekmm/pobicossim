package pl.edu.pw.pobicos.mw.port;

public class NodeElement {

	String name, nodeDef;
	int id, x, y, range;
	long memory;
	
	public NodeElement(String name, int id, int x, int y, long memory, int range, String nodeDef)
	{
		this.name = name;
		this.id = id;
		this.x = x;
		this.y = y;
		this.memory = memory;
		this.range = range;
		this.nodeDef = nodeDef;
	}
	
	public String getName()
	{
		return name;
	}
	
	public String getNodeDef()
	{
		return nodeDef;
	}
	
	public int getId()
	{
		return id;
	}
	
	public int getX()
	{
		return x;
	}
	
	public int getY()
	{
		return y;
	}
	
	public long getMemory()
	{
		return memory;
	}
	
	public int getRange()
	{
		return range;
	}
}
