package pl.edu.pw.pobicos.mw.view.action;

import org.eclipse.jface.action.Action;
import org.eclipse.jface.viewers.ISelection;
import org.eclipse.jface.viewers.IStructuredSelection;
import org.eclipse.ui.ISelectionListener;
import org.eclipse.ui.IWorkbenchPart;
import org.eclipse.ui.IWorkbenchWindow;
import org.eclipse.ui.actions.ActionFactory.IWorkbenchAction;

import pl.edu.pw.pobicos.mw.middleware.NodesManager;
import pl.edu.pw.pobicos.mw.node.GenericNode;

public class DeleteNodeAction extends Action implements ISelectionListener,
		IWorkbenchAction {

	private final IWorkbenchWindow window;
	
	private static final String ID = "action.deleteNode";

	private IStructuredSelection selection;
	
	public DeleteNodeAction(IWorkbenchWindow window) {
		this.window = window;
		setId(ID);
		setText("Delete Node");
		setEnabled(false);
		window.getSelectionService().addSelectionListener(this);
	}

	@Override
	public void run() {
		if (selection == null)
			return;
		Object item = selection.getFirstElement(); 
		if (!(item instanceof GenericNode))
			return;
		GenericNode node = (GenericNode) item;
		NodesManager.getInstance().removeNode(node);
	}

	public void selectionChanged(IWorkbenchPart part, ISelection incoming) {
		if (incoming instanceof IStructuredSelection) {
			selection = (IStructuredSelection) incoming;
			setEnabled(selection.size() == 1
					&& selection.getFirstElement() instanceof GenericNode);
		} else
			setEnabled(false);
	}

	public void dispose() {
		window.getSelectionService().removeSelectionListener(this);
	}
}
