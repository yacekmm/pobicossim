package pl.edu.pw.pobicos.mw.port;

import java.util.Vector;

public class NetworkElement {

	Vector<NodeElement> nodes = new Vector<NodeElement>();
	Vector<ApplicationElement> apps = new Vector<ApplicationElement>();
	String name;
	long id;
	
	public NetworkElement(String name, long id)
	{
		this.name = name;
		this.id = id;
	}
	
	public void addNode(NodeElement node)
	{
		nodes.add(node);
	}
	
	public void addApplication(ApplicationElement app)
	{
		apps.add(app);
	}
	
	public Vector<NodeElement> getNodes()
	{
		return nodes;
	}
	
	public Vector<ApplicationElement> getApplications()
	{
		return apps;
	}
	
	public String getName()
	{
		return name;
	}
	
	public long getId()
	{
		return id;
	}
}
