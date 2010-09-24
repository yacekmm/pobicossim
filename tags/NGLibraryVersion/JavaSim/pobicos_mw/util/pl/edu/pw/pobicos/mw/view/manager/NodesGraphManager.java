package pl.edu.pw.pobicos.mw.view.manager;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.ScrolledComposite;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.widgets.Canvas;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.ui.PlatformUI;

import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.INodeListener;
import pl.edu.pw.pobicos.mw.view.NetworkView;
import pl.edu.pw.pobicos.mw.view.graph.NodeGraph;
import pl.edu.pw.pobicos.mw.view.listener.AddNodeListener;
import pl.edu.pw.pobicos.mw.view.listener.NodeIconListener;
import pl.edu.pw.pobicos.mw.view.listener.RangeListener;

/**
 * Instance of this class allows manage of the nodes graphic elements.
 * 
 * @author Marcin Smialek
 * @created 2006-09-05 15:32:37
 */
public class NodesGraphManager {

	//private static final Log LOG = LogFactory.getLog(NodesGraphManager.class);

	private List<NodeGraph> nodeGraphList = new ArrayList<NodeGraph>();

	private NodeGraph currentNodeGraph;

	private static NodesGraphManager instance;

	private static final int canvasMargin = 50;

	private Canvas canvas;

	private float zoomFactor;

	private boolean showInfoChecked;

	private boolean nameVisible;

	private boolean rangeVisible;

	private boolean neighboursVisible;

	private boolean agentsVisible;

	private AddNodeListener addNodeListener;

	private Object lockingObject;

	private NodesManager nodesManager;

	private byte[] tempNodeDef = null;

	private NodesGraphManager(NodesManager nodesManager) {
		this.zoomFactor = 1.0F;
		this.nodesManager = nodesManager;
		lockingObject = this;

		nodesManager.addNodeListener(new INodeListener() {
			public void nodeChanged(AbstractNode node) {
				updateNodeGraphList(node);
				redrawCanvas();
			}
		});
	}
	
	private void redrawCanvas()
	{
		Display.getDefault().asyncExec(new Runnable(){
			public void run() {
				canvas.redraw();
			}					
		});
	}

	/**
	 * @param zoomFactor
	 */
	public void redraw(float zoomFactor) {
		this.zoomFactor = zoomFactor;
		setPreferredSize();
		redrawCanvas();
	}

	/**
	 * @return Preferred size
	 */
	public Point getPreferredSize() {
		int maxX = 0;
		int maxY = 0;
		for (AbstractNode nodeModel : nodesManager.getNodes()) {
			int tempX = nodeModel.getX() + nodeModel.getRange();
			if (tempX > maxX)
				maxX = tempX;
			int tempY = nodeModel.getY() + nodeModel.getRange();
			if (tempY > maxY)
				maxY = tempY;
		}
		maxX = new Float(maxX * zoomFactor).intValue() + canvasMargin;
		maxY = new Float(maxY * zoomFactor).intValue() + canvasMargin;

		NetworkView view = (NetworkView) PlatformUI.getWorkbench().getActiveWorkbenchWindow().getActivePage()
				.findView(NetworkView.ID);
		Point p = view.getParent().getSize();
		maxX = maxX > p.x ? maxX : p.x;
		maxY = maxY > p.y ? maxY : p.y;

		return new Point(maxX, maxY);
	}

	/**
	 * 
	 */
	public void setPreferredSize() {
		if (!(canvas.getParent() instanceof ScrolledComposite))
			return;
		canvas.setSize(getPreferredSize());
	}

	/**
	 * @return Node's canvas
	 */
	public Canvas getCanvas() {
		return canvas;
	}

	/**
	 * @return showInfoChecked flag value
	 */
	public boolean isShowInfoChecked() {
		return showInfoChecked;
	}

	/**
	 * @param checked
	 */
	public void setShowInfoChecked(boolean checked) {
		showInfoChecked = checked;
		if (canvas != null)
			redrawCanvas();
	}

	/**
	 * @return a flag that describes if circle of the node needs to be painted
	 */
	public boolean isRangeVisible() {
		return rangeVisible;
	}

	/**
	 * Sets a flag which indicates GUI to set or not node's range
	 * @param rangeVisible - requested flag value
	 */
	public void setRangeVisible(boolean rangeVisible) {
		this.rangeVisible = rangeVisible;
		if (canvas != null)
			redrawCanvas();
	}

