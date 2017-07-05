using System;
using Newtonsoft.Json;

namespace BackgroundTasks.Models
{
    [Serializable]
    public sealed class Owner
    {

        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string OwnerIcon { get; set; }

        [JsonProperty(PropertyName = "coverFileUrl")]
        public string BackgroundForTile { get; set; }
        

        public Owner() { }

        public Owner(string contractorName, string ownerIcon, string backgroundForTile)
        {
            ContractorName = contractorName;
            OwnerIcon = ownerIcon;
            BackgroundForTile = backgroundForTile;
        }

    }
}
