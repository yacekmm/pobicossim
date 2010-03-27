package pl.edu.pw.pobicos.ng;

import org.eclipse.jface.action.ICoolBarManager;
import org.eclipse.jface.action.IMenuManager;
import org.eclipse.jface.action.IToolBarManager;
import org.eclipse.jface.action.MenuManager;
import org.eclipse.jface.action.ToolBarManager;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;
import org.eclipse.ui.application.ActionBarAdvisor;
import org.eclipse.ui.application.IActionBarConfigurer;

import pl.edu.pw.pobicos.ng.view.action.AboutAction;
import pl.edu.pw.pobicos.ng.view.action.ClearAction;
import pl.edu.pw.pobicos.ng.view.action.ConnectAction;

/**
 * An action bar advisor is responsible for creating, adding, and disposing of
 * the actions added to a workbench window. Each window will be populated with
 * new actions.
 */
public class ApplicationActionBarAdvisor extends ActionBarAdvisor {

	// Actions - important to allocate these only in makeActions, and then use
	// them
	// in the fill methods. This ensures that the actions aren't recreated
	// when fillActionBars is called with FILL_PROXY.
	private IWorkbenchAction aboutAction;
	private IWorkbenchAction exitAction;
	private IWorkbenchAction clearAction;
	private IWorkbenchAction connectAction;

	public ApplicationActionBarAdvisor(IActionBarConfigurer configurer) {
		super(configurer);
	}

	protected void makeActions(final IWorkbenchWindow window) {
		// Creates the actions and registers them.
		// Registering is needed to ensure that key bindings work.
		// The corresponding commands keybindings are defined in the plugin.xml
		// file.
		// Registering also provides automatic disposal of the actions when
		// the window is closed.

		exitAction = ActionFactory.QUIT.create(window);
		register(exitAction);
		aboutAction = new AboutAction(window);
		register(aboutAction);
		this.clearAction = new ClearAction(window);
		register(this.clearAction);		
		this.connectAction =  new ConnectAction(window);
		register(this.connectAction);
	}

	protected void fillMenuBar(IMenuManager menuBar) {
		MenuManager networkMenu = new MenuManager("&Network","network");
		menuBar.add(networkMenu);
		networkMenu.add(this.clearAction);
		networkMenu.add(this.connectAction);
		MenuManager applicationMenu = new MenuManager("&Application","application");
		menuBar.add(applicationMenu);
		applicationMenu.add(aboutAction);
		applicationMenu.add(exitAction);
	}
	
	@Override
	protected void fillCoolBar(ICoolBarManager coolBar) {
		IToolBarManager toolBar1 = new ToolBarManager(coolBar.getStyle());
		toolBar1.add(this.connectAction);
		toolBar1.add(this.clearAction);
		toolBar1.add(this.aboutAction);
		coolBar.add(toolBar1);
	}

}
