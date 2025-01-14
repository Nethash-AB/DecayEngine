using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.AnimatedSprite;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Camera;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Light;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Mesh;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.PbrMaterial;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.RenderTargetSprite;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.RigidBody;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Script;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.ShaderProgram;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Sound;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.SoundBank;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.TextSprite;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;
using DecayEngine.DecPakLib.Resource.Structure.Component.TextSprite;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.DecPakLib.Resource.Structure.Transform;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Statement.Component
{
    public class CreateComponentSerializer : IResourceSerializer<CreateComponentExpression>
    {
        public string Tag => "component";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is CreateComponentExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(CreateComponentExpression resource)
        {
            XElement element = new XElement(Tag);

            char[] typeChars = resource.ComponentType.ToString().ToCharArray();
            typeChars[0] = char.ToLowerInvariant(typeChars[0]);

            element.SetAttributeValue("type", new string(typeChars));
            element.SetAttributeValue("name", resource.Name);
            element.SetAttributeValue("active", resource.Active);

            if (resource.Template != null)
            {
                IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Template);
                if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                {
                    XElement propertyExpressionElement = serializer.Serialize(resource.Template);
                    if (propertyExpressionElement != null)
                    {
                        XElement tempateElement = new XElement("template");
                        tempateElement.Add(propertyExpressionElement);
                        element.Add(tempateElement);
                    }
                }
            }

            XElement propertiesElement = new XElement("properties");
            switch (resource)
            {
                case CreateAnimatedSpriteComponentExpression animatedSpriteComponentExpression:
                {
                    XElement transformElement = ResourceSerializationController
                        .GetSerializer<TransformStructure>()
                        .Serialize(animatedSpriteComponentExpression.Transform);
                    if (transformElement != null)
                    {
                        propertiesElement.Add(transformElement);
                    }

                    XElement materialElement = new XElement("material");
                    XElement materialExpressionElement = ParseExpression(animatedSpriteComponentExpression.Material);
                    if (materialExpressionElement != null)
                    {
                        materialElement.Add(materialExpressionElement);
                        propertiesElement.Add(materialElement);
                    }

                    XElement shaderProgramElement = new XElement("shaderProgram");
                    XElement shaderProgramExpressionElement = ParseExpression(animatedSpriteComponentExpression.ShaderProgram);
                    if (shaderProgramExpressionElement != null)
                    {
                        shaderProgramElement.Add(shaderProgramExpressionElement);
                        propertiesElement.Add(shaderProgramElement);
                    }

                    XElement cameraElement = new XElement("camera");
                    XElement cameraExpressionElement = ParseExpression(animatedSpriteComponentExpression.Camera);
                    if (cameraExpressionElement != null)
                    {
                        cameraElement.Add(cameraExpressionElement);
                        propertiesElement.Add(cameraElement);
                    }

                    break;
                }

                case CreateRenderTargetSpriteComponentExpression renderTargetSpriteComponentExpression:
                {
                    XElement transformElement = ResourceSerializationController
                        .GetSerializer<TransformStructure>()
                        .Serialize(renderTargetSpriteComponentExpression.Transform);
                    if (transformElement != null)
                    {
                        propertiesElement.Add(transformElement);
                    }

                    XElement frameBufferElement = new XElement("frameBuffer");
                    XElement frameBufferExpressionElement = ParseExpression(renderTargetSpriteComponentExpression.FrameBuffer);
                    if (frameBufferExpressionElement != null)
                    {
                        frameBufferElement.Add(frameBufferExpressionElement);
                        propertiesElement.Add(frameBufferElement);
                    }

                    XElement shaderProgramElement = new XElement("shaderProgram");
                    XElement shaderProgramExpressionElement = ParseExpression(renderTargetSpriteComponentExpression.ShaderProgram);
                    if (shaderProgramExpressionElement != null)
                    {
                        shaderProgramElement.Add(shaderProgramExpressionElement);
                        propertiesElement.Add(shaderProgramElement);
                    }

                    XElement cameraElement = new XElement("camera");
                    XElement cameraExpressionElement = ParseExpression(renderTargetSpriteComponentExpression.Camera);
                    if (cameraExpressionElement != null)
                    {
                        cameraElement.Add(cameraExpressionElement);
                        propertiesElement.Add(cameraElement);
                    }

                    XElement maintainAspectRatioElement = new XElement("maintainAspectRatio", renderTargetSpriteComponentExpression.MaintainAspectRatio);
                    propertiesElement.Add(maintainAspectRatioElement);

                    break;
                }

                case CreateTextSpriteComponentExpression textSpriteComponentExpression:
                {
                    XElement textElement = new XElement("text", textSpriteComponentExpression.Text);
                    propertiesElement.Add(textElement);

                    string alignment;
                    if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.HorizontalLeft))
                    {
                        alignment = "horizontalLeft";
                    }
                    else if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.HorizontalCenter))
                    {
                        alignment = "horizontalCenter";
                    }
                    else if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.HorizontalRight))
                    {
                        alignment = "horizontalRight";
                    }
                    else
                    {
                        alignment = "horizontalLeft";
                    }

                    if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.VerticalTop))
                    {
                        alignment += "|verticalTop";
                    }
                    else if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.VerticalCenter))
                    {
                        alignment += "|verticalCenter";
                    }
                    else if (textSpriteComponentExpression.Alignment.HasFlag(TextSpriteAlignment.VerticalBottom))
                    {
                        alignment += "|verticalBottom";
                    }
                    else
                    {
                        alignment += "|verticalTop";
                    }
                    XElement alignmentElement = new XElement("alignment", alignment);
                    propertiesElement.Add(alignmentElement);

                    XElement colorElement = ResourceSerializationController.GetSerializer<ColorStructure>().Serialize(textSpriteComponentExpression.Color);
                    if (colorElement != null)
                    {
                        propertiesElement.Add(colorElement);
                    }

                    XElement fontSizeElement = new XElement("fontSize", textSpriteComponentExpression.FontSize);
                    propertiesElement.Add(fontSizeElement);

                    XElement characterSeparationElement = new XElement("characterSeparation", textSpriteComponentExpression.CharacterSeparation);
                    propertiesElement.Add(characterSeparationElement);

                    XElement whiteSpaceSeparationElement = new XElement("whiteSpaceSeparation", textSpriteComponentExpression.WhiteSpaceSeparation);
                    propertiesElement.Add(whiteSpaceSeparationElement);

                    XElement transformElement = ResourceSerializationController
                        .GetSerializer<TransformStructure>()
                        .Serialize(textSpriteComponentExpression.Transform);
                    if (transformElement != null)
                    {
                        propertiesElement.Add(transformElement);
                    }

                    XElement shaderProgramElement = new XElement("shaderProgram");
                    XElement shaderProgramExpressionElement = ParseExpression(textSpriteComponentExpression.ShaderProgram);
                    if (shaderProgramExpressionElement != null)
                    {
                        shaderProgramElement.Add(shaderProgramExpressionElement);
                        propertiesElement.Add(shaderProgramElement);
                    }

                    XElement cameraElement = new XElement("camera");
                    XElement cameraExpressionElement = ParseExpression(textSpriteComponentExpression.Camera);
                    if (cameraExpressionElement != null)
                    {
                        cameraElement.Add(cameraExpressionElement);
                        propertiesElement.Add(cameraElement);
                    }

                    break;
                }

                case CreateMeshComponentExpression meshComponentExpression:
                {
                    XElement materialElement = new XElement("material");
                    XElement materialExpressionElement = ParseExpression(meshComponentExpression.Material);
                    if (materialExpressionElement != null)
                    {
                        materialElement.Add(materialExpressionElement);
                        propertiesElement.Add(materialElement);
                    }

                    XElement shaderProgramElement = new XElement("shaderProgram");
                    XElement shaderProgramExpressionElement = ParseExpression(meshComponentExpression.ShaderProgram);
                    if (shaderProgramExpressionElement != null)
                    {
                        shaderProgramElement.Add(shaderProgramExpressionElement);
                        propertiesElement.Add(shaderProgramElement);
                    }

                    XElement cameraElement = new XElement("camera");
                    XElement cameraExpressionElement = ParseExpression(meshComponentExpression.Camera);
                    if (cameraExpressionElement != null)
                    {
                        cameraElement.Add(cameraExpressionElement);
                        propertiesElement.Add(cameraElement);
                    }

                    break;
                }

                case CreateCameraComponentExpression cameraComponentExpression:
                {
                    XElement transformElement = ResourceSerializationController
                        .GetSerializer<TransformStructure>()
                        .Serialize(cameraComponentExpression.Transform);
                    if (transformElement != null)
                    {
                        propertiesElement.Add(transformElement);
                    }

                    if (cameraComponentExpression.PostProcessingPreset != null)
                    {
                        IResourceSerializer serializer = ResourceSerializationController.GetSerializer(cameraComponentExpression.PostProcessingPreset);
                        if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                        {
                            XElement propertyExpressionElement = serializer.Serialize(cameraComponentExpression.PostProcessingPreset);
                            if (propertyExpressionElement != null)
                            {
                                XElement postProcessingPresetElement = new XElement("postProcessingPreset");
                                postProcessingPresetElement.Add(propertyExpressionElement);
                                propertiesElement.Add(postProcessingPresetElement);
                            }
                        }
                    }

                    XElement zNearElement = new XElement("zNear", cameraComponentExpression.ZNear);
                    propertiesElement.Add(zNearElement);

                    XElement zFarElement = new XElement("zFar", cameraComponentExpression.ZFar);
                    propertiesElement.Add(zFarElement);

                    if (cameraComponentExpression is CreateCameraPerspectiveComponentExpression perspCameraComponentExpression)
                    {
                        XElement fovElement = new XElement("fov", perspCameraComponentExpression.FieldOfView);
                        propertiesElement.Add(fovElement);

                        XElement isPbrElement = new XElement("isPbr", perspCameraComponentExpression.IsPbr);
                        propertiesElement.Add(isPbrElement);

                        if (cameraComponentExpression.EnvironmentTexture != null)
                        {
                            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(cameraComponentExpression.EnvironmentTexture);
                            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                            {
                                XElement propertyExpressionElement = serializer.Serialize(cameraComponentExpression.EnvironmentTexture);
                                if (propertyExpressionElement != null)
                                {
                                    XElement environmentTextureElement = new XElement("environmentTexture");
                                    environmentTextureElement.Add(propertyExpressionElement);
                                    propertiesElement.Add(environmentTextureElement);
                                }
                            }
                        }
                    }

                    XElement isAudioListenerElement = new XElement("isAudioListener", cameraComponentExpression.IsAudioListener);
                    propertiesElement.Add(isAudioListenerElement);

                    XElement renderToScreenElement = new XElement("renderToScreen", cameraComponentExpression.RenderToScreen);
                    propertiesElement.Add(renderToScreenElement);

                    break;
                }

                case CreateScriptComponentExpression scriptComponentExpression:
                {
                    if (scriptComponentExpression.Injections != null && scriptComponentExpression.Injections.Count > 0)
                    {
                        XElement injectionsElement = new XElement("injections");
                        foreach (ScriptInjection injection in scriptComponentExpression.Injections)
                        {
                            XElement injectionElement = ResourceSerializationController.GetSerializer<ScriptInjection>().Serialize(injection);
                            if (injectionElement != null)
                            {
                                injectionsElement.Add(injectionElement);
                            }
                        }

                        if (injectionsElement.HasElements)
                        {
                            propertiesElement.Add(injectionsElement);
                        }
                    }

                    break;
                }

                case CreateSoundComponentExpression soundComponentExpression:
                {
                    XElement transformElement = ResourceSerializationController
                        .GetSerializer<TransformStructure>()
                        .Serialize(soundComponentExpression.Transform);
                    if (transformElement != null)
                    {
                        propertiesElement.Add(transformElement);
                    }

                    XElement autoPlayOnActiveElement = new XElement("autoPlayOnActive", soundComponentExpression.AutoPlayOnActive);
                    propertiesElement.Add(autoPlayOnActiveElement);

                    break;
                }

                case CreateLightComponentExpression lightComponentExpression:
                {
                    XElement cameraElement = new XElement("camera");
                    XElement cameraExpressionElement = ParseExpression(lightComponentExpression.Camera);
                    if (cameraExpressionElement != null)
                    {
                        cameraElement.Add(cameraExpressionElement);
                        propertiesElement.Add(cameraElement);
                    }

                    XElement strengthElement = new XElement("strength", lightComponentExpression.Strength);
                    propertiesElement.Add(strengthElement);

                    XElement colorElement = ResourceSerializationController.GetSerializer<ColorStructure>().Serialize(lightComponentExpression.Color);
                    if (colorElement != null)
                    {
                        propertiesElement.Add(colorElement);
                    }

                    switch (lightComponentExpression)
                    {
                        case CreateLightPointComponentExpression pointLightComponentExpression:
                        {
                            XElement radiusElement = new XElement("radius", pointLightComponentExpression.Radius);
                            propertiesElement.Add(radiusElement);
                            break;
                        }
                        case CreateLightSpotComponentExpression spotLightComponentExpression:
                        {
                            XElement radiusElement = new XElement("radius", spotLightComponentExpression.Radius);
                            propertiesElement.Add(radiusElement);

                            XElement cutoffAngleElement = new XElement("cutoffAngle", spotLightComponentExpression.CutoffAngle);
                            propertiesElement.Add(cutoffAngleElement);
                            break;
                        }
                    }

                    break;
                }
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public CreateComponentExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            CreateComponentExpression resource;
            switch ((ComponentType) Enum.Parse(typeof(ComponentType), element.Attribute("type")?.Value, true))
            {
                case ComponentType.RigidBody:
                    resource = new CreateRigidBodyComponentExpression();
                    break;
                case ComponentType.AnimatedMaterial:
                    resource = new CreateAnimatedMaterialComponentExpression();
                    break;
                case ComponentType.PbrMaterial:
                    resource = new CreatePbrMaterialComponentExpression();
                    break;
                case ComponentType.Script:
                    resource = new CreateScriptComponentExpression();
                    break;
                case ComponentType.ShaderProgram:
                    resource = new CreateShaderProgramComponentExpression();
                    break;
                case ComponentType.AnimatedSprite:
                    resource = new CreateAnimatedSpriteComponentExpression();
                    break;
                case ComponentType.RenderTargetSprite:
                    resource = new CreateRenderTargetSpriteComponentExpression();
                    break;
                case ComponentType.TextSprite:
                    resource = new CreateTextSpriteComponentExpression();
                    break;
                case ComponentType.Mesh:
                    resource = new CreateMeshComponentExpression();
                    break;
                case ComponentType.CameraPersp:
                    resource = new CreateCameraPerspectiveComponentExpression();
                    break;
                case ComponentType.CameraOrtho:
                    resource = new CreateCameraOrthographicComponentExpression();
                    break;
                case ComponentType.Sound:
                    resource = new CreateSoundComponentExpression();
                    break;
                case ComponentType.SoundBank:
                    resource = new CreateSoundBankComponentExpression();
                    break;
                case ComponentType.LightPoint:
                    resource = new CreateLightPointComponentExpression();
                    break;
                case ComponentType.LightDirectional:
                    resource = new CreateLightDirectionalComponentExpression();
                    break;
                case ComponentType.LightSpot:
                    resource = new CreateLightSpotComponentExpression();
                    break;
                default:
                    return null;
            }

            resource.Name = element.Attribute("name")?.Value;
            resource.Active = bool.Parse(element.Attribute("active")?.Value ?? "true");

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "template":
                        foreach (XElement template in child.Elements())
                        {
                            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(template.Name.ToString());
                            if (expressionSerializer == null || !expressionSerializer.BuildsType<IPropertyExpression>()) continue;

                            IPropertyExpression expression = (IPropertyExpression) expressionSerializer.Deserialize(template);
                            if (expression == null) continue;

                            resource.Template = expression;
                        }
                        break;
                    case "properties":
                        switch (resource)
                        {
                            case CreateAnimatedSpriteComponentExpression animatedSpriteComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "transform":
                                            IResourceSerializer<TransformStructure> transformSerializer =
                                                ResourceSerializationController.GetSerializer<TransformStructure>();
                                            if (transformSerializer == null) continue;

                                            animatedSpriteComponentExpression.Transform = transformSerializer.Deserialize(property);
                                            break;
                                        case "material":
                                            animatedSpriteComponentExpression.Material = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "shaderProgram":
                                            animatedSpriteComponentExpression.ShaderProgram = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "camera":
                                            animatedSpriteComponentExpression.Camera = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                    }
                                }
                                break;
                            }
                            case CreateRenderTargetSpriteComponentExpression renderTargetSpriteComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "transform":
                                            IResourceSerializer<TransformStructure> transformSerializer =
                                                ResourceSerializationController.GetSerializer<TransformStructure>();
                                            if (transformSerializer == null) continue;

                                            renderTargetSpriteComponentExpression.Transform = transformSerializer.Deserialize(property);
                                            break;
                                        case "frameBuffer":
                                            renderTargetSpriteComponentExpression.FrameBuffer = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "shaderProgram":
                                            renderTargetSpriteComponentExpression.ShaderProgram = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "camera":
                                            renderTargetSpriteComponentExpression.Camera = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "maintainAspectRatio":
                                            renderTargetSpriteComponentExpression.MaintainAspectRatio = bool.Parse(property.Value);
                                            break;
                                    }
                                }
                                break;
                            }
                            case CreateMeshComponentExpression meshComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "material":
                                            meshComponentExpression.Material = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "shaderProgram":
                                            meshComponentExpression.ShaderProgram = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "camera":
                                            meshComponentExpression.Camera = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                    }
                                }
                                break;
                            }
                            case CreateTextSpriteComponentExpression textSpriteComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "text":
                                            textSpriteComponentExpression.Text = property.Value;
                                            break;
                                        case "alignment":
                                            if (property.Value == null) continue;

                                            bool alignmentHSet = false;
                                            bool alignmentVSet = false;

                                            string[] values = property.Value.Split('|');
                                            if (values.Length > 0 && values.Length < 3)
                                            {
                                                foreach (string value in values)
                                                {
                                                    switch (value)
                                                    {
                                                        case "horizontalLeft" when !alignmentHSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.HorizontalLeft;
                                                            alignmentHSet = true;
                                                            break;
                                                        case "horizontalCenter" when !alignmentHSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.HorizontalCenter;
                                                            alignmentHSet = true;
                                                            break;
                                                        case "horizontalRight" when !alignmentHSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.HorizontalRight;
                                                            alignmentHSet = true;
                                                            break;
                                                        case "verticalTop" when !alignmentVSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.VerticalTop;
                                                            alignmentVSet = true;
                                                            break;
                                                        case "verticalCenter" when !alignmentVSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.VerticalCenter;
                                                            alignmentVSet = true;
                                                            break;
                                                        case "verticalBottom" when !alignmentVSet:
                                                            textSpriteComponentExpression.Alignment |= TextSpriteAlignment.VerticalBottom;
                                                            alignmentVSet = true;
                                                            break;
                                                    }
                                                }
                                            }

                                            if (!alignmentHSet)
                                            {
                                                textSpriteComponentExpression.Alignment |= TextSpriteAlignment.HorizontalLeft;
                                            }
                                            if (!alignmentVSet)
                                            {
                                                textSpriteComponentExpression.Alignment |= TextSpriteAlignment.VerticalTop;
                                            }

                                            break;
                                        case "color":
                                            IResourceSerializer<ColorStructure> colorSerializer =
                                                ResourceSerializationController.GetSerializer<ColorStructure>();
                                            if (colorSerializer == null) continue;

                                            textSpriteComponentExpression.Color = colorSerializer.Deserialize(property);
                                            break;
                                        case "fontSize":
                                            textSpriteComponentExpression.FontSize = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "characterSeparation":
                                            textSpriteComponentExpression.CharacterSeparation = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "whiteSpaceSeparation":
                                            textSpriteComponentExpression.WhiteSpaceSeparation = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "transform":
                                            IResourceSerializer<TransformStructure> transformSerializer =
                                                ResourceSerializationController.GetSerializer<TransformStructure>();
                                            if (transformSerializer == null) continue;

                                            textSpriteComponentExpression.Transform = transformSerializer.Deserialize(property);
                                            break;
                                        case "shaderProgram":
                                            textSpriteComponentExpression.ShaderProgram = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "camera":
                                            textSpriteComponentExpression.Camera = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                    }
                                }
                                break;
                            }
                            case CreateCameraComponentExpression cameraComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "transform":
                                            IResourceSerializer<TransformStructure> transformSerializer =
                                                ResourceSerializationController.GetSerializer<TransformStructure>();
                                            if (transformSerializer == null) continue;

                                            cameraComponentExpression.Transform = transformSerializer.Deserialize(property);
                                            break;
                                        case "postProcessingPreset":
                                            XElement expressionElement = property.Elements().FirstOrDefault();
                                            if (expressionElement == null) continue;

                                            IResourceSerializer expressionSerializer =
                                                ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                                            if (expressionSerializer == null || !expressionSerializer.BuildsType<IPropertyExpression>()) continue;

                                            IPropertyExpression expression = (IPropertyExpression) expressionSerializer.Deserialize(expressionElement);
                                            if (expression == null) continue;

                                            cameraComponentExpression.PostProcessingPreset = expression;
                                            break;
                                        case "zNear":
                                            cameraComponentExpression.ZNear = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "zFar":
                                            cameraComponentExpression.ZFar = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "fov" when cameraComponentExpression is CreateCameraPerspectiveComponentExpression perspCameraComponentExpression:
                                            perspCameraComponentExpression.FieldOfView = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case "isAudioListener":
                                            cameraComponentExpression.IsAudioListener = bool.Parse(property.Value);
                                            break;
                                        case "isPbr"
                                            when cameraComponentExpression is CreateCameraPerspectiveComponentExpression perspCameraComponentExpression:

                                            perspCameraComponentExpression.IsPbr = bool.Parse(property.Value);
                                            break;
                                        case "environmentTexture"
                                            when cameraComponentExpression is CreateCameraPerspectiveComponentExpression perspCameraComponentExpression:

                                            XElement envTextureExpressionElement = property.Elements().FirstOrDefault();
                                            if (envTextureExpressionElement == null) continue;

                                            IResourceSerializer envTextureExpressionSerializer =
                                                ResourceSerializationController.GetSerializer(envTextureExpressionElement.Name.ToString());
                                            if (envTextureExpressionSerializer == null || !envTextureExpressionSerializer.BuildsType<IPropertyExpression>())
                                                continue;

                                            IPropertyExpression envTextureExpression =
                                                (IPropertyExpression) envTextureExpressionSerializer.Deserialize(envTextureExpressionElement);
                                            if (envTextureExpression == null) continue;

                                            perspCameraComponentExpression.EnvironmentTexture = envTextureExpression;

                                            break;
                                        case "renderToScreen":
                                            cameraComponentExpression.RenderToScreen = bool.Parse(property.Value);
                                            break;
                                    }
                                }
                                break;
                            }

                            case CreateScriptComponentExpression scriptComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "injections":
                                            scriptComponentExpression.Injections = new List<ScriptInjection>();

                                            IResourceSerializer<ScriptInjection> injectionSerializer =
                                                ResourceSerializationController.GetSerializer<ScriptInjection>();
                                            if (injectionSerializer == null) continue;

                                            foreach (XElement injectionElement in property.Elements())
                                            {
                                                ScriptInjection injection = injectionSerializer.Deserialize(injectionElement);
                                                if (injection == null || scriptComponentExpression.Injections.Any(inj => inj.Id == injection.Id)) continue;

                                                scriptComponentExpression.Injections.Add(injection);
                                            }
                                            break;
                                    }
                                }
                                break;
                            }

                            case CreateSoundComponentExpression soundComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "transform":
                                            IResourceSerializer<TransformStructure> transformSerializer =
                                                ResourceSerializationController.GetSerializer<TransformStructure>();
                                            if (transformSerializer == null) continue;

                                            soundComponentExpression.Transform = transformSerializer.Deserialize(property);
                                            break;
                                        case "autoPlayOnActive":
                                            soundComponentExpression.AutoPlayOnActive = bool.Parse(property.Value);
                                            break;
                                    }
                                }
                                break;
                            }

                            case CreateLightComponentExpression lightComponentExpression:
                            {
                                foreach (XElement property in child.Elements())
                                {
                                    switch (property.Name.ToString())
                                    {
                                        case "camera":
                                            lightComponentExpression.Camera = ParseExpression<IQueryInitiatorExpression>(property);
                                            break;
                                        case "strength":
                                        {
                                            lightComponentExpression.Strength =
                                                float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        }
                                        case "color":
                                        {
                                            IResourceSerializer<ColorStructure> colorSerializer =
                                                ResourceSerializationController.GetSerializer<ColorStructure>();
                                            if (colorSerializer == null) continue;

                                            lightComponentExpression.Color = colorSerializer.Deserialize(property);
                                            break;
                                        }
                                        case "radius":
                                        {
                                            switch (lightComponentExpression)
                                            {
                                                case CreateLightPointComponentExpression pointLightComponentExpression:
                                                    pointLightComponentExpression.Radius =
                                                        float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    break;
                                                case CreateLightSpotComponentExpression spotLightComponentExpression:
                                                    spotLightComponentExpression.Radius =
                                                        float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    break;
                                            }
                                            break;
                                        }
                                        case "cutoffAngle":
                                        {
                                            if (!(lightComponentExpression is CreateLightSpotComponentExpression spotLightComponentExpression)) continue;
                                            spotLightComponentExpression.CutoffAngle =
                                                float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);

                                            break;
                                        }
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
            return typeof(TResource).IsAssignableFrom(typeof(CreateComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateRigidBodyComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateAnimatedMaterialComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateScriptComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateShaderProgramComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateAnimatedSpriteComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateTextSpriteComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateMeshComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreatePbrMaterialComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateSoundComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateCameraComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateCameraPerspectiveComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateCameraOrthographicComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateLightComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateLightPointComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateLightDirectionalComponentExpression))
                || typeof(TResource).IsAssignableFrom(typeof(CreateLightSpotComponentExpression));
        }

        private static TExpression ParseExpression<TExpression>(XContainer property)
            where TExpression : class, IExpression
        {
            XElement expressionElement = property.Elements().FirstOrDefault();
            if(expressionElement == null) return null;

            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
            if (expressionSerializer == null || !expressionSerializer.BuildsType<TExpression>()) return null;

            TExpression expression = (TExpression) expressionSerializer.Deserialize(expressionElement);

            return expression;
        }

        private static XElement ParseExpression<TExpression>(TExpression expression)
            where TExpression : class, IExpression
        {
            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(expression);
            if (expressionSerializer == null || !expressionSerializer.BuildsType<TExpression>()) return null;

            XElement expressionElement = expressionSerializer.Serialize(expression);
            return expressionElement;
        }
    }
}