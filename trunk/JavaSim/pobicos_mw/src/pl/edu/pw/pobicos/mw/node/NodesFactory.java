package pl.edu.pw.pobicos.mw.node;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.List;
import java.util.Vector;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;

import pl.edu.pw.pobicos.mw.port.NodeElement;
import pl.edu.pw.pobicos.mw.resource.AbstractResource;
import pl.edu.pw.pobicos.mw.resource.ResourceFactory;
import pl.edu.pw.pobicos.mw.taxonomy.LocationTree;
import pl.edu.pw.pobicos.mw.taxonomy.ProductTree;
import pl.edu.pw.pobicos.mw.Conversions;

/**
 * This Factory class allows for creating different types of nodes. This class cannot be instantiated, only static
 * factory methods can be used.
 * 
 * @author Tomasz Anuszewski
 * @created 2007-11-27 12:50:31
 */
public class NodesFactory {

	// private constructor - prevents from being instantiated beyond this class
	private NodesFactory() {
		// empty
	}
	
	public static AbstractNode createNode()
	{
		AbstractNode node = new GenericNode();
		node.init(-1, -1, null, null);
		return node;
	}

	public static AbstractNode createNode(File file, String path) {
		AbstractNode newNode = null;
		long locationId = 0, productId = 0;
		List<AbstractResource> resourceList = new ArrayList<AbstractResource>();
		String name = "";
		try {//parsing the xmls
			try{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				//dbf.setValidating(false);
				//dbf.setNamespaceAware(false);
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(file);
				doc.getDocumentElement().normalize();
				Node root = doc.getDocumentElement();
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
						if(node.getNodeName().equals("name"))
						{
							name = node.getChildNodes().item(0).getNodeValue().toString();
						}
						else if(node.getNodeName().equals("resources"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
							{
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("resource"))
									{
										String resourceFile = ((Attr)node.getChildNodes().item(j).getAttributes().getNamedItem("xlink:href")).getValue();
										File plik = new File(path + resourceFile);
										resourceList.add(ResourceFactory.getInstance().createResource(plik, path));
									}
							}
						else if(node.getNodeName().equals("descriptors"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
							{
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("descriptor"))
									{
										String resourceFile = ((Attr)node.getChildNodes().item(j).getAttributes().getNamedItem("xlink:href")).getValue();
										File plik = new File(path + resourceFile);
							    		try
							    		{
								    		FileInputStream fis = new FileInputStream(plik);
								    		Document doc2 = db.parse(fis);
											doc2.getDocumentElement().normalize();
											Node root2 = doc2.getDocumentElement();
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
							    		}catch(Exception ex){}
									}
							}
				}
			  } catch (Exception e) {
			    e.printStackTrace();
			  }
		} catch (Exception e) {
		}
		String nodeDef = getNodeDef(file, path);
		newNode = new NonGenericNode();
		newNode.init(productId, locationId, resourceList, nodeDef);
		newNode.setName(name);
		return newNode;
	}
	
	public static AbstractNode createNode(String xml)
	{
		AbstractNode newNode = null;
		long locationId = LocationTree.getRootCode(), productId = ProductTree.getRootCode();
		Vector<AbstractResource> resourceList = new Vector<AbstractResource>();
		//resourceList.add(ResourceFactory.getInstance().createResource());
		String name = "";
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
						if(node.getNodeName().equals("name"))
						{
							name = node.getChildNodes().item(0).getNodeValue().toString();
						}
						else if(node.getNodeName().equals("resources"))
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
		newNode = new NonGenericNode();
		newNode.init(productId, locationId, resourceList, xml);
		newNode.setName(name);
		return newNode;
	}

	public static AbstractNode createNode(NodeElement node) {
		return (node.getNodeDef().equals("null") ? createNode() : createNode(node.getNodeDef()));
	}
	
	private static String getNodeDef(File file, String path)
	{
		String result = "";
		try {//parsing the xmls
			try{
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				FileInputStream fis = null;
			    DataInputStream in = null;
				BufferedReader br = null;
				try {
					fis = new FileInputStream(file);
					in = new DataInputStream(fis);
					br = new BufferedReader(new InputStreamReader(in));
					String tmp;
					while ((tmp = br.readLine()) != null) 
						result += tmp;
					fis.close();	
					in.close();
				} catch (Exception e) {}
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document doc = db.parse(file);
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
										String resourceFile = ((Attr)node.getChildNodes().item(j).getAttributes().getNamedItem("xlink:href")).getValue();
										String resourceFile1 = node.getChildNodes().item(j).getChildNodes().item(0).getNodeValue();
										File plik = new File(path + resourceFile);
										fis = null;
									    in = null;
										br = null;
										String temp = "";
										try {
											fis = new FileInputStream(plik);
											in = new DataInputStream(fis);
											br = new BufferedReader(new InputStreamReader(in));
											String tmp;
											while ((tmp = br.readLine()) != null) 
												if(!tmp.startsWith("<?"))
													temp += tmp;
											fis.close();	
											in.close();
										} catch (Exception e) {}
										
										Document doc2 = db.parse(plik);
										doc2.getDocumentElement().normalize();
										Node root2 = doc2.getDocumentElement();
										///////
										try {//parsing the xmls
											try{
												for(int q = 0; q < root2.getChildNodes().getLength(); q++)
												{
													Node node2 = root2.getChildNodes().item(q);
													if(node2.getNodeType() == Node.ELEMENT_NODE)
														if(node2.getNodeName().equals("primitives"))
															for(int w = 0; w < node2.getChildNodes().getLength(); w++)
																if(node2.getChildNodes().item(w).getNodeType() == Node.ELEMENT_NODE)
																	if(node2.getChildNodes().item(w).getNodeName().equals("instruction"))
																	{
																		for(int k = 0; k < node2.getChildNodes().item(w).getChildNodes().getLength(); k++)
																			if(node2.getChildNodes().item(w).getChildNodes().item(k).getNodeType() == Node.ELEMENT_NODE)
																				if(node2.getChildNodes().item(w).getChildNodes().item(k).getNodeName().equals("definition"))
																				{
																					String resourceFile2 = ((Attr)node2.getChildNodes().item(w).getChildNodes().item(k).getAttributes().getNamedItem("xlink:href")).getValue();
																					String resourceFile3 = node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().item(0).getNodeValue();
																					File plik2 = new File(path + resourceFile2);
																					fis = null;
																				    in = null;
																					br = null;
																					String temp2 = "";
																					try {
																						fis = new FileInputStream(plik2);
																						in = new DataInputStream(fis);
																						br = new BufferedReader(new InputStreamReader(in));
																						String tmp;
																						while ((tmp = br.readLine()) != null) 
																							if(!tmp.startsWith("<?"))
																								temp2 += tmp;
																						fis.close();	
																						in.close();
																					} catch (Exception e) {}
																					System.out.println(temp + "\n" + temp2);
																					temp = temp.substring(0, temp.indexOf("<definition xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</definition>")) + "<definition>" + temp2 + "</definition>" + temp.substring(temp.indexOf("<definition xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</definition>") + ("<definition xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</definition>").length());
																					System.out.println(temp + "\n" + temp2);
																				}
																	}
																	else if(node2.getChildNodes().item(w).getNodeName().equals("physical_event"))
																	{
																		for(int k = 0; k < node2.getChildNodes().item(w).getChildNodes().getLength(); k++)
																			if(node2.getChildNodes().item(w).getChildNodes().item(k).getNodeType() == Node.ELEMENT_NODE)
																				if(node2.getChildNodes().item(w).getChildNodes().item(k).getNodeName().equals("events"))
																					for(int l = 0; l < node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().getLength(); l++)
																						if(node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().item(l).getNodeType() == Node.ELEMENT_NODE)
																							if(node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().item(l).getNodeName().equals("event"))
																							{
																								String resourceFile2 = ((Attr)node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().item(l).getAttributes().getNamedItem("xlink:href")).getValue();
																								String resourceFile3 = node2.getChildNodes().item(w).getChildNodes().item(k).getChildNodes().item(l).getChildNodes().item(0).getNodeValue();
																								File plik2 = new File(path + resourceFile2);
																								fis = null;
																							    in = null;
																								br = null;
																								String temp2 = "";
																								try {
																									fis = new FileInputStream(plik2);
																									in = new DataInputStream(fis);
																									br = new BufferedReader(new InputStreamReader(in));
																									String tmp;
																									while ((tmp = br.readLine()) != null) 
																										if(!tmp.startsWith("<?"))
																											temp2 += tmp;
																									fis.close();	
																									in.close();
																								} catch (Exception e) {}
																								System.out.println(temp + "\n" + temp2);
																								temp = temp.substring(0, temp.indexOf("<event xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</event>")) + "<event>" + temp2 + "</event>" + temp.substring(temp.indexOf("<event xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</event>") + ("<event xlink:href=\"" + resourceFile2 + "\">" + resourceFile3 + "</event>").length());
																								System.out.println(temp + "\n" + temp2);
																							}
																	}
													}
											  } catch (Exception e) {
											    e.printStackTrace();
											  }
										} catch (Exception e) {
										}
										///////
										
										result = result.substring(0, result.indexOf("<resource xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</resource>")) + "<resource>" + temp + "</resource>" + result.substring(result.indexOf("<resource xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</resource>") + ("<resource xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</resource>").length());
									}
							}
						else if(node.getNodeName().equals("descriptors"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
							{
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("descriptor"))
									{
										String resourceFile = ((Attr)node.getChildNodes().item(j).getAttributes().getNamedItem("xlink:href")).getValue();
										String resourceFile1 = node.getChildNodes().item(j).getChildNodes().item(0).getNodeValue();
										File plik = new File(path + resourceFile);
										fis = null;
									    in = null;
										br = null;
										String temp = "";
										try {
											fis = new FileInputStream(plik);
											in = new DataInputStream(fis);
											br = new BufferedReader(new InputStreamReader(in));
											String tmp;
											while ((tmp = br.readLine()) != null) 
												if(!tmp.startsWith("<?"))
													temp += tmp;
											fis.close();	
											in.close();
										} catch (Exception e) {}
										result = result.substring(0, result.indexOf("<descriptor xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</descriptor>")) + "<descriptor>" + temp + "</descriptor>" + result.substring(result.indexOf("<descriptor xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</descriptor>") + ("<descriptor xlink:href=\"" + resourceFile + "\">" + resourceFile1 + "</descriptor>").length());
									}
							}
				}
			  } catch (Exception e) {
			    e.printStackTrace();
			  }
		} catch (Exception e) {
		}
		
		return result;
	}
}
