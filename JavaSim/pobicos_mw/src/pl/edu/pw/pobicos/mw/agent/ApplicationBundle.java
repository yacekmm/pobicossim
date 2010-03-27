package pl.edu.pw.pobicos.mw.agent;

import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.Conversions;

public class ApplicationBundle {
	
	private Vector<AbstractAgent> agents = new Vector<AbstractAgent>();
	private Vector<ConfigSetting> settings = new Vector<ConfigSetting>();
	private Vector<Long> agentTypes = new Vector<Long>();
	private byte[] byteCode;
	
	private Map<Long, byte[]> possibleEmployees = new HashMap<Long, byte[]>();
	
	public ApplicationBundle(AbstractNode node, String name, byte[] applicationBundle)
	{
		//String applicationBundle = Conversions.byteArrayToHexString(AB);
		byteCode = applicationBundle;
		@SuppressWarnings("unused")
		long length = Conversions.byteToInt(0, 4, applicationBundle, true);
		int agentsNumber = (int)Conversions.byteToInt(4, 2, applicationBundle, true);
		Vector<Long> agentsLength = new Vector<Long>();
		int offset = 6;
		for(int i = 0; i < agentsNumber; i++)
			agentsLength.add(Conversions.byteToInt(offset + i * 4, 4, applicationBundle, true));
		offset += agentsNumber * 4;
		@SuppressWarnings("unused")
		long settingsLength = Conversions.byteToInt(offset, 4, applicationBundle, true);
		offset += 4;
		for(int i = 0; i < agentsNumber; i++)
		{
			byte[] agentCode = Conversions.subByte(offset, agentsLength.elementAt(i).intValue(), applicationBundle);
			if(i > 0)
			{
				//agents.add(AgentsFactory.createAgent(node, name+"_"+i, boss, agentCode, null));
				addPossibleEmployee(agentCode);
			}
			else
			{
				agents.add(AgentsFactory.createAgent(node, name, agentCode));
			}
			agentTypes.add(Conversions.byteToInt(0, 4, agentCode, true));
			offset += agentsLength.elementAt(i);
		}
		int settingsNumber = (int)Conversions.byteToInt(offset, 4, applicationBundle, true);
		offset += 4;
		for(int i = 0; i < settingsNumber; i++)
		{
			int end1 = Conversions.indexOfByte((byte)0, applicationBundle, offset + 5);
			int end2 = Conversions.indexOfByte((byte)0, applicationBundle, end1 + 1);
			settings.add(new ConfigSetting(Conversions.byteToInt(offset, 4, applicationBundle, true), i, new String(Conversions.subByte(offset + 5, end1 - offset - 5, applicationBundle)), new String(Conversions.subByte(end1 + 1, end2 - end1 - 1, applicationBundle))));
			offset = end2 + 1;
		}
		int end3 = Conversions.indexOfByte((byte)0, applicationBundle, offset);
		@SuppressWarnings("unused")
		String appName = new String(Conversions.subByte(offset, end3 - offset - 1, applicationBundle));
	}
	
	public int getAgentNumber()
	{
		return agents.size();
	}
	
	public AbstractAgent getAgent(int index)
	{
		return agents.elementAt(index);
	}
	
	public boolean agentExists(long type)
	{
		for(Long a : agentTypes)
			if(a.equals(type))
				return true;
		return false;
	}
	
	public AbstractAgent getRootAgent()
	{
		return getAgent(0);
	}
	
	public Vector<ConfigSetting> getConfigSettings()
	{
		return settings;
	}
	
	public ConfigSetting getConfigSetting(int index)
	{
		return settings.elementAt(index);
	}

	public void setConfigSetting(ConfigSetting set, String value) 
	{
		for(ConfigSetting cset : settings)
			if(set.equals(cset))
				cset.setValue(value);
	}

	public byte[] getByteCode() 
	{
		return byteCode;
	}
	
	public void addPossibleEmployee(byte[] code)
	{
		possibleEmployees.put(Conversions.byteToInt(0, 4, code, true), code);
	}
	
	public byte[] getPossibleEmployee(long type)
	{
		return possibleEmployees.get(type);
	}
}
