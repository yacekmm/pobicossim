package pl.edu.pw.pobicos.mw.network;

import java.io.*;
import java.net.Socket;
import java.util.Properties;
import java.util.StringTokenizer;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.*;
import org.xml.sax.InputSource;

//import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.NonGenericNode;
//import pl.edu.pw.pobicos.mw.view.ConnectionView;
import pl.edu.pw.pobicos.mw.vm.Primitives;
import pl.edu.pw.pobicos.mw.network.Protocol;
//import pl.edu.pw.pobicos.ng.product.ProductManager;
//import pl.edu.pw.pobicos.ng.view.NetworkViewManager;

public class Client implements Runnable {
	
	private static final long serialVersionUID = 777;
	private Socket socket;
	private BufferedReader input;
	private PrintWriter output;
	private boolean goon = false;
	private static Client instance;
	
	private Client()
	{	//empty
	}
	
	public static Client getInstance()
	{
		if(instance == null)
			instance = new Client();
		return instance;
	}
	
	public boolean init(Properties props) {
		//LogView.getInstance().clear();
        goon = true;
		String adres = props.getProperty("ip");
        int port = Integer.parseInt(props.getProperty("port"));
		try
		{
			socket = new Socket(adres, port);
		} catch (Exception e) { 
			//Log.error("Unable to connect to server at " + adres + ":" + port); 
			goon = false; 
			return false;
		} 
		try {
			input = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			output = new PrintWriter(socket.getOutputStream(), true);
		} catch (Exception e){ return false; }

        new Thread(this).start();
        
		//send(Protocol.CONNECT + Protocol.div + "OBJECT");
		return true;
	}
	
	public void disconnect()
	{
		try
		{
			for(AbstractNode node : NodesManager.getInstance().getNodes())
				{
					oldNode(node);
					//System.out.println("Od��czam node: " + node.getId());
				}
		
			send(Protocol.DISCONNECT);
		}
		catch(Exception e)
		{}
        goon = false;
        //Log.append("Ko�cz� prac�...\n");
	}
    
    public void run()
    {
    	while(goon)
    	{
    	    try {
            	String command = input.readLine();
            	//Log.append("dosta�em: "+command+"\n");
            	if (!handleCommand(command)) {
            		//Log.append("nie poradzi�em sobie\n");
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
	
    public boolean handleCommand(String command) 
    {
        StringTokenizer st = new StringTokenizer(command, String.valueOf(Protocol.div));
        String cd = st.nextToken();
        //if (cd.equals(Protocol.STOP))
        if (cd.equals(Protocol.LINK_STATUS))
        {
        	String status = st.nextToken();
        	if(status.equalsIgnoreCase(Protocol.OFF))
        	{
        		//enter code here...
//        		try{
//	        		int id = Integer.parseInt(st.nextToken());
//	        		//disconnect node with 'id'
//	        	}
//	        	catch (Exception e)
//	        	{
//	        		disconnect();
//		        	Display.getDefault().asyncExec(new Runnable(){
//		        		public void run()
//		        		{
//		        			ConnectionView.disconnect();
//		        		}
//		        	});
//	        	}
        	}
        	else if (status.equalsIgnoreCase(Protocol.ON))
        	{        		
    	    	//enter code here...
        	}
        }
//        else if (cd.equals(Protocol.DESCRIBE)) 
//        {
//	    	for(AbstractNode node : NodesManager.getInstance().getNodes())
//	    		if(node.getClass().equals(pl.edu.pw.pobicos.mw.node.NonGenericNode.class))
//	    			newNode((NonGenericNode)node);
//        }
        else if (cd.equals(Protocol.EVENT)) 
        {
        	String callID = st.nextToken();
    		long id = Long.parseLong(callID.substring(0, callID.indexOf("#")));
    		
	    	for(AbstractNode node : NodesManager.getInstance().getNodes())
	    		if(node.getId() == id)
	    			PobicosManager.getInstance().addPhysicalEvent(callID, node, st.nextToken(), st.nextToken());
	    	//send null event_Return
	    	send(Protocol.EVENT_RETURN + Protocol.div + callID + Protocol.div + "()");
        }
        else if (cd.equals(Protocol.INSTR_RETURN)) 
        {
	    	Primitives.respond(st.nextToken(), st.nextToken());
        }
        return true;
    }
    
    private void send(String command)
    {
    	//Log.append("moj kom: "+command);
    	if(socket!=null)
    		;//Log.append(" ...wysy�am\n");
    	else
    		;//Log.append(" ...wtyczka od��czona\n");
    	if(socket!=null&&output!=null&&command!=null&&command!="\n")
    	    output.println(command);
    }
	
	public void newNode(NonGenericNode node)
	{
		//send(Protocol.HELLO + Protocol.div + node.getId() + Protocol.div + node.getNodeDef().replaceAll("\n", ""));

		//extract instruction labels:
		String instructions = extractObjDescription(node.getNodeDef().replaceAll("\n", ""), "instruction");
		String events = extractObjDescription(node.getNodeDef().replaceAll("\n", ""), "event");
		
		send(Protocol.CONNECT + Protocol.div + "NODE" + Protocol.div + node.getId() + Protocol.div + instructions + events);
	}
	
	public String extractObjDescription(String nodeDef, String tag)
	{
		String resDesc = "";
		String separator = ";";
		
		try
		{
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder db = dbf.newDocumentBuilder();
			InputSource is = new InputSource();
			is.setCharacterStream(new StringReader(nodeDef));
			
			Document doc = db.parse(is);
			NodeList nodes = doc.getElementsByTagName(tag);
			
			//iterate by tag
			for(int i=0; i< nodes.getLength(); i++)
			{
				Element element = (Element)nodes.item(i);
				
				NodeList name = element.getElementsByTagName("name");
				Element line = (Element)name.item(0);
				resDesc+= getCharacterDataFromElement(line) + separator;
			}
			
		}
		catch(Exception e)
		{}
		
		
		return resDesc;
	}
	
	public String getCharacterDataFromElement(Element e)
	{
		Node child = e.getFirstChild();
		if(child instanceof CharacterData)
		{
			CharacterData cd = (CharacterData)child;
			return cd.getData();
		}
		return "";
	}
	
	public void sendInstruction(String id, long code, String params)
	{
		send(Protocol.INSTR + Protocol.div + id + Protocol.div + code + Protocol.div + "(" + params + ")");
	}

	public void changeNodeType(NonGenericNode node) 
	{
		//send(Protocol.BYE + Protocol.div + node.getId());
		send(Protocol.DISCONNECT + Protocol.div + node.getId());
		//send(Protocol.HELLO + Protocol.div + node.getId() + Protocol.div + node.getNodeDef());
		send(Protocol.CONNECT + Protocol.div + "NODE" + node.getId() + Protocol.div + node.getNodeDef().replaceAll("\n", ""));
	}

	public void oldNode(AbstractNode node) 
	{
		//send(Protocol.BYE + Protocol.div + node.getId());
		send(Protocol.DISCONNECT + Protocol.div + node.getId());
	}
	
	public boolean isRunning()
	{
		return goon;
	}
}