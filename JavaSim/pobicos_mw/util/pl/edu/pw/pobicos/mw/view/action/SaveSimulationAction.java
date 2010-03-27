package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.window.Window;
import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.view.dialog.SimulationDescriptionDialog;

public class SaveSimulationAction extends Action implements IWorkbenchAction {
	
    public final static String ID = "action.saveSimulation";
    
	public SaveSimulationAction(IWorkbenchWindow window) {
		setId(ID);
		setText("&Save...");
		setToolTipText("Opens dialog to save simulation file");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	@Override
	public void run() {
		String simulationDescr = "";
		SimulationDescriptionDialog d = new SimulationDescriptionDialog(Display.getDefault().getActiveShell());
		int code = d.open();
		if (code == Window.CANCEL)
			return;
		if (code == Window.OK)
			simulationDescr = d.getSimulationDescription();
		
		FileDialog fileDialog = new FileDialog(Display.getDefault()
				.getActiveShell(), SWT.SAVE);
		String[] extensions = {"*.xml"};
		fileDialog.setFilterExtensions(extensions);
		String fileName = fileDialog.open();
		SimulationsManager.getInstance().saveSimulation(simulationDescr, fileName);
	}

	public void dispose() {
		// empty
	}
}
