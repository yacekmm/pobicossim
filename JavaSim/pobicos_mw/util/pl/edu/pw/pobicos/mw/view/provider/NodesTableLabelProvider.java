package pl.edu.pw.pobicos.mw.view.provider;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.jface.viewers.ITableLabelProvider;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class NodesTableLabelProvider implements ITableLabelProvider {

	private List<ILabelProviderListener> listeners = new ArrayList<ILabelProviderListener>();

	public Image getColumnImage(Object element, int columnIndex) {
		return null;
	}

	public String getColumnText(Object element, int columnIndex) {
		AbstractNode nodeModel = (AbstractNode) element;

		switch (columnIndex) {
		case (0):
			return nodeModel.getName();
		case (1):
			return String.valueOf(nodeModel.getId());
		case (2):
			return (Long.toString(nodeModel.getMemory()));
		case (3):
			long memoryUsed = 0;
			if(AgentsManager.getInstance().getAgentList(nodeModel) != null)
				for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(nodeModel))
					memoryUsed += agent.getSize();
			else
				memoryUsed = 0;
			return (Long.toString(memoryUsed));
		case (4):
			return (Integer.toString(nodeModel.getX()));
		case (5):
			return (Integer.toString(nodeModel.getY()));
		case (6):
			return (Integer.toString(nodeModel.getRange()));
		default:
			return "";
		}
	}

	public void addListener(ILabelProviderListener listener) {
		listeners.add(listener);
	}

	public void dispose() {
		// empty
	}

	public boolean isLabelProperty(Object element, String property) {
		return false;
	}

	public void removeListener(ILabelProviderListener listener) {
		listeners.remove(listener);
	}
}
