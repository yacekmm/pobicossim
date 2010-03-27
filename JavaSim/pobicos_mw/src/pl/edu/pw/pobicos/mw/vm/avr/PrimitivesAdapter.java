package pl.edu.pw.pobicos.mw.vm.avr;

import java.lang.reflect.Method;
import java.util.Vector;

import org.apache.log4j.Logger;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.instruction.InstructionMap;
import pl.edu.pw.pobicos.mw.instruction.ReturnValue;
import pl.edu.pw.pobicos.mw.vm.Primitives;
import pl.edu.pw.pobicos.mw.Conversions;

/**
 * Mediates between a specific virtual machine and POBICOS. Makes the AVR virtual machine an exchangeable module.
 * @author Micha³ Krzysztof Szczerbak
 */
public class PrimitivesAdapter {
	
	private final static Logger LOG = Logger.getLogger(PrimitivesAdapter.class);
	
	/**
	 * Executes a specific primitive by selecting a proper method which takes the necessary register values and passes them into a generic primitives processor.
	 * @see pl.edu.pw.pobicos.mw.vm.Primitives
	 * @param primitive name of a primitive
	 * @param agent micro-agent who calls primitive
	 * @param regs current state of registers
	 * @return value if primitive return one or special code
	 * @see pl.edu.pw.pobicos.mw.instruction.ReturnValue
	 */
	public static synchronized Object primitive_execute(String primitive, AbstractAgent agent, byte[] regs)
	{
		LOG.info(primitive + "_execute");
        for(Method m : PrimitivesAdapter.class.getDeclaredMethods())
        	if(m.getName().equals(primitive + "_execute"))
        	{
        		try{
        			Object[] os = new Object[2];
        			os[0] = agent;
        			os[1] = regs;
        			Object ret = m.invoke(null, os);
        			return ret;
        		}catch(Exception ex){
        			ex.printStackTrace();
        			try{
            			Object[] os = new Object[1];
            			os[0] = agent;
            			Object ret = m.invoke(null, os);
            			return ret;
        			}catch(Exception ex2){
        				LOG.error(ex2.getMessage()+";"+ex2.getClass().getName());
        				return new ReturnValue((short)-1);
        			}
        		}
        	}
        return Primitives.sendRequest(primitive, agent, getParams(primitive, regs, agent));
	}
	
	/**
	 * Gets instruction's parameters to pass to non generic resources.
	 * @param primitive name
	 * @param regs registers state
	 * @param agent micro-agent
	 * @return formatted parameters
	 */
	private static String getParams(String primitive, byte[] regs, AbstractAgent agent)
	{
		Vector<String> types = InstructionMap.getParams(InstructionMap.getCode(primitive));
		String params = "";
		int curReg = 24;
		for(String type : types)
		{
			if(type.equals(InstructionMap.UINT8))
				params += params.concat(String.valueOf(Conversions.byteToInt(curReg, 1, regs, true))) + " ";
			else if(type.equals(InstructionMap.UINT16))
				params += params.concat(String.valueOf(Conversions.byteToInt(curReg, 2, regs, true))) + " ";
			else if(type.equals(InstructionMap.UINT32))
			{
				curReg -= 2;
				params += params.concat(String.valueOf(Conversions.byteToInt(curReg, 4, regs, true))) + " ";
			}
			else if(type.equals(InstructionMap.UINT8P))
			{
				String msg = "\"";
				char c;
				int i = 0;
				while(1 == 1)
				{
		    		c = (char)agent.getData().get8((int)Conversions.byteToInt(curReg, 2, regs, true) + i - Properties.AVR_REG_IO_SIZE);
		    		if(c == 0)
		    			break;
		    		msg += new Character((char)c).toString();
		    		i++;
				}
				msg += "\"";
				params += msg;
			}
			curReg -= 2;
		}
		return params;
	}

	protected static ReturnValue EnableEvent_execute(AbstractAgent a, byte[] regs)
    {
    	//System.out.println("EnableEvent_execute on agent "+a.getName());
    	long eventType = Conversions.byteToInt(22, 4, regs, true);
    	return Primitives.EnableEvent_execute(a, eventType);
    }

    protected static ReturnValue DisableEvent_execute(AbstractAgent a, byte[] regs)
	{
		//System.out.println("DisableEvent_execute on agent "+a.getName());
		long eventType = Conversions.byteToInt(22, 4, regs, true);
    	return Primitives.DisableEvent_execute(a, eventType);
	}

    protected static ReturnValue CreateGenericAgent_execute(AbstractAgent a, byte[] regs)
    {
    	//System.out.println("CreateGenericAgent_execute");
    	long agentType = Conversions.byteToInt(22, 4, regs, true);
    	int timeout = (int)Conversions.byteToInt(20, 2, regs, true);
    	long reqHandle = Conversions.byteToInt(16, 4, regs, true);
    	return Primitives.CreateGenericAgent_execute(a, agentType, timeout, reqHandle);
    }

    protected static ReturnValue CreateNonGenericAgents_execute(AbstractAgent a, byte[] regs)
    {
    	//System.out.println("CreateNonGenericAgents_execute");
    	long agentType = Conversions.byteToInt(22, 4, regs, true);
    	int pointer = (int)Conversions.byteToInt(20, 2, regs, true);
    	short number = (short)Conversions.byteToInt(18, 1, regs, true);
    	int timeout = (int)Conversions.byteToInt(16, 2, regs, true);
    	long reqHandle = Conversions.byteToInt(12, 4, regs, true);
    	return Primitives.CreateNonGenericAgents_execute(a, agentType, pointer, number, timeout, reqHandle);
    }
    
