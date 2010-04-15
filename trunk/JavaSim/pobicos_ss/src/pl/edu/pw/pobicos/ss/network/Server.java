package pl.edu.pw.pobicos.ss.network;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.awt.Button;
import java.awt.Frame;
import java.awt.TextArea;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Vector;
import java.util.Enumeration;
import java.util.StringTokenizer;
import java.util.regex.Pattern;
import java.lang.Long;
import javax.swing.BoxLayout;
//import javax.xml.parsers.DocumentBuilder;
//import javax.xml.parsers.DocumentBuilderFactory;

//import org.w3c.dom.Document;
//import org.w3c.dom.Node;

/**
 * Manages server services.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Server extends Frame implements Runnable{
	private static final long serialVersionUID = 777;
	private Thread thread = null;
	private static Server instance;
	private ServerSocket serverSocket;
	private int nofclients = 0;
	protected TextArea log;
    private Vector<ServerService> clients = new Vector<ServerService>();
    private Vector<ServerService> sims = new Vector<ServerService>();
    private Vector<ServerService> nodeprvdr = new Vector<ServerService>();
    private enum NodeType {none, node, sim};
    private NodeType nodeType = NodeType.none;    
    private class NodeMap
	{
		ServerService nodeService;
		long nodeId;
		String nodeDef, name;
		public NodeMap(ServerService nodeService, long id, String nodeDef)
		{
			this(nodeService, nodeDef);
			nodeId = id;
		}
		
		public NodeMap(ServerService nodeService, String nodeDef)
		{
			this.nodeService = nodeService;
			this.nodeDef = nodeDef;
			this.name = nodeDef;
			//this.name = "?";
/*			try {//parsing the xmls
				try{
					DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
					DocumentBuilder db = dbf.newDocumentBuilder();
					StringBuffer sb = new StringBuffer(nodeDef);
					ByteArrayInputStream bis = new ByteArrayInputStream(sb.toString().getBytes());
					Document doc = db.parse(bis);
					doc.getDocumentElement().normalize();
					Node root = doc.getDocumentElement();
					for(int i = 0; i < root.getChildNodes().getLength(); i++)
					{
						Node node = root.getChildNodes().item(i);
						if(node.getNodeType() == Node.ELEMENT_NODE)
							if(node.getNodeName().equals("name"))
							{
								this.name = node.getChildNodes().item(0).getNodeValue();
								break;
							}
					}
				}catch(Exception e){}
			}catch(Exception e){}
*/			nodeId = nodeService.getId();
		}

		public long getNodeId() {
			return nodeId;
		}
	}	
	private Vector<NodeMap> nodeMap = new Vector<NodeMap>();
    private class BidMap
	{
		ServerService sservice;
		long id;
		public BidMap(ServerService sservice, long id)
		{
			this.sservice = sservice;
			this.id = id;
		}
	}	
	private Vector<BidMap> bidMap = new Vector<BidMap>();
	private class SimMap
	{
		ServerService sservice;
		String desc;
		public SimMap(ServerService sservice, String desc)
		{
			this.sservice = sservice;
			this.desc = desc;
		}
	}	
	private Vector<SimMap> simMap = new Vector<SimMap>();
	private boolean goon = false;

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
    public static Server getInstance()
    {
    	if(instance == null)
    		instance = new Server();
		return instance;
    }
	
	private Server() {//empty
	}
	
	private void init(long port)
	{
        try {
            serverSocket = new ServerSocket((int)port);
        } catch (IOException e) {
            System.err.println("B³¹d wczytu serwera.");
            System.exit(1);
        }
        
        Button b = new Button("Zakoñcz");
        this.setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));
        add(b);
        log = new TextArea(27,47);
        add(log);
        
        thread = new Thread(this);
        thread.start();
        
        b.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent ae) {
                //send(Protocol.STOP);
            	send(Protocol.LINK_STATUS + Protocol.div + Protocol.OFF);
                log.append("---END\n");
                while (clients.size() != 0)
                    try {
                        Thread.sleep(500);
                    } catch (InterruptedException e) {
                    }
                System.exit(0);
            }
        }); 
	}

	/**
	 * Starts the server.
	 * @param port port for listening
	 */
	public void start(long port)
	{
		goon  = true;
		getInstance().init(port);
	}
	
	/**
	 * Stops the server.
	 */
	public void stop()
	{
        //send(Protocol.STOP);
		send(Protocol.LINK_STATUS + Protocol.div + Protocol.OFF);
        //log.append("Koñczê pracê...\n");
        try
        {
	        while (clients.size() != 0)
	            try {
	                Thread.sleep(500);
	            } catch (InterruptedException e) {
	            }
	        log.append("---END\n");
			goon = false;
        }finally
        {
			try
			{
				serverSocket.close();
			}catch(Exception ex){}
        }
	}
	
	/**
	 * Stops the server on exit.
	 */
	public void forceStop()
	{
		//send(Protocol.STOP);
		send(Protocol.LINK_STATUS + Protocol.div + Protocol.OFF);
		try
		{
			serverSocket.close();
		}catch(Exception ex){}
	}

    /* (non-Javadoc)
     * @see java.lang.Runnable#run()
     */
    public void run() {
        while (goon){
            try {
                Socket clientSocket = serverSocket.accept();
                ServerService clientService = new ServerService(clientSocket, nofclients++);
                addClientService(clientService);
                try {Thread.sleep(500);} catch (InterruptedException e) {}
            } catch (IOException e) {
                System.err.println("B³ad przyjmowania klienta. Klient nie bêdzie obs³u¿ony...");
            }
        }
    }
	
    /**
     * Parses messages received from clients. Invokes private methods for creating or removing nodes and simulators.
     * @param command string to parse
     * @param cid client's id
     * @return true if parsing succeeded
     */
    protected boolean handleCommand(String command, int cid) {
    	log.append("RCV: "+command+"\n");
        StringTokenizer st = new StringTokenizer(command, String.valueOf(Protocol.div));
        String cd = st.nextToken();
        if (cd.equals(Protocol.CONNECT)) {
        	String tmpToken = st.nextToken();
 //           if(tmpToken.equals("SIM"))
//            	newSimulator(Integer.parseInt(st.nextToken()), String.valueOf(cid));
 //           	newSimulator(cid, st.nextToken());
//            else 
            if(tmpToken.equals("OBJECT"))
            {
            	String id = st.nextToken();
            	String desc = st.nextToken();
            	newNodeProvider(cid, id, desc);
            	
            	findNodeToBid(cid, Long.parseLong(id), desc);
            	//newObject(cid, st.nextToken());
            }
            else if(tmpToken.equals("NODE"))
            {
            	String token = st.nextToken();
	        	try
	        	{
	        		long id = Long.parseLong(token);
	        		String desc = st.nextToken();
	                newNode(cid, id, desc);
	                findObjectToBid(cid, id, desc);
	        	}catch(Exception e)
	        	{
	                newNode(cid, token);
	        	}
	        }
        }
        else if (cd.equals(Protocol.DISCONNECT)) {
        	try
        	{
      		Long id = Long.parseLong(st.nextToken());
        		oldNode(cid, id);
        	}
        	catch (Exception e)
        	{
        		old(cid);
        	}
        }
//        else if (cd.equals(Protocol.HELLO)) {
//        	String token = st.nextToken();
//        	try
//        	{
//        		long id = Long.parseLong(token);
//                newNode(cid, id, st.nextToken());
//        	}catch(Exception e)
//        	{
//                newNode(cid, token);
//        	}
//        }
//        else if (cd.equals(Protocol.BYE)) {
//            oldNode(cid, Long.parseLong(st.nextToken()));
//        }
        else if (cd.equals(Protocol.EVENT)) {
        	event(cid, st.nextToken(), st.nextToken(), st.nextToken());
        }
        else if (cd.equals(Protocol.INSTR)) {
        	if(nodeType.equals(NodeType.sim))
        		instr(cid, st.nextToken(), st.nextToken(), st.nextToken());
        	else
        		instr(cid, cid, st.nextToken(), st.nextToken());
        }
        else if (cd.equals(Protocol.INSTR_RETURN)) {
            ret_instr(cid, st.nextToken(), st.nextToken());
        }
        else if (cd.equals(Protocol.EVENT_RETURN)) {
            ret_event(cid, st.nextToken(), st.nextToken());
        }
        return true;
    }

	private void findObjectToBid(int cid, long id, String desc) 
	{    	
		String myDesc[] = parseHeaders(desc);
		boolean preventBiding;
		
		for(SimMap smap : simMap)
		{
			preventBiding = false;
	    	for(BidMap bmap : bidMap)
	    		if(smap.sservice.getId() == bmap.sservice.getId())
	    			preventBiding = true;
	    	
	    	if(!preventBiding)
	    	{
	    		String simDesc[] = parseHeaders(smap.desc);
	    		if(compareDesc(simDesc, myDesc))
	    		{
	    			newBid(smap.sservice.getId(), id);
	    			return;
	    		}
	    	}
		}
	}

	private void findNodeToBid(int cid, long id, String desc) 
	{
		String myDesc[] = parseHeaders(desc);

		for(NodeMap nmap : nodeMap)
		{
			String simDesc[] = parseHeaders(nmap.nodeDef);
			if(compareDesc(simDesc, myDesc))
			{
				if(bidMap.size() == 0)
				{
					newBid(cid, nmap.nodeId);
					return;
				}

				//for(BidMap bmap : bidMap)
				//	if(bmap.id != nmap.nodeId)
				if(newBid(cid, nmap.nodeId).equals("bid done"))
					return;
					
//				newBid(cid, nmap.nodeId);
//				return;
			}
		}
	}
	
	private boolean compareDesc(String[] simDesc, String[] myDesc) 
	{
		if(simDesc.length == myDesc.length)
		{
			List<String> simList = Arrays.asList(simDesc);
			List<String> myList = Arrays.asList(myDesc);
			Collections.sort(simList);
			Collections.sort(myList);
			
			for(int i = 0; i < simList.size(); i++)
				if(!simList.get(i).equalsIgnoreCase(myList.get(i)))
					return false;
			return true;
		}
		return false;
	}

	private String[] parseHeaders(String desc) {
		String regex = ";";
		Pattern p = Pattern.compile(regex);
		return p.split(desc.substring(0, desc.length()-1));
	}

	/**
	 * Sends messages to all clients.
	 * @param command string to send
	 */
	protected void send(String command){
        Enumeration<ServerService> e = clients.elements();
        while (e.hasMoreElements())
            ((ServerService) e.nextElement()).send(command);
        }

	/**
	 * Sends messages to all simulators.
	 * @param command string to send
	 */
    protected void sendToSim(String command){
        Enumeration<ServerService> e = sims.elements();
        while (e.hasMoreElements())
            ((ServerService) e.nextElement()).send(command);
        }

	/**
	 * Sends messages to all nodes.
	 * @param command string to send
	 */
    protected void sendToNode(String command){
        Enumeration<ServerService> e = nodeprvdr.elements();
        while (e.hasMoreElements())
            ((ServerService) e.nextElement()).send(command);
        }

	/**
	 * Sends messages to client of a specified id.
	 * @param cid client's id
	 * @param command string to send
	 */
    protected void sendToId(int cid, String command){
    	ServerService temp = getServiceById(cid);
    	if(temp != null)
    		temp.send(command);
        }
    
    private synchronized void addClientService(ServerService clientService)
    throws IOException {
    	clientService.init();
    	clients.addElement(clientService);
    	new Thread(clientService).start();
    	System.out.println("Doda³em. " + clients.size());
    }

    private synchronized void removeClientService(ServerService clientService) {
    	clients.removeElement(clientService);
    	clientService.close();
    	System.out.println("Usun¹³em. " + clients.size());
    }
    
