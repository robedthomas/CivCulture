using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CivCulture_Model.Models.MetaComponents.UserMutables
{
    public class CultureGroupData : MetaComponent
    {
        #region Events
        #endregion

        #region Fields
        public const string XML_ELEMENT_NAME = "CultureGroup";
        public const string XML_NAME_ATTRIBUTE_NAME = "name";
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public List<CultureData> Cultures { get; protected set; }
        #endregion

        #region Constructors
        public CultureGroupData(string name)
        {
            Name = name;
            Cultures = new List<CultureData>();
        }

        public CultureGroupData(XmlReader reader)
        {
            Cultures = new List<CultureData>();
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_ELEMENT_NAME:
                                Name = reader.GetAttribute(XML_NAME_ATTRIBUTE_NAME);
                                break;
                            case CultureData.XML_ELEMENT_NAME:
                                Cultures.Add(new CultureData(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing CultureGroupData");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_ELEMENT_NAME:
                                return;
                            case CultureData.XML_ELEMENT_NAME:
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing CultureGroupData");
                        }
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing CultureGroupData");
                }
            } while (reader.Read());
        }
        #endregion

        #region Methods
        #endregion
    }
}
