package pl.edu.pw.pobicos.mw.middleware;

import java.io.FileInputStream;
import java.util.List;

import org.eclipse.core.runtime.ListenerList;
import org.eclipse.swt.widgets.Display;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.port.EventElement;
import pl.edu.pw.pobicos.mw.port.SimulationElement;
import pl.edu.pw.pobicos.mw.port.Import;
import pl.edu.pw.pobicos.mw.port.Export;
import pl.edu.pw.pobicos.mw.simulation.ISimulationListener;
import pl.edu.pw.pobicos.mw.simulation.Simulation;
import pl.edu.pw.pobicos.mw.view.SimulationView;

/**
 * Singleton class responsible for management of simulations in the ROVERS
 * system.
 * 
 * @author Tomasz Anuszewski
 * @created 2006-09-04 22:02:38
 */
public class SimulationsManager {

	private static SimulationsManager instance;

	private static boolean isConnected = false;

	// simualation status
	private boolean stopped = true;

	private boolean running = false;

	private boolean paused = false;

	/**
	 * time between two subsequent simulation steps
	 */
	private int stepDuration = 700;
	
	/*
	 * Time between two subsequent simulation steps. Every simulation step may
	 * be thought of either as firing of event or message sending.
	 */
	//TODO: make it alive
	private int messageDuration = 700;
	
	private Simulation simulation = new Simulation("");

	// list of listeners added to objects listening to modifications of this
	// simulation
	private ListenerList listeners = new ListenerList();

	private SimulationsManager() {
		// empty
	}

	/**
	 * Returns singleton instance of this class.
	 * 
	 * @return SimulationsManager singleton instance
	 */
	public static SimulationsManager getInstance() {
		if (instance == null) {
			instance = new SimulationsManager();
		}
		return instance;
	}

	/**
	 * Initializes SimulationsManager with data from the XML simualtion file
	 * defined by the argument.
	 * 
	 * @param is -
	 *            input stream pointing to simulation file
	 */
	public void loadSimulation(FileInputStream fis) {
		if (fis != null) {
			try {
				SimulationElement sim = Import.importSimulation(fis);
				SimulationsManager.getInstance().clear();
				SimulationsManager.getInstance().setSimulation(sim);
			} catch (Exception e) {
				//LOG.error("Cannot initialize Rovers simulation. See log file for more details", e);
			}
		} else{
			//LOG.warn("'InputStream' to the simulation config not found.");
		}
	}

	/**
	 * Saves the current simulation state in the stream defined by the argument.
	 * 
	 * @param simulationName - user friendly description of this instance of simulation
	 * @param os -
	 *            defines where to store the simulation data
	 * @return status of the save operation
	 */
	public boolean saveSimulation(String simulationName, String path) {
		//LOG.debug("Saving simulation");
		
		// save options

		SimulationElement sim = new SimulationElement(simulationName, PobicosManager.getInstance().getNetworkId());
		
		List<Event> events = getSimulation().getEventList();
		for (Event event : events)
			if(!event.isGeneric() && event.getNode() != null)
			{
				//LOG.debug("Saving event: " + event);
				sim.addEvent(new EventElement(event.getCode(), (int)event.getNode().getId(), event.getVirtualTime()));
			}
		
		// save
		try {
			Export.exportSimulation(sim, path);
			//LOG.debug("Rovers config saved ...");
			return true;
		} catch (Exception e) {
			//LOG.error("Cannot save Rovers simulation.", e);
			//LOG.debug("Cannot save ROVERS config.");
			return false;
		}
	}

	/**
	 * Clears SimulationsManager from current simulation data i.e. list of
	 * simulations, current simulation. It ensures that system is free of being
	 * polluted by uncleared remainings of previous simulation data.
	 */
	public void clear() {
		this.simulation.clear();
	}

	public void clearToCome() 
	{
		this.simulation.clearToCome();
	}
	
	public void reset() 
	{
		this.simulation.reset();
	}

	/**
	 * Starts the currently specified simulation.
	 */
	public void startSimulation() {
		setStarted();
		this.simulation.start();
	}
	
