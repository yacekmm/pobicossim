package pl.edu.pw.pobicos.mw.view.listener;

import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;
import pl.edu.pw.pobicos.mw.view.manager.SimulationGraphManager;

/**
 * Paints whole Rovers network.
 * 
 * @author Marcin Smialek
 * @created 2006-09-05 15:31:09
 */
public class NetworkPaintListener implements Listener {

    public void handleEvent(Event event) 
    {
		SimulationGraphManager.getInstance().drawMessages(event);
		NodesGraphManager.getInstance(NodesManager.getInstance()).drawNodes(event);
    }
}