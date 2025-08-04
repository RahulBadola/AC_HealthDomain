using Avro;
using Avro.Generic;
using System;

namespace time_track_dshp.Extensions
{
    public static class AvroExtensions
    {
        public static T ParseConsumeResult<T>(this GenericRecord record, RecordSchema schema) where T : new()
        {
            var obj = new T();

            foreach (var field in schema.Fields)
            {
                if (record.TryGetValue(field.Name, out var value) && value != null)
                {
                    var propInfo = obj.GetType().GetProperty(field.Name);
                    if (propInfo != null)
                    {
                        if (propInfo.PropertyType.Name.ToLower() == "string")
                        {
                            propInfo.SetValue(obj, value);
                        }
                        else if (propInfo.PropertyType.Name.ToLower() == "guid")
                        {
                            propInfo.SetValue(obj, Guid.Parse(value.ToString()));
                        }
                        else if (propInfo.PropertyType.Name.ToLower() == "datetime")
                        {
                            propInfo.SetValue(obj, GetDefaultDateTime(value));
                        }
                        else if (propInfo.PropertyType.Name.ToLower() == "boolean")
                        {
                            propInfo.SetValue(obj, bool.Parse(value.ToString()));
                        }
                    }
                }
            }
            return obj;
        }

        private static DateTime GetDefaultDateTime(object value)
        {
            return string.IsNullOrWhiteSpace(value.ToString()) ? default(DateTime) : DateTime.Parse(value.ToString());
        }
    }
}
