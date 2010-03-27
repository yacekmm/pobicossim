package pl.edu.pw.pobicos.mw.view.dialog;

import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.swt.widgets.Shell;

public class AboutDialog extends MessageDialog {
	
	public AboutDialog(Shell parentShell) {
		super(parentShell,"About Rovers Simulator", null, "About this application there will be written here...", 2, null, 0);
	}
}
