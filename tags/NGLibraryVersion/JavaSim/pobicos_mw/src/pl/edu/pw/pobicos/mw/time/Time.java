package pl.edu.pw.pobicos.mw.time;

import pl.edu.pw.pobicos.mw.simulation.Simulation;

/**
 * Implements virtual clock.
 * @author Micha³ Krzysztof Szczerbak
 */
public class Time implements Runnable{

	private boolean isTicking;
	private int interval_ds = 1;
	
	/**
	 * Constructor.
	 * @param ds one virtual tick interval in 1/10s
	 */
	public Time(int ds)
	{
		interval_ds = ds;
	}
	
	/**
	 * Starts.
	 */
	public void start()
	{
		isTicking = true;
		(new Thread(this)).start();
	}
	
	/**
	 * Stops.
	 */
	public void stop()
	{
		isTicking = false;
	}
	
	/* 
	 * Fires an event each interval has passed.
	 * @see java.lang.Runnable#run()
	 */
	public void run()
	{
		int id = 0;
		while(isTicking)
		{
			try{
				fireTimeTick(new TimeEvent(this, id++));
				synchronized(this){
				this.wait(Simulation.getVirtualTimeStep() * interval_ds);}
			}catch(Exception ex){ System.out.println(ex.toString());}
		}
	}

    protected javax.swing.event.EventListenerList listenerList = new javax.swing.event.EventListenerList();

    /**
     * This methods allows classes to register for TimeEvent
     * @param listener
     */
    public void addMyEventListener(TimeListener listener) {
        listenerList.add(TimeListener.class, listener);
    }

    /**
     * This methods allows classes to unregister for TimeEvent
     * @param listener
     */
    public void removeMyEventListener(TimeListener listener) {
        listenerList.remove(TimeListener.class, listener);
    }

    private void fireTimeTick(TimeEvent evt) {
        Object[] listeners = listenerList.getListenerList();
        // Each listener occupies two elements - the first is the listener class
        // and the second is the listener instance
        for (int i=0; i<listeners.length; i+=2) {
            if (listeners[i]==TimeListener.class) {
                ((TimeListener)listeners[i+1]).timeTick(evt);
            }
        }
    }

	/**
	 * Checks if clock is running.
	 * @return true if is running, false otherwise
	 */
	public boolean isTicking() 
	{
		return isTicking;
	}
}
