using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [JsonFormatter(typeof(PointFormatter))]
    public struct Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        internal class PointFormatter : IJsonFormatter<Point>
        {
            public Point Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                double[] inputValues =
                    formatterResolver
                    .GetFormatter<double[]>()
                    .Deserialize(ref reader, formatterResolver);

                if (inputValues?.Length == 2)
                {
                    new Point
                    {
                        Latitude = inputValues[0],
                        Longitude = inputValues[1],
                    };
                }

                throw new NotImplementedException();
            }

            public void Serialize(ref JsonWriter writer, Point value, IJsonFormatterResolver formatterResolver)
            {
                double[] outputValues = new []
                {
                    value.Latitude,
                    value.Longitude,
                };

                formatterResolver
                    .GetFormatter<double[]>()
                    .Serialize(ref writer, outputValues, formatterResolver);
            }
        }
    }
}