    protected static long GetMyID_execute(AbstractAgent a)
    {
    	return Primitives.GetMyID_execute(a);
    }
    
    protected static ReturnValue GetChildInfo_execute(AbstractAgent a, byte[] regs)
    {
    	int pid = (int)Conversions.byteToInt(24, 2, regs, true);
    	int ptp = (int)Conversions.byteToInt(22, 2, regs, true);
    	int prh = (int)Conversions.byteToInt(20, 2, regs, true);
    	return Primitives.GetChildInfo_execute(a, pid, ptp, prh);
    }
    
    protected static ReturnValue Release_execute(AbstractAgent a, byte[] regs)
    {
    	long agentId = Conversions.byteToInt(22, 4, regs, true);
    	return Primitives.Release_execute(a, agentId);
    }
    
    protected static ReturnValue SendCommand_execute(AbstractAgent a, byte[] regs)
    {
    	long agentId = Conversions.byteToInt(22, 4, regs, true);
    	int pointer = (int)Conversions.byteToInt(20, 2, regs, true);
    	short reliable = (short)Conversions.byteToInt(18, 1, regs, true);
    	return Primitives.SendCommand_execute(a, agentId, pointer, reliable);
    }
    
    protected static ReturnValue GetCommand_execute(AbstractAgent a, byte[] regs)
    {
    	int pointer = (int)Conversions.byteToInt(24, 2, regs, true);
    	return Primitives.GetCommand_execute(a, pointer);
    }
    
    protected static ReturnValue SendReport_execute(AbstractAgent a, byte[] regs)
    {
    	int pointer = (int)Conversions.byteToInt(24, 2, regs, true);
    	short reliable = (short)Conversions.byteToInt(22, 1, regs, true);
    	return Primitives.SendReport_execute(a, pointer, reliable);
    }
    
    protected static ReturnValue GetReport_execute(AbstractAgent a, byte[] regs)
    {
    	int pointer = (int)Conversions.byteToInt(24, 2, regs, true);
    	return Primitives.GetReport_execute(a, pointer);
    }
    
    protected static ReturnValue CreateReportList_execute(AbstractAgent a, byte[] regs)
    {
    	long agentType = Conversions.byteToInt(22, 4, regs, true);
    	int pointer = (int)Conversions.byteToInt(20, 2, regs, true);
    	return Primitives.CreateReportList_execute(a, agentType, pointer);
    }
    
    protected static ReturnValue DestroyReportList_execute(AbstractAgent a, byte[] regs)
    {
    	short id = (short)Conversions.byteToInt(24, 1, regs, true);
    	return Primitives.DestroyReportList_execute(a, id);
    }
    
    protected static ReturnValue AddReport_execute(AbstractAgent a, byte[] regs)
    {
    	short rid = (short)Conversions.byteToInt(24, 1, regs, true);
    	long agentId = Conversions.byteToInt(20, 4, regs, true);
    	int pointer = (int)Conversions.byteToInt(18, 2, regs, true);
    	return Primitives.AddReport_execute(a, rid, agentId, pointer);
    }
    
    protected static ReturnValue GetNextReport_execute(AbstractAgent a, byte[] regs)
    {
    	short id = (short)Conversions.byteToInt(24, 1, regs, true);
    	int pid = (int)Conversions.byteToInt(22, 2, regs, true);
    	int pmsg = (int)Conversions.byteToInt(20, 2, regs, true);
    	return Primitives.GetNextReport_execute(a, id, pid, pmsg);
    }
    
    protected static ReturnValue GetConfigSetting_execute(AbstractAgent a, byte[] regs)
    {
    	short id = (short)Conversions.byteToInt(24, 1, regs, true);
    	int pointer = (short)Conversions.byteToInt(22, 2, regs, true);
    	short size = (short)Conversions.byteToInt(20, 1, regs, true);
    	return Primitives.GetConfigSetting_execute(a, id, pointer, size);
    }
    
    protected static ReturnValue SetTimer_execute(AbstractAgent a, byte[] regs)
    {
    	short tid = (short)Conversions.byteToInt(24, 1, regs, true);
    	long time = Conversions.byteToInt(20, 4, regs, true);
    	return Primitives.SetTimer_execute(a, tid, time);
    }
    
    protected static ReturnValue GetTimerId_execute(AbstractAgent a, byte[] regs)
    {
    	int pointer = (int)Conversions.byteToInt(24, 2, regs, true);
    	return Primitives.GetTimerId_execute(a, pointer);
    }
    
    protected static ReturnValue DbgString_execute(AbstractAgent a, byte[] regs)
    {
    	int pointer = (int)Conversions.byteToInt(24, 2, regs, true);
    	return Primitives.DbgString_execute(a, pointer);
    }
    
    protected static ReturnValue DbgUInt32_execute(AbstractAgent a, byte[] regs)
    {
    	long integer = Conversions.byteToInt(22, 4, regs, true);
    	return Primitives.DbgUInt32_execute(a, integer);
    }
}
