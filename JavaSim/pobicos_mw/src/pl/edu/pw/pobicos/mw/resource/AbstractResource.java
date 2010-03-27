package pl.edu.pw.pobicos.mw.resource;

import java.util.Vector;

import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.event.PhysicalEvent;

public abstract class AbstractResource {
	protected Image icon;

	abstract public Image loadImage();
	
	abstract public Vector<Event> eventsRaisen();
	
	abstract public Vector<PhysicalEvent> physicalEventsRaisen();
	
	abstract public boolean raisesEvent(Event event);

	abstract public boolean supportsInstruction(long instr);
	
	public Image getIcon() {
		return this.icon;
	}
}
