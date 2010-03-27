package pl.edu.pw.pobicos.mw;

import org.eclipse.swt.graphics.Color;
import org.eclipse.swt.widgets.Display;

/**
 * Keeps references to selected graphical elements such as colors and images.
 * @author Marcin Smialek, Michal Szczerbak
 */
public class GraphicalSettings {
	
	public static Color passiveNetworkViewBackground = new Color(Display.getDefault(), 77, 77, 77);
	public static Color activeNetworkViewBackground = new Color(Display.getDefault(), 237, 237, 237);
	public static Color nodesViewBackground = new Color(Display.getDefault(), 152, 255, 152);
	public static Color agentsViewBackground = new Color(Display.getDefault(), 173, 216, 230);
	public static Color applicationsViewBackground = new Color(Display.getDefault(), 244, 164, 96);
	public static Color hostsViewBackground = new Color(Display.getDefault(), 230, 103, 97);
	public static Color simulationViewBackground = new Color(Display.getDefault(), 255, 255, 153);
	public static Color traceViewBackground = new Color(Display.getDefault(), 216, 191, 216);
	public static Color nodeName = new Color(Display.getDefault(), 18, 174, 18);
	public static Color nodeCircle = new Color(Display.getDefault(), 177, 186, 177);
	public static Color agentInstalled = new Color(Display.getDefault(), 146, 113, 208);
	public static Color agentNumBackground = new Color(Display.getDefault(), 30, 220, 220);
	public static Color currentNode = new Color(Display.getDefault(), 184, 174, 174);
	public static Color currentNeighbour = new Color(Display.getDefault(), 100, 240, 120);
	public static Color currentSubNetwork = new Color(Display.getDefault(), 177, 240, 177);
	public static Color simulationViewBackgroundCurrent = new Color(Display.getDefault(), 207, 207, 57);

	public static String genericNode = "/resources/icons/generic.gif";
	public static String nonGenericNode = "/resources/icons/nongeneric.gif";
	public static String addNode = "/resources/icons/addnode.gif";
	public static String addSimpleNode = "/resources/icons/addnode2.gif";
	public static String panic = "/resources/icons/panic.gif";
	public static String settings = "/resources/icons/settings.gif";
	public static String info = "/resources/icons/info.gif";
	public static String play = "/resources/icons/play.gif";
	public static String step = "/resources/icons/step.gif";
	public static String stop = "/resources/icons/stop.gif";
	public static String plus = "/resources/icons/plus.gif";
	public static String minus = "/resources/icons/minus.gif";
	
}
