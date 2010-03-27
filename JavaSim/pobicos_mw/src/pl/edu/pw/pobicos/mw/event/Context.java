package pl.edu.pw.pobicos.mw.event;

public class Context {

	public static enum ContextType { child, message, timer };
	
	public ContextType type;
	
	public long childId, childType, childRequestHandle;//POBICOS RequestHandle
	public String messageMsg;
	public short timerId;//POBICOS TimerId
	
	public Context(long childId, long childType, long childRequestHandle)
	{
		this.childId = childId;
		this.childType = childType;
		this.childRequestHandle = childRequestHandle;
		this.type = ContextType.child;
	}
	
	public Context(String messageMsg)
	{
		this.messageMsg = messageMsg;
		this.type = ContextType.message;
	}
	
	public Context(short timerId)
	{
		this.timerId = timerId;
		this.type = ContextType.timer;
	}
}
