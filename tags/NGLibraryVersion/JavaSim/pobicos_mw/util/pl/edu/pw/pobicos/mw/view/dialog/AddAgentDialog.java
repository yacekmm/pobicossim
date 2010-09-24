package pl.edu.pw.pobicos.mw.view.dialog;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Shell;

public class AddAgentDialog extends Dialog {

	private StyledText nameST;
	private StyledText idST;
	private StyledText nodeIdST;
	
	private String name;
	private String id;
	private String nodeId;
	
	public AddAgentDialog(Shell parentShell) {
		super(parentShell);
	}

	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Add micro-agent");
	}

	@Override
	protected Control createDialogArea(Composite parent) {
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(2, false);
		container.setLayout(layout);

		CLabel nameLabel = new CLabel(container, SWT.NONE);
		nameLabel.setText("&Name:");
		nameLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		nameST = new StyledText(container, SWT.BORDER);
		nameST.setLayoutData(new GridData(GridData.FILL, GridData.FILL, false,
				false));

		CLabel idLabel = new CLabel(container, SWT.NONE);
		idLabel.setText("&ID:");
		idLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		idST = new StyledText(container, SWT.BORDER);
		idST.setLayoutData(new GridData(GridData.FILL, GridData.FILL, true,
				false));

		CLabel nodeIdLabel = new CLabel(container, SWT.NONE);
		nodeIdLabel.setText("&Node id:");
		nodeIdLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER,
				false, false));

		nodeIdST = new StyledText(container, SWT.BORDER);
		GridData gridData = new GridData(GridData.FILL, GridData.FILL, true,
				false);
		gridData.widthHint = convertHeightInCharsToPixels(20);
		nodeIdST.setLayoutData(gridData);

		return container;
	}

	@Override
	protected void okPressed() {
		name = nameST.getText();
		id = idST.getText();
		nodeId = nodeIdST.getText();

		if (name.length() < 1 || id.length() < 1 || nodeId.length() < 1) {
			MessageDialog.openWarning(getShell(), "Incorrect data",
					"All fields must be filled in.");
			return;
		}

		super.okPressed();
	}

	public String getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public String getNodeId() {
		return nodeId;
	}

	
}
