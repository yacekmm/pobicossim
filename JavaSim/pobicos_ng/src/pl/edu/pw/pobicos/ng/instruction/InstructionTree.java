package pl.edu.pw.pobicos.ng.instruction;

import java.util.Vector;

import pl.edu.pw.pobicos.ng.Conversions;

import org.w3c.dom.*;
import java.io.InputStream;
import javax.xml.parsers.*;

public class InstructionTree {
	
	private static Node findNode(Instruction myInstruction, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getNodeName().equals("Instruction"))
					if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(myInstruction.getName()))
						return node;
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getNodeName().equals("Instruction"))
					if(node.getChildNodes().getLength() > 0)
					{
						Node sub = findNode(myInstruction, node);
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
				if(node.getNodeName().equals("Instruction"))
					if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(name))
						return Conversions.hexStringToLong(((Attr)node.getAttributes().getNamedItem("code")).getValue());
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getNodeName().equals("Instruction"))
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
				if(node.getNodeName().equals("Instruction"))
					if(((Attr)node.getAttributes().getNamedItem("code")).getValue().equals(Conversions.longToHexString(code, 4)))
						return ((Attr)node.getAttributes().getNamedItem("name")).getValue();
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getNodeName().equals("Instruction"))
					if(node.getChildNodes().getLength() > 0)
					{
						String sub = findByNode(code, node);
						if(sub != null)
							return sub;
					}
		}
		return null;
	}
	
	private static Vector<Instruction> addChildren(Node startNode)
	{
		Vector<Instruction> result = new Vector<Instruction>();
		result.add(new Instruction(Conversions.hexStringToLong(startNode.getAttributes().getNamedItem("code").getNodeValue()), startNode.getAttributes().getNamedItem("name").getNodeValue()));
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node temp = startNode.getChildNodes().item(i);
			if(temp.getNodeType() == Node.ELEMENT_NODE)
				if(temp.getNodeName().equals("Instruction"))
					for(Instruction instr : addChildren(temp))
						result.add(instr);
		}
		return result;
	}
	
	public static Vector<Instruction> instructionsUnder(Instruction myInstruction)
	{
		Vector<Instruction> result = new Vector<Instruction>();
		Document doc = parseXml();
		if(doc == null)
			return result;
		Node root = doc.getDocumentElement();		
		Node main = findNode(myInstruction, root);
		if(main == null)
			return result;		
		result.add(myInstruction);
		for(Instruction instr : addChildren(main))
			result.add(instr);
		return result;
	}
	
	public static Vector<Instruction> instructionsOver(Instruction myInstruction)
	{
		Vector<Instruction> result = new Vector<Instruction>();
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
				result.add(new Instruction(Conversions.hexStringToLong(((Attr)main.getAttributes().getNamedItem("code")).getValue()), ((Attr)main.getAttributes().getNamedItem("name")).getValue()));
			}catch(Exception ex){break;}
			if(main.equals(root))
				break;
		}
		return result;
	}
	
	private static Document parseXml()
	{
		try{
			InputStream file = InstructionTree.class.getResourceAsStream("/resources/xml/InstructionTaxonomy.xml");
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
}
