package pl.edu.pw.pobicos.mw.view.dialog;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.jface.dialogs.IDialogConstants;
import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.custom.VerifyKeyListener;
import org.eclipse.swt.events.TraverseEvent;
import org.eclipse.swt.events.TraverseListener;
import org.eclipse.swt.events.VerifyEvent;
import org.eclipse.swt.events.VerifyListener;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Group;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.swt.widgets.Shell;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.ConfigSetting;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;

/**
 * This class represents dialog that is part of graphical interface used to
 * modify settings of instances derived from AbstractNode class.
 * 
 * @author Marcin Smialek
 * @created 2006-09-11 17:32:45
 */
public class PobicosSettingsDialog extends Dialog {

	public PobicosSettingsDialog(Shell shell) {
		super(shell);
	}

	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Edit POBICOS settings");
	}

	@Override
	protected Control createDialogArea(Composite parent) {
		
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(1, false);
		container.setLayout(layout);

		//nameST = createField(container, "name", name, nameST);
		for(AbstractAgent agent : AgentsManager.getInstance().getAgents())
			if(AgentsManager.getInstance().isRootAgent(agent))
				if(AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(agent)).getConfigSettings().size() > 0)
				{
					Group g = new Group(container, SWT.SHADOW_IN);
					g.setLayout(new GridLayout(3, true));
					g.setText(agent.getName());
					g.setLayoutData(new GridData(SWT.FILL, SWT.FILL, false, false));
					for(final ConfigSetting set : AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(agent)).getConfigSettings())
					{
						CLabel l = new CLabel(g, SWT.NONE);
						l.setText(set.getName());
						l.setLayoutData(new GridData(SWT.END, SWT.CENTER, false, false));
						final StyledText st = new StyledText(g, SWT.BORDER);
						createField(g, set.getValue(), st);
						/*st.setText(set.getValue());
						st.setLayoutData(new GridData(SWT.FILL, SWT.CENTER, false, false));*/
						Button b = new Button(g, SWT.PUSH);
						b.setText("Change");
						final AbstractAgent tempAgent = agent;
						b.addListener(SWT.MouseUp, new Listener(){
							public void handleEvent(Event event) 
							{
								AgentsManager.getInstance().getBundle(AgentsManager.getInstance().getRoot(tempAgent)).setConfigSetting(set, st.getText());
								PobicosManager.getInstance().addEvent("ConfigSettingsChanged", null, tempAgent, null);
							}							
						});
					}
				}

		return container;
	}
	
	@Override
	protected void createButtonsForButtonBar(Composite parent) {
		createButton(parent, IDialogConstants.OK_ID, IDialogConstants.OK_LABEL,	true);
	}

	@Override
	protected void okPressed() {

		super.okPressed();
	}

	private StyledText createField(Composite container, final Object value, StyledText st) {
		/*CLabel label = new CLabel(container, SWT.NONE);
		label.setLayoutData(new GridData(SWT.END, SWT.CENTER, false, false));
		if (description != null)
			label.setText(description);*/

		//st = new StyledText(container, SWT.BORDER);
		if (value instanceof String)
			st.setText((String) value);
		else if (value instanceof Integer) {
			st.setText(Integer.toString((Integer) value));
		} else
			return st;

		st.setLayoutData(new GridData(SWT.FILL, SWT.FILL, false, false));
		st.addVerifyListener(new VerifyListener() {

			public void verifyText(VerifyEvent e) {
				if (value instanceof Integer) {
					e.doit = "0123456789".indexOf(e.text) >= 0;
				}
			}
		});
		st.addVerifyKeyListener(new VerifyKeyListener() {

			public void verifyKey(VerifyEvent event) {
				if (event.keyCode == SWT.CR) {
					event.doit = false;
					okPressed();
				} else if (event.keyCode == SWT.TAB) {
					event.doit = false;
				}
			}
		});

		st.addTraverseListener(new TraverseListener() {

			public void keyTraversed(TraverseEvent e) {
				switch (e.detail) {
				case SWT.TRAVERSE_TAB_NEXT:
				case SWT.TRAVERSE_TAB_PREVIOUS:
					e.doit = true;
					break;
				}
			}
		});

		return st;
	}

}
