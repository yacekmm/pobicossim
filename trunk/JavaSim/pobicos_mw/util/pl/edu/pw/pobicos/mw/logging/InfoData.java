package pl.edu.pw.pobicos.mw.logging;

public class InfoData extends TraceData {
	private String info;
	
	public InfoData(String info)
	{
		this.info = info;
	}

	@Override
	public String toString() 
	{
		return info;
	}

}
