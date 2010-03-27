package pl.edu.pw.pobicos.mw.view;

import org.eclipse.jface.viewers.CellEditor;
import org.eclipse.jface.viewers.ICellModifier;
import org.eclipse.jface.viewers.TableViewer;
import org.eclipse.jface.viewers.TextCellEditor;
import org.eclipse.swt.SWT;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Item;
import org.eclipse.swt.widgets.Table;
import org.eclipse.swt.widgets.TableColumn;
import org.eclipse.swt.widgets.Text;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.agent.AbstractAgent;
import pl.edu.pw.pobicos.mw.agent.IAgentListener;
import pl.edu.pw.pobicos.mw.middleware.AgentsManager;
import pl.edu.pw.pobicos.mw.view.provider.AgentsTableContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.AgentsTableLabelProvider;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class AgentsView extends ViewPart {

	//private static final Log LOG = LogFactory.getLog(AgentsTableView.class);

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "view.AgentsView";

	private static final String[] COLUMN_NAMES = { "name", "type", "id", "bossId", "nodeId" };

	private TableViewer tableViewer;

	private Table table;

	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());

		this.table = new Table(parent, SWT.SINGLE | SWT.BORDER | SWT.H_SCROLL | SWT.V_SCROLL | SWT.FULL_SELECTION
				| SWT.HIDE_SELECTION);
		this.table.setLinesVisible(true);
		this.table.setHeaderVisible(true);
		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			TableColumn column = new TableColumn(this.table, SWT.LEFT, i);
			column.setText(COLUMN_NAMES[i]);
			column.setWidth(80);
			if(i == 0)
				column.setWidth(100);
		}
		this.table.setBackground(GraphicalSettings.agentsViewBackground);

		tableViewer = new TableViewer(table);
		tableViewer.setUseHashlookup(true);
		tableViewer.setLabelProvider(new AgentsTableLabelProvider());
		tableViewer.setContentProvider(new AgentsTableContentProvider());
		tableViewer.setInput(AgentsManager.getInstance());
		getSite().setSelectionProvider(tableViewer);
		AgentsManager.getInstance().addAgentListener(new IAgentListener() {
			public void agentChanged(AbstractAgent agent) {
				Display.getDefault().asyncExec(new Runnable(){

					public void run() {
						tableViewer.refresh();
					}
					
				});
			}
		});
		createCellEditors();
	}

	@Override
	public void setFocus() {
		// empty
	}

	private void createCellEditors() {
		tableViewer.setColumnProperties(COLUMN_NAMES);
		CellEditor[] editors = new TextCellEditor[COLUMN_NAMES.length];

		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			CellEditor editor = null;
			if (COLUMN_NAMES[i].equals("gluglus")) {
				// TODO
			} else {
				editor = new TextCellEditor(table);
				((Text) editor.getControl()).setTextLimit(60);
			}
			editors[i] = editor;
		}
		tableViewer.setCellEditors(editors);
		tableViewer.setCellModifier(new AgentsCellModifier());
	}

	private class AgentsCellModifier implements ICellModifier {
		public boolean canModify(Object element, String property) {
			if ("name".equals(property)) {
				return true;
			}
			return false;
		}

		public Object getValue(Object o, String property) {
			AbstractAgent agentModel = (AbstractAgent) o;
			if ("name".equals(property))
				return agentModel.getName();
			if ("id".equals(property))
				return agentModel.getType();
			if ("boss".equals(property))
				return AgentsManager.getInstance().getBoss(agentModel).getType();
			if ("node".equals(property))
				return AgentsManager.getInstance().getNode(agentModel).getId();
			return null;
		}

		public void modify(Object element, String property, Object value) {
			if (element instanceof Item)
				element = ((Item) element).getData();
			AbstractAgent agentModel = (AbstractAgent) element;
			if (agentModel == null) {
				AgentsManager.getInstance().updateAgent(agentModel);
				return;
			}
			if ("name".equals(property))
				agentModel.setName((String) value);
			if ("id".equals(property));
				//agentModel.setId((String) value);
			if ("bossId".equals(property));
				//agentModel.setBoss((String) value);
			if ("nodeId".equals(property))
				// TODO Check this out agentModel.setNodeId((String) value);

				tableViewer.refresh();
			/*for (int i = 0; i < COLUMN_NAMES.length; i++) {
				if (property.equals(COLUMN_NAMES[i])) {
					table.getColumn(i).pack();
					break;
				}
			}*/
			AgentsManager.getInstance().updateAgent(agentModel);
		}
	}

}
