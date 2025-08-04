using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace time_track_dshp.Utils
{
    public class UnixTimestampConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((DateTime)value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            else if (reader.Value is DateTime)
                return reader.Value;
            else 
            {
                try
                {
                    return TimeFromUnixTimestampMilliseconds((long)reader.Value);
                }
                catch (Exception)
                {
                    //rarest of rare case we  may receive nanoseconds. Hence converting nano to seconds.
                    return TimeFromUnixTimestamp((long)reader.Value);
                }
            }
        }
        private static DateTime TimeFromUnixTimestampMilliseconds(long unixTimestamp)
        {
            var inSeconds = (long)(unixTimestamp * 0.001);
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(inSeconds);

            return dateTimeOffset.UtcDateTime;
        }
        private static DateTime TimeFromUnixTimestamp(long unixTimestamp)
        {
            var inSeconds = (long)(unixTimestamp * 0.000000001);
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(inSeconds);

            return dateTimeOffset.UtcDateTime;
        }
        public static long UnixTimestampFromDateTime(DateTime date)
        {
            long unixTimestamp = date.Ticks - _epoch.Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }
    }
}
