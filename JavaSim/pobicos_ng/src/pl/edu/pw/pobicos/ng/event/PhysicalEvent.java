package pl.edu.pw.pobicos.ng.event;

import java.util.Vector;

public class PhysicalEvent {

	private Vector<Event> events = new Vector<Event>();
	private String name = "";
	
	public PhysicalEvent(String name)
	{
		this.name = name;
	}
	
	public void addEvent(Event event)
	{
		events.add(event);
	}
	
	public Vector<Event> getEvents()
	{
		return events;
	}
	
	public String getName()
	{
		return name;
	}
}
