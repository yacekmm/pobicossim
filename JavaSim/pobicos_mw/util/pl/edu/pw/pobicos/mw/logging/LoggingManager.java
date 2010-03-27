package pl.edu.pw.pobicos.mw.logging;

import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.logging.LoggingManager;

public class LoggingManager {
	
	private static LoggingManager instance;
	
	private Display display;
	private StyledText log;
	public Display getDisplay()
	{
		return display;
	}
	
	public StyledText getLog() {
		return log;
	}
	public void init(Display display, StyledText log)
	{
		this.display = display;
		this.log = log;
	}
	
	public static LoggingManager getInstance() {
		if(instance == null)
			instance = new LoggingManager();
		return instance;
	}
}
