package pl.edu.pw.pobicos.mw.node;

import java.util.List;

import pl.edu.pw.pobicos.mw.resource.AbstractResource;
import pl.edu.pw.pobicos.mw.resource.ResourceFactory;

/**
 * This class represent node associated with LED hardware and allow to observe
 * and control LED properties (e.g. whether it is turned on).
 * 
 * @author Tomasz Anuszewski
 * @created 2007-11-25 12:49:08
 */
public class NonGenericNode extends AbstractNode {
	
	public void init(long productId, long locationId, List<AbstractResource> resourceList, String nodeDef) {
		this.productId = productId;
		this.locationId = locationId;
		this.resourceList.add(ResourceFactory.getInstance().createResource());
		for(AbstractResource res : resourceList)
			this.resourceList.add(res);
		initVirtualMachine();
		this.setNodeDef(nodeDef);
	}
}
