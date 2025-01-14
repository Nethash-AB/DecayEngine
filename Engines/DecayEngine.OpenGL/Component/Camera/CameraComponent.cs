using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Exports.BaseExports.Camera;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.OpenGL.Component.Camera
{
    public abstract class CameraComponent : ICamera
    {
        private IGameObject _parentGameObject;
        private IScene _parentScene;

        private bool _active;
        private Transform _transform;
        private readonly List<IDrawable> _drawables;
        private readonly List<ILight> _lights;
        private readonly List<IFrameBuffer> _frameBuffers;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parentGameObject;
        public ByReference<IGameObject> ParentByRef => () => ref _parentGameObject;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        public virtual Type ExportType => typeof(CameraExport);

        public bool Active
        {
            get
            {
                if (_parentScene == null && _parentGameObject == null)
                {
                    return false;
                }

                return _active;
            }
            set
            {
                if (!Active && value)
                {
                    _frameBuffers.ForEach(fbo =>
                    {
                        fbo.Size = new Vector2(Transform.Scale.X, Transform.Scale.Y);
                        fbo.Active = true;
                    });

                    UpdateCameraProperties();

                    _active = true;
                }
                else if (Active && !value)
                {
                    _frameBuffers.ForEach(fbo => fbo.Active = false);

                    _active = false;
                }
            }
        }

        public Transform Transform => _transform;
        public ByReference<Transform> TransformByRef => () => ref _transform;
        public Transform WorldSpaceTransform => this.GetWorldSpaceTransform();

        public bool RenderToScreen { get; set; }
        public bool ManualSize { get; set; }
        public float ZNear { get; set; }
        public float ZFar { get; set; }

        public IDebugDrawer DebugDrawer { get; set; }
        public IEnumerable<IDrawable> Drawables => _drawables;
        public IEnumerable<ILight> Lights => _lights;
        public IEnumerable<IFrameBuffer> FrameBuffers => _frameBuffers;

        public bool Persistent { get; set; }

        protected CameraComponent()
        {
            _transform = new Transform();
            _drawables = new List<IDrawable>();
            _lights = new List<ILight>();
            _frameBuffers = new List<IFrameBuffer>();
        }

        ~CameraComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveComponent(this);

            _parentScene?.RemoveComponent(this);
            _parentScene = null;

            parent?.AttachComponent(this);
            _parentGameObject = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        void IParentable<IScene>.SetParent(IScene parent)
        {
            _parentScene?.RemoveComponent(this);

            _parentGameObject?.RemoveComponent(this);
            _parentGameObject = null;

            parent?.AttachComponent(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
        }

        public void AddDrawable(IDrawable drawable)
        {
            if (!_drawables.Contains(drawable))
            {
                _drawables.Add(drawable);
            }
        }

        public void RemoveDrawable(IDrawable drawable)
        {
            if (_drawables.Contains(drawable))
            {
                _drawables.Remove(drawable);
            }
        }

        public void AddLight(ILight light)
        {
            if (!_lights.Contains(light))
            {
                _lights.Add(light);
            }
        }

        public void RemoveLight(ILight light)
        {
            if (_lights.Contains(light))
            {
                _lights.Remove(light);
            }
        }

        public void AddFrameBuffer(IFrameBuffer frameBuffer)
        {
            if (!_frameBuffers.Contains(frameBuffer))
            {
                _frameBuffers.Add(frameBuffer);
            }
        }

        public void InsertFrameBufferAt(IFrameBuffer frameBuffer, uint position)
        {
            if (!_frameBuffers.Contains(frameBuffer))
            {
                _frameBuffers.Insert((int) position, frameBuffer);
            }
        }

        public void RemoveFrameBuffer(IFrameBuffer frameBuffer)
        {
            if (_frameBuffers.Contains(frameBuffer))
            {
                _frameBuffers.Remove(frameBuffer);
            }
        }

        public void RemoveFrameBufferAt(uint position)
        {
            if (position >= _frameBuffers.Count)
            {
                position = _frameBuffers.Count > 0 ? (uint) _frameBuffers.Count - 1 : 0u;
            }

            _frameBuffers.RemoveAt((int) position);
        }

        public virtual void UpdateCameraProperties()
        {
        }

        public virtual void Destroy()
        {
            foreach (IFrameBuffer frameBuffer in _frameBuffers)
            {
                frameBuffer.Destroy();
            }
            _frameBuffers.Clear();

            SetParent(null);

            _transform = null;
            _drawables.Clear();

            Destroyed = true;
        }

        public void DrawGeometry()
        {
            foreach (IFrameBuffer frameBuffer in _frameBuffers)
            {
                frameBuffer.Size = new Vector2(Transform.Scale.X, Transform.Scale.Y);
            }

            // TEST DEBUG
            if (DebugDrawer != null)
            {
                DebugDrawer.Active = GameEngine.RenderEngine.DrawDebug;
            }
            // TEST DEBUG

            GetCameraMatrices(out Matrix4 viewMatrix, out Matrix4 projectionMatrix);

            List<IDrawable> drawables = Drawables
                .Where(d => d != null && d.ActiveInGraph() && (!(d is IDestroyable destroyable) || !destroyable.Destroyed))
                .OrderBy(component => Vector3.DistanceSquared(WorldSpaceTransform.Position, component.WorldSpaceTransform.Position))
                .ToList();

            if (this is ICameraPersp cameraPersp)
            {
                IEnumerable<IDrawable> pbrDrawables = drawables.TakeWhile(drawable => !drawable.IsPbrCapable);
                foreach (IDrawable pbrDrawable in pbrDrawables)
                {
                    GameEngine.LogAppendLine(LogSeverity.Warning, "OpenGL",
                        $"Tried to render non-pbr capable drawable ({pbrDrawable}) with a PBR camera ({Name})");
                }
            }
//            if (drawables.Count < 1) return;

            DrawCameraDrawables(drawables, viewMatrix, projectionMatrix, true);
        }

        private void DrawCameraDrawables(IReadOnlyCollection<IDrawable> drawables, Matrix4 viewMatrix, Matrix4 projectionMatrix, bool drawDebug)
        {
            if (_frameBuffers.Count > 0)
            {
                List<IFrameBuffer> activeFrameBuffers = _frameBuffers.Where(fbo => fbo.Active).ToList();
                if (activeFrameBuffers.Count > 0)
                {
                    if (activeFrameBuffers.Count > 1)
                    {
                        for (int i = 0; i < activeFrameBuffers.Count; i++)
                        {
                            IFrameBuffer currentFbo = activeFrameBuffers[i];

                            if (i < 1)
                            {
                                currentFbo.Bind(true);
                                GameEngine.RenderEngine.DepthTestEnabled = true;

                                foreach (IDrawable drawable in drawables)
                                {
                                    drawable.Draw(viewMatrix, projectionMatrix);
                                }

                                currentFbo.Unbind();
                            }

                            if (i + 1 < activeFrameBuffers.Count)
                            {
                                IFrameBuffer nextFbo = activeFrameBuffers[i + 1];

                                nextFbo.Bind(true);
                                GameEngine.RenderEngine.DepthTestEnabled = false;

                                currentFbo.Draw(this, viewMatrix, projectionMatrix);

                                nextFbo.Unbind();
                            }
                            else
                            {
                                if (!drawDebug || DebugDrawer == null || !DebugDrawer.Active) continue;

                                currentFbo.Bind(false);

                                if (!(this is ICameraOrtho))
                                {
                                    IFrameBuffer geometryFbo = activeFrameBuffers[0];
                                    geometryFbo.CopyDepthBuffer(currentFbo);
                                    GameEngine.RenderEngine.DepthTestEnabled = true;
                                }

                                DrawDebug(drawables, viewMatrix, projectionMatrix);

                                currentFbo.Unbind();
                            }
                        }
                    }
                    else
                    {
                        IFrameBuffer fbo = activeFrameBuffers.First();

                        fbo.Bind(true);
                        GameEngine.RenderEngine.DepthTestEnabled = true;

                        foreach (IDrawable drawable in drawables)
                        {
                            drawable.Draw(viewMatrix, projectionMatrix);
                        }
                        if (drawDebug && DebugDrawer != null && DebugDrawer.Active)
                        {
                            DrawDebug(drawables, viewMatrix, projectionMatrix);
                        }

                        fbo.Unbind();
                    }
                }
            }
        }

        private void DrawDebug(IReadOnlyCollection<IDrawable> drawables, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (GameEngine.RenderEngine.DrawDebugOriginCrosshair)
            {
                DebugDrawer.AddPlanarGrid(
                    Vector3.Zero,
                    Quaternion.Identity,
                    Vector3.One,
                    (int) ZFar
                );

                DebugDrawer.AddPlanarGrid(
                    Vector3.Zero,
                    Quaternion.FromEulerAngles(
                        MathHelper.DegreesToRadians(90f),
                        MathHelper.DegreesToRadians(0f),
                        MathHelper.DegreesToRadians(0f)
                    ),
                    Vector3.One,
                    (int) ZFar
                );

                DebugDrawer.AddPlanarGrid(
                    Vector3.Zero,
                    Quaternion.FromEulerAngles(
                        MathHelper.DegreesToRadians(90f),
                        MathHelper.DegreesToRadians(90f),
                        MathHelper.DegreesToRadians(0f)
                    ),
                    Vector3.One,
                    (int) ZFar
                );

                DebugDrawer.AddCrosshair(Vector3.Zero, Quaternion.Identity, Vector3.One);

//                GameEngine.RenderEngine.DepthTestEnabled = true;
                DebugDrawer.Draw(viewMatrix, projectionMatrix);
            }

            foreach (IDrawable drawable in drawables)
            {
                if (!(drawable is IDebugDrawable debugDrawable)) continue;

                debugDrawable.DrawDebug(viewMatrix, projectionMatrix, DebugDrawer);
            }

            List<IGameObject> cameraGameObjects = drawables.Where(d => d is IParentable<IGameObject> goParentable && goParentable.Parent != null)
                .Select(d => ((IParentable<IGameObject>) d).Parent).Distinct().ToList();

            cameraGameObjects.AddRange(_lights.Where(l => l.Active && l.Parent != null && l.Parent.Active).Select(l => l.Parent));

            foreach (IGameObject cameraGameObject in cameraGameObjects)
            {
                Transform goWorldSpaceTransform = cameraGameObject.WorldSpaceTransform;
                if (GameEngine.RenderEngine.DrawDebugNames)
                {
                    DebugDrawer.AddText(
                        goWorldSpaceTransform.Position,
                        Transform.Rotation,
                        Vector3.One * 5f,
                        new Vector4(0f, 0f, 1f, 1f),
                        cameraGameObject.Name
                    );
                }
                DebugDrawer.AddCrosshair(goWorldSpaceTransform.Position, goWorldSpaceTransform.Rotation, Vector3.One / 2f);
            }

            DebugDrawer.Draw(viewMatrix, projectionMatrix);
        }

        protected abstract void GetCameraMatrices(out Matrix4 viewMatrix, out Matrix4 projectionMatrix);
    }
}