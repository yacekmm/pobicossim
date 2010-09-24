package pl.edu.pw.pobicos.mw;

import org.eclipse.swt.graphics.Point;
import org.eclipse.ui.application.ActionBarAdvisor;
import org.eclipse.ui.application.IActionBarConfigurer;
import org.eclipse.ui.application.IWorkbenchWindowConfigurer;
import org.eclipse.ui.application.WorkbenchWindowAdvisor;

import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.network.Client;

import java.util.Properties;

public class ApplicationWorkbenchWindowAdvisor extends WorkbenchWindowAdvisor {

    public ApplicationWorkbenchWindowAdvisor(IWorkbenchWindowConfigurer configurer) {
        super(configurer);
    }

    public ActionBarAdvisor createActionBarAdvisor(IActionBarConfigurer configurer) {
        return new ApplicationActionBarAdvisor(configurer);
    }
    
    public void preWindowOpen() {
        IWorkbenchWindowConfigurer configurer = getWindowConfigurer();
		configurer.setInitialSize(new Point(900, 600));
		configurer.setShowCoolBar(true);
		configurer.setShowStatusLine(true);
        configurer.setTitle("POBICOS Middleware Simulator");
		configurer.setShowMenuBar(true);
		configurer.setShowPerspectiveBar(false);
//		try {
//			Runtime.getRuntime().exec("D:\\Studia\\Semestr VII\\PDI\\sim - JM\\MKS_sim_Compiled\\ss\\eclipse\\pobicos_ss.exe");
//		} catch (IOException e) {
//			e.printStackTrace();
//		}
    }
    
    public void postWindowOpen()
    {
		Properties props = new Properties();
		props.put("ip", pl.edu.pw.pobicos.mw.view.ConnectionView.st.getText());
		props.put("port", pl.edu.pw.pobicos.mw.view.ConnectionView.st1.getText());
		if(Client.getInstance().init(props))
		{
			SimulationsManager.setConnected(true);
			pl.edu.pw.pobicos.mw.view.ConnectionView.b.setText("Disconnect");
			pl.edu.pw.pobicos.mw.view.ConnectionView.st.setEnabled(false);
			pl.edu.pw.pobicos.mw.view.ConnectionView.st1.setEnabled(false);
			pl.edu.pw.pobicos.mw.view.ConnectionView.l2.setVisible(true);
		}
	}
}
