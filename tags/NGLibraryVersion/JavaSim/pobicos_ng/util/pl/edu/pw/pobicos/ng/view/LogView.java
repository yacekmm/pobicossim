package pl.edu.pw.pobicos.ng.view;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.events.ModifyEvent;
import org.eclipse.swt.events.ModifyListener;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.ng.logging.LoggingManager;

/**
 * View responsible for showing log in runtime.
 * @author Micha³ Krzysztof Szczerbak
 */
public class LogView extends ViewPart {

	/**
	 * Public view id.
	 */
	public static final String ID = "view.LogView";
	
	private StyledText log;
	
	private static LogView instance;

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static LogView getInstance()
	{
		if(instance == null)
			instance = new LogView();
		return instance;
	}
	
	/* (non-Javadoc)
	 * @see org.eclipse.ui.part.WorkbenchPart#createPartControl(org.eclipse.swt.widgets.Composite)
	 */
	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());
		log = new StyledText(parent, SWT.BORDER | SWT.MULTI | SWT.V_SCROLL | SWT.H_SCROLL);
		log.setLayoutData(new GridData(GridData.FILL_BOTH | GridData.GRAB_HORIZONTAL | GridData.GRAB_VERTICAL));
		log.addModifyListener(new ModifyListener(){
			public void modifyText(ModifyEvent e) {
				log.setTopIndex(log.getText().length());
			}
		});
		log.setEditable(false);
		LoggingManager.getInstance().init(parent.getDisplay(), log);
	}

	@Override
	public void setFocus() {
		log.setFocus();	
	}

	/**
	 * Clears the log text.
	 */
	public void clear() 
	{
		if (log!=null)
			log.setText("");
		return;
	}
	
}
