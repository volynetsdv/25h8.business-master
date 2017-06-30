using Newtonsoft.Json;
using System;

namespace BackgroundTasks
{
    [Serializable]
    public sealed class Owner
    {

        [JsonProperty(PropertyName = "contractorName")]
        public string ContractorName { get; set; }

        [JsonProperty(PropertyName = "contractorAvatarUrl")]
        public string LogoURL { get; set; }

        [JsonProperty(PropertyName = "coverFileUrl")]
        public string BackgroundForTile { get; set; }
        

        public Owner() { }

        public Owner(string contractorName, string logoURL, string backgroundForTile)
        {
            ContractorName = contractorName;
            LogoURL = logoURL;
            BackgroundForTile = backgroundForTile;
        }

    }
}
