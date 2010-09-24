package pl.edu.pw.pobicos.ng.instruction;

import java.io.InputStream;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;

public class NonGenericInstructions {
	
	private static Node findNode(String myInstruction, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getNodeName().equals("Instruction"))
					if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(myInstruction))
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
	
	public static Vector<String> getParams(long code)
	{
		Vector<String> params = new Vector<String>();
		Node node = findNode(String.valueOf(code), parseXml().getDocumentElement());
		for(int i = 0; i < node.getChildNodes().getLength(); i++)
			if(node.getChildNodes().item(i).getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().item(i).getNodeName().equals("Params"))
					for(int j = 0; j < node.getChildNodes().getLength(); j++)
						if(node.getChildNodes().item(i).getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
							params.add(node.getChildNodes().item(i).getChildNodes().item(j).getChildNodes().item(0).getNodeValue());
		return params;
	}
	
	public static String getReturn(long code)
	{
		Node node = findNode(InstructionMap.getName(code), parseXml().getDocumentElement());
		for(int i = 0; i < node.getChildNodes().getLength(); i++)
			if(node.getChildNodes().item(i).getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().item(i).getNodeName().equals("Ret"))
					return node.getChildNodes().item(i).getChildNodes().item(0).getNodeValue();
		return null;
	}
}
