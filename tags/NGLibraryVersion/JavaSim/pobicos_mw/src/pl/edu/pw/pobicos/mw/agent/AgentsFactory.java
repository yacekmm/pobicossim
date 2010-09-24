package pl.edu.pw.pobicos.mw.agent;

import org.apache.log4j.*;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.GenericAgent;
import pl.edu.pw.pobicos.mw.agent.NonGenericAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.Conversions;

/**
 * This class is the Factory class that allows for creating different types of
 * agents, by invoking static factory methods. This class cannot be
 * instantiated.
 * 
 * @author Tomasz Anuszewski
 * @created 2007-11-26 19:03:42
 */
public final class AgentsFactory {
	
	private final static Logger LOG = Logger.getLogger(AgentsFactory.class);

	// prevents from instantiating this class
	private AgentsFactory() {
		// empty
	}
	
	public static ApplicationBundle importBundle(AbstractNode node, String name, byte[] ab)
	{
		LOG.info("importing bundle");
		ApplicationBundle AB = new ApplicationBundle(node, name, ab);
		return AB;
	}
	
	public static AbstractAgent createAgent(AbstractNode node, String name, byte[] uA)
	{
		LOG.info("creating agent");
		boolean isGeneric = (Conversions.byteToInt(3, 1, uA, true) <16 ? true : false);
		long type = Conversions.byteToInt(0, 4, uA, true);
		long magicField = Conversions.byteToInt(4, 1, uA, true);
		long imageSize = Conversions.byteToInt(5, 1, uA, true) * 256;
		
		byte[] code, data;
		Map<Long, Integer> nonGenericEvent = new HashMap<Long, Integer>(), genericEvent = new HashMap<Long, Integer>(), genericCommunication = new HashMap<Long, Integer>();
		List<Long> nonGenericInstruction = new ArrayList<Long>();
		int offset = 6;
		if(isGeneric)
		{
			int genericEventNumber = (int)Conversions.byteToInt(offset, 1, uA, true);
			for (int i=0 ; i < genericEventNumber ; i++)
			{
				genericEvent.put(Conversions.byteToInt(offset + 1 + i * 3, 1, uA, true), (int)Conversions.byteToInt(offset + 2 + i * 3, 2, uA, true));
			}
			offset += 1 + genericEventNumber * 3;
			int genericCommunicationNumber = (int)Conversions.byteToInt(offset, 1, uA, true);
			for (int i=0 ; i < genericCommunicationNumber ; i++)
			{
				genericCommunication.put(Conversions.byteToInt(offset + 1 + i * 3, 4, uA, true), (int)Conversions.byteToInt(offset + 5 + i * 3, 2, uA, true));
			}
			offset += 1 + genericCommunicationNumber * 6;
			int codeLength = (int)Conversions.byteToInt(offset, 2, uA, true);
			code = Conversions.subByte(offset + 2, codeLength, uA);
			offset += 2 + codeLength;
			int dataLength = (int)Conversions.byteToInt(offset, 2, uA, true);
			data = Conversions.subByte(offset + 2, dataLength, uA);
			return new GenericAgent(node, name, type, magicField, imageSize, genericEvent, genericCommunication, code, data, uA, AgentsManager.getInstance().findId());
		}
		else{
			int nonGenericInstructionNumber = (int)Conversions.byteToInt(offset, 1, uA, true);
			int nonGenericEventNumber = (int)Conversions.byteToInt(offset + 1, 1, uA, true);
			offset += 2;
			for(int i = 0; i < nonGenericInstructionNumber; i++)
			{
				nonGenericInstruction.add(Conversions.byteToInt(offset + i * 4, 4, uA, true));
			}
			offset += nonGenericInstructionNumber * 4;
			for(int i = 0; i < nonGenericEventNumber; i++)
			{
				nonGenericEvent.put(Conversions.byteToInt(offset + i * 4, 4, uA, true), (int)Conversions.byteToInt(offset + 4 + i * 4, 2, uA, true));
			}
			offset += nonGenericEventNumber * 6;
			int genericEventNumber = (int)Conversions.byteToInt(offset, 1, uA, true);
			for (int i=0 ; i < genericEventNumber ; i++)
			{
				genericEvent.put(Conversions.byteToInt(offset + 1 + i * 3, 1, uA, true), (int)Conversions.byteToInt(offset + 2 + i * 3, 2, uA, true));
			}
			offset += 1 + genericEventNumber * 3;
			int genericCommunicationNumber = (int)Conversions.byteToInt(offset, 1, uA, true);
			for (int i=0 ; i < genericCommunicationNumber ; i++)
			{
				genericCommunication.put(Conversions.byteToInt(offset + 1 + i * 3, 4, uA, true), (int)Conversions.byteToInt(offset + 5 + i * 3, 2, uA, true));
			}
			offset += 1 + genericCommunicationNumber * 6;
			int codeLength = (int)Conversions.byteToInt(offset, 2, uA, true);
			code = Conversions.subByte(offset + 2, codeLength, uA);
			offset += 2 + codeLength;
			int dataLength = (int)Conversions.byteToInt(offset, 2, uA, true);
			data = Conversions.subByte(offset + 2, dataLength, uA);
			return new NonGenericAgent(node, name, type, magicField, imageSize, nonGenericInstruction, nonGenericEvent, genericEvent, genericCommunication, code, data, uA, AgentsManager.getInstance().findId());
		}
	}
}
