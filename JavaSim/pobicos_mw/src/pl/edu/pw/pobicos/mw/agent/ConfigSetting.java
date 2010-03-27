package pl.edu.pw.pobicos.mw.agent;

public class ConfigSetting {
	
	public static final short CONFIG_VAL_MAXLEN = 64;

	private long agentType;
	@SuppressWarnings("unused")
	private int index;//POBICOS ConfigIndex
	private String name, value;
	
	public ConfigSetting(long agentType, int index, String name, String value)
	{
		this.agentType = agentType;
		this.index = index;
		this.name = name;
		this.value = (value.length() > CONFIG_VAL_MAXLEN ? value.substring(0, CONFIG_VAL_MAXLEN - 1) : value);
	}
	
	public long getAgentType()
	{
		return agentType;
	}
	
	public String getName()
	{
		return name;
	}
	
	public String getValue()
	{
		return value;
	}

	public void setValue(String value) 
	{
		this.value = value;
	}
}
