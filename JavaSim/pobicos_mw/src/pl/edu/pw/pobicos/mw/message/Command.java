package pl.edu.pw.pobicos.mw.message;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;

public class Command extends AbstractMessage {

	public Command(AbstractAgent sender, AbstractAgent recipient, String msg, long virtualTime)
	{
		this.message = new Message((short)msg.length(), msg.toCharArray());
		this.sender = sender;
		this.recipient = recipient;
		this.virtualTime = virtualTime;
	}
}
