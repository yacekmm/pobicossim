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
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.simulation.ISimulationListener;

public class StepForwardAction extends Action implements IWorkbenchAction, ISelectionListener {

	//private static final Log LOG = LogFactory.getLog(StepForwardAction.class);

	public static final String ID = "action.stepForward";

	//private IWorkbenchWindow window;

	public StepForwardAction(IWorkbenchWindow window) {
		//this.window = window;
		setId(ID);
		setToolTipText("Simulation step forward");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.step));
			}
		});
		ActionContainer.add(ID, this);
		SimulationsManager.getInstance().addSimulationListener(new ISimulationListener() {
			public void simulationChanged(Event event) {
				if (SimulationsManager.getInstance().isRunning()) {
					setEnabled(false);
					return;
				}
				if (SimulationsManager.getInstance().getSimulation().isLastEvent()) {
					setEnabled(false);
				} else {
					//setEnabled(true);
				}
			}
		});
		setEnabled(false);
	}

	@Override
	public void run() {
		super.run();
		SimulationsManager.getInstance().stepForward();
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
	}

	public void dispose() {
	}

}
