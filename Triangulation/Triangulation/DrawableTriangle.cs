using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Triangulation
{
    public class DrawableTriangle : Shape
    {
        public List<Point> Points {
            get { return (List<Point>)this.GetValue(PointProperty); }
            set { this.SetValue(PointProperty, value); }
        }

        public static readonly DependencyProperty PointProperty = DependencyProperty.Register("Points", typeof(List<Point>), typeof(Triangle));

        public DrawableTriangle()
        {
        }

        protected override Geometry DefiningGeometry {
            get {

                Point p1 = new Point(Points[0].X, Points[0].Y);
                Point p2 = new Point(Points[1].X, Points[1].Y);
                Point p3 = new Point(Points[2].X, Points[2].Y);

                List<PathSegment> segments = new List<PathSegment>(3);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));

                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p1, segments, true);
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);

                return g;
            }
        }
    }
}
