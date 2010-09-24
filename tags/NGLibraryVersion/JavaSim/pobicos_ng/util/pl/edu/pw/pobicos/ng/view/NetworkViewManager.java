package pl.edu.pw.pobicos.ng.view;

import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Display;

/**
 * Manages SWT controls for network update in runtime.
 * @author Micha³ Krzysztof Szczerbak
 */
public class NetworkViewManager {
	
	private static NetworkViewManager instance;

	private NetworkViewManager()
	{//empty
	}
	
	private Display display;
	private Composite container;

	/**
	 * Returns an SWT context display.
	 * @return display
	 */
	public Display getDisplay()
	{
		return display;
	}

	/**
	 * Sets the needed SWT elements.
	 * @param display context SWT display
	 * @param container panel container for network
	 */
	public void init(Display display, Composite container)
	{
		this.display = display;
		this.container = container;
	}

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static NetworkViewManager getInstance() {
		if(instance == null)
			instance = new NetworkViewManager();
		return instance;
	}

	/**
	 * Paints the whole network view.
	 */
	public void paint() {
		display.asyncExec(new Runnable(){

			public void run() {
				if(!container.isDisposed())
					NetworkView.getInstance().paint(container);
			}
		});
	}

	/**
	 * Clears the whole network view.
	 */
	public void clear() {
		display.asyncExec(new Runnable(){

			public void run() {
				NetworkView.getInstance().clear(container);
			}
		});
	}
}
