package pl.edu.pw.pobicos.ss;

import java.io.InputStream;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.TrayItem;
import org.eclipse.ui.application.ActionBarAdvisor;
import org.eclipse.ui.application.IActionBarConfigurer;
import org.eclipse.ui.application.IWorkbenchWindowConfigurer;
import org.eclipse.ui.application.WorkbenchWindowAdvisor;

public class ApplicationWorkbenchWindowAdvisor extends WorkbenchWindowAdvisor {

    public ApplicationWorkbenchWindowAdvisor(IWorkbenchWindowConfigurer configurer) {
        super(configurer);
    }

    public ActionBarAdvisor createActionBarAdvisor(IActionBarConfigurer configurer) {
        return new ApplicationActionBarAdvisor(configurer);
    }

    public void preWindowOpen() {
        IWorkbenchWindowConfigurer configurer = getWindowConfigurer();
        configurer.setInitialSize(new Point(700, 550));
        configurer.setShowCoolBar(false);
        configurer.setShowStatusLine(false);
        configurer.setTitle("POBICOS Management Server");
    }

    public void postWindowOpen()
    {
    	super.postWindowOpen();
 //   	minimizeToTray();
    }
    
    /**
     * Creates Tray Icon and minimizes server window to tray
     * @author Jacek Milewski
     */
    public void minimizeToTray()
    {
    	Display display = Display.getCurrent();
    	InputStream is = this.getClass().getResourceAsStream("/resources/icons/p.gif");
    	final Image image = new Image(display, is);
    	final TrayItem trayItem = new TrayItem(display.getSystemTray(), SWT.NONE);
    	trayItem.setImage(image);
    	trayItem.setToolTipText("POBICOS Management Server");
    	getWindowConfigurer().getWindow().getShell().setVisible(false);
    	
    	trayItem.addSelectionListener(new SelectionAdapter() {
	    		public void widgetDefaultSelected(SelectionEvent e) {
	    			Shell workbenchWindowShell = getWindowConfigurer().getWindow().getShell();
					if(!workbenchWindowShell.isVisible())
					{
						workbenchWindowShell.setVisible(true);
						workbenchWindowShell.setActive();
						workbenchWindowShell.setFocus();
						workbenchWindowShell.setMinimized(false);
					}
					else
					{
						workbenchWindowShell.setVisible(false);
					}
					//image.dispose();
					//trayItem.dispose();
				}
			});
	
    	Shell workbenchWindowShell = getWindowConfigurer().getWindow().getShell();
		//Create a Menu
		final Menu menu = new Menu(workbenchWindowShell, SWT.POP_UP);
		//Create the exit menu item.
		final MenuItem exit = new MenuItem(menu, SWT.PUSH);
		exit.setText("Exit");
		//Do a workbench close in the event handler for exit menu item.
		exit.addListener(SWT.Selection, new Listener() {
				public void handleEvent(Event event) {
					//image.dispose();
					//trayItem.dispose();
					//open.dispose();
					exit.dispose();
					menu.dispose();
					getWindowConfigurer().getWorkbenchConfigurer().getWorkbench().close();
					}
			});
		trayItem.addListener(SWT.MenuDetect, new Listener() {
		public void handleEvent(Event event) {
		menu.setVisible(true);
		}
		});
    }
    
    /**
     * Hides Server to tray after clicking the Closing Button 
     * (instead of closing application)
     * @author Jacek Milewski
     */
    public boolean preWindowShellClose()
    {
//    	getWindowConfigurer().getWindow().getShell().setVisible(false);
//    	return false;
    	return true;
    }
}
