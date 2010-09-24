package pl.edu.pw.pobicos.mw.agent;

import java.util.EventListener;

/**
 * Implementation of this listener allows to watch and respond to the fact that some micro-agent in the ROVERS
 * system just has been created, modified, or removed.
 * 
 * @author Marcin Smialek
 * @created 2006-09-05 14:15:15
 */
public interface IAgentListener extends EventListener {

    /**
     * Method invoked when any micro-agent in the system has been created, modified, or removed.
     * @param agent - created, modified micro-agent; if null it might be caused by removal of agent from the system
     */
    public void agentChanged(AbstractAgent agent);
}
