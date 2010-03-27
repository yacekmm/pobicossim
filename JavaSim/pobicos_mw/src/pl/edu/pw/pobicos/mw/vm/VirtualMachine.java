package pl.edu.pw.pobicos.mw.vm;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;

/**
 * Creates and sets tasks for any virtual machine of type given in the parameter
 * @author Micha³ Krzysztof Szczerbak
 * @param <X> type of a virtual machine module
 */
public class VirtualMachine<X> {
	
	private X vm;

	/**
	 * Instantiates a virtual machine.
	 * @param clazz class of the virtual machine type
	 */
	public VirtualMachine(Class<X> clazz)
	{
		try {
			vm = clazz.newInstance();
		} catch (Exception e) {
		}
	}
	
	/**
	 * Initiates a virtual machine.
	 */
	@SuppressWarnings("unchecked")
	public void init()
	{
		Class[] params = new Class[0];
		Object[] os = new Object[0];
		try {
			vm.getClass().getMethod("init", params).invoke(vm, os);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * Sets a task fired by an agent for a virtual machine by executing a given address in the byte code.
	 * @param a agent firing a task
	 * @param addr address to start execution from
	 */
	@SuppressWarnings("unchecked")
	public void setTask(AbstractAgent a, int addr)
	{
		Class[] params = new Class[2];
		params[0] = AbstractAgent.class;
		params[1] = Integer.TYPE;
		Object[] os = new Object[2];
		os[0] = a;
		os[1] = addr;
		try {
			vm.getClass().getMethod("setTask", params).invoke(vm, os);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * Gets the virtual machine object o a parameterized type.
	 * @return virtual machine
	 */
	public X getVM()
	{
		return vm;
	}
}
