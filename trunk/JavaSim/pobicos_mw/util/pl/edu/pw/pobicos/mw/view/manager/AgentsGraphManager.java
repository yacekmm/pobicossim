package pl.edu.pw.pobicos.mw.view.manager;

import org.eclipse.swt.widgets.Canvas;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.IAgentListener;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;

/**
 * This singleton class is manager for graphical representation of micro-agent behaviour
 * 
 * @author msmialek
 *
 */
public class AgentsGraphManager {

	private static AgentsGraphManager instance;

	private Canvas canvas;

	private AgentsGraphManager() {
		AgentsManager.getInstance().addAgentListener(new IAgentListener() {

			public void agentChanged(AbstractAgent agent) {
				if (canvas != null && !canvas.isDisposed()) {
					Display.getDefault().asyncExec(new Runnable(){
						public void run() {
							canvas.redraw();
						}						
					});
				}
			}

		});
	}

	/**
	 * Returns reference to the singleton instance of this class.
	 * 
	 * @return - reference to the singleton instance of this class 
	 */
	public static AgentsGraphManager getInstance() {
		if (instance == null)
			instance = new AgentsGraphManager();
		return instance;
	}

	/**
	 * @param canvas
	 */
	public void setCanvas(Canvas canvas) {
		this.canvas = canvas;
	}
}
