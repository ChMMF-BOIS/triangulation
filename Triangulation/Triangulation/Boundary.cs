using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Topology;

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

        private List<MySubSegment> Segments;

        public Boundary()
        {
            begin = new Vertex();
            end = new Vertex();
            Id = 0;
            B = 0.0;
            G = 0.0;
            Uc = 0.0;
            Segments = new List<MySubSegment>();
        }

        public Boundary(Vertex begin, Vertex end, int Id)
        {
            this.begin = begin;
            this.end = end;
            this.Id = Id;
            B = 0.0;
            G = 0.0;
            Uc = 0.0;
            Segments = new List<MySubSegment>();
        }

        public Boundary(Vertex begin, Vertex end, int Id, double B, double G, double Uc)
        {
            this.begin = begin;
            this.end = end;
            this.Id = Id;
            this.B = B;
            this.G = G;
            this.Uc = Uc;
            Segments = new List<MySubSegment>();
        }

        public List<MySubSegment> GetSegments()
        {
            return Segments;
        }

        public void SetSegments(List<MySubSegment> Segments)
        {
            this.Segments = Segments;
        }

        public void SetSegments(ICollection<TriangleNet.Topology.SubSegment> subSegments, int inputVertexes)
        {
            Segments = new List<MySubSegment>();
            int begId = begin.ID, endId = end.ID;

            //var subSegment = subSegments.FirstOrDefault(ss => ss.P1 == begId);
            //while (subSegment.P0 != endId)
            //{
            //    begId = subSegment.P0;
            //    Segments.Insert(0,subSegment);
            //    subSegment = subSegments.FirstOrDefault(ss => ss.P1 == begId);
            //}
            //Segments.Insert(0, subSegment);

            var subSegment = subSegments.Where(ss => (ss.P1 == begin.ID) || (ss.P0 == begin.ID)).ElementAt(0);
            Segments.Add(new MySubSegment(subSegment));

            var usedVertexes = new List<int>();
            usedVertexes.Add(begin.ID);
            usedVertexes.Add(subSegment.P0 == begin.ID ? subSegment.P1 : subSegment.P0);

            while (usedVertexes.Last() != endId)
            {
                begId = usedVertexes.Last();
                var segmentsWithBegIdVertex = subSegments.Where(ss => ss.P0 == begId || ss.P1 == begId);
                foreach(var segment in segmentsWithBegIdVertex)
                {
                    if (!usedVertexes.Contains(segment.P0))
                    {
                        Segments.Add(new MySubSegment(segment));
                        usedVertexes.Add(segment.P0);
                        if(usedVertexes.Last() != endId && usedVertexes.Last() < inputVertexes)
                        {
                            Segments.Clear();
                            subSegment = subSegments.Where(ss => (ss.P1 == begin.ID) || (ss.P0 == begin.ID)).ElementAt(1);
                            Segments.Add(new MySubSegment(subSegment));
                            usedVertexes.Clear();
                            usedVertexes.Add(begin.ID);
                            usedVertexes.Add(subSegment.P0 == begin.ID ? subSegment.P1 : subSegment.P0);
                        }
                        break;
                    }
                    if (!usedVertexes.Contains(segment.P1))
                    {
                        Segments.Add(new MySubSegment(segment));
                        usedVertexes.Add(segment.P1);
                        if (usedVertexes.Last() != endId && usedVertexes.Last() < inputVertexes)
                        {
                            Segments.Clear();
                            subSegment = subSegments.Where(ss => (ss.P1 == begin.ID) || (ss.P0 == begin.ID)).ElementAt(1);
                            Segments.Add(new MySubSegment(subSegment));
                            usedVertexes.Clear();
                            usedVertexes.Add(begin.ID);
                            usedVertexes.Add(subSegment.P0 == begin.ID ? subSegment.P1 : subSegment.P0);
                        }
                        break;
                    }
                }
            }

        }

        public void SetSegmentsOrientationToCounterclockwise(int fromIndex, int toIndex)
        {
            int currentIndex = fromIndex;
            var subSegment = Segments.Where(ss => (ss.P1 == currentIndex) || (ss.P0 == currentIndex)).ElementAt(0);
            if(subSegment.P0 != currentIndex)
            {
                subSegment.Reverse();
            }
            currentIndex = subSegment.P1;
            int previousIndex = subSegment.P0;
            while(currentIndex != toIndex)
            {
                subSegment = Segments.Where(ss => ((ss.P1 == currentIndex) || (ss.P0 == currentIndex))&&(ss.P0 != previousIndex)).ElementAt(0);
                if (subSegment.P0 != currentIndex)
                {
                    subSegment.Reverse();
                }
                currentIndex = subSegment.P1;
                previousIndex = subSegment.P0;
            }
        }

    }
}
