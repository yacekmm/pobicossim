package pl.edu.pw.pobicos.mw.middleware;

import java.io.FileInputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.Vector;

import org.apache.log4j.*;
import org.eclipse.core.runtime.ListenerList;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.AgentsFactory;
import pl.edu.pw.pobicos.mw.agent.ApplicationBundle;
import pl.edu.pw.pobicos.mw.agent.NonGenericAgent;
import pl.edu.pw.pobicos.mw.event.Context;
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.event.EventMap;
import pl.edu.pw.pobicos.mw.event.EventTree;
import pl.edu.pw.pobicos.mw.event.PhysicalEvent;
import pl.edu.pw.pobicos.mw.logging.InfoData;
import pl.edu.pw.pobicos.mw.logging.Trace;
import pl.edu.pw.pobicos.mw.message.AbstractMessage;
import pl.edu.pw.pobicos.mw.message.Command;
import pl.edu.pw.pobicos.mw.message.IMessageListener;
import pl.edu.pw.pobicos.mw.message.Report;
import pl.edu.pw.pobicos.mw.message.ReportList;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.INodeListener;
import pl.edu.pw.pobicos.mw.node.NodesFactory;
import pl.edu.pw.pobicos.mw.port.ApplicationElement;
import pl.edu.pw.pobicos.mw.port.Export;
import pl.edu.pw.pobicos.mw.port.Import;
import pl.edu.pw.pobicos.mw.port.NetworkElement;
import pl.edu.pw.pobicos.mw.port.NodeElement;
import pl.edu.pw.pobicos.mw.resource.AbstractResource;
import pl.edu.pw.pobicos.mw.taxonomy.ObjectClassQualifier;
import pl.edu.pw.pobicos.mw.view.action.ActionContainer;
import pl.edu.pw.pobicos.mw.view.action.SaveSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.StartSimulationAction;
import pl.edu.pw.pobicos.mw.view.action.StepForwardAction;

/**
 * Singleton class representing POBICOS system manager. It defines and controls
 * all functions of the middleware such as event-handling, messages exchange
 * etc. Furthermore, this class is responsible for keeping references to all
 * model data initialized from a config file i.e. lists of all nodes,
 * micro-agents in the system.
 */
public class PobicosManager {
	
	private final Logger LOG = Logger.getLogger(this.getClass());
	
	private static PobicosManager instance;
	
	private ListenerList listeners = new ListenerList();

	private String pobicosName = "";

	private Random prob = new Random();

	private boolean memoryLimited = true;

	private boolean rangeLimited = true;

	private Vector<List<AbstractNode>> subnets = new Vector<List<AbstractNode>>();

	private Vector<WaitingAgent> waitingAgents = new Vector<WaitingAgent>();

	private long networkId = -1;

	private int reliability = 77;

	private Map<Command, MessageStatus> commands = new HashMap<Command, MessageStatus>();

	private Map<Report, MessageStatus> reports = new HashMap<Report, MessageStatus>();
	
	private Map<Integer, Event> timers = new HashMap<Integer, Event>();
	
	private PobicosManager() {
		NodesManager.getInstance().addNodeListener(new INodeListener() {

			public void nodeChanged(
			AbstractNode node) {
				updateSubNetworks();
				checkReachability();
				checkWaitingAgents();
			}
		});
	}

	/**
	 * Returns singleton instance of this class.
	 * 
	 * @return RoversManager singleton instance
	 */
	public static PobicosManager getInstance() {
		if (instance == null) {
			instance = new PobicosManager();
		}
		return instance;
	}
	
	private void checkWaitingAgents() 
	{
		Vector<WaitingAgent> toDelete = new Vector<WaitingAgent>();
		for(WaitingAgent agent : waitingAgents)
		{
			if(agent.time <= SimulationsManager.getInstance().getSimulation().getVirtualTime() && agent.timeout != 0)
				toDelete.add(agent);
			else if(agent.agent.isGeneric())
			{
				installAgent(agent.agent.getType(), agent.boss, -1, agent.request);
				toDelete.add(agent);
			}
			else if(agent.many)
				installNonGenericAgents(agent.agent.getType(), agent.boss, -1, agent.request, agent.objQual);
			else
			{
				installNonGenericAgent(agent.agent.getType(), agent.boss, -1, agent.request, agent.objQual);
				toDelete.add(agent);
			}
		}
	}
	
