package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;

/**
 * Represents action that can stop currently performed Rovers Network simulation.
 * 
 * @author Marcin Smialek
 * @created 2006-09-05 17:15:00
 */
public class StopSimulationAction extends Action implements IWorkbenchAction, ISelectionListener {

	/**
	 * Unique action id.
	 */
	public static final String ID = "action.stopSimulation";

	// private IWorkbenchWindow window;

	/**
	 * @param window
	 */
	public StopSimulationAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setToolTipText("Stop simulation");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.stop));
			}
		});
		setEnabled(false);
		ActionContainer.add(ID, this);

		/*SimulationsManager.getInstance().addSimulationListener(new ISimulationListener() {

			public void simulationChanged(Event event) {
				if (SimulationsManager.getInstance().getSimulation() != null
						&& SimulationsManager.getInstance().getSimulation().isLastEvent()) {
					//Clocks.stop();
					//StopSimulationAction.this.setEnabled(false);
					//ActionContainer.getAction(StartSimulationAction.ID).setEnabled(true);
					//ActionContainer.getAction(StepForwardAction.ID).setEnabled(true);
				}
			}
		});*/
	}

	@Override
	public void run() {
		SimulationsManager.getInstance().stopSimulation();
		this.setEnabled(false);
		ActionContainer.getAction(StartSimulationAction.ID).setEnabled(true);
		ActionContainer.getAction(StepForwardAction.ID).setEnabled(true);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
		// empty
	}

	public void dispose() {
		// empty
	}
}
