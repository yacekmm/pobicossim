package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.jface.viewers.IStructuredSelection;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.event.Event;
import pl.edu.pw.pobicos.mw.middleware.SimulationsManager;
import pl.edu.pw.pobicos.mw.simulation.Simulation;

public class DeleteEventAction extends Action implements ISelectionListener, IWorkbenchAction {

    private static final String ID = "action.deleteEvent";
    
    private IWorkbenchWindow window;
    
    private IStructuredSelection selection;
    
    public DeleteEventAction(IWorkbenchWindow window) 
    {
		this.window = window;
		setId(ID);
		ActionContainer.add(ID, this);
		setText("Remove Event");
		setEnabled(false);
		window.getSelectionService().addSelectionListener(this);
    }
    
    
    @Override
    public void run() 
    {
		if (selection == null)
		    return;
		Object item = selection.getFirstElement();
		if (!(item instanceof Event))
		    return;
		Event event = (Event) item;
		if(event.isGeneric())
			return;
		Simulation simulation = SimulationsManager.getInstance().getSimulation();
		if (simulation != null)
			if(SimulationsManager.getInstance().getSimulation().getVirtualTime() > event.getVirtualTime())
				simulation.removeEvent(event);
    }

    public void selectionChanged(IWorkbenchPart part, ISelection selection) 
    {
	    setEnabled(false);
		if (selection instanceof IStructuredSelection) 
		{
		    this.selection = (IStructuredSelection) selection;
		    if(this.selection.size() == 1 && this.selection.getFirstElement() instanceof Event)
		    	if(!((Event)this.selection.getFirstElement()).isGeneric())
		    		setEnabled(true);
		}
    }

    public void dispose() 
    {
    	window.getSelectionService().removeSelectionListener(this);
    }

}
