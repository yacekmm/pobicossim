package pl.edu.pw.pobicos.ng.product;

import java.util.Vector;

import pl.edu.pw.pobicos.ng.resource.AbstractResource;

public class AbstractProduct {

	private long locationId, typeId;
	private Vector<AbstractResource> resources;
	
	public AbstractProduct()
	{	//empty
	}
	
	public void init(long locationId, long typeId, Vector<AbstractResource> resources)
	{
		this.locationId = locationId;
		this.typeId = typeId;
		this.resources = resources;
	}
	
	public Vector<AbstractResource> getResources()
	{
		return resources;
	}
	
	public long getLocationId()
	{
		return locationId;
	}
	
	public long getTypeId()
	{
		return typeId;
	}
}
