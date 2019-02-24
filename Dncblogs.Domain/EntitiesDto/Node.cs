using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class Node
    {
        public int id { get; set; }
        public int pId { get; set; }
        public int userId { get; set; }
        public string text { get; set; }
        public string href { get; set; }
        public int sort { get; set; }
        public List<Node> nodes { get; set; }
    }
}
