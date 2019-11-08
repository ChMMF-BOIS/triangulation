using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    class QE
    {
        private string format = "{0,6:00.00}";
        public int Id { get; set; }
        public double Se { get; set; }
        public double[][] Qe { get; set; }
        public double[] f { get; set; }
        public QE(int Id, double Se, double[] f)
        {
            this.Id = Id;
            this.Se = Se;
            this.f = f;
            double[][] Qe1 = new double[3][];
            Qe = new double[3][];
            Qe1[0] = new double[] { 2.0, 1.0, 1.0 };
            Qe1[1] = new double[] { 1.0, 2.0, 1.0 };
            Qe1[2] = new double[] { 1.0, 1.0, 2.0 };

            for (int i = 0; i < 3; i++)
            {
                Qe[i] = new double[1];

                    Qe[i][0] = Se / (12.0) * (Qe1[i][0]*f[0]+ Qe1[i][1] * f[1]+ Qe1[i][2] * f[2]);
                
            }


        }
        public void print()
        {
            bool append = true;
            if (Id == 0)
            {
                append = false;
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\Matrixes\Qe.txt", append))
            {
                string matrixMultiplier = Se.ToString() + "/12";
                string[] matrixVecrtorMultiplier = new string[3];
                string leadingSpaces = new string(' ', matrixMultiplier.Length);
                string elementsSpaces = new string(' ', 2);
                string multiplier = " * ";
                double[][] Qe2 = new double[3][];
                Qe2[0] = new double[] { 2.0, 1.0, 1.0 };
                Qe2[1] = new double[] { 1.0, 2.0, 1.0 };
                Qe2[2] = new double[] { 1.0, 1.0, 2.0 };
                //
                file.WriteLine("Qe " + Id.ToString());
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
                        line += String.Format(format, Qe2[i][j]);
                        line += elementsSpaces;
                    }
                    //line += String.Format(format, (Qe2[i][2] + "|" + multiplier + "|" + f[i] + "|"));
                    line += String.Format(format, Qe2[i][2]);
                    line += "|";
                    line += multiplier + "|";
                    line += String.Format(format, f[i]);
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
                    line += String.Format(format, Qe[i][0]);
                    line += "|";
                    file.WriteLine(line);
                }
            }
        }

    }
}
