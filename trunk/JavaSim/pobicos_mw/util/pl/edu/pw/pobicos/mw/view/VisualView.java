package pl.edu.pw.pobicos.mw.view;

import org.eclipse.jface.viewers.TableViewer;
import org.eclipse.swt.SWT;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Table;
import org.eclipse.swt.widgets.TableColumn;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.GraphicalSettings;
import pl.edu.pw.pobicos.mw.logging.ITraceListener;
import pl.edu.pw.pobicos.mw.logging.Trace;
import pl.edu.pw.pobicos.mw.view.provider.TraceTableContentProvider;
import pl.edu.pw.pobicos.mw.view.provider.TraceTableLabelProvider;

/**
 * This class represents sensor network view i.e. graphical representation of
 * nodes, agents etc.
 * 
 * @author Marcin Smialek
 * @created 2006-09-04 19:58:16
 */
public class VisualView extends ViewPart {
	//private static final Log LOG = LogFactory.getLog(NetworkView.class);

	/**
	 * Unique view id
	 */
	public static final String ID = "view.VisualView";

	private static final String[] COLUMN_NAMES = { "message", "node", "agent", "time", "data" };

	private TableViewer tableViewer;

	private Table table;

	/**
	 * Constructor
	 */
	public VisualView() {
		// empty
	}

	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());
		this.table = new Table(parent, SWT.SINGLE | SWT.BORDER | SWT.H_SCROLL | SWT.V_SCROLL | SWT.FULL_SELECTION
				| SWT.HIDE_SELECTION);
		this.table.setLinesVisible(true);
		this.table.setHeaderVisible(true);
		this.table.setBackground(GraphicalSettings.traceViewBackground);
		for (int i = 0; i < COLUMN_NAMES.length; i++) {
			TableColumn column = new TableColumn(table, SWT.LEFT, i);
			column.setText(COLUMN_NAMES[i]);
			column.setWidth(100);
			if(i == 4)
				column.setWidth(150);
		}

		tableViewer = new TableViewer(table);
		tableViewer.setUseHashlookup(true);
		tableViewer.setLabelProvider(new TraceTableLabelProvider());
		tableViewer.setContentProvider(new TraceTableContentProvider());
		tableViewer.setInput(Trace.class);
		Trace.addTraceListener(new ITraceListener() {

			public void traceChanged() {
				tableViewer.refresh();
			}

		});
	}

	@Override
	public void setFocus() {
		// empty
	}

	

}