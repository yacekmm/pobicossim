package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.IStructuredContentProvider;
import org.eclipse.jface.viewers.TableViewer;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.logging.ITraceListener;
import pl.edu.pw.pobicos.mw.logging.Trace;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class TraceTableContentProvider implements IStructuredContentProvider {

	/**
	 * TODO MS - comments here
	 */
	public static final Object[] EMPTY_ARRAY = new Object[0];

	public Object[] getElements(Object o) {
		if (o.equals(Trace.class))
			return Trace.getEvents().toArray();
		return EMPTY_ARRAY;
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer,	Object oldInput, Object newInput) {
		final TableViewer tableViewer = (TableViewer) viewer;
		Trace.addTraceListener(new ITraceListener() {

			public void traceChanged() {
				tableViewer.refresh();
			}

		});

	}
}
