package pl.edu.pw.pobicos.ss;

import org.eclipse.ui.IPageLayout;
import org.eclipse.ui.IPerspectiveFactory;

import pl.edu.pw.pobicos.ss.view.ConsoleView;
import pl.edu.pw.pobicos.ss.view.LogView;

public class Perspective implements IPerspectiveFactory {

	public void createInitialLayout(IPageLayout layout) {
		String editorArea = layout.getEditorArea();
		layout.setEditorAreaVisible(false);
		layout.setFixed(true);

		layout.addStandaloneView(LogView.ID, false, IPageLayout.TOP, 0.5f, editorArea);
		layout.addStandaloneView(ConsoleView.ID, false, IPageLayout.BOTTOM, 0.5f, editorArea);
	}
}
