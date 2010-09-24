package pl.edu.pw.pobicos.mw.view;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.ScrolledComposite;
import org.eclipse.swt.events.MouseEvent;
import org.eclipse.swt.events.MouseMoveListener;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Canvas;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.ui.IActionBars;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.listener.NetworkPaintListener;
import pl.edu.pw.pobicos.mw.view.manager.AgentsGraphManager;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;
import pl.edu.pw.pobicos.mw.view.manager.SimulationGraphManager;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * This class represents sensor network view i.e. graphical representation of
 * nodes, agents etc.
 * 
 * @author Marcin Smialek
 * @created 2006-09-04 19:58:16
 */
public class NetworkView extends ViewPart {
	//private static final Log LOG = LogFactory.getLog(NetworkView.class);

	/**
	 * Unique view id
	 */
	public static final String ID = "view.NetworkView";

	private Composite parent;

	private Canvas canvas;

	private float zoomFactor;

	/**
	 * Constructor
	 */
	public NetworkView() {
		// empty
	}

	@Override
	public void createPartControl(Composite parent) {
		this.parent = parent;
		parent.setLayout(new FillLayout());
		ScrolledComposite scrolledComposite = new ScrolledComposite(parent, SWT.BORDER | SWT.H_SCROLL
				| SWT.V_SCROLL);
		canvas = new Canvas(scrolledComposite, SWT.DOUBLE_BUFFERED);
		scrolledComposite.setContent(canvas);
		NodesGraphManager.getInstance(NodesManager.getInstance()).setCanvas(canvas);
		SimulationGraphManager.getInstance().setCanvas(canvas);
		AgentsGraphManager.getInstance().setCanvas(canvas);
		canvas.addListener(SWT.Paint, new NetworkPaintListener());
		canvas.setBackground(GraphicalSettings.passiveNetworkViewBackground);
		zoomFactor = 1;
		showPosition();
		parent.addListener(SWT.Resize, new Listener() {
			public void handleEvent(Event event) {
				NodesGraphManager.getInstance(NodesManager.getInstance()).setPreferredSize();
				canvas.redraw();
			}
		});
	}

	@Override
	public void setFocus() {
		// empty
	}

	private void showPosition() {
		if (canvas == null) {
			return;
		}
		canvas.addMouseMoveListener(new MouseMoveListener() {
			public void mouseMove(MouseEvent e) {
				IActionBars bars = getViewSite().getActionBars();
				bars.getStatusLineManager().setMessage(
						new Float(e.x / zoomFactor).intValue() + " : " + new Float(e.y / zoomFactor).intValue());
			}
		});
	}

	/**
	 * Redraws specified elements with currently chosen zoom factor (out or in).
	 */
	public void zoom() {
		NodesGraphManager.getInstance(NodesManager.getInstance()).redraw(zoomFactor);
		SimulationGraphManager.getInstance().setZoomFactor(zoomFactor);
	}

	/**
	 * @return current zoom factor value
	 */
	public float getZoomFactor() {
		return this.zoomFactor;
	}

	/**
	 * Sets a new value for the zoom factor.
	 * @param zoomFactor - value to be set as zoom factor
	 */
	public void setZoomFactor(float zoomFactor) {
		this.zoomFactor = zoomFactor;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @return ?
	 */
	public Composite getParent() {
		return this.parent;
	}

	/**
	 * TODO MS - comments here
	 * @return ?
	 */
	public Canvas getCanvas() {
		return this.canvas;
	}

}