package pl.edu.pw.pobicos.mw.view.provider;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.viewers.ILabelProviderListener;
import org.eclipse.jface.viewers.ITableLabelProvider;
import org.eclipse.swt.graphics.Image;

import pl.edu.pw.pobicos.mw.logging.TraceElement;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class TraceTableLabelProvider implements ITableLabelProvider {

	private List<ILabelProviderListener> listeners = new ArrayList<ILabelProviderListener>();

	public Image getColumnImage(Object element, int columnIndex) {
		return null;
	}

	public String getColumnText(Object element, int columnIndex) {
		if (!(element instanceof TraceElement))
			return "-";
		TraceElement baseEvent = (TraceElement) element;
		switch (columnIndex) {
		case (0):
			return baseEvent.getMessage();
		case (1):
			if(baseEvent.getNode() != null)
				return String.valueOf(baseEvent.getNode().getId());
			return "";
		case (2):
			if(baseEvent.getAgent() != null)
				return String.valueOf(baseEvent.getAgent().getId());
			return "";
		case (3):
			return String.valueOf(baseEvent.getTime()/1000) + "." + String.valueOf((baseEvent.getTime()%1000/100));
		case (4):
			return baseEvent.getData().toString();
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
