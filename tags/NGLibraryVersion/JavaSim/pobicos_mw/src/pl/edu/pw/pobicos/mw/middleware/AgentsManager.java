/**
 * 
 */
package pl.edu.pw.pobicos.mw.middleware;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.eclipse.core.runtime.ListenerList;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.AgentsFactory;
import pl.edu.pw.pobicos.mw.agent.ApplicationBundle;
import pl.edu.pw.pobicos.mw.agent.GenericAgent;
import pl.edu.pw.pobicos.mw.agent.IAgentListener;
import pl.edu.pw.pobicos.mw.logging.InfoData;
import pl.edu.pw.pobicos.mw.logging.Trace;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.INodeListener;

/**
 * This singleton class is supposed to manage hierarchy of agents.
 */
public class AgentsManager {

	private static AgentsManager instance;

	// helps to create unique id for newly created agent
	private static int idCounter = 1;

	// Maps agentId -> agent
	private LinkedHashMap<Long, AbstractAgent> agentsMap = new LinkedHashMap<Long, AbstractAgent>();
	
	// Maps agent to its node
	private Map<AbstractAgent, AbstractNode> agentToNodeMap = new HashMap<AbstractAgent, AbstractNode>();
	
	// Maps agent to its boss
	private Map<AbstractAgent, AbstractAgent> agentToBossMap = new HashMap<AbstractAgent, AbstractAgent>();
	
	// Maps root agent to application bundle
	private Map<AbstractAgent, ApplicationBundle> rootToBundleMap = new HashMap<AbstractAgent, ApplicationBundle>();
	
	// Maps node -> agents List
	private Map<AbstractNode, List<AbstractAgent>> nodeToAgentsMap = new HashMap<AbstractNode, List<AbstractAgent>>();

	// Maps agent -> employees
	private Map<AbstractAgent, List<AbstractAgent>> agentToEmployeesMap = new HashMap<AbstractAgent, List<AbstractAgent>>();

	// list of listeners added to objects listening to agents' changes
	private ListenerList listeners = new ListenerList();

	// private default constructor; private - prevents from being instantiated
	// beyond this class
	private AgentsManager() {
		NodesManager.getInstance().addNodeListener(new INodeListener() {

			public void nodeChanged(AbstractNode node) {
				verifyAgents();
				//TODO: continuous migration
			}
		});
	}

	/**
	 * Returns singleton instance of this class.
	 * 
	 * @return AgentsManager singleton instance
	 */
	public static AgentsManager getInstance() {
		if (instance == null) {
			instance = new AgentsManager();
		}
		return instance;
	}

	/**
	 * Registers given agent on a given node.
	 * 
	 * @param agent -
	 *            agent to be registered / installed
	 * @param node -
	 *            host node for that agent
	 */
	public AbstractAgent registerAgent(AbstractAgent agent, AbstractNode node, AbstractAgent boss) {
		if (agent == null) {
			throw new IllegalArgumentException("Agent to register cannot be null.");
		}
		if (node == null) {
			throw new IllegalArgumentException("Host node cannot be null.");
		}
		
		//if (!this.agentsMap.values().contains(agent)) {
			for(AbstractAgent ag : AgentsManager.getInstance().getAgents())
				if(ag.getId() == agent.getId())
					agent = AgentsFactory.createAgent(getNode(agent), null, getBundle(getRoot(agent)).getPossibleEmployee(agent.getType()));
			List<AbstractAgent> tempAgentsList = this.nodeToAgentsMap.get(node);
			if (tempAgentsList == null) {
				tempAgentsList = new ArrayList<AbstractAgent>();
				tempAgentsList.add(agent);
				this.nodeToAgentsMap.put(node, tempAgentsList);
			} else {
				tempAgentsList.add(agent);
			}
			this.agentsMap.put(agent.getId(), agent);
			this.agentToNodeMap.put(agent, node);
			this.agentToEmployeesMap.put(agent, new ArrayList<AbstractAgent>());
			this.agentToBossMap.put(agent, boss);
			if(boss != null)
			{
				List<AbstractAgent> employees = agentToEmployeesMap.get(boss);
				agentToEmployeesMap.remove(boss);
				employees.add(agent);
				agentToEmployeesMap.put(boss, employees);
			}
			if (agent.getName() == null) 
			{
				AbstractAgent root = getRoot(agent);
				agent.setName(root.getName() + "_" + agent.getId() + "_" + (agent.isGeneric() ? "G" : "NG"));
			}
			NodesManager.getInstance().updateAgentsList(node);
			fireAgentsChanged(agent);
			Trace.trace(Trace.AGENTREG, node, agent, new InfoData(agent.toString()));
			return agent;
		/*} else {
			LOG.warn("Agent already installed in the Network.");
		}*/
	}

