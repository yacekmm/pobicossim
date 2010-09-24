package pl.edu.pw.pobicos.mw.resource;

import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.event.EventMap;
import pl.edu.pw.pobicos.mw.event.EventTree;
import pl.edu.pw.pobicos.mw.event.PhysicalEvent;

public class GenericResource extends AbstractResource{
	private Map<Long, Method> instructionsSupported;
	private Vector<Event> eventsRaisen;
	private Vector<PhysicalEvent> physicalEventsRaisen;
	
	public GenericResource()
	{
		this.icon = loadImage();
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
	public Image loadImage() 
	{
		return new Image(Display.getCurrent(), this.getClass().getResourceAsStream(GraphicalSettings.genericNode));
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
}
