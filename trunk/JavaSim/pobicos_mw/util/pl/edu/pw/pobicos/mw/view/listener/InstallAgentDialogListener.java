package pl.edu.pw.pobicos.mw.view.listener;

import java.io.File;
import java.io.FileInputStream;

import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.swt.widgets.MenuItem;

import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * This class is a listener to pop-up menu attached to graphical representation of micro-agent to be installed on selected node.
 * It listens to selections that enable the user choosing one of available agents.
 * 
 * @author Marcin Smialek 
 * 
 * @created 2007-07-05 20:16:45
 *
 */
public class InstallAgentDialogListener extends SelectionAdapter {
	
	private String[] filterExtensions = { "*.poa" };
	AbstractNode currentNode;

	@Override
	public void widgetSelected(SelectionEvent e) {
		if (!(e.getSource() instanceof MenuItem))
			return;
		MenuItem menuItem = (MenuItem) e.getSource();
		currentNode = (AbstractNode)menuItem.getData();
		//okienko wybierania pliku
		FileDialog fileDialog = new FileDialog(Display.getCurrent().getActiveShell());
		fileDialog.setFilterExtensions(filterExtensions);
		final String fileName = fileDialog.open();
		Display.getDefault().asyncExec(new Runnable() {
			public void run() {
	            File plikAgenta = new File(fileName);
	    		String rootAgentName = plikAgenta.getName().substring(0, plikAgenta.getName().lastIndexOf('.'));
	    		try
	    		{
		    		FileInputStream fis = new FileInputStream(plikAgenta);
		    		byte[] kodAgenta=new byte[(int)plikAgenta.length()];
		    		fis.read(kodAgenta);
		    		PobicosManager.getInstance().installRootAgent(currentNode, rootAgentName, kodAgenta);
	    		}catch(Exception ex){}
			}
		});
		//JFileChooser fc = new JFileChooser();
		//fc.addChoosableFileFilter(new AgentFilter());
		//fc.setAcceptAllFileFilterUsed(false);
		//int returnVal = fc.showDialog(new java.awt.Canvas(), "Select an agent");
		/*if (returnVal == JFileChooser.APPROVE_OPTION) {
            File plikAgenta = fc.getSelectedFile();
    		String rootAgentName = plikAgenta.getName().substring(0, plikAgenta.getName().lastIndexOf('.'));
    		try
    		{
	    		FileInputStream fis = new FileInputStream(plikAgenta);
	    		byte[] kodAgenta=new byte[(int)plikAgenta.length()];
	    		fis.read(kodAgenta);
	    		RoversManager.getInstance().installRootAgent(currentNode, rootAgentName, kodAgenta);
    		}catch(Exception ex){}
        }*/
	}
}
