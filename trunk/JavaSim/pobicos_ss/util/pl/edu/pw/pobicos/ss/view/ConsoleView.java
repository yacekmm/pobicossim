package pl.edu.pw.pobicos.ss.view;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.events.KeyEvent;
import org.eclipse.swt.events.KeyListener;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Display;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.ss.network.Console;

/**
 * View responsible for showing a command console.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ConsoleView extends ViewPart {
	/**
	 * Public view id.
	 */
	public static final String ID = "view.ConsoleView";
	private StyledText log;
	private String command = "";
	
	private static ConsoleView instance;

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static ConsoleView getInstance()
	{
		if(instance == null)
			instance = new ConsoleView();
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
		log.setFont(Display.getCurrent().getSystemFont());
		
		getLog().setText("?> ");
		log.setCaretOffset(log.getText().length());
		getLog().setEditable(false);
		getLog().setForeground(Display.getCurrent().getSystemColor(SWT.COLOR_WHITE));
		getLog().setBackground(Display.getCurrent().getSystemColor(SWT.COLOR_BLACK));
		getLog().setFocus();
		Console.getInstance().init(parent.getDisplay(), log);
		log.addKeyListener(new KeyListener(){

			public void keyPressed(KeyEvent e) {
				if((e.keyCode >= 97 && e.keyCode <= 122) || (e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 32 || e.keyCode == 47)
				{
					command += String.valueOf(e.character);
					log.append(String.valueOf(e.character));
				}
				else if(e.keyCode == 8)
				{
					try
					{
						command = command.substring(0, command.length() - 1);
					}catch(Exception ex){}
					log.setText(log.getText().substring(0, log.getText().length() - 1));
				}
				else if(e.keyCode == 13)
				{
					log.append(Console.getInstance().handleCommand(command));
					command = "";
				}
				log.setCaretOffset(log.getText().length());
				log.setTopIndex(log.getText().length());
			}

			public void keyReleased(KeyEvent e) {
			}
			
		});
	}

	@Override
	public void setFocus() {
		// empty		
	}

	/**
	 * Appends string of chars to the console.
	 * @param message string of chars
	 */
	public void append(String message) {
		getLog().setText(getLog().getText() + message);
	}

	private StyledText getLog() {
		return log;
	}

	/**
	 * Clears the console.
	 */
	public void clear() 
	{
		getLog().setText("");
	}
}
