package pl.edu.pw.pobicos.mw;

import org.eclipse.jface.action.ICoolBarManager;
import org.eclipse.jface.action.IMenuManager;
import org.eclipse.jface.action.IToolBarManager;
import org.eclipse.jface.action.MenuManager;
import org.eclipse.jface.action.ToolBarManager;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;
import org.eclipse.ui.application.ActionBarAdvisor;
import org.eclipse.ui.application.IActionBarConfigurer;

import pl.edu.pw.pobicos.mw.view.action.AboutAction;
import pl.edu.pw.pobicos.mw.view.action.AddNodeAction;
import pl.edu.pw.pobicos.mw.view.action.AddSpecificNodeAction;
import pl.edu.pw.pobicos.mw.view.action.CloseAction;
import pl.edu.pw.pobicos.mw.view.action.DeleteNodeAction;
import pl.edu.pw.pobicos.mw.view.action.ExitAction;
import pl.edu.pw.pobicos.mw.view.action.LogTraceAction;
import pl.edu.pw.pobicos.mw.view.action.NewNetworkAction;
import pl.edu.pw.pobicos.mw.view.action.OpenNetworkAction;
import pl.edu.pw.pobicos.mw.view.action.OpenSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.PanicButtonAction;
import pl.edu.pw.pobicos.mw.view.action.PobicosSettingsAction;
import pl.edu.pw.pobicos.mw.view.action.RestartSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.SaveNetworkAction;
import pl.edu.pw.pobicos.mw.view.action.SaveSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.ShowAdditionalInfoAction;
import pl.edu.pw.pobicos.mw.view.action.SimulationPropertiesAction;
import pl.edu.pw.pobicos.mw.view.action.StartSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.StepForwardAction;
import pl.edu.pw.pobicos.mw.view.action.StopSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.ZoomInAction;
import pl.edu.pw.pobicos.mw.view.action.ZoomOutAction;

/**
 * This class is responsible for creating actions and for placing them in the Workbench. 
 * @author Marcin Smialek, Michal Szczerbak
 */
public class ApplicationActionBarAdvisor extends ActionBarAdvisor {

	//private action variables
	private IWorkbenchAction newConfigAction;
	private IWorkbenchAction openConfigAction;
	private IWorkbenchAction closeAction;
	private IWorkbenchAction saveAsAction;
	private IWorkbenchAction aboutAction;
	private IWorkbenchAction exitAction;
	private IWorkbenchAction openSimulationAction;
	private IWorkbenchAction restartSimulationAction;	
	private IWorkbenchAction saveSimulationAsAction;	
	private IWorkbenchAction logTraceAction;
	private IWorkbenchAction simulationPropertiesAction;
	private IWorkbenchAction showAdditionalInfoAction;
	private IWorkbenchAction addNodeAction;
	private IWorkbenchAction removeNodeAction;
	private IWorkbenchAction zoomOutAction;
	private IWorkbenchAction zoomInAction;
	private IWorkbenchAction stepForwardAction;
	private IWorkbenchAction startSimulationAction;
	private IWorkbenchAction stopSimulationAction;
	private IWorkbenchAction addSpecificNodeAction;
	private IWorkbenchAction pobicosSettingsAction;
	private IWorkbenchAction panicButtonAction;
	
	/**
	 * Constructs a new instance of this class given configurer interface.
	 * @param configurer - interface enabling the programmer to configure action bars
	 */
	public ApplicationActionBarAdvisor(IActionBarConfigurer configurer) 
	{
		super(configurer);
	}

