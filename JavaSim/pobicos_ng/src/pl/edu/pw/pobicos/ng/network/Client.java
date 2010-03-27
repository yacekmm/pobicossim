package pl.edu.pw.pobicos.ng.network;

import java.io.*;
import java.net.Socket;
import java.util.Properties;
import java.util.StringTokenizer;
import java.lang.Long;

import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.ng.Conversions;
import pl.edu.pw.pobicos.ng.instruction.InstructionMap;
import pl.edu.pw.pobicos.ng.logging.Log;
import pl.edu.pw.pobicos.ng.product.ProductManager;
import pl.edu.pw.pobicos.ng.product.ProductMap;
import pl.edu.pw.pobicos.ng.product.SensorValue;
//import pl.edu.pw.pobicos.ng.view.LogView;
import pl.edu.pw.pobicos.ng.view.LogView;
import pl.edu.pw.pobicos.ng.view.NetworkViewManager;
import pl.edu.pw.pobicos.ng.view.action.ActionContainer;

public class Client implements Runnable {
	private static final long serialVersionUID = 777;
	private Socket socket;
	private BufferedReader input;
	private PrintWriter output;
	private boolean goon = false;
	private static Client instance;
	private static long eventNumber = 0;
	
	public static Client getInstance()
	{
		if(instance == null)
			instance = new Client();
		return instance;
	}
	
	public Client()
	{	//empty
	}
	
	public void init(Properties props) {
		LogView.getInstance().clear();
        goon = true;
		String adres = props.getProperty("ip");
        int port = Integer.parseInt(props.getProperty("port"));
		try
		{
			socket = new Socket(adres, port);
		} 
		catch (Exception e) 
		{ 
			Log.error("Unable to connect to server at " + adres + ":" + port); 
			goon = false; 
			return;
		} 
		try {
			input = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			output = new PrintWriter(socket.getOutputStream(), true);
		} 
		catch (Exception e)
		{ return; }

        new Thread(this).start();
        
		send(Protocol.CONNECT + Protocol.div + "OBJECT" + Protocol.div + props.getProperty("desc"));
	}
	
	public void disconnect()
	{
		Display.getDefault().asyncExec(new Runnable(){
			public void run()
			{
				send(Protocol.DISCONNECT);
		        goon = false;
		        Log.info("---END\n");
				ActionContainer.getAction("ConnectAction").setChecked(false);
				ActionContainer.getAction("ConnectAction").setEnabled(true);
				ProductManager.getInstance().clear();
				NetworkViewManager.getInstance().clear();
			}
		});
	}
	
	public void stop()
	{
		send(Protocol.DISCONNECT);
	}
    
    public void run()
    {
    	while(goon)
    	{
    	    try {
            	String command = input.readLine();
            	Log.info("RCV: "+command+"\n");
            	if (!handleCommand(command)) {
            		Log.info(" ...ERROR\n");
                	    output.close();
                	    input.close();
                 	    socket.close();
                	    break;
            	}
        		try {Thread.sleep(500);} catch (InterruptedException e) {}
        	    } catch(Exception ex){}
    	}
    	output = null;
    	input = null;
    	synchronized (this) {
    		socket = null;
    	}
    }
	
    public boolean handleCommand(String command) {
        final StringTokenizer st = new StringTokenizer(command, String.valueOf(Protocol.div));
        String cd = st.nextToken();
        //if (cd.equals(Protocol.STOP)) {
        //	disconnect();
        //}
        if (cd.equals(Protocol.HELLO)) {
	    	final long id = Long.parseLong(st.nextToken());
	    	String nodeDef = st.nextToken();
        	ProductManager.getInstance().addProduct(id, nodeDef);
		    NetworkViewManager.getInstance().paint();
        }
        else if (cd.equals(Protocol.LINK_STATUS)) {
        	String status = st.nextToken();
        	if(status.equalsIgnoreCase(Protocol.OFF))
        	{
	        	try{
	        		int id = Integer.parseInt(st.nextToken());
	        		ProductManager.getInstance().removeProduct(id);
	        		NetworkViewManager.getInstance().paint();
	        	}
	        	catch (Exception e)
	        	{
	        		disconnect();
	        	}
        	}
        	else if (status.equalsIgnoreCase(Protocol.ON))
        	{        		
    	    	//enter code here...
        	}
        }
        else if (cd.equals(Protocol.INSTR)) {//parameteres separated with space - not ;
        	final String call_id = st.nextToken();
        	String code = st.nextToken();
        	Log.info("       Object #" + call_id + " handling instruction " + Conversions.longToHexString(Long.parseLong(code), 4) + " " + st.nextToken() + "\n");
        	if(InstructionMap.getReturn(Long.parseLong(code)) != null)
        	{
        		/*Display.getDefault().asyncExec(new Runnable(){

					public void run() 
					{
		        		InstructionReturnDialog d = new InstructionReturnDialog(Display.getDefault().getActiveShell());
		        		int button = d.open();
		        		if (button == Window.OK)
		        			send(Protocol.RETURN + Protocol.div + id + Protocol.div + d.getRetValue());
					}
        			
        		});*/
        		Object toReturn = null;
        		for(ProductMap product : ProductManager.getInstance().getProducts())
        			if(product.getId() == Long.parseLong(call_id.substring(0, call_id.indexOf("#"))))
	        			for(SensorValue sens : product.getSensors())
			        		toReturn = sens.getValue();
        		if(toReturn != null)
        			send(Protocol.INSTR_RETURN + Protocol.div + call_id + Protocol.div + String.valueOf(toReturn));
        	}
        }
        else if (cd.equals(Protocol.EVENT_RETURN)) 
        {
        	final String call_id = st.nextToken();
        	Log.info("       EventReturn: #" + call_id + ": " + st.nextToken() + "\n");
        }
        return true;
    }
    
    private void send(String command){
    	try{
    		Log.info("SND: "+command);
    	}catch(Exception e){}
    	try
    	{
	        if(socket!=null)
	          	Log.info("\n");
	        else
	           	Log.info(" ...ERROR\n");
    	}catch(Exception e){}
    	if(socket!=null&&output!=null&&command!=null&&command!="\n")
    	    output.println(command);
        }

	public void notifyEvent(long id, String name, String params) {
		long thisEventNumber = eventNumber++;
		if (eventNumber> 1000000)
			eventNumber=0;
		send(Protocol.EVENT + Protocol.div + id + "#" + thisEventNumber + Protocol.div + name + Protocol.div + "(" + params + ")");
	}

	public boolean isRunning()
	{
		return goon;
	}
}
