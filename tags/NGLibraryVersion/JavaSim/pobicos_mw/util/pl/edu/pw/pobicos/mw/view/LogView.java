package pl.edu.pw.pobicos.mw.view;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.mw.logging.LoggingManager;

public class LogView extends ViewPart {

	public static final String ID = "view.LogView";
	private StyledText log;
	
	private static LogView instance;
	
	public static LogView getInstance()
	{
		if(instance == null)
			instance = new LogView();
		return instance;
	}
	
	public LogView()
	{
		instance = this;
	}
	
	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());
		log = new StyledText(parent, SWT.BORDER | SWT.MULTI | SWT.V_SCROLL | SWT.H_SCROLL);
		log.setLayoutData(new GridData(GridData.FILL_BOTH | GridData.GRAB_HORIZONTAL | GridData.GRAB_VERTICAL));
		
		log.setEditable(false);
		LoggingManager.getInstance().init(parent.getDisplay(), log);
	}

	@Override
	public void setFocus() {
		getLog().setFocus();	
	}

	/**
	 * @param log the log to set
	 */
	public void setLog(StyledText log) {
		this.log = log;
	}

	/**
	 * @return the log
	 */
	public StyledText getLog() {
		return log;
	}

	public void clear() 
	{
		getLog().setText("");
	}
	
}
