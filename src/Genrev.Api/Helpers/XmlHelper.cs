using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Xml.Serialization;

namespace Genrev.Api
{
    public class XmlHelper
    {



        public static T FromXml<T>(string xml) {

            T returnedXmlClass = default(T);

            using (TextReader reader = new StringReader(xml)) {
                returnedXmlClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }

            return returnedXmlClass;
        }

    }
}