	/**
	 * Stops current sim
	 */
	public void stopSimulation() {
		setStopped();
		this.simulation.stop();
	}

	/**
	 * Causes that the next event in the simulation event queue is fired.
	 */
	public void stepForward() {
		this.simulation.stepForward();
	}
	
	Display simulationDisplay;
	SimulationView simulationView;

	public void setView(SimulationView simulationView, Display display) {
		this.simulationDisplay = display;
		this.simulationView = simulationView;
	}
	
	public void setTimeInLabel(final long virtualTime)
	{
		if(simulationDisplay == null)
			return;
		simulationDisplay.asyncExec(new Runnable(){
			public void run() {
	        	SimulationView.getInstance().setTimeInLabel(simulationView, virtualTime);
			}	        		
    	});
	}
	
	public Simulation getSimulation()
	{
		return simulation;
	}
	
	private void setSimulation(SimulationElement simulation)
	{
		this.simulation = new Simulation(simulation);
	}

	/**
	 * Returns message delivery time value.
	 * 
	 * @return message delivery time value
	 */
	public int getMessageDuration() {
		return this.messageDuration;
	}

	/**
	 * Sets message delivery time value
	 * 
	 * @param messageDuration -
	 *            message delivery duration
	 */
	public void setMessageDuration(int messageDuration) {
		this.messageDuration = messageDuration;
	}

	/**
	 * Returns current value of time gap between two subsequent events.
	 * 
	 * @return current value of time gap between two subsequent events
	 */
	public int getStepDuration() {
		return stepDuration;
	}

	/**
	 * Sets the value for time gap between two subsequent events
	 * 
	 * @param stepDuration -
	 *            time gap value between two subsequent events
	 */
	public void setStepDuration(int stepDuration) {
		this.stepDuration = stepDuration;
	}

	/**
	 * @return TRUE when simulations is paused
	 */
	public boolean isPaused() {
		return this.paused;
	}

	/**
	 * This method indicates simulation manager that simulation has to be
	 * paused.
	 */
	public void setPaused() {
		this.running = false;
		this.paused = true;
		this.stopped = false;
	}

	/**
	 * @return TRUE when simulation is runnig
	 */
	public boolean isRunning() {
		return this.running;
	}

	/**
	 * Starting current simulation
	 */
	public void setStarted() {
		this.running = true;
		this.paused = false;
		this.stopped = false;
	}

	/**
	 * @return TRUE when simulation is stopped
	 */
	public boolean isStopped() {
		return this.stopped;
	}

	/**
	 * Stopping current simulation
	 */
	public void setStopped() {
		this.running = false;
		this.paused = false;
		this.stopped = true;
	}

	public static boolean isConnected() {
		return isConnected ;
	}

	public static void setConnected(boolean b) {
		isConnected = b;
	}

	// --------------------------------------------------------------------------------------------
	/**
	 * Adds listener to the listeners list. Each listeners is installed on
	 * object that listents if this simulation just has been modified.
	 * 
	 * @param listener -
	 *            listenter that is to be added to the listeners list
	 */
	public void addSimulationListener(ISimulationListener listener) {
		this.listeners.add(listener);
	}

	/**
	 * Removes listener from listners list. Each listener, that is supposed to
	 * be removed, has been installed on object listening if this simulation
	 * just has been modified.
	 * 
	 * @param listener -
	 *            listener to be removed from listners list
	 */
	public void removeSimulationListener(ISimulationListener listener) {
		this.listeners.remove(listener);
	}

	/**
	 * Indicates all simulation listeners about the changes in the current
	 * simulation configuration.
	 * 
	 * @param event
	 */
	public void fireSimulationChanged(final Event event) {
		final ListenerList tmpListeners = listeners;
    	Display.getDefault().asyncExec(new Runnable() {
			public void run() {
		    	try {
		    		Object[] ls = tmpListeners.getListeners();
		    		for (int i = 0; i < ls.length; i++) {
		    			ISimulationListener listener = (ISimulationListener) ls[i];
		    			listener.simulationChanged(event);
		    		}
				} catch (Exception e) {
				}
			}
		});
	}
}
