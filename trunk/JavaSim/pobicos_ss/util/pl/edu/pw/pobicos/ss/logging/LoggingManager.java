package pl.edu.pw.pobicos.ss.logging;

import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.widgets.Display;

/**
 * Manages SWT controls for logging in runtime.
 * @author Micha³ Krzysztof Szczerbak
 */
public class LoggingManager {
	
	private static LoggingManager instance;	
	private Display display;
	private StyledText log;
	
	/**
	 * Returns an SWT context display.
	 * @return display
	 */
	public Display getDisplay()
	{
		return display;
	}
	
	/**
	 * Returns a text area container for logs.
	 * @return container
	 */
	public StyledText getLog() {
		return log;
	}
	
	/**
	 * Sets the needed SWT elements.
	 * @param display context SWT display
	 * @param log text area container for logs
	 */
	public void init(Display display, StyledText log)
	{
		this.display = display;
		this.log = log;
	}

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static LoggingManager getInstance() {
		if(instance == null)
			instance = new LoggingManager();
		return instance;
	}
}
