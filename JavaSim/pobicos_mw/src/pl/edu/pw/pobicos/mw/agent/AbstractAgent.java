package pl.edu.pw.pobicos.mw.agent;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * Defines every micro-agent.
 * @author Michal Szczerbak, Tomasz Anuszewski
 */
public abstract class AbstractAgent {
	
	private byte[] byteCode;

	private String name;
	private long type;//POBICOS AgentType
	private long imageSize;	
	@SuppressWarnings("unused")
	private long magicField;	
	private boolean isGeneric;
	private long agentId;//POBICOS AgentId
		
	private Code code;	
	private Data data;	
	
	protected final List<Event> eventsList = new ArrayList<Event>();	
	protected final List<Event> eventsEnabledList = new ArrayList<Event>();	
	private Map<Long, Integer> genericEvents;	
	//TODO: check if there is still message handler dependent on child's id
	@SuppressWarnings("unused")
	private Map<Long, Integer> genericCommunications;
	
	public abstract List<Long> getNonGenericInstructions();
	
	public abstract Map<Long, Integer> getNonGenericEvents();
		
	/**
	 * Creates a new instance micro-agent.
	 * @param node destination node
	 * @param name agent's name
	 * @param isGeneric true if agent is a generic agent, false otherwise
	 * @param type POBICOS AgentType identifier
	 * @param magicField system-specific
	 * @param imageSize size of agent's image in bytes
	 * @param genericEvents map of events to addresses in code
	 * @param genericCommunications map of communication events to addresses in code
	 * @param code byte code
	 * @param data byte data
	 * @param uA image
	 * @param agentId id
	 */
	public AbstractAgent(AbstractNode node, String name, boolean isGeneric, long type, long magicField, long imageSize, Map<Long, Integer> genericEvents, Map<Long, Integer> genericCommunications, byte[] code, byte[] data, byte[] uA, long agentId) {
		this.type = type;
		this.name = name;
		this.isGeneric = isGeneric;
		this.magicField = magicField;
		this.imageSize = imageSize;
		this.code = new Code(code);
		this.data = new Data(data, node.getVm().getVM().getDataSize());
		this.genericEvents = genericEvents;
		this.genericCommunications = genericCommunications;
		this.byteCode = uA;
		this.agentId = agentId;
		
		for(Long map : genericEvents.keySet())
		{
			eventsList.add(new Event(map.longValue()));
			if(map.longValue() == 1)
				eventsEnabledList.add(eventsList.get(eventsList.size() - 1));
		}
	}
	
	public int getAddrForGenericEvent(long code)
	{
		return genericEvents.get(code);
	}
	
	public long getType()
	{
		return type;
	}
	
	public long getId()
	{
		return agentId;
	}

	/**
	 * Returns name of this micro-agent.
	 * 
	 * @return name of this micro-agent
	 */
	public String getName() {
		return this.name;
	}

	/**
	 * Sets new name for this micro-agent.
	 * 
	 * @param name - new name for this micro-agent 
	 */
	public void setName(String name) 
	{
		this.name = name;
	}
	
	public List<Event> getEventsList() 
	{
		return eventsList;
	}
	
	public List<Event> getEventsEnabledList() 
	{
		return eventsEnabledList;
	}
	
	public Code getCode()
	{
		return code;
	}
	
	public Data getData()
	{
		return data;
	}
	
	public byte[] getByteCode()
	{
		return byteCode;
	}
	
	public long getSize()
	{
		return imageSize;
	}

	public boolean isGeneric() 
	{
		return isGeneric;
	}
		
	@Override
	public final String toString() {
		String toString = "Agent: " + agentId + "(" + type + ") ";
		return toString;
	}
}
