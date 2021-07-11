using System;
using System.Collections.Generic;
using System.Text;

namespace BeautySearch
{

    class FeatureControl
    {
        private Dictionary<string, string> features = new Dictionary<string, string>();

        public void Enable(string feature)
        {
            SetRaw(feature, "true");
        }

        public bool IsEnabled(string feature)
        {
            return Get(feature) != null;
        }

        public void Exclude(string feature)
        {
            features.Remove(feature);
        }

        public string Get(string feature)
        {
            return features.ContainsKey(feature) ? features[feature] : null;
        }

        public void Set(string feature, string value)
        {
            SetRaw(feature, $"'{value}'");
        }

        public void SetRaw(string feature, string value)
        {
            if (features == null) throw new InvalidOperationException("FeatureControl is not writable");
            if (Get(feature) != null) Exclude(feature);
            features.Add(feature, value);
        }

        public string Build()
        {
            var builder = new StringBuilder("const SETTINGS = {");
            foreach (KeyValuePair<string, string> pair in features)
            {
                builder.Append($"{pair.Key}:{pair.Value},");
            }
            builder.Append("};");

            features = null;
            return builder.ToString();
        }
    }
}
