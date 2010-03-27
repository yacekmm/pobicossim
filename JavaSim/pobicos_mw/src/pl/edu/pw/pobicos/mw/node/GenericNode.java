package pl.edu.pw.pobicos.mw.node;

import java.util.List;

import pl.edu.pw.pobicos.mw.resource.AbstractResource;
import pl.edu.pw.pobicos.mw.resource.ResourceFactory;
import pl.edu.pw.pobicos.mw.taxonomy.LocationTree;
import pl.edu.pw.pobicos.mw.taxonomy.ProductTree;

/**
 * Represents generic node in the ROVERS network not associated with any sensor resource.
 * 
 * @author Tomasz Anuszewski
 * @created 2007-11-25 11:01:15
 */
public class GenericNode extends AbstractNode {

	public void init(long productId, long locationId, List<AbstractResource> resourceList, String nodeDef) 
	{
		this.productId = ProductTree.getRootCode();
		this.locationId = LocationTree.getRootCode();
		this.resourceList.add(ResourceFactory.getInstance().createResource());
		initVirtualMachine();
	}
}