	/**
	 * @return a flag that describes if name of the node needs to be painted
	 */
	public boolean isNameVisible() {
		return nameVisible;
	}

	/**
	 * @param nameVisible
	 */
	public void setNameVisible(boolean nameVisible) {
		this.nameVisible = nameVisible;
		if (canvas != null)
			redrawCanvas();
	}

	/**
	 * Returns if neighbours of the currently selected node are visible or not.
	 * 
	 * @return true - if neighbours of the currently selected node are visible, false - otherwise
	 */
	public boolean isNeighboursVisible() {
		return neighboursVisible;
	}

	/**
	 * Sets if neighbours of the currently selected node are supposed to be visible or not.
	 * 
	 * @param neighboursVisible - flag for visibility of the neighbours of the currently selected node
	 */
	public void setNeighboursVisible(boolean neighboursVisible) {
		this.neighboursVisible = neighboursVisible;
	}

	/**
	 * Returns true if info about installed agents is to be displayed on the Network view.
	 * 
	 * @return agentsVisible - flag for displaying info if at least one agent is installed on node
	 */
	public boolean isAgentsVisible() {
		return agentsVisible;
	}

	/**
	 * Sets if info about if at least one agent is installed on node
	 * 
	 * @param agentsVisible - flag for displaying info if at least one agent is installed on node
	 */
	public void setAgentsVisible(boolean agentsVisible) {
		this.agentsVisible = agentsVisible;
	}

	/**
	 * @return lock object
	 */
	public Object getLockingObject() {
		return lockingObject;
	}

