package pl.edu.pw.pobicos.mw.view.listener;

import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Cursor;
import org.eclipse.swt.graphics.ImageData;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.node.GenericNode;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

/**
 * This helper class adds a new node to the Rovers Network Graph.
 * 
 * @author Tomek
 */
public class AddNodeListener implements Listener {

	private Cursor cursor;

	private boolean addNodeChecked;

	private ImageData cursorImData;

	private NodesGraphManager nodesGraphManager;

	/**
	 * @param nodesGraphManager
	 */
	public AddNodeListener(NodesGraphManager nodesGraphManager) {
		this.nodesGraphManager = nodesGraphManager;
	}

	public void handleEvent(Event e) {
		if (e.type == SWT.MouseEnter) {
			cursorImData = new ImageData(this.getClass().getResourceAsStream(GraphicalSettings.addNode));
			if (addNodeChecked) {
				cursor = new Cursor(Display.getDefault(), cursorImData, 0, 0);
			} else {
				cursor = Display.getDefault().getSystemCursor(SWT.CURSOR_ARROW);
			}
			if (Display.getDefault().getActiveShell() != null) {
				Display.getDefault().getActiveShell().setCursor(cursor);
			}
		} else if (e.type == SWT.MouseExit || e.type == SWT.FocusOut) {
			if (Display.getDefault().getActiveShell() != null) {
				Display.getDefault().getActiveShell().setCursor(
						Display.getDefault().getSystemCursor(SWT.CURSOR_ARROW));
				if (cursor != null && cursor != Display.getDefault().getSystemCursor(SWT.CURSOR_ARROW))
					cursor.dispose();
			}
		} else if (e.type == SWT.MouseDoubleClick) {
			if (addNodeChecked) {
				int tempX = new Float(e.x / nodesGraphManager.getZoomFactor()).intValue();
				int tempY = new Float(e.y / nodesGraphManager.getZoomFactor()).intValue();

				GenericNode node = NodesManager.getInstance().createDefaultNode(tempX, tempY);
				if(NodesGraphManager.getInstance(NodesManager.getInstance()).getTempNodeDef() != null)
					NodesManager.getInstance().changeNodeType(node, new String(NodesGraphManager.getInstance(NodesManager.getInstance()).getTempNodeDef()));
				cursor = Display.getDefault().getSystemCursor(SWT.CURSOR_ARROW);
			}
		}
	}

	/**
	 * @param checked
	 */
	public void setAddNodeChecked(boolean checked) {
		addNodeChecked = checked;
	}
}