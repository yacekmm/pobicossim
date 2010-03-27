package pl.edu.pw.pobicos.ng.view;

import java.util.Vector;

import org.eclipse.swt.SWT;
import org.eclipse.swt.custom.CLabel;
import org.eclipse.swt.custom.ScrolledComposite;
import org.eclipse.swt.custom.StyledText;
import org.eclipse.swt.events.ModifyEvent;
import org.eclipse.swt.events.ModifyListener;
import org.eclipse.swt.events.PaintEvent;
import org.eclipse.swt.events.PaintListener;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.layout.FillLayout;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Combo;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Control;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Event;
import org.eclipse.swt.widgets.Listener;
import org.eclipse.ui.part.ViewPart;

import pl.edu.pw.pobicos.ng.event.PhysicalEvent;
import pl.edu.pw.pobicos.ng.instruction.InstructionMap;
import pl.edu.pw.pobicos.ng.network.Client;
import pl.edu.pw.pobicos.ng.product.ProductManager;
import pl.edu.pw.pobicos.ng.product.ProductMap;
import pl.edu.pw.pobicos.ng.product.SensorValue;
import pl.edu.pw.pobicos.ng.resource.AbstractResource;

/**
 * View showing the network.
 * @author Micha³ Krzysztof Szczerbak
 */
public class NetworkView extends ViewPart {
	//TODO: make the network view graphical

	/**
	 * Public view id.
	 */
	public static final String ID = "view.NetworkView";
	
	private static NetworkView instance;

	/**
	 * Gets an instance of this singleton class.
	 * @return instance
	 */
	public static NetworkView getInstance()
	{
		if(instance == null)
			instance = new NetworkView();
		return instance;
	}
	
	private static Vector<Integer> lines = new Vector<Integer>();
	
	/* (non-Javadoc)
	 * @see org.eclipse.ui.part.WorkbenchPart#createPartControl(org.eclipse.swt.widgets.Composite)
	 */
	@Override
	public void createPartControl(Composite parent) {
		parent.setLayout(new FillLayout());
		final ScrolledComposite scrolled = new ScrolledComposite(parent, SWT.BORDER | SWT.H_SCROLL | SWT.V_SCROLL);
		scrolled.setExpandHorizontal(false);
		scrolled.setExpandVertical(false);
		final Composite panel = new Composite(scrolled, SWT.NONE);
		scrolled.setContent(panel);
		panel.setLayout(new GridLayout(3, false));
		scrolled.setSize(panel.computeSize(SWT.DEFAULT, SWT.DEFAULT));
		panel.addPaintListener(new PaintListener(){
			public void paintControl(PaintEvent e) {
				boolean par = false;
				int offset = 0;
				for(Integer integer : lines)
				{
					if(par)
					{
						Rectangle area = panel.getParent().getClientArea();
						area.y = offset;
						area.height = integer.intValue() * 27;
						e.gc.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));
						e.gc.fillRectangle(area);
					}
					offset += integer.intValue() * 27;
					par = !par;
				}
			}
		});
		NetworkViewManager.getInstance().init(parent.getDisplay(), panel);
	}

	@Override
	public void setFocus() {
		// empty		
	}
	
	/**
	 * Fills the view with products.
	 * @param container container
	 */
	public void paint(Composite container)
	{		
		clear(container);
		lines.clear();
		boolean par = false;
		for(ProductMap productMap : ProductManager.getInstance().getProducts())
		{	
			int line = 0;
			final CLabel label = new CLabel(container, SWT.NONE);
			label.setText("Product #" + productMap.getId() + " '" + productMap.getName() + "'");
			if(par)
				label.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));

			final Combo list = new Combo(container, SWT.BORDER | SWT.SIMPLE | SWT.DROP_DOWN | SWT.READ_ONLY);
			
			int count = 0;
			for(AbstractResource resource : productMap.getProduct().getResources())
				if(resource.getClass() != pl.edu.pw.pobicos.ng.resource.GenericResource.class)
					for(PhysicalEvent pevent : resource.physicalEventsRaisen())
					{
						count++;
						list.add(pevent.getName());
						list.setData(pevent);
					}
			list.select(0);

			Button button = new Button(container, SWT.NONE);
			button.setText("Happen!");
			final ProductMap temp = productMap;
			button.addListener(SWT.Selection, new Listener(){
			      public void handleEvent(Event event) {
				        Client.getInstance().notifyEvent(temp.getId(), list.getItem(list.getSelectionIndex()), "");
				  }
			});
			
			if(count == 0)
			{
				list.setVisible(false);
				button.setVisible(false);
			}
			line++;

			for(SensorValue sens : productMap.getSensors())
			{
				CLabel none = new CLabel(container, SWT.NONE);
				none.setText("");
				if(par)
					none.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));

				CLabel lab = new CLabel(container, SWT.NONE);
				lab.setText(sens.getInstruction() + ": ");
				if(par)
					lab.setBackground(Display.getDefault().getSystemColor(SWT.COLOR_GRAY));

				StyledText text = new StyledText(container, SWT.BORDER);
				if(sens.getType().equals(InstructionMap.UINT8))
					text.setText(String.valueOf((Short)sens.getValue()));
				else if(sens.getType().equals(InstructionMap.UINT16))
					text.setText(String.valueOf((Integer)sens.getValue()));
				else if(sens.getType().equals(InstructionMap.UINT32))
					text.setText(String.valueOf((Long)sens.getValue()));
				final SensorValue tempSens = sens;
				text.addModifyListener(new ModifyListener(){
					public void modifyText(ModifyEvent e) {
						StyledText text = (StyledText)e.widget;
						try
						{
							if(tempSens.getType().equals(InstructionMap.UINT8))
								tempSens.setValue(Short.parseShort(text.getText()));
							else if(tempSens.getType().equals(InstructionMap.UINT16))
								tempSens.setValue(Integer.parseInt(text.getText()));
							else if(tempSens.getType().equals(InstructionMap.UINT32))
								tempSens.setValue(Long.parseLong(text.getText()));
						}catch(Exception ex)
						{

							if(tempSens.getType().equals(InstructionMap.UINT8))
								text.setText(String.valueOf((Short)tempSens.getValue()));
							else if(tempSens.getType().equals(InstructionMap.UINT16))
								text.setText(String.valueOf((Integer)tempSens.getValue()));
							else if(tempSens.getType().equals(InstructionMap.UINT32))
								text.setText(String.valueOf((Long)tempSens.getValue()));
						}
					}
				});
				line++;
			}
			lines.add(line);
			par = !par;
		}
		
		container.pack();
	}

	/**
	 * Clears the view from products.
	 * @param cont container
	 */
	public void clear(Composite cont) 
	{
		try{
			for(Control c : cont.getChildren())
				{
					c.setVisible(false);
					c.dispose();
				}
		}catch(Exception e){}
		cont.pack();
	}
}
