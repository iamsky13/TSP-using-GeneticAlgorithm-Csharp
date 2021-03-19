using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShortestPathGenetic
{
    public class Nodes
    {
        public int[] Coordinates { get; set; }
        public int OrderIndex { get; set; }
    }

    
    public class Genetic
    {
        Random random = new Random();
        public Nodes[] GenerateNodes(int TotalNodes)
        {
            Nodes[] nodes = new Nodes[TotalNodes];
            
            for(int i = 0; i < TotalNodes; i++)
            {
                nodes[i] = new Nodes();
                nodes[i].Coordinates = new int[2] {random.Next(0,450), random.Next(0, 450)};
                nodes[i].OrderIndex = i;
            }
            return nodes;
        }

        public Nodes[] ShuffleOrderIndex(Nodes[] nodes, int shuffleFrequency)
        {
            for (int i = 0; i < shuffleFrequency; i++)
            {
                var firstIndex = random.Next(0, nodes.Length);
                var secondIndex = random.Next(0, nodes.Length);
                nodes = swapIndex(nodes, firstIndex, secondIndex);
            }
            return nodes;
        }

        private Nodes[] swapIndex(Nodes[] nodes, int firstIndex, int secondIndex)
        {
            int temp = nodes[firstIndex].OrderIndex;
            nodes[firstIndex].OrderIndex = nodes[secondIndex].OrderIndex;
            nodes[secondIndex].OrderIndex = temp;
            return nodes;
        }
    }
}
