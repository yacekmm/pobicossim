package pl.edu.pw.pobicos.mw.view.provider;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.viewers.ITreeContentProvider;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class HostsContentProvider implements ITreeContentProvider {

	private static final Object[] EMPTY_ARRAY = new Object[0];
	
	
	public Object[] getChildren(Object parentElement) {
		AgentsManager agentMgr = AgentsManager.getInstance();
		if (parentElement instanceof NodesManager)
		{
			if(((NodesManager)parentElement).getNodes() == null)
				return EMPTY_ARRAY;
			List<AbstractNode> nodes = new ArrayList<AbstractNode>();
			for(AbstractNode node : ((NodesManager)parentElement).getNodes())
				if(agentMgr.getAgentList(node) != null)
					if(agentMgr.getAgentList(node).size() > 0)
						nodes.add(node);
			return nodes.toArray();
		}
		else if (parentElement instanceof AbstractNode)
			return (agentMgr.getAgentList((AbstractNode)parentElement) == null ? EMPTY_ARRAY : agentMgr.getAgentList((AbstractNode)parentElement).toArray());
		else
			return EMPTY_ARRAY;
	}

	public Object getParent(Object element) {
		return NodesManager.getInstance();
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