/*    private void newObject (int cid, String desc)
	{
    	newSimulator(cid, desc);
    }
    */
    
    private void newSimulator(int cid, String desc)
    {
    	sims.add(getServiceById(cid));
    	simMap.add(new SimMap(getServiceById(cid), desc));
    }
    
    private void newNodeProvider(int cid, String id, String desc)
    {
    	nodeprvdr.add(getServiceById(cid));
    	newSimulator(cid, desc);
    	//sendToId(cid, Protocol.DESCRIBE);
    }
    
    private void newNode(int cid, String nodeDef)
    {
    	if(nodeType.equals(NodeType.none))
    		nodeType = NodeType.node;
    	if(nodeType.equals(NodeType.node))
    		nodeMap.add(new NodeMap(getServiceById(cid), nodeDef));
    }
    
    private void newNode(int cid, long id, String nodeDef)
    {
    	if(nodeType.equals(NodeType.none))
    		nodeType = NodeType.sim;
    	if(nodeType.equals(NodeType.sim))
    		nodeMap.add(new NodeMap(getServiceById(cid), id, nodeDef));
    }
    
    private void oldNode(int cid, long id)
    {
    	for(NodeMap nmap : nodeMap)
    		if(nmap.getNodeId() == id)
    		{
    			nodeMap.removeElement(nmap);
    			break;
    		}
    	for(BidMap bmap : bidMap)
    		if(bmap.id == id)
    		{
    			sendToId(bmap.sservice.getId(), Protocol.LINK_STATUS + Protocol.div + Protocol.OFF + Protocol.div + id);
    			bidMap.removeElement(bmap);
    			break;
    		}
    }
    
    private void old(int cid)
    {
    	ServerService temp = getServiceById(cid);
    	temp.stop();
    	removeClientService(temp);
    	sims.removeElement(temp);
    	nodeprvdr.removeElement(temp);
    	Vector<BidMap> todelbmap = new Vector<BidMap>();
    	Vector<NodeMap> todelnmap = new Vector<NodeMap>();
    	Vector<SimMap> todelsmap = new Vector<SimMap>();
		for(SimMap sim : simMap)
			if(sim.sservice.getId() == cid)
				todelsmap.add(sim);
    	for(BidMap bmap : bidMap)
    		if(bmap.sservice.getId() == cid)
    			todelbmap.add(bmap);
    	for(NodeMap nmap : nodeMap)
    		if(nmap.nodeService.getId() == cid)
    		{
    			for(BidMap bmap : bidMap)
    				if(bmap.id == nmap.nodeId)
    				{
    	    			sendToId(bmap.sservice.getId(), Protocol.LINK_STATUS + Protocol.div + Protocol.OFF + Protocol.div + bmap.id);
    	    			todelbmap.add(bmap);
    				}
    			todelnmap.add(nmap);
    		}
    	for(BidMap bmap : todelbmap)
    		bidMap.removeElement(bmap);
    	for(NodeMap nmap : todelnmap)
    		nodeMap.removeElement(nmap);
    	for(SimMap smap : todelsmap)
    		simMap.removeElement(smap);
    }
    
    /**
     * Creates new simulator to node bid;
     * @param cid simulator's id
     * @param id node's id
     * @return console return message
     */
    protected String newBid(int cid, long id)
    {
    	ServerService temp = getServiceById(cid);
    	for(BidMap bmap : bidMap)
    		if(bmap.id == id)
    			return "node #" + id + "already simulated";
    	boolean goon = false;
    	for(SimMap sim : simMap)
    		if(sim.sservice.getId() == cid)
    			goon = true;
    	if(!goon)
        	return "no simulator #" + cid;
    	for(NodeMap node : nodeMap)
    		if(node.getNodeId() == id)
    		{
    	    	bidMap.add(new BidMap(temp, id));
    	    	sendToId(cid, Protocol.LINK_STATUS + Protocol.div + Protocol.ON + Protocol.div + cid);
    	    	sendToId((int)id , Protocol.LINK_STATUS + Protocol.div + Protocol.ON + Protocol.div + id);
    	    	return "bid done";
    		}
    	return "no node #" + id;
    }
    
    protected String fastBid()
    {
    	int simId = 0;
    	for(SimMap sim : simMap)
    	{
    		simId = sim.sservice.getId();
    		break;
    	}
    	for(NodeMap node : nodeMap)
    	{
    		this.newBid(simId, node.getNodeId());
    	}
		this.list('n');
		this.list('s');
		this.list('b');
    	return "\nFast bid done\n?> ";
    	
    }

    /**
     * Removes simulator to node bid;
     * @param cid simulator's id
     * @param id node's id
     * @return console return message
     */
    protected String oldBid(int cid, long id)
    {
    	Vector<BidMap> todelbmap = new Vector<BidMap>();
    	boolean goon = false;
    	for(BidMap bmap : bidMap)
    		if(bmap.sservice.getId() == cid && bmap.id == id)
    		{
    			todelbmap.add(bmap);
    			goon = true;
    		}
    	if(goon)
    	{
	    	for(BidMap bmap : todelbmap)
	    		bidMap.removeElement(bmap);
	    	sendToId(cid, Protocol.LINK_STATUS + Protocol.div + Protocol.OFF + Protocol.div + cid);
	    	sendToId((int)id, Protocol.LINK_STATUS + Protocol.div + Protocol.OFF + Protocol.div + id);
	    	return "bid undone";
    	}
    	return "no bid of node #" + id + " on simulator #" + cid;
    }
    
    private ServerService getServiceById(int cid)
    {
    	for(ServerService s : clients)
    		if(cid == s.getId())
    			return s;
    	return null;
    }
    
    private void event(int cid, String callID, String pevent, String params)
    {
    	long id = Long.parseLong(callID.substring(0, callID.indexOf("#")));
    	for(NodeMap nmap : nodeMap)
    		if(nmap.getNodeId() == id)
    		{
    			if(nodeType.equals(NodeType.sim))
    				sendToId(nmap.nodeService.getId(), Protocol.EVENT + Protocol.div + callID + Protocol.div + pevent + Protocol.div + params);
    			else
    				sendToId(nmap.nodeService.getId(), Protocol.EVENT + Protocol.div + pevent + Protocol.div + params);
    			break;
    		}
    }
    
    private void instr(int cid, String call_id, String code, String param)
    {
    	String cli_id = call_id.substring(0, call_id.indexOf("#"));
    	int tmpID = Integer.parseInt(cli_id);
    	for(BidMap bmap : bidMap)
    		if(tmpID == bmap.id)
    			sendToId(bmap.sservice.getId(), Protocol.INSTR + Protocol.div + call_id + Protocol.div + code + Protocol.div + param);
    }
    
    private void instr(int cid, long id, String code, String param)
    {
       	for(BidMap bmap : bidMap)
    		if(bmap.id == id)
    			sendToId(bmap.sservice.getId(), Protocol.INSTR + Protocol.div + id + Protocol.div + code + Protocol.div + param);
    }
    
    private void ret_instr(int cid, String id, String value) 
    {
    	Long nodeID = Long.parseLong(id.substring(0, id.indexOf("#")));
    	for(NodeMap nmap : nodeMap)
    		if(nmap.getNodeId() == nodeID)
   				sendToId(nmap.nodeService.getId(), Protocol.INSTR_RETURN + Protocol.div + id + Protocol.div + value);
	}
    
    private void ret_event(int cid, String id, String value) 
    {
    	Long destId = Long.parseLong(id.substring(0, id.indexOf("#")));
    	for(BidMap bmap : bidMap)
    		if(bmap.id == destId)
    			sendToId(bmap.sservice.getId(), Protocol.EVENT_RETURN + Protocol.div + id + ": " + value);
	}
    
    /**
     * Checks if server is running.
     * @return state
     */
    public boolean isRunning()
    {
    	return goon;
    }

	/**
	 * Generates lists of nodes/simulators/bids.
	 * @param charAt command line specifier
	 * @return lists
	 */
	public String list(char charAt) {
		String tmp = "";
		int count = 0;
		if(charAt == 'n')
		{
			tmp += "\nNODES LIST:\n";
			for(NodeMap node : nodeMap)
				tmp += (++count) + ") node#" + node.getNodeId() + ": " + node.name + "\n";
		}
		else if(charAt == 's')
		{
			tmp += "\nSIM LIST:\n";
			for(SimMap sim : simMap)
				tmp += (++count) + ") sim#" + sim.sservice.getId() + ": " + sim.desc + "\n";
		}
		else if(charAt == 'b')
		{
			tmp += "\nBID LIST:\n";
			for(BidMap bid : bidMap)
				tmp += (++count) + ") node#" + bid.id + ", " + bid.sservice.getId() + "\n";
		}
		return tmp;
	}
}