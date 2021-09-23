using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tracers;

namespace Savers
{
    public class XmlSaver : ISaver
    {
        private void Save(XmlDocument document, XmlElement parent, IEnumerable<IMethod> methods)
        {
            foreach (IMethod method in methods)
            {
                XmlElement element = document.CreateElement("method");

                XmlAttribute classAttr = document.CreateAttribute("class");
                XmlText classText = document.CreateTextNode(method.ClassName);
                classAttr.AppendChild(classText);
                element.Attributes.Append(classAttr);

                XmlAttribute nameAttr = document.CreateAttribute("name");
                XmlText nameText = document.CreateTextNode(method.MethodName);
                nameAttr.AppendChild(nameText);
                element.Attributes.Append(nameAttr);

                XmlAttribute deltaTimeAttr = document.CreateAttribute("time");
                XmlText deltaTimeText = document.CreateTextNode(method.DeltaTimeString);
                deltaTimeAttr.AppendChild(deltaTimeText);
                element.Attributes.Append(deltaTimeAttr);

                parent.AppendChild(element);

                Save(document, element, method.Methods);
            }
        }
        public void Save(Stream output, IEnumerable<INode> traceResult)
        {
            using XmlWriter writer = XmlWriter.Create(output);

            XmlDocument document = new XmlDocument();
            XmlElement root = document.DocumentElement;
            if (root == null)
            {
                root = document.CreateElement("TraceResult");
                document.AppendChild(root);
            }


            foreach (INode node in traceResult)
            {
                XmlElement threadElement = document.CreateElement("thread");

                XmlAttribute nameAttr = document.CreateAttribute("name");
                XmlText nameText = document.CreateTextNode(node.ThreadName);
                nameAttr.AppendChild(nameText);
                threadElement.Attributes.Append(nameAttr);


                XmlAttribute deltaTimeAttr = document.CreateAttribute("time");
                XmlText deltaTimeText = document.CreateTextNode(node.DeltaTimeString);
                deltaTimeAttr.AppendChild(deltaTimeText);
                threadElement.Attributes.Append(deltaTimeAttr);

                root.AppendChild(threadElement);

                Save(document, threadElement, node.Methods);
            }

            document.WriteTo(writer);
        }
    }
}
