package pl.edu.pw.pobicos.mw.port;

import java.io.BufferedWriter;
import java.io.FileOutputStream;
import java.io.FileWriter;

public class Export {

	public static void exportSimulation(SimulationElement sim, String filePath)
	{
		String xml = "<?xml version=\"1.0\"?>\n";	
		if(sim != null)
		{
			xml += "<simulation name=\"" + sim.getName() + "\" networkId=\"" + sim.getNetworkId() + "\">\n";
			for(EventElement event : sim.getEvents())
				xml += "\t<event code=\"" + event.getCode() + "\" nodeId=\"" + event.getNodeId() + "\" virtualTime=\"" + event.getVirtualTime() + "\" />\n";
			xml += "</simulation>";
		}
		try{
			FileWriter fstream = new FileWriter(filePath);
		    BufferedWriter out = new BufferedWriter(fstream);
		    out.write(xml);
		    out.close();
	    }catch (Exception e){}
	}

	public static void exportNetwork(NetworkElement net, String filePath)
	{
		String xml = "<?xml version=\"1.0\"?>\n";	
		if(net != null)
		{
			xml += "<network name=\"" + net.getName() + "\" id=\"" + net.getId() + "\">\n";
			for(NodeElement node : net.getNodes())
				xml += "\t<node name=\"" + node.getName() + "\" id=\"" + node.getId() + "\" x=\"" + node.getX() + "\" y=\"" + node.getY() + "\" memory=\"" + node.getMemory() + "\" range=\"" + node.getRange() + "\">" + stripTags((node.getNodeDef() == null ? "null" : node.getNodeDef())) + "</node>\n";
			for(ApplicationElement app : net.getApplications())
			{
				xml += "\t<application name=\"" + app.getName() + "\" nodeId=\"" + app.getNodeId() + "\">" + app.getName() + ".poabkp" + "</application>\n";
				try {
					FileOutputStream fos = new FileOutputStream(filePath.substring(0, filePath.lastIndexOf("\\")) + "\\" + app.getName() + ".poabkp");
					fos.write(app.getAppBundle());
				} catch (Exception e) {
				}
			}
			xml += "</network>";
		}
		try{
			FileWriter fstream = new FileWriter(filePath);
		    BufferedWriter out = new BufferedWriter(fstream);
		    out.write(xml);
		    out.close();
	    }catch (Exception e){}
	}
	
	private static String stripTags(String in)
	{
		String out = in.replaceAll("<", "&lt;");
		out = out.replaceAll(">", "&gt;");
		return out;
	}
}
