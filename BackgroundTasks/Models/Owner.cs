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

        public Owner() { }

        public Owner(string contractorName, string logoURL)
        {
            ContractorName = contractorName;
            LogoURL = logoURL;
        }

    }
}
