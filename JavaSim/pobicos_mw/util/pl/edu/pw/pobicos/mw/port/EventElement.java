package pl.edu.pw.pobicos.mw.port;

public class EventElement {

	long code, virtualTime;
	int nodeId;
	
	public EventElement(long code, int nodeId, long virtualTime)
	{
		this.code = code;
		this.nodeId = nodeId;
		this.virtualTime = virtualTime;
	}
	
	public long getCode()
	{
		return code;
	}
	
	public int getNodeId()
	{
		return nodeId;
	}
	
	public long getVirtualTime()
	{
		return virtualTime;
	}
}
