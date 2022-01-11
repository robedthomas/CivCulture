using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CivCulture_Model.Models.MetaComponents.UserMutables
{
    public class CultureData : MetaComponent
    {
        #region Events
        #endregion

        #region Fields
        public const string XML_ELEMENT_NAME = "Culture";
        public const string XML_NAME_ATTRIBUTE_NAME = "name";
        public const string XML_CITY_NAME_GROUP_ELEMENT_NAME = "CityNames";
        public const string XML_CITY_NAME_ELEMENT_NAME = "CityName";
        public const string XML_CITY_NAME_ATTRIBUTE_NAME = "name";
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public List<string> CityNames { get; protected set; }
        #endregion

        #region Constructors
        public CultureData(string name, IEnumerable<string> cityNames = null)
        {
            Name = name;
            if (cityNames != null)
            {
                CityNames = new List<string>(cityNames);
            }
            else
            {
                CityNames = new List<string>();
            }
        }

        public CultureData(XmlReader reader)
        {
            CityNames = new List<string>();
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
                            case XML_CITY_NAME_GROUP_ELEMENT_NAME:
                                CityNames.AddRange(ReadCityNamesFromXml(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing CultureData");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_ELEMENT_NAME:
                                return;
                            case XML_CITY_NAME_GROUP_ELEMENT_NAME:
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing CultureData");
                        }
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing CultureData");
                }
            } while (reader.Read());
        }
        #endregion

        #region Methods
        private static List<string> ReadCityNamesFromXml(XmlReader reader)
        {
            bool reachedEnd = false;
            List<string> cityNames = new List<string>();
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_CITY_NAME_ELEMENT_NAME:
                                cityNames.Add(reader.GetAttribute(XML_CITY_NAME_ATTRIBUTE_NAME));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing CultureData");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_CITY_NAME_GROUP_ELEMENT_NAME:
                                reachedEnd = true;
                                break;
                            case XML_CITY_NAME_ELEMENT_NAME:
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing CultureData");
                        }
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing CultureData");
                }
            } while (!reachedEnd && reader.Read());
            return cityNames;
        }
        #endregion
    }
}