	public void replaceAgent(GenericAgent agent, AbstractNode node) 
	{
		this.agentsMap.remove(agent.getType());
		List<AbstractAgent> agents = this.nodeToAgentsMap.get(getNode(agent));
		agents.remove(agent);
		this.nodeToAgentsMap.remove(agent.getType());
		if (agent == null) {
			throw new IllegalArgumentException("Agent to register cannot be null.");
		}
		if (node == null) {
			throw new IllegalArgumentException("Host node cannot be null.");
		}
		
			List<AbstractAgent> tempAgentsList = this.nodeToAgentsMap.get(node);
			if (tempAgentsList == null) {
				tempAgentsList = new ArrayList<AbstractAgent>();
				tempAgentsList.add(agent);
				this.nodeToAgentsMap.put(node, tempAgentsList);
			} else {
				tempAgentsList.add(agent);
			}
			this.agentsMap.put(agent.getId(), agent);
			NodesManager.getInstance().updateAgentsList(node);
			fireAgentsChanged(agent);
			Trace.trace(Trace.AGENTREP, node, agent, new InfoData(agent.toString()));
	}

	/**
	 * Removes given micro-agent from the system.
	 * 
	 * @param agent -
	 *            micro-agent that is supposed to be removed
	 */
	public void removeAgent(AbstractAgent agent) {
		this.agentsMap.remove(agent.getType());
		List<AbstractAgent> agents = this.nodeToAgentsMap.get(getNode(agent));
		agents.remove(agent);
		this.nodeToAgentsMap.remove(agent.getType());
		fireAgentsChanged(agent);
		PobicosManager.getInstance().addEvent("Finalize", null, agent, null);
		Trace.trace(Trace.AGENTREM, AgentsManager.getInstance().getNode(agent), agent, new InfoData(agent.toString()));
	}

	/**
	 * Removes agents from the ROVERS Network and cleans-up all auxiliary data
	 * structures.
	 */
	public void clear() 
	{
		this.agentsMap.clear();
		this.agentsMap.clear();
		this.nodeToAgentsMap.clear();
		fireAgentsChanged(null);
	}

	private void verifyAgents() 
	{
		Collection<AbstractAgent> tempAgents = this.agentsMap.values();
		List<AbstractAgent> agentsToRemove = new ArrayList<AbstractAgent>();
		for (Iterator<AbstractAgent> iter = tempAgents.iterator(); iter.hasNext();) {
			AbstractAgent agent = iter.next();
			if (!NodesManager.getInstance().getNodes().contains((getNode(agent)))) {
				agentsToRemove.add(agent);
			}
		}
		for (AbstractAgent agent : agentsToRemove)
			removeAgent(agent);
	}

	/*
	 * Creates unique id for newly created agents
	 */
	public long findId() {
		long id;
		boolean idExists;
		do {
			idExists = false;
			id = idCounter;
			for (AbstractAgent agent : this.agentsMap.values()) {
				if (agent.getId() == id) {
					idExists = true;
					idCounter++;
					break;
				}
			}
		} while (idExists);
		return id;
	}

	public ApplicationBundle getBundle(AbstractAgent root) 
	{
		return rootToBundleMap.get(root);
	}

	public AbstractAgent getRoot(AbstractAgent agent) 
	{
		if(getBoss(agent) == null)
			return agent;
		AbstractAgent tempBoss = getBoss(agent);
		while(getBoss(tempBoss) != null)
			tempBoss = getBoss(tempBoss);
		return tempBoss;
	}

