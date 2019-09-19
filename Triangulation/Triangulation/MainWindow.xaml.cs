using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TriangleNet;
using System.Threading.Tasks;
using TriangleNet.Geometry;
using System.IO;
using Microsoft.Win32;
using Point = TriangleNet.Geometry.Point;

namespace Triangulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Vertex> mycoordinates = new List<Vertex>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
                    }
                }
            }
            // IPolygon poly = new TriangleNet.Geometry.Polygon();
           
          
            coordinates.ItemsSource = mycoordinates;

            //boundaries
            var boundaries = GetBoundariesFromVertexes(mycoordinates);
            boundariesGrid.ItemsSource = boundaries;
        }

        private List<Boundary> GetBoundariesFromVertexes(List<Vertex> vertexes)
        {
            List<Boundary> boundaries = new List<Boundary>();

            int i = 0;
            for(i = 0; i < vertexes.Count - 1; i++)
            {
                boundaries.Add(new Boundary(vertexes[i], vertexes[i + 1], i));
            }

            boundaries.Add(new Boundary(vertexes[i], vertexes[0], i));

            return boundaries;
        }

        private void Triangulate_Click(object sender, RoutedEventArgs e)
        {
            var options = new TriangleNet.Meshing.QualityOptions();
            options.MaximumArea = double.Parse(hTextBox.Text);
            options.MinimumAngle = double.Parse(LTextBox.Text);

            var objct = new TriangleNet.Geometry.Polygon();

            foreach (var item in mycoordinates)
            {
                objct.Add(item);
            }

            var somet = objct.Triangulate(options);
            results.ItemsSource = somet.Vertices;
        }
    }
}
