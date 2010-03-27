package pl.edu.pw.pobicos.ng.logging;

/**
 * For logging events into a log view and a file.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Log {

	/**
	 * Appends a message of type INFO.
	 * @param message event message
	 */
	public static void info(String message)
	{
		//TODO: file logging
		//appendToFile(message);
		appendToLog(message);
	}

	/**
	 * Appends a message of type ERROR.
	 * @param message event message
	 */
	public static void error(String message)
	{
		//appendToFile("!!!" + message);
		appendToLog("!!!" + message);
	}
	
	private static void appendToLog(final String message)
	{
		LoggingManager.getInstance().getDisplay().asyncExec(new Runnable(){
			public void run()
			{
				if (LoggingManager.getInstance().getLog().isDisposed())
					return;
				LoggingManager.getInstance().getLog().append(message);
			}
		});
	}
}
