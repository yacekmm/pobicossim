package pl.edu.pw.pobicos.mw.view.listener;

import java.util.List;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.events.SelectionListener;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.MenuItem;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.taxonomy.LocationMap;
import pl.edu.pw.pobicos.mw.taxonomy.ProductMap;
import pl.edu.pw.pobicos.mw.view.graph.NodeGraph;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

/**
 * This listener is looking for the selected node (if any) and:
 * - redraws it (if necessary), 
 * - shows dialog with properties available on the selected node
 * - shows popup menu with available events on this node
 *  
 * @author Tomasz Anuszewski
 */
public class NodeIconListener implements Listener {

	//private Log LOG = LogFactory.getLog(NodeIconListener.class);

	private Point offset = null;

	private NodeGraph nodeGraph;

	private NodesGraphManager nodesGraphManager;

	private List<NodeGraph> nodeGraphList;

	private Rectangle iconBounds;
	
	private InstallAgentDialogListener agentsPopUpMenuListener = new InstallAgentDialogListener();
	
	private ChangeNodeDialogListener nodePopUpMenuListener = new ChangeNodeDialogListener();

	/**
	 * @param nodesGraphManager
	 */
	public NodeIconListener(NodesGraphManager nodesGraphManager) {
		this.nodesGraphManager = nodesGraphManager;
		nodeGraphList = nodesGraphManager.getNodeGraphList();
	}

	public void handleEvent(Event e) {

		if (e.type == SWT.MouseDown) {
			nodeGraph = findNodeGraph(e);
			if (nodeGraph != null) {
				nodesGraphManager.setCurrentNodeGraph(nodeGraph);
				nodesGraphManager.lock(this);
				iconBounds = nodeGraph.getIconBounds();
				offset = new Point(e.x - iconBounds.x, e.y - iconBounds.y);
			}
		} else if (e.type == SWT.MouseMove && (e.stateMask & SWT.BUTTON1) != 0) {
			if (offset != null) {
				nodeGraph.getNode().setX(
						new Float((e.x - offset.x) * 1.0F / nodesGraphManager.getZoomFactor()).intValue());
				nodeGraph.getNode().setY(
						new Float((e.y - offset.y) * 1.0F / nodesGraphManager.getZoomFactor()).intValue());
				nodesGraphManager.setPreferredSize();
			}
			if (nodesGraphManager.getCanvas().getMenu() != null) {
				NodeGraph tempNodeGraph = findNodeGraph(e);
				if (tempNodeGraph == null) {
					nodesGraphManager.getCanvas().getMenu().dispose();
				}
			}
			nodesGraphManager.getCanvas().redraw();
		} else if (e.type == SWT.MouseUp) {
			if (findNodeGraph(e) == null && nodesGraphManager.getCanvas().getMenu() != null) {
				nodesGraphManager.getCanvas().getMenu().dispose();
			}
			if (offset != null) {
				if(nodeGraph.getNode() != null)
					NodesManager.getInstance().updateNode(nodeGraph.getNode());
				else
					nodesGraphManager.setCurrentNodeGraph(null);
				if ((e.stateMask & SWT.BUTTON3) != 0) {
					showEventsMenu(nodeGraph);
				}
			}
			else
				nodesGraphManager.setCurrentNodeGraph(null);
			offset = null;
			nodesGraphManager.unlock();
			nodeGraph = null;
		} else if (e.type == SWT.MouseHover) {
			NodeGraph tempNodeGraph = findNodeGraph(e);
			if (tempNodeGraph != null) {
				nodesGraphManager.getCanvas().setToolTipText(
						" type  : " + tempNodeGraph.getNode().getClass().getSimpleName() + "\n name : "
								+ tempNodeGraph.getNode().getName() + "\n [x,y] : ["
								+ tempNodeGraph.getNode().getX() + ", " + tempNodeGraph.getNode().getY()
								+ "]\n range: " + tempNodeGraph.getNode().getRange());
			} else {
				nodesGraphManager.getCanvas().setToolTipText(null);
			}
		} /*else if (e.type == SWT.MouseDoubleClick) {
			offset = null;
			NodeGraph tempNodeGraph = findNodeGraph(e);
			if (tempNodeGraph == null || Display.getDefault().getActiveShell() == null) {
				nodesGraphManager.setCurrentNodeGraph(null);
				return;
			}
			EditNodeDialog d = new EditNodeDialog(Display.getDefault().getActiveShell(), tempNodeGraph.getNode());
			int code = d.open();
			if (code == Window.OK) {
				NodesManager.getInstance().updateNode(tempNodeGraph.getNode(), d.getNodeModel());
			}
		}*/
	}

	/**
	 * Shows popup menu with the list of available events on the selected node 
	 * and on agents installed on this node.
	 */
	private void showEventsMenu(final NodeGraph nodeGraph) {
		if (nodesGraphManager.getCanvas().getMenu() != null) {
			nodesGraphManager.getCanvas().getMenu().dispose();
		}
		
		Menu menu = new Menu(Display.getDefault().getActiveShell(), SWT.POP_UP);	

		MenuItem menuItem = new MenuItem(menu, SWT.TITLE);
		menuItem.setEnabled(false);
		menuItem.setText(ProductMap.getName(nodeGraph.getNode().getProductId()) + " @ " + LocationMap.getName(nodeGraph.getNode().getLocationId()));

		new MenuItem(menu, SWT.SEPARATOR);
		
		/*menuItem = new MenuItem(menu, SWT.PUSH);
		menuItem.setText("Change properties...");
		menuItem.addSelectionListener(new SelectionListener(){

			public void widgetDefaultSelected(SelectionEvent e) {
			}

			public void widgetSelected(SelectionEvent e) {
				EditNodeDialog d = new EditNodeDialog(Display.getDefault().getActiveShell(), nodeGraph.getNode());
				int code = d.open();
				if (code == Window.OK) {
					NodesManager.getInstance().updateNode(nodeGraph.getNode(), d.getNodeModel());
				}
			}
			
		});*/
		
				
		menuItem = new MenuItem(menu, SWT.PUSH);
		menuItem.setText("Change Node Definition...");
		menuItem.setData(nodeGraph.getNode());
		menuItem.addSelectionListener(nodePopUpMenuListener);
		
		
		menuItem = new MenuItem(menu, SWT.PUSH);
		menuItem.setText("Insert Application Bundle...");
		menuItem.setData(nodeGraph.getNode());
		menuItem.addSelectionListener(agentsPopUpMenuListener);

		new MenuItem(menu, SWT.SEPARATOR);

		menuItem = new MenuItem(menu, SWT.PUSH);
		menuItem.setText("Delete node");
		menuItem.addSelectionListener(new SelectionListener(){

			public void widgetDefaultSelected(SelectionEvent e) {
			}

			public void widgetSelected(SelectionEvent e) {
				NodesManager.getInstance().removeNode(nodeGraph.getNode());
			}
			
		});
		
		nodesGraphManager.getCanvas().setMenu(menu);
		menu.setVisible(true);
	}

	/**
	 * Checks if given point belongs to area of any node. Node icon defines this area.
	 * 
	 * @param e - mouse event
	 * @return nodeGraph if current coordinates belong to nodeGraph area or null otherwise 
	 */
	private NodeGraph findNodeGraph(Event e) {
		for (NodeGraph nodeGraph : nodeGraphList) {
			if (nodeGraph.getIconBounds().contains(e.x, e.y)) {
				return nodeGraph;
			}
		}
		return null;
	}
}
