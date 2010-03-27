package pl.edu.pw.pobicos.mw.port;

import java.io.File;
import java.io.FileInputStream;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;

public class Import {

	public static SimulationElement importSimulation(FileInputStream fis)
	{
		SimulationElement sim = null;
		try {//parsing the xmls
			try{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(fis);
				doc.getDocumentElement().normalize();
				Node root = doc.getDocumentElement();
				String name = ((Attr)root.getAttributes().getNamedItem("name")).getValue();
				long networkId = Long.parseLong(((Attr)root.getAttributes().getNamedItem("networkId")).getValue());
				sim = new SimulationElement(name, networkId);
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
						if(node.getNodeName().equals("event"))
						{
							long code = Long.parseLong(((Attr)node.getAttributes().getNamedItem("code")).getValue());
							int nodeId = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("nodeId")).getValue());
							long virtualTime = Long.parseLong(((Attr)node.getAttributes().getNamedItem("virtualTime")).getValue());
							sim.addEvent(new EventElement(code, nodeId, virtualTime));
						}
				}
			  } catch (Exception e) {
			    e.printStackTrace();
			  }
		} catch (Exception e) {
		}
		return sim;
	}

	public static NetworkElement importNetwork(FileInputStream fis, String path)
	{
		NetworkElement net = null;
		try {//parsing the xmls
			try{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				dbf.setValidating(false);
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(fis);
				doc.getDocumentElement().normalize();
				Node root = doc.getDocumentElement();
				String name = ((Attr)root.getAttributes().getNamedItem("name")).getValue();
				long id = Long.parseLong(((Attr)root.getAttributes().getNamedItem("id")).getValue());
				net = new NetworkElement(name, id);
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
						if(node.getNodeName().equals("node"))
						{
							name = ((Attr)node.getAttributes().getNamedItem("name")).getValue();
							int nodeId = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("id")).getValue());
							int x = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("x")).getValue());
							int y = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("y")).getValue());
							int memory = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("memory")).getValue());
							int range = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("range")).getValue());
							String nodeDef = addTags(node.getChildNodes().item(0).getNodeValue());
							net.addNode(new NodeElement(name, nodeId, x, y, memory, range, nodeDef));
						}
						else if(node.getNodeName().equals("application"))
						{
							name = ((Attr)node.getAttributes().getNamedItem("name")).getValue();
							int nodeId = Integer.parseInt(((Attr)node.getAttributes().getNamedItem("nodeId")).getValue());
				    		FileInputStream is = new FileInputStream(path + node.getChildNodes().item(0).getNodeValue());
				    		byte[] appBundle=new byte[(int)(new File(path + node.getChildNodes().item(0).getNodeValue())).length()];
				    		is.read(appBundle);
							net.addApplication(new ApplicationElement(name, nodeId, appBundle));
						}
				}
			  } catch (Exception e) {
			    e.printStackTrace();
			  }
		} catch (Exception e) {
		}
		return net;
	}
	
	private static String addTags(String in)
	{
		String out = in.replaceAll("&lt;", "<");
		out = out.replaceAll("&gt;", ">");
		return out;
	}
}
