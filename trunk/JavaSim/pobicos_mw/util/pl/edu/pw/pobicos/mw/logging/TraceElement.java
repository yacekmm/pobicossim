package pl.edu.pw.pobicos.mw.logging;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

public class TraceElement {

	private String message;
	private AbstractNode node;
	private AbstractAgent agent;
	private long time;
	private TraceData data;

	public TraceElement(String message, AbstractNode node, AbstractAgent agent, long time, TraceData data)
	{
		this.message = message;
		this.node = node;
		this.agent = agent;
		this.time = time;
		this.data = data;
	}
	
	public String getMessage()
	{
		return message;
	}
	
	public AbstractNode getNode()
	{
		return node;
	}
	
	public AbstractAgent getAgent()
	{
		return agent;
	}
	
	public long getTime()
	{
		return time;
	}
	
	public TraceData getData()
	{
		return data;
	}
}
