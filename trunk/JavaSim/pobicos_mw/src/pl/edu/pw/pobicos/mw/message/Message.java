package pl.edu.pw.pobicos.mw.message;

public class Message {
	public static final short MSG_DATA_MAXLEN = 177;

	short length;
	char msg[];
	
	public Message(short length, char msg[])
	{
		this.length = length;
		this.msg = msg;
	}
	
	public String toString()
	{
		String temp = "";
		for(int i = 0; i < length; i++)
			temp += msg[i];
		return temp;
	}

	public short getLength() 
	{
		return length;
	}

	public short getCharAt(short i) 
	{
		return (short)msg[i];
	}
}
