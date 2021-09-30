using System;
using System.Collections.Generic;

namespace Seed.Parameter.Operation
{
    public class ResourceGroup
    {
        public ResourceGroup(string name) 
        {
            Name = name;
            OperationDurationDistributionParameter = new();
            SetupDurationDistributionParameter = new();
        }

        public string Name { get; set; }
        public long ResourceQuantity { get; set; }
        public double CostRateIdleTime { get; set;}
        public double CostRateProcessing { get; set; }
        public double CostRateSetup { get; set; }
        public DistributionParameter OperationDurationDistributionParameter { get; set; }
        public DistributionParameter SetupDurationDistributionParameter { get; set; }
        public List<ResourceTool> Tools { get; set; }

        public ResourceGroup WithDefaultOperationDurationMean(TimeSpan timeSpan)
        {
            this.OperationDurationDistributionParameter.Mean = timeSpan.TotalSeconds;
            return this;
        }
        public ResourceGroup WithDefaultOperationDurationVariance(double varianceInPercent)
        {
            this.OperationDurationDistributionParameter.Variance = varianceInPercent;
            return this;
        }
        public ResourceGroup WithDefaultSetupDurationMean(TimeSpan timeSpan)
        {
            this.SetupDurationDistributionParameter.Mean = timeSpan.TotalSeconds;
            return this;
        }
        public ResourceGroup WithDefaultSetupDurationVariance(double varianceInPercent)
        {
            this.SetupDurationDistributionParameter.Variance = varianceInPercent;
            return this;
        }
        public ResourceGroup WithTools(List<ResourceTool> tools)
        {
            this.Tools = tools;
            return this;
        }
        public ResourceGroup WithResourceuQuantity(int quantity)
        {
            this.ResourceQuantity = quantity;
            return this;
        }
    }
}
