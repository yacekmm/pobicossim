package pl.edu.pw.pobicos.mw.simulation;

import pl.edu.pw.pobicos.mw.event.Event;


/**
 * Implementation of this listener allows to watch and respond to the fact that current simulation has been modified.
 *
 * @author Marcin Smialek
 * @created 2006-09-18 13:52:43
 */
public interface ISimulationListener {

    /**
     * This method is invoked every time when current simulation has been modified, especially when new event
     * has been added to or removed from the current simulation.
     * 
     * @param event - modified, added to simulation; if null it might be caused by removal of event from the
     *                current simulation
     */
    public void simulationChanged(Event event);

}

