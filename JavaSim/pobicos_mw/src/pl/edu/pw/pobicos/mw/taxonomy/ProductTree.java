package pl.edu.pw.pobicos.mw.taxonomy;

import java.io.InputStream;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;

import pl.edu.pw.pobicos.mw.Conversions;

/**
 * Manages hierarchy of product taxonomy.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ProductTree {
	
	private static Document parseXml()
	{
		return parseXml("Product");
	}
	
	private static Document parseXml(String what)
	{
		try{
			InputStream file = ProductTree.class.getResourceAsStream("/resources/xml/" + what + "Taxonomy.xml");
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
	
	private static Node findNode(String myLocation, Node startNode)
	{
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(((Attr)node.getAttributes().getNamedItem("name")).getValue().equals(myLocation))
					return node;
		}
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node node = startNode.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				if(node.getChildNodes().getLength() > 0)
				{
					Node sub = findNode(myLocation, node);
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
	
	private static Vector<String> addChildren(Node startNode)
	{
		Vector<String> result = new Vector<String>();
		if(startNode == null)
			return result;
		result.add(startNode.getAttributes().getNamedItem("name").getNodeValue());
		for(int i = 0; i < startNode.getChildNodes().getLength(); i++)
		{
			Node temp = startNode.getChildNodes().item(i);
			if(temp.getNodeType() == Node.ELEMENT_NODE)
				for(String loc : addChildren(temp))
					result.add(loc);
		}
		return result;
	}
	
	private static Vector<String> productsOver(String myLocation)
	{
		Vector<String> result = new Vector<String>();
		Document doc = parseXml();
		if(doc == null)
			return result;
		Node root = doc.getDocumentElement();		
		Node main = findNode(myLocation, root);
		if(main == null)
			return result;		
		result.add(myLocation);
		while(main.getParentNode() != null)
		{
			main = main.getParentNode();
			try{
				result.add(((Attr)main.getAttributes().getNamedItem("name")).getValue());
			}catch(Exception ex){break;}
			if(main.equals(root))
				break;
		}
		return result;
	}
	
	private static boolean isIndirectParent(long spec, long abst)
	{
		for(String l : productsOver(ProductMap.getName(spec)))
			if(l.equals(ProductMap.getName(abst)))
				return true;
		return false;
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
	
	public static long getRootCode()
	{
		Document doc = parseXml();;
		Node root = doc.getDocumentElement();
		for(int i = 0; i < root.getChildNodes().getLength(); i++)
		{
			Node node = root.getChildNodes().item(i);
			if(node.getNodeType() == Node.ELEMENT_NODE)
				return Conversions.hexStringToLong(((Attr)node.getAttributes().getNamedItem("code")).getValue());
		}
		return -1;
	}
	
	public static boolean isInRange(long value, long down, long up)
	{
		Document doc = parseXml();
		if(doc == null)
			return false;
		Node root = doc.getDocumentElement();
		for(String l : addChildren(findNode(ProductMap.getName(up), root)))
			if(l.equals(ProductMap.getName(value)))
				return true;
		return false;
	}

	public static int distance(long value, long spec, long abst) 
	{
		if(spec == -1)
			return -1;
		if(isIndirectParent(spec, abst))
		{
			Document doc = parseXml();;
			Node root = doc.getDocumentElement();
			for(String l : addChildren(ProductTree.findNode(ProductMap.getName(spec), root)))
				if(l.equals(ProductMap.getName(value)))
					return 0;
			if(spec == abst)
				return -1;
			return 1 + distance(value, ProductMap.getCode(productsOver(ProductMap.getName(spec)).elementAt(1)), abst);
		}
		return -1;
	}
	
	public static int distance(long spec, long abst) 
	{
		Vector<String> parents = productsOver(ProductMap.getName(spec));
		if(!parents.contains(ProductMap.getName(abst)))
			return -1;
		int dist = 1;
		for(String str : parents)
			if(str.equals(ProductMap.getName(abst)))
				return dist;
			else
				dist++;
		return -1;
	}
}