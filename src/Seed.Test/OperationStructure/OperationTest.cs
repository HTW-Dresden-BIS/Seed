using MathNet.Numerics.Distributions;
using Seed.Distributions;
using Seed.Generator.Material;
using Seed.Generator.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Seed.Test.OperationStructure
{
    public class OperationTest : IClassFixture<OperationFixture>
    {
        private OperationFixture _operationFixture;
        private OperationGenerator _operationGenerator;
        private ITestOutputHelper _outputHelper;
        private List<double> _averageOg;
        private int _numberOfOperationsCreated;

        public OperationTest(OperationFixture operationFixture, ITestOutputHelper outputHelper)
        {
            _operationFixture = operationFixture;
            _outputHelper = outputHelper;
            _averageOg = new List<double>();
        }

        private void WriteIf(bool write, string line)
        {
            if (write)
                _outputHelper.WriteLine(line);
        }

        [Theory]
        [InlineData(100, 1, true)]
        public void GenerateOperations(int numberOfOperations, int seed, bool withOut)
        {

            var Randomizer = new RandomizerBase(seed);
            var randomizerCollection = RandomizerCollection.WithTransition(Randomizer)
                                                .WithOperationDuration(new RandomizerLogNormal(Randomizer.Next(int.MaxValue)))
                                                .WithOperationAmount(new RandomizerBinominial(Randomizer.Next(int.MaxValue)))
                                                .Build();
           //var material = new Materials();
            var operationDistributor = OperationDistributor.WithTransitionMatrix(_operationFixture.TransitionMatrix)
                                                            .WithRandomizerCollection(randomizerCollection)
                                                            .WithResourceConfig(_operationFixture.ResourceConfig)
                                                            .Build(); 

            var materials = _operationFixture.CreateMaterials(numberOfOperations);
            
            _operationGenerator = OperationGenerator.WithOperationDistributor(operationDistributor)
                                                    .WithMaterials(materials)
                                                    .Generate();
            
            var generatedOperationsMatrix = _operationGenerator.GetGeneratedTransitionMatrix();
            WriteIf(withOut, "Generated Transitions");
            WriteIf(withOut, generatedOperationsMatrix.GetMatrix.ToString());

            generatedOperationsMatrix.Normalize();
            WriteIf(withOut, "Normalized: ");
            WriteIf(withOut, generatedOperationsMatrix.GetMatrix.ToString());
          

            var og = generatedOperationsMatrix.GetOrganizationalDegree();
            _numberOfOperationsCreated = materials.Sum(x => x.Operations.Count());
            var operations =  materials.SelectMany(x => x.Operations);
            var group = operations.GroupBy(x => new { x.Node.Id }).Select(g => new { Key = g.Key, Count = g.Count() });
            var average = group.Average(x => x.Count);

            WriteIf(withOut, materials.Count() + " Materials created");
            WriteIf(withOut, _numberOfOperationsCreated + " Operations created");
            WriteIf(withOut, average + " Operations per Material " + group.Count());
            WriteIf(withOut, "Organizational Degree on Generated Matrix    : " + _operationFixture.TransitionMatrix.GetOrganizationalDegree());
            WriteIf(withOut, "Organizational Degree on Generated Materials : " + og);
            _averageOg.Add(og);
            
            Assert.InRange(og, _operationFixture.OrganizationalDegree - 0.1, _operationFixture.OrganizationalDegree + 0.1);
            Assert.True(materials.ToList().TrueForAll(x => x.Operations.TrueForAll(y => y.Duration.TotalSeconds != 0)));
        }



        //[Theory(Skip = "manual testing")]
        [Theory]
        [InlineData(1000, 100)]
        public void OrganizationalDegreeMassTest(int numberOfOperations, int rounds)
        {
            _averageOg.Clear();
            for (int i = 0; i < rounds; i++)
            {
                GenerateOperations(numberOfOperations, i , false);
            }
            
            _outputHelper.WriteLine($"based on {_numberOfOperationsCreated} operations.");
            _outputHelper.WriteLine($"OG Avg = {_averageOg.Average()}");
            _outputHelper.WriteLine($"OG Max = {_averageOg.Max()} ");
            _outputHelper.WriteLine($"OG Min = {_averageOg.Min()} ");
            _outputHelper.WriteLine($"OG Varianze = {MathNet.Numerics.Statistics.Statistics.Variance(_averageOg)} ");
            _outputHelper.WriteLine($"OG StdDev = {MathNet.Numerics.Statistics.Statistics.StandardDeviation(_averageOg)} ");
        }

        [Fact]
        public void ProbabilityMatrixFromTransitionMatrix()
        {
            var transitionMatrix = TransitionMatrixGenerator.WithConfiguration(_operationFixture.Configuration).Generate();
            var matrix = transitionMatrix.TransformToProbability();
            _outputHelper.WriteLine(matrix.ToString());
            Assert.Equal(4, matrix.Column(3).AsArray().Sum(x => x));
        }


        [Fact(Skip = "Manual only")]
        public void DistributionTest()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000; i++)
            {
                var val = LogNormal.WithMeanVariance(10, 2, rnd).Sample();
                System.Diagnostics.Debug.WriteLine(val + " | " +  Math.Round(val));
            }
        }
    }
}
