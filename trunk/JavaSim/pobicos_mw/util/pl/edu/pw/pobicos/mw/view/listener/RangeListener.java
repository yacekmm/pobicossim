package pl.edu.pw.pobicos.mw.view.listener;

import java.util.List;

import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.view.graph.NodeGraph;
import pl.edu.pw.pobicos.mw.view.manager.NodesGraphManager;

/**
 * This listener is redrawing nodes range indicator.
 * 
 * @author Tomek
 */
public class RangeListener implements Listener {

	private static int margin = 10;

	private int offset = 0;

	private boolean withinBoundary = false;

	private Point circleCentre = null;

	private int radius = 0;

	private NodeGraph nodeGraph;

	private NodesGraphManager nodesGraphManager;

	private List<NodeGraph> nodeGraphList;

	/**
	 * @param nodesGraphManager
	 */
	public RangeListener(NodesGraphManager nodesGraphManager) {
		this.nodesGraphManager = nodesGraphManager;
		nodeGraphList = nodesGraphManager.getNodeGraphList();
	}

	public void handleEvent(Event e) {
		if (!nodesGraphManager.isShowInfoChecked() || !nodesGraphManager.isRangeVisible())
			return;
		if (nodesGraphManager.getLockingObject() != null
				&& !(nodesGraphManager.getLockingObject() instanceof RangeListener)) {
			return;
		}

		if (e.type == SWT.MouseDown) {
			int tempX = new Float(e.x * 1.0F / nodesGraphManager.getZoomFactor()).intValue();
			int tempY = new Float(e.y * 1.0F / nodesGraphManager.getZoomFactor()).intValue();
			int tempMargin = new Float(margin * 1.0F / nodesGraphManager.getZoomFactor()).intValue();

			for (NodeGraph tempNodeGraph : nodeGraphList) {
				Point tempCircleCentre = new Point(tempNodeGraph.getNode().getX(), tempNodeGraph.getNode().getY());
				int tempRadius = tempNodeGraph.getNode().getRange();
				if (isCircleBoundary(new Point(tempX, tempY), tempCircleCentre, tempRadius, tempMargin)) {
					withinBoundary = true;
					nodeGraph = tempNodeGraph;
					circleCentre = tempCircleCentre;
					radius = tempRadius;
					offset = calculateDistance(new Point(tempX, tempY), circleCentre) - radius;
					break;
				}
			}
		} else if (e.type == SWT.MouseMove) {
			if (!withinBoundary) {
				return;
			}
			int tempX = new Float(e.x * 1.0F / nodesGraphManager.getZoomFactor()).intValue();
			int tempY = new Float(e.y * 1.0F / nodesGraphManager.getZoomFactor()).intValue();
			radius = calculateDistance(new Point(tempX, tempY), circleCentre) - offset;
			nodeGraph.getNode().setRange(radius);
		} else if (e.type == SWT.MouseUp) {
			if (withinBoundary) {
				NodesManager.getInstance().updateNode(nodeGraph.getNode());
			}
			withinBoundary = false;
			offset = 0;
			nodesGraphManager.setPreferredSize();
		}
	}

	private synchronized boolean isInCircle(Point p, Point circleCentre, int radius) {
		if (radius < 0)
			return false;
		int x = p.x - circleCentre.x;
		int y = p.y - circleCentre.y;
		if (x * x + y * y <= radius * radius)
			return true;
		return false;
	}

	// margin - describes circle resize active area
	private boolean isCircleBoundary(Point p, Point circleCentre, int radius, int margin) {
		if (radius < 0)
			return false;
		if (isInCircle(p, circleCentre, radius) && !isInCircle(p, circleCentre, radius - margin)) {
			return true;
		}
		return false;
	}

	// cc - circleCentre
	private int calculateDistance(Point p, Point cc) {
		double temp = (p.x - cc.x) * (p.x - cc.x) + (p.y - cc.y) * (p.y - cc.y);
		temp = Math.sqrt(temp);
		int distance = (new Double(temp)).intValue();
		return distance;
	}
}