package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.ILabelProvider;
import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class ApplicationLabelProvider implements ILabelProvider {

	/**
	 * Default constructor
	 */
	public ApplicationLabelProvider() {
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
		AbstractAgent agent = (AbstractAgent)element;
		String text = "";
		if(AgentsManager.getInstance().isRootAgent(agent))
			text += "[" + agent.getName() + "] ";
		else
			text += agent.getName();
		text += " (" + agent.getId()/*Conversions.longToHexString(agent.getId(), 4)*/ + ")";
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
