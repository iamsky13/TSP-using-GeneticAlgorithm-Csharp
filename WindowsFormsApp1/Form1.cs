using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortestPathGenetic
{
    public partial class Form1 : Form
    {
        Graphics graphic;
        Graphics bestPathGraphic;
        Pen pen;
        Pen pen1;
        Genetic geneticAlgService = new Genetic();
        CancellationTokenSource cancellationTokenSource;
        public Form1()
        {
            InitializeComponent();
            graphic = panel1.CreateGraphics();
            bestPathGraphic = bestPathPanel.CreateGraphics();
            button1.Text = "Start";
            //currentPath.Text = "0";
        }
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            int startNodeIndex = 0;
            int DestinationNodeIndex = 6;
            int totalNodes = 10;
            if (button1.Text == "Start")
            {
                button1.Text = "Stop";
            }
            await Task.Factory.StartNew(async () =>
            {
                int populationSize = 1000;
                var nodes = geneticAlgService.GenerateNodes(totalNodes, startNodeIndex, DestinationNodeIndex);

                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    try
                    {
                        Population[] population = await CalculateSamplePopulation(populationSize, nodes, cancellationTokenSource.Token);
                        Population bestPopulation = await CalculateBestPopulation(population, double.MaxValue, cancellationTokenSource.Token);
                        NormalizeFitness(population);

                        Population[] nextGenerationPopulation = await geneticAlgService.CalculateNextGenerationPopulation(population);
                        await CalculateBestPopulation(nextGenerationPopulation, bestPopulation.PathLength, cancellationTokenSource.Token);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        cancellationTokenSource = null;
                    }
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = null;
                }

                //}
            });
            //graphic.Clear(Color.White);
            //GenerateGraphic(bestPopulation.Nodes);

        }



        private async Task<Population> CalculateBestPopulation(Population[] samplePopulations, double minDistance, CancellationToken token)
        {
            Population bestPopulation = new Population();

            for (int i = 0; i < samplePopulations.Length; i++)
            {
                double TotalPathDistance = geneticAlgService.CalculateTotalDistance(samplePopulations[i].Nodes);
                samplePopulations[i].PathLength = TotalPathDistance;
                graphic.Clear(Color.White);
                await GenerateGraphic(samplePopulations[i]);
                Console.WriteLine(i);

                if (TotalPathDistance < minDistance)
                {
                    minDistance = TotalPathDistance;
                    bestPopulation = samplePopulations[i];
                    bestPathGraphic.Clear(Color.White);
                    await GenerateBestpathGraphic(bestPopulation);
                }
                samplePopulations[i].Fitness = 1 / (TotalPathDistance + 1);
            }
            return bestPopulation;
        }

        private void NormalizeFitness(Population[] population)
        {
            double sum = 0;
            for (int i = 0; i < population.Length; i++)
            {
                sum += population[i].Fitness;
            }
            for (int j = 0; j < population.Length; j++)
            {
                population[j].Fitness = population[j].Fitness / sum;
            }
        }

        private async Task<Population[]> CalculateSamplePopulation(int populationSize, Nodes[] nodes, CancellationToken cancellationToken)
        {
            Population[] samplePopulation = new Population[populationSize];
            for (var i = 0; i < populationSize; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    button1.Text = "Start";
                    break;
                }
                Nodes[] copyofNodes = geneticAlgService.DeepCopyNodes(nodes);
                samplePopulation[i] = new Population();
                samplePopulation[i].Nodes = geneticAlgService.ShuffleNodes(copyofNodes, 10 * (i + 1));

            }
            //button1.Text = "Start";
            return samplePopulation;
        }



        private async Task GenerateGraphic(Population population)
        {
            //button1.Text = population.PathLength.ToString();
            //currentPath.Text = population.PathLength.ToString("0.####");
            await Task.Factory.StartNew(() =>
            {

                pen = new Pen(Color.Red, 5);
                pen1 = new Pen(Color.White, 1);
                SolidBrush sb = new SolidBrush(Color.Red);
                Point p1 = new Point(0, 0);

                graphic.DrawString(population.PathLength.ToString("0.####"),new Font("Arial",12),Brushes.Black,327, 20);
                for (int i = 0; i < population.Nodes.Length; i++)
                {

                    graphic.DrawEllipse(pen, population.Nodes[i].Coordinates[0], population.Nodes[i].Coordinates[1], 5, 5);
                    graphic.DrawString(population.Nodes[i].NodeName, new Font("Arial", 10), Brushes.Black, population.Nodes[i].Coordinates[0] + 5, population.Nodes[i].Coordinates[1] + 5);
                }
                for (int i = 0; i < population.Nodes.Length - 1; i++)
                {
                    Nodes NodeU = Array.Find(population.Nodes, a => a.OrderIndex == i);
                    Nodes NodeV = Array.Find(population.Nodes, a => a.OrderIndex == (i + 1));

                    graphic.DrawLine(pen,
                        new Point(NodeU.Coordinates[0], NodeU.Coordinates[1]),
                         new Point(NodeV.Coordinates[0], NodeV.Coordinates[1]));
                }
            });

        }
        private async Task GenerateBestpathGraphic(Population bestPopulation)
        {

            await Task.Factory.StartNew(() =>
            {

                pen = new Pen(Color.Red, 5);
                pen1 = new Pen(Color.White, 1);
                SolidBrush sb = new SolidBrush(Color.Red);
                Point p1 = new Point(0, 0);
                bestPathGraphic.DrawString(bestPopulation.PathLength.ToString("0.####"), new Font("Arial", 12), Brushes.Green, 327, 20);

                for (int i = 0; i < bestPopulation.Nodes.Length; i++)
                {

                    bestPathGraphic.DrawEllipse(pen, bestPopulation.Nodes[i].Coordinates[0], bestPopulation.Nodes[i].Coordinates[1], 5, 5);
                    bestPathGraphic.DrawString(bestPopulation.Nodes[i].NodeName, new Font("Arial", 10), Brushes.Black, bestPopulation.Nodes[i].Coordinates[0] + 5, bestPopulation.Nodes[i].Coordinates[1] + 5);
                }
                for (int i = 0; i < bestPopulation.Nodes.Length - 1; i++)
                {

                    Nodes NodeU = Array.Find(bestPopulation.Nodes, a => a.OrderIndex == i);
                    Nodes NodeV = Array.Find(bestPopulation.Nodes, a => a.OrderIndex == (i + 1));

                    bestPathGraphic.DrawLine(pen,
                        new Point(NodeU.Coordinates[0], NodeU.Coordinates[1]),
                         new Point(NodeV.Coordinates[0], NodeV.Coordinates[1]));
                }
            });

        }

        private void currentPath_Click(object sender, EventArgs e)
        {

        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
