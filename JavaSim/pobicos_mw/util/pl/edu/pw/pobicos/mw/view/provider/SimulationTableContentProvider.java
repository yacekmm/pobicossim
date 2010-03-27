package pl.edu.pw.pobicos.mw.view.provider;

import org.eclipse.jface.viewers.IStructuredContentProvider;
import org.eclipse.jface.viewers.TableViewer;
import org.eclipse.jface.viewers.Viewer;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.simulation.ISimulationListener;
import pl.edu.pw.pobicos.mw.simulation.Simulation;

/**
 * TODO MS - comments here
 * 
 * @author Marcin Smialek
 */
public class SimulationTableContentProvider implements IStructuredContentProvider {

	// private static final Log LOG =
	// LogFactory.getLog(SimulationTableContentProvider.class);

	/**
	 * 
	 */
	public static final Object[] EMPTY_ARRAY = new Object[0];

	public Object[] getElements(Object o) {
		if (o instanceof Simulation)
			return SimulationsManager.getInstance().getSimulation().getEventList().toArray();
		return EMPTY_ARRAY;
	}

	public void dispose() {
		// empty
	}

	public void inputChanged(Viewer viewer,	Object oldInput, Object newInput) {
		final TableViewer tableViewer = (TableViewer) viewer;
		SimulationsManager.getInstance().addSimulationListener(new ISimulationListener() {

			public void simulationChanged(Event event) {
				tableViewer.refresh();
			}

		});

	}
}
