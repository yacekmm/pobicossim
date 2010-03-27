package pl.edu.pw.pobicos.ng.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.ng.network.Client;

/**
 * Action disconnecting and clearing.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ClearAction extends Action implements IWorkbenchAction  {

	private final String ID = "ClearAction"; 

	/**
	 * Prepares action for menus.
	 * @param window window
	 */
	public ClearAction(IWorkbenchWindow window)
	{
		setId(ID);
		setText("C&lear");
		setEnabled(true);
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream("/resources/icons/stop.png"));
			}
		});
		ActionContainer.add(ID, this);
	}
	
	/* (non-Javadoc)
	 * @see org.eclipse.ui.actions.ActionFactory.IWorkbenchAction#dispose()
	 */
	public void dispose() {
	}
	
	/**
	 * Disconnects.
	 * @see org.eclipse.jface.action.Action#run()
	 */
	public void run()
	{
		super.run();
		Client.getInstance().disconnect();
	}

}
