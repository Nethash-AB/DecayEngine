using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
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
using DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.Collider;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.DecPakLib.Resource.RootElement.Script;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.DecPakLib.Resource.RootElement.Sound;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Component.TextSprite;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.Mesh;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.GameStructure;
using DecayEngine.ModuleSDK.Object.Scene;
using TransformMode = DecayEngine.DecPakLib.Resource.Structure.Transform.TransformMode;

namespace DecayEngine.ModuleSDK.Expression
{
    public class StatementExpressionResolver
    {
        private readonly IStatementExpression _rootExpression;

        public StatementExpressionResolver(IStatementExpression rootExpression)
        {
            _rootExpression = rootExpression;
        }

        public object Resolve(IComponentable target)
        {
            return ResolveRoot(_rootExpression, target);
        }

        public object Resolve(IComponentable<ISceneAttachableComponent> target)
        {
            return ResolveRoot(_rootExpression, target);
        }

        public object Resolve(IChildBearer<IGameObject> target)
        {
            return ResolveRoot(_rootExpression, target);
        }

        public object Resolve(IGameStructure target)
        {
            return ResolveRoot(_rootExpression, target);
        }

        private static object ResolveRoot(IStatementExpression expression, object target)
        {
            switch (expression)
            {
                case CreateComponentExpression createComponent:
                    return target switch
                    {
                        IComponentable componentable => ResolveCreateComponent(createComponent, componentable),
                        IComponentable<ISceneAttachableComponent> sceneAttachableComponent => ResolveCreateComponent(createComponent, sceneAttachableComponent),
                        _ => null
                    };

                case CreateGameObjectExpression createGameObject:
                    return !(target is IChildBearer<IGameObject> childBearer) ? null : ResolveCreateGameObject(createGameObject, childBearer);
                default:
                    return null;
            }
        }