	@Override
	protected void makeActions(IWorkbenchWindow window) {
		
		this.newConfigAction = new NewNetworkAction(window);
		register(this.newConfigAction);
		
		this.openConfigAction = new OpenNetworkAction(window);
		register(this.openConfigAction);

		this.closeAction = new CloseAction(window);
		register(this.closeAction);

		this.exitAction = new ExitAction(window);
		register(this.exitAction);

		//this.aboutAction = ActionFactory.ABOUT.create(window);
		this.aboutAction = new AboutAction(window);
		register(this.aboutAction);

		this.saveAsAction = new SaveNetworkAction(window);
		register(this.saveAsAction);

		this.logTraceAction = new LogTraceAction(window);
		register(this.logTraceAction);

		this.restartSimulationAction = new RestartSimulationAction(window);
		register(this.restartSimulationAction);

		this.openSimulationAction = new OpenSimulationAction(window);
		register(this.openSimulationAction);
		
		this.saveSimulationAsAction = new SaveSimulationAction(window);
		register(this.saveSimulationAsAction);

		this.simulationPropertiesAction = new SimulationPropertiesAction(window);
		register(this.simulationPropertiesAction);

		this.showAdditionalInfoAction = new ShowAdditionalInfoAction(window);
		register(this.showAdditionalInfoAction);

		this.addNodeAction = new AddNodeAction(window);
		register(this.addNodeAction);

		this.removeNodeAction = new DeleteNodeAction(window);
		register(this.removeNodeAction);

		this.zoomOutAction = new ZoomOutAction(window);
		register(this.zoomOutAction);

		this.zoomInAction = new ZoomInAction(window);
		register(this.zoomInAction);

		this.stepForwardAction = new StepForwardAction(window);
		register(this.stepForwardAction);

		this.startSimulationAction = new StartSimulationAction(window);
		register(this.startSimulationAction);

		this.stopSimulationAction = new StopSimulationAction(window);
		register(this.stopSimulationAction);

		this.addSpecificNodeAction = new AddSpecificNodeAction(window);
		register(this.addSpecificNodeAction);

		this.pobicosSettingsAction = new PobicosSettingsAction(window);
		register(this.pobicosSettingsAction);

		this.panicButtonAction = new PanicButtonAction(window);
		register(this.panicButtonAction);
	}

	@Override
	protected void fillMenuBar(IMenuManager menuBar) {
		MenuManager networkMenu = new MenuManager("&Network", "network");
		networkMenu.add(this.newConfigAction);
		networkMenu.add(this.openConfigAction);
		networkMenu.add(this.closeAction);
		networkMenu.add(this.saveAsAction);

		MenuManager simulationMenu = new MenuManager("&Simulation", "simulation");
		simulationMenu.add(this.restartSimulationAction);
		simulationMenu.add(this.openSimulationAction);
		simulationMenu.add(this.saveSimulationAsAction);
		simulationMenu.add(this.logTraceAction);
		simulationMenu.add(this.simulationPropertiesAction);

		MenuManager programMenu = new MenuManager("&Program", "program");
		programMenu.add(aboutAction);
		programMenu.add(this.exitAction);

		menuBar.add(networkMenu);
		menuBar.add(simulationMenu);
		menuBar.add(programMenu);
	}

	@Override
	protected void fillCoolBar(ICoolBarManager coolBar) {
		IToolBarManager toolBar1 = new ToolBarManager(coolBar.getStyle());
		toolBar1.add(this.addNodeAction);
		toolBar1.add(this.addSpecificNodeAction);
		coolBar.add(toolBar1);

		IToolBarManager toolBar2 = new ToolBarManager(coolBar.getStyle());
		toolBar2.add(this.zoomOutAction);
		toolBar2.add(this.zoomInAction);
		toolBar2.add(this.showAdditionalInfoAction);
		coolBar.add(toolBar2);

		IToolBarManager toolBar3 = new ToolBarManager(coolBar.getStyle());
		toolBar3.add(this.startSimulationAction);
		toolBar3.add(this.stopSimulationAction);
		toolBar3.add(this.stepForwardAction);
		coolBar.add(toolBar3);

		IToolBarManager toolBar4 = new ToolBarManager(coolBar.getStyle());
		toolBar4.add(this.pobicosSettingsAction);
		toolBar4.add(this.panicButtonAction);
		coolBar.add(toolBar4);
	}
}
