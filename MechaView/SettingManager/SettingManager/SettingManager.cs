using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SettingManagement
{
    public class SettingItem
    {
        public SettingItem(string name, Type type, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            Value = value;
        }
        public string Name { get; }
        public object Value { get; }
        public Type Type { get; }
    }

    public class Settings
    {
        public SerializedSettingItem[] Items { get; set; }
    }
    public class SerializedSettingItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public TypeCode Type { get; set; }
    }

    public static class SettingManager
    {
        public static void Save(string path, IEnumerable<SettingItem> settings)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            settings.ToDictionary(x => x.Name);

            var items = settings
                .Select(x => new SerializedSettingItem
                {
                    Name = x.Name,
                    Value = x.Value != null
                        ? (string) Convert.ChangeType(x.Value, TypeCode.String)
                        : null,
                    Type = Type.GetTypeCode(x.Type)
                }).ToArray();

            Save(path, new Settings { Items = items });
        }

        public static void Save(string path, object obj)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            using (var s = new StreamWriter(path))
            {
                var xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(s, obj);
            }
        }

        public static T Load<T>(string path) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (File.Exists(path))
            {
                using (var s = new StreamReader(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return xmlSerializer.Deserialize(s) as T;
                }
            }
            return new T();
        }

        public static Dictionary<string, object> Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                return new Dictionary<string, object>();

            var s = Load<Settings>(path);

            return s.Items.ToDictionary(
                x => x.Name,
                x => !string.IsNullOrWhiteSpace(x.Value)
                    ? Convert.ChangeType(x.Value, x.Type)
                    : null);
        }
    }
}
