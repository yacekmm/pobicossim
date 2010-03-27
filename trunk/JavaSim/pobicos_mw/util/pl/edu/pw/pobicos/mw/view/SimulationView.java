package pl.edu.pw.pobicos.mw.view;

import java.util.Collections;

import org.eclipse.jface.action.MenuManager;
import org.eclipse.jface.action.Separator;
import org.eclipse.jface.viewers.CellEditor;
import org.eclipse.jface.viewers.ICellModifier;
import org.eclipse.jface.viewers.TableViewer;
import org.eclipse.jface.viewers.TextCellEditor;
import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Item;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.Table;
import org.eclipse.swt.widgets.TableColumn;
import org.eclipse.swt.widgets.TableItem;
import org.eclipse.swt.widgets.Text;
import org.eclipse.ui.IWorkbenchActionConstants;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.simulation.ISimulationListener;
import pl.edu.pw.pobicos.mw.simulation.Simulation;
import pl.edu.pw.pobicos.mw.view.action.DeleteEventAction;
import pl.edu.pw.pobicos.mw.view.provider.SimulationTableContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.SimulationTableLabelProvider;
import pl.edu.pw.pobicos.mw.GraphicalSettings;

/**
 * This view represents a simulation tab in the tabbed pannel in main simulation window of the simulator application.
 * 
 * @author Tomasz Anuszewski
 */
public class SimulationView extends ViewPart {

	//private static final Log LOG = LogFactory.getLog(SimulationTableView.class);

	public static final String ID = "view.SimulationView";

	private static final String[] COLUMN_NAMES = { "name", "source", "time[s]" };

	private TableViewer tableViewer;

