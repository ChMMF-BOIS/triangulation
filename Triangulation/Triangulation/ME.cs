using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    class ME
    {
        private string format = "{0,6:00.00}";
        public int Id { get; set; }
        public double Se { get; set; }
        public double d { get; set; }
        public double[][] Me { get; set; }
        public ME(int Id, double Se, double d)
        {
            this.Id = Id;
            this.Se = Se;
            this.d = d;
            Me = new double[3][];
            Me[0] = new double[] { 2.0, 1.0, 1.0 };
            Me[1] = new double[] { 1.0, 2.0, 1.0 };
            Me[2] = new double[] { 1.0, 1.0, 2.0 };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Me[i][j] = (2.0 * d * Se) / (24.0) * (Me[i][j]);
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
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Me.txt", append))
            {
                string matrixMultiplier = "(" + d + "*2*" + Se.ToString() + ")/24";
                string leadingSpaces = new string(' ', matrixMultiplier.Length);
                string elementsSpaces = new string(' ', 2);
                double[][] Me2 = new double[3][];
                Me2[0] = new double[] { 2.0, 1.0, 1.0 };
                Me2[1] = new double[] { 1.0, 2.0, 1.0 };
                Me2[2] = new double[] { 1.0, 1.0, 2.0 };
                //
                file.WriteLine("Me " + Id.ToString());
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
                        line += String.Format(format, Me2[i][j]);
                        line += elementsSpaces;
                    }
                    line += String.Format(format, Me2[i][2]);
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
                        line += String.Format(format, Me[i][j]);
                        line += elementsSpaces;
                    }
                    line += String.Format(format, Me[i][2]);
                    line += "|";
                    file.WriteLine(line);
                }
            }
        }      
    }
}
