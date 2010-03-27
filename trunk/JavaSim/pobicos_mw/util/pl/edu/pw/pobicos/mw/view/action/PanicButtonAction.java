package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IAction;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class PanicButtonAction extends Action implements ISelectionListener, IWorkbenchAction {
	// private IWorkbenchWindow window;

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "action.PanicButton";

	/**
	 * NodeAction constructor
	 * 
	 * @param window
	 */
	public PanicButtonAction(IWorkbenchWindow window) {
		super("", IAction.AS_CHECK_BOX);
		// this.window = window;

		setId(ID);
		setText("Panic &button");
		setToolTipText("Panic button");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.panic));
			}
		});
		setEnabled(false);
		ActionContainer.add(ID, this);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
		// empty
	}

	@Override
	public void run() {
		super.run();
		for(AbstractAgent agent : AgentsManager.getInstance().getAgents())
			AgentsManager.getInstance().removeAgent(agent);
		SimulationsManager.getInstance().clearToCome();
		setChecked(false);
	}

	public void dispose() {
		// empty
	}
}
