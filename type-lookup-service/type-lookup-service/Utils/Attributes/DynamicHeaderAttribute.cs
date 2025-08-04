using System;

namespace type_lookup_service.Utils.Attributes
{
    public class DynamicHeaderAttribute : Attribute
    {
        public string Name { get; set; }

        public bool VisibleOutSideK8s { get; set; }

        public bool Required { get; set; }

        public DynamicHeaderAttribute()
        {
            Name = null;
            VisibleOutSideK8s = true;
            Required = false;
        }

        public DynamicHeaderAttribute(string name, bool visibleOutSideK8s, bool required)
        {
            Name = name;
            VisibleOutSideK8s = visibleOutSideK8s;
            Required = required;
        }
    }
}