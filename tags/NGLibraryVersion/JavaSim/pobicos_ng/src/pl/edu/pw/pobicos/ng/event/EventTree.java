package pl.edu.pw.pobicos.ng.event;

import java.io.InputStream;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;

import pl.edu.pw.pobicos.ng.Conversions;

public class EventTree {
	private static Node findNode(Event myEvent, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(myEvent.getName()))
					return node;
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().getLength() > 0)
				{
					Node sub = findNode(myEvent, node);
					if(sub != null)
						return sub;
				}
		}
		return null;
	}
	
	private static long findByNode(String name, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(name))
					return Conversions.hexStringToLong(((Attr)node.getAttributes().getNamedItem("code")).getValue());
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().getLength() > 0)
				{
					long sub = findByNode(name, node);
					if(sub != -1)
						return sub;
				}
		}
		return -1;
	}
	
	private static String findByNode(long code, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(((Attr)node.getAttributes().getNamedItem("code")).getValue().equals(Conversions.longToHexString(code, 4)))
					return ((Attr)node.getAttributes().getNamedItem("name")).getValue();
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().getLength() > 0)
				{
					String sub = findByNode(code, node);
					if(sub != null)
						return sub;
				}
		}
		return null;
	}
	
	private static Vector<Event> addChildren(Node startNode)
	{
		Vector<Event> result = new Vector<Event>();
		result.add(new Event(Conversions.hexStringToLong(startNode.getAttributes().getNamedItem("code").getNodeValue())));
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node temp = startNode.getChildNodes().item(i);
			if(temp.getNodeType() == Node.ELEMENT_NODE)
				for(Event instr : addChildren(temp))
					result.add(instr);
		}
		return result;
	}
	
	public static Vector<Event> eventsUnder(Event myEvent)
	{
		Vector<Event> result = new Vector<Event>();
		Document doc = parseXml();
		if(doc == null)
			return result;
		Node root = doc.getDocumentElement();		
		Node main = findNode(myEvent, root);
		if(main == null)
			return result;		
		result.add(myEvent);
		for(Event instr : addChildren(main)){System.out.print(instr.getName()+";");
			result.add(instr);}
		return result;
	}
	
	public static Vector<Event> eventsOver(Event myInstruction)
	{
		Vector<Event> result = new Vector<Event>();
		Document doc = parseXml();
		if(doc == null)
			return result;
		Node root = doc.getDocumentElement();		
		Node main = findNode(myInstruction, root);
		if(main == null)
			return result;		
		result.add(myInstruction);
		while(main.getParentNode() != null)
		{
			main = main.getParentNode();
			try{
				result.add(new Event(Conversions.hexStringToLong(((Attr)main.getAttributes().getNamedItem("code")).getValue())));
			}catch(Exception ex){break;}
			if(main.equals(root))
				break;
		}
		return result;
	}
	
	private static Document parseXml()
	{
		return parseXml("Event");
	}
	
	private static Document parseXml(String what)
	{
		try{
			InputStream file = EventTree.class.getResourceAsStream("/resources/xml/" + what + "Taxonomy.xml");
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder db = dbf.newDocumentBuilder();
			Document doc = db.parse(file);
			doc.getDocumentElement().normalize();
			return doc;
		  } catch (Exception e) {
		    e.printStackTrace();
		  }
		  return null;
	}
	
	public static String getName(long code)
	{
		Document doc = parseXml();;
		return findByNode(code, doc.getDocumentElement());
	}
	
	public static long getCode(String name)
	{
		Document doc = parseXml();;
		return findByNode(name, doc.getDocumentElement());
	}
	
	public static boolean eventFires(Event first, Event second)
	{
		Vector<Event> fired = eventsOver(first);
		for(Event event : fired)
			if(event.getCode() == second.getCode())
				return true;
		return false;
	}
	
	public static String getOriginName(long code)
	{
		Document doc = parseXml("Origin");;
		return findByNode(code, doc.getDocumentElement());
	}
	
	public static long getOriginCode(String name)
	{
		Document doc = parseXml("Origin");;
		return findByNode(name, doc.getDocumentElement());
	}
}