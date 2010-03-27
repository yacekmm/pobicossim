package pl.edu.pw.pobicos.mw.view.action;

import java.util.LinkedHashMap;
import java.util.Map;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.action.IMenuCreator;
import org.eclipse.jface.resource.ImageDescriptor;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.view.NetworkView;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class ZoomOutAction extends Action implements ISelectionListener,
		IWorkbenchAction {

	static final String ID = "action.zoomOut";

	private IWorkbenchWindow window;

	private int numSteps;

	private float ratio;

	private float minimum;

	private Menu menu;

	private Map <String, Float> zoomValues;

	private float currentZoomFactor;

	/**
	 * Constructor
	 * 
	 * @param window
	 */
	public ZoomOutAction(IWorkbenchWindow window) {

		this.window = window;
		numSteps = 4;
		ratio = 0.8F;
		minimum = 1.0F / 16;
		currentZoomFactor = -1;
		setId(ID);
		//setText("-");
		setImageDescriptor(new ImageDescriptor() {
		    @Override
		    public ImageData getImageData() {
			return new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.minus));
		    }
		});
		setToolTipText("Zoom out");
		setEnabled(false);
		ActionContainer.add(ID, this);
		
		zoomValues = new LinkedHashMap<String, Float>();

		setMenuCreator(new IMenuCreator() {

			public void dispose() {
			}

			public Menu getMenu(Control parent) {

				if (menu == null || menu.isDisposed()) {
					menu = new Menu(parent);

					for (int i = 0; i < numSteps; i++) {
						Float tempValue = new Float(Math.pow(0.5, i));
						String tempKey = "1 : " + new Float(1.0F / tempValue).intValue();
						zoomValues.put(tempKey, tempValue);

						final MenuItem menuItem = new MenuItem(menu, SWT.NONE);
						menuItem.setText(tempKey);
						menuItem.addSelectionListener(new SelectionAdapter() {

							@Override
							public void widgetSelected(SelectionEvent e) {
								currentZoomFactor = zoomValues.get(menuItem
										.getText());
								run();
							}
						});
					}
				}
				return menu;
			}

			public Menu getMenu(Menu parent) {
				return null;
			}

		});

	}

	@Override
	public void run() {

		NetworkView networkView = (NetworkView) window.getWorkbench()
				.getActiveWorkbenchWindow().getActivePage().findView(
						NetworkView.ID);
		if (networkView == null)
			return;
		if (currentZoomFactor == -1) 
			currentZoomFactor = networkView.getZoomFactor() * ratio;
		if (currentZoomFactor > minimum)
			networkView.setZoomFactor(currentZoomFactor);

		networkView.zoom();
		currentZoomFactor = -1;
	
}

	public void selectionChanged(IWorkbenchPart part, ISelection selection) {
	}

	public void dispose() {
	}

}
