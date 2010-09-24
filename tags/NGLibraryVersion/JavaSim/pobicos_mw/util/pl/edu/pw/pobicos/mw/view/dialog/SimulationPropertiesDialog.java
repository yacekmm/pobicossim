package pl.edu.pw.pobicos.mw.view.dialog;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Spinner;

import pl.edu.pw.pobicos.mw.agent.ConfigSetting;
import pl.edu.pw.pobicos.mw.message.Message;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.taxonomy.ObjectClassQualifier;

public class SimulationPropertiesDialog extends Dialog {

	private Spinner messageDurationSpinner;
	
	private Spinner eventDurationSpinner;
	
	private CLabel messageDurationLabel;
	
	private CLabel eventDurationLabel;
	
	private Button numAgentsLimitedButton;
	
	private Button rangeLimitedButton;

	private CLabel reliabilityLabel;

	private Spinner reliabilitySpinner;
	
	public SimulationPropertiesDialog(Shell parentShell) {
		super(parentShell);
	}

	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Properties");
	}

	@Override
	protected Control createDialogArea(Composite parent) {
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(2, false);
		container.setLayout(layout);
		container.forceFocus();
		
		messageDurationLabel = new CLabel(container, SWT.NONE);
		messageDurationLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER, false, false));
	    messageDurationLabel.setText("Message delivery duration [msec]");
	    
	    messageDurationSpinner = new Spinner (container, SWT.BORDER);
		messageDurationSpinner.setMinimum(500);
		messageDurationSpinner.setMaximum(5000);
		messageDurationSpinner.setSelection(SimulationsManager.getInstance().getMessageDuration());
		messageDurationSpinner.setIncrement(100);
		messageDurationSpinner.setPageIncrement(500);
		messageDurationSpinner.setLayoutData(new GridData(GridData.END, GridData.FILL, false, false));
		messageDurationSpinner.pack();
		
		eventDurationLabel = new CLabel(container, SWT.NONE);
		eventDurationLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER, false, false));
	    eventDurationLabel.setText("Minimal time between events [msec]");
	    
	    eventDurationSpinner = new Spinner (container, SWT.BORDER);
		eventDurationSpinner.setMinimum(500);
		eventDurationSpinner.setMaximum(5000);
		eventDurationSpinner.setSelection(SimulationsManager.getInstance().getStepDuration());
		eventDurationSpinner.setIncrement(100);
		eventDurationSpinner.setPageIncrement(500);
		eventDurationSpinner.setLayoutData(new GridData(GridData.END, GridData.FILL, false, false));
		eventDurationSpinner.pack();
		
		reliabilityLabel = new CLabel(container, SWT.NONE);
		reliabilityLabel.setLayoutData(new GridData(GridData.END, GridData.CENTER, false, false));
		reliabilityLabel.setText("Message delivery probability [%]");
	    
		reliabilitySpinner = new Spinner (container, SWT.BORDER);
		reliabilitySpinner.setMinimum(1);
		reliabilitySpinner.setMaximum(100);
		reliabilitySpinner.setSelection(PobicosManager.getInstance().getReliability());
		reliabilitySpinner.setIncrement(1);
		reliabilitySpinner.setPageIncrement(1);
		reliabilitySpinner.setLayoutData(new GridData(GridData.END, GridData.FILL, false, false));
		reliabilitySpinner.pack();
		
		numAgentsLimitedButton = new Button(container, SWT.CHECK | SWT.RIGHT);
		numAgentsLimitedButton.setLayoutData(new GridData(SWT.BEGINNING, SWT.FILL, false, false));
		numAgentsLimitedButton.setText("Limited nodes' memory resources");
		numAgentsLimitedButton.setSelection(PobicosManager.getInstance().isMemoryLimited());
		
		@SuppressWarnings("unused")
		CLabel tempLabel = new CLabel(container, SWT.NONE);
		
		rangeLimitedButton = new Button(container, SWT.CHECK | SWT.RIGHT);
		rangeLimitedButton.setLayoutData(new GridData(SWT.BEGINNING, SWT.FILL, false, false));
		rangeLimitedButton.setText("Limited nodes' antenna range");
		rangeLimitedButton.setSelection(PobicosManager.getInstance().isRangeLimited());
		
		@SuppressWarnings("unused")
		CLabel info0 = new CLabel(container, SWT.NONE);
		
		CLabel info11 = new CLabel(container, SWT.NONE);
		info11.setLayoutData(new GridData(SWT.END, SWT.FILL, false, false));
		info11.setText("MSG_DATA_MAXLEN");
		CLabel info12 = new CLabel(container, SWT.NONE);
		info12.setLayoutData(new GridData(SWT.BEGINNING, SWT.FILL, false, false));
		info12.setText(new Short(Message.MSG_DATA_MAXLEN).toString());
		
		CLabel info21 = new CLabel(container, SWT.NONE);
		info21.setLayoutData(new GridData(SWT.END, SWT.FILL, false, false));
		info21.setText("CONFIG_VAL_MAXLEN");
		CLabel info22 = new CLabel(container, SWT.NONE);
		info22.setLayoutData(new GridData(SWT.BEGINNING, SWT.FILL, false, false));
		info22.setText(new Short(ConfigSetting.CONFIG_VAL_MAXLEN).toString());
		
		CLabel info31 = new CLabel(container, SWT.NONE);
		info31.setLayoutData(new GridData(SWT.END, SWT.FILL, false, false));
		info31.setText("OBJ_TAXONOMIES");
		CLabel info32 = new CLabel(container, SWT.NONE);
		info32.setLayoutData(new GridData(SWT.BEGINNING, SWT.FILL, false, false));
		info32.setText(new Integer(ObjectClassQualifier.OBJ_TAXONOMIES).toString());
		
		return super.createDialogArea(parent);
	}

	@Override
	protected void okPressed() {
		SimulationsManager.getInstance().setMessageDuration(messageDurationSpinner.getSelection());
		SimulationsManager.getInstance().setStepDuration(eventDurationSpinner.getSelection());
		PobicosManager.getInstance().setMemoryLimited(numAgentsLimitedButton.getSelection());
		PobicosManager.getInstance().setRangeLimited(rangeLimitedButton.getSelection());
		PobicosManager.getInstance().setReliability(reliabilitySpinner.getSelection());
		
		super.okPressed();
	}
}
