package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.ILabelProvider;
import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class HostsLabelProvider implements ILabelProvider {

	/**
	 * Default constructor
	 */
	public HostsLabelProvider() {
	}

	public Image getImage(Object element) {
		return null;
	}

	public String getText(Object element) {
		/*if (element instanceof GenericAgent) {
			String text = ((GenericAgent) element).getType() + " ("
					+ ((GenericAgent) element).getClass().getSimpleName() + ")";
			return text;
		}*/
		String text = "";
		if(element instanceof AbstractAgent)
		{
			AbstractAgent agent = (AbstractAgent)element;
			if(AgentsManager.getInstance().isRootAgent(agent))
				text += "[" + agent.getName() + "] ";
			else
				text += agent.getName();
			text += " (" + agent.getId()/*Conversions.longToHexString(agent.getId(), 4)*/ + ")";
		}
		else
		{
			AbstractNode node = (AbstractNode)element;
			text += "<" + node.getName() + " (" + node.getId() + ")>";
		}
		return text;
	}

	public void addListener(ILabelProviderListener listener) {
	}

	public void dispose() {
	}

	public boolean isLabelProperty(Object element, String property) {
		return false;
	}

	public void removeListener(ILabelProviderListener listener) {
	}
}
