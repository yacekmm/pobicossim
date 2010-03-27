package pl.edu.pw.pobicos.mw.time;

/**
 * Defines listener for virtual time event.
 * @author Micha³ Krzysztof Szczerbak
 */
public interface TimeListener extends java.util.EventListener {
	public abstract void timeTick(TimeEvent e);
}