	private void updateSubNetworks()
	{
		subnets.clear();
		Vector<AbstractNode> nodesUsed = new Vector<AbstractNode>();
		for(AbstractNode node : NodesManager.getInstance().getNodes())
			if(!nodesUsed.contains(node))
			{
				List<AbstractNode> newList = new ArrayList<AbstractNode>();
				subnets.add(newList);
				for(AbstractNode near : NodesManager.getInstance().getFullyReachableNodes(node))
					if(!nodesUsed.contains(near))
						spreadSubNetwork(near, newList, nodesUsed);
			}
	}
	
	private void spreadSubNetwork(AbstractNode node, List<AbstractNode> newList, Vector<AbstractNode> nodesUsed)
	{
		newList.add(node);
		nodesUsed.add(node);
		for(AbstractNode near : NodesManager.getInstance().getFullyReachableNodes(node))
			if(!nodesUsed.contains(near))
				spreadSubNetwork(near, newList, nodesUsed);
	}
	
	public List<AbstractNode> getSubNetworkNodes(AbstractNode node)
	{
		for(List<AbstractNode> subnet : subnets)
			if(subnet.contains(node))
				return subnet;
		return null;
	}
	
	public boolean doNodesShareSubNetwork(AbstractNode first, AbstractNode second)
	{
		for(List<AbstractNode> subnet : subnets)
			if(subnet.contains(first))
			{
				if(subnet.contains(second))
					return true;
				return false;
			}
		return false;
	}
	
	public void checkReachability()
	{
		for(AbstractAgent agent : AgentsManager.getInstance().getAgents())
			if(AgentsManager.getInstance().getBoss(agent) != null)
				if(!doNodesShareSubNetwork(AgentsManager.getInstance().getNode(agent), AgentsManager.getInstance().getNode(AgentsManager.getInstance().getBoss(agent))))
				{
					releaseAgent(agent);
					Vector<Context> contexts = new Vector<Context>();
					contexts.add(new Context(agent.getId(), agent.getType(), -1));
					addEvent("ChildUnreachable", null, AgentsManager.getInstance().getBoss(agent), null);
					Vector<Event> toDelete = new Vector<Event>();
					for(Event event : SimulationsManager.getInstance().getSimulation().getEventList())
						if(event.getAgent() != null)
							if(event.getAgent().equals(agent))
								toDelete.add(event);
					for(Event event : toDelete)
						SimulationsManager.getInstance().getSimulation().removeEvent(event);
				}
	}

	private void releaseAgent(AbstractAgent agent) 
	{
		//releases instantly all children
		for(AbstractAgent a : AgentsManager.getInstance().getEmployees(agent))
			releaseAgent(a);
		AgentsManager.getInstance().removeAgent(agent);
	}

	/**
	 * Initializes RoversManager with data from the XML configuration file
	 * defined by the argument.
	 * 
	 * @param is -
	 *            input stream pointing to configuration file
	 */
	public void loadConfig(FileInputStream fis, String path) {
		if (fis != null) {
			try {
				NetworkElement net = Import.importNetwork(fis, path);
				if (net != null) {
					this.pobicosName = net.getName();
					this.networkId = net.getId();

					clear();
					
					for(NodeElement node : net.getNodes())
					{
						AbstractNode newNode = NodesFactory.createNode(node);
						newNode.setId(node.getId());
						newNode.setName(node.getName());
						newNode.setX(node.getX());
						newNode.setY(node.getY());
						newNode.setMemory(node.getMemory());
						newNode.setRange(node.getRange());
						//LOG.debug("New node created: " + newNode.getName());
						NodesManager.getInstance().addNode(newNode);
					}
					
					for(ApplicationElement app : net.getApplications())
					{
						PobicosManager.getInstance().installRootAgent(NodesManager.getInstance().getNode(app.getNodeId()), app.getName(), app.getAppBundle());
					}
				}
			} catch (Exception e) {
				//LOG.error("Cannot initialize Rovers Manager. See log file for more details", e);
			}
		}
	}

