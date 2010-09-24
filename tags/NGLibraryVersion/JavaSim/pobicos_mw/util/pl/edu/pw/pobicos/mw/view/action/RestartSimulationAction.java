package pl.edu.pw.pobicos.mw.view.action;

import java.util.Vector;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;

public class RestartSimulationAction extends Action implements IWorkbenchAction {

	final static String ID = "action.restartSimulation";

	// private final IWorkbenchWindow window;

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public RestartSimulationAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setText("&Restart");
		setToolTipText("Restarts the simulation");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	public void run() {

		Display.getDefault().asyncExec(new Runnable() {

			public void run() {
				try 
				{
					Vector<AbstractAgent> agents = new Vector<AbstractAgent>();
					for(AbstractAgent agent : AgentsManager.getInstance().getAgents())
						if(AgentsManager.getInstance().isRootAgent(agent))
							agents.add(agent);
					AgentsManager.getInstance().clear();
					Vector<Event> events = new Vector<Event>();
					for(Event event : SimulationsManager.getInstance().getSimulation().getEventList())
						if(!event.isGeneric())
							events.add(event);
					SimulationsManager.getInstance().reset();
					for(AbstractAgent agent : agents)
						PobicosManager.getInstance().installRootAgent(AgentsManager.getInstance().getNode(agent), agent.getName(), AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(agent)).getByteCode());
					for(Event event : events)
						PobicosManager.getInstance().addEvent(event.getName(), event.getNode(), event.getAgent(), event.getSource());
					ActionContainer.getAction("action.stepForward").setEnabled(true);
					ActionContainer.getAction("action.startSimulation").setEnabled(true);
				} catch (Exception e) {
				}
			}
		});
	}

	public void dispose() {
		// empty
	}
}
