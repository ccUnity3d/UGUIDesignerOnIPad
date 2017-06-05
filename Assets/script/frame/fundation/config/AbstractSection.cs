using System.Xml;

namespace foundation
{
    public class AbstractSection : IXMLBinder
    {
        virtual public string value
        {
            get
            {
               return _value;
            }
        }
        protected string _name;
        protected string _value;
        protected string _hash;

        protected ConfigurationUtil _conf;
        public AbstractSection()
        {
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public static string getSpcode(string value)
        {
            string sp = "?";
            if (value.IndexOf(sp) != -1)
            {
                sp = "&";
            }
            return sp;
        }


        public ConfigurationUtil config
        {
            set
            {
                this._conf = value;
            }
        }

        virtual public void bindXML(XmlNode xml)
        {
            _name = getAttributeValue(xml.Attributes, "name");

            _value = getAttributeValue(xml.Attributes, "value");

            _hash = getAttributeValue(xml.Attributes, "hash");
        }



        internal string getAttributeValue(XmlAttributeCollection attributes,string key)
        {
            XmlAttribute attribute=attributes[key];

            if (attribute != null)
            {
                return attribute.InnerText.Trim();
            }
            return "";
        }
    }
}
