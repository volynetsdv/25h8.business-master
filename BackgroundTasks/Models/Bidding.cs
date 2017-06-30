using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Bidding
    {
        [JsonProperty(PropertyName = "entityType")]
        public string EntityType { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "proc")]
        public string Process { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public Owner Owner { get; set; }

        public Bidding() { }

        public Bidding(string entityType, string title, string process, string contractorName, string logoURL, int id, string state, Owner owner)
        {
            EntityType = entityType;
            Title = title;
            Process = process;
            Id = id;
            State = state;
            Owner = owner;

        }

    }
}
