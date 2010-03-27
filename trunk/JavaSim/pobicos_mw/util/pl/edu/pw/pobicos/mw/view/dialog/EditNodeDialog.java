package pl.edu.pw.pobicos.mw.view.dialog;

import java.util.ArrayList;
import java.util.List;

import org.eclipse.jface.dialogs.Dialog;
import org.eclipse.jface.dialogs.MessageDialog;
import org.eclipse.jface.viewers.ListViewer;
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
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Shell;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.GenericNode;
import pl.edu.pw.pobicos.mw.node.NodesFactory;
import pl.edu.pw.pobicos.mw.view.provider.AgentsListContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.ApplicationLabelProvider;

/**
 * This class represents dialog that is part of graphical interface used to
 * modify settings of instances derived from AbstractNode class.
 * 
 * @author Marcin Smialek
 * @created 2006-09-11 17:32:45
 */
public class EditNodeDialog extends Dialog {

	private StyledText nameST;

	private StyledText idST;

	private StyledText xST;

	private StyledText yST;

	private StyledText rangeST;

	private StyledText maxNumAgentsST;

	private String name;

	private String id;

	private int x;

	private int y;

	private int range;

	private long memory;

	private AbstractNode node;

	private List<AbstractAgent> agents = new ArrayList<AbstractAgent>();

	public EditNodeDialog(Shell parentShell, AbstractNode nodeModel) {
		super(parentShell);

		this.node = nodeModel;
		name = nodeModel.getName();
		id = String.valueOf(nodeModel.getId());
		x = nodeModel.getX();
		y = nodeModel.getY();
		range = nodeModel.getRange();
		memory = nodeModel.getMemory();
		//agents = AgentsManager.getInstance().getAgentList(nodeModel);
	}

	@Override
	protected void configureShell(Shell newShell) {
		super.configureShell(newShell);
		newShell.setText("Edit node");
	}

	@Override
	protected Control createDialogArea(Composite parent) {
		Composite container = new Composite(parent, SWT.NONE);
		GridLayout layout = new GridLayout(2, false);
		container.setLayout(layout);

		nameST = createField(container, "name", name, nameST);
		idST = createField(container, "id", id, idST);
		idST.setEditable(false);
		xST = createField(container, "x", x, xST);
		yST = createField(container, "y", y, yST);
		rangeST = createField(container, "range", range, rangeST);
		maxNumAgentsST = createField(container, "memory", memory, maxNumAgentsST);

		createAgentsItem(container, "agents");

		return container;
	}

	@Override
	protected void okPressed() {
		if (name.length() < 1 || id.length() < 1 || xST.getText().length() < 1 || yST.getText().length() < 1
				|| rangeST.getText().length() < 1 || maxNumAgentsST.getText().length() < 1) {
			MessageDialog.openWarning(getShell(), "Incorrect data", "All fields must be filled in.");
			return;
		}

		name = nameST.getText();
		id = idST.getText();
		x = Integer.parseInt(xST.getText());
		y = Integer.parseInt(yST.getText());
		range = Integer.parseInt(rangeST.getText());
		memory = Long.parseLong(maxNumAgentsST.getText());

		super.okPressed();
	}

	private StyledText createField(Composite container, String description, final Object value, StyledText st) {
		CLabel label = new CLabel(container, SWT.NONE);
		label.setLayoutData(new GridData(SWT.END, SWT.CENTER, false, false));
		if (description != null)
			label.setText(description);

		st = new StyledText(container, SWT.BORDER);
		if (value instanceof String)
			st.setText((String) value);
		else if (value instanceof Integer) {
			st.setText(Integer.toString((Integer) value));
		}
		else if (value instanceof Long) {
			st.setText(Long.toString((Long) value));
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

	private void createAgentsItem(Composite container, String description) {
		if (agents == null || agents.isEmpty())
			return;

		CLabel label = new CLabel(container, SWT.NONE);
		label.setLayoutData(new GridData(GridData.END, GridData.CENTER, false, false));
		if (description != null)
			label.setText(description);

		ListViewer agentsListViewer = new ListViewer(container, SWT.BORDER);
		agentsListViewer.getControl().setLayoutData(new GridData(GridData.FILL, GridData.FILL, false, false));
		agentsListViewer.setContentProvider(new AgentsListContentProvider());
		agentsListViewer.setLabelProvider(new ApplicationLabelProvider());
		agentsListViewer.setInput(node);

	}

	// -------- getters --------

	public GenericNode getNodeModel() {
		GenericNode nodeModel = 
			(GenericNode) NodesFactory.createNode();
		nodeModel.setName(name);
		//nodeModel.setId(id);
		nodeModel.setX(x);
		nodeModel.setY(y);
		nodeModel.setRange(range);
		nodeModel.setMemory(memory);
		return nodeModel;
	}

	public String getId() {
		return id;
	}

	public long getMemory() {
		return memory;
	}

	public String getName() {
		return name;
	}

	public int getRange() {
		return range;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

}
