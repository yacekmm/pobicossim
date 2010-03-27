package pl.edu.pw.pobicos.mw.node;

import java.util.ArrayList;
import java.util.List;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.port.NodeElement;
import pl.edu.pw.pobicos.mw.resource.AbstractResource;
import pl.edu.pw.pobicos.mw.taxonomy.LocationMap;
import pl.edu.pw.pobicos.mw.taxonomy.ProductMap;
import pl.edu.pw.pobicos.mw.vm.VirtualMachine;
import pl.edu.pw.pobicos.mw.vm.avr.AvrVM;

/**
 * This class defines nodes hierarchy in the ROVERS system.
 * 
 * @author Tomasz Anuszewski
 * @created 2007-11-25 11:38:37
 */
public abstract class AbstractNode {
	
	private String nodeDef;
	
	protected long productId;
	
	protected long locationId;
	
	/**
	 * unique name, controlled by ROVERS, chosen to distinguish this node from
	 * others
	 */
	protected long id;

	/**
	 * user-friendly name, created by user independently from other nodes names
	 * in the ROVERS network; may be non-unique
	 */
	protected String name;

	/**
	 * x-coordinate in 2D system
	 */
	protected int x;

	/**
	 * y-coordinate in 2D system
	 */
	protected int y;

	/**
	 * range of the node antenna (it is assumed 
	 * characteristics for the )
	 */
	protected int range;

	/**
	 * maximum number of agents that may be installed on this node at the same
	 * time
	 */
	protected long memory;

	/**
	 * reference to the virtual machine installed on this node
	 */
	protected VirtualMachine<AvrVM> vm;

	/**
	 * value of timeout for the node to be switched off, after PowerDownEvent
	 * has been fired
	 */
	protected int timeout;

	/**
	 * list of hardware attached to this node
	 */
	protected List<AbstractResource> resourceList = new ArrayList<AbstractResource>();
	
	/**
	 * Default constructor
	 */
	public AbstractNode() {
		// empty
	}
	
	/**
	 * Sets default nodes values.
	 */
	public abstract void init(long productId, long locationId, List<AbstractResource> resourceList, String nodeDef);
	
	/**
	 * Sets initial values of the node with given values.
	 * 
	 * @param configNode
	 */
	/**
	 * Initilizes node with the values taken from XML configuration file.
	 * 
	 * @param node - instance of the auxiliary XML Schema built pl.edu.pw.pobicos.mw.RoversDocument.Rovers.Node object 
	 */
	public void init(NodeElement node) {
		this.name = node.getName();
		this.x = node.getX();
		this.y = node.getY();
		this.range = node.getRange();
		this.id = node.getId();
		this.memory = node.getMemory();
		
		try {
			initVirtualMachine();
		} catch(Exception e) {
			//LOG.error("Can't initialize node: " + this.getId(), e);
		}
	}
	
	protected void initVirtualMachine()
	{
		this.vm = new VirtualMachine<AvrVM>(AvrVM.class);
		this.vm.init();
	}
	
	public boolean supportsInstruction(long instr)
	{
		for(int i = 0; i < resourceList.size(); i++)
			if(((AbstractResource)resourceList.get(i)).supportsInstruction(instr))
				return true;
		return false;
	}

	public boolean raisesEvent(Event event)
	{
		for(int i = 0; i < resourceList.size(); i++)
			if(((AbstractResource)resourceList.get(i)).raisesEvent(event))
				return true;
		return false;
	}

	/**
	 * Returns ID of this node - unique name, controlled by ROVERS, chosen to
	 * distinguish this node from others.
	 * 
	 * @return ID of this node
	 */
	public long getId() 
	{
		return id;
	}

	/**
	 * Sets ID for this node - unique name, controlled by ROVERS, chosen to
	 * distinguish this node from others.
	 * 
	 * @param id -
	 *            new ID for this node
	 */
	public void setId(long id) 
	{
		this.id = id;
	}

