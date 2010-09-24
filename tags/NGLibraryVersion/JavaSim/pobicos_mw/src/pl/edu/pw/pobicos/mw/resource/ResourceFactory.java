package pl.edu.pw.pobicos.mw.resource;

import java.io.File;

import org.w3c.dom.Node;

public class ResourceFactory {
	private static ResourceFactory instance;
	
	public static ResourceFactory getInstance()
	{
		if(instance == null)
			instance = new ResourceFactory();
		return instance;
	}

	public AbstractResource createResource()
	{
		return new GenericResource();
	}

	public AbstractResource createResource(File resourceXml, String path)
	{
		return new NonGenericResource(resourceXml, path);
	}

	public AbstractResource createResource(Node item) {
		return new NonGenericResource(item);
	}
}
