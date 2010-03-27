package pl.edu.pw.pobicos.mw.view.dialog;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.custom.VerifyKeyListener;
import org.eclipse.swt.events.VerifyEvent;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Shell;

import pl.edu.pw.pobicos.mw.middleware.PobicosManager;

public class RoversDescriptionDialog extends Dialog {

	private StyledText st;
	
	private String roversDescription;
	
	public RoversDescriptionDialog(Shell parentShell) {
		super(parentShell);
	}

	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Configuration Descritpion");
	}

	@Override
	protected Control createDialogArea(Composite parent) {
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(2, false);
		container.setLayout(layout);
		
		CLabel label = new CLabel(container, SWT.NONE);
		label.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));
		label.setText("New name: ");
		
		st = new StyledText(container, SWT.BORDER);
		st.setLayoutData(new GridData(GridData.FILL, GridData.FILL, false,
				false));
		if (PobicosManager.getInstance() != null
				&& PobicosManager.getInstance().getPobicosName() != null) {
			st.setText(PobicosManager.getInstance().getPobicosName());
		}

		st.addVerifyKeyListener(new VerifyKeyListener() {
			public void verifyKey(VerifyEvent event) {
				if (event.keyCode == SWT.CR) {
					event.doit = false;
					okPressed();
				} else if (event.keyCode == SWT.TAB)
					event.doit = false;
			}
		});
		
		return container;
	}
	
	
	@Override
	protected void okPressed() {
		roversDescription = st.getText();
		PobicosManager.getInstance().setPobicosName(roversDescription);
		super.okPressed();
	}
	

	@Override
	protected void cancelPressed() {
		// TODO Auto-generated method stub
		super.cancelPressed();
	}

	public String getRoversDescritption() {
		return roversDescription;
	}

}
