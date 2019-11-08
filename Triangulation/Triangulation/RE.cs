using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    class RE
    {
        private string format = "{0,6:00.00}";
        public int Id { get; set; }
        public double Length { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public double[][] CoefMatrix = new double[][]
        {
            new double[] { 2.0 / 6.0, 1.0 / 6.0 },
            new double[] { 1.0 / 6.0, 2.0 / 6.0 }
        };

        public double[][] Re;
        public string SegmentInfo { get; set; }
        public RE(int Id, double G, double B, double Length, string SegmentInfo)
        {
            this.Id = Id;
            this.G = G;
            this.B = B;
            this.Length = Length;
            this.SegmentInfo = SegmentInfo;
            Re = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                Re[i] = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    Re[i][j] = G/B*CoefMatrix[i][j]*Length;
                }
            }
        }
        public void print()
        {
            bool append = true;
            if (Id == 0)
            {
                append = false;
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Re.txt", append))
            {
                string matrixMultiplier = G.ToString() + "/" + B.ToString() + "*"+Length.ToString();
                string leadingSpaces = new string(' ', matrixMultiplier.Length);
                string elementsSpaces = new string(' ', 2);
                //
                file.WriteLine("Re " + Id.ToString());
                file.WriteLine("Segment: " + SegmentInfo);
                //матриці
                for (int i = 0; i < 2; i++)
                {
                    string line = "";
                    if (i != 0)
                    {
                        line += leadingSpaces;
                    }
                    else
                    {
                        line += matrixMultiplier;
                    }
                    line += "|";
                    for (int j = 0; j < 1; j++)
                    {
                        line += String.Format(format, CoefMatrix[i][j]);
                        line += elementsSpaces;
                    }
                    line += String.Format(format, CoefMatrix[i][1]);
                    line += "|";
                    if (i != 0)
                    {
                        line += "   ";
                    }
                    else
                    {
                        line += " = ";
                    }
                    line += "|";
                    for (int j = 0; j < 1; j++)
                    {
                        line += String.Format(format, Re[i][j]);
                        line += elementsSpaces;
                    }
                    line += String.Format(format, Re[i][1]);
                    line += "|";
                    file.WriteLine(line);
                }
            }
        }
    }
}
