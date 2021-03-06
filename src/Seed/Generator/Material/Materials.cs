using Seed.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seed.Generator.Material
{
    public class Materials : List<MaterialHirachie>, IWithOperationsInUse, IWithNodeInUse
    {
        public List<MaterialNode> NodesInUse { get;  } = new List<MaterialNode>();
        public List<MaterialNodeOperation> Operations { get => NodesInUse.SelectMany(x => x.Operations).ToList(); }
        public List<MaterialEdge> Edges {  get; } = new List<MaterialEdge>();
        public int CountDequeuedNodesFor(int level) => NodesInUse.Count(x => x.InitialLevel == level);
        /// <summary>
        /// Returns one node from NodeInUse store.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Node at Index</returns>
        public MaterialNode GetNodeInUseBy(int index)
        {
            return NodesInUse[index];
        }
        public MaterialNode GetNodeInUseBy(int level, int index)
        {
            return NodesInUse.Where(x => x.InitialLevel == level)
                             .ElementAt(index);
        }
        public int CountNodesInUse => NodesInUse.Count();

        public MaterialNode[] ToNodeArray => NodesInUse.ToArray();
 
        public MaterialNode[] NodesWithoutPurchase()
        {
            return NodesInUse.Where(x => x.IncomingEdges.Any() && !x.OutgoingEdges.Any()).ToArray();
        }
        public MaterialNode[] NodesWithoutSales()
        {
            return NodesInUse.Where(x => x.InitialLevel != 0).ToArray();
        }
        public MaterialNode[] NodesSalesOnly()
        {
            return NodesInUse.Where(x => x.IncomingEdges.Any() && !x.OutgoingEdges.Any()).ToArray();
        }
        public MaterialNode[] NodesPurchaseOnly()
        {
            return NodesInUse.Where(x => !x.IncomingEdges.Any() && x.OutgoingEdges.Any()).ToArray();
        }
        public MaterialNode[] NodesAssemblyOnly()
        {
            return NodesInUse.Where(x => x.IncomingEdges.Any() && x.OutgoingEdges.Any()).ToArray();
        }
        public static void CalculateCosts(MaterialEdge[] edges)
        {
            foreach (var edge in edges)
            {
                CalculateCosts(edge.From.IncomingEdges.ToArray());
                edge.To.Cost = Math.Round(edge.To.Operations.Sum(x => x.Cost) + edge.From.Cost, 2);
            }
        }
    }
}
