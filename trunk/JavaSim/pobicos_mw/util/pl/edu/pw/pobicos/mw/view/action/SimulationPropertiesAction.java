package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.window.Window;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.view.dialog.SimulationPropertiesDialog;

public class SimulationPropertiesAction extends Action implements
		IWorkbenchAction {

	private final String ID = "action.simulationProperties";

	//private IWorkbenchWindow window;

	public SimulationPropertiesAction(IWorkbenchWindow window) {
		//this.window = window;
		setId(ID);
		setText("&Properties...");
		ActionContainer.add(ID, this);
		setEnabled(true);
	}

	@Override
	public void run() {
		super.run();
		SimulationPropertiesDialog d = new SimulationPropertiesDialog(Display
				.getDefault().getActiveShell());
		int code = d.open();
	    if (code == Window.OK) {
	    	// TODO
	    }
		
	}

	public void dispose() {
		//empty
	}

}
