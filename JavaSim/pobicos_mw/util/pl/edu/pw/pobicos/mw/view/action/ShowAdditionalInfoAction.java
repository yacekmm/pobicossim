package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IMenuCreator;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

public class ShowAdditionalInfoAction extends Action implements
		ActionFactory.IWorkbenchAction {

	final static String ID = "action.showAdditionalInfo";

	// private IWorkbenchWindow window;

	private Menu menu;

	private boolean infoVisible;

	private boolean nameVisible;

	private boolean circleVisible;

	private boolean neighboursVisible;

	private boolean agentsVisible;

	public ShowAdditionalInfoAction(IWorkbenchWindow window) {
		// this.window = window;
		setId(ID);
		setText("");
		setImageDescriptor(new ImageDescriptor() {
			@Override
			public ImageData getImageData() {
				return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.info));
			}
		});
		setToolTipText("Showing additional graphical information");
		setEnabled(false);
		ActionContainer.add(ID, this);
		setInitialFlags();
		setMenuCreator(new IMenuCreator() {

			public void dispose() {
			}

			public Menu getMenu(Control parent) {
				if (menu != null)
					return menu;

				if (menu == null || menu.isDisposed())
					menu = new Menu(parent);

				final MenuItem nameItem = new MenuItem(menu, SWT.CHECK);
				nameItem.setText("name");
				nameItem.setSelection(nameVisible);
				nameItem.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						nameVisible = nameItem.getSelection();
						infoVisible = true;
						refresh();
					}
				});

				final MenuItem circleItem = new MenuItem(menu, SWT.CHECK);
				circleItem.setText("range");
				circleItem.setSelection(circleVisible);
				circleItem.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						circleVisible = circleItem.getSelection();
						infoVisible = true;
						refresh();
					}
				});

				final MenuItem neighboursItem = new MenuItem(menu, SWT.CHECK);
				neighboursItem.setText("neighbours");
				neighboursItem.setSelection(neighboursVisible);
				neighboursItem.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						neighboursVisible = neighboursItem.getSelection();
						infoVisible = true;
						refresh();
					}
				});

				final MenuItem agentsItem = new MenuItem(menu, SWT.CHECK);
				agentsItem.setText("agents");
				agentsItem.setSelection(agentsVisible);
				agentsItem.addSelectionListener(new SelectionAdapter() {
					@Override
					public void widgetSelected(SelectionEvent e) {
						agentsVisible = agentsItem.getSelection();
						infoVisible = true;
						refresh();
					}
				});

				return menu;
			}

			public Menu getMenu(Menu parent) {
				return null;
			}

		});
	}

	public void setInitialFlags() {
		infoVisible = true;
		nameVisible = true;
		circleVisible = true;
		neighboursVisible = true;
		agentsVisible = true;
	}

	@Override
	public void run() {
		infoVisible = !infoVisible;
		refresh();
	}

	public void dispose() {
		// empty
	}

	public void refresh() {
		if (NodesGraphManager.getInstance(NodesManager.getInstance()) != null) {
			NodesGraphManager.getInstance(NodesManager.getInstance()).setNameVisible(nameVisible);
			NodesGraphManager.getInstance(NodesManager.getInstance()).setRangeVisible(circleVisible);
			NodesGraphManager.getInstance(NodesManager.getInstance()).setNeighboursVisible(neighboursVisible);
			NodesGraphManager.getInstance(NodesManager.getInstance()).setAgentsVisible(agentsVisible);
			NodesGraphManager.getInstance(NodesManager.getInstance()).setShowInfoChecked(infoVisible);
		}
	}
}