﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CivCulture_Model.Models.MetaComponents.UserMutables
{
    public class NamesDatabase : MetaComponent
    {
        #region Events
        #endregion

        #region Fields
        public const string XML_DATABASE_ELEMENT_NAME = "NamesDatabase";
        public const string XML_GROUPS_ELEMENT_NAME = "CultureGroups";
        #endregion

        #region Properties
        public List<CultureGroupData> CultureGroups { get; protected set; }

        public HashSet<CultureData> UnusedCultures { get; protected set; }
        #endregion

        #region Constructors
        public NamesDatabase(string databaseFileLocation)
        {
            bool endReached = false;
            UnusedCultures = new HashSet<CultureData>();
            CultureGroups = new List<CultureGroupData>();
            using (XmlReader reader = XmlReader.Create(databaseFileLocation))
            {
                while (!endReached && reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XML_DATABASE_ELEMENT_NAME:
                                case XML_GROUPS_ELEMENT_NAME:
                                    break;
                                case CultureGroupData.XML_ELEMENT_NAME:
                                    CultureGroups.Add(new CultureGroupData(reader));
                                    break;
                                default:
                                    throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing NamesDatabase");
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case XML_DATABASE_ELEMENT_NAME:
                                    endReached = true;
                                    break;
                                case XML_GROUPS_ELEMENT_NAME:
                                    break;
                                default:
                                    throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing NamesDatabase");
                            }
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.Whitespace:
                            break;
                        default:
                            throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing NamesDatabase");
                    }
                }
            }
            foreach (CultureGroupData group in CultureGroups)
            {
                foreach (CultureData culture in group.Cultures)
                {
                    UnusedCultures.Add(culture);
                }
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
