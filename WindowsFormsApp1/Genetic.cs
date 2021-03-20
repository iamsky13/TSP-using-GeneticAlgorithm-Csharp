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
        public string NodeName { get; set; }
        public bool IsStartOrEndNode { get; set; }
    }

    public class Population
    {
        public Nodes[] Nodes { get; set; }
        public Double Fitness { get; set; }
        public double PathLength { get; set; }
    }
    public class Genetic
    {
        Random random = new Random();
        public Nodes[] GenerateNodes(int TotalNodes, int startNodeIndex, int destinationNodeIndex)
        {
            Nodes[] nodes = new Nodes[TotalNodes];
            
            for(int i = 0; i < TotalNodes; i++)
            {
                nodes[i] = new Nodes();
                nodes[i].Coordinates = new int[2] {random.Next(0,430), random.Next(0, 530)};
                nodes[i].OrderIndex = i;
                nodes[i].NodeName = "V" + i;
            }
            nodes[startNodeIndex].IsStartOrEndNode = true;
            nodes[destinationNodeIndex].IsStartOrEndNode = true;
            if(destinationNodeIndex+1 != TotalNodes)
            {
                SwapCoordinates(nodes, destinationNodeIndex, TotalNodes - 1);
            }
            return nodes;
        }

        public Nodes[] ShuffleNodes(Nodes[] nodes, int shuffleFrequency)
        {
            for (int i = 0; i < shuffleFrequency; i++)
            {
                var firstIndex = random.Next(0, nodes.Length);
                var secondIndex = random.Next(0, nodes.Length);
                if(!nodes[firstIndex].IsStartOrEndNode && !nodes[secondIndex].IsStartOrEndNode)
                {
                    nodes = SwapCoordinates(nodes, firstIndex, secondIndex);
                }
                
            }
            return nodes;
        }

        private Nodes[] SwapCoordinates(Nodes[] nodes, int firstIndex, int secondIndex)
        {
            var temp = nodes[firstIndex].OrderIndex;
            nodes[firstIndex].OrderIndex = nodes[secondIndex].OrderIndex;
            nodes[secondIndex].OrderIndex = temp;
            return nodes;
        }

        public double CalculateTotalDistance(Nodes[] nodes)
        {
            double PathSum = 0;
            for (int i=0; i < nodes.Length - 1; i++)
            {
                Nodes nodeA = Array.Find(nodes, a => a.OrderIndex == i);
                Nodes nodeB = Array.Find(nodes, a => a.OrderIndex == (i + 1));
                double distance = CalculateDisplacement(nodeA.Coordinates, nodeB.Coordinates);
                PathSum += distance;
            }
            return PathSum;
        }

        private double CalculateDisplacement(int[] coordinates1, int[] coordinates2)
        {
            double distX = coordinates1[0] - coordinates2[0];
            double distY = coordinates1[1] - coordinates2[1];
            return Math.Sqrt(distX * distX + distY * distY);
        }
        public async Task<Population[]> CalculateNextGenerationPopulation(Population[] populations)
        {
            Population[] newPopulations = new Population[populations.Length];
            for (var i = 0; i < populations.Length; i++)
            {
                var tempPopulation = PickOnePopulation(populations);
                newPopulations[i] = new Population();
                MutatePopulation(tempPopulation,10*(i+1));
                newPopulations[i].Nodes = DeepCopyNodes(tempPopulation.Nodes);
            }
            return newPopulations;
        }

        public void MutatePopulation(Population population, int mutationRate)
        {
            ShuffleNodes(population.Nodes, mutationRate); 
        }
        public Population PickOnePopulation(Population[] populations)
        {   int index = 0;
            double r = random.NextDouble();
            while (r > 0)
            {
                r = r - populations[index].Fitness;
                index++;
            }
            index--;
            return (populations[index]);
        }
        public Nodes[] DeepCopyNodes(Nodes[] nodes)
        {
            Nodes[] newNodes = new Nodes[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                newNodes[i] = new Nodes
                {
                    Coordinates = nodes[i].Coordinates,
                    OrderIndex = nodes[i].OrderIndex,
                    NodeName=nodes[i].NodeName,
                    IsStartOrEndNode=nodes[i].IsStartOrEndNode
                };
            }
            return newNodes;
        }
    }
}
