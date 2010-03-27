package pl.edu.pw.pobicos.mw.logging;

import java.io.File;
import java.math.BigInteger;
import java.util.Vector;

import org.eclipse.core.runtime.ListenerList;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.xml.TraceDocument;
import pl.edu.pw.pobicos.xml.TraceDocument.Trace.Event;

public class Trace {
	
	public static final String INSTRUCTIONSENT = "Instruction sent";
	public static final String INSTRUCTIONRESP = "Instruction response";
	public static final String PRIMITIVECALL = "Primitive called";
	public static final String AGENTREG = "Agent registered";
	public static final String AGENTREP = "Agent moved";
	public static final String AGENTREM = "Agent removed";
	public static final String NODEADD = "Node added";
	public static final String NODEREM = "Node removed";
	public static final String NODECHANGE = "Node changed";
	public static final String HANDLE = "Handling event";
	public static final String COMMAND = "Command sent";
	public static final String REPORT = "Report sent";
	public static final String COMMANDLOST = "Command lost";
	public static final String REPORTLOST = "Report lost";
	
	private static Vector<TraceElement> events = new Vector<TraceElement>();
	private static ListenerList listeners = new ListenerList();
	
	public static Vector<TraceElement> getEvents()
	{
		return events;
	}
	
	public static void trace(String message, AbstractNode node, AbstractAgent agent, TraceData data)
	{
		long time = SimulationsManager.getInstance().getSimulation().getVirtualTime();
		events.add(0, new TraceElement(message, node, agent, time, data));
		fireTraceChanged();
	}
	
	public static void saveTrace(String fileName)
	{
		TraceDocument traceDoc = TraceDocument.Factory.newInstance();
		//String traceXML = "<trace>\n";
		for(int i = events.size() - 1; i >= 0; i--)
		{
			TraceElement element = events.get(i);
			//traceXML += "\t<event message=\"" + element.getMessage() + "\" node=\"" + (element.getNode() != null ? element.getNode().getId() : "") + "\" agent=\"" + (element.getAgent() != null ? element.getAgent().getId() : "") + "\" time=\"" + element.getTime() + "\"" + (element.getData() == null ? "" : " data=\"" + element.getData().toString() + "\"") + "/>\n";
			//xmlbeans
			Event event = traceDoc.addNewTrace().addNewEvent();
			if(element.getAgent() != null)
				event.setAgent((int)element.getAgent().getId());
			if(element.getData() != null)
				event.setData(element.getData().toString());
			event.setMessage(element.getMessage());
			if(element.getNode() != null)
				event.setNode((int)element.getNode().getId());
			event.setTime(BigInteger.valueOf(element.getTime()));
			System.out.println(element.getMessage());
		}
		//traceXML += "</trace>";
		//FIXME unwanted <xml-fragment>
		try {
			traceDoc.save(new File(fileName));
		} catch (Exception e) {}
		//writeToFile(fileName, traceXML, false);
	}
	
	/*public static void writeToFile(String fileName, String message, boolean append)
	{
		String oldData = "";
		if(append)
		{
			FileInputStream fis = null;
		    DataInputStream in = null;
			BufferedReader br = null;
			try {
				fis = new FileInputStream(fileName);
				in = new DataInputStream(fis);
				br = new BufferedReader(new InputStreamReader(in));
				String tmp;
				while ((tmp = br.readLine()) != null) 
					oldData += tmp + "\n";
				fis.close();	
				in.close();
			} catch (Exception e) {}
		}
		try{
			FileWriter fstream = new FileWriter(fileName);
		    BufferedWriter out = new BufferedWriter(fstream);
		    out.write(oldData + message);
		    out.close();
	    }catch (Exception e){}
	}*/
	
	public static void addTraceListener(ITraceListener listener) {
		listeners.add(listener);
	}

	public static void removeTraceListener(ITraceListener listener) {
		listeners.remove(listener);
	}

	public static void fireTraceChanged() {
		final ListenerList tmpListeners = listeners;
    	Display.getDefault().asyncExec(new Runnable() {
			public void run() {
		    	try {
		    		Object[] ls = tmpListeners.getListeners();
		    		for (int i = 0; i < ls.length; i++) {
		    			ITraceListener listener = (ITraceListener) ls[i];
		    			listener.traceChanged();
		    		}
				} catch (Exception e) {
				}
			}
		});
	}
}
