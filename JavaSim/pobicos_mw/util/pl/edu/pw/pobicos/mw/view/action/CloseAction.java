package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.PlatformUI;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.view.NetworkView;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class CloseAction extends Action implements IWorkbenchAction, ISelectionListener {

	//private static final Log LOG = LogFactory.getLog(CloseAction.class);

	/**
	 * TODO MS - comments here
	 */
	public final static String ID = "action.close";

	//private IWorkbenchWindow window;

	/**
	 * @param window
	 */
	public CloseAction(IWorkbenchWindow window) {
		//this.window = window;
		setId(ID);
		setText("&Close");
		setToolTipText("Close");
		setEnabled(false);
		ActionContainer.add(ID, this);
	}

	@Override
	public void run() {
		clear();
		NetworkView view = (NetworkView) PlatformUI.getWorkbench().getActiveWorkbenchWindow().getActivePage()
				.findView(NetworkView.ID);
		view.getCanvas().setBackground(GraphicalSettings.passiveNetworkViewBackground);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
		// TODO Auto-generated method stub

	}

	public void dispose() {
		// TODO Auto-generated method stub

	}

	/**
	 * TODO MS - comments here
	 */
	private void clear() {
		PobicosManager.getInstance().clear();
		SimulationsManager.getInstance().clear();
		ActionContainer.getAction(AddNodeAction.ID).setEnabled(false);
		ActionContainer.getAction(AddSpecificNodeAction.ID).setEnabled(false);
		ActionContainer.getAction(StepForwardAction.ID).setEnabled(false);
		ActionContainer.getAction(LogTraceAction.ID).setEnabled(false);
		ActionContainer.getAction(OpenSimulationAction.ID).setEnabled(false);
		ActionContainer.getAction(PanicButtonAction.ID).setEnabled(false);
		ActionContainer.getAction(PobicosSettingsAction.ID).setEnabled(false);
		ActionContainer.getAction(RestartSimulationAction.ID).setEnabled(false);
		ActionContainer.getAction(SaveSimulationAction.ID).setEnabled(false);
		ActionContainer.getAction(SaveNetworkAction.ID).setEnabled(false);
		ActionContainer.getAction(StartSimulationAction.ID).setEnabled(false);
		ActionContainer.getAction(StopSimulationAction.ID).setEnabled(false);
		ActionContainer.getAction(ZoomOutAction.ID).setEnabled(false);
		ActionContainer.getAction(ZoomInAction.ID).setEnabled(false);
		ActionContainer.getAction(ShowAdditionalInfoAction.ID).setEnabled(false);
		setEnabled(false);
	}
}
