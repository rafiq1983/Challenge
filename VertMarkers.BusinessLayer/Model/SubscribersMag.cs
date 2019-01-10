using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace VertMarket.Model
{
    public partial class SubscribersMag
    {
        [JsonProperty("subscribers")]
        public List<Guid> Subscribers { get; set; }

        public SubscribersMag()
        {
            Subscribers = new List<Guid>();
        }
    }

}
