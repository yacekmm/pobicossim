package pl.edu.pw.pobicos.ng.product;

import java.util.Vector;

import pl.edu.pw.pobicos.ng.instruction.InstructionMap;
import pl.edu.pw.pobicos.ng.resource.AbstractResource;

public class ProductMap {

	private long id;
	private String NodeDef;
	private AbstractProduct product;
	private String name;
	private Vector<SensorValue> sensors = new Vector<SensorValue>();
	
	public ProductMap(long id, String NodeDef, AbstractProduct product, String name)
	{
		this.setId(id);
		this.setNodeDef(NodeDef);
		this.setProduct(product);
		this.setName(name);
		for(AbstractResource res : product.getResources())
			for(Long code : res.instructionsSupported())
				if(InstructionMap.getReturn(code) != null)
					sensors.add(new SensorValue(InstructionMap.getName(code), InstructionMap.getReturn(code), new Long(777)));
	}
	/**
	 * @param id the id to set
	 */
	public void setId(long id) {
		this.id = id;
	}
	/**
	 * @return the id
	 */
	public long getId() {
		return id;
	}
	/**
	 * @param nodeDef the nodeDef to set
	 */
	public void setNodeDef(String nodeDef) {
		NodeDef = nodeDef;
	}
	/**
	 * @return the nodeDef
	 */
	public String getNodeDef() {
		return NodeDef;
	}
	/**
	 * @param product the product to set
	 */
	public void setProduct(AbstractProduct product) {
		this.product = product;
	}
	/**
	 * @return the product
	 */
	public AbstractProduct getProduct() {
		return product;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getName() {
		return name;
	}
	
	public Vector<SensorValue> getSensors()
	{
		return sensors;
	}
}
