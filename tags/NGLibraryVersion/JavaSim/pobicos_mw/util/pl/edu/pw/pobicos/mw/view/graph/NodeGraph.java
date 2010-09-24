package pl.edu.pw.pobicos.mw.view.graph;

import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Color;
import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;

import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * Instances of this class are graphical representatives of objects derived from AbstractNode class.
 * 
 * @author Marcin Smialek
 * @created 2006-09-09 18:13:30
 */
public class NodeGraph {

	private final static Color circleColor = GraphicalSettings.nodeCircle;

	private final static Color nodeNameColor = GraphicalSettings.nodeName;
	
	private static int nameMargin = 8;
	
	private static int edgeMargin = 8;
	
	private static int edgeMarginOffset = 2;

	private AbstractNode node;

	private Image mainIcon;

	private Rectangle iconBounds;

	/**
	 * @param node - reference to node model object
	 */
	public NodeGraph(AbstractNode node) {
		this.node = node;
		if (node.getResourceList().size() == 1)
			mainIcon = node.getResourceList().get(0).getIcon();
		if (node.getResourceList().size() > 1)
			mainIcon = node.getResourceList().get(1).getIcon();
		if (mainIcon == null)
			mainIcon = new Image(Display.getCurrent(), this.getClass().getResourceAsStream(GraphicalSettings.genericNode));

		iconBounds = new Rectangle(0, 0, mainIcon.getImageData().width, mainIcon.getImageData().height);
	}

	/**
	 * @param event
	 * @param zoomFactor
	 */
	public void drawIcon(Event event, float zoomFactor) {
		iconBounds.x = new Float(node.getX() * zoomFactor).intValue() - iconBounds.width / 2;
		iconBounds.y = new Float(node.getY() * zoomFactor).intValue() - iconBounds.height / 2;
		event.gc.drawImage(mainIcon, iconBounds.x, iconBounds.y);
	}

	/**
	 * @param event
	 * @param zoomFactor
	 */
	public void drawName(Event event, float zoomFactor) {
		iconBounds.x = new Float(node.getX() * zoomFactor).intValue() - iconBounds.width / 2;
		iconBounds.y = new Float(node.getY() * zoomFactor).intValue() - iconBounds.height / 2;

		//String text = node.getName() + " (" + node.getId() + ")";
		
		event.gc.setLineWidth(1);
		event.gc.setLineStyle(SWT.LINE_CUSTOM);
		event.gc.setForeground(nodeNameColor);
		Point size = event.gc.stringExtent(node.getName());
		Point p = new Point(new Float(node.getX() * zoomFactor).intValue() - size.x / 2, new Float(node.getY()
				* zoomFactor).intValue() + iconBounds.height / 2);
		event.gc.drawString(node.getName(), p.x, p.y + nameMargin, true);
	}

	/**
	 * @param event
	 * @param zoomFactor
	 */
	public void drawCircle(Event event, float zoomFactor) {
		int x = new Float(node.getX() * zoomFactor).intValue();
		int y = new Float(node.getY() * zoomFactor).intValue();
		int r = new Float(node.getRange() * zoomFactor).intValue();
		iconBounds.x = new Float(node.getX() * zoomFactor).intValue() - iconBounds.width / 2;
		iconBounds.y = new Float(node.getY() * zoomFactor).intValue() - iconBounds.height / 2;

		event.gc.setLineWidth(2);
		event.gc.setLineStyle(SWT.LINE_DASH);
		event.gc.setForeground(circleColor);
		event.gc.drawOval(x - r, y - r, 2 * r, 2 * r);
	}

	/**
	 * Draws current node
	 * 
	 * @param event
	 * @param zoomFactor
	 * @param numAgents
	 */
	public void drawAgent(Event event, float zoomFactor, int numAgents) {
		drawEdge(event, zoomFactor, GraphicalSettings.agentInstalled, edgeMargin, false);
		drawNumAgents(event, zoomFactor, GraphicalSettings.agentInstalled,
				GraphicalSettings.agentNumBackground, edgeMargin, numAgents);
	}

