namespace Triangulation
{
    class Triangle
    {
        public int TriangleId { get; set; }
        public int Vertex0Id { get; set; }
        public int Vertex1Id { get; set; }
        public int Vertex2Id { get; set; }
        private double Area { get; set; }  
    }
}
