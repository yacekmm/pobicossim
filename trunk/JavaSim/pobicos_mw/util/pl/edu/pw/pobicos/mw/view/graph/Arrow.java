package pl.edu.pw.pobicos.mw.view.graph;

import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.graphics.Rectangle;

/**
 * This class is used for calculation of arrow coordinates.
 * 
 * @author Marcin Smialek
 * @created 2006-09-12 19:21:01
 */
public class Arrow {

	// private static final Log LOG = LogFactory.getLog(Arrow.class);

	private static final double d1 = 12;

	private static final double d2 = 4;

	private Point A = new Point(0, 0);

	private Point B = new Point(0, 0);

	private Point C = new Point(0, 0);

	private static Arrow instance;

	private Arrow() {
		// empty
	}

	/**
	 * @return Arrow instance
	 */
	public static Arrow getInstance() {
		if (instance == null) {
			instance = new Arrow();
		}
		return instance;
	}

	private void calculatePointsABC(Point p1, Point p2) {
		double l = d1 / 2;
		double x1, y1, x2, y2, aX, aY, bX, bY, cX, cY;
		aX = aY = bX = bY = cX = cY = 0.0;
		x1 = p1.x;
		y1 = p1.y;
		x2 = p2.x;
		y2 = p2.y;

		if (p1.x == p2.x) {
			int aux = 1;
			aY = (y1 + y2) / 2 + l;
			if (Math.abs(aY - y1) < Math.abs(y2 - aY))
				aux = -1;
			aX = x1;
			aY = (y1 + y2) / 2 + l * aux;
			bX = x1 - d2;
			bY = (y1 + y2) / 2 - l * aux;
			cX = x1 + d2;
			cY = bY;
		} else if (p1.y == p2.y) {
			int aux = 1;
			aX = (x1 + x2) / 2 + l;
			if (Math.abs(aX - x1) < Math.abs(x2 - aX))
				aux = -1;
			aX = (x1 + x2) / 2 + l * aux;
			aY = y1;
			bX = (x1 + x2) / 2 - l * aux;
			bY = y1 + d2;
			cX = bX;
			cY = y1 - d2;
		} else if (p1.x != p2.x && p1.y != p2.y) {
			double length = Math.sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
			if (length <= d1)
				return;

			// calculate p4, p5
			double x3, x5, y5;
			x3 = (x1 + x2) / 2;

			aX = l / length * (x2 - x1) + x3;
			aY = findY(aX, x1, y1, x2, y2);

			x5 = l / length * (x1 - x2) + x3;
			y5 = findY(x5, x1, y1, x2, y2);

			double a2, b2, alfa;
			a2 = (x1 - x2) / (y2 - y1);
			b2 = y5 - a2 * x5;
			alfa = Math.atan(a2);

			bX = x5 - d2 * Math.cos(alfa);
			bY = findYPerp(bX, a2, b2);

			cX = x5 + d2 * Math.cos(alfa);
			cY = findYPerp(cX, a2, b2);
		}
		// set A, B and C
		A.x = new Double(aX).intValue();
		A.y = new Double(aY).intValue();
		B.x = new Double(bX).intValue();
		B.y = new Double(bY).intValue();
		C.x = new Double(cX).intValue();
		C.y = new Double(cY).intValue();
	}

	private double findY(double x, double x1, double y1, double x2, double y2) {
		return (y2 - y1) / (x2 - x1) * (x - x1) + y1;
	}

	private double findYPerp(double x, double a2, double b2) {
		return a2 * x + b2;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @return ?
	 */
	public Point getA() {
		return A;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @return ?
	 */
	public Point getB() {
		return B;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @return ?
	 */
	public Point getC() {
		return C;
	}

	/**
	 * TODO MS - comments here
	 * 
	 * @return ?
	 */
	public int[] getArrowCoordinates() {
		return new int[] { A.x, A.y, B.x, B.y, C.x, C.y };
		// return temp;
	}

	/**
	 * TODO MS - comments here
	 * @param p1 
	 * 
	 * @param p2 
	 * 
	 * @return ?
	 */
	public Rectangle getArrowLineAB(Point p1, Point p2) {
		calculatePointsABC(p1, p2);
		return new Rectangle(A.x, A.y, B.x, B.y);
	}

	/**
	 * TODO MS - comments here
	 * @param p1 
	 * @param p2 
	 * 
	 * @return ?
	 */
	public Rectangle getArrowLineAC(Point p1, Point p2) {
		calculatePointsABC(p1, p2);
		return new Rectangle(A.x, A.y, C.x, C.y);
	}

}