        private static TComponent ResolveCreateComponent<TComponent>(CreateComponentExpression expression, IComponentable<TComponent> target)
            where TComponent : class, IComponent
        {
            switch (expression)
            {
                case CreateCameraOrthographicComponentExpression createCameraOrtho:
                {
                    ICameraOrtho component = (ICameraOrtho) CreateComponent(ComponentType.CameraOrtho);
                    if (!(component is TComponent castedComponent)) return null;

                    Vector3 scale = component.Transform.Scale;

                    castedComponent.Name = createCameraOrtho.Name;
                    target.AttachComponent(castedComponent);

                    if (createCameraOrtho.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createCameraOrtho.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createCameraOrtho.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createCameraOrtho.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createCameraOrtho.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createCameraOrtho.Transform.Scale ?? Vector3.One
                        );

                        if (!component.ManualSize)
                        {
                            component.Transform.Scale = scale;
                        }
                    }

                    component.ZNear = createCameraOrtho.ZNear;
                    component.ZFar = createCameraOrtho.ZFar;
                    component.RenderToScreen = createCameraOrtho.RenderToScreen;

                    if (createCameraOrtho.IsAudioListener)
                    {
                        GameEngine.SoundEngine.AddListener(component);
                    }

                    PostProcessingPresetResource postProcessingPresetResource =
                        new PropertyExpressionResolver(createCameraOrtho.PostProcessingPreset).Resolve<PostProcessingPresetResource>();
                    if (postProcessingPresetResource != null)
                    {
                        CreatePostProcessingFrameBuffers((IGameStructure) target, component, postProcessingPresetResource);
                    }

                    IShaderProgram defaultFboShaderProgram =
                        (IShaderProgram) CreateComponent(ComponentType.ShaderProgram, GameEngine.RenderEngine.DefaultPostProcessingShaderProgram);
                    if (defaultFboShaderProgram != null)
                    {
                        if (target is IScene targetScene)
                        {
                            defaultFboShaderProgram.SetParent(targetScene);
                        }
                        else
                        {
                            defaultFboShaderProgram.SetParent((IGameObject) target);
                        }

                        IRenderFrameBuffer geometryFrameBuffer = GameEngine.RenderEngine.CreateRenderFrameBuffer();
                        geometryFrameBuffer.Name = "geometry";
                        geometryFrameBuffer.ShaderProgram = defaultFboShaderProgram;
                        component.InsertFrameBufferAt(geometryFrameBuffer, 0);

                        IRenderFrameBuffer cameraFrameBuffer = GameEngine.RenderEngine.CreateRenderFrameBuffer();
                        cameraFrameBuffer.Name = "camera";
                        cameraFrameBuffer.ShaderProgram = defaultFboShaderProgram;

                        component.AddFrameBuffer(cameraFrameBuffer);
                    }

                    return castedComponent;
                }

                case CreateCameraPerspectiveComponentExpression createCameraPersp:
                {
                    ICameraPersp component = (ICameraPersp) CreateComponent(ComponentType.CameraPersp);
                    if (!(component is TComponent castedComponent)) return null;

                    Vector3 scale = component.Transform.Scale;

                    castedComponent.Name = createCameraPersp.Name;
                    target.AttachComponent(castedComponent);

                    if (createCameraPersp.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createCameraPersp.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createCameraPersp.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createCameraPersp.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createCameraPersp.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createCameraPersp.Transform.Scale ?? Vector3.One
                        );

                        if (!component.ManualSize)
                        {
                            component.Transform.Scale = scale;
                        }
                    }

                    component.ZNear = createCameraPersp.ZNear;
                    component.ZFar = createCameraPersp.ZFar;
                    component.FieldOfView = createCameraPersp.FieldOfView;
                    component.RenderToScreen = createCameraPersp.RenderToScreen;

                    if (createCameraPersp.IsAudioListener)
                    {
                        GameEngine.SoundEngine.AddListener(component);
                    }

                    PostProcessingPresetResource postProcessingPresetResource =
                        new PropertyExpressionResolver(createCameraPersp.PostProcessingPreset).Resolve<PostProcessingPresetResource>();
                    if (postProcessingPresetResource != null)
                    {
                        CreatePostProcessingFrameBuffers((IGameStructure) target, component, postProcessingPresetResource);
                    }

                    IShaderProgram defaultFboShaderProgram =
                        (IShaderProgram) CreateComponent(ComponentType.ShaderProgram, GameEngine.RenderEngine.DefaultPostProcessingShaderProgram);

                    if (target is IScene targetScene)
                    {
                        defaultFboShaderProgram.SetParent(targetScene);
                    }
                    else
                    {
                        defaultFboShaderProgram.SetParent((IGameObject) target);
                    }

                    defaultFboShaderProgram.Active = true;

                    if (createCameraPersp.IsPbr)
                    {
                        IDeferredShadingFrameBuffer lightingFrameBuffer = GameEngine.RenderEngine.CreateDeferredShadingFrameBuffer();
                        lightingFrameBuffer.Name = "geometry";
                        lightingFrameBuffer.ShaderProgram =
                            (IShaderProgram) CreateComponent(ComponentType.ShaderProgram, GameEngine.RenderEngine.DefaultPbrLightingShaderProgram);
                        if (target is IScene scene)
                        {
                            lightingFrameBuffer.ShaderProgram.SetParent(scene);
                        }
                        else
                        {
                            lightingFrameBuffer.ShaderProgram.SetParent((IGameObject) target);
                        }

                        lightingFrameBuffer.ShaderProgram.Active = true;
                        component.InsertFrameBufferAt(lightingFrameBuffer, 0);

                        Texture3DResource environmentTextureResource =
                            new PropertyExpressionResolver(createCameraPersp.EnvironmentTexture).Resolve<Texture3DResource>();
                        component.EnvironmentTextureResource = environmentTextureResource;
                    }
                    else
                    {
                        IRenderFrameBuffer geometryFrameBuffer = GameEngine.RenderEngine.CreateRenderFrameBuffer();
                        geometryFrameBuffer.Name = "geometry";
                        geometryFrameBuffer.ShaderProgram = defaultFboShaderProgram;
                        component.InsertFrameBufferAt(geometryFrameBuffer, 0);
                    }

                    IRenderFrameBuffer cameraFrameBuffer = GameEngine.RenderEngine.CreateRenderFrameBuffer();
                    cameraFrameBuffer.Name = "camera";
                    cameraFrameBuffer.ShaderProgram = defaultFboShaderProgram;

                    component.AddFrameBuffer(cameraFrameBuffer);

                    return castedComponent;
                }

