package pl.edu.pw.pobicos.mw.port;

import java.util.Vector;

public class SimulationElement {

	Vector<EventElement> events = new Vector<EventElement>();
	String name;
	long networkId;
	
	public SimulationElement(String name, long networkId)
	{
		this.name = name;
		this.networkId = networkId;
	}
	
	public void addEvent(EventElement event)
	{
		events.add(event);
	}
	
	public Vector<EventElement> getEvents()
	{
		return events;
	}
	
	public String getName()
	{
		return name;
	}
	
	public long getNetworkId()
	{
		return networkId;
	}
}
