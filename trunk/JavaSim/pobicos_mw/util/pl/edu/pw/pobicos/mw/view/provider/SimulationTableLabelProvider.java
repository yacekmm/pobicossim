package pl.edu.pw.pobicos.mw.view.provider;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.jface.viewers.ITableLabelProvider;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.event.Event;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class SimulationTableLabelProvider implements ITableLabelProvider {

	private List<ILabelProviderListener> listeners = new ArrayList<ILabelProviderListener>();

	public Image getColumnImage(Object element, int columnIndex) {
		return null;
	}

	public String getColumnText(Object element, int columnIndex) {
		if (!(element instanceof Event))
			return "";
		Event baseEvent = (Event) element;
		switch (columnIndex) {
		case (0):
			return baseEvent.getName();
		case (1):
			if(baseEvent.getType().equals("node"))
				return "node " + baseEvent.getNode().getId();
			if(baseEvent.getType().equals("agent"))
				return "agent " + baseEvent.getAgent().getId();//Conversions.longToHexString(baseEvent.getAgent().getId(), 4);
			return baseEvent.getSource();
		case (2):
			return String.valueOf(baseEvent.getVirtualTime()/1000) + "." + String.valueOf((baseEvent.getVirtualTime()%1000/100));
		default:
			return "!";
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
