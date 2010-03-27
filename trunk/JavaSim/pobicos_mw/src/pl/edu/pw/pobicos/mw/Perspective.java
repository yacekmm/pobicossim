package pl.edu.pw.pobicos.mw;

import org.eclipse.ui.IFolderLayout;
import org.eclipse.ui.IPageLayout;
import org.eclipse.ui.IPerspectiveFactory;

import pl.edu.pw.pobicos.mw.view.AgentsView;
import pl.edu.pw.pobicos.mw.view.ApplicationsView;
import pl.edu.pw.pobicos.mw.view.ConnectionView;
import pl.edu.pw.pobicos.mw.view.HostsView;
import pl.edu.pw.pobicos.mw.view.LogView;
import pl.edu.pw.pobicos.mw.view.NetworkView;
import pl.edu.pw.pobicos.mw.view.NodesView;
import pl.edu.pw.pobicos.mw.view.SimulationView;
import pl.edu.pw.pobicos.mw.view.VisualView;

public class Perspective implements IPerspectiveFactory {

	public void createInitialLayout(IPageLayout layout) {
		layout.setEditorAreaVisible(false);
		IFolderLayout topLeft = layout.createFolder("topLeft", IPageLayout.LEFT, 0.7f, layout.getEditorArea());
		topLeft.addView(NetworkView.ID);
		topLeft.addView(VisualView.ID);
		//layout.addView(NetworkView.ID, IPageLayout.LEFT, 0.7f, layout.getEditorArea());
		layout.addStandaloneView(SimulationView.ID, true, IPageLayout.RIGHT, 0.4f, layout.getEditorArea());
		IFolderLayout topRight = layout.createFolder("topRight", IPageLayout.TOP, 0.4f, SimulationView.ID);
		topRight.addView(NodesView.ID);
		topRight.addView(HostsView.ID);
		topRight.addView(AgentsView.ID);
		topRight.addView(ApplicationsView.ID);
		//layout.addStandaloneView(LogView.ID, false, IPageLayout.BOTTOM, 0.3f, SimulationTableView.ID);
		layout.addStandaloneView(ConnectionView.ID, false, IPageLayout.BOTTOM, 0.9f, SimulationView.ID);
		
		layout.getViewLayout(NetworkView.ID).setCloseable(false);
		layout.getViewLayout(VisualView.ID).setCloseable(false);
		layout.getViewLayout(NodesView.ID).setCloseable(false);
		layout.getViewLayout(ApplicationsView.ID).setCloseable(false);
		layout.getViewLayout(AgentsView.ID).setCloseable(false);
		layout.getViewLayout(SimulationView.ID).setCloseable(false);
		layout.getViewLayout(LogView.ID).setCloseable(false);
		layout.getViewLayout(ConnectionView.ID).setCloseable(false);
		layout.getViewLayout(HostsView.ID).setCloseable(false);
		layout.getViewLayout(NetworkView.ID).setMoveable(false);
		layout.getViewLayout(VisualView.ID).setMoveable(false);
		layout.getViewLayout(NodesView.ID).setMoveable(false);
		layout.getViewLayout(ApplicationsView.ID).setMoveable(false);
		layout.getViewLayout(AgentsView.ID).setMoveable(false);
		layout.getViewLayout(SimulationView.ID).setMoveable(false);
		layout.getViewLayout(LogView.ID).setMoveable(false);
		layout.getViewLayout(ConnectionView.ID).setMoveable(false);
		layout.getViewLayout(HostsView.ID).setMoveable(false);
	}
}
