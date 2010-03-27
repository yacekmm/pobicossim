package pl.edu.pw.pobicos.ng.view.dialog;

import java.util.Properties;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.events.ModifyEvent;
import org.eclipse.swt.events.ModifyListener;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Shell;

import pl.edu.pw.pobicos.ng.network.Client;

/**
 * Dialog for network properties.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ConnectDialog extends Dialog {

	private String ip = "127.0.0.1", port="40007", descr="...";

	/**
	 * Constructor.
	 * @param parentShell shell
	 */
	public ConnectDialog(Shell parentShell) {
		super(parentShell);
	}

	/* (non-Javadoc)
	 * @see org.eclipse.jface.window.Window#configureShell(org.eclipse.swt.widgets.Shell)
	 */
	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Input simulation server address");
	}

	/** 
	 * Sets up the graphical elements.
	 * @see org.eclipse.jface.dialogs.Dialog#createDialogArea(org.eclipse.swt.widgets.Composite)
	 */
	@Override
	protected Control createDialogArea(Composite parent) {
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(2, false);
		container.setLayout(layout);

		CLabel address = new CLabel(container, SWT.NONE);
		address.setText("&Address:");
		address.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		StyledText addressST = new StyledText(container, SWT.BORDER);
		addressST.setLayoutData(new GridData(GridData.FILL, GridData.FILL, false,
				false));
		addressST.setText(ip);
		addressST.addModifyListener(new ModifyListener(){
			public void modifyText(ModifyEvent e) {
				ip = ((StyledText)e.getSource()).getText();
			}			
		});

		CLabel aport = new CLabel(container, SWT.NONE);
		aport.setText("&Port:");
		aport.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		StyledText aportST = new StyledText(container, SWT.BORDER);
		aportST.setLayoutData(new GridData(GridData.FILL, GridData.FILL, false,
				false));
		aportST.setText(port);
		aportST.addModifyListener(new ModifyListener(){
			public void modifyText(ModifyEvent e) {
				port = ((StyledText)e.getSource()).getText();
			}			
		});

		CLabel desc = new CLabel(container, SWT.NONE);
		desc.setText("&Description:");
		desc.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		StyledText descST = new StyledText(container, SWT.BORDER);
		descST.setLayoutData(new GridData(GridData.FILL, GridData.FILL, false,
				false));
		descST.setText(descr);
		descST.addModifyListener(new ModifyListener(){
			public void modifyText(ModifyEvent e) {
				descr = ((StyledText)e.getSource()).getText();
			}			
		});
		
		return container;
	}

	/**
	 * Instantiates client with given parameters.
	 * @see org.eclipse.jface.dialogs.Dialog#okPressed()
	 */
	@Override
	protected void okPressed() {
		super.okPressed();
		Properties p = new Properties();
		p.put("ip", ip);
    	p.put("port", port);
    	p.put("desc", descr);
        Client.getInstance().init(p);
	}
}
