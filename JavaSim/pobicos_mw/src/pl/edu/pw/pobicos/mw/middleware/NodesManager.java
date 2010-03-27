package pl.edu.pw.pobicos.mw.middleware;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Vector;

import org.eclipse.core.runtime.ListenerList;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.GenericAgent;
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.logging.InfoData;
import pl.edu.pw.pobicos.mw.logging.Trace;
import pl.edu.pw.pobicos.mw.network.Client;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.GenericNode;
import pl.edu.pw.pobicos.mw.node.INodeListener;
import pl.edu.pw.pobicos.mw.node.NodesFactory;
import pl.edu.pw.pobicos.mw.node.NonGenericNode;
import pl.edu.pw.pobicos.mw.taxonomy.LocationTree;
import pl.edu.pw.pobicos.mw.taxonomy.ObjectClassQualifier;
import pl.edu.pw.pobicos.mw.taxonomy.ProductTree;

/**
 * This singleton class represents manager that handles all necessary operations
 * on nodes in the ROVERS system.
 * 
 * @author Tomasz Anuszewski
 * @created 2006-09-05 10:50:24
 */
public class NodesManager {

	private static NodesManager instance;

	// helps to create unique id for newly created nodes
	private static int idCounter = 1;

	// list of all nodes in the ROVERS system.
	private List<AbstractNode> nodesList = new ArrayList<AbstractNode>();

	// maps nodeId -> NodeModel
	private Map<Long, AbstractNode> nodesMap = new HashMap<Long, AbstractNode>();

	// maps node -> amount of agents working on the node
	private Map<AbstractNode, Integer> agentsOnNodesMap = new HashMap<AbstractNode, Integer>();

	// list of listeners added to objects listening to nodes changes
	private ListenerList listeners = new ListenerList();

	// private default constructor, private - prevents from being instantiated
	// outside this class
	private NodesManager() {
		// empty
	}

	/**
	 * Returns singleton instance of this class.
	 * 
	 * @return NodesModelManager singleton instance
	 */
	public static NodesManager getInstance() {
		if (instance == null) {
			instance = new NodesManager();
		}
		return instance;
	}

	/**
	 * Adds new node to the system and appends the specified node to the end of
	 * the list of all nodes.
	 * 
	 * @param node -
	 *            node to be added to the ROVERS system
	 */
	public void addNode(AbstractNode node) {
		if (!this.nodesMap.values().contains(node)) {
			this.nodesMap.put(node.getId(), node);
			this.nodesList.add(node);
			if(node.getClass().equals(NonGenericNode.class))
				Client.getInstance().newNode((NonGenericNode)node);
			fireNodesChanged(node);
			Trace.trace(Trace.NODEADD, node, null, new InfoData(node.toString()));
		}
	}

	/**
	 * Adds a new node to the system, node with default values.
	 * 
	 * @param x -
	 *            coordinate of the initial position
	 * @param y -
	 *            coordinate of the initial position
	 */
	public GenericNode createDefaultNode(int x, int y) 
	{
		if (x < 0 || y < 0) {
			return null;
		}
		GenericNode nodeModel = (GenericNode) NodesFactory.createNode();
		nodeModel.setX(x);
		nodeModel.setY(y);
		nodeModel.setId(findId());
		nodeModel.setName("node_" + nodeModel.getId());
		nodeModel.setMemory(7168);
		nodeModel.setRange(177);
		addNode(nodeModel);
		fireNodesChanged(nodeModel);
		return nodeModel;
	}

	/**
	 * Replaces one node with another one in the nodeList.
	 * 
	 * @param oldNode -
	 *            node to be removed
	 * @param newNode -
	 *            node that is set instead of the old node
	 */
	public void replaceNode(AbstractNode oldNode, AbstractNode newNode) {
		int index = -1;
		for (int i = 0; i < this.nodesList.size(); i++) {
			if (this.nodesList.get(i) != null && this.nodesList.get(i).equals(oldNode))
				index = i;
		}
		if (index < 0)
			return;
		removeNode(oldNode);
		addNode(newNode);
		Trace.trace(Trace.NODECHANGE, newNode, null, new InfoData(oldNode.toString()));
	}

