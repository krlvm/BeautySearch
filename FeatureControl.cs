using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySearch
{

    class FeatureControl
    {
        private StringBuilder sb = new StringBuilder("const SETTINGS = {");
        private bool writeable = true;

        public void Enable(string feature)
        {
            if (!writeable)
            {
                throw new InvalidOperationException("FeatureControl is not writeable");
            }
            sb.Append(feature + ":true,");
        }

        public bool IsEnabled(string feature)
        {
            return sb.ToString().Contains(feature);
        }

        public string Build()
        {
            writeable = false;
            sb.Append("};");
            return sb.ToString();
        }

        override public string ToString()
        {
            return Build();
        }
    }
}