	private void drawEdge(Event event, float zoomFactor, Color color, int margin, boolean filledOut) {

		iconBounds.x = new Float(node.getX() * zoomFactor).intValue() - iconBounds.width / 2;
		iconBounds.y = new Float(node.getY() * zoomFactor).intValue() - iconBounds.height / 2;

		Rectangle edgeBounds = new Rectangle(iconBounds.x - margin, iconBounds.y - margin, iconBounds.width + 2
				* margin, iconBounds.height + 2 * margin);

		if (filledOut) {
			event.gc.setBackground(color);
			event.gc.setLineStyle(SWT.LINE_CUSTOM);
			event.gc.fillRectangle(edgeBounds);
		} else {
			event.gc.setForeground(color);
			event.gc.setLineWidth(2);
			event.gc.setLineStyle(SWT.LINE_DOT);
			event.gc.drawRectangle(edgeBounds);
		}
	}

	private void drawNumAgents(Event event, float zoomFactor, Color foregroundColor, Color backgroundColor,
			int margin, int numAgents) {

		if (numAgents <= 1)
			return;
		String numAgentsStr = " x " + numAgents + " ";

		iconBounds.x = new Float(node.getX() * zoomFactor).intValue() - iconBounds.width / 2;
		iconBounds.y = new Float(node.getY() * zoomFactor).intValue() - iconBounds.height / 2;

		event.gc.setLineWidth(3);
		event.gc.setLineStyle(SWT.LINE_SOLID);
		event.gc.setForeground(foregroundColor);
		event.gc.setBackground(backgroundColor);
		Point size = event.gc.stringExtent(numAgentsStr);
		Point p = new Point(new Float(node.getX() * zoomFactor).intValue() - size.x / 2, new Float(node.getY()
				* zoomFactor).intValue() - iconBounds.height / 2 - iconBounds.height);
		p = new Point(new Float(node.getX() * zoomFactor).intValue() - size.x / 2, iconBounds.y - margin
				- edgeMarginOffset - size.y / 2);
		event.gc.drawString(numAgentsStr, p.x, p.y, false);

	}

	/**
	 * Distinguishes this NodeGraph as the current one by drawing its edge in a special way.
	 * 
	 * @param event -
	 * @param zoomFactor - current zoom factor
	 */
	public void checkAsCurrent(Event event, float zoomFactor) {
		int margin = 3;
		drawEdge(event, zoomFactor, GraphicalSettings.currentNode, margin, true);
	}

	/**
	 * Checks this NodeGraph as the neighbour of he currently selected node.
	 * 
	 * @param event - to be drawn
	 * @param zoomFactor - current zoom factor
	 */
	public void checkAsNeighbour(Event event, float zoomFactor) {
		int margin = 3;
		drawEdge(event, zoomFactor, GraphicalSettings.currentNeighbour, margin, true);
	}

	public void checkAsSubNetwork(Event event, float zoomFactor) {
		int margin = 3;
		drawEdge(event, zoomFactor, GraphicalSettings.currentSubNetwork, margin, true);
	}

	/*private void drawCircle(Event event, float zoomFactor, Color color, int margin) {
	 int x = new Float(node.getX() * zoomFactor).intValue();
	 int y = new Float(node.getY() * zoomFactor).intValue();

	 iconBounds.x = new Float(node.getX() * zoomFactor).intValue()- iconBounds.width / 2;
	 iconBounds.y = new Float(node.getY() * zoomFactor).intValue()- iconBounds.height / 2;

	 int r = new Float(Math.sqrt(iconBounds.width / 2 * iconBounds.width / 2
	 + iconBounds.height / 2 * iconBounds.height / 2)).intValue();
	 r += margin;
	 event.gc.setBackground(color);
	 event.gc.setLineWidth(2);
	 event.gc.setLineStyle(SWT.LINE_DOT);
	 event.gc.drawOval(x - r, y - r, 2 * r, 2 * r);
	 }*/

	// --- getters and setters ---
	/**
	 * @return X coordinate for the node
	 */
	public int getX() {
		return node.getX();
	}

	/**
	 * @return Y coordinate for the node
	 */
	public int getY() {
		return node.getY();
	}

	/**
	 * @return Node's range
	 */
	public int getRange() {
		return node.getRange();
	}

	/**
	 * @return Reference to the node's model
	 */
	public AbstractNode getNode() {
		return node;
	}

	/**
	 * @return Bounds of the node's icon
	 */
	public Rectangle getIconBounds() {
		return this.iconBounds;
	}

	/**
	 * @return Graphic representation of the node
	 */
	public Image getNodeIcon() {
		if (mainIcon == null) {
			mainIcon = new Image(Display.getCurrent(), this.getClass().getResourceAsStream(GraphicalSettings.genericNode));
		}
		return mainIcon;
	}

}
