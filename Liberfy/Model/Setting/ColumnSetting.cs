using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    [DataContract]
    internal class ColumnSetting : ICloneable
    {
        [Key("service")]
        [DataMember(Name = "service")]
        public ServiceType Service { get; set; }

        [Key("user_id")]
        [DataMember(Name = "user_id")]
        public long UserId { get; set; }

        [Key("type")]
        [DataMember(Name = "type")]
        public ColumnType Type { get; set; }

        [Key("options")]
        [DataMember(Name = "options", EmitDefaultValue = false)]
        public IDictionary<string, IConvertible> Options { get; set; }

        public ColumnSetting Clone() => new ColumnSetting
        {
            Service = this.Service,
            UserId = this.UserId,
            Type = this.Type,
            Options = this.Options == null ? null : new Dictionary<string, IConvertible>(this.Options),
        };

        object ICloneable.Clone() => this.Clone();

        public static ColumnSetting FromDefault(ColumnSetting item) => item.Clone();

        public static ColumnSetting GetDefault(ColumnType type)
        {
            return new ColumnSetting
            {
                Type = type,
            };
        }

        public T GetValue<T>(string propertyName)
        {
            var value = default(IConvertible);

            if (this.Options?.TryGetValue(propertyName, out value) ?? false)
                if (value is T tValue)
                    return tValue;

            return default(T);
        }

        public void SetValue(string propertyName, IConvertible value)
        {
            if (this.Options == null)
                this.Options = new Dictionary<string, IConvertible>();

            if (this.Options.ContainsKey(propertyName))
                this.Options.Add(propertyName, value);
            else
                this.Options[propertyName] = value;
        }
    }
}
