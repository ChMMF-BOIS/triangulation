using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TriangleNet.Geometry;

namespace Triangulation
{
    class Boundary
    {
        private Vertex begin;
        private Vertex end;

        public int Id { get; set; }
        public double B { get; set; }
        public double G { get; set; }
        public double Uc { get; set; }

        public Boundary()
        {
            begin = new Vertex();
            end = new Vertex();
            Id = 0;
            B = 0.0;
            G = 0.0;
            Uc = 0.0;
        }

        public Boundary(Vertex begin, Vertex end, int Id)
        {
            this.begin = begin;
            this.end = end;
            this.Id = Id;
            B = 0.0;
            G = 0.0;
            Uc = 0.0;
        }

        public Boundary(Vertex begin, Vertex end, int Id, double B, double G, double Uc)
        {
            this.begin = begin;
            this.end = end;
            this.Id = Id;
            this.B = B;
            this.G = G;
            this.Uc = Uc;
        }

    }
}