                case CreateAnimatedMaterialComponentExpression createAnimatedMaterialComponentExpression:
                {
                    AnimatedMaterialResource template = new PropertyExpressionResolver(expression.Template).Resolve<AnimatedMaterialResource>();
                    IComponent component = CreateComponent(ComponentType.AnimatedMaterial, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createAnimatedMaterialComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    return castedComponent;
                }

                case CreatePbrMaterialComponentExpression createPbrMaterialComponentExpression:
                {
                    PbrMaterialResource template = new PropertyExpressionResolver(expression.Template).Resolve<PbrMaterialResource>();
                    IComponent component = CreateComponent(ComponentType.PbrMaterial, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createPbrMaterialComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    return castedComponent;
                }

                case CreateScriptComponentExpression createScript:
                {
                    ScriptResource template = new PropertyExpressionResolver(expression.Template).Resolve<ScriptResource>();
                    IScript component = (IScript) CreateComponent(ComponentType.Script, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createScript.Name;
                    target.AttachComponent(castedComponent);

                    if (createScript.Injections == null || createScript.Injections.Count < 1) return castedComponent;

                    component.Injections = createScript.Injections.Where(inj => inj.Id != "self").ToList();

                    return castedComponent;
                }

                case CreateShaderProgramComponentExpression createShaderProgramComponentExpression:
                {
                    ShaderProgramResource template = new PropertyExpressionResolver(expression.Template).Resolve<ShaderProgramResource>();
                    IComponent component = CreateComponent(ComponentType.ShaderProgram, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createShaderProgramComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    return castedComponent;
                }

                case CreateAnimatedSpriteComponentExpression createAnimatedSprite:
                {
                    IAnimatedSprite component = (IAnimatedSprite) CreateComponent(ComponentType.AnimatedSprite);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createAnimatedSprite.Name;
                    target.AttachComponent(castedComponent);

                    if (createAnimatedSprite.Material != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createAnimatedSprite.Material);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IAnimatedMaterial material)
                        {
                            if (material.Parent == null)
                            {
                                material.SetParent(component.Parent);
                            }
                            component.Material = material;
                        }
                    }

                    if (createAnimatedSprite.ShaderProgram != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createAnimatedSprite.ShaderProgram);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IShaderProgram shaderProgram)
                        {
                            if (shaderProgram.AsParentable<IGameObject>().Parent == null && shaderProgram.AsParentable<IScene>().Parent == null)
                            {
                                shaderProgram.SetParent(component.Parent);
                            }
                            component.ShaderProgram = shaderProgram;
                        }
                    }

                    if (createAnimatedSprite.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createAnimatedSprite.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddDrawable(component);
                        }
                    }

                    if (createAnimatedSprite.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createAnimatedSprite.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createAnimatedSprite.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createAnimatedSprite.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createAnimatedSprite.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createAnimatedSprite.Transform.Scale ?? Vector3.One
                        );
                    }

