package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.SWT;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.logging.Trace;

public class LogTraceAction extends Action implements IWorkbenchAction {
	
    final static String ID = "action.logTrace";
    
	public LogTraceAction(IWorkbenchWindow window) {
		setId(ID);
		setText("&Log trace...");
		setToolTipText("Opens dialog to save trace file");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	@Override
	public void run() {
		FileDialog fileDialog = new FileDialog(Display.getDefault()
				.getActiveShell(), SWT.SAVE);
		String[] extensions = {"*.xml"};
		fileDialog.setFilterExtensions(extensions);
		String fileName = fileDialog.open();
		if(fileName != null)
			Trace.saveTrace(fileName);
	}

	public void dispose() {
		// empty
	}
}
