package pl.edu.pw.pobicos.ng.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IAction;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.window.Window;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.ng.network.Client;
import pl.edu.pw.pobicos.ng.view.NetworkViewManager;
import pl.edu.pw.pobicos.ng.view.dialog.ConnectDialog;

/**
 * Action making a network connection.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ConnectAction extends Action implements IWorkbenchAction  {

	private final String ID = "ConnectAction"; 

	/**
	 * Prepares action for menus.
	 * @param window window
	 */
	public ConnectAction(IWorkbenchWindow window)
	{
		super("", IAction.AS_CHECK_BOX);
		setId(ID);
		setText("&Connect...");
		setEnabled(true);
		setDisabledImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream("/resources/icons/connectx.png"));
			}
		});
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream("/resources/icons/connect.png"));
			}
		});
		ActionContainer.add(ID, this);
	}
	
	/* (non-Javadoc)
	 * @see org.eclipse.ui.actions.ActionFactory.IWorkbenchAction#dispose()
	 */
	public void dispose() {
		// empty
	}
	
	/**
	 * Connects.
	 * @see org.eclipse.jface.action.Action#run()
	 */
	public void run()
	{
		if(isChecked())
		{
			ConnectDialog d = new ConnectDialog(Display.getDefault().getActiveShell());
			int code = d.open();
			setChecked(false);
			if (code == Window.OK)
				if(Client.getInstance().isRunning())
				{
					setEnabled(false);
					setChecked(true);
					NetworkViewManager.getInstance().clear();
				}
		}
	}

}
