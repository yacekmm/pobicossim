package pl.edu.pw.pobicos.mw.node;

/**
 * Implementation of this listener allows to watch and respond to the fact that some node in the ROVERS system just
 * has been created, modified, or removed.
 * 
 * 
 * @author Marcin Smialek
 * @created 2006-09-08 12:49:59
 */
public interface INodeListener {

    /**
     * Method invoked when any node in the system has been created, modified, or removed.
     * @param node - created, modified node; if null it might be caused by removal of node from the system
     */
    public void nodeChanged(AbstractNode node);
}
