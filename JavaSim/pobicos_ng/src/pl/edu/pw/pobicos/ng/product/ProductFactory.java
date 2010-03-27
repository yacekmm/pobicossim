package pl.edu.pw.pobicos.ng.product;

import java.io.StringReader;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;

import pl.edu.pw.pobicos.ng.Conversions;
import pl.edu.pw.pobicos.ng.resource.AbstractResource;
import pl.edu.pw.pobicos.ng.resource.ResourceFactory;
import pl.edu.pw.pobicos.ng.taxonomy.LocationTree;
import pl.edu.pw.pobicos.ng.taxonomy.ProductTree;

public class ProductFactory {

	private ProductFactory() {
		// empty
	}
	
	public static AbstractProduct createNode()
	{
		AbstractProduct product = new AbstractProduct();
		Vector<AbstractResource> resourceList = new Vector<AbstractResource>();
		resourceList.add(ResourceFactory.getInstance().createResource());
		product.init(LocationTree.getRootCode(), ProductTree.getRootCode(), resourceList);
		return product;
	}

	public static AbstractProduct createNode(String xml) {
		AbstractProduct newProduct = null;
		long locationId = LocationTree.getRootCode(), productId = ProductTree.getRootCode();
		Vector<AbstractResource> resourceList = new Vector<AbstractResource>();
		resourceList.add(ResourceFactory.getInstance().createResource());
		try {//parsing the xmls
			try{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				dbf.setValidating(false);
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(new InputSource(new StringReader(xml)));
				doc.getDocumentElement().normalize();
				Node root = doc.getDocumentElement();
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
						if(node.getNodeName().equals("resources"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
							{
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("resource"))
									{
										resourceList.add(ResourceFactory.getInstance().createResource(node.getChildNodes().item(j)));
									}
							}
						else if(node.getNodeName().equals("descriptors"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
							{
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("descriptor"))
									{
										Node root2 = node.getChildNodes().item(j);
										for(int m = 0; m < root2.getChildNodes().getLength(); m++)
										{
											Node node2 = root2.getChildNodes().item(m);
											if(node2.getNodeType() == Node.ELEMENT_NODE)
											{
												root2 = node2;
												break;
											}
										}
										for(int k = 0; k < root2.getChildNodes().getLength(); k++)
										{
											Node node2 = root2.getChildNodes().item(k);
											if(node2.getNodeType() == Node.ELEMENT_NODE)
												if(node2.getNodeName().equals("id"))
													for(int l = 0; l < node2.getChildNodes().getLength(); l++)
														if(node2.getChildNodes().item(l).getNodeType() == Node.TEXT_NODE)
														{
															long temp = Conversions.hexStringToLong(node2.getChildNodes().item(l).getNodeValue().substring(2));
															if(LocationTree.getName(temp) != null)
																locationId = temp;
															else if(ProductTree.getName(temp) != null)
																productId = temp;
														}
										}
									}
							}
				}
			  } catch (Exception e) {
			    e.printStackTrace();
			  }
		} catch (Exception e) {
		}
		newProduct = new AbstractProduct();
		newProduct.init(productId, locationId, resourceList);
		return newProduct;
	}
}
