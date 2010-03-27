package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.ITreeContentProvider;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class ApplicationContentProvider implements ITreeContentProvider {

	private static final Object[] EMPTY_ARRAY = new Object[0];
	
	
	public Object[] getChildren(Object parentElement) {
		AgentsManager agentMgr = AgentsManager.getInstance();
		if (parentElement instanceof AgentsManager)
			return agentMgr.getRootAgents().toArray();
		else if (parentElement instanceof AbstractAgent)
			return agentMgr.getEmployees((AbstractAgent) parentElement).toArray();
		else
			return EMPTY_ARRAY;
	}

	public Object getParent(Object element) {/*
		AgentsManager agentMgr = AgentsManager.getInstance();
		return agentMgr.getBoss((GenericAgent) element);
	*/
	return AgentsManager.getInstance();
	}

	public boolean hasChildren(Object element) {
		return getChildren(element).length > 0;
	}

	public Object[] getElements(Object inputElement) {
		return getChildren(inputElement);
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer, Object oldInput, Object newInput) {
		// empty
	}
}
