package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.IStructuredContentProvider;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class NodesTableContentProvider implements IStructuredContentProvider {

	/**
	 * TODO MS - comments here
	 */
	public static final Object[] EMPTY_ARRAY = new Object[0];

	public Object[] getElements(Object o) {
		if (o instanceof NodesManager)
			return ((NodesManager) o).getNodes().toArray();
		return EMPTY_ARRAY;
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer, Object oldInput, Object newInput) {
		// empty
	}
}
