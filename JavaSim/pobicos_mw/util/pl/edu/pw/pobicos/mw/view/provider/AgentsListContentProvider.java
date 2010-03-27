package pl.edu.pw.pobicos.mw.view.provider;

import java.util.List;

import org.eclipse.jface.viewers.IStructuredContentProvider;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * @author Marcin Smialek
 */
public class AgentsListContentProvider implements IStructuredContentProvider {

	private static final Object[] emptyArray = new Object[0];

	public Object[] getElements(Object inputElement) {
		if (inputElement instanceof AbstractNode) {
			List<AbstractAgent> agents = AgentsManager.getInstance().getAgentList((AbstractNode) inputElement);
			if (agents != null && agents.size() > 0)
				return agents.toArray();
		}
		return emptyArray;
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer, Object oldInput, Object newInput) {
		// empty
	}

}
