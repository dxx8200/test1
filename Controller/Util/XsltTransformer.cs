using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Controller.Util
{
    class XsltTransformer
    {
        public static string Transform(string xsltpath, string xmlpath, IDictionary<string, object> param = null)
        {
             MemoryStream memStream = new MemoryStream();
             using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.UTF8))
             {
                 XsltArgumentList xslArgs = new XsltArgumentList();
                 XslCompiledTransform xslt = new XslCompiledTransform();
                 XPathDocument xpath = new XPathDocument(xmlpath);
                 if(param != null)
                 {
                     foreach (KeyValuePair<string, object> item in param)
                     {
                         xslArgs.AddParam(item.Key, "", item.Value);
                     }
                 }

                 xslt.Load(xsltpath);
                 xslt.Transform(xpath, xslArgs, textWriter, null);
                 memStream = textWriter.BaseStream as MemoryStream;
             }
             if (memStream != null)
                 return Encoding.UTF8.GetString(memStream.ToArray());
             else
                 return null;
            
        }

        public static void TransformFile(string xsltpath, string xmlpath, string outputpath, bool indent = false,
            IDictionary<string, object> param = null)
        {
            FileStream fileStream = new FileStream(outputpath, FileMode.Create, FileAccess.Write);
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = indent;
            using (XmlTextWriter textWriter = new XmlTextWriter(fileStream, Encoding.Default))
            {
                XsltArgumentList xslArgs = new XsltArgumentList();
                XslCompiledTransform xslt = new XslCompiledTransform();
                XPathDocument xpath = new XPathDocument(xmlpath);
                if (param != null)
                {
                    foreach (KeyValuePair<string, object> item in param)
                    {
                        xslArgs.AddParam(item.Key, "", item.Value);
                    }
                }

                xslt.Load(xsltpath, new XsltSettings(true, true), new XmlUrlResolver());
                xslt.Transform(xpath, xslArgs, textWriter, new XmlUrlResolver());
            }

        }
    }
}
