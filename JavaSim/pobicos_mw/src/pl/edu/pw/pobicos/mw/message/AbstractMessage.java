package pl.edu.pw.pobicos.mw.message;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;

/**
 * This class defines messages (reports and commands) hierarchy in the ROVERS system.
 * 
 *
 * @author Marcin Smialek
 * @created 2006-10-16 16:30:28
 */
public abstract class AbstractMessage {

	protected AbstractAgent sender, recipient;
	
	protected Message message;//POBICOS Msg
	
	protected long virtualTime;
	
	public Message getMessage()
	{
		return message;
	}

    /**
     * Returns ID of micro-agent, which is recipient of the this message.  
     * 
     * @return ID of this message recipient micro-agent
     */
    public AbstractAgent getRecipient() {
        return recipient;
    }

    /**
     * Returns ID of micro-agent, which is the sender of this message.  
     * 
     * @return ID of this message sender micro-agent
     */
    public AbstractAgent getSender() {
        return sender;
    }
    
    public long getVirtualTime()
    {
    	return virtualTime;
    }
}
