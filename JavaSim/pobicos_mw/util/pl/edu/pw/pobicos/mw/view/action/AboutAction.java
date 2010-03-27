package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

public class AboutAction extends Action implements
		IWorkbenchAction {

	private final String ID = "action.about";

	//private IWorkbenchWindow window;

	public AboutAction(IWorkbenchWindow window) {
		//this.window = window;
		setId(ID);
		setText("&About...");
		ActionContainer.add(ID, this);
		setEnabled(true);
	}

	@Override
	public void run() {
		super.run();
		//AboutDialog d = new AboutDialog(Display.getDefault().getActiveShell());
		//d.open();
		MessageDialog.openInformation(Display.getDefault().getActiveShell(), "About Rovers Simulator", "About this application there will be written here...");
	    /*if (code == Window.OK) {
	    	// TODO
	    }*/
		
	}

	public void dispose() {
		//empty
	}

}
