﻿using Seed.Data;
using Seed.Distributions;
using Seed.Generator;
using Seed.Parameter;
using System;
using System.Collections.Generic;

namespace Seed.Test.MaterialStructure
{
    public class MaterialFixture : IDisposable
    {
        public Configuration Configuration { get; } = new();
        public RandomizerBase Randomizer { get; } = new RandomizerBase(29);
        public Materials Materials { get; private set; }
        public MaterialEdge[] Edges { get; private set; }
        public Queue<MaterialEdge> InitialEdges { get; private set; }
        public Action<MaterialEdge[], int> solveStructure { get; } = null;
        public MaterialFixture()
        {
            solveStructure = (edges, lvl) =>
            {
                foreach (var edge in edges)
                {
                    var intend = "".PadLeft(lvl, '-');
                    System.Diagnostics.Debug.WriteLine(intend + $"> {edge.From.InitialLevel} {edge.From.Id}");
                    solveStructure(edge.From.IncomingEdges.ToArray(), lvl + 2);
                }
            };
        }
        public void GenerateMaterials()
        {
            var matGenerator = new MaterialGenerator(Configuration, Randomizer);
            Materials = matGenerator.CreateMaterials();
            InitialEdges = matGenerator.CreateEdges();
            Edges = InitialEdges.ToArray();
            matGenerator.ConnectEdges();
        }

        public void Dispose() { }


    }
}