	/**
	 * @return zooms factory
	 */
	public float getZoomFactor() {
		return zoomFactor;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @param checked
	 */
	public void setAddNodeChecked(boolean checked) {
		addNodeListener.setAddNodeChecked(checked);
	}

	public void setAddNodeChecked(boolean checked, byte[] nodeDef) {
		addNodeListener.setAddNodeChecked(checked);
		this.setTempNodeDef(nodeDef);
	}

	/**
	 * @param canvas
	 */
	public void setCanvas(Canvas canvas) {
		this.canvas = canvas;

		NodeIconListener iconListener = new NodeIconListener(this);
		canvas.addListener(SWT.MouseDown, iconListener);
		canvas.addListener(SWT.MouseUp, iconListener);
		canvas.addListener(SWT.MouseMove, iconListener);
		canvas.addListener(SWT.MouseHover, iconListener);
		canvas.addListener(SWT.MouseDoubleClick, iconListener);

		RangeListener rangeListener = new RangeListener(this);
		canvas.addListener(SWT.MouseDown, rangeListener);
		canvas.addListener(SWT.MouseUp, rangeListener);
		canvas.addListener(SWT.MouseMove, rangeListener);

		addNodeListener = new AddNodeListener(this);
		canvas.addListener(SWT.MouseEnter, addNodeListener);
		canvas.addListener(SWT.MouseExit, addNodeListener);
		canvas.addListener(SWT.FocusOut, addNodeListener);
		canvas.addListener(SWT.MouseDoubleClick, addNodeListener);
	}

	/**
	 * @param nodesManager
	 * @return NodesGraphManager instance
	 */
	public static NodesGraphManager getInstance(NodesManager nodesManager) {
		if (instance == null) {
			instance = new NodesGraphManager(nodesManager);
		}
		return instance;
	}

	/**
	 * Paints nodes graphical representation.
	 * 
	 * @param event
	 */
	public void drawNodes(Event event) {

		// range
		if (rangeVisible && showInfoChecked && currentNodeGraph != null) {
			currentNodeGraph.drawCircle(event, zoomFactor);
		}

		// currently selected node
		if (currentNodeGraph != null) {
			currentNodeGraph.checkAsCurrent(event, zoomFactor);
			if (neighboursVisible) {
				for (NodeGraph nodeGraph : getSubNetworkGraphNodes(currentNodeGraph)) {
					nodeGraph.checkAsSubNetwork(event, zoomFactor);
				}
				for (NodeGraph nodeGraph : getNeighbourGraphNodes(currentNodeGraph)) {
					nodeGraph.checkAsNeighbour(event, zoomFactor);
				}
			}
		}

		// show if there is an agent installed on a given node
		if (agentsVisible && showInfoChecked) {
			for (NodeGraph nodeGraph : getNodesWithAgents()) {
				nodeGraph.drawAgent(event, zoomFactor, AgentsManager.getInstance().getNumAgents(nodeGraph.getNode()));
			}
		}
		
		// nodeIcon
		for (NodeGraph nodeGraph : nodeGraphList) {
			nodeGraph.drawIcon(event, zoomFactor);
		}

		// node name
		if (nameVisible && showInfoChecked) {
			for (NodeGraph nodeGraph : nodeGraphList) {
				nodeGraph.drawName(event, zoomFactor);
			}
		}

	}

	private synchronized void updateNodeGraphList(AbstractNode tempNode) {
		boolean nodeFound = false;

		List<NodeGraph> obsoleteGraphNodes = new ArrayList<NodeGraph>();
		for (NodeGraph nodeGraph : nodeGraphList) {

			// check if new AbstractNode has just been added
			if (tempNode != null && nodeGraph.getNode().equals(tempNode))
				nodeFound = true;

			// find GraphNodes to be no longer needed
			if (!NodesManager.getInstance().getNodes().contains(nodeGraph.getNode())) {
				obsoleteGraphNodes.add(nodeGraph);
			}
		}
		// remove GraphNodes for removed AbstractNodes
		nodeGraphList.removeAll(obsoleteGraphNodes);

		// if AbstractNode not on list, create its graphical representation
		if (!nodeFound && tempNode != null) {
			nodeGraphList.add(new NodeGraph(tempNode));
		}

		// check if currentNodeGraph has been removed too
		if (!(nodeGraphList.contains(currentNodeGraph))) {
			currentNodeGraph = null;
		}
	}

	/**
	 * @return reference to list of all graphical representatives of AbstractNode objects.
	 */
	public List<NodeGraph> getNodeGraphList() {
		return nodeGraphList;
	}

	/**
	 * Sets pointed graphNode as the current one.
	 * 
	 * @param currentNodeGraph - reference to NodeGraph that is supposed to be set as the current one
	 */
	public void setCurrentNodeGraph(NodeGraph currentNodeGraph) {
		this.currentNodeGraph = currentNodeGraph;
		if (currentNodeGraph == null)
			redrawCanvas();
	}

	private List<NodeGraph> getNeighbourGraphNodes(NodeGraph nodeGraph) {
		List<NodeGraph> graphNeighbours = new ArrayList<NodeGraph>();
		List<AbstractNode> modelNeighbours = NodesManager.getInstance().getNeighbours(nodeGraph.getNode());

		for (AbstractNode node : modelNeighbours) {
			for (NodeGraph graphNode : nodeGraphList) {
				if (graphNode.getNode().equals(node)) {
					graphNeighbours.add(graphNode);
					break;
				}
			}
		}

		return graphNeighbours;
	}

	private List<NodeGraph> getSubNetworkGraphNodes(NodeGraph nodeGraph) {
		List<NodeGraph> graphNeighbours = new ArrayList<NodeGraph>();
		List<AbstractNode> modelNeighbours = new ArrayList<AbstractNode>();
		for(AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(nodeGraph.getNode()))
				modelNeighbours.add(node);

		for (AbstractNode node : modelNeighbours) {
			for (NodeGraph graphNode : nodeGraphList) {
				if (graphNode.getNode().equals(node)) {
					graphNeighbours.add(graphNode);
					break;
				}
			}
		}

		return graphNeighbours;
	}

	private List<NodeGraph> getNodesWithAgents() {
		List<NodeGraph> nodesWithAgents = new ArrayList<NodeGraph>();

		for (AbstractNode node : NodesManager.getInstance().getNodesWithAgents()) {
			for (NodeGraph graphNode : nodeGraphList) {
				if (graphNode.getNode().equals(node)) {
					nodesWithAgents.add(graphNode);
					break;
				}
			}
		}
		return nodesWithAgents;
	}

	
	/**
	 * @param lockingObject
	 */
	public synchronized void lock(Object lockingObject) {
		this.lockingObject = lockingObject;
	}

	/**
	 * Releases the lock.
	 */
	public synchronized void unlock() {
		lockingObject = null;
	}

	public void setTempNodeDef(byte[] tempNodeDef) {
		this.tempNodeDef = tempNodeDef;
	}

	public byte[] getTempNodeDef() {
		return tempNodeDef;
	}

}
