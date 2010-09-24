package pl.edu.pw.pobicos.mw.view.provider;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.jface.viewers.ITableLabelProvider;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.Conversions;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class AgentsTableLabelProvider implements ITableLabelProvider {

	private List<ILabelProviderListener> listeners = new ArrayList <ILabelProviderListener>();
	
	public Image getColumnImage(Object element, int columnIndex) {
		return null;
	}

	public String getColumnText(Object element, int columnIndex) {

		/*if (!(element instanceof GenericAgent))
			return null;
		GenericAgent agent = (GenericAgent) element;*/
		AbstractAgent agent = (AbstractAgent) element;
		switch (columnIndex) {
		case (0):
			return agent.getName();
		case (1):
			return Conversions.longToHexString(agent.getType(), 4);
		case (2):
			return String.valueOf(agent.getId());//Conversions.longToHexString(agent.getId(), 4);
		case (3):
			return (AgentsManager.getInstance().getBoss(agent) != null ? /*Conversions.longToHexString(*/String.valueOf(AgentsManager.getInstance().getBoss(agent).getId())/*, 4)*/ : "");
		case (4):
			return String.valueOf(AgentsManager.getInstance().getNode(agent).getId());
		default:
			return null;
		}
	}

	public void addListener(ILabelProviderListener listener) {
		listeners.add(listener);
	}

	public void dispose() {
	    //empty
	}

	public boolean isLabelProperty(Object element, String property) {
		return false;
	}

	public void removeListener(ILabelProviderListener listener) {
		listeners.remove(listener);
	}
}
