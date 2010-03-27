package pl.edu.pw.pobicos.mw.taxonomy;

public class ObjectClassQualifier {
	public static final int OBJ_TAXONOMIES = 2;

	public int numberOfRanges;
	public ObjectClassRange ranges[];
	
	public class ObjectClassRange
	{
		public ObjectClass mostSpecific = new ObjectClass();
		public ObjectClass mostAbstract = new ObjectClass();
	}
	
	public class ObjectClass
	{
		public long code = -1;
	}

	public void createRanges() 
	{
		ranges = new ObjectClassRange[numberOfRanges];
		for(int i = 0; i < numberOfRanges; i++)
			ranges[i] = new ObjectClassRange();
	}
}
