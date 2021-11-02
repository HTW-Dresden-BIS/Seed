using System;
using System.Text.Json.Serialization;

namespace Seed.Data
{
    public class MaterialNodeOperation
    {
        [JsonIgnore]
        private static int IdCounter = 0;
        public MaterialNodeOperation()
        {
            Id = IdCounter++;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public TimeSpan Duration { get; set;}
        public double DurationInSeconds { get => Duration.TotalSeconds; }
        public double Cost { get; set; }
        public int SequenceNumber { get; set; }
        public int TargetResourceIdent { get; set;}
        public int TargetToolIdent { get; set; }
        [JsonIgnore]
        public MaterialNode Node { get; set; }
        public int NodeId => Node.Id;
    }
}
