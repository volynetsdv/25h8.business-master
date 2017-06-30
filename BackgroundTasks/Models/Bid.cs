using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Bid
    {
        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "proc")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public Owner Owner { get; set; }

        public Bid() { }

        public Bid(string entityType, string title, string process, int id, Owner owner)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            Id = id;
            Owner = owner;
        }
    }
}
