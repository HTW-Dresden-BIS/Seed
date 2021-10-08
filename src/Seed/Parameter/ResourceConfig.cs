using Seed.Parameter.Operation;
using System;
using System.Collections.Generic;

namespace Seed.Parameter
{
    public class ResourceConfig : IParameter
    {
        public ResourceConfig() 
        {
            DefaultOperationDurationDistributionParameter = new();
            DefaultOperationAmountDistributionParameter = new();
            DefaultSetupDurationDistributionParameter = new();
            ResourceGroupList = new();
        }
        public List<ResourceGroup> ResourceGroupList { get; set; }
        public DistributionParameter DefaultOperationDurationDistributionParameter { get; set; }
        public DistributionParameter DefaultOperationAmountDistributionParameter { get; set; }
        public DistributionParameter DefaultSetupDurationDistributionParameter { get; set; }
        public double DefaultCostRateIdleTime { get; set; }
        public double DefaultCostRateProcessing { get; set; }
        public double DefaultCostRateSetup { get; set; }
        public List<ResourceTool> GetToolsFor(int resourceGroupIndex)
        {
            return this.ResourceGroupList[resourceGroupIndex].Tools;
        }

        private bool CheckToolOperationDurationParameterValue(int resourceIndex, int toolIndex)
        {
            return this.ResourceGroupList[resourceIndex].Tools[toolIndex].OperationDurationDistributionParameter.Mean != 0;
        }

        private bool CheckResourceOperationDurationParameterValue(int resourceIndex)
        {
            return this.ResourceGroupList[resourceIndex].OperationDurationDistributionParameter.Mean != 0;
        }

        private bool CheckToolSetupParameterValue(int resourceIndex, int toolIndex)
        {
            return this.ResourceGroupList[resourceIndex].Tools[toolIndex].SetupDurationDistributionParameter.Mean != 0;
        }

        private bool CheckResourceSetupParameterValue(int resourceIndex)
        {
            return this.ResourceGroupList[resourceIndex].SetupDurationDistributionParameter.Mean != 0;
        }
        /// <summary>
        /// Returns the Distribution from Config, with fallback > Tools > ResourceGroup > 10
        /// </summary>
        /// <param name="resourceIndex"></param>
        /// <param name="toolIndex"></param>
        /// <returns></returns>
        public TimeSpan GetMeanOperationDurationFor(int resourceIndex, int toolIndex)
        {
            return TimeSpan.FromSeconds(CheckToolOperationDurationParameterValue(resourceIndex, toolIndex) ? 
                                         this.ResourceGroupList[resourceIndex].Tools[toolIndex].OperationDurationDistributionParameter.Mean // Per Tool
                                      : CheckResourceOperationDurationParameterValue(resourceIndex) ? 
                                         this.ResourceGroupList[resourceIndex].OperationDurationDistributionParameter.Mean
                                      : DefaultOperationDurationDistributionParameter.Mean != 0 ?
                                         DefaultOperationDurationDistributionParameter.Mean
                                      : 10);         // Resource Default;
        }

        /// <summary>
        /// Returns the Distribution from Config, with fallback > Tools > ResourceGroup > 0
        /// </summary>
        /// <param name="resourceIndex"></param>
        /// <param name="toolIndex"></param>
        /// <returns></returns>
        public TimeSpan GetMeanSetupDurationFor(int resourceIndex, int toolIndex)
        {
            return TimeSpan.FromSeconds(CheckToolSetupParameterValue(resourceIndex, toolIndex) ?
                                         this.ResourceGroupList[resourceIndex].Tools[toolIndex].SetupDurationDistributionParameter.Mean // Per Tool
                                      : CheckResourceSetupParameterValue(resourceIndex) ?
                                         this.ResourceGroupList[resourceIndex].SetupDurationDistributionParameter.Mean
                                      : DefaultSetupDurationDistributionParameter.Mean != 0 ?
                                         DefaultSetupDurationDistributionParameter.Mean
                                      : 0);         // Resource Default;
        }

        public double GetCostRateIdleTimeFor(int resourceIndex)
        {
            return this.ResourceGroupList[resourceIndex].CostRateIdleTime != 0.0 ? 
                   /* then */ this.ResourceGroupList[resourceIndex].CostRateIdleTime 
                   /* else */ : this.DefaultCostRateIdleTime;
        }

        public double GetCostRateSetupFor(int resourceIndex)
        {
            return this.ResourceGroupList[resourceIndex].CostRateSetup != 0.0 ?
                   /* then */ this.ResourceGroupList[resourceIndex].CostRateSetup
                   /* else */ : this.DefaultCostRateSetup;
        }

        public double GetCostRateProcessingFor(int resourceIndex)
        {
            return this.ResourceGroupList[resourceIndex].CostRateProcessing != 0.0 ?
                   /* then */ this.ResourceGroupList[resourceIndex].CostRateProcessing
                   /* else */ : this.DefaultCostRateProcessing;
        }

        public double GetVarianceOperationDurationFor(int resourceIndex, int toolIndex)
        {
            return CheckToolOperationDurationParameterValue(resourceIndex, toolIndex) ?
                this.ResourceGroupList[resourceIndex].Tools[toolIndex].OperationDurationDistributionParameter.Variance // Per Tool
                : CheckResourceOperationDurationParameterValue(resourceIndex) ? 
                this.ResourceGroupList[resourceIndex].OperationDurationDistributionParameter.Variance
                : DefaultOperationDurationDistributionParameter.Variance;    // No further check needed as int is 0 anyway
        }

        public double GetVarianceSetupDurationFor(int resourceIndex, int toolIndex)
        {
            return CheckToolSetupParameterValue(resourceIndex, toolIndex) ?
                this.ResourceGroupList[resourceIndex].Tools[toolIndex].SetupDurationDistributionParameter.Variance // Per Tool
                : CheckResourceSetupParameterValue(resourceIndex) ?
                this.ResourceGroupList[resourceIndex].SetupDurationDistributionParameter.Variance
                : DefaultSetupDurationDistributionParameter.Variance;    // No further check needed as int is 0 anyway
        }

        public ResourceConfig WithDefaultOperationsDurationVariance(double varianceInPercent)
        {
            this.DefaultOperationDurationDistributionParameter.Variance = varianceInPercent;
            return this;
        }
        public ResourceConfig WithDefaultOperationsDurationMean(TimeSpan mean)
        {
            this.DefaultOperationDurationDistributionParameter.Mean = mean.TotalSeconds;
            return this;
        }
        public ResourceConfig WithDefaultSetupDurationVariance(double varianceInPercent)
        {
            this.DefaultSetupDurationDistributionParameter.Variance = varianceInPercent;
            return this;
        }
        public ResourceConfig WithDefaultSetupDurationMean(TimeSpan mean)
        {
            this.DefaultSetupDurationDistributionParameter.Mean = mean.TotalSeconds;
            return this;
        }
        public ResourceConfig WithDefaultOperationsAmountVariance(double varianceInPercent)
        {
            this.DefaultOperationAmountDistributionParameter.Variance = varianceInPercent;
            return this;
        }
        public ResourceConfig WithDefaultOperationsAmountMean(int quantity)
        {
            this.DefaultOperationAmountDistributionParameter.Mean = quantity;
            return this;
        }
        public ResourceConfig WithResourceGroup(List<ResourceGroup> groups)
        {
            this.ResourceGroupList.AddRange(groups); 
            return this;
        }
    }
}
