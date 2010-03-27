package pl.edu.pw.pobicos.ng.resource;

import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import pl.edu.pw.pobicos.ng.event.Event;
import pl.edu.pw.pobicos.ng.event.EventMap;
import pl.edu.pw.pobicos.ng.event.EventTree;
import pl.edu.pw.pobicos.ng.event.PhysicalEvent;

public class GenericResource extends AbstractResource{
	private Map<Long, Method> instructionsSupported;
	private Vector<Event> eventsRaisen;
	private Vector<PhysicalEvent> physicalEventsRaisen;
	
	public GenericResource()
	{
		eventsRaisen = new Vector<Event>();
		physicalEventsRaisen = new Vector<PhysicalEvent>();
		instructionsSupported = new HashMap<Long, Method>();
		eventsRaisen.add(new Event(EventMap.getCode("Init")));
		eventsRaisen.add(new Event(EventMap.getCode("Finalize")));
		eventsRaisen.add(new Event(EventMap.getCode("ChildCreated")));
		eventsRaisen.add(new Event(EventMap.getCode("ChildCreationTimeout")));
		eventsRaisen.add(new Event(EventMap.getCode("ChildUnreachable")));
		eventsRaisen.add(new Event(EventMap.getCode("CommandArrived")));
		eventsRaisen.add(new Event(EventMap.getCode("ReportArrived")));
		eventsRaisen.add(new Event(EventMap.getCode("Timeout")));
		eventsRaisen.add(new Event(EventMap.getCode("ConfigSettingsChanged")));
    }
	
	@Override
	public Vector<Event> eventsRaisen()
	{
		return eventsRaisen;
	}
	
	@Override
	public Vector<PhysicalEvent> physicalEventsRaisen()
	{
		return physicalEventsRaisen;
	}
	
	@Override
	public boolean raisesEvent(Event event)
	{
		for(int i = 0; i < eventsRaisen.size(); i++)
			if(EventTree.eventFires(eventsRaisen.elementAt(i), event))
				return true;
		return false;
	}

	@Override
	public boolean supportsInstruction(long instr)
	{
		if(instructionsSupported.containsKey(new Long(instr)))
			return true;
		return false;
	}

	@Override
	public Vector<Long> instructionsSupported()
	{
		Vector<Long> result = new Vector<Long>();
		for(Long code : instructionsSupported.keySet())
			result.add(code);
		return result;
	}
	
	@Override
	public void runInstruction(long instr){}
}
