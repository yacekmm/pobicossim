package pl.edu.pw.pobicos.mw.time;

/**
 * Defines event for ticking of virtual time each defined period.
 * @author Micha³ Krzysztod Szczerbak
 */
public class TimeEvent extends java.awt.AWTEvent {
	
	private static final long serialVersionUID = 1L;

	/**
	 * Constructor.
	 * @param source virtual time
	 * @param id global identifier
	 */
	public TimeEvent(Time source, int id) {
        super(source, id);
    }
}