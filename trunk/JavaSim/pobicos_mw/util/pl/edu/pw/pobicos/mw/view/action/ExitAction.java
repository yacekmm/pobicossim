package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

public class ExitAction extends Action implements
		IWorkbenchAction {

	private final String ID = "action.exit";
	private IWorkbenchWindow window;

	//private IWorkbenchWindow window;

	public ExitAction(IWorkbenchWindow window) {
		this.window = window;
		setId(ID);
		setText("&Exit...");
		ActionContainer.add(ID, this);
		setEnabled(true);
	}

	@Override
	public void run() {
		super.run();
		if(MessageDialog.openConfirm(Display.getDefault().getActiveShell(), "Exit POBICOS", "Are you sure to exit this application?"))
		{
			//TODO finish the unfinished businesses
			ActionFactory.QUIT.create(window).run();
		}
	}

	public void dispose() {
		//empty
	}

}
