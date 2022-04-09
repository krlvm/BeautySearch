using System;
using System.Collections.Generic;
using System.Text;

namespace BeautySearch
{
    class FeatureControl
    {
        private Dictionary<string, object> features = new Dictionary<string, object>();

        public void Enable(string feature)
        {
            Set(feature, true);
        }

        public bool IsEnabled(string feature)
        {
            return Get(feature) != null;
        }

        public void Exclude(string feature)
        {
            if (features.ContainsKey(feature))
            {
                features.Remove(feature);
            }
        }

        public string Get(string feature)
        {
            return features.ContainsKey(feature) ? features[feature].ToString() : null;
        }

        public void Set(string feature, object value)
        {
            if (features == null) throw new InvalidOperationException("FeatureControl is not writable");
            if (Get(feature) != null) Exclude(feature);
            features.Add(feature, value);
        }

        public string Build()
        {
            string code = "const SETTINGS = " + ToJson() + ";";
            features = null;
            return code;
        }

        public string ToJson()
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in features)
            {
                object val = pair.Value is bool ? pair.Value.ToString().ToLower() : (pair.Value is string ? $"'{pair.Value}'" : pair.Value);
                builder.Append($"{pair.Key}:{val},");
            }
            var body = builder.ToString();
            body = body.Substring(0, body.Length - 1);
            return "{" + body + "}";
        }

        public static FeatureControl Parse(string json)
        {
            Console.WriteLine(json);
            json = json.Substring(1, json.Length - 2);
            Console.WriteLine(json);
            var features = new FeatureControl();
            foreach (var entry in json.Split(','))
            {
                var arr = entry.Split(':');
                var key = arr[0];
                var val = arr[1];
                object targetVal;
                if (val.StartsWith("'"))
                {
                    targetVal = val.Substring(1, val.Length - 2);
                }
                else if ("true".Equals(val) || "false".Equals(val))
                {
                    targetVal = bool.Parse(val);
                }
                else
                {
                    if (val.Contains("."))
                    {
                        targetVal = double.Parse(val);
                    }
                    else
                    {
                        targetVal = int.Parse(val);
                    }
                }
                Console.WriteLine(key + "/" + targetVal);
                features.Set(key, targetVal);
            }
            return features;
        }
    }
}
