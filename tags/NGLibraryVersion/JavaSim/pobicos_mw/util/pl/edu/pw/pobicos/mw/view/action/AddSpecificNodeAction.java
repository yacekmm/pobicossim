package pl.edu.pw.pobicos.mw.view.action;

import java.io.File;
import java.io.FileInputStream;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IAction;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class AddSpecificNodeAction extends Action implements ISelectionListener, IWorkbenchAction {
	// private IWorkbenchWindow window;

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "action.addSpecificNode";

	private String[] filterExtensions = { "*.xml" };
	/**
	 * NodeAction constructor
	 * 
	 * @param window
	 */
	public AddSpecificNodeAction(IWorkbenchWindow window) {
		super("", IAction.AS_CHECK_BOX);
		// this.window = window;

		setId(ID);
		setText("Add &Specific Node ...");
		setToolTipText("New Specific Node");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.addNode));
			}
		});
		setEnabled(false);
		ActionContainer.add(ID, this);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
		// empty
	}

	@Override
	public void run() {
		super.run();
		if(isChecked())
		{
			FileDialog fileDialog = new FileDialog(Display.getCurrent().getActiveShell());
			fileDialog.setFilterExtensions(filterExtensions);
			final String fileName = fileDialog.open();
			Display.getDefault().asyncExec(new Runnable() {
				public void run() {
		            File plikWezla = new File(fileName);
		    		try
		    		{
			    		FileInputStream fis = new FileInputStream(plikWezla);
			    		byte[] xml = new byte[(int)plikWezla.length()];
			    		fis.read(xml);
			    		NodesGraphManager.getInstance(NodesManager.getInstance()).setAddNodeChecked(isChecked(), xml);
		    		}catch(Exception ex){}
				}
			});
			if(fileName == null)
				setChecked(false);
		}
		else
		{
			NodesGraphManager.getInstance(NodesManager.getInstance()).setAddNodeChecked(isChecked());
			NodesGraphManager.getInstance(NodesManager.getInstance()).setTempNodeDef(null);
		}
	}

	public void dispose() {
		// empty
	}
}
