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
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.node.AbstractNode;
import pl.edu.pw.pobicos.mw.node.INodeListener;
import pl.edu.pw.pobicos.mw.view.provider.NodesTableContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.NodesTableLabelProvider;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * TODO MS - comments here
 * 
 * @author Tomasz Anuszewski
 */
public class NodesView extends ViewPart {

	// private static final Log LOG = LogFactory.getLog(NodesTableView.class);

	/**
	 * TODO MS - comments here
	 */
	public static final String ID = "view.NodesView";

	private static final String[] COLUMN_NAMES = { "name", "id", "memory", "used", "x", "y", "range" };

	private TableViewer tableViewer;

	private Table table;

	@Override
	public void createPartControl(Composite parent) {

		parent.setLayout(new FillLayout());
		createTable(parent);
		this.tableViewer = new TableViewer(this.table);
		this.tableViewer.setUseHashlookup(true);
		getSite().setSelectionProvider(this.tableViewer);
		tableViewer.setLabelProvider(new NodesTableLabelProvider());
		tableViewer.setContentProvider(new NodesTableContentProvider());
		tableViewer.setInput(NodesManager.getInstance());
		NodesManager.getInstance().addNodeListener(new INodeListener() {
			public void nodeChanged(AbstractNode node) {
				Display.getDefault().asyncExec(new Runnable(){
					public void run() {
						tableViewer.refresh();
					}
				});
			}
		});
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
		tableViewer.getControl().setFocus();
	}

	private void createTable(final Composite parent) {
		table = new Table(parent, SWT.SINGLE | SWT.BORDER | SWT.H_SCROLL | SWT.V_SCROLL | SWT.FULL_SELECTION
				| SWT.HIDE_SELECTION);
		table.setLinesVisible(true);
		table.setHeaderVisible(true);
		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			TableColumn column = new TableColumn(table, SWT.LEFT, i);
			column.setText(COLUMN_NAMES[i]);
			column.setWidth(80);
			if(i == 0)
				column.setWidth(100);
		}
		table.setBackground(GraphicalSettings.nodesViewBackground);

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
		tableViewer.setCellModifier(new NodesCellModifier());
	}

	private class NodesCellModifier implements ICellModifier {

		public boolean canModify(Object element, String property) {
			if ("name".equals(property)) 
				return true;
			else if ("memory".equals(property)) 
				return true;
			return false;
		}

		public Object getValue(Object o, String property) {
			AbstractNode nodeModel = (AbstractNode) o;
			if ("name".equals(property))
				return nodeModel.getName();
			if ("memory".equals(property))
				return Long.valueOf(nodeModel.getMemory()).toString();
			return null;
		}

		public void modify(Object element, String property, Object value) {
			if (element instanceof Item)
				element = ((Item) element).getData();
			AbstractNode nodeModel = (AbstractNode) element;
			if (nodeModel == null) {
				NodesManager.getInstance().updateNode(nodeModel);
				return;
			}

			if ("name".equals(property))
				nodeModel.setName((String) value);
			else if ("memory".equals(property))
			{

				long memoryUsed = 0;
				if(AgentsManager.getInstance().getAgentList(nodeModel) != null)
					for(AbstractAgent agent : AgentsManager.getInstance().getAgentList(nodeModel))
						memoryUsed += agent.getSize();
				if(memoryUsed <= getIntValue(value))
					nodeModel.setMemory(getIntValue(value));
				else
					nodeModel.setMemory(memoryUsed);
			}

			tableViewer.refresh();
			/*for (int i = 0; i < COLUMN_NAMES.length; i++) {
				if (property.equals(COLUMN_NAMES[i])) {
					table.getColumn(i).pack();
					break;
				}
			}*/
			NodesManager.getInstance().updateNode(nodeModel);
		}

		private int getIntValue(Object value) {
			String valueString = ((String) value).trim();
			if (valueString.length() == 0)
				valueString = "0";
			return Integer.parseInt(valueString);
		}
	}
}