	private Table table;

	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());
		this.table = new Table(parent, SWT.SINGLE | SWT.BORDER | SWT.H_SCROLL | SWT.V_SCROLL | SWT.FULL_SELECTION
				| SWT.HIDE_SELECTION);
		this.table.setLinesVisible(true);
		this.table.setHeaderVisible(true);
		this.table.setBackground(GraphicalSettings.simulationViewBackground);
		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			TableColumn column = new TableColumn(table, SWT.LEFT, i);
			column.setText(COLUMN_NAMES[i]);
			column.setWidth(80);
			if(i == 1)
				column.setWidth(100);
		}
		SimulationsManager.getInstance().addSimulationListener(new ISimulationListener() {
			public void simulationChanged(
			Event event) {
				table.setFocus();
				int i = SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex();
				if (i < 0) {
					table.deselectAll();
				} else {
					table.setSelection(i);
				}
			}
		});
		table.addSelectionListener(new SelectionAdapter() {
			@Override
			public void widgetSelected(
			SelectionEvent e) {
				Simulation simulation = SimulationsManager.getInstance().getSimulation();
				if (simulation != null) {
					highlightCurrent();
					//table.getItem(simulation.getCurrentSimulationIndex()).setBackground(GraphicalSettings.simulationViewBackgroundCurrent);
					//simulation.setCurrentSimulationIndex(simulation.getCurrentSimulationIndex()/*table.getSelectionIndex()*/);
				}
			}
		});

		tableViewer = new TableViewer(table);
		tableViewer.setUseHashlookup(true);
		tableViewer.setLabelProvider(new SimulationTableLabelProvider());
		tableViewer.setContentProvider(new SimulationTableContentProvider());
		tableViewer.setInput(SimulationsManager.getInstance().getSimulation());
		//getSite().setSelectionProvider(tableViewer);
		SimulationsManager.getInstance().addSimulationListener(new ISimulationListener() {

			public void simulationChanged(Event event) {
				tableViewer.refresh();
				highlightCurrent();
			}
		});

		createCellEditors();
		makeActions();
		SimulationsManager.getInstance().setView(this, parent.getDisplay());
		setTimeInLabel(this, 0);
	}
	
	private void highlightCurrent()
	{
		for(TableItem item : table.getItems())
			item.setBackground(GraphicalSettings.simulationViewBackground);
		table.getItem(SimulationsManager.getInstance().getSimulation().getCurrentSimulationIndex()).setBackground(GraphicalSettings.simulationViewBackgroundCurrent);
	}

	@Override
	public void setFocus() {
		// empty
	}

	/**
	 * TODO MS - comments here
	 */
	public void fillTable() {
		// TODO
		Simulation simulation = SimulationsManager.getInstance().getSimulation();
		this.tableViewer.setInput(simulation);
		for (TableColumn tableColumn : table.getColumns())
			tableColumn.pack();
	}

	private void makeActions() {
		DeleteEventAction deleteEventAction = new DeleteEventAction(getSite().getWorkbenchWindow());
		MenuManager menuMgr = new MenuManager("#PopupMenu");
		menuMgr.add(deleteEventAction);
		menuMgr.add(new Separator(IWorkbenchActionConstants.MB_ADDITIONS));
		Menu menu = menuMgr.createContextMenu(this.tableViewer.getControl());
		this.tableViewer.getControl().setMenu(menu);
		getSite().registerContextMenu(menuMgr, this.tableViewer);
	}

	private void createCellEditors() {
		this.tableViewer.setColumnProperties(COLUMN_NAMES);
		CellEditor[] editors = new TextCellEditor[COLUMN_NAMES.length];
		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			CellEditor editor = null;
			if (i < 2) {
				// empty
			} else {
				editor = new TextCellEditor(table);
				((Text) editor.getControl()).setTextLimit(60);
			}
			editors[i] = editor;
		}
		tableViewer.setCellEditors(editors);
		tableViewer.setCellModifier(new SimulationCellModifier());
	}

	private class SimulationCellModifier implements ICellModifier {
		public boolean canModify(
		Object element, 
		String property) {
			if (COLUMN_NAMES[2].equals(property)) 
				return true;
			return false;
		}

		public Object getValue(Object o, String property) {
			Event baseEvent = (Event) o;
			if (COLUMN_NAMES[2].equals(property))
				return String.valueOf(baseEvent.getVirtualTime()/1000) + "." + String.valueOf((baseEvent.getVirtualTime()%1000/100));
			return null;
		}

		public void modify(Object element, String property, Object value) {
			Object obj = element;
			if (obj instanceof Item)
				obj = ((Item) obj).getData();
			Event baseEvent = (Event) obj;
			if(baseEvent.getVirtualTime() <= SimulationsManager.getInstance().getSimulation().getVirtualTime())
			{
				tableViewer.refresh();
				return;
			}
			if (COLUMN_NAMES[2].equals(property))
			{
				String result = "";
				if(((String)value).contains(","))
					result = ((String)value).replace(",", "").concat("00");
				else if(((String)value).contains("."))
					result = ((String)value).replace(".", "").concat("00");
				else
					result = ((String)value).concat("000");
				if(Long.parseLong(result) > SimulationsManager.getInstance().getSimulation().getVirtualTime())
				{
					baseEvent.setVirtualTime(Long.parseLong(result));
					int index = table.indexOf((TableItem)element);
					while(index > 0)
						if(((Event)table.getItem(index - 1).getData()).getVirtualTime() > Long.parseLong(result))
							Collections.swap(SimulationsManager.getInstance().getSimulation().getEventList(), index , --index);
						else
							break;
					while(index < SimulationsManager.getInstance().getSimulation().getEventList().size() - 1)
						if(((Event)table.getItem(index + 1).getData()).getVirtualTime() <= Long.parseLong(result))
							Collections.swap(SimulationsManager.getInstance().getSimulation().getEventList(), index , ++index);
						else
							break;
				}
				
			}
			tableViewer.refresh();
			/*for (int i = 0; i < COLUMN_NAMES.length; i++) {
				if (property.equals(COLUMN_NAMES[i])) {
					table.getColumn(i).pack();
					break;
				}
			}*/
		}
	}
	
	public void setTimeInLabel(SimulationView view, final long virtualTime)
	{
		view.setPartName("Simulation (" + virtualTime/60000 + ":" + ((virtualTime%60000)/1000 < 10 ? "0" + (virtualTime%60000)/1000 : (virtualTime%60000)/1000) + ":" + (virtualTime%1000)/100 + "00)");
	}

	private static SimulationView instance;
	
	public static SimulationView getInstance()
	{
		if(instance == null)
			instance = new SimulationView();
		return instance;
	}
}
