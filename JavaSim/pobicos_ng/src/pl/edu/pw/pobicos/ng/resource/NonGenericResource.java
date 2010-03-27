package pl.edu.pw.pobicos.ng.resource;

import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;
import java.util.Vector;

import org.w3c.dom.Node;

import pl.edu.pw.pobicos.ng.Conversions;
import pl.edu.pw.pobicos.ng.event.Event;
import pl.edu.pw.pobicos.ng.event.EventTree;
import pl.edu.pw.pobicos.ng.event.PhysicalEvent;

public class NonGenericResource extends AbstractResource{
	private Map<Long, Method> instructionsSupported;
	private Vector<Event> eventsRaisen;
	private Vector<PhysicalEvent> physicalEventsRaisen;
	
	public NonGenericResource(Node item)
	{
		eventsRaisen = new Vector<Event>();
		physicalEventsRaisen = new Vector<PhysicalEvent>();
		instructionsSupported = new HashMap<Long, Method>();
		try {//parsing the xmls
			try{
				Node root = item;
				//List<String> resourceXMLs = new ArrayList<String>();
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
					{
						root = node;
						break;
					}
				}
				for(int i = 0; i < root.getChildNodes().getLength(); i++)
				{
					Node node = root.getChildNodes().item(i);
					if(node.getNodeType() == Node.ELEMENT_NODE)
						if(node.getNodeName().equals("primitives"))
							for(int j = 0; j < node.getChildNodes().getLength(); j++)
								if(node.getChildNodes().item(j).getNodeType() == Node.ELEMENT_NODE)
									if(node.getChildNodes().item(j).getNodeName().equals("instruction"))
									{
										for(int k = 0; k < node.getChildNodes().item(j).getChildNodes().getLength(); k++)
											if(node.getChildNodes().item(j).getChildNodes().item(k).getNodeType() == Node.ELEMENT_NODE)
												if(node.getChildNodes().item(j).getChildNodes().item(k).getNodeName().equals("definition"))
												{
													Node root2 = node.getChildNodes().item(j).getChildNodes().item(k);
													for(int m = 0; m < root2.getChildNodes().getLength(); m++)
													{
														Node node2 = root2.getChildNodes().item(m);
														if(node2.getNodeType() == Node.ELEMENT_NODE)
														{
															root2 = node2;
															break;
														}
													}
													for(int m = 0; m < root2.getChildNodes().getLength(); m++)
													{
														Node node2 = root2.getChildNodes().item(m);
														if(node2.getNodeType() == Node.ELEMENT_NODE)
															if(node2.getNodeName().equals("id"))
																for(int l = 0; l < node2.getChildNodes().getLength(); l++)
																	if(node2.getChildNodes().item(l).getNodeType() == Node.TEXT_NODE)
																	{
																		long temp = Conversions.hexStringToLong(node2.getChildNodes().item(l).getNodeValue().substring(2));
																		/*for(Method method : this.getClass().getDeclaredMethods())
																			if(method.getName().equals(InstructionMap.getName(temp)))
																			{*/
																				instructionsSupported.put(temp, null);
																				/*break;
																			}*/
																	}
													}
												}
									}
									else if(node.getChildNodes().item(j).getNodeName().equals("physical_event"))
									{
										PhysicalEvent pevent = null;
										for(int k = 0; k < node.getChildNodes().item(j).getChildNodes().getLength(); k++)
											if(node.getChildNodes().item(j).getChildNodes().item(k).getNodeType() == Node.ELEMENT_NODE)
												if(node.getChildNodes().item(j).getChildNodes().item(k).getNodeName().equals("label"))
												{
													for(int b = 0; b < node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().getLength(); b++)
														if(node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().item(b).getNodeType() == Node.TEXT_NODE)
														{
															pevent = new PhysicalEvent(node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().item(b).getNodeValue());
															physicalEventsRaisen.add(pevent);
														}
												}
												else if(node.getChildNodes().item(j).getChildNodes().item(k).getNodeName().equals("events"))
													for(int l = 0; l < node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().getLength(); l++)
														if(node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().item(l).getNodeType() == Node.ELEMENT_NODE)
															if(node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().item(l).getNodeName().equals("event"))
															{
																Node root2 = node.getChildNodes().item(j).getChildNodes().item(k).getChildNodes().item(l);
																for(int m = 0; m < root2.getChildNodes().getLength(); m++)
																{
																	Node node2 = root2.getChildNodes().item(m);
																	if(node2.getNodeType() == Node.ELEMENT_NODE)
																	{
																		root2 = node2;
																		break;
																	}
																}
																for(int m = 0; m < root2.getChildNodes().getLength(); m++)
																{
																	Node node2 = root2.getChildNodes().item(m);
																	if(node2.getNodeType() == Node.ELEMENT_NODE)
																		if(node2.getNodeName().equals("id"))
																			for(int n = 0; n < node2.getChildNodes().getLength(); n++)
																				if(node2.getChildNodes().item(n).getNodeType() == Node.TEXT_NODE)
																				{
																					long temp = Conversions.hexStringToLong(node2.getChildNodes().item(n).getNodeValue().substring(2));
																					Event ev = new Event(temp);
																					eventsRaisen.add(ev);
																					try
																					{
																						pevent.addEvent(ev);
																					}catch(Exception ex){}
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
	}
	
	@Override
	public Vector<Event> eventsRaisen()
	{
		return eventsRaisen;
	}
	
	@Override
	public Vector<PhysicalEvent> physicalEventsRaisen()
	{
		return physicalEventsRaisen;
	}
	
	@Override
	public boolean raisesEvent(Event event)
	{
		for(int i = 0; i < eventsRaisen.size(); i++)
			if(EventTree.eventFires(eventsRaisen.elementAt(i), event))
				return true;
		return false;
	}

	@Override
	public boolean supportsInstruction(long instr)
	{
		if(instructionsSupported.containsKey(new Long(instr)))
			return true;
		return false;
	}

	@Override
	public Vector<Long> instructionsSupported()
	{
		Vector<Long> result = new Vector<Long>();
		for(Long code : instructionsSupported.keySet())
			result.add(code);
		return result;
	}
	
	@Override
	public void runInstruction(long instr){}
}
