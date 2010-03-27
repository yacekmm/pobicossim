package pl.edu.pw.pobicos.mw.port;

public class ApplicationElement {

	String name;
	int nodeId;
	byte[] appBundle;
	
	public ApplicationElement(String name, int nodeId, byte[] appBundle)
	{
		this.name = name;
		this.nodeId = nodeId;
		this.appBundle = appBundle;
	}
	
	public String getName()
	{
		return name;
	}
	
	public int getNodeId()
	{
		return nodeId;
	}
	
	public byte[] getAppBundle()
	{
		return appBundle;
	}
}
