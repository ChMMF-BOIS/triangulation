using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TriangleNet.Geometry;
using System.IO;
using Microsoft.Win32;
using Point = TriangleNet.Geometry.Point;
using TriangleNet.Meshing;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Triangulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Vertex> mycoordinates = new List<Vertex>();
        TriangleNet.Meshing.IMesh meshResult;
        //List<TriangleNet.Topology.Triangle> triangleResult = new List<TriangleNet.Topology.Triangle>();
        List<Triangle> trianglesResult = new List<Triangle>();
        List<Vertex> verticesResult = new List<Vertex>();
        List<MyVertex> myvertixesResult = new List<MyVertex>();
        List<Boundary> boundaries = new List<Boundary>();

        public MainWindow()
        {
            InitializeComponent();
            SetupGridWithBoundaries(resultsBoundaries);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            mycoordinates.Clear();
            boundariesGrid.ItemsSource = null;
            coordinates.ItemsSource = null;

            hTextBox.Text = "0,1";
            LTextBox.Text = "30";

            resultsTriangles.ItemsSource = null;
            results.ItemsSource = null;
            resultsBoundaries.ItemsSource = null;
            resultsBoundaries.Items.Clear();

            MainCanvas.Children.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mycoordinates.Clear();
            //Vertex myvertex = new Vertex();
            //myvertex.X = 10;
            //myvertex.Y = 15;


            //// Constaint options
            //// Quality options 
            //// ITriangulator triangulator
            //// var options = new Tri
            //objct.Triangulate();

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = @"D:\4 курс 1 сем\ЧММФ\testTriangle\examples",
                Title = "Browse Text Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            Nullable<bool> result = openFileDialog1.ShowDialog();


            if (result == true)
            {
                //textBox1.Text = openFileDialog1.SafeFileName;
                //textBox2.Text = openFileDialog1.FileName;
                string readPath = openFileDialog1.FileName;
                using (StreamReader sr = new StreamReader(readPath))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var lineWords = line.Split(' ');
                        mycoordinates.Add(new Vertex(Convert.ToDouble(lineWords[0]), Convert.ToDouble(lineWords[1])));
                        mycoordinates[mycoordinates.Count - 1].ID = mycoordinates.Count - 1;
                    }
                }

                coordinates.ItemsSource = mycoordinates;

                //boundaries
                boundaries = GetBoundariesFromVertexes(mycoordinates);
                boundariesGrid.ItemsSource = boundaries;
            }
            // IPolygon poly = new TriangleNet.Geometry.Polygon();

        }

        private void Triangulate_Click(object sender, RoutedEventArgs e)
        {
            var qualityOptions = new TriangleNet.Meshing.QualityOptions();
            qualityOptions.MaximumArea = double.Parse(hTextBox.Text);
            qualityOptions.MinimumAngle = double.Parse(LTextBox.Text);

            var objct = new TriangleNet.Geometry.Polygon();

            foreach (var item in mycoordinates)
            {
                objct.Add(item);
            }

            for (int i = 0; i < mycoordinates.Count - 1; i++)
            {
                objct.Add(new Segment(mycoordinates[i], mycoordinates[i + 1]));
            }
            objct.Add(new Segment(mycoordinates.LastOrDefault(), mycoordinates.FirstOrDefault()));


            //var test = new TriangleNet.Meshing.Algorithm.SweepLine();
            var constraintOption = new TriangleNet.Meshing.ConstraintOptions();
            meshResult = objct.Triangulate(constraintOption, qualityOptions, new TriangleNet.Meshing.Algorithm.Incremental());
            //Triangulation
            //meshResult = objct.Triangulate(qualityOptions);
            meshResult.Renumber();

            //triangleResult = new List<TriangleNet.Topology.Triangle>(meshResult.Triangles);
            //Triangles
            trianglesResult = GetTrianglesFromMeshTriangles(meshResult.Triangles);
            resultsTriangles.ItemsSource = trianglesResult;
            //Vertices
            verticesResult = new List<Vertex>(meshResult.Vertices);
            myvertixesResult = meshResult.Vertices.Select(v => new MyVertex { Id = v.ID, X = v.X, Y = v.Y }).ToList();
            results.ItemsSource = myvertixesResult;

            //Заповнення граничних сегментів
            for (int i = 0; i < boundaries.Count; i++)
            {
                boundaries[i].SetSegments(meshResult.Segments, boundaries.Count);
            }
            //Перетворення орієнтації всіх сегментів проти годинникової стрілки
            for (int i = 0; i < boundaries.Count; i++)
            {
                if (i != boundaries.Count - 1)
                {
                    boundaries[i].SetSegmentsOrientationToCounterclockwise(i, i + 1);
                }
                else
                {
                    boundaries[i].SetSegmentsOrientationToCounterclockwise(i, 0);
                }
            }
            //
            FillGridWithBoundaries(resultsBoundaries, boundaries);
            DrawData(meshResult);

            //Етап 2: Створення матриць Me, Ke, Qe, Re, Pe
            //Підготовка
            //Зчитування параметрів
            double a11 = double.Parse(a11TextBox.Text);
            double a22 = double.Parse(a22TextBox.Text);
            double d = double.Parse(dTextBox.Text);
            double f = double.Parse(fTextBox.Text);
            //Створення масиву точок
            var CT = GetPointsArray(myvertixesResult);
            //Створення масиву трикутників
            var NT = GetTrianglesArray(trianglesResult);
            //Створення масиву граничних сегментів
            var NTGR = GetBoundarySegments(boundaries);
            //Створення масиву значень ф-кції f у точках
            var fe = GetFe(CT, f);
            //Етап 3 (ініціалізація A,B)
            //Кількість вузлів
            int nodeNumber = CT.Length;
            var A = new double[nodeNumber][];

            for (int i = 0; i < nodeNumber; i++)
            {
                A[i] = new double[nodeNumber];
            }

            var b = new double[nodeNumber];
            //Створення Me, Ke, Qe (та їх розсилання)
            double[] function_value = new double[3];
            for (int k = 0; k < trianglesResult.Count; k++)
            {
                //трикутник містить координати вузлів скінченного елемента
                var triangle = NT[k];
                //Підготовка
                double[] i = CT[triangle[0]],
                    j = CT[triangle[1]],
                    m = CT[triangle[2]];
                double Se = GetArea(i, j, m);
                double ai = GetA(j, m), bi = GetB(j, m), ci = GetC(j, m),
                    aj = GetA(m, i), bj = GetB(m, i), cj = GetC(m, i),
                    am = GetA(i, j), bm = GetB(i, j), cm = GetC(i, j);
                double[] a = new double[] { ai, aj, am },
                    B = new double[] { bi, bj, bm },
                    c = new double[] { ci, cj, cm };
                function_value[0] = fe[triangle[0]];
                function_value[1] = fe[triangle[1]];
                function_value[2] = fe[triangle[2]];
                //Ke
                var ke = new KE(k, Se, a11, a22, B, c);
                ke.print();
                //Me
                var me = new ME(k, Se, d);
                me.print();
                //Qe
                var qe = new QE(k, Se, fe);
                qe.print();
                //Етап 3
                for (int q = 0; q < 3; q++)
                {
                    //trinangle[q] це номер вузла елемента
                    for (int w = 0; w < 3; w++)
                    {
                        A[triangle[q]][triangle[w]] += ke.Ke[q][w] + me.Me[q][w];
                    }

                    b[triangle[q]] += qe.Qe[q][0];
                }
            }
            //Створення Re, Pe (та їх розсилання)
            for (int k = 0; k < boundaries.Count; k++)
            {
                for (int l = 0; l < NTGR[k].Length; l++)
                {
                    //сегмент містить координати вузлів граничного сегмента
                    var segment = NTGR[k][l];
                    //Підготовка
                    double[] i = CT[NTGR[k][l][0]],
                        j = CT[NTGR[k][l][1]];
                    double Length = GetLength(i, j);
                    //Re
                    var re = new RE(k + l, boundaries[k].G, boundaries[k].B,
                        Length, k + "|" + l + "|" + NTGR[k][l][0] + "-" + NTGR[k][l][1]);
                    re.print();
                    //Pe
                    var pe = new PE(k + l, boundaries[k].G, boundaries[k].B, boundaries[k].Uc,
                        Length, k + "|" + l + "|" + NTGR[k][l][0] + "-" + NTGR[k][l][1]);
                    pe.print();

                    //Етап 3
                    for (int q = 0; q < 2; q++)
                    {
                        //segment[q] це номер вузла сегмента
                        for (int w = 0; w < 2; w++)
                        {
                            A[segment[q]][segment[w]] += re.Re[q][w];
                        }

                        b[segment[q]] += pe.Pe[q][0];
                    }
                }
            }
            //Запис у файл A,b
            print(A, b);
        }
        //Обчислення довжини сегмента
        double GetLength(double[] i, double[] j)
        {
            return Math.Sqrt(Math.Pow(i[0] - j[0], 2) + Math.Pow(i[1] - j[1], 2));
        }
        //Обчислення площі Sijm
        double GetArea(double Xi1, double Xi2, double Xj1, double Xj2, double Xm1, double Xm2)
        {
            return 0.5 * (Xi1 * Xj2 + Xj1 * Xm2 + Xm1 * Xi2 - Xi2 * Xj1 - Xj2 * Xm1 - Xm2 * Xi1);
        }
        double GetArea(double[] i, double[] j, double[] m)
        {
            return 0.5 * (i[0] * j[1] + j[0] * m[1] + m[0] * i[1] - i[1] * j[0] - j[1] * m[0] - m[1] * i[0]);
        }
        //Створення масиву значень ф-кції f у точках
        double[] GetFe(double[][] CT, double f)
        {
            double[] result = new double[CT.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = f;
            }

            return result;
        }
        //Створення масиву точок
        double[][] GetPointsArray(List<MyVertex> vertices)
        {
            var pointsArray = new double[vertices.Count][];

            for (int i = 0; i < vertices.Count; i++)
            {
                pointsArray[i] = new double[2];
                if (i == vertices[i].Id)
                {
                    pointsArray[i][0] = vertices[i].X;
                    pointsArray[i][1] = vertices[i].Y;
                }
                else
                {
                    throw new Exception("Vertex Id does not correspond it's array Id.");
                }
            }

            return pointsArray;
        }
        //Створення масиву точок
        int[][] GetTrianglesArray(List<Triangle> triangles)
        {
            var trianglesArray = new int[triangles.Count][];

            for (int i = 0; i < triangles.Count; i++)
            {
                trianglesArray[i] = new int[3];
                if (i == triangles[i].TriangleId)
                {
                    trianglesArray[i][0] = triangles[i].Vertex0Id;
                    trianglesArray[i][1] = triangles[i].Vertex1Id;
                    trianglesArray[i][2] = triangles[i].Vertex2Id;
                }
                else
                {
                    throw new Exception("Triangle Id does not correspond it's array Id.");
                }
            }

            return trianglesArray;
        }
        //Створення масиву граничних сегментів
        private int[][][] GetBoundarySegments(List<Boundary> boundaries)
        {
            var boundarySegments = new int[boundaries.Count][][];

            for (int i = 0; i < boundaries.Count; i++)
            {
                var segments = boundaries[i].GetSegments();
                boundarySegments[i] = new int[segments.Count][];

                for (int j = 0; j < segments.Count; j++)
                {
                    boundarySegments[i][j] = new int[2];
                    boundarySegments[i][j][0] = segments[j].P0;
                    boundarySegments[i][j][1] = segments[j].P1;
                }
            }

            return boundarySegments;
        }
        //Обчислення a
        double GetA(double[] j, double[] m)
        {
            return j[0] * m[1] - j[1] * m[0];
        }
        //Обчислення b
        double GetB(double[] j, double[] m)
        {
            return j[1] - m[1];
        }
        //Обчислення c
        double GetC(double[] j, double[] m)
        {
            return m[0] - j[0];
        }

        //Виведення матриці A та вектора b
        private void print(double[][] A, double[] b)
        {
            //
            int leadingSpacesWidth = b.Length.ToString().Count();
            double aMinElement = A[0][0], aMaxElement = A[0][0],
                bMinElement = b[0], bMaxElement = b[0];

            for (int i = 0; i < b.Length; i++)
            {
                if (bMinElement > b[i])
                {
                    bMinElement = b[i];
                }
                if (bMaxElement < b[i])
                {
                    bMaxElement = b[i];
                }

                for (int j = 0; j < b.Length; j++)
                {
                    if (aMinElement > A[i][j])
                    {
                        aMinElement = A[i][j];
                    }
                    if (aMaxElement < A[i][j])
                    {
                        aMaxElement = A[i][j];
                    }
                }
            }

            int AMinElementLength = Math.Truncate(aMinElement).ToString().Count(),
                 AMaxElementLength = Math.Truncate(aMaxElement).ToString().Count(),
                 bMinElementLength = Math.Truncate(bMinElement).ToString().Count(),
                 bMaxElementLength = Math.Truncate(bMaxElement).ToString().Count();

            int precision = 14,
                elementSpaces = 3,
                aElementWidth = precision + Math.Max(AMinElementLength, AMaxElementLength),
                bElementWidth = precision + Math.Max(bMinElementLength, bMaxElementLength);
            //перша стрічка
            string firstline = new string(' ', leadingSpacesWidth+1);
            for (int i = 0; i < b.Length; i++)
            {
                //firstline += String.Format("{0:d" + (aElementWidth+1).ToString() + "}{1}", i, new string(' ', elementSpaces));
                firstline += String.Format(new string(' ', aElementWidth + 1 - i.ToString().Count())+"{0:d}{1}", i, new string(' ', elementSpaces));
            }
            //друга стрічка
            string secondline = new string(' ', leadingSpacesWidth+1);
            for (int i = 0; i < b.Length; i++)
            {
                secondline += String.Format("{0}{1}", new string('-', aElementWidth+1), new string(' ', elementSpaces));
            }
            //всі інші стрічки 6:00.00
            string otherline = "{0:d" + leadingSpacesWidth.ToString() + "}|";
            for (int i = 0; i < b.Length; i++)
            {
                otherline += 
                    "{" + (i + 1).ToString() + "," 
                    +(aElementWidth+1).ToString()+ ":"+new string('0', aElementWidth - precision) + "." + new string('0', precision - 1) + "}"
                    + new string(' ', elementSpaces);
            }
            otherline += "|{"+(b.Length + 1).ToString()+","
                 + (bElementWidth+1).ToString() + ":" + new string('0', bElementWidth - precision) + "." + new string('0', precision - 1) + "}" +
                "|{" + (b.Length + 2).ToString()+"}";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Ab.txt", false))
            {
                file.WriteLine(firstline);
                file.WriteLine(secondline);
                for(int i = 0; i < b.Length; i++)
                {
                    var argumentsRow = new object[b.Length + 3];
                    argumentsRow[0] = i;
                    for(int j = 1; j < b.Length + 1; j++)
                    {
                        argumentsRow[j] = A[i][j - 1];
                    }
                    argumentsRow[b.Length + 1] = b[i];
                    argumentsRow[b.Length + 2] = i;
                    file.WriteLine(String.Format(otherline, argumentsRow));
                }
                file.WriteLine(secondline);
                file.WriteLine(firstline);
            }
        }

        private List<Boundary> GetBoundariesFromVertexes(List<Vertex> vertexes)
        {
            List<Boundary> boundaries = new List<Boundary>();

            int i = 0;
            for (i = 0; i < vertexes.Count - 1; i++)
            {
                boundaries.Add(new Boundary(vertexes[i], vertexes[i + 1], i));
            }

            boundaries.Add(new Boundary(vertexes[i], vertexes[0], i));

            return boundaries;
        }

        private List<Triangle> GetTrianglesFromMeshTriangles(IEnumerable<TriangleNet.Topology.Triangle> collection)
        {
            var triangles = new List<Triangle>(collection.Count());

            foreach (var triangle in collection)
            {
                triangles.Add(new Triangle
                {
                    TriangleId = triangle.ID,
                    Vertex0Id = triangle.GetVertexID(0),
                    Vertex1Id = triangle.GetVertexID(1),
                    Vertex2Id = triangle.GetVertexID(2)
                });
            }

            return triangles;
        }

        private void SetupGridWithBoundaries(DataGrid boundariesGrid)
        {
            boundariesGrid.Items.Clear();
            boundariesGrid.Columns.Clear();
            var GtextColumn = new DataGridTextColumn();
            GtextColumn.Header = "G";
            GtextColumn.Binding = new Binding("G");
            boundariesGrid.Columns.Add(GtextColumn);
            var P0textColumn = new DataGridTextColumn();
            P0textColumn.Header = "P0";
            P0textColumn.Binding = new Binding("P0");
            boundariesGrid.Columns.Add(P0textColumn);
            var P1textColumn = new DataGridTextColumn();
            P1textColumn.Header = "P1";
            P1textColumn.Binding = new Binding("P1");
            boundariesGrid.Columns.Add(P1textColumn);
        }

        private void FillGridWithBoundaries(DataGrid boundariesGrid, List<Boundary> boundaries)
        {
            foreach (var boundary in boundaries)
            {
                foreach (var segment in boundary.GetSegments())
                {
                    boundariesGrid.Items.Add(new { G = boundary.Id, P0 = segment.P0, P1 = segment.P1 });
                }
            }

            boundariesGrid.Items.Refresh();
        }

        private void DrawData(IMesh mesh)
        {
            foreach (ITriangle triangle in mesh.Triangles)
            {
                DrawTriangle(triangle);
                DrawCircle(triangle);
            }

            foreach (var vertex in mesh.Vertices)
            {
                DrawVertex(vertex);
            }

            MainViewBox.UpdateLayout();
        }

        private double GetLength(Vertex x, Vertex y)
        {
            return Math.Sqrt(Math.Pow(x.X - y.X, 2) + Math.Pow(x.Y - y.Y, 2));
        }

        private Point GetIncenter(ITriangle triangle)
        {
            Point incenter = new Point();
            Vertex A = triangle.GetVertex(0), B = triangle.GetVertex(1), C = triangle.GetVertex(2);
            double a = GetLength(B, C), b = GetLength(A, C), c = GetLength(A, B);
            double p = a + b + c;
            incenter.X = (a * A.X + b * B.X + c * C.X) / p;
            incenter.Y = (a * A.Y + b * B.Y + c * C.Y) / p;
            return incenter;
        }

        private void DrawCircle(ITriangle triangle)
        {
            Point incenter = GetIncenter(triangle);

            var k = 80.0;
            System.Windows.Point center = new System.Windows.Point
            {
                X = k * incenter.X + 250.0,
                Y = k * incenter.Y + 100.0
            };

            double size = 10;
            //add circle           
            Ellipse circle = new Ellipse()
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Red
            };

            circle.SetValue(Canvas.LeftProperty, center.X - circle.Width / 2);
            circle.SetValue(Canvas.TopProperty, center.Y - circle.Height / 2);
            MainCanvas.Children.Add(circle);
            //add ID
            TextBlock triangleId = new TextBlock();
            triangleId.Text = triangle.ID.ToString();
            triangleId.Foreground = circle.Stroke;
            Viewbox viewbox = new Viewbox();
            viewbox.Visibility = Visibility.Visible;
            viewbox.Stretch = Stretch.Uniform;
            viewbox.Child = triangleId;
            viewbox.Height = circle.Height / 2;
            viewbox.Width = circle.Width / 2;
            viewbox.MouseEnter += ViewBoxOnMouseDown;
            viewbox.SetValue(Canvas.LeftProperty, center.X - viewbox.Width / 2);
            viewbox.SetValue(Canvas.TopProperty, center.Y - viewbox.Height / 2);
            MainCanvas.Children.Add(viewbox);
        }

        private void DrawVertex(Vertex vertex)
        {
            var k = 80.0;
            System.Windows.Point center = new System.Windows.Point
            {
                X = k * vertex.X + 250.0,
                Y = k * vertex.Y + 100.0
            };

            double size = 10;
            //add circle           
            Ellipse circle = new Ellipse()
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Green
            };

            circle.SetValue(Canvas.LeftProperty, center.X - circle.Width / 2);
            circle.SetValue(Canvas.TopProperty, center.Y - circle.Height / 2);
            MainCanvas.Children.Add(circle);
            //add ID
            TextBlock vertexId = new TextBlock();
            vertexId.Text = vertex.ID.ToString();
            vertexId.Foreground = circle.Stroke;
            Viewbox viewbox = new Viewbox();
            viewbox.Visibility = Visibility.Visible;
            viewbox.Stretch = Stretch.Uniform;
            viewbox.Child = vertexId;
            viewbox.Height = circle.Height / 2;
            viewbox.Width = circle.Width / 2;
            viewbox.MouseEnter += ViewBoxOnMouseDown;
            viewbox.SetValue(Canvas.LeftProperty, center.X - viewbox.Width / 2);
            viewbox.SetValue(Canvas.TopProperty, center.Y - viewbox.Height / 2);
            MainCanvas.Children.Add(viewbox);
        }

        private void ViewBoxOnMouseDown(object sender, MouseEventArgs e)
        {
            var viewBox = (Viewbox)sender;
            var idBlock = viewBox.Child as TextBlock;
            if (idBlock.Foreground == Brushes.Red)
            {
                ElementId.Text = "Triangle id: " + (idBlock).Text;
            }
            else
            {
                ElementId.Text = "Vertex id: " + (idBlock).Text;
            }
        }

        private void DrawTriangle(ITriangle triangle)
        {
            var k = 80.0;
            System.Windows.Point p1 = new System.Windows.Point
            {
                X = k * triangle.GetVertex(0).X + 250.0,
                Y = k * triangle.GetVertex(0).Y + 100.0
            };
            System.Windows.Point p2 = new System.Windows.Point
            {
                X = k * triangle.GetVertex(1).X + 250.0,
                Y = k * triangle.GetVertex(1).Y + 100.0
            };
            System.Windows.Point p3 = new System.Windows.Point
            {
                X = k * triangle.GetVertex(2).X + 250.0,
                Y = k * triangle.GetVertex(2).Y + 100.0
            };
            DrawableTriangle myTriangle = new DrawableTriangle
            {
                Points = new List<System.Windows.Point> { p1, p2, p3 },
            };

            myTriangle.Stroke = Brushes.Black;
            myTriangle.StrokeThickness = 0.2;
            MainCanvas.Children.Add(myTriangle);
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //double zoom = e.Delta > 0 ? .2 : -.2;


            //foreach (var child in MainCanvas.Children)
            //{
            //    var currentZoom = (child as UIElement).RenderTransform.Value.M11;
            //    var renderTransform = new MatrixTransform();
            //    renderTransform.Matrix = new Matrix(currentZoom + zoom, 0, 0, currentZoom + zoom, 0, 0);

            //    (child as UIElement).RenderTransform = renderTransform;
            //}
        }

    }
}
