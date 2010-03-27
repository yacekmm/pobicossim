package pl.edu.pw.pobicos.mw.view.action;

import java.util.Random;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.PlatformUI;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.view.NetworkView;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

public class NewNetworkAction extends Action implements IWorkbenchAction {


	private final String ID = "action.newConfig";

	// private final IWorkbenchWindow window;

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public NewNetworkAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setText("&New");
		setToolTipText("Opens dialog to get network configuration file");
		ActionContainer.add(ID, this);
	}

	public void run() {

		Display.getDefault().asyncExec(new Runnable() {

			public void run() {
				try {
					NodesManager.resetIdCounter();
				    ActionContainer.getAction(CloseAction.ID).run();
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
					
					PobicosManager.getInstance().setNetworkId((new Random()).nextLong());
				} catch (Exception e) {

				}
			}
		});
	}

	public void dispose() {
		// empty
	}
}
