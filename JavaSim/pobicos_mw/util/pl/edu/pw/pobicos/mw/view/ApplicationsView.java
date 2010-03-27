package pl.edu.pw.pobicos.mw.view;

import org.eclipse.jface.viewers.DoubleClickEvent;
import org.eclipse.jface.viewers.IDoubleClickListener;
import org.eclipse.jface.viewers.TreeViewer;
import org.eclipse.swt.SWT;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.IAgentListener;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.view.provider.ApplicationContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.ApplicationLabelProvider;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class ApplicationsView extends ViewPart {

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "view.ApplicationsView";

	private TreeViewer treeViewer;

	@Override
	public void createPartControl(Composite parent) {
		this.treeViewer = new TreeViewer(parent, SWT.BORDER | SWT.MULTI);
		this.treeViewer.setLabelProvider(new ApplicationLabelProvider());
		this.treeViewer.setContentProvider(new ApplicationContentProvider());
		this.treeViewer.setInput(AgentsManager.getInstance());
		AgentsManager.getInstance().addAgentListener(new IAgentListener() {
			public void agentChanged(AbstractAgent agent) {
				Display.getDefault().asyncExec(new Runnable(){

					public void run() {
						ApplicationsView.this.treeViewer.refresh();
						ApplicationsView.this.treeViewer.expandAll();
					}
					
				});
			}
		});
		this.treeViewer.getTree().setBackground(GraphicalSettings.applicationsViewBackground);

		this.treeViewer.addDoubleClickListener(new IDoubleClickListener() {
			public void doubleClick(DoubleClickEvent event) {
				treeViewer.getTree().getSelection();
				/*Object o = treeViewer.getInput();
				if (o instanceof GenericAgent) {
					LOG.debug(((GenericAgent) o).getBossId());
				}*/
			}
		});

		getSite().setSelectionProvider(this.treeViewer);
		parent.setLayout(new FillLayout());
		this.treeViewer.expandAll();
	}

	@Override
	public void setFocus() {
		// empty
	}

	/**
	 * TODO MS - comments here
	 */
	public void clear() {
		if (this.treeViewer != null)
			this.treeViewer.getTree().removeAll();
	}
}
