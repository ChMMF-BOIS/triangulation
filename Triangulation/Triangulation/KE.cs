using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    public class KE
    {
        private string format = "{0,6:00.00}";
        public int Id { get; set; }
        public double Se { get; set; }
        public double a11 { get; set; }
        public double a22 { get; set; }
        public double[] b { get; set; }
        public double[] c { get; set; }
        public double[][] Ke { get; set; }
        public KE(int Id, double Se, double a11, double a22, double[] b, double[] c)
        {
            this.Id = Id;
            this.Se = Se;
            this.a11 = a11;
            this.a22 = a22;
            this.b = b;
            this.c = c;
            Ke = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                Ke[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    Ke[i][j] = 1.0 / (4.0 * Se) * (a11 * b[i] * b[j] + a22 * c[i] * c[j]);
                }
            }
        }
        public void print()
        {
            bool append = true;
            if(Id == 0)
            {
                append = false;
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Ke.txt", append))
            {
                string matrixMultiplier = "1/(4*" + Se.ToString() + ")";
                string leadingSpaces = new string(' ', matrixMultiplier.Length);
                string elementsSpaces = new string(' ', 2);
                string plus = " + ";
                //
                file.WriteLine("Ke " + Id.ToString());
                //матриці
                for (int i = 0; i < 3; i++)
                {
                    string line = "";
                    if (i != 1)
                    {
                        line += leadingSpaces;
                    }
                    else
                    {
                        line += matrixMultiplier;
                    }
                    line += "|";
                    for (int j = 0; j < 2; j++)
                    {
                        line += GetLine(b, i, j, a11);
                        line += plus;
                        line += GetLine(c, i, j, a22);
                        line += elementsSpaces;
                    }
                    line += GetLine(b, i, 2, a11);
                    line += plus;
                    line += GetLine(c, i, 2, a22);
                    line += "|";
                    if (i != 1)
                    {
                        line += "   ";
                    }
                    else
                    {
                        line += " = ";
                    }
                    line += "|";
                    for (int j = 0; j < 2; j++)
                    {
                        line += String.Format(format, Ke[i][j]);
                        line += elementsSpaces;
                    }
                    line += String.Format(format, Ke[i][2]);
                    line += "|";
                    file.WriteLine(line);
                }
            }
        }
        private string GetLine(double[] array, int iIndex, int jIndex, double multiplayer)
        {
            return String.Format(format, multiplayer) + " *" + String.Format(format, array[iIndex]) + " *" + String.Format(format, array[jIndex]);
        }
    }
}
