package pl.edu.pw.pobicos.ng.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

/**
 * Action displaying "about" window.
 * @author Micha³ Krzysztof Szczerbak
 */
public class AboutAction extends Action implements IWorkbenchAction  {

	private final String ID = "AboutAction"; 
		
	/**
	 * Prepares action for menus.
	 * @param window window
	 */
	public AboutAction(IWorkbenchWindow window)
	{
		//super("", IAction.AS_CHECK_BOX);
		setId(ID);
		setText("&About...");
		setEnabled(true);
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream("/resources/icons/about.png"));
			}
		});
		ActionContainer.add(ID, this);
	}
	
	/**
	 * Opens an information dialog.
	 * @see org.eclipse.jface.action.Action#run()
	 */
	public void run() {
		super.run();
		MessageDialog.openInformation(Display.getDefault().getActiveShell(), "About POBICOS NGsim", "About this application there will be written here...");
		
	}
	
	/* (non-Javadoc)
	 * @see org.eclipse.ui.actions.ActionFactory.IWorkbenchAction#dispose()
	 */
	public void dispose() {
	}

}
