﻿using Seed.Generator;
using Seed.Parameter;
using Seed.Parameter.TransitionMatrix;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Seed.Test.MaterialStructure
{
    public class DivergingMaterials : IClassFixture<MaterialFixture>
    {
        private MaterialFixture _materialFixture;
        private TransitionMatrixParameter tmp = new () { Lambda = 2, OrganizationalDegree = 0.15 };
        private StructureParameter msp = new () { ComplexityRatio = 2, ReuseRatio = 4, NumberOfSalesMaterials = 8, VerticalIntegration = 4 };
        private ITestOutputHelper _out;

        public DivergingMaterials(MaterialFixture materialFixture, ITestOutputHelper outputHelper)
        {
            _out = outputHelper;
            _materialFixture = materialFixture;
            _materialFixture.MaterialConfiguration = new MaterialConfig { StructureParameter = msp, TransitionMatrixParameter = tmp };
            _materialFixture.GenerateMaterials();
        }

        [Fact]
        public void NumberOfMaterialsPerLevel()
        {
            int maxLevel = _materialFixture.Materials.Max(x => x.Level);
            double salesMaterials = _materialFixture.Materials.NodesInUse.Where(x => x.InitialLevel == 0).Count();
            double purchaseMaterials = _materialFixture.Materials.NodesInUse.Where(x => x.InitialLevel == maxLevel - 1).Count();
            foreach (var materialHirarchie in _materialFixture.Materials)
            {
                var expected = 0.0;
                if (materialHirarchie.Level == 1)
                    expected = purchaseMaterials / (Math.Pow((double)msp.ComplexityRatio / msp.ReuseRatio, msp.VerticalIntegration - 1));
                else
                    expected = salesMaterials * (Math.Pow((double)msp.ComplexityRatio / msp.ReuseRatio, materialHirarchie.Level - 1));

                Assert.Equal(Math.Round(expected, 0), _materialFixture.Materials.NodesInUse.Count(y => y.InitialLevel + 1 == materialHirarchie.Level));
            }
        }

        [Fact]
        public void NumberOfLevels()
        {
            Assert.Equal(4, _materialFixture.Materials.Count());
        }

        [Fact]
        public void NumberOfEdgesPerLevel()
        {
            Assert.Equal(28, _materialFixture.Edges.Count());
        }
        [Fact]
        public void NoEdgesNoNodesLeftBehind()
        {
            Assert.Equal(_materialFixture.InitialEdges, _materialFixture.Materials.Edges.Count());
        }
        [Fact(Skip = "Manual")]
        public void ShowNodesStructures()
        {

            var sales = _materialFixture.Materials.NodesSalesOnly();
            var purchase = _materialFixture.Materials.NodesPurchaseOnly();
            _out.WriteLine($"> {sales.Count()} Sales Materials");
            _out.WriteLine($"> {purchase.Count()} Purchase Materials ");

            foreach (var node in sales)
            {
                _out.WriteLine($"> {node.InitialLevel} {node.Id}");
                _materialFixture.solveStructure(node.IncomingEdges.ToArray(), 2);
            }
        }

        [Fact]
        public void DegreeOfCoplexityAndMultipleUse()
        {
            var allMats = _materialFixture.Materials.ToNodeArray;
            var totalMats = allMats.Count();
            var numberOfOutgoingEdgesOnMatsWithSuccessor = allMats.Sum(x => x.OutgoingEdges.Count);
            var matsSalesOnly = _materialFixture.Materials.NodesSalesOnly().Count();
            var multipleUse = (double)numberOfOutgoingEdgesOnMatsWithSuccessor / (totalMats - matsSalesOnly);
            _out.WriteLine($" Multiple Use : {multipleUse}");
            Assert.Equal(msp.ReuseRatio, multipleUse);

            var matsWithPredecessor = allMats.Sum(x => x.IncomingEdges.Count);
            var matsPurchaseOnly = _materialFixture.Materials.NodesPurchaseOnly().Count();
            var degreeOfComplexity = (double)matsWithPredecessor / (totalMats - matsPurchaseOnly);
            _out.WriteLine($" Complexity : {degreeOfComplexity}");
            Assert.Equal(msp.ComplexityRatio, degreeOfComplexity);
        }
    }
}