	/**
	 * Returns list of employees for a given micro-agent.
	 * 
	 * @param agent -
	 *            boss micro-agent reference
	 * @return list of micro-agent employees
	 */
	public List<AbstractAgent> getEmployees(AbstractAgent agent) {
		return agentToEmployeesMap.get(agent);
	}

	/**
	 * Returns the boss micro-agent for a given micro-agent.
	 * 
	 * @param agent -
	 *            micro-agent for which the boss-agent is to be found
	 * @return boss micro-agent reference or null if given micro-agent is a
	 *         root-agent
	 */
	public AbstractAgent getBoss(AbstractAgent agent) {
		return agentToBossMap.get(agent);
	}

	/**
	 * Returns AbstractAgent instance associated with a given ID.
	 * 
	 * @param agentId -
	 *            unique micro-agent ID used to differentiate every agent from
	 *            others
	 * @return reference to the micro-agent with given id
	 */
	public AbstractAgent getAgent(long agentId) {
		return this.agentsMap.get(agentId);
	}

	/**
	 * Returns list of all agents installed in the ROVERS Network.
	 * 
	 * @return list of all agents installed in the ROVERS system
	 */
	public List<AbstractAgent> getAgents() {
		List<AbstractAgent> resultList = new ArrayList<AbstractAgent>();
		resultList.addAll(this.agentsMap.values());
		return resultList;
	}

	/**
	 * Returns list of all root-agents (such as that their bosses point to null)
	 * in the ROVERS system.
	 * 
	 * @return list of the root agents in the ROVERS Network
	 */
	public List<AbstractAgent> getRootAgents() {
		List<AbstractAgent> rootAgents = new ArrayList<AbstractAgent>(0);
		for (AbstractAgent agentModel : this.agentsMap.values()) {
			if (getBoss(agentModel) == null)
				rootAgents.add(agentModel);
		}
		return rootAgents;
	}

	/**
	 * Returns agent list installed on a given node.
	 * 
	 * @param nodeId -
	 *            node ID
	 * @return list of agents installed on a given node
	 */
	public List<AbstractAgent> getAgentList(AbstractNode node) {
		if(this.nodeToAgentsMap.get(node) == null)
			nodeToAgentsMap.put(node, new ArrayList<AbstractAgent>());
		return nodeToAgentsMap.get(node);
	}

	/**
	 * Returns number of agents installed on a given node.
	 * 
	 * @param nodeId -
	 *            node id
	 * @return number of agents installed on a given node
	 */
	public int getNumAgents(AbstractNode node) {
		return (getAgentList(node) == null || getAgentList(node).isEmpty()) ? 0 : getAgentList(node).size();
	}
	
	public AbstractNode getNode(AbstractAgent agent)
	{
		return agentToNodeMap.get(agent);
	}

	/**
	 * Updates agent state. This information is then sent to objects
	 * implementing appropriate listener to agentChanged(AbstractAgent agent)
	 * events.
	 * 
	 * @param agent -
	 *            ID of the micro-agent that is supposed to be updated
	 */
	public void updateAgent(AbstractAgent agent) {
		fireAgentsChanged(agent);
	}

	/**
	 * Adds listener to the listeners list. Each listeners is installed on
	 * object that listents to micro-agents changes.
	 * 
	 * @param listener -
	 *            listenter that is to be added to the listeners list
	 */
	public void addAgentListener(IAgentListener listener) {
		this.listeners.add(listener);
	}

	/**
	 * Removes listener from listners list. Each listener that is supposed to be
	 * removed has been installed on object listening to changes in the agent
	 * list.
	 * 
	 * @param listener -
	 *            listener to be removed from listners list
	 */
	public void removeAgentListener(IAgentListener listener) {
		this.listeners.remove(listener);
	}

	private void fireAgentsChanged(AbstractAgent agent) {
		Object[] ls = this.listeners.getListeners();
		for (int i = 0; i < ls.length; i++) {
			IAgentListener al = (IAgentListener) ls[i];
			al.agentChanged(agent);
		}
	}

	public boolean isRootAgent(AbstractAgent agent) 
	{
		if(getRoot(agent).equals(agent))
			return true;
		return false;
	}

	public void registerBundle(ApplicationBundle bundle) 
	{
		this.rootToBundleMap.put(bundle.getRootAgent(), bundle);
	}
}
