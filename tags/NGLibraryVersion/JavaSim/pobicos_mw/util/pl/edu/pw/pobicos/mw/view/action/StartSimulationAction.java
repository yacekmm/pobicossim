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
 * Represents start simulation action.
 * 
 * @author Marcin Smialek
 * @created 2006-09-05 16:10:00
 */
public class StartSimulationAction extends Action implements IWorkbenchAction,
		ISelectionListener {

	/**
	 * Unique Id of the action
	 */
	public static final String ID = "action.startSimulation";

	// private IWorkbenchWindow window;

	/**
	 * @param window
	 */
	public StartSimulationAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setToolTipText("Start simulation");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.play));
			}
		});
		setEnabled(false);
		ActionContainer.add(ID, this);
	}

	@Override
	public void run() {
		super.run();
		SimulationsManager.getInstance().startSimulation();
		this.setEnabled(false);
		ActionContainer.getAction(ID).setEnabled(false);
		ActionContainer.getAction(StopSimulationAction.ID).setEnabled(true);
		ActionContainer.getAction(StepForwardAction.ID).setEnabled(false);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
		// empty
	}

	public void dispose() {
		// empty
	}
}
