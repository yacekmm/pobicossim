package pl.edu.pw.pobicos.mw.view;

import java.util.Properties;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.network.Client;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.NonGenericNode;

public class ConnectionView extends ViewPart {

	public static final String ID = "view.ConnectionView";
	
	public static Button b;
	public static StyledText st;

	public static StyledText st1;
	public static Label l2;
	
	private static ConnectionView instance;
	
	public static ConnectionView getInstance()
	{
		if(instance == null)
			instance = new ConnectionView();
		return instance;
	}
	
	public ConnectionView()
	{//empty
	}
	
	@Override
	public void createPartControl(final Composite parent) {
		GridLayout gl = new GridLayout();
		gl.numColumns = 7;
		parent.setLayout(gl);
		Label l = new Label(parent, SWT.NONE);
		l.setText("Server:");
		st = new StyledText(parent, SWT.BORDER);
		st.setText("127.0.0.1");
		Label l1 = new Label(parent, SWT.NONE);
		l1.setText("Port:");
		st1 = new StyledText(parent, SWT.BORDER);
		st1.setText("40007");
		l2 = new Label(parent, SWT.BOLD);
		l2.setForeground(parent.getDisplay().getSystemColor(SWT.COLOR_GREEN));
		l2.setVisible(false);
		l2.setText("OK!");
		b = new Button(parent, SWT.CHECK | SWT.PUSH);
		b.setText("   Connect   ");
		b.addListener(SWT.MouseUp, new Listener(){

			public void handleEvent(Event event) {
				if(SimulationsManager.isConnected())
				{
					disconnect();
				}
				else
				{
					Properties props = new Properties();
					props.put("ip", st.getText());
					props.put("port", st1.getText());
					if(Client.getInstance().init(props))
					{
						SimulationsManager.setConnected(true);
						b.setText("Disconnect");
						st.setEnabled(false);
						st1.setEnabled(false);
						l2.setVisible(true);
						
						for(AbstractNode node : NodesManager.getInstance().getNodes())
						{
							try
							{
								Client.getInstance().newNode((NonGenericNode)node);
								//System.out.println("Pod³¹czam NonGenerigNode: " + node.getId());
							}
							catch(ClassCastException e)
							{
								//Log.append("Próba od³¹czenia GenericNode (ClassCastException)");
								//Client.getInstance().newNode((GenericNode)node);
								//System.out.println("Pod³¹czam GenerigNode: " + node.getId());
							}
							
						}
					}
				}
			}
			
		});
	}

	@Override
	public void setFocus() {
		// empty
	}
	
	public static void disconnect()
	{
		SimulationsManager.setConnected(false);
		b.setText("Connect");
		Client.getInstance().disconnect();
		st.setEnabled(true);
		st1.setEnabled(true);
		l2.setVisible(false);
	}
	
}
