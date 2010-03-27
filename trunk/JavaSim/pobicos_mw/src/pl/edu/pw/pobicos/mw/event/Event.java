package pl.edu.pw.pobicos.mw.event;

import java.util.Vector;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

public class Event {
	
	private long code;//POBICOS EventType
	private String name, source = "";//, callID;
	private long virtualTime;
	
	private AbstractNode node;
	private AbstractAgent agent;
	
	private Vector<Context> contexts = new Vector<Context>();
	
	public Event(long code)
	{
		this.code = code;
		this.name = EventMap.getName(code);
	}
	
	public Event(long code, String source, long time)
	{
		this.code = code;
		this.name = EventMap.getName(code);
		this.source = source;
		this.virtualTime = time;
	}
	
	public Event(long code, AbstractNode node, long time)
	{
		this.code = code;
		this.name = EventMap.getName(code);
		this.node = node;
		this.virtualTime = time;
	}

	public Event(long code, AbstractNode node, String callID, long time)
	{
		this.code = code;
		this.name = EventMap.getName(code);
		this.node = node;
//		this.callID = callID;
		this.virtualTime = time;
	}

	public Event(long code, AbstractAgent agent, long time)
	{
		this.code = code;
		this.name = EventMap.getName(code);
		this.agent = agent;
		this.virtualTime = time;
	}
	
	public long getCode()
	{
		return code;
	}
	
	public String getName()
	{
		return name;
	}
	
	public String getSource()
	{
		return source;
	}
	
	public long getVirtualTime()
	{
		return virtualTime;
	}
	
	public boolean isGeneric()
	{
		if(code < 268435456L)//code.startsWith("0"))
			return true;
		return false;
	}
	
	public String getType()
	{
		return (node != null ? "node" : (agent != null ? "agent" : "environment"));
	}
	
	public AbstractNode getNode()
	{
		return node;
	}
	
	public AbstractAgent getAgent()
	{
		return agent;
	}

	public void setVirtualTime(long virtualTime) 
	{
		this.virtualTime = virtualTime;
	}
	
	public void addContexts(Vector<Context> conts)
	{
		this.contexts = conts;
	}
	
	public Vector<Context> getContexts()
	{
		return contexts;
	}
}
