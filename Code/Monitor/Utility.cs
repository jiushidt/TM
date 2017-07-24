using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Monitor
{
    public static class Utility
    {
        public static String ObjectToString<T>(T o)
        {
            if (o == null)
            {
                return null;
            }
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Seek(0, SeekOrigin.Begin);
                ser.Serialize(ms, o);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static T StringToObject<T>(String s)
            where T : class
        {
            if (s == null || s.Trim() == "")
            {
                return null;
            }
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            byte[] array = Encoding.UTF8.GetBytes(s);
            using (MemoryStream ms = new MemoryStream(array))
            {
                ms.Seek(0, SeekOrigin.Begin);
                T desprop = (T)ser.Deserialize(ms);
                return desprop;
            }

        }
    }
}
