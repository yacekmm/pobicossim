package pl.edu.pw.pobicos.ss.network;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.io.Reader;
import java.net.Socket;

import pl.edu.pw.pobicos.ss.logging.Log;

/**
 * Manages network connection with one client.
 * @author Micha³ Krzysztof Szczerbak
 */
public class ServerService implements Runnable {
    private int id = -1;
    private Socket clientSocket;
    private BufferedReader input;
    private PrintWriter output;
    private boolean goon = false;

    /**
     * Constructor.
     * @param clientSocket socket for client's packets
     * @param id global client's id
     */
    public ServerService(Socket clientSocket, int id) {
        this.clientSocket = clientSocket;
        this.id = id;
    }

    /**
     * Initiates the service on the socket.
     * @throws IOException
     */
    public void init() throws IOException 
    {
    	goon = true;
        Reader reader = new InputStreamReader(clientSocket.getInputStream());
        input = new BufferedReader(reader);
        output = new PrintWriter(clientSocket.getOutputStream(), true);
    }

    /**
     * Finalizes the service.
     */
    public void close() 
    {
        try {
            output.close();
            input.close();
            clientSocket.close();
        } catch (IOException e) {
            System.err.println("B³¹d roz³¹czania.");
        } finally {
            output = null;
            input = null;
            clientSocket = null;
        }
    }

    /* (non-Javadoc)
     * @see java.lang.Runnable#run()
     */
    public void run() {
        while (goon) {
            String request = receive();
            while(Server.getInstance() == null);
			Server.getInstance().handleCommand(request, id);
			try {Thread.sleep(200);} 
			catch (InterruptedException e) {}
        }
    }
    
    /**
     * Stops thread for the service.
     */
    public void stop()
    {
    	goon = false;
    }

    /**
     * Sends message over the socket to the client.
     * @param command message to send
     */
    public void send(String command) {
    	try{
    		Log.info("SND(" + id + "): " + command + "\n");
    	}catch(Exception e){}
    	if(clientSocket!=null&&command!=null)
        {
    		output.println(command);
        }
    }

    private String receive() {
        try {
        	String msg=input.readLine();
    		Log.info("RCV: " + msg + "\n");
            return (msg==null ? Protocol.NULL : msg);
        } catch (Exception e) {
            System.err.println("ERROR reading from (" + id + ").");
            stop();
        }
        return Protocol.NULL;
    }
    
    /**
     * Gets global client's id.
     * @return id
     */
    protected int getId()
    {
    	return id;
    }
}
