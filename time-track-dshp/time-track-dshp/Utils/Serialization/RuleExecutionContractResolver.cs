using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace time_track_dshp.Utils.Serialization
{
    public class RuleExecutionContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);

            foreach (var prop in props)
                prop.PropertyName = prop.UnderlyingName;

            return props;
        }
    }
}