	/**
	 * Removes given node from the ROVERS Network.
	 * 
	 * @param node -
	 *            node to be removed
	 */
	public void removeNode(AbstractNode node) {
		this.nodesMap.remove(node.getId());
		this.nodesList.remove(node);
		if(node.getClass().equals(NonGenericNode.class))
			Client.getInstance().oldNode(node);
		fireNodesChanged(node);
		Trace.trace(Trace.NODEREM, node, null, new InfoData(node.toString()));
	}

	public boolean isAgentInstallable(AbstractAgent agent, AbstractNode node) 
	{		
		if (PobicosManager.getInstance().isMemoryLimited()) {
			long currentNumAgents = 0;
			if (AgentsManager.getInstance().getAgentList(node) == null) {
			} else {
				for(AbstractAgent ag : AgentsManager.getInstance().getAgentList(node))
					currentNumAgents += ag.getSize(); 
			}

			if (!(currentNumAgents + agent.getSize() <= node.getMemory())) {
				//LOG.debug("No more micro-agents can be installed on " + node.getName());
				return false;
			}
		}
		boolean agentAllowed = true;
		if(AgentsManager.getInstance().getAgentList(node) != null)
			for(AbstractAgent oldAgent : AgentsManager.getInstance().getAgentList(node))
				if(oldAgent.getType() == agent.getType())
					return false;
		if(agent.isGeneric())
			return agentAllowed;
		for(int i = 0; i < agent.getNonGenericInstructions().size(); i++)
			if(!node.supportsInstruction(agent.getNonGenericInstructions().get(i)))
				return false;
		for(int i = 0; i < agent.getNonGenericEvents().keySet().toArray().length; i++)
			if(!node.raisesEvent(new Event((Long)agent.getNonGenericEvents().keySet().toArray()[i])))
				return false;
		return agentAllowed;
	}
	
	public boolean isNodeInRanges(AbstractNode node, ObjectClassQualifier objQual)
	{
		if(objQual.numberOfRanges == 0)
			return true;
		if(objQual.numberOfRanges == 1)
		{
			if(LocationTree.isInRange(node.getLocationId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code))
				return true;
			if(ProductTree.isInRange(node.getProductId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code))
				return true;
		}
		if(objQual.numberOfRanges == 2)
		{
			if(LocationTree.isInRange(node.getLocationId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) && ProductTree.isInRange(node.getProductId(), objQual.ranges[1].mostSpecific.code, objQual.ranges[1].mostAbstract.code))
				return true;
			if(ProductTree.isInRange(node.getProductId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) && LocationTree.isInRange(node.getLocationId(), objQual.ranges[1].mostSpecific.code, objQual.ranges[1].mostAbstract.code))
				return true;
		}
		return false;
	}
	
