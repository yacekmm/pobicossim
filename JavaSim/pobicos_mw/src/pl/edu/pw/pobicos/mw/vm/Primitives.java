package pl.edu.pw.pobicos.mw.vm;

import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.event.Context;
import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.instruction.InstructionMap;
import pl.edu.pw.pobicos.mw.instruction.ReturnValue;
import pl.edu.pw.pobicos.mw.logging.InfoData;
import pl.edu.pw.pobicos.mw.logging.Trace;
import pl.edu.pw.pobicos.mw.message.Message;
import pl.edu.pw.pobicos.mw.message.Report;
import pl.edu.pw.pobicos.mw.message.ReportList;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.network.Client;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.taxonomy.ObjectClassQualifier;
import pl.edu.pw.pobicos.mw.vm.avr.Properties;

/**
 * Implements the primitives processing by the middleware.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Primitives {
	private static Map<AbstractNode, Long> waiting = new HashMap<AbstractNode, Long>();
	//nor__ private static Map<AbstractNode, String> waiting = new HashMap<AbstractNode, String>();
	private static long instrNumber = 0;
	//private static long eventNumber = 0;

	/**
	 * Sends instruction to a non-generic resource.
	 * @param primitive instruction name
	 * @param a micro-agent firing instruction
	 * @param params instruction parameters
	 * @return return value: RETCODE_OK or other value depending on the instruction specification
	 */
	public static Object sendRequest(String primitive, AbstractAgent a, String params) 
	{
		long localInstrNumber = instrNumber++;
		if (instrNumber > 1000000)
			instrNumber = 0;
		String callID = AgentsManager.getInstance().getNode(a).getId() + "#" + localInstrNumber;
		Trace.trace(Trace.INSTRUCTIONSENT, AgentsManager.getInstance().getNode(a), a, new InfoData(primitive + " (" + params + ")"));
		if(InstructionMap.getReturn(InstructionMap.getCode(primitive)) != null)
			waiting.put(AgentsManager.getInstance().getNode(a), null);
			//ret__ waiting.put(AgentsManager.getInstance().getNode(a), callID);
        Client.getInstance().sendInstruction(callID, InstructionMap.getCode(primitive), params);
		if(InstructionMap.getReturn(InstructionMap.getCode(primitive)) != null)
		{
			while(1==1)
			{
				if(waiting.keySet().contains(AgentsManager.getInstance().getNode(a)))
					//ret__ if(!waiting.get(AgentsManager.getInstance().getNode(a)).equals(callID))// != null)
					if(waiting.get(AgentsManager.getInstance().getNode(a)) != null)
						break;
			}
			//nor__ String response = waiting.get(AgentsManager.getInstance().getNode(a));
			long response = waiting.get(AgentsManager.getInstance().getNode(a));
			waiting.remove(AgentsManager.getInstance().getNode(a));
			Trace.trace(Trace.INSTRUCTIONRESP, AgentsManager.getInstance().getNode(a), a, new InfoData(String.valueOf((response))));
			return response;
		}
		return new ReturnValue(ReturnValue.RETCODE_OK);
	}
	
	/**
	 * Finishes a waiting process after receiving a return value by a non-generic resource.
	 * @param callID call_id of an instruction sent by waiting micro-agent
	 * @param value returned value
	 */
	public static void respond(String callID, String value)
	{
		long id = Long.parseLong(callID.substring(0, callID.indexOf("#")));
		System.out.println(",");
		AbstractNode node = NodesManager.getInstance().getNode(id);
		
		if(waiting.keySet().contains(node))
		{
			System.out.println(";");
			waiting.put(NodesManager.getInstance().getNode(id), Long.parseLong(value));
		}
		
/*nor__		if(waiting.keySet().contains(node))
		{
			if(waiting.get(node).equals(callID))
			{
				System.out.println(";");
				waiting.put(NodesManager.getInstance().getNode(id), value);
			}
		}
		*/
		else
			for(AbstractNode aNode : waiting.keySet())
				System.out.println(aNode.toString());
	}

	/**
	 * <b>Description:</b><br/>
	 * This instruction enables an event, so that the corresponding handler (which must be available) is invoked when such events occur. All events, except for the InitEvent, must be enabled explicitly, if/as needed.
	 * <br/><br/>
	 * <b>Associated Events:</b><br/>
	 * None.
	 * @param a micro-agent firing instruction
	 * @param eventType Type of event to enable
	 * @return RETCODE_OK if successful, RETCODE_BADARGS if the event type supplied is invalid or the agent does not have a handler for this event type
	 */
	public static ReturnValue EnableEvent_execute(AbstractAgent a, long eventType)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("EnableEvent"));
    	boolean goon = false;
    	for(Event e : a.getEventsList())
    		if(e.getCode() == eventType)
    		{
    			goon = true;
    			break;
    		}
    	if(!goon)
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	a.getEventsEnabledList().add(new Event(eventType));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }

	/**
	 * <b>Description:</b><br/>
	 * This instruction disables an event, so that the invocation of the corresponding handler is suppressed when such events occur. All events, except for the InitEvent, are disabled by default.
	 * <br/><br/>
	 * <b>Associated Events:</b><br/>
	 * None.
	 * @param a micro-agent firing instruction
	 * @param eventType Type of event to disable
	 * @return RETCODE_OK if successful, RETCODE_BADARGS if the event type supplied is invalid
	 */
    public static ReturnValue DisableEvent_execute(AbstractAgent a, long eventType)
	{
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("DisableEvent"));
		boolean goon = false;
		for(Event e : a.getEventsList())
			if(e.getCode() == eventType)
			{
				goon = true;
				break;
			}
		if(!goon)
	    	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
		for(Event e : a.getEventsEnabledList())
			if(e.getCode() == eventType)
			{
				a.getEventsEnabledList().remove(e);
				break;
			}
		return new ReturnValue(ReturnValue.RETCODE_OK);
	}

    /**
     * <b>Description:</b><br/>
     * This instruction adds a request for creating an instance of a generic agent type to the POBICOS middleware, and returns immediately. The middleware will asynchronously give its best to find a POBICOS-enabled object that can host an instance of the specified agent type, within the specified time limit (if any). The request is removed if it succeeds or times out.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * If a new agent is successfully created, a <i>ChildCreatedEvent</i> will be issued. If the supplied timeout value is positive and no agent could be created within the specified time limit, a <i>ChildCreationTimeoutEvent</i> will be issued.
     * @param a micro-agent firing instruction
     * @param agentType The type of the agent to be created as a child
     * @param timeout Timeout in seconds (0 means try for ever)
     * @param reqHandle The handle for this request
     * @return RETCODE_OK if request was successfully queued, RETCODE_NORES if request cannot be queued due to lack of resources, RETCODE_BADARGS if the specified micro-agent type does not exist
     */
    public static ReturnValue CreateGenericAgent_execute(AbstractAgent a, long agentType, int timeout, long reqHandle)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("CreateGenericAgent"));
    	if(!AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(a)).agentExists(agentType))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	PobicosManager.getInstance().installAgent(agentType, a, timeout, reqHandle);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }

    /**
     * <b>Description:</b><br/>
     * This instruction adds a request for creating one or more instances of a non-generic agent type to the POBICOS middleware, and returns immediately. The middleware will asynchronously give its best to find one or more POBICOS-enabled objects that can host an instance of the specified agent type9, within the specified time limit (if any).
     * <br/><br/>
     * For “exactly one instance”, the request is removed as soon as it succeeds or times out. For “as many instances as possible”, the request is removed only if it times out. If the supplied timeout value is zero, the request will remain active until the agent that issued the request is removed.
     * <br/><br/>
     * The object selection expression can be optionally used to specify (constrain) the range of objects which should be considered as hosts. If this parameter is NULL, any object that supports the nongeneric instructions and events employed by the specified agent type is eligible as a host.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Each time a new agent is created, a <i>ChildCreatedEvent</i> will be issued. If the supplied timeout value is positive, a <i>ChildCreationTimeoutEvent</i> will be issued when the request times out.
     * @param a micro-agent firing instruction
     * @param agentType The type of the agent to be created as a child
     * @param pointer Pointer to the object selection expression
     * @param number Number of children to be created (1 = exactly one instance, <>1 as many instances as possible)
     * @param timeout Timeout in seconds (0 means try for ever)
     * @param reqHandle The handle for this request
     * @return RETCODE_OK if request was successfully queued, RETCODE_NORES if request cannot be queued due to lack of resources, RETCODE_BADARGS if the specified agent type does not exist
     */
    public static ReturnValue CreateNonGenericAgents_execute(AbstractAgent a, long agentType, int pointer, short number, int timeout, long reqHandle)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("CreateNonGenericAgents"));
    	if(!AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(a)).agentExists(agentType))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	ObjectClassQualifier qual = new ObjectClassQualifier();
    	qual.numberOfRanges = a.getData().get8(pointer - Properties.AVR_REG_IO_SIZE);
		qual.createRanges();
    	for(int i = 0; i < qual.numberOfRanges; i++)
    	{
    		qual.ranges[i].mostSpecific.code = a.getData().get32(pointer - Properties.AVR_REG_IO_SIZE + 1 + i * 8);
    		qual.ranges[i].mostAbstract.code = a.getData().get32(pointer - Properties.AVR_REG_IO_SIZE + 1 + i * 8 + 4);
    	}
    	if(number == 1)
    	{
        	PobicosManager.getInstance().installNonGenericAgent(agentType, a, timeout, reqHandle, qual);
    	}
        else
        {
        	PobicosManager.getInstance().installNonGenericAgents(agentType, a, timeout, reqHandle, qual);
        }
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction returns the identifier of the agent.<br/>
     * An agent can pass this value as a parameter to the Release instruction to kill itself.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @return The identifier of the agent
     */
    public static long GetMyID_execute(AbstractAgent a)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetMyID"));
    	return a.getId();
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves the identifier and type of the child agent associated with an event as well as the handle associated with the corresponding agent creation request.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Should be invoked only within the context of a <i>ChildCreatedEvent</i>, <i>ChildCreationTimeoutEvent</i>, <i>ChildUnreachableEvent</i> and <i>ReportArrivedEvent</i> handler. In the second case, the agent identifier information is not valid (there is no child identifier to return).
     * @param a micro-agent firing instruction
     * @param pid The identifier of the child associated with an event
     * @param ptp The type of the child associated with an event
     * @param prh The handle that was assigned to the corresponding agent creation request
     * @return RETCODE_OK if successful, RETCODE_NODATA if no such data is available for retrieval
     */
    public static ReturnValue GetChildInfo_execute(AbstractAgent a, int pid, int ptp, int prh)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetChildInfo"));
    	Event event = SimulationsManager.getInstance().getSimulation().getEventList().get(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex());
    	if(!(event.getName().equals("ChildCreated") || event.getName().equals("ChildCreationTimeout") || event.getName().equals("ChildUnrecheable") || event.getName().equals("ReportArrived")))
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Vector<Context> contexts = event.getContexts();
    	if(contexts == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Context context = null;
    	for(Context cont : contexts)
    		if(cont.type == Context.ContextType.child)
    			context = cont;
    	if(context == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	long rhandle = context.childRequestHandle;
    	long id = context.childId;
    	long type = context.childType;
    	a.getData().put32((char)(prh - Properties.AVR_REG_IO_SIZE), rhandle);
    	a.getData().put32((char)(pid - Properties.AVR_REG_IO_SIZE), id);
    	a.getData().put32((char)(ptp - Properties.AVR_REG_IO_SIZE), type);
    	System.out.println(id+":"+type);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/> 
     * This instruction invalidates the supplied agent identifier and returns immediately. The POBICOS middleware will (asynchronously) remove the agent. An agent can only release its children and itself (by passing its own identifier as a parameter). 
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Before the released agent is removed, it will be issued a <i>FinalizeEvent</i>.
     * @param a micro-agent firing instruction
     * @param agentId The identifier of the agent to be released.
     * @return RETCODE_OK if successful, RETCODE_BADARGS if the specified agent identifier is invalid.
     */
    public static ReturnValue Release_execute(AbstractAgent a, long agentId)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("Release"));
    	AbstractAgent aa = AgentsManager.getInstance().getAgent(agentId);
    	if(aa == null)
    		return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	AgentsManager.getInstance().removeAgent(aa);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/> 
     * This instruction queues up a command message for transmission to a child. Best-effort commands are delivered in FIFO order at most once. Reliable commands are delivered in FIFO order exactly once, unless the child becomes unreachable. The FIFO delivery order is not guaranteed between best-effort and reliable commands. 
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param agentId The identifier of the child agent to which to send the command.
     * @param pointer The command message to send.
     * @param reliable Reliable delivery option (=0 disable, >0 enable)
     * @return RETCODE_OK if the message was successfully queued, RETCODE_NORES if it was not queued due to lack of resources, RETCODE_BADARGS if the specified agent identifier is invalid.
     */
    public static ReturnValue SendCommand_execute(AbstractAgent a, long agentId, int pointer, short reliable)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("SendCommand"));
    	AbstractAgent aa = AgentsManager.getInstance().getAgent(agentId);
    	if(aa == null)
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	int msgLength = a.getData().get8(pointer - Properties.AVR_REG_IO_SIZE);
    	if(msgLength > Message.MSG_DATA_MAXLEN)
        	return new ReturnValue(ReturnValue.RETCODE_NORES);
    	String msg = "";
    	for(int i = 0; i < msgLength; i++)
    	{
    		char c = (char)a.getData().get8(pointer + 1 + i - Properties.AVR_REG_IO_SIZE);
    		if(c == 0)
    			break;
    		msg += new Character((char)c).toString();
    	}
    	PobicosManager.getInstance().sendCommand(a, aa, msg, (reliable == 0 ? false : true));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves the command message that has arrived from the agent’s parent.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Should be invoked only within a <i>CommandArrivedEvent</i> handler.
     * @param a micro-agent firing instruction
     * @param pointer The command message (sent by the parent).
     * @return RETCODE_OK if successful, RETCODE_NODATA if no such data is available for retrieval.
     */
    public static ReturnValue GetCommand_execute(AbstractAgent a, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetCommand"));
    	Event event = SimulationsManager.getInstance().getSimulation().getEventList().get(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex());
    	if(!(event.getName().equals("CommandArrived")))
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Vector<Context> contexts = event.getContexts();
    	if(contexts == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Context context = null;
    	for(Context cont : contexts)
    		if(cont.type == Context.ContextType.message)
    			context = cont;
    	if(context == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	if(context.messageMsg == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE), (short)context.messageMsg.length());
    	for(int i = 0; i < (short)context.messageMsg.length(); i++)
    		a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE + 1 + i), (short)context.messageMsg.toCharArray()[i]);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction queues up a report message for transmission to the parent. Best-effort reports are delivered in FIFO order at most once. Reliable reports are delivered in FIFO order exactly once, unless the parent becomes unreachable. The FIFO delivery order is not guaranteed between besteffort and reliable reports.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param pointer The report message to send.
     * @param reliable Reliable delivery option (=0 disabled, >0 enabled)
     * @return RETCODE_OK if the message was successfully queued, RETCODE_NORES if it was not queued due to lack of resources.
     */
    public static ReturnValue SendReport_execute(AbstractAgent a, int pointer, short reliable)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("SendReport"));
    	AbstractAgent aa = AgentsManager.getInstance().getBoss(a);
    	int msgLength = a.getData().get8(pointer - Properties.AVR_REG_IO_SIZE);
    	if(msgLength > Message.MSG_DATA_MAXLEN)
        	return new ReturnValue(ReturnValue.RETCODE_NORES);
    	String msg = "";
    	for(int i = 0; i < msgLength; i++){
    		char c = (char)a.getData().get8(pointer + 1 + i - Properties.AVR_REG_IO_SIZE);
    		if(c == 0)
    			break;
    		msg += new Character((char)c).toString();
    	}
    	PobicosManager.getInstance().sendReport(a, aa, msg, (reliable == 0 ? false : true));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves a report message that has arrived from one of the agent’s children. The identifier and type of the child can be retrieved via the GetChildInfo instruction.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Should be invoked only within a <i>ReportArrivedEvent</i> handler.
     * @param a micro-agent firing instruction
     * @param pointer The report message (sent by the child).
     * @return RETCODE_OK if successful, RETCODE_NODATA if no such data is available for retrieval.
     */
    public static ReturnValue GetReport_execute(AbstractAgent a, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetReport"));
    	Event event = SimulationsManager.getInstance().getSimulation().getEventList().get(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex());
    	if(!(event.getName().equals("ReportArrived")))
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Vector<Context> contexts = event.getContexts();
    	if(contexts == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Context context = null;
    	for(Context cont : contexts)
    		if(cont.type == Context.ContextType.message)
    			context = cont;
    	if(context == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	String msg = context.messageMsg;
    	System.out.println(msg);
    	if(msg == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE), (short)msg.length() + 1);
    	for(int i = 0; i < (short)msg.length(); i++)
    		a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE + 1 + i), (short)msg.toCharArray()[i]);
    	a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE + 1 + msg.length()), 0);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction creates a new report list and returns its identifier (handler).
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param agentType The type of the child agents whose reports are to be stored in the list to be created.
     * @param pointer pointer to The identifier of the report list created.
     * @return RETCODE_OK if a report list was successfully created, RETCODE_BADARGS if the specified agent type is invalid, RETCODE_NORES if it was not possible to create a new report list due to lack of resources.
     */
    public static ReturnValue CreateReportList_execute(AbstractAgent a, long agentType, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("CreateReportList"));
    	if(!AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(a)).agentExists(agentType))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	ReportList rl = new ReportList(agentType);
    	short id = rl.getId();
    	a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE), id);
    	PobicosManager.getInstance().createReportList(rl);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction destroys a report list.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param id The identifier of the report list to be destroyed.
     * @return RETCODE_OK if successful, RETCODE_BADARGS if the specified report list identifier is invalid.
     */
    public static ReturnValue DestroyReportList_execute(AbstractAgent a, short id)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("DestroyReportList"));
    	if(!PobicosManager.getInstance().reportListExists(id))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	PobicosManager.getInstance().destroyReportList(id);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction adds a report in a report list. If a report from the same agent exists already in the report list, it will be overwritten with the newly supplied report message.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param id The identifier of the report list.
     * @param agentId The identifier of the child that generated the report.
     * @param pointer The report message to be added in the list.
     * @return RETCODE_OK if the message was successfully added to the list, RETCODE_BADARGS if the specified report list identifier or agent identifier is invalid or the type of the agent does not match the type of the report list, RETCODE_NORES if the message was not added due to lack of resources.
     */
    public static ReturnValue AddReport_execute(AbstractAgent a, short id, long agentId, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("AddReport"));
    	if(!PobicosManager.getInstance().reportListExists(id))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	ReportList rl = PobicosManager.getInstance().getReportList(id);
    	boolean goon = false;
    	for(AbstractAgent aa : AgentsManager.getInstance().getAgents())
    		if(aa.getId() == agentId)
    			goon = true;
    	if(!goon)
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	if(AgentsManager.getInstance().getAgent(agentId).getType() != rl.getType())
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	int msgLength = a.getData().get8(pointer - Properties.AVR_REG_IO_SIZE);
    	String msg = "";
    	for(int i = 0; i < msgLength; i++)
    	{
    		char c = (char)a.getData().get8(pointer + 1 + i - Properties.AVR_REG_IO_SIZE);
    		if(c == 0)
    			break;
    		msg += new Character((char)c).toString();
    	}
    	PobicosManager.getInstance().addReport(id, AgentsManager.getInstance().getAgent(agentId), a, msg);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves the next report in the list. After the last report is retrieved, the list is automatically initialized for a fresh traversal (the contents remain the same).<br/>There is no “destructive” report retrieval or reset instruction. To “empty” a report list, one may destroy the one currently used and create a new one.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param id The identifier of the report list.
     * @param pid pointer to The identifier of the agent that generated the report.
     * @param pmsg pointer to The report message.
     * @return RETCODE_OK if successful, RETCODE_BADARGS if the specified report list identifier is invalid, RETCODE_NODATA if there is no report message to retrieve.
     */
    public static ReturnValue GetNextReport_execute(AbstractAgent a, short id, int pid, int pmsg)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetNextReport"));
    	if(!PobicosManager.getInstance().reportListExists(id))
        	return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	Report report = PobicosManager.getInstance().getReportList(id).getNextReport();
    	if(report == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	a.getData().put32((char)(pid - Properties.AVR_REG_IO_SIZE), report.getSender().getId());
    	a.getData().put8((char)(pmsg - Properties.AVR_REG_IO_SIZE), (short)report.getMessage().getLength());
    	for(short i = 0; i < (short)report.getMessage().getLength(); i++)
    		a.getData().put8((char)(pmsg - Properties.AVR_REG_IO_SIZE + 1 + i), report.getMessage().getCharAt(i));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves the value for an agent configuration setting and stores it in the supplied buffer as a (zero delimited) ASCII string. The programmer is responsible for providing a sufficiently long buffer (configuration values are at most <i>CONFIG_VAL_MAXLEN</i> long).
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Can be invoked from a <i>ConfigSettingsChangedEvent</i> handler, but also from anywhere in the code.
     * @param a micro-agent firing instruction
     * @param id Index of the configuration setting for which the value should be returned.
     * @param pointer pointer to Buffer to store the value of the configuration setting.
     * @param size Size of the buffer for storing the configuration value.
     * @return RETCODE_OK if successful, RETCODE_BADARGS if the supplied index is invalid, RETCODE_NORES if the value does not fit into the supplied buffer.
     */
    public static ReturnValue GetConfigSetting_execute(AbstractAgent a, short id, int pointer, short size)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetConfigSetting"));
    	String setting = "";
    	try
    	{
    		setting = /*AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(a)).getConfigSetting(id).getName() + '\0' +*/ AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(a)).getConfigSetting(id).getValue() + '\0';
    	}catch(Exception ex)
    	{
			return new ReturnValue(ReturnValue.RETCODE_BADARGS);
    	}
    	try
    	{
    	for(int i = 0; i < size; i++)
    		a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE + i), (short)setting.charAt(i));
    	}catch(Exception ex)
    	{
    		if(size >= setting.length())
    			return new ReturnValue(ReturnValue.RETCODE_OK);
    		else
    			return new ReturnValue(ReturnValue.RETCODE_NORES);
    	}
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction installs a timer with a given identifier to expire after the supplied timeout. If there is already a timer installed with the same identifier on behalf of the same agent, it is overwritten using the new timeout value.<br/>A timeout of 0 cancels the timer with the supplied identifier, if there is such a timer installed, else has no effect.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * When the timer expires, a <i>TimeoutEvent</i> is issued.
     * @param a micro-agent firing instruction
     * @param tid The identifier for this timer.
     * @param time The timeout value in ms (=0 cancel).
     * @return RETCODE_OK if a timer was successfully installed, RETCODE_NORES if it is not possible to install a timer due to lack of resources.
     */
    public static ReturnValue SetTimer_execute(AbstractAgent a, short tid, long time)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("SetTimer"));
    	PobicosManager.getInstance().setTimer(tid, time, a);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction retrieves the identifier associated with an expired timer.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * Should be invoked only within a <i>TimeoutEvent</i> handler.
     * @param a micro-agent firing instruction
     * @param pointer pointer to The identifier associated with the expired timer.
     * @return RETCODE_OK if successful, RETCODE_NODATA if no such data is available for retrieval.
     */
    public static ReturnValue GetTimerId_execute(AbstractAgent a, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("GetTimerId"));
    	Event event = SimulationsManager.getInstance().getSimulation().getEventList().get(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex());
    	if(!(event.getName().equals("Timeout")))
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Vector<Context> contexts = event.getContexts();
    	if(contexts == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	Context context = null;
    	for(Context cont : contexts)
    		if(cont.type == Context.ContextType.timer)
    			context = cont;
    	if(context == null)
    		return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	short id = context.timerId;
    	if(id == -1)
        	return new ReturnValue(ReturnValue.RETCODE_NODATA);
    	a.getData().put8((char)(pointer - Properties.AVR_REG_IO_SIZE), id);
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction prints an ASCII string (zero delimited) to the debugging output channel.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param pointer pointer to The string (message) to print in the debug output.
     * @return RETCODE_OK
     */
    public static ReturnValue DbgString_execute(AbstractAgent a, int pointer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("DbgString"));
    	String s = "";
    	char current;
    	int i = 0;
    	while(true)
    	{
    		current = (char)a.getData().get8(pointer - Properties.AVR_REG_IO_SIZE + i);
    		s += current;
    		i++;
    		if(current == '\0')
    			break;
    	}
    	Trace.trace("Primitive DbgString", AgentsManager.getInstance().getNode(a), a, new InfoData(s));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
    
    /**
     * <b>Description:</b><br/>
     * This instruction prints an integer value to the debugging output channel.
     * <br/><br/>
     * <b>Associated Events:</b><br/>
     * None.
     * @param a micro-agent firing instruction
     * @param integer The integer value to print on the debug output.
     * @return RETCODE_OK
     */
    public static ReturnValue DbgUInt32_execute(AbstractAgent a, long integer)
    {
		Trace.trace(Trace.PRIMITIVECALL, AgentsManager.getInstance().getNode(a), a, new InfoData("DbgUInt32"));
    	Trace.trace("Primitive DbgUInt32", AgentsManager.getInstance().getNode(a), a, new InfoData(((Long)integer).toString()));
    	return new ReturnValue(ReturnValue.RETCODE_OK);
    }
}
