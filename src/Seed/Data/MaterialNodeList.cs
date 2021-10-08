
using Seed.Generator;
using Seed.Generator.Material;
using System.Collections.Generic;

namespace Seed.Data
{
    public class MaterialNodeList: List<MaterialNode>
    {
        IWithNodeInUse _nodeCollector;
        public MaterialNodeList(IWithNodeInUse nodeCollector) : base() {
            _nodeCollector = nodeCollector;
        }
        private MaterialNode[] NodeStoreage { get; set; }
        public void SaveNodes()
        {
            NodeStoreage = this.ToArray();
        }
        public MaterialNode GetAndRemoveNodeAt(int index)
        {
            var node = this[index];
            this.RemoveAt(index);
            _nodeCollector.NodesInUse.Add(node);
            return node;
        }

        public MaterialNode GetNodeAt(int index)
        {
            return this[index];
        }
        public MaterialNode DequeueNode()
        {
            return GetAndRemoveNodeAt(0);
        }

        public int CountAll => NodeStoreage.Length;
        public MaterialNode GetNodeFromStorage(int index)
        {
            return NodeStoreage[index];
        }

        public MaterialNode[] AllNodes => NodeStoreage;
        
        /// <summary>
        /// Transfers Remaining nodes to NodesInUse list.
        /// </summary>
        /// <returns></returns>
        public void TransferNodesToNodesInUse()
        {
            _nodeCollector.NodesInUse.AddRange(this);
            this.Clear();
        }
    }
}
