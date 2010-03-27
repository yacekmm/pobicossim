package pl.edu.pw.pobicos.ng;

import org.eclipse.ui.IPageLayout;
import org.eclipse.ui.IPerspectiveFactory;

import pl.edu.pw.pobicos.ng.view.LogView;
import pl.edu.pw.pobicos.ng.view.NetworkView;

public class Perspective implements IPerspectiveFactory {

	public void createInitialLayout(IPageLayout layout) {
		String editorArea = layout.getEditorArea();
		layout.setEditorAreaVisible(false);
		layout.setFixed(true);

		layout.addStandaloneView(NetworkView.ID, false, IPageLayout.LEFT, 0.5f, editorArea);
		layout.addStandaloneView(LogView.ID, false, IPageLayout.RIGHT, 0.5f, editorArea);
		//layout.addView(LogView.ID, IPageLayout.RIGHT, 0.5f, editorArea);
		//layout.addStandaloneView(View.ID,  false, IPageLayout.LEFT, 1.0f, editorArea);
	}

}
