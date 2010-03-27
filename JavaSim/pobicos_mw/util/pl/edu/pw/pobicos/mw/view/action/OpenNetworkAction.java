package pl.edu.pw.pobicos.mw.view.action;

import java.io.FileInputStream;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.PlatformUI;
import org.eclipse.ui.actions.ActionFactory;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.view.NetworkView;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * Instances of this class allows opening Rovers Network config file.
 * 
 * @author Marcin Smialek
 * @created 2006-09-04 20:48:00
 */
public class OpenNetworkAction extends Action implements ActionFactory.IWorkbenchAction {

	private final String ID = "action.openConfig";

	// private final IWorkbenchWindow window;

	private String[] filterExtensions = { "*.xml" };

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public OpenNetworkAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setText("&Open...");
		setToolTipText("Opens dialog to get network configuration file");
		ActionContainer.add(ID, this);
	}

	public void run() {
		FileDialog fileDialog = new FileDialog(Display.getCurrent().getActiveShell());
		fileDialog.setFilterExtensions(filterExtensions);
		final String fileName = fileDialog.open();

		Display.getDefault().asyncExec(new Runnable() {
			public void run() {
				try {
					if (fileName != null) {
						NodesManager.resetIdCounter();
						ActionContainer.getAction(CloseAction.ID).run();
						PobicosManager.getInstance().loadConfig(new FileInputStream(fileName), fileName.substring(0, fileName.lastIndexOf("\\")) + "\\");
					    ActionContainer.getAction(SaveNetworkAction.ID).setEnabled(true);
						ActionContainer.getAction(AddNodeAction.ID).setEnabled(true);
						ActionContainer.getAction(AddSpecificNodeAction.ID).setEnabled(true);
						ActionContainer.getAction(ZoomOutAction.ID).setEnabled(true);
						ActionContainer.getAction(ZoomInAction.ID).setEnabled(true);
						ActionContainer.getAction(ShowAdditionalInfoAction.ID).setEnabled(true);
						ActionContainer.getAction(CloseAction.ID).setEnabled(true);
						ActionContainer.getAction(LogTraceAction.ID).setEnabled(true);
						ActionContainer.getAction(OpenSimulationAction.ID).setEnabled(true);
						ActionContainer.getAction(PanicButtonAction.ID).setEnabled(true);
						ActionContainer.getAction(PobicosSettingsAction.ID).setEnabled(true);
						ActionContainer.getAction(RestartSimulationAction.ID).setEnabled(true);
						ActionContainer.getAction(SaveSimulationAction.ID).setEnabled(true);
						ActionContainer.getAction(SaveNetworkAction.ID).setEnabled(true);
						//ActionContainer.getAction(StartSimulationAction.ID).setEnabled(true);
						//ActionContainer.getAction(StepForwardAction.ID).setEnabled(true);
						((ShowAdditionalInfoAction) (ActionContainer.getAction(ShowAdditionalInfoAction.ID))).refresh();

						NetworkView view = (NetworkView) PlatformUI.getWorkbench().getActiveWorkbenchWindow()
								.getActivePage().findView(NetworkView.ID);
						view.getCanvas().setBackground(
								GraphicalSettings.activeNetworkViewBackground);
					}
				} catch (Exception e) {
				}
			}
		});
	}

	public void dispose() {
		// empty
	}
}