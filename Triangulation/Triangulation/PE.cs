using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    class PE
    {
        private string format = "{0,6:00.00}";
        public int Id { get; set; }
        public double Length { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double Uc { get; set; }

        public double[][] CoefMatrix = new double[][]
      {
            new double[] { 0.5 },
            new double[] { 0.5 }
      };

        public double[][] Pe;

        public string SegmentInfo { get; set; }

        public PE(int Id, double G, double B,double Uc, double Length, string SegmentInfo)
        {
            this.Id = Id;
            this.G = G;
            this.B = B;
            this.Uc = Uc;
            this.Length = Length;
            this.SegmentInfo = SegmentInfo;
            Pe = new double[2][];
            for (int i = 0; i < 2; i++)
            {
                Pe[i] = new double[1];
                for (int j = 0; j < 1; j++)
                {
                    Pe[i][j] = Uc* G / B * CoefMatrix[i][j] * Length;
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
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Pe.txt", append))
            {
                string matrixMultiplier = Uc.ToString() + "*" + G.ToString() + "/" + B.ToString() + "*" + Length.ToString();
                string leadingSpaces = new string(' ', matrixMultiplier.Length);
                //
                file.WriteLine("Pe " + Id.ToString());
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
                    line += String.Format(format, CoefMatrix[i][0]);
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
                    line += String.Format(format, Pe[i][0]);
                    line += "|";
                    file.WriteLine(line);
                }
            }
        }
    }
}