	/**
	 * Returns name of this node - user-friendly name, created by user
	 * independently from other nodes names in the ROVERS system;
	 * 
	 * @return user-friendly name of this node
	 */
	public String getName() 
	{
		return name;
	}

	/**
	 * Sets the new user-friendly name for this node. This name is created
	 * independently from names of other nodes in the ROVERS system.
	 * 
	 * @param name -
	 *            new name to be set
	 */
	public void setName(String name) 
	{
		this.name = name;
	}

	/**
	 * Returns range of this node.
	 * 
	 * @return range of this node
	 */
	public int getRange() 
	{
		return range;
	}

	/**
	 * Sets new value for the range of this node.
	 * 
	 * @param range -
	 *            new value of range
	 */
	public void setRange(int range) 
	{
		this.range = range;
	}

	/**
	 * Returns x coordinate of this node (in 2D system).
	 * 
	 * @return x coordinate of this node
	 */
	public int getX() 
	{
		return x;
	}

	/**
	 * Sets the new value of x-coordinate for this node (in 2D system).
	 * 
	 * @param x -
	 *            new value of x-coordinate for this node
	 */
	public void setX(int x) 
	{
		this.x = x;
	}

	/**
	 * Returns y-coordinate of this node (in 2D system).
	 * 
	 * @return y-coordinate of this node
	 */
	public int getY() 
	{
		return y;
	}

	/**
	 * Sets the new value of y-coordinate for this node (in 2D system).
	 * 
	 * @param y -
	 *            new value of y-coordinate for this node
	 */
	public void setY(int y) 
	{
		this.y = y;
	}

	/**
	 * Returns maximum number of agents that can be installed on this node at
	 * the same time.
	 * 
	 * @return maximum number of agents that can be installed on this node at
	 *         the same time
	 */
	public long getMemory() 
	{
		return memory;
	}

	/**
	 * Sets maximum number of agents that can be installed on this node at the
	 * same time.
	 * 
	 * @param maxNumAgents -
	 *            maximum number of agents that can be on this node at the same
	 *            time
	 */
	public void setMemory(long memory) 
	{
		this.memory = memory;
	}

	/**
	 * Returns reference to the virtual machine installed on this node.
	 * 
	 * @return - reference to the virtual machine installed on this node
	 */
	public VirtualMachine<AvrVM> getVm() 
	{
		return vm;
	}

	/**
	 * // TODO check if it's necessary Returns value of timeout for the node to
	 * be switched off, after PowerDownEvent has been fired.
	 * 
	 * @return number of seconds for the node to be switched off, after
	 *         PowerDownEvent has been fired
	 */
	public int getTimeout() 
	{
		return timeout;
	}

	/**
	 * Sets a new value of timeout for the node to be switched off, after
	 * PowerDownEvent has been fired.
	 * 
	 * @param timeout -
	 *            new value of timeout -number of seconds for the node to be
	 *            switched off, after PowerDownEvent has been fired
	 */
	public void setTimeout(int timeout) 
	{
		this.timeout = timeout;
	}
	
	public void setLocationId(long id)
	{
		locationId = id;
	}
	
	public long getLocationId()
	{
		return locationId;
	}
	
	public long getProductId()
	{
		return productId;
	}
	
	public void setProductId(long id)
	{
		productId = id;
	}

	public void setNodeDef(String nodeDef) {
		this.nodeDef = nodeDef;
	}

	public String getNodeDef() {
		return nodeDef;
	}

	/**
	 * Returns list of hardware attached to this node.
	 * 
	 * @return list of hardware attached to this node.
	 */
	public List<AbstractResource> getResourceList() 
	{
		return resourceList;
	}

	@Override
	public final String toString() {
		String toString = "Node: " + name + " is " + ProductMap.getName(productId) + "@" + LocationMap.getName(locationId);
		return toString;
	}
}
