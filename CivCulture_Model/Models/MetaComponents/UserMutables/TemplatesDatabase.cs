using CivCulture_Model.Models.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CivCulture_Model.Models.MetaComponents.UserMutables
{
    struct PopTemplateData
    {
        public string name;
        public string displayName;
        public decimal progressFromSatisfactionRatio;
        public Dictionary<string, decimal> necessities;
        public Dictionary<string, decimal> comforts;
        public Dictionary<string, decimal> luxuries;
    }

    struct JobTemplateData
    {
        public string name;
        public string displayName;
        public decimal priority;
        public decimal basePay;
        public string popTemplateName;
        public Dictionary<string, decimal> inputs;
        public Dictionary<string, decimal> outputs;
    }

    struct BuildingTemplateData
    {
        public string name;
        public string displayName;
        public bool isSpaceUnique;
        public Dictionary<string, decimal> constructionCosts;
        public Dictionary<string, decimal> resourceOutputs;
        public Dictionary<string, decimal> jobOutputs;
        public List<string> requisitBuildingSlots;
    }

    struct BuildingSlotTemplateData
    {
        public string name;
        public string displayName;
        public Dictionary<string, Tuple<decimal, string>> likelihoodPerTerrainType;
        public Dictionary<string, decimal> childJobs;
        public Dictionary<string, decimal> resourcesUponRemoval;
    }

    struct ResourceQuantityData
    {
        public string resourceName;
        public decimal quantity;
    }

    struct JobQuantityData
    {
        public string jobTemplateName;
        public decimal quantity;
    }

    public class TemplatesDatabase : MetaComponent
    {
        #region Fields
        public const string XML_DATABASE_ELEMENT_NAME = "Templates";

        public const string XML_POP_TEMPLATES_ELEMENT_NAME = "PopTemplates";
        public const string XML_SINGULAR_POP_TEMPLATE_ELEMENT_NAME = "PopTemplate";

        public const string XML_JOB_TEMPLATES_ELEMENT_NAME = "JobTemplates";
        public const string XML_SINGULAR_JOB_TEMPLATE_ELEMENT_NAME = "JobTemplate";

        public const string XML_BUILDING_TEMPLATES_ELEMENT_NAME = "BuildingTemplates";
        public const string XML_SINGULAR_BUILDING_TEMPLATE_ELEMENT_NAME = "BuildingTemplate";

        public const string XML_BUILDING_SLOT_TEMPLATES_ELEMENT_NAME = "BuildingSlotTemplates";
        public const string XML_SINGULAR_BUILDING_SLOT_TEMPLATE_ELEMENT_NAME = "BuildingSlotTemplate";

        public const string XML_TECHNOLOGY_TEMPLATES_ELEMENT_NAME = "TechnologyTemplates";
        public const string XML_SINGULAR_TECHNOLOGY_TEMPLATE_ELEMENT_NAME = "TechnologyTemplate";
        #endregion

        #region Properties
        public Dictionary<string, PopTemplate> PopTemplatesByName { get; } = new Dictionary<string, PopTemplate>();

        public Dictionary<string, JobTemplate> JobTemplatesByName { get; } = new Dictionary<string, JobTemplate>();

        public Dictionary<string, BuildingTemplate> BuildingTemplatesByName { get; } = new Dictionary<string, BuildingTemplate>();

        public Dictionary<string, BuildingSlotTemplate> BuildingSlotTemplatesByName { get; } = new Dictionary<string, BuildingSlotTemplate>();

        public Dictionary<string, TechnologyTemplate> TechnologyTemplatesByName { get; } = new Dictionary<string, TechnologyTemplate>();

        protected Dictionary<Terrain, Dictionary<BuildingSlotTemplate, decimal>> TerrainToBuildingSlotLikelihood { get; } = new Dictionary<Terrain, Dictionary<BuildingSlotTemplate, decimal>>();
        #endregion

        #region Constructors
        public TemplatesDatabase(string databaseFileLocation)
        {
            List<PopTemplateData> popTemplateData;
            List<JobTemplateData> jobTemplateData;
            List<BuildingTemplateData> buildingTemplateData;
            List<BuildingSlotTemplateData> buildingSlotTemplateData;

            ReadRawDataFromXML(databaseFileLocation, out popTemplateData, out jobTemplateData, out buildingTemplateData, out buildingSlotTemplateData);

            if (popTemplateData == null)
            {
                throw new InvalidDataException($"Found that no pop template data was successfully read in from {databaseFileLocation}");
            }
            else if (jobTemplateData == null)
            {
                throw new InvalidDataException($"Found that no job template data was successfully read in from {databaseFileLocation}");
            }
            else if (buildingTemplateData == null)
            {
                throw new InvalidDataException($"Found that no building template data was successfully read in from {databaseFileLocation}");
            }
            else if (buildingSlotTemplateData == null)
            {
                throw new InvalidDataException($"Found that no building slot template data was successfully read in from {databaseFileLocation}");
            }

            foreach (PopTemplateData pop in popTemplateData)
            {
                ConsumeablesCollection necessities = InterpretResourcesIntoCollection(pop.necessities);
                ConsumeablesCollection comforts = InterpretResourcesIntoCollection(pop.comforts);
                ConsumeablesCollection luxuries = InterpretResourcesIntoCollection(pop.luxuries);
                PopTemplate newPop = new PopTemplate(pop.name, pop.displayName, necessities, comforts, luxuries);
                PopTemplatesByName.Add(pop.name, newPop);
            }

            foreach (JobTemplateData job in jobTemplateData)
            {
                if (PopTemplatesByName.ContainsKey(job.popTemplateName))
                {
                    ConsumeablesCollection inputs = InterpretResourcesIntoCollection(job.inputs);
                    ConsumeablesCollection outputs = InterpretResourcesIntoCollection(job.outputs);
                    PopTemplate popType = PopTemplatesByName[job.popTemplateName];
                    JobTemplate newJob = new JobTemplate(job.name, job.displayName, (int)job.priority, (int)job.basePay, popType, inputs, outputs);
                    JobTemplatesByName.Add(job.name, newJob);
                }
                else
                {
                    throw new InvalidDataException($"While constructing job templates, job template with name {job.name} references unrecognized pop template with name {job.popTemplateName}");
                }
            }

            foreach (BuildingSlotTemplateData buildingSlot in buildingSlotTemplateData)
            {
                ConsumeablesCollection resourcesUponRemoval = InterpretResourcesIntoCollection(buildingSlot.resourcesUponRemoval);
                List<JobTemplate> childJobs = new List<JobTemplate>();
                foreach (string jobName in buildingSlot.childJobs.Keys)
                {
                    if (JobTemplatesByName.ContainsKey(jobName))
                    {
                        childJobs.Add(JobTemplatesByName[jobName]);
                    }
                    else
                    {
                        throw new InvalidDataException($"While constructing building slot templates, building slot template with name {buildingSlot.name} references unrecognized job template with name {jobName}");
                    }
                }
                Dictionary<Terrain, decimal> likelihoodPerTerrainType = new Dictionary<Terrain, decimal>();
                foreach (string terrainName in buildingSlot.likelihoodPerTerrainType.Keys)
                {
                    Terrain terrainType = Terrain.AllTerrains.FirstOrDefault(t => t.Name == terrainName);
                    likelihoodPerTerrainType.Add(terrainType, buildingSlot.likelihoodPerTerrainType[terrainName].Item1);
                }
                BuildingSlotTemplate newBuildingSlot = new BuildingSlotTemplate(buildingSlot.name, buildingSlot.displayName, childJobs, resourcesUponRemoval, likelihoodPerTerrainType);
                BuildingSlotTemplatesByName.Add(buildingSlot.name, newBuildingSlot);
            }
            // After initializing each BuildingSlotTemplate, iterate through once more to connect them together
            foreach (BuildingSlotTemplateData buildingSlot in buildingSlotTemplateData)
            {
                BuildingSlotTemplate sourceTemplate = BuildingSlotTemplatesByName[buildingSlot.name];
                foreach (string terrainName in buildingSlot.likelihoodPerTerrainType.Keys)
                {
                    Terrain terrainType = Terrain.AllTerrains.FirstOrDefault(t => t.Name == terrainName);
                    string targetTerrainName = buildingSlot.likelihoodPerTerrainType[terrainName].Item2;
                    if (targetTerrainName != null)
                    {
                        if (BuildingSlotTemplatesByName.ContainsKey(targetTerrainName))
                        {
                            BuildingSlotTemplate targetTemplate = BuildingSlotTemplatesByName[targetTerrainName];
                            sourceTemplate.UnderlyingSlotType.Add(terrainType, targetTemplate);
                        }
                        else
                        {
                            throw new InvalidDataException($"While constructing building slot templates, building slot template with name {buildingSlot.name} references unrecognized building slot template with name {buildingSlot.likelihoodPerTerrainType[terrainName].Item2}");
                        }
                    }
                }
            }
            // Finally, iterate through all the BuildingSlotTemplates to organize them for random generation
            foreach (BuildingSlotTemplate buildingSlot in BuildingSlotTemplatesByName.Values)
            {
                foreach (Terrain terrainType in buildingSlot.ProbabilityWeightPerTerrainType.Keys)
                {
                    if (!TerrainToBuildingSlotLikelihood.ContainsKey(terrainType))
                    {
                        TerrainToBuildingSlotLikelihood.Add(terrainType, new Dictionary<BuildingSlotTemplate, decimal>());
                    }
                    TerrainToBuildingSlotLikelihood[terrainType].Add(buildingSlot, buildingSlot.ProbabilityWeightPerTerrainType[terrainType]);
                }
            }

            foreach (BuildingTemplateData building in buildingTemplateData)
            {
                ConsumeablesCollection costs = InterpretResourcesIntoCollection(building.constructionCosts);
                ConsumeablesCollection outputs = InterpretResourcesIntoCollection(building.resourceOutputs);
                List<JobTemplate> childJobs = new List<JobTemplate>();
                List<BuildingSlotTemplate> requisitBuildingSlots = new List<BuildingSlotTemplate>();
                foreach (string jobName in building.jobOutputs.Keys)
                {
                    if (JobTemplatesByName.ContainsKey(jobName))
                    {
                        JobTemplate job = JobTemplatesByName[jobName];
                        for (int i = 0; i < building.jobOutputs[jobName]; i++)
                        {
                            childJobs.Add(job);
                        }
                    }
                    else
                    {
                        throw new InvalidDataException($"While constructing building templates, building template with name {building.name} references unrecognized job template with name {jobName}");
                    }
                }
                foreach (string buildingSlotName in building.requisitBuildingSlots)
                {
                    if (BuildingSlotTemplatesByName.ContainsKey(buildingSlotName))
                    {
                        BuildingSlotTemplate buildingSlot = BuildingSlotTemplatesByName[buildingSlotName];
                        requisitBuildingSlots.Add(buildingSlot);
                    }
                    else
                    {
                        throw new InvalidDataException($"While constructing building templates, building template with name {building.name} references unrecognized building slot template with name {buildingSlotName}");
                    }
                }
                BuildingTemplate newBuilding = new BuildingTemplate(building.name, building.displayName, childJobs, requisitBuildingSlots, building.isSpaceUnique, costs, outputs);
                BuildingTemplatesByName.Add(building.name, newBuilding);
            }
        }
        #endregion

        #region Methods
        public PopTemplate GetPopTemplate(string templateName)
        {
            if (PopTemplatesByName.ContainsKey(templateName))
            {
                return PopTemplatesByName[templateName];
            }
            throw new ArgumentException($"No pop template with name {templateName} was found");
        }

        public JobTemplate GetJobTemplate(string templateName)
        {
            if (JobTemplatesByName.ContainsKey(templateName))
            {
                return JobTemplatesByName[templateName];
            }
            throw new ArgumentException($"No job template with name {templateName} was found");
        }

        public BuildingTemplate GetBuildingTemplate(string templateName)
        {
            if (BuildingTemplatesByName.ContainsKey(templateName))
            {
                return BuildingTemplatesByName[templateName];
            }
            throw new ArgumentException($"No building template with name {templateName} was found");
        }

        public BuildingSlotTemplate GetBuildingSlotTemplate(string templateName)
        {
            if (BuildingSlotTemplatesByName.ContainsKey(templateName))
            {
                return BuildingSlotTemplatesByName[templateName];
            }
            throw new ArgumentException($"No building slot template with name {templateName} was found");
        }

        public TechnologyTemplate GetTechnologyTemplate(string templateName)
        {
            if (TechnologyTemplatesByName.ContainsKey(templateName))
            {
                return TechnologyTemplatesByName[templateName];
            }
            throw new ArgumentException($"No technology template with name {templateName} was found");
        }

        public Dictionary<BuildingSlotTemplate, decimal> GetPossibleBuildingSlotsForTerrain(Terrain terrainType)
        {
            if (TerrainToBuildingSlotLikelihood.ContainsKey(terrainType))
            {
                return TerrainToBuildingSlotLikelihood[terrainType];
            }
            return null;
        }

        #region XML Reading Methods
        private void ReadRawDataFromXML(string databaseFileLocation, out List<PopTemplateData> popTemplateData, out List<JobTemplateData> jobTemplateData, out List<BuildingTemplateData> buildingTemplateData, out List<BuildingSlotTemplateData> buildingSlotTemplateData)
        {
            popTemplateData = null;
            jobTemplateData = null;
            buildingTemplateData = null;
            buildingSlotTemplateData = null;
            bool endReached = false;
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
                                    break;
                                case XML_POP_TEMPLATES_ELEMENT_NAME:
                                    popTemplateData = ReadPopTemplates(reader);
                                    break;
                                case XML_JOB_TEMPLATES_ELEMENT_NAME:
                                    jobTemplateData = ReadJobTemplates(reader);
                                    break;
                                case XML_BUILDING_TEMPLATES_ELEMENT_NAME:
                                    buildingTemplateData = ReadBuildingTemplates(reader);
                                    break;
                                case XML_BUILDING_SLOT_TEMPLATES_ELEMENT_NAME:
                                    buildingSlotTemplateData = ReadBuildingSlotTemplates(reader);
                                    break;
                                case XML_TECHNOLOGY_TEMPLATES_ELEMENT_NAME:
                                    // @TODO
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.Whitespace:
                            break;
                        default:
                            throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                    }
                }
            }
        }

        private List<ResourceQuantityData> ReadResourceQuantities(XmlReader reader)
        {
            List<ResourceQuantityData> output = new List<ResourceQuantityData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Resource":
                                ResourceQuantityData newElement = new ResourceQuantityData();
                                newElement.resourceName = reader.GetAttribute("name");
                                newElement.quantity = decimal.Parse(reader.GetAttribute("quantity"));
                                output.Add(newElement);
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        endReached = true;
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }

        private List<JobQuantityData> ReadJobQuantities(XmlReader reader)
        {
            List<JobQuantityData> output = new List<JobQuantityData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "JobTemplate":
                                JobQuantityData newElement = new JobQuantityData();
                                newElement.jobTemplateName = reader.GetAttribute("name");
                                newElement.quantity = string.IsNullOrEmpty(reader.GetAttribute("quantity")) ? 1 : decimal.Parse(reader.GetAttribute("quantity"));
                                output.Add(newElement);
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        endReached = true;
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }

        private List<PopTemplateData> ReadPopTemplates(XmlReader reader)
        {
            List<PopTemplateData> templates = new List<PopTemplateData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_POP_TEMPLATE_ELEMENT_NAME:
                                templates.Add(ReadSinglePopTemplate(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_POP_TEMPLATES_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return templates;
        }

        private PopTemplateData ReadSinglePopTemplate(XmlReader reader)
        {
            bool endReached = false;
            PopTemplateData output = new PopTemplateData();
            output.name = reader.GetAttribute("name");
            output.displayName = string.IsNullOrEmpty(reader.GetAttribute("displayName")) ? null : reader.GetAttribute("displayName");
            output.progressFromSatisfactionRatio = decimal.Parse(reader.GetAttribute("progressFromSatisfactionRatio"));
            output.necessities = new Dictionary<string, decimal>();
            output.comforts = new Dictionary<string, decimal>();
            output.luxuries = new Dictionary<string, decimal>();
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Necessities":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.necessities.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            case "Comforts":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.comforts.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            case "Luxuries":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.luxuries.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_POP_TEMPLATE_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }

        private List<JobTemplateData> ReadJobTemplates(XmlReader reader)
        {
            List<JobTemplateData> templates = new List<JobTemplateData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_JOB_TEMPLATE_ELEMENT_NAME:
                                templates.Add(ReadSingleJobTemplate(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_JOB_TEMPLATES_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return templates;
        }

        private JobTemplateData ReadSingleJobTemplate(XmlReader reader)
        {
            bool endReached = false;
            JobTemplateData output = new JobTemplateData();
            output.name = reader.GetAttribute("name");
            output.displayName = string.IsNullOrEmpty(reader.GetAttribute("displayName")) ? null : reader.GetAttribute("displayName");
            output.popTemplateName = reader.GetAttribute("popTemplate");
            output.priority = decimal.Parse(reader.GetAttribute("priority"));
            output.basePay = decimal.Parse(reader.GetAttribute("basePay"));
            output.inputs = new Dictionary<string, decimal>();
            output.outputs = new Dictionary<string, decimal>();
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Inputs":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.inputs.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            case "Outputs":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.outputs.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_JOB_TEMPLATE_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }

        private List<BuildingTemplateData> ReadBuildingTemplates(XmlReader reader)
        {
            List<BuildingTemplateData> templates = new List<BuildingTemplateData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_BUILDING_TEMPLATE_ELEMENT_NAME:
                                templates.Add(ReadSingleBuildingTemplate(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_BUILDING_TEMPLATES_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return templates;
        }

        private BuildingTemplateData ReadSingleBuildingTemplate(XmlReader reader)
        {
            bool endReached = false;
            BuildingTemplateData output = new BuildingTemplateData();
            output.name = reader.GetAttribute("name");
            output.displayName = string.IsNullOrEmpty(reader.GetAttribute("displayName")) ? null : reader.GetAttribute("displayName");
            output.isSpaceUnique = bool.Parse(reader.GetAttribute("isSpaceUnique"));
            output.constructionCosts = new Dictionary<string, decimal>();
            output.resourceOutputs = new Dictionary<string, decimal>();
            output.jobOutputs = new Dictionary<string, decimal>();
            output.requisitBuildingSlots = new List<string>();
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "ConstructionCosts":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.constructionCosts.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            case "Outputs":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.resourceOutputs.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            case "Jobs":
                                foreach (JobQuantityData job in ReadJobQuantities(reader))
                                {
                                    output.jobOutputs.Add(job.jobTemplateName, job.quantity);
                                }
                                break;
                            case "RequisitBuildingSlots":
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "BuildingSlotTemplate")
                                    {
                                        output.requisitBuildingSlots.Add(reader.GetAttribute("name"));
                                    }
                                    reader.Read();
                                }
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_BUILDING_TEMPLATE_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }

        private List<BuildingSlotTemplateData> ReadBuildingSlotTemplates(XmlReader reader)
        {
            List<BuildingSlotTemplateData> templates = new List<BuildingSlotTemplateData>();
            bool endReached = false;
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_BUILDING_SLOT_TEMPLATE_ELEMENT_NAME:
                                templates.Add(ReadSingleBuildingSlotTemplate(reader));
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_BUILDING_SLOT_TEMPLATES_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return templates;
        }

        private BuildingSlotTemplateData ReadSingleBuildingSlotTemplate(XmlReader reader)
        {
            bool endReached = false;
            BuildingSlotTemplateData output = new BuildingSlotTemplateData();
            output.name = reader.GetAttribute("name");
            output.displayName = string.IsNullOrEmpty(reader.GetAttribute("displayName")) ? null : reader.GetAttribute("displayName");
            output.likelihoodPerTerrainType = new Dictionary<string, Tuple<decimal, string>>();
            output.childJobs = new Dictionary<string, decimal>();
            output.resourcesUponRemoval = new Dictionary<string, decimal>();
            while (!endReached && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "PossibleTerrainTypes":
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Terrain")
                                    {
                                        output.likelihoodPerTerrainType.Add(reader.GetAttribute("name"), new Tuple<decimal, string>(decimal.Parse(reader.GetAttribute("likelihood")), reader.GetAttribute("underlyingBuildingSlot")));
                                    }
                                    reader.Read();
                                }
                                break;
                            case "ChildJobs":
                                foreach (JobQuantityData job in ReadJobQuantities(reader))
                                {
                                    output.childJobs.Add(job.jobTemplateName, job.quantity);
                                }
                                break;
                            case "ResourcesUponRemoval":
                                foreach (ResourceQuantityData resource in ReadResourceQuantities(reader))
                                {
                                    output.resourcesUponRemoval.Add(resource.resourceName, resource.quantity);
                                }
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case XML_SINGULAR_BUILDING_SLOT_TEMPLATE_ELEMENT_NAME:
                                endReached = true;
                                break;
                            default:
                                throw new InvalidDataException($"Got invalid XML end element with name \"{reader.Name}\" while parsing TemplatesDatabase");
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw new InvalidDataException($"Got invalid XML node type {reader.NodeType} with name \"{reader.Name}\" while parsing TemplatesDatabase");
                }
            }
            return output;
        }
        #endregion

        private ConsumeablesCollection InterpretResourcesIntoCollection(Dictionary<string, decimal> resourcesByName)
        {
            ConsumeablesCollection output = new ConsumeablesCollection();
            foreach (string resourceName in resourcesByName.Keys)
            {
                Fundamental resource = Fundamental.AllFundamentals.FirstOrDefault(f => f.Name == resourceName);
                if (resource != null)
                {
                    output.Add(resource, resourcesByName[resourceName]);
                }
            }
            return output;
        }
        #endregion
    }
}
