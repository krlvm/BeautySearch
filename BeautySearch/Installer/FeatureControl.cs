using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            return JsonConvert.SerializeObject(features, Formatting.None);
        }

        public static FeatureControl Parse(string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var features = new FeatureControl();
            foreach (var entry in data)
            {
                features.Set(entry.Key, entry.Value.ToString());
            }
            return features;
        }
    }
}