	public AbstractNode findHostNode(AbstractAgent agent, AbstractNode first) {
		//LOG.debug("Finding host for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(first) == null)
			return null;
		AbstractNode hostNode = null;
		for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(first)) {
			if (isAgentInstallable(agent, node)) {
				hostNode = node;
				break;
			}
		}
		return hostNode;
	}
	
	public AbstractNode findHostNode(AbstractAgent agent, AbstractNode first, ObjectClassQualifier objQual) {
		//LOG.debug("Finding host for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(first) == null)
			return null;
		AbstractNode hostNode = null;
		for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(first)) {
			if (isAgentInstallable(agent, node) && isNodeInRanges(node, objQual)) {
				hostNode = node;
				break;
			}
		}
		return hostNode;
	}
	
	public Vector<AbstractNode> findHostNodes(AbstractAgent agent, AbstractNode first, ObjectClassQualifier objQual) {
		Vector<AbstractNode> hosts = new Vector<AbstractNode>();
		//LOG.debug("Finding hosts for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(first) == null)
			return null;
		for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(first)) 
		{
			if (isNodeInRanges(node, objQual)) 
				if(!isAgentInstallable(agent, node))
					tryMigrate(node, agent.getSize(), hosts);
			if (isAgentInstallable(agent, node) && isNodeInRanges(node, objQual)) 
				hosts.add(node);
		}
		return hosts;
	}
	
	public AbstractNode findOtherHostNode(AbstractAgent agent, AbstractNode excluded, long size, Vector<AbstractNode> excludeds) {
		//LOG.debug("Finding other host for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(excluded) == null)
			return null;
		AbstractNode hostNode = null;
		for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(excluded)) {
			if (isAgentInstallable(agent, node) && !node.equals(excluded)) {
				if(!excludeds.contains(node))
				{
					hostNode = node;
					break;
				}
			}
		}
		return hostNode;
	}
	
	public AbstractNode findOtherHostNode(AbstractAgent agent, AbstractNode excluded) {
		//LOG.debug("Finding other host for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(excluded) == null)
			return null;
		AbstractNode hostNode = null;
		for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(excluded)) {
			if (isAgentInstallable(agent, node) && !node.equals(excluded)) {
				hostNode = node;
				break;
			}
		}
		return hostNode;
	}
	
	public AbstractNode findBestHostNode(AbstractAgent agent, AbstractNode first, ObjectClassQualifier objQual)
	{
		//LOG.debug("Finding perfect host for: '" + agent.getName() + "'");
		if(PobicosManager.getInstance().getSubNetworkNodes(first) == null)
			return null;
		int nofranges = objQual.numberOfRanges;
		if(nofranges == 0)
			for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(first)) {
				if (isAgentInstallable(agent, node))
					return node;
			}
		else
		{
			Map<AbstractNode, Integer> ranking = new HashMap<AbstractNode, Integer>();
			boolean isProduct = (ProductTree.getName(objQual.ranges[0].mostSpecific.code) == null ? false : true);
			for (AbstractNode node : PobicosManager.getInstance().getSubNetworkNodes(first)) 
			{
				if (isNodeInRanges(node, objQual)) 
					if(isAgentInstallable(agent, node) || (!isAgentInstallable(agent, node) && checkMigrate(node, agent.getSize())))
				//if (isNodeInRanges(node, objQual) && isAgentInstallable(agent, node)) 
				{
					if(nofranges == 1)
						if(isProduct)
							ranking.put(node, ProductTree.distance(node.getProductId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code));
						else
							ranking.put(node, LocationTree.distance(node.getLocationId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code));
					else
						if(isProduct)
							ranking.put(node, ProductTree.distance(objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) * ProductTree.distance(node.getProductId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) + LocationTree.distance(node.getLocationId(), objQual.ranges[1].mostSpecific.code, objQual.ranges[1].mostAbstract.code));
						else
							ranking.put(node, LocationTree.distance(objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) * LocationTree.distance(node.getLocationId(), objQual.ranges[0].mostSpecific.code, objQual.ranges[0].mostAbstract.code) + ProductTree.distance(node.getProductId(), objQual.ranges[1].mostSpecific.code, objQual.ranges[1].mostAbstract.code));
				}
			}
			int min = -1, index = -1;
	    	for(int i = 0; i < ranking.values().size(); i++)
	    		if(min == -1)
	    		{
	    			min = (Integer)ranking.values().toArray()[i];
	    			index = i;
	    		}
	    		else if((Integer)ranking.values().toArray()[i] < min && (Integer)ranking.values().toArray()[i] != -1)
	    		{
	    			min = (Integer)ranking.values().toArray()[i];
	    			index = i;
	    		}
	    	if(min >= 0)
	    	{
	    		if(!isAgentInstallable(agent, (AbstractNode)ranking.keySet().toArray()[index]))
					tryMigrate((AbstractNode)ranking.keySet().toArray()[index], agent.getSize());
	    		return (AbstractNode)ranking.keySet().toArray()[index];
	    	}
	    	return null;
		}
		return null;
	}
	
	private boolean checkMigrate(AbstractNode node, long spaceNeeded)
	{
		Vector<GenericAgent> agentsToMove = new Vector<GenericAgent>();
		int spaceUsed = 0;
		for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(node))
		{
			if(agent.isGeneric())
				agentsToMove.add((GenericAgent)agent);
			spaceUsed += agent.getSize();
		}
		while(node.getMemory() < spaceUsed + spaceNeeded)
		{
			long maxSize = 0;
			GenericAgent maxAgent = null;
			for(GenericAgent agent : agentsToMove)
				if(agent.getSize() > maxSize)
				{
					maxSize = agent.getSize();
					maxAgent = agent;
				}
			if(maxSize > 0)
			{
				agentsToMove.remove(maxAgent);
				AbstractNode destination = findOtherHostNode(maxAgent, node);
				if(destination != null)
				{
					spaceUsed -= maxAgent.getSize();
				}
			}
			else
				break;
		}
		if(node.getMemory() < spaceUsed + spaceNeeded)
			return false;
		return true;
	}
	
	private void tryMigrate(AbstractNode node, long spaceNeeded)
	{
		Vector<GenericAgent> agentsToMove = new Vector<GenericAgent>();
		int spaceUsed = 0;
		for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(node))
		{
			if(agent.isGeneric())
				agentsToMove.add((GenericAgent)agent);
			spaceUsed += agent.getSize();
		}
		while(node.getMemory() < spaceUsed + spaceNeeded)
		{
			long maxSize = 0;
			GenericAgent maxAgent = null;
			for(GenericAgent agent : agentsToMove)
				if(agent.getSize() > maxSize)
				{
					maxSize = agent.getSize();
					maxAgent = agent;
				}
			if(maxSize > 0)
			{
				agentsToMove.remove(maxAgent);
				AbstractNode destination = findOtherHostNode(maxAgent, node);
				if(destination != null)
				{
					AgentsManager.getInstance().replaceAgent(maxAgent, destination);
				}
			}
			else
				break;
		}
	}
	
	private void tryMigrate(AbstractNode node, long spaceNeeded, Vector<AbstractNode> hosts)
	{
		Vector<GenericAgent> agentsToMove = new Vector<GenericAgent>();
		int spaceUsed = 0;
		for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(node))
		{
			if(agent.isGeneric())
				agentsToMove.add((GenericAgent)agent);
			spaceUsed += agent.getSize();
		}
		while(node.getMemory() < spaceUsed + spaceNeeded)
		{
			long maxSize = 0;
			GenericAgent maxAgent = null;
			for(GenericAgent agent : agentsToMove)
				if(agent.getSize() > maxSize)
				{
					maxSize = agent.getSize();
					maxAgent = agent;
				}
			if(maxSize > 0)
			{
				agentsToMove.remove(maxAgent);
				AbstractNode destination = findOtherHostNode(maxAgent, node, spaceNeeded, hosts);
				if(destination != null)
				{
					AgentsManager.getInstance().replaceAgent(maxAgent, destination);
				}
			}
			else
				break;
		}
	}

	/**
	 * Updates given node state. This information is then sent to objects
	 * implementing appropriate listener to nodeChanged(AbstractNode node)
	 * events.
	 * 
	 * @param node -
	 *            reference to node that is supposed to be updated
	 */
	public void updateNode(AbstractNode node) {
		fireNodesChanged(node);
	}

	/**
	 * Updates node with values of temporary, auxiliary instance of GenericNode.
	 * 
	 * @param node -
	 *            node to be updated
	 * @param tempNode -
	 *            temporary node that contains new values to set
	 */
	public void updateNode(AbstractNode node, AbstractNode tempNode) {
		if (node == null) {
			throw new IllegalArgumentException("Node to update cannot be null.");
		}
		if (tempNode == null) {
			throw new IllegalArgumentException("Node with new values cannot be null.");
		}
		node.setName(tempNode.getName());
		node.setId(tempNode.getId());
		node.setX(tempNode.getX());
		node.setY(tempNode.getY());
		node.setRange(tempNode.getRange());
		node.setMemory(tempNode.getMemory());

		fireNodesChanged(node);
	}

	/**
	 * Changes type of a given node i.e. from generic to sensoric one (with
	 * smoke detector) or vice versa.
	 * 
	 * @param node -
	 *            reference to node that that is supposed to change its type
	 * @param newTypeName -
	 *            name of class - new type for the pointed node
	 */
	public void changeNodeType(AbstractNode node, File file, String path) {
		//System.out.println("Change type of node " + node.getClass().getSimpleName() + " into: " + path);
		Vector<AbstractAgent> toDelete = new Vector<AbstractAgent>();
		for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(node))
			toDelete.add(agent);
		for(AbstractAgent agent : toDelete)
			AgentsManager.getInstance().removeAgent(agent);
		AbstractNode newNode = (AbstractNode) NodesFactory.createNode(file, path);
		if (newNode == null)
			return;
		//newNode.setName(node.getName());
		newNode.setId(node.getId());
		newNode.setX(node.getX());
		newNode.setY(node.getY());
		newNode.setRange(node.getRange());
		newNode.setMemory(node.getMemory());
		newNode.setTimeout(node.getTimeout());

		replaceNode(node, newNode);
		Client.getInstance().changeNodeType((NonGenericNode)newNode);
	}

	public void changeNodeType(AbstractNode node, String xml) {
		AbstractNode newNode = (AbstractNode) NodesFactory.createNode(xml);
		if (newNode == null)
			return;
		//newNode.setName(node.getName());
		newNode.setId(node.getId());
		newNode.setX(node.getX());
		newNode.setY(node.getY());
		newNode.setRange(node.getRange());
		newNode.setMemory(node.getMemory());
		newNode.setTimeout(node.getTimeout());

		replaceNode(node, newNode);
		Client.getInstance().changeNodeType((NonGenericNode)newNode);
	}

	private long findId() {
		long id;
		boolean idExists;
		do {
			idExists = false;
			id = idCounter;
			for (AbstractNode nodeModel : this.nodesMap.values()) {
				if (nodeModel.getId() == id) {
					idExists = true;
					idCounter++;
					break;
				}
			}
		} while (idExists);
		return id;
	}

	/**
	 * Removes all nodes from the ROVERS Network and cleans-up all auxiliary
	 * data structures.
	 */
	public void clear() {
		for(AbstractNode node : nodesList)
			if(node.getClass().equals(NonGenericNode.class))
				Client.getInstance().oldNode(node);
		this.nodesMap.clear();
		this.nodesList.clear();
		this.agentsOnNodesMap.clear();
		fireNodesChanged(null);
	}

	/**
	 * Resets id counter. Method is supposed to be invoked when new
	 * configuration is to be loaded.
	 */
	public static void resetIdCounter() {
		NodesManager.idCounter = 1;
	}

	/**
	 * Returns GenericNode reference associated with a given ID.
	 * 
	 * @param nodeId -
	 *            unique node ID used to differentiate every node from others
	 * @return reference to the node with given id
	 */
	public AbstractNode getNode(long nodeId) {
		return this.nodesMap.get(nodeId);
	}

	/**
	 * Returns list of all nodes in the ROVERS system.
	 * 
	 * @return list of all nodes in the ROVERS system
	 */
	public List<AbstractNode> getNodes() {
		return this.nodesList;
	}

	/**
	 * Returns list of nodes that are within range of a given node.
	 * 
	 * @param node -
	 *            node for which neighbour nodes are to be found
	 * @return - list of nodes that are within range of a given node
	 */
	public List<AbstractNode> getNeighbours(AbstractNode node) {
		List<AbstractNode> neighbours = new ArrayList<AbstractNode>();

		for (AbstractNode remoteNode : this.nodesList) {
			if (isNodeReachable(node, remoteNode)) {
				neighbours.add(remoteNode);
			}
		}
		return neighbours;
	}
	
	public List<AbstractNode> getFullyReachableNodes(AbstractNode primaryNode) {
		List<AbstractNode> neighbours = new ArrayList<AbstractNode>();

		for (AbstractNode remoteNode : this.nodesList) {
			if (isNodeReachable(primaryNode, remoteNode) && isNodeReachable(remoteNode, primaryNode)) {
				neighbours.add(remoteNode);
			}
		}
		return neighbours;
	}

	/**
	 * Returns if given remote Node is in range of primary Node (considering
	 * primaryNode antenna)
	 * 
	 * @param primaryNode -
	 *            node posessing antenna of specified, limited range
	 * @param remoteNode -
	 *            node to be checked if it is in range of primary node antenna
	 * @return if given remote Node is in range of primary Node antenna
	 */
	public boolean isNodeReachable(AbstractNode primaryNode, AbstractNode remoteNode) {
		int x = primaryNode.getX();
		int y = primaryNode.getY();
		int range = primaryNode.getRange();
		double temp = ((x - remoteNode.getX()) * (x - remoteNode.getX()) + (y - remoteNode.getY())
				* (y - remoteNode.getY()));
		temp = Math.sqrt(temp);
		int distance = (new Double(temp)).intValue();
		if (distance <= range && distance >= 0) {
			return true;
		}
		return false;
	}

	/**
	 * Returns list with all nodes that have installed at least one micro-agent.
	 * 
	 * @return list with all nodes that have installed at least one micro-agent.
	 */
	public List<AbstractNode> getNodesWithAgents() {
		List<AbstractNode> nodesWithAgents = new ArrayList<AbstractNode>();
		for (AbstractNode node : this.nodesList) {
			if ((AgentsManager.getInstance().getAgentList(node) != null)
					&& (!AgentsManager.getInstance().getAgentList(node).isEmpty())) {
				nodesWithAgents.add(node);
			}
		}
		return nodesWithAgents;
	}

	/**
	 * Returns refrence to node given its id.
	 * 
	 * @param id -
	 *            id of node that is to be found
	 * @return - reference to node of the specified ID or null, if there is no
	 *         node pointed by such a id
	 */
	public AbstractNode findNode(long id) {
		return this.nodesMap.get(id);
	}

	public void updateAgentsList(AbstractNode node) {
		Integer oldAmount = this.agentsOnNodesMap.get(node);
		if (oldAmount != null) {
			this.agentsOnNodesMap.put(node, oldAmount + 1);
		} else {
			this.agentsOnNodesMap.put(node, 1);
		}
	}

	/**
	 * Adds listener to the listeners list. Each listeners is installed on
	 * object that listents to nodes changes.
	 * 
	 * @param listener -
	 *            listenter that is to be added to the listeners list
	 */
	public void addNodeListener(INodeListener listener) {
		this.listeners.add(listener);
	}

	/**
	 * Removes listener from listners list. Each listener that is supposed to be
	 * removed has been installed on object listening to changes in the nodes
	 * list.
	 * 
	 * @param listener -
	 *            listener to be removed from listners list
	 */
	public void removeNodeListener(INodeListener listener) {
		this.listeners.remove(listener);
	}

	private void fireNodesChanged(AbstractNode node) {
		Object[] ls = this.listeners.getListeners();
		for (int i = 0; i < ls.length; i++) {
			INodeListener nl = (INodeListener) ls[i];
			nl.nodeChanged(node);
		}
	}

}
