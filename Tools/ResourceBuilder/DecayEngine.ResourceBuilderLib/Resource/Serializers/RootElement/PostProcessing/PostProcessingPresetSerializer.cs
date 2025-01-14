using System.Collections.Generic;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.PostProcessing
{
    public class PostProcessingPresetSerializer : IResourceSerializer<PostProcessingPresetResource>
    {
        public string Tag => "postProcessingPreset";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is PostProcessingPresetResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(PostProcessingPresetResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");
            XElement stagesElement = new XElement("stages");

            IResourceSerializer<PostProcessingStage> postProcessingStageSerializer = ResourceSerializationController.GetSerializer<PostProcessingStage>();
            if (postProcessingStageSerializer != null)
            {
                foreach (PostProcessingStage postProcessingStage in resource.Stages)
                {
                    XElement postProcessingStageElement = postProcessingStageSerializer.Serialize(postProcessingStage);
                    if (postProcessingStageElement == null) continue;

                    stagesElement.Add(postProcessingStageElement);
                }
            }

            if (stagesElement.HasElements)
            {
                propertiesElement.Add(stagesElement);
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public PostProcessingPresetResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            PostProcessingPresetResource resource = new PostProcessingPresetResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                Stages = new List<PostProcessingStage>()
            };

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "properties":
                        foreach (XElement property in child.Elements())
                        {
                            switch (property.Name.ToString())
                            {
                                case "stages":
                                {
                                    IResourceSerializer<PostProcessingStage> postProcessingStageSerializer =
                                        ResourceSerializationController.GetSerializer<PostProcessingStage>();
                                    if (postProcessingStageSerializer == null) continue;

                                    foreach (XElement postProcessingStageElement in property.Elements())
                                    {
                                        PostProcessingStage postProcessingStage = postProcessingStageSerializer.Deserialize(postProcessingStageElement);
                                        if (postProcessingStage == null) continue;

                                        resource.Stages.Add(postProcessingStage);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                }
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(PostProcessingPresetResource));
        }
    }
}