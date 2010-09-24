package pl.edu.pw.pobicos.ng.resource;

import java.util.Vector;

import pl.edu.pw.pobicos.ng.event.Event;
import pl.edu.pw.pobicos.ng.event.PhysicalEvent;

public abstract class AbstractResource {
	
	abstract public Vector<Event> eventsRaisen();
	
	abstract public Vector<PhysicalEvent> physicalEventsRaisen();
	
	abstract public boolean raisesEvent(Event event);

	abstract public boolean supportsInstruction(long instr);
	
	abstract public void runInstruction(long instr);
	
	abstract public Vector<Long> instructionsSupported();
}
