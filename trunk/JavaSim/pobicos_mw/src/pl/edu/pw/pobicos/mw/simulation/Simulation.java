package pl.edu.pw.pobicos.mw.simulation;

import java.util.ArrayList;
import java.util.List;
import java.util.Vector;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.middleware.PobicosManager;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.port.EventElement;
import pl.edu.pw.pobicos.mw.port.SimulationElement;
import pl.edu.pw.pobicos.mw.time.Time;
import pl.edu.pw.pobicos.mw.time.TimeEvent;
import pl.edu.pw.pobicos.mw.time.TimeListener;

/**
 * This class represents single instance of simulation in the ROVERS system. Each simulation reflects event-driven
 * programming model applied to the architecture of every micro-agent and is defined by list of events that may be
 * fired in the ROVERS network. The user may create many instances of this class, but only one is chosen as the
 * current one.
 * 
 * @author Tomasz Anuszewski
 */
public class Simulation {
	
	private long virtualTime = 0;
	
	private Time virtualClock;
	
	private static int virtualTimeStep = 100;
	
	/**
	 * name of this simulation
	 */
	private String name;

	/**
	 * reference to the most recently performed event 
	 */
	private Event currentSimulationEvent;

	/**
	 * list of events constituting this simulation
	 */
	private List<Event> eventList = new ArrayList<Event>();
	
	private Simulation()
	{
		virtualClock = new Time(1);
		virtualClock.addMyEventListener(new TimeListener() {
	        public void timeTick(TimeEvent evt) {
	        	virtualTime += virtualTimeStep;
	        	setTimeInLabel();
	        	checkStepsToRun();
	        	PobicosManager.getInstance().removeOldMessages();
	        }
	    });
	}

	/**
	 * Creates a new instance of simulation.
	 * 
	 * @param name - name for new instance of RoversSimulation  
	 */
	public Simulation(String name) 
	{
		this();
		this.name = name;
	}

	/**
	 * Loads the simulation from the XML simulation file.
	 * 
	 * @param simulation - simulation to be loaded
	 */
	public Simulation(SimulationElement simulation) 
	{
		this();
		this.name = simulation.getName();
		for (EventElement event : simulation.getEvents()) 
		{
			eventList.add(new Event(event.getCode(), NodesManager.getInstance().getNode(event.getNodeId()), event.getVirtualTime()));
		}
	}

	/**
	 * Starts this simulation.
	 */
	public void start() 
	{
		virtualClock.start();
	}

	/**
	 * Causes firing of the next event from the events' queue.
	 */
	public void stepForward() 
	{
		if (eventList.size() < 1)
			return;

		int currentPosition = 0;
		if (currentSimulationEvent == null) 
		{
			currentSimulationEvent = eventList.get(currentPosition);
			if(currentSimulationEvent != null)
			{
				if(!virtualClock.isTicking())
					virtualTime = currentSimulationEvent.getVirtualTime();
				if(currentSimulationEvent.getVirtualTime()  <= virtualTime && currentSimulationEvent.getVirtualTime() > virtualTime - virtualTimeStep)
				{
					PobicosManager.getInstance().handleEvent(currentSimulationEvent);
					fireSimulationChanged(currentSimulationEvent);
					setTimeInLabel();
				}
				else
					currentSimulationEvent = null;
			}
			return;
		}
		currentPosition = eventList.indexOf(currentSimulationEvent);
		if (currentPosition >= eventList.size() - 1) {
			currentPosition = eventList.size() - 1;
			fireSimulationChanged(currentSimulationEvent);
		}
		else {
			currentPosition++;
			currentSimulationEvent = eventList.get(currentPosition);
			if(!virtualClock.isTicking())
				virtualTime = currentSimulationEvent.getVirtualTime();
			if(currentSimulationEvent.getVirtualTime()  <= virtualTime && currentSimulationEvent.getVirtualTime() > virtualTime - virtualTimeStep)
			{
				PobicosManager.getInstance().handleEvent(currentSimulationEvent);
				fireSimulationChanged(currentSimulationEvent);
			}
		}
		setTimeInLabel();
	}
	
	private void checkStepsToRun()
	{
		if(currentSimulationEvent == null)
			stepForward();
		while(getNextStep() != null)
			if(getNextStep().getVirtualTime()  <= virtualTime && getNextStep().getVirtualTime() > virtualTime - virtualTimeStep)
				stepForward();
			else
				break;
	}
	
	private Event getNextStep()
	{
		if(currentSimulationEvent == null)
			return null;
		return (eventList.size() > eventList.indexOf(currentSimulationEvent) + 1 ? eventList.get(eventList.indexOf(currentSimulationEvent) + 1) : null);
	}

