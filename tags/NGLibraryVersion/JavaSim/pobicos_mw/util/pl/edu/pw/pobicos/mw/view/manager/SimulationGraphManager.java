package pl.edu.pw.pobicos.mw.view.manager;

import java.util.ConcurrentModificationException;
import java.util.Map.Entry;

import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.widgets.Canvas;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.logging.TraceData;
import pl.edu.pw.pobicos.mw.message.AbstractMessage;
import pl.edu.pw.pobicos.mw.message.Command;
import pl.edu.pw.pobicos.mw.message.IMessageListener;
import pl.edu.pw.pobicos.mw.message.Report;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager.MessageStatus;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.view.graph.Arrow;

/**
 * Manager class for graphical representation of ROVERS simulation object.
 * It allows to visualize simulation processes.
 * 
 * @author Marcin Smialek
 * @created 2006-09-08 14:26:04
 */
public class SimulationGraphManager {

	//private static final Log LOG = LogFactory.getLog(SimulationGraphManager.class);

	private static SimulationGraphManager instance;

	private Canvas canvas;

	private float zoomFactor;

	private Canvas visual;

	private SimulationGraphManager() {
		zoomFactor = 1.0F;
		PobicosManager.getInstance().addMessageListener(new IMessageListener() {

			public void messageSent(AbstractMessage message) {
				if (canvas != null && !canvas.isDisposed())
					Display.getDefault().asyncExec(new Runnable(){
						public void run() {
							canvas.redraw();
						}			
					});
			}
		});
	}

	/**
	 * @return Instance to the SimulationGraphmanager
	 */
	public static SimulationGraphManager getInstance() {
		if (instance == null)
			instance = new SimulationGraphManager();
		return instance;
	}

	/**
	 * Visualizes flow of messages (reports, commands) in the ROVERS system. Each messages is supposed to be 
	 * displayed in the finite length of time.
	 * 
	 * @param event - Paint event that enables drawing
	 */
	public void drawMessages(Event event) {

		// reports
		try
		{
			for (Entry<Report, PobicosManager.MessageStatus> reports : PobicosManager.getInstance().getReports().entrySet()) {
				if (reports.getKey() == null)
					continue;
				AbstractAgent senderAgent = reports.getKey().getSender();
				AbstractAgent recipientAgent = reports.getKey().getRecipient();
				if (senderAgent == null || recipientAgent == null || senderAgent.getType() == -1 || AgentsManager.getInstance().getNode(recipientAgent) == null)
					continue;
	
				AbstractNode senderNode = AgentsManager.getInstance().getNode(senderAgent);
				AbstractNode recipientNode = AgentsManager.getInstance().getNode(recipientAgent);
	
				if(reports.getValue().equals(MessageStatus.SENT))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_GREEN));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GREEN));
				}
				else if(reports.getValue().equals(MessageStatus.LOST))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_RED));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_RED));
				}
				else if(reports.getValue().equals(MessageStatus.OLD))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));
				}
				connectNodes(senderNode, recipientNode, event);
			}
		}catch(ConcurrentModificationException e){}

		// commands
		try
		{
			for (Entry<Command, PobicosManager.MessageStatus> commands : PobicosManager.getInstance().getCommands().entrySet()) {
				if (commands.getKey() == null)
					continue;
				AbstractAgent senderAgent = commands.getKey().getSender();
				AbstractAgent recipientAgent = commands.getKey().getRecipient();
				if (senderAgent == null || recipientAgent == null || senderAgent.getType() == -1 || AgentsManager.getInstance().getNode(recipientAgent) == null)
					continue;

				AbstractNode senderNode = AgentsManager.getInstance().getNode(senderAgent);
				AbstractNode recipientNode = AgentsManager.getInstance().getNode(recipientAgent);
	
				if(commands.getValue().equals(MessageStatus.SENT))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_DARK_BLUE));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_DARK_BLUE));
				}
				else if(commands.getValue().equals(MessageStatus.LOST))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_MAGENTA));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_MAGENTA));
				}
				else if(commands.getValue().equals(MessageStatus.OLD))
				{
					event.gc.setForeground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));
					event.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));
				}
				connectNodes(senderNode, recipientNode, event);
			}
		}catch(ConcurrentModificationException e){}
	}

	private void connectNodes(AbstractNode sender, AbstractNode recipient, Event event) {
		// draw line
		event.gc.setLineWidth(1);
		int x1 = new Float(zoomFactor * sender.getX()).intValue();
		int y1 = new Float(zoomFactor * sender.getY()).intValue();
		int x2 = new Float(zoomFactor * recipient.getX()).intValue();
		int y2 = new Float(zoomFactor * recipient.getY()).intValue();
		event.gc.drawLine(x1, y1, x2, y2);

		// draw arrow
		Rectangle l1 = Arrow.getInstance().getArrowLineAB(new Point(x1, y1), new Point(x2, y2));
		event.gc.setLineWidth(1);
		event.gc.drawLine(l1.x, l1.y, l1.width, l1.height);

		Rectangle l2 = Arrow.getInstance().getArrowLineAC(new Point(x1, y1), new Point(x2, y2));
		event.gc.drawLine(l2.x, l2.y, l2.width, l2.height);
		event.gc.fillPolygon(Arrow.getInstance().getArrowCoordinates());
	}

	/**
	 * @param canvas
	 */
	public void setCanvas(Canvas canvas) {
		this.canvas = canvas;
	}

	/**
	 * @param zoomFactor
	 */
	public void setZoomFactor(float zoomFactor) {
		this.zoomFactor = zoomFactor;
	}

	public void setVisual(Canvas visual) 
	{
		this.visual = visual;
	}
	
	public void paintEvent(AbstractNode node, AbstractAgent agent, String message, long time, TraceData data)
	{
		visual.getBounds();
	}
}
