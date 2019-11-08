using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TriangleNet.Topology;

namespace Triangulation
{
    class MySubSegment
    {
        public MySubSegment()
        {
            P0 = 0;
            P1 = 0;
            Label = 0;
        }
        public MySubSegment(SubSegment subSegment)
        {
            this.P0 = subSegment.P0;
            this.P1 = subSegment.P1;
            this.Label = subSegment.Label;
        }

        public int P0 { get; set; }
        public int P1 { get; set; }
        public int Label { get; set; }
        public void Reverse()
        {
            int temp = P0;
            P0 = P1;
            P1 = temp;
        }
    }
}
