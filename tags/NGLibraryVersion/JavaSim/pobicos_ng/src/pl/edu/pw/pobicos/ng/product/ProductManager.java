package pl.edu.pw.pobicos.ng.product;

import java.io.ByteArrayInputStream;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Node;

public class ProductManager {

	private static ProductManager instance = null;
	
	private Vector<ProductMap> productMap = new Vector<ProductMap>();
	
	public ProductManager()
	{	//empty
	}
	
	public static ProductManager getInstance()
	{
		if(instance == null)
			instance = new ProductManager();
		return instance;
	}
	
	public void addProduct(long id, String nodeDef)
	{
		AbstractProduct product = ProductFactory.createNode(nodeDef);
    	String name = "?";
		try {//parsing the xmls
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
							name = node.getChildNodes().item(0).getNodeValue();
							break;
						}
				}
			}catch(Exception e){}
		}catch(Exception e){}
		productMap.add(new ProductMap(id, nodeDef, product, name));
	}
	
	public void removeProduct(int id)
	{
    	for(ProductMap nmap : productMap)
    		if(nmap.getId() == id)
    		{
    			productMap.removeElement(nmap);
    			break;
    		}		
	}

	public Vector<ProductMap> getProducts() {
		return productMap;
	}

	public void clear() 
	{
		productMap.clear();
	}
}
