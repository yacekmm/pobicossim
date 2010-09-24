package pl.edu.pw.pobicos.mw.agent;

import java.util.List;
import java.util.Map;

import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * This class represent generic agent - one that does not require sensor, actuator resources, thus it may be
 * installed on every type of node. This class is also base class for any sensor agent type.
 *
 * @author Tomasz Anuszewski
 * @created 2007-11-14 21:12:44
 */
public class GenericAgent extends AbstractAgent {

	public GenericAgent(AbstractNode node, String name, long type, long magicField, long imageSize, Map<Long, Integer> genericEvent, Map<Long, Integer> genericCommunication, byte[] code, byte[] data, byte[] uA, long agentId)
	{
		super(node, name, true, type, magicField, imageSize, genericEvent, genericCommunication, code, data, uA, agentId);
	}
	
	public List<Long> getNonGenericInstructions()
	{
		return null;
	}
	
	public Map<Long, Integer> getNonGenericEvents()
	{
		return null;
	}
}
