package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.IStructuredContentProvider;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.middleware.AgentsManager;

/**
 * TODO MS - comments here
 * @author Marcin Smialek
 */
public class AgentsTableContentProvider implements IStructuredContentProvider {

	/**
	 * TODO MS - comments here
	 */
	public static final Object[] EMPTY_ARRAY = new Object[0];
	
	public Object[] getElements(Object o) {
		if (o instanceof AgentsManager)
			return ((AgentsManager) o).getAgents().toArray();
		return null;
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer, Object oldInput, Object newInput) {
		// empty
	}
}
