package pl.edu.pw.pobicos.mw.message;

import java.util.Vector;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;

public class ReportList {
	private long agentType;
	private short id;//POBICOS ReportListID
	private static short count = 0;
	private Vector<Report> reports = new Vector<Report>();
	
	public ReportList(long agentType)
	{
		this.agentType = agentType;
		this.id = count++;
	}
	
	public short getId()
	{
		return id;
	}
	
	public long getType()
	{
		return agentType;
	}
	
	public void addReport(AbstractAgent sender, AbstractAgent recipient, String message, long virtualTime)
	{
		reports.add(new Report(sender, recipient, message, virtualTime));
	}
	
	public Report getNextReport()
	{
		if(reports.size() == 0)
			return null;
		Report temp = reports.get(0);
		reports.remove(0);
		return temp;
	}
}
