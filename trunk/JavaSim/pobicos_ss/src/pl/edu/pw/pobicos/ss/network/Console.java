package pl.edu.pw.pobicos.ss.network;

import java.util.StringTokenizer;

import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.ss.view.ConsoleView;

/**
 * Manages the console and commands.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Console {

	private static Console instance;

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static Console getInstance()
	{
		if(instance == null)
			instance = new Console();
		return instance;
	}

	private Display display;
	private StyledText log;
	
	/**
	 * Gets SWT controls for the console.
	 * @param display SWT display context
	 * @param log text area container
	 */
	/**
	 * Gets SWT controls for the console. Initiating Server connection.
	 * @param display SWT display context
	 * @param log text area container
	 */
	public void init(Display display, StyledText log)
	{	
		this.display = display;
		this.log = log;
		log.append(this.StartServer());
		log.setCaretOffset(log.getText().length());
		log.setTopIndex(log.getText().length());
	}
	
	/**
	 * Starts connection on default port
	 * @return command result
	 * @author Jacek Milewski
	 */
	public String StartServer()
	{
		StringTokenizer st = new StringTokenizer("start");
		String tmp = "";
		
		if(!Server.getInstance().isRunning())
    	{
        	try
        	{
        		long port = Long.parseLong(st.nextToken());
            	Server.getInstance().start(port);
            	tmp += "\nServer started on port " + port + "\n" + input();
        	}catch(Exception e){
        		try{
            	Server.getInstance().start(40007);
            	tmp += "\nServer started on default port 40007\n" + input();
        		}catch(Exception ex){tmp += "\nFailed to start server\n" + input();}
        		}
        	}
    	else
    		tmp += "\nServer is already running\n" + input();
		
		return tmp;
	}
	
	/**
	 * Parses commands written in the console. Invokes methods connected to server, logical bidding and a view.
	 * @param command instruction
	 * @return true if parsing succeeded
	 */
	public String handleCommand(String command)
	{
		StringTokenizer st = new StringTokenizer(command);
        String cd = st.nextToken();
		String tmp = "";
        if (cd.equals(Commands.LIST)) {
        	try
        	{
            	String what = st.nextToken();
	        	if(what.equals("b") || what.equals("n") || what.equals("s"))
	        		tmp += list(what.charAt(0));
	        	else
	        	{
	        		tmp += list('n');
	        		tmp += list('s');
	        		tmp += list('b');
	        	}
        	}catch(Exception e)
        	{
        		tmp += list('n');
        		tmp += list('s');
        		tmp += list('b');
        	}
        	tmp += input();
        }
        else if (cd.equals(Commands.CLEAR)) {
    		display.asyncExec(new Runnable(){
    			public void run()
    			{
    				if (log.isDisposed())
    					return;
    				log.setText(input());
    			}
    		});
        }
        else if (cd.equals(Commands.STOP)) {
        	Server.getInstance().stop();
        	tmp += "\nServer stopped\n" + input();
        }
        else if (cd.equals(Commands.START)) {
        	if(!Server.getInstance().isRunning())
        	{
	        	try
	        	{
	        		long port = Long.parseLong(st.nextToken());
	            	Server.getInstance().start(port);
	            	tmp += "\nServer started on port " + port + "\n" + input();
	        	}catch(Exception e){
	        		try{
	            	Server.getInstance().start(40007);
	            	tmp += "\nServer started on default port 40007\n" + input();
	        		}catch(Exception ex){tmp += "\nFailed to start server\n" + input();}
	        		}
	        	}
        	else
        		tmp += "\nServer is already running\n" + input();
        }
        else if (cd.equals(Commands.BID)) {
        	try
        	{
        		int product = Integer.parseInt(st.nextToken());
        		long node = Long.parseLong(st.nextToken());
                tmp += "\n" + Server.getInstance().newBid(product, node) + "\n" + input();
        	}catch(Exception e){tmp += help() + input();}
        }
        else if (cd.equals(Commands.UNBID)) {
        	try
        	{
        		int product = Integer.parseInt(st.nextToken());
        		long node = Long.parseLong(st.nextToken());
                tmp += "\n" + Server.getInstance().oldBid(product, node) + "\n" + input();
        	}catch(Exception e){ConsoleView.getInstance().append(help() + input());}
        }
        else if (cd.equals(Commands.HELP))
        	tmp += help() + input();
        else if (cd.equals(Commands.FAST_BID))
        	tmp += Server.getInstance().fastBid();
        else
        	tmp += "\ninvalid command, try '" + Commands.HELP + "' for help\n" + input();
        return tmp;
	}
	
	private String list(char charAt) 
	{
		return Server.getInstance().list(charAt);
	}

	private String help()
	{
		return  "\n********************************************* USAGE *********************************************\n"
			+	"* ?                                                    - help                                   *\n"
			+	"* start <port=40007>                                   - start the server on that port           *\n"
			+	"* ls [b|n|s]                                           - list [limit to bids, nodes, simulators]*\n"
			+	"* bid <sim_id> <node_id>                               - make a logical connection              *\n"
			+	"* ubid <sim_id> <node_id>                              - disable the connection                 *\n"
			+	"* stop                                                 - stop the server                        *\n"
			+	"* clr                                                  - clear the screen                       *\n"
			+   "*************************************************************************************************\n";
	}
	
	private String input()
	{
		return "?> ";
	}
}
