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
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class AddNodeAction extends Action implements ISelectionListener, IWorkbenchAction {
	// private IWorkbenchWindow window;

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "action.addNode";

	/**
	 * NodeAction constructor
	 * 
	 * @param window
	 */
	public AddNodeAction(IWorkbenchWindow window) {
		super("", IAction.AS_CHECK_BOX);
		// this.window = window;

		setId(ID);
		setText("Add &Node ...");
		setToolTipText("New Node");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.addSimpleNode));
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
		NodesGraphManager.getInstance(NodesManager.getInstance()).setAddNodeChecked(isChecked());
	}

	public void dispose() {
		// empty
	}
}
