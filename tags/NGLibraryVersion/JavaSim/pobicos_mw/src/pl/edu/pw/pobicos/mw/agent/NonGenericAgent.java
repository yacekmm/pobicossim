package pl.edu.pw.pobicos.mw.agent;

import java.util.ArrayList;
import java.util.Map;
import java.util.List;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.node.AbstractNode;


public class NonGenericAgent extends AbstractAgent {

	Map<Long, Integer> nonGenericEvents;
	List<Long> nonGenericInstructions;
	List<Event> nonGenericEventsList = new ArrayList<Event>();
	
	public NonGenericAgent(AbstractNode node, String name, long type, long magicField, long imageSize, List<Long> nonGenericInstruction, Map<Long, Integer> nonGenericEvent, Map<Long, Integer> genericEvent, Map<Long, Integer> genericCommunication, byte[] code, byte[] data, byte[] uA, long agentId)
	{
		super(node, name, false, type, magicField, imageSize, genericEvent, genericCommunication, code, data, uA, agentId);
		nonGenericEvents = nonGenericEvent;
		nonGenericInstructions = nonGenericInstruction;
		
		for(Long map : nonGenericEvents.keySet())
			nonGenericEventsList.add(new Event(map.longValue()));
	}
	
	public long getNonGenericInstruction(short idx)
	{
		if(idx < nonGenericInstructions.size())
			return nonGenericInstructions.get(idx);
		return -1;
	}
	
	public List<Long> getNonGenericInstructions()
	{
		return nonGenericInstructions;
	}
	
	public Map<Long, Integer> getNonGenericEvents()
	{
		return nonGenericEvents;
	}
	
	public int getAddrForNonGenericEvent(long code)
	{
		//String key = Conversions.intToHexString(code, 1);
		return nonGenericEvents.get(code);
	}
	
	public List<Event> getEventsList() 
	{
		List<Event> tempList = eventsList;
		for(Event event : nonGenericEventsList)
			tempList.add(event);
		return tempList;
	}
}