	/**
	 * Saves the current simulation state in the stream defined by the argument.
	 * 
	 * @param roversName - TODO
	 *            name of the ROVERS network.
	 * @param os -
	 *            defines where to store the simulation data
	 * @return status of the save operation
	 */
	public boolean saveConfig(String pobicosName, String path) {
		// save options

		NetworkElement net = new NetworkElement(pobicosName, networkId);
		
		for(AbstractNode node : NodesManager.getInstance().getNodes())
			net.addNode(new NodeElement(node.getName(), (int)node.getId(), node.getX(), node.getY(), node.getMemory(), node.getRange(), node.getNodeDef()));
		
		for(AbstractAgent agent : AgentsManager.getInstance().getAgents())
			if(AgentsManager.getInstance().isRootAgent(agent))
				net.addApplication(new ApplicationElement(agent.getName(), (int)AgentsManager.getInstance().getNode(agent).getId(), AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(agent)).getByteCode()));

		try {
			Export.exportNetwork(net, path);
			//LOG.debug("Pobicos config saved ...");
			return true;
		} catch (Exception e) {
			//LOG.error("Cannot save Pobicos simulation.", e);
			return false;
		}
	}

	/**
	 * Clears ROVERS manager from current configuration data i.e. present list
	 * of nodes, micro-agents etc. It ensures that system is free of being
	 * polluted by remainings of previous configuration data.
	 */
	public void clear() {
		waitingAgents.clear();
		reports.clear();
		commands.clear();
		AgentsManager.getInstance().clear();
		NodesManager.getInstance().clear();
	}

	/**
	 * @param node
	 * @param rootAgentTypeName
	 */
	public void installRootAgent(AbstractNode node, String rootAgentName, byte[] uA) {
		//AbstractAgent agent = AgentsFactory.createAgent(node.getId().toString(),rootAgentName,"-1",uA);
		LOG.debug("installRootAgent");
		LOG.error("installRootAgent");
		ApplicationBundle bundle = AgentsFactory.importBundle(node,rootAgentName,uA);
		AgentsManager.getInstance().registerBundle(bundle);
		AbstractAgent agent = bundle.getRootAgent();
		if (agent == null)
			return;
		if(AgentsManager.getInstance().getRootAgents() != null)
			for(AbstractAgent oldAgent : AgentsManager.getInstance().getRootAgents())
				if(oldAgent.getType() == agent.getType())
					//if(!oldAgent.getNode().equals(node))
						return;
		if (!NodesManager.getInstance().isAgentInstallable(agent, node)) {
			node =  NodesManager.getInstance().findHostNode(agent, node);
			if(node == null)
				return;
		}
		//AgentsManager.getInstance().setDefaultValues(agent);
		AgentsManager.getInstance().registerAgent(agent, node, null);
		addEvent("Init", null, agent, null);
	}

	public boolean installAgent(long agentType, AbstractAgent boss, int timeout, long request) {
		//AbstractAgent agent = AgentsFactory.createAgent(node.getId().toString(),rootAgentName,"-1",uA);
		if(timeout > 0) 
		{
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(-1, agentType, request));
			addEvent("ChildCreationTimeout", null, boss, null, timeout).addContexts(contexts);
		}
		AbstractNode node = AgentsManager.getInstance().getNode(boss);
		AbstractAgent agent = AgentsFactory.createAgent(node, null, AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(boss)).getPossibleEmployee(agentType));
		AbstractAgent root = AgentsManager.getInstance().getRoot(boss);
		agent.setName(root.getName() + "_" + agent.getId() + "_" + (agent.isGeneric() ? "G" : "NG"));
		if (!NodesManager.getInstance().isAgentInstallable(agent, node)) {
			node =  NodesManager.getInstance().findHostNode(agent, node);
			if(node == null)
			{//TODO: start timer to sent childnotcreated event
				LOG.debug("no proper node found...");
				if(timeout >= 0)
					addToWaitingAgents(agent, request, timeout, false, boss, null);
				agent = null;
				return false;
			}
		}
		AgentsManager.getInstance().registerAgent(agent, node, boss);
		Vector<Context> contexts = new Vector<Context>();
		contexts.add(new Context(agent.getId(), agent.getType(), request));
		addEvent("Init", null, agent, null);
		addEvent("ChildCreated", null, boss, null).addContexts(contexts);
		return true;
	}

	public boolean installNonGenericAgent(long agentType, AbstractAgent boss, int timeout, long request, ObjectClassQualifier objQual) {
		//AbstractAgent agent = AgentsFactory.createAgent(node.getId().toString(),rootAgentName,"-1",uA);
		if(timeout > 0) 
		{
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(-1, agentType, request));
			addEvent("ChildCreationTimeout", null, boss, null, timeout).addContexts(contexts);
		}
		AbstractNode node = AgentsManager.getInstance().getNode(boss);
		AbstractAgent agent = AgentsFactory.createAgent(node, null, AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(boss)).getPossibleEmployee(agentType));
		AbstractAgent root = AgentsManager.getInstance().getRoot(boss);
		agent.setName(root.getName() + "_" + agent.getId() + "_" + (agent.isGeneric() ? "G" : "NG"));
		//if (!NodesManager.getInstance().isAgentInstallable(agent, node) || !NodesManager.getInstance().isNodeInRanges(node, ranges)) {
			node =  NodesManager.getInstance().findBestHostNode(agent, node, objQual);
			if(node == null)
			{//TODO: start timer to sent childnotcreated event
				LOG.debug("no proper node found...");
				if(timeout >= 0)
					addToWaitingAgents(agent, request, timeout, false, boss, objQual);
				agent = null;
				return false;
			}
		//}
		AgentsManager.getInstance().registerAgent(agent, node, boss);
		Vector<Context> contexts = new Vector<Context>();
		contexts.add(new Context(agent.getId(), agent.getType(), request));
		addEvent("Init", null, agent, null);
		addEvent("ChildCreated", null, boss, null).addContexts(contexts);
		return true;
	}

	public boolean installNonGenericAgents(long agentType, AbstractAgent boss, int timeout, long request, ObjectClassQualifier objQual) {
		//AbstractAgent agent = AgentsFactory.createAgent(node.getId().toString(),rootAgentName,"-1",uA);
		if(timeout > 0) 
		{
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(-1, agentType, request));
			addEvent("ChildCreationTimeout", null, boss, null, timeout).addContexts(contexts);
		}
		AbstractNode node = AgentsManager.getInstance().getNode(boss);
		AbstractAgent agent = AgentsFactory.createAgent(node, null, AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(boss)).getPossibleEmployee(agentType));
		Vector<AbstractNode> hosts = new Vector<AbstractNode>();
		hosts = NodesManager.getInstance().findHostNodes(agent, node, objQual);
		LOG.debug(hosts.size()+" hosts found");
			if(hosts == null)
			{//TODO: start timer to sent childnotcreated event
				LOG.debug("no proper node found...");
				//przekazanie numeru requesta
				agent = null;
				return false;
			}
		for(AbstractNode anode : hosts)
		{
			AbstractAgent tempAgent = AgentsManager.getInstance().registerAgent(agent, anode, boss);
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(tempAgent.getId(), agent.getType(), request));
			addEvent("Init", null, tempAgent, null);
			addEvent("ChildCreated", null, boss, null).addContexts(contexts);
		}
		if(timeout >= 0)
			addToWaitingAgents(agent, request, timeout, true, boss, objQual);
		return true;
	}

	private void addToWaitingAgents(AbstractAgent agent, long request, int timeout, boolean many, AbstractAgent boss, ObjectClassQualifier objQual) 
	{
		waitingAgents.add(new WaitingAgent(agent, request, timeout, many, boss, objQual));
	}
	
	/**
	 * Provides handle events functionality. Finds out which agent should be notified about the event.
	 * 
	 * @param event - to be handled
	 */
	public void handleEvent(Event event) 
	{
		//LOG.debug("Pobicos handling event: "  + event.getName() );
		List<AbstractNode> nodesList = NodesManager.getInstance().getNodesWithAgents();
		if(event.getType() == "environment")
		{
			Trace.trace(Trace.HANDLE, null, null, new InfoData("environment"));
			if (nodesList != null) {
				for (Iterator<AbstractNode> it = nodesList.iterator(); it.hasNext();) 
				{
					AbstractNode tempNode = it.next();
					if(tempNode.raisesEvent(event))
						handleEvent(event, tempNode);
				}
			} else {
				//LOG.warn("No agents installed");
			}
		}
		else if(event.getType() == "node")
			handleEvent(event, event.getNode());
		else if(event.getType() == "agent")
			handleEvent(event, event.getAgent());
	}
	
	public void handleEvent(Event event, AbstractNode node) 
	{
		//LOG.debug("Pobicos handling event: "  + event.getName() + " on node " + node.getId());
		Trace.trace(Trace.HANDLE, event.getNode(), null, new InfoData(event.getName()));
		List<AbstractAgent> agentsList = AgentsManager.getInstance().getAgentList(node);
		if (agentsList != null) {
			for (Iterator<AbstractAgent> it = agentsList.iterator(); it.hasNext();) {
				AbstractAgent tempAgent = it.next();
				invokeHandler(tempAgent, event);
			}
		} else {
			//LOG.warn("No agents on node: '" + node.getId() + "' installed");
		}
	}
	
	public void handleEvent(Event event, AbstractAgent agent) 
	{
		Trace.trace(Trace.HANDLE, AgentsManager.getInstance().getNode(event.getAgent()), event.getAgent(), new InfoData(event.getName()));
		//LOG.debug("pobicos handling event: "  + event.getName() + " on agent " + agent.getType());
		invokeHandler(agent, event);
	}
	
	/**
	 * Invokes event handler method on a given agent. If handler cannot be
	 * found, such an event is ignored.
	 * 
	 * @param agent -
	 *            on which event handler is invoked (if it is only possible)
	 * @param event -
	 *            reference to BaseEvent that is supposed to be invoked
	 */
	private void invokeHandler(final AbstractAgent agent, final Event event) 
	{
		//LOG.debug("handler invoked; agent=" + agent.getName() + ", event=" + event.getCode());
		for(Event e : agent.getEventsEnabledList())
			if(EventTree.eventFires(event, e)){
				if(event.isGeneric())
					AgentsManager.getInstance().getNode(agent).getVm().setTask(agent, agent.getAddrForGenericEvent(event.getCode()));
				else/* if(agent.getIsGeneric())*/
					AgentsManager.getInstance().getNode(agent).getVm().setTask(agent, ((NonGenericAgent)agent).getAddrForNonGenericEvent(event.getCode()));
		}
	}

	public void addPhysicalEvent(String callID, AbstractNode node, String name, String params)
	{
		for(AbstractResource res : node.getResourceList())
			for(PhysicalEvent pevent : res.physicalEventsRaisen())
				for(Event event : pevent.getEvents())
					addEvent(callID, event.getCode(), node, null, null, SimulationsManager.getInstance().getStepDuration());
	}
	
	public Event addEvent(String name, AbstractNode node, AbstractAgent agent, String source)
	{
		return addEvent(null, EventMap.getCode(name), node, agent, source, SimulationsManager.getInstance().getStepDuration());
	}

	private Event addEvent(String name, AbstractNode node, AbstractAgent agent, String source, long timeout) 
	{
		return addEvent(null, EventMap.getCode(name), node, agent, source, timeout);
	}
	
	protected Event addEvent(String callID, final long code, final AbstractNode node, final AbstractAgent agent, final String source, long timeout)
	{
		Event event = null;
		if(source != null)
			event = new Event(code, source, SimulationsManager.getInstance().getSimulation().getVirtualTime() + timeout);
		else if(node != null)
			event = new Event(code, node/*, callID*/, SimulationsManager.getInstance().getSimulation().getVirtualTime() + timeout);
		else if(agent != null)
			event = new Event(code, agent, SimulationsManager.getInstance().getSimulation().getVirtualTime() + timeout);
		else
			return null;
		
		if (event != null) {
			SimulationsManager.getInstance().getSimulation().addSimulationEvent(event);
			ActionContainer.getAction(StartSimulationAction.ID).setEnabled(true);
			ActionContainer.getAction(StepForwardAction.ID).setEnabled(true);
			ActionContainer.getAction(SaveSimulationAction.ID).setEnabled(true);
		}	
		return event;
		//SimulationsManager.getInstance().getSimulation().setCurrentSimulationIndex(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex() - 1);
	}
	
	public enum MessageStatus {SENT, LOST, OLD};
	
	public void sendCommand(AbstractAgent sender, AbstractAgent recipient, String msg, boolean reliable)
	{
		Trace.trace(Trace.COMMAND, AgentsManager.getInstance().getNode(sender), sender, new InfoData(recipient.toString()));
		LOG.debug("Command sent: \"" + msg + "\"");
    	int proba = prob.nextInt(101);
    	Command command = new Command(sender, recipient, msg, SimulationsManager.getInstance().getSimulation().getVirtualTime());
    	for(Command com : commands.keySet())
    		if(com.getSender().equals(sender) && com.getRecipient().equals(recipient))
    		{
    			commands.remove(com);
    			break;
    		}
    	//if(commands.size() >= 3)
    	//	commands.values().toArray()[commands.size() - 3] = MessageStatus.OLD;
		if(reliable || proba <= PobicosManager.getInstance().getReliability())
		{
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(msg));
			addEvent("CommandArrived", null, recipient, null).addContexts(contexts);
			commands.put(command, MessageStatus.SENT);
		}
		else
		{
			Trace.trace(Trace.COMMANDLOST, AgentsManager.getInstance().getNode(sender), sender, new InfoData(recipient.toString()));
			commands.put(command, MessageStatus.LOST);
		}
		fireMessageSent(command);
	}
	
	public void sendReport(AbstractAgent sender, AbstractAgent recipient, String msg, boolean reliable)
	{
		Trace.trace(Trace.REPORT, AgentsManager.getInstance().getNode(sender), sender, new InfoData(recipient.toString()));
		LOG.debug("Report sent: \"" + msg + "\"");
		int proba = prob.nextInt(101);
    	Report report = new Report(sender, recipient, msg, SimulationsManager.getInstance().getSimulation().getVirtualTime());
    	for(Report rep : reports.keySet())
    		if(rep.getSender().equals(sender) && rep.getRecipient().equals(recipient))
    		{
    			reports.remove(rep);
    			break;
    		}
    	//if(reports.size() >= 3)
    	//	reports.values().toArray()[reports.size() - 3] = MessageStatus.OLD;
		if(reliable || proba <= PobicosManager.getInstance().getReliability())
		{
			Vector<Context> contexts = new Vector<Context>();
			contexts.add(new Context(sender.getId(), sender.getType(), -1));
			contexts.add(new Context(msg));
			addEvent("ReportArrived", null, recipient, null).addContexts(contexts);
			reports.put(report, MessageStatus.SENT);
		}
		else
		{
			Trace.trace(Trace.REPORTLOST, AgentsManager.getInstance().getNode(sender), sender, new InfoData(recipient.toString()));
			reports.put(report, MessageStatus.LOST);
		}
		fireMessageSent(report);
	}
	
	private Vector<ReportList> reportLists = new Vector<ReportList>();
	
	public void createReportList(ReportList rl)
	{
		reportLists.add(rl);
	}
	
	public boolean reportListExists(short id)
	{
		for(ReportList rl : reportLists)
			if(rl.getId() == id)
				return true;
		return false;
	}
	
	public void destroyReportList(short id)
	{
		for(ReportList rl : reportLists)
			if(rl.getId() == id)
			{
				reportLists.removeElement(rl);
				return;
			}
	}
	
	public ReportList getReportList(short id)
	{
		for(ReportList rl : reportLists)
			if(rl.getId() == id)
				return rl;
		return null;
	}
	
	public void addReport(short reportListId, AbstractAgent sender, AbstractAgent recipient, String msg)
	{
		getReportList(reportListId).addReport(sender, recipient, msg, SimulationsManager.getInstance().getSimulation().getVirtualTime());
	}
	
	public void setTimer(short tid, long timeout, final AbstractAgent agent)
	{
		for(Map.Entry<Integer, Event> timer : timers.entrySet())
			if(timer.getKey().equals(tid))
			{
				if(timeout == 0)
					timers.remove(timer);
				else
					timer.getValue().setVirtualTime(SimulationsManager.getInstance().getSimulation().getVirtualTime() + timeout);
				SimulationsManager.getInstance().getSimulation().getEventList().remove(timer.getValue());
				return;
			}
		//FIXME: ms / s
		if(timeout > 0)
			addEvent("Timeout", null, agent, null, timeout);
	}

	public void removeOldMessages() 
	{//FIXME: remove hard-code
		Vector<AbstractMessage> toDelete = new Vector<AbstractMessage>();
		Vector<AbstractMessage> toPut = new Vector<AbstractMessage>();
		int i = 0;
		for(Report report : reports.keySet())
		{
			if(report.getVirtualTime() + 7000 < SimulationsManager.getInstance().getSimulation().getVirtualTime())
				toDelete.add(report);
			else if(report.getVirtualTime() + 3000 < SimulationsManager.getInstance().getSimulation().getVirtualTime())
			{
				toDelete.add(report);
				toPut.add(report);
			}
			i++;
			//LOG.debug(report.getVirtualTime() + 7000);
		}
		i = 0;
		for(Command command : commands.keySet())
		{
			if(command.getVirtualTime() + 7000 < SimulationsManager.getInstance().getSimulation().getVirtualTime())
				toDelete.add(command);
			else if(command.getVirtualTime() + 3000 < SimulationsManager.getInstance().getSimulation().getVirtualTime())
			{
				toDelete.add(command);
				toPut.add(command);
			}
			i++;
		}
		for(AbstractMessage message : toDelete)
		{
			if(message instanceof Report)
				reports.remove(message);
			else
				commands.remove(message);
			fireMessageSent(message);
		}
		for(AbstractMessage message : toPut)
			if(message instanceof Report)
				reports.put((Report)message, MessageStatus.OLD);
			else
				commands.put((Command)message, MessageStatus.OLD);
	}

	/**
	 * Returns current ROVERS network name.
	 * 
	 * @return current ROVERS network name
	 */
	public String getPobicosName() {
		return this.pobicosName;
	}

	/**
	 * Sets the new ROVERS Network name.
	 * 
	 * @param roversName -
	 *            name to set
	 */
	public void setPobicosName(String pobicosName) {
		this.pobicosName = pobicosName;
	}

	/**
	 * Returns if maximum number of agents, defined for each node should be
	 * taken into account.
	 * 
	 * @return flag if maximum number of agents should be taken into account
	 */
	public boolean isMemoryLimited() {
		return this.memoryLimited;
	}

	/**
	 * Sets if maximum number of agents, defined for each node should be taken
	 * into account.
	 * 
	 * @param numAgentsLimited -
	 *            flag if maximum number of agents should be taken into account
	 */
	public void setMemoryLimited(boolean memoryLimited) {
		this.memoryLimited = memoryLimited;
	}

	/**
	 * Returns if antenna range defined for each node should be taken into
	 * account.
	 * 
	 * @return flag if nodes' antenna ranges should be taken into account
	 */
	public boolean isRangeLimited() {
		return this.rangeLimited;
	}

	/**
	 * Sets if antenna range, defined for each node should be taken into
	 * account.
	 * 
	 * @param rangeLimited -
	 *            flag if nodes' antenna range should be taken into account
	 */
	public void setRangeLimited(boolean rangeLimited) {
		this.rangeLimited = rangeLimited;
	}

	public long getNetworkId() {
		return networkId;
	}
	
	public void setNetworkId(long id)
	{
		this.networkId = id;
	}

	public int getReliability() 
	{
		return reliability;
	}

	public void setReliability(int reliability) 
	{
		this.reliability = reliability;
	}

	public Map<Report, MessageStatus> getReports() 
	{
		return reports;
	}

	public Map<Command, MessageStatus> getCommands() 
	{
		return commands;
	}

	private void fireMessageSent(AbstractMessage abstractMessage) {
		Object[] ls = this.listeners.getListeners();
		for (int i = 0; i < ls.length; i++) {
			IMessageListener messageListener = (IMessageListener) ls[i];
			messageListener.messageSent(abstractMessage);
		}
	}

	/**
	 * Adds listener to the listeners list. Each listeners is installed on
	 * object that listents if any message has just been sent.
	 * 
	 * @param listener -
	 *            listenter that is to be added to the listeners list
	 */
	public void addMessageListener(IMessageListener listener) {
		this.listeners.add(listener);
	}

	/**
	 * Removes listener from listners list. Each listener, that is supposed to
	 * be removed, has been installed on object listening if any message has
	 * just been sent.
	 * 
	 * @param listener -
	 *            listener to be removed from listners list
	 */
	public void removeMessageListner(IMessageListener listener) {
		this.listeners.remove(listener);
	}
	
	class WaitingAgent
	{
		private AbstractAgent agent;
		private long request;
		private int timeout;
		private long time;
		private boolean many;
		private AbstractAgent boss;
		private ObjectClassQualifier objQual;

		public WaitingAgent(AbstractAgent agent, long request, int timeout, boolean many, AbstractAgent boss, ObjectClassQualifier objQual)
		{
			this.agent = agent;
			this.request = request;
			this.timeout = timeout;
			this.time = time + timeout;
			this.many = many;
			this.boss = boss;
			this.objQual = objQual;
		}
	}
}