                    return castedComponent;
                }

                case CreateRenderTargetSpriteComponentExpression createRenderTargetSprite:
                {
                    IRenderTargetSprite component = (IRenderTargetSprite) CreateComponent(ComponentType.RenderTargetSprite);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createRenderTargetSprite.Name;
                    target.AttachComponent(castedComponent);

                    if (createRenderTargetSprite.FrameBuffer != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createRenderTargetSprite.FrameBuffer);
                        object result = resolver.Resolve(component.Parent);
                        switch (result)
                        {
                            case IRenderFrameBuffer fbo:
                                component.SourceFrameBuffer = fbo;
                                break;
                            case ByReference<IRenderFrameBuffer> fboRef:
                                component.SourceFrameBufferByRef = fboRef;
                                break;
                        }
                    }

                    if (createRenderTargetSprite.ShaderProgram != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createRenderTargetSprite.ShaderProgram);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IShaderProgram shaderProgram)
                        {
                            if (shaderProgram.AsParentable<IGameObject>().Parent == null && shaderProgram.AsParentable<IScene>().Parent == null)
                            {
                                shaderProgram.SetParent(component.Parent);
                            }
                            component.ShaderProgram = shaderProgram;
                        }
                    }

                    if (createRenderTargetSprite.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createRenderTargetSprite.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddDrawable(component);
                        }
                    }

                    if (createRenderTargetSprite.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createRenderTargetSprite.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createRenderTargetSprite.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createRenderTargetSprite.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createRenderTargetSprite.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createRenderTargetSprite.Transform.Scale ?? Vector3.One
                        );
                    }

                    component.MaintainAspectRatio = createRenderTargetSprite.MaintainAspectRatio;
                    return castedComponent;
                }

                case CreateTextSpriteComponentExpression createTextSprite:
                {
                    FontResource template = new PropertyExpressionResolver(expression.Template).Resolve<FontResource>();
                    ITextSprite component = (ITextSprite) CreateComponent(ComponentType.TextSprite, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createTextSprite.Name;
                    target.AttachComponent(castedComponent);

                    if (createTextSprite.ShaderProgram != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createTextSprite.ShaderProgram);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IShaderProgram shaderProgram)
                        {
                            if (shaderProgram.AsParentable<IGameObject>().Parent == null && shaderProgram.AsParentable<IScene>().Parent == null)
                            {
                                shaderProgram.SetParent(component.Parent);
                            }
                            component.ShaderProgram = shaderProgram;
                        }
                    }

                    if (createTextSprite.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createTextSprite.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddDrawable(component);
                        }
                    }

                    component.Text = Regex.Unescape(createTextSprite.Text);

                    TextSpriteAlignment alignment = createTextSprite.Alignment;

                    if (alignment.HasFlag(TextSpriteAlignment.HorizontalLeft))
                    {
                        component.AlignmentHorizontal = TextAlignmentHorizontal.Left;
                    }
                    else if (alignment.HasFlag(TextSpriteAlignment.HorizontalCenter))
                    {
                        component.AlignmentHorizontal = TextAlignmentHorizontal.Center;
                    }
                    else if (alignment.HasFlag(TextSpriteAlignment.HorizontalRight))
                    {
                        component.AlignmentHorizontal = TextAlignmentHorizontal.Right;
                    }
                    else
                    {
                        component.AlignmentHorizontal = TextAlignmentHorizontal.Left;
                    }

                    if (alignment.HasFlag(TextSpriteAlignment.VerticalTop))
                    {
                        component.AlignmentVertical = TextAlignmentVertical.Top;
                    }
                    else if (alignment.HasFlag(TextSpriteAlignment.VerticalCenter))
                    {
                        component.AlignmentVertical = TextAlignmentVertical.Center;
                    }
                    else if (alignment.HasFlag(TextSpriteAlignment.VerticalBottom))
                    {
                        component.AlignmentVertical = TextAlignmentVertical.Bottom;
                    }
                    else
                    {
                        component.AlignmentVertical = TextAlignmentVertical.Top;
                    }

                    component.Color = createTextSprite.Color;
                    component.CharacterSeparation = createTextSprite.CharacterSeparation;
                    component.WhiteSpaceSeparation = createTextSprite.WhiteSpaceSeparation;
                    component.FontSize = createTextSprite.FontSize;

                    if (createTextSprite.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createTextSprite.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createTextSprite.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createTextSprite.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createTextSprite.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createTextSprite.Transform.Scale ?? Vector3.One
                        );
                    }

                    return castedComponent;
                }

                case CreateMeshComponentExpression createMesh:
                {
                    MeshResource template = new PropertyExpressionResolver(expression.Template).Resolve<MeshResource>();
                    IMesh component = (IMesh) CreateComponent(ComponentType.Mesh, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createMesh.Name;
                    target.AttachComponent(castedComponent);

                    if (createMesh.Material != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createMesh.Material);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IPbrMaterial material)
                        {
                            if (material.Parent == null)
                            {
                                material.SetParent(component.Parent);
                            }
                            component.Material = material;
                        }
                    }

                    if (createMesh.ShaderProgram != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createMesh.ShaderProgram);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is IShaderProgram shaderProgram)
                        {
                            if (shaderProgram.AsParentable<IGameObject>().Parent == null && shaderProgram.AsParentable<IScene>().Parent == null)
                            {
                                shaderProgram.SetParent(component.Parent);
                            }
                            component.ShaderProgram = shaderProgram;
                        }
                    }

                    if (createMesh.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createMesh.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddDrawable(component);
                        }
                    }

                    return castedComponent;
                }

                case CreateSoundComponentExpression createSound:
                {
                    SoundResource template = new PropertyExpressionResolver(expression.Template).Resolve<SoundResource>();
                    ISound component = (ISound) CreateComponent(ComponentType.Sound, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createSound.Name;
                    target.AttachComponent(castedComponent);

                    if (createSound.Transform != null)
                    {
                        Object.Transform.TransformMode mode = createSound.Transform.Mode switch
                        {
                            TransformMode.NotSet => Object.Transform.TransformMode.Absolute,
                            TransformMode.Absolute => Object.Transform.TransformMode.Absolute,
                            TransformMode.WorldSpace => Object.Transform.TransformMode.WorldSpace,
                            TransformMode.OrthoRelative => Object.Transform.TransformMode.OrthoRelative,
                            _ => Object.Transform.TransformMode.Absolute
                        };

                        Quaternion rotation = Quaternion.Identity;
                        if (createSound.Transform.Rotation != null)
                        {
                            rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(createSound.Transform.Rotation));
                        }

                        component.MoveInMode(
                            mode,
                            createSound.Transform.Position ?? Vector3.Zero,
                            rotation,
                            createSound.Transform.Scale ?? Vector3.One
                        );
                    }

                    component.AutoPlayOnActive = createSound.AutoPlayOnActive;

                    return castedComponent;
                }

                case CreateSoundBankComponentExpression createSoundBankComponentExpression:
                {
                    SoundBankResource template = new PropertyExpressionResolver(expression.Template).Resolve<SoundBankResource>();
                    IComponent component = CreateComponent(ComponentType.SoundBank, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createSoundBankComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    return castedComponent;
                }

                // ToDo: Refactor collider resource.
                case CreateRigidBodyComponentExpression createRigidBodyComponentExpression:
                {
                    Collider2DResource template = new PropertyExpressionResolver(expression.Template).Resolve<Collider2DResource>();
                    IRigidBody component = (IRigidBody) CreateComponent(ComponentType.RigidBody, template);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createRigidBodyComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    return castedComponent;
                }

                case CreateLightPointComponentExpression createLightPointComponentExpression:
                {
                    IPointLight component = (IPointLight) CreateComponent(ComponentType.LightPoint);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createLightPointComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    if (createLightPointComponentExpression.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createLightPointComponentExpression.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddLight(component);
                        }
                    }

                    component.Strength = createLightPointComponentExpression.Strength;
                    component.Color = new Vector3(
                        createLightPointComponentExpression.Color.R,
                        createLightPointComponentExpression.Color.G,
                        createLightPointComponentExpression.Color.B
                    );

                    component.Radius = createLightPointComponentExpression.Radius;

                    return castedComponent;
                }

                case CreateLightDirectionalComponentExpression createLightDirectionalComponentExpression:
                {
                    IDirectionalLight component = (IDirectionalLight) CreateComponent(ComponentType.LightDirectional);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createLightDirectionalComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    if (createLightDirectionalComponentExpression.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createLightDirectionalComponentExpression.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddLight(component);
                        }
                    }

                    component.Strength = createLightDirectionalComponentExpression.Strength;
                    component.Color = new Vector3(
                        createLightDirectionalComponentExpression.Color.R,
                        createLightDirectionalComponentExpression.Color.G,
                        createLightDirectionalComponentExpression.Color.B
                    );

                    return castedComponent;
                }

                case CreateLightSpotComponentExpression createLightSpotComponentExpression:
                {
                    ISpotLight component = (ISpotLight) CreateComponent(ComponentType.LightSpot);
                    if (!(component is TComponent castedComponent)) return null;

                    castedComponent.Name = createLightSpotComponentExpression.Name;
                    target.AttachComponent(castedComponent);

                    if (createLightSpotComponentExpression.Camera != null)
                    {
                        QueryExpressionResolver resolver = new QueryExpressionResolver(createLightSpotComponentExpression.Camera);
                        object result = resolver.Resolve(component.Parent);
                        if (result != null && result is ICamera camera)
                        {
                            if (((IParentable<IScene>) camera).Parent == null && ((IParentable<IGameObject>) camera).Parent == null)
                            {
                                camera.SetParent(component.Parent);
                            }
                            camera.AddLight(component);
                        }
                    }

                    component.Strength = createLightSpotComponentExpression.Strength;
                    component.Color = new Vector3(
                        createLightSpotComponentExpression.Color.R,
                        createLightSpotComponentExpression.Color.G,
                        createLightSpotComponentExpression.Color.B
                    );

                    component.Radius = createLightSpotComponentExpression.Radius;
                    component.CutoffAngle = createLightSpotComponentExpression.CutoffAngle;

                    return castedComponent;
                }

                default:
                    return null;
            }
        }

        private static IGameObject ResolveCreateGameObject(CreateGameObjectExpression expression, IChildBearer<IGameObject> target)
        {
            PrefabResource template = new PropertyExpressionResolver(expression.Template).Resolve<PrefabResource>();

            PrefabResource runtimePrefab;
            if (template == null)
            {
                runtimePrefab = new PrefabResource
                {
                    Id = $"runtime_prefab_{Guid.NewGuid().ToString()}",
                    Children = new List<IStatementExpression>()
                };

                if (expression.Children != null && expression.Children.Count > 0)
                {
                    runtimePrefab.Children.AddRange(expression.Children);
                }
            }
            else
            {
                if (expression.Children != null && expression.Children.Count > 0)
                {
                    runtimePrefab = new PrefabResource
                    {
                        Id = $"runtime_prefab_{template.Id}",
                        Children = template.Children.ToList()
                    };

                    runtimePrefab.Children.AddRange(expression.Children);
                }
                else
                {
                    runtimePrefab = template;
                }
            }

            IGameObject gameObject = GameEngine.CreateGameObject(runtimePrefab, expression.Name, target);

            if (expression.Transform == null) return gameObject;

            if (expression.Transform.Position != null)
            {
                gameObject.Transform.Position = new Vector3(
                    expression.Transform.Position.X,
                    expression.Transform.Position.Y,
                    expression.Transform.Position.Z
                );
            }

            if (expression.Transform.Rotation != null)
            {
                gameObject.Transform.Rotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(expression.Transform.Rotation));
            }

            if (expression.Transform.Scale != null)
            {
                gameObject.Transform.Scale = new Vector3(
                    expression.Transform.Scale.X,
                    expression.Transform.Scale.Y,
                    expression.Transform.Scale.Z
                );
            }

            return gameObject;
        }

        private static IComponent CreateComponent(ComponentType componentType, IRootResource template = null)
        {
            if (template == null)
            {
                return componentType switch
                {
                    ComponentType.CameraOrtho => (IComponent) GameEngine.CreateComponent<ICameraOrtho>(),
                    ComponentType.CameraPersp => GameEngine.CreateComponent<ICameraPersp>(),
                    ComponentType.AnimatedSprite => GameEngine.CreateComponent<IAnimatedSprite>(),
                    ComponentType.RenderTargetSprite => GameEngine.CreateComponent<IRenderTargetSprite>(),
                    ComponentType.LightPoint => GameEngine.CreateComponent<IPointLight>(),
                    ComponentType.LightDirectional => GameEngine.CreateComponent<IDirectionalLight>(),
                    ComponentType.LightSpot => GameEngine.CreateComponent<ISpotLight>(),
                    _ => null
                };
            }

            IComponent component = GameEngine.CreateComponent(template);
            if (component.IsComponentType(componentType)) return component;

            component.Destroy();
            return null;
        }

        private static void CreatePostProcessingFrameBuffers(IGameStructure target, ICamera camera, PostProcessingPresetResource postProcessingPresetResource)
        {
            foreach (PostProcessingStage postProcessingStage in postProcessingPresetResource.Stages)
            {
                ShaderProgramResource postProcessingShaderProgramResource =
                    new PropertyExpressionResolver(postProcessingStage.ShaderProgram).Resolve<ShaderProgramResource>();
                if (postProcessingShaderProgramResource == null) continue;

                IShaderProgram postProcessingShaderProgram =
                    (IShaderProgram) CreateComponent(ComponentType.ShaderProgram, postProcessingShaderProgramResource);
                if (postProcessingShaderProgram == null) continue;

                if (target is IScene targetScene)
                {
                    postProcessingShaderProgram.SetParent(targetScene);
                }
                else
                {
                    postProcessingShaderProgram.SetParent((IGameObject) target);
                }

                postProcessingShaderProgram.Active = true;

                IRenderFrameBuffer frameBuffer = GameEngine.RenderEngine.CreateRenderFrameBuffer(postProcessingStage);
                frameBuffer.Name = postProcessingStage.Name;
                frameBuffer.ShaderProgram = postProcessingShaderProgram;

                camera.AddFrameBuffer(frameBuffer);
            }
        }
    }
}