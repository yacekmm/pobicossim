package pl.edu.pw.pobicos.mw.instruction;

public class ReturnValue {
	public static final short RETCODE_OK = 0;
	public static final short RETCODE_NORES = 1;
	public static final short RETCODE_BADARGS = 2;
	public static final short RETCODE_NODATA = 3;
	
	private short returnValue;//POBICOS RetCode
	
	public ReturnValue(short value)
	{
		returnValue = value;
	}
	
	public long getReturnValue()
	{
		return returnValue;
	}
}