	/**
	 * Causes that the current event is fired again.
	 */
	public void fireCurrentEvent() 
	{
		if (eventList.size() < 1) 
			return;
		if (currentSimulationEvent == null) 
			currentSimulationEvent = eventList.get(0);
		PobicosManager.getInstance().handleEvent(currentSimulationEvent);
		fireSimulationChanged(currentSimulationEvent);
	}
	
	/**
	 * Permanently stops this simulation.
	 */
	public void stop() 
	{
		virtualClock.stop();
		/*for(AbstractNode node : NodesManager.getInstance().getNodes())
			node.getVm().stop();*/
		// TODO save simulation results
	}

	/**
	 * Adds event to this simulation.
	 * @param event - event to be added to this simulation
	 */
	public void addSimulationEvent(Event event) 
	{
		int index = 0;
		for(Event ev : eventList)
			if(ev.getVirtualTime() <= event.getVirtualTime())
			{
				index++;
			}
			else
				break;
		eventList.add(index, event);
		fireSimulationChanged(event);
	}

	/**
	 * Removes event from this simlation.
	 * 
	 * @param event
	 */
	public void removeEvent(Event event) 
	{
		eventList.remove(event);
		fireSimulationChanged(event);
	}

	/**
	 * Clears this simulation from all events and sets properties of this simulation to default values.
	 */
	public void clear() 
	{
		stop();
		virtualTime = 0;
		SimulationsManager.getInstance().setTimeInLabel(virtualTime);
		eventList.clear();
		currentSimulationEvent = null;
		fireSimulationChanged(null);
	}

	public void clearToCome() 
	{
		Vector<Event> toDelete = new Vector<Event>();
		stop();
		for(Event event : eventList)
			if(event.getVirtualTime() >= virtualTime)
				toDelete.add(event);
		for(Event event : toDelete)
			eventList.remove(event);
		currentSimulationEvent = null;
		fireSimulationChanged(null);
	}
	
	public void reset() 
	{
		eventList.clear();
		fireSimulationChanged(null);
	}

	protected void setTimeInLabel() 
	{
		SimulationsManager.getInstance().setTimeInLabel(virtualTime);
	}

	/**
	 * Returns index of the event that has been lately fired.
	 * 
	 * @return index of the has event that been lately fired
	 */
	public int getCurrentSimulationIndex() 
	{
		if (this.currentSimulationEvent == null)
			return -1;
		return this.eventList.indexOf(this.currentSimulationEvent);
	}

	/**
	 * Sets index of the event in the current simulation that needs to be performed
	 * @param index - value of the index
	 */
	public void setCurrentSimulationIndex(int index) 
	{
		if (index < 0 || index >= this.eventList.size()) {
			this.currentSimulationEvent = null;
		} else {
			this.currentSimulationEvent = this.eventList.get(index);
		}
		fireSimulationChanged(this.currentSimulationEvent);
	}

	/**
	 * Returns true if lately fired event is the first one in the simulation queue, false otherwise,
	 * 
	 * @return true if lately fired event is the first one in the simulation queue, false otherwise
	 */
	public boolean isFirstEvent() 
	{
		return (currentSimulationEvent == null || eventList.indexOf(currentSimulationEvent) == 0) ? true : false;
	}

	/**
	 * Returns true if lately fired event is the last one in the simulation queue, false otherwise,
	 * 
	 * @return true if lately fired event is the last one in the simulation queue, false otherwise
	 */
	public boolean isLastEvent() 
	{
		if (currentSimulationEvent != null && !(eventList.indexOf(currentSimulationEvent) < eventList.size() - 1)) {
			return true;
		}
		return false;
	}

	/**
	 * Returns list of events constituting this simulation.
	 * 
	 * @return list of the events that this simulation constists of
	 */
	public List<Event> getEventList() 
	{
		return eventList;
	}

	/**
	 * Returns name of this simulation.
	 * 
	 * @return simulation name
	 */
	public String getName() 
	{
		return this.name;
	}

	/**
	 * Sets name for this simulation.
	 * 
	 * @param name  - for this simulation to be set
	 */
	public void setName(String name) {
		this.name = name;
	}

	public long getVirtualTime() 
	{
		return virtualTime;
	}
	
	public static int getVirtualTimeStep()
	{
		return virtualTimeStep;
	}
  
	private void fireSimulationChanged(Event event) 
	{
		SimulationsManager.getInstance().fireSimulationChanged(event);
	}
}
