using System;
using System.Text.Json.Serialization;

namespace api.models
{
    /// <summary>
    /// means record of run
    /// </summary>
    public class Record
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public float AmountKmRun { get; set; }
        public bool IsPublic { get;set;}
        public float AvgSpeed { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}