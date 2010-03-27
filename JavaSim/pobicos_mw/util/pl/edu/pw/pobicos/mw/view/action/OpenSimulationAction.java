package pl.edu.pw.pobicos.mw.view.action;

import java.io.FileInputStream;

import org.eclipse.jface.action.Action;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.view.SimulationView;

/**
 * Instances of this class allows to open Roves Simulation XML config file.
 * 
 * @author Marcin Smialek
 * @created 2006-09-04 22:03:53
 */
public class OpenSimulationAction extends Action implements IWorkbenchAction {

	final static String ID = "action.openSimulation";

	private IWorkbenchWindow window;

	private String[] filterExtendsions = { "*.xml" };

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public OpenSimulationAction(IWorkbenchWindow window) {
		this.window = window;
		setId(ID);
		setText("&Open...");
		setToolTipText("Opens wizard to get Rovers simulation file");
		ActionContainer.add(ID, this);
		setEnabled(false);
	}

	public void run() {
		FileDialog fileDialog = new FileDialog(Display.getCurrent().getActiveShell());
		fileDialog.setFilterExtensions(filterExtendsions);
		final String fileName = fileDialog.open();

		Display.getDefault().asyncExec(new Runnable() {
			public void run() {
				try {
					if (fileName != null) {
						SimulationsManager.getInstance().reset();
						SimulationsManager.getInstance().loadSimulation(new FileInputStream(fileName));
						
						SimulationView simulationView = (SimulationView) window.getWorkbench()
								.getActiveWorkbenchWindow().getActivePage().findView(SimulationView.ID);
						simulationView.fillTable();
						if(SimulationsManager.getInstance().getSimulation().getEventList().size() > 0)
						{
							ActionContainer.getAction(StartSimulationAction.ID).setEnabled(true);
							ActionContainer.getAction(StepForwardAction.ID).setEnabled(true);
							ActionContainer.getAction(SaveSimulationAction.ID).setEnabled(true);
						}
					} else{
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
