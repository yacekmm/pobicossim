package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;

public class ClearSimulationAction extends Action implements IWorkbenchAction {

	final static String ID = "action.clearSimulation";

	// private final IWorkbenchWindow window;

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public ClearSimulationAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setText("&Reset");
		setToolTipText("Resets the events' list");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	public void run() {

		Display.getDefault().asyncExec(new Runnable() {

			public void run() {
				try {
					SimulationsManager.getInstance().reset();
					ActionContainer.getAction(StartSimulationAction.ID).setEnabled(false);
					ActionContainer.getAction(StepForwardAction.ID).setEnabled(false);
					ActionContainer.getAction(SaveSimulationAction.ID).setEnabled(false);
				} catch (Exception e) {
				}
			}
		});
	}

	public void dispose() {
		// empty
	}
}
