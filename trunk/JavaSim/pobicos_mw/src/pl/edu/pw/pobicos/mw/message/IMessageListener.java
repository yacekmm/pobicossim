package pl.edu.pw.pobicos.mw.message;

/**
 * Implementation of this listener allows to watch and respond if any message (report, command) just has been sent
 * somewhere in the ROVERS system.
 * 
 * @author Marcin Smialek
 * @created 2006-09-09 16:54:13
 */
public interface IMessageListener {

    /**
     * Method invoked if there just has been sent any message (report or command).
     * 
     * @param abstractMessage - report or command that just has been sent somewhere in the system
     */
    public void messageSent(AbstractMessage abstractMessage);
}
 