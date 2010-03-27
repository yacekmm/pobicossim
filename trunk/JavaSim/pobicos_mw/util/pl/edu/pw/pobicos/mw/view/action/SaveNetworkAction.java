package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.window.Window;
import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.view.dialog.RoversDescriptionDialog;

public class SaveNetworkAction extends Action implements IWorkbenchAction {
	
    final static String ID = "action.saveNetwork";
    
	public SaveNetworkAction(IWorkbenchWindow window) {
		setId(ID);
		setText("&Save...");
		setToolTipText("Opens dialog to save network configuration file");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	@Override
	public void run() {
		String roversDescr = "";
		RoversDescriptionDialog d = new RoversDescriptionDialog(Display.getDefault()
				.getActiveShell());
		int code = d.open();
		if (code == Window.CANCEL)
			return;
		if (code == Window.OK)
			roversDescr = d.getRoversDescritption();
		
		FileDialog fileDialog = new FileDialog(Display.getDefault()
				.getActiveShell(), SWT.SAVE);
		String[] extensions = {"*.xml"};
		fileDialog.setFilterExtensions(extensions);
		String fileName = fileDialog.open();
		PobicosManager.getInstance().saveConfig(roversDescr, fileName);
	}

	public void dispose() {
		// empty
	}
}
