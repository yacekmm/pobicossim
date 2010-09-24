package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IAction;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.view.dialog.PobicosSettingsDialog;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class PobicosSettingsAction extends Action implements ISelectionListener, IWorkbenchAction {
	// private IWorkbenchWindow window;

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "action.pobicosSettings";

	/**
	 * NodeAction constructor
	 * 
	 * @param window
	 */
	public PobicosSettingsAction(IWorkbenchWindow window) {
		super("", IAction.AS_CHECK_BOX);
		// this.window = window;

		setId(ID);
		setText("&POBICOS settings ...");
		setToolTipText("POBICOS settings");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.settings));
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
		(new PobicosSettingsDialog(Display.getDefault().getActiveShell())).open();
		setChecked(false);
	}

	public void dispose() {
		// empty
	}
}
