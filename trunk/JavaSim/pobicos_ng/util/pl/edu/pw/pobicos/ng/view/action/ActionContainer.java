package pl.edu.pw.pobicos.ng.view.action;

import java.util.HashMap;
import java.util.Map;

import org.eclipse.jface.action.Action;

/**
 * Stores all actions.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ActionContainer {

	private static Map<String, Action> actions =  new HashMap <String, Action> ();
	
	/**
	 * Adds action to the actions map
	 * 
	 * @param id
	 * @param action
	 */
	public static void add(String id, Action action) {
		actions.put(id, action);
	}
	
	/**
	 * @param id - unique name of the given action
	 * @return requested action
	 */
	public static Action getAction(String id) {
		return actions.get(id);
	}
}
