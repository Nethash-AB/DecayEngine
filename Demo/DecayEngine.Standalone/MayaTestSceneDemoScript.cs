using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Coroutines.Results;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.Standalone
{
    public static class MayaTestSceneDemoScript
    {
        private static ICameraPersp _cameraPersp;
        private static ITextSprite _textDrawer;
        private static List<IGameObject> _demoObjects;
        private static IGameObject _lightGo;
        private static List<ILight> _lights;
        private static ISound _music;

        private static IInputAction _pitchAction;
        private static IInputAction _yawAction;
        private static IInputAction _rightAction;
        private static IInputAction _upAction;
        private static IInputAction _forwardAction;

        private static float _pitch;
        private static float _yaw;
        private static int _selectedObject;
        private static int _selectedLight;

        public static void Run()
        {
            _cameraPersp = GameEngine.ActiveScene.Components.OfType<ICameraPersp>().FirstOrDefault();
            _cameraPersp.FieldOfView = MathHelper.DegreesToRadians(75f);

            _demoObjects = GameEngine
                .ActiveScene
                .Children
                .Where(go => go.Name == "test_rr_car" || go.Name == "test_sphere" || go.Name == "test_platform")
                .ToList();

            _textDrawer = GameEngine
                .ActiveScene
                .Children
                .Where(go => go.Name == "debug_text_label")
                .SelectMany(go => go.Components)
                .OfType<ITextSprite>()
                .FirstOrDefault();

            _lightGo = GameEngine
                .ActiveScene
                .Children
                .FirstOrDefault(go => go.Name == "test_light_go");

            _lights = _lightGo.Components.OfType<ILight>().ToList();
            ILight activeLight = _lights.FirstOrDefault(light => light.Active);
            _selectedLight = _lights.IndexOf(activeLight);

            IGameObject musicGo = GameEngine.CreateGameObject((PrefabResource) null, "music_go");
            musicGo.Active = true;
            _music = GameEngine.CreateComponent<ISound>("test_music_sound");
            musicGo.AttachComponent(_music);
            _music.AutoPlayOnActive = false;
            _music.Active = true;

            _pitch = 0f;
            _yaw = 0f;
            _cameraPersp.Transform.Position = new Vector3(0f, 2f, 8f);
            _selectedObject = 0;

            _lightGo.Transform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(180f));

            _pitchAction = GameEngine.CreateInputAction("pitch");
            _pitchAction.AddKeyboardTrigger(KeyboardScanCode.ArrowUp, 1f);
            _pitchAction.AddKeyboardTrigger(KeyboardScanCode.ArrowDown, -1f);
            _pitchAction.AddGamePadAxisTrigger(GamePadAxisScanCode.RightY, 0, 0f, 0.25f, true);

            _yawAction = GameEngine.CreateInputAction("yaw");
            _yawAction.AddKeyboardTrigger(KeyboardScanCode.ArrowRight, -1f);
            _yawAction.AddKeyboardTrigger(KeyboardScanCode.ArrowLeft, 1f);
            _yawAction.AddGamePadAxisTrigger(GamePadAxisScanCode.RightX, 0, 0f, 0.25f, true);

            _rightAction = GameEngine.CreateInputAction("right");
            _rightAction.AddKeyboardTrigger(KeyboardScanCode.D, 1f);
            _rightAction.AddKeyboardTrigger(KeyboardScanCode.A, -1f);
            _rightAction.AddGamePadAxisTrigger(GamePadAxisScanCode.LeftX, 0, 0f, 0.25f, false);

            _upAction = GameEngine.CreateInputAction("up");
            _upAction.AddKeyboardTrigger(KeyboardScanCode.Q, 1f);
            _upAction.AddKeyboardTrigger(KeyboardScanCode.E, -1f);
            _upAction.AddGamePadAxisTrigger(GamePadAxisScanCode.R2, 0, 0f, 0f, false);
            _upAction.AddGamePadAxisTrigger(GamePadAxisScanCode.L2, 0, 0f, 0f, true);


            _forwardAction = GameEngine.CreateInputAction("forward");
            _forwardAction.AddKeyboardTrigger(KeyboardScanCode.W, 1f);
            _forwardAction.AddKeyboardTrigger(KeyboardScanCode.S, -1f);
            _forwardAction.AddGamePadAxisTrigger(GamePadAxisScanCode.LeftY, 0, 0f, 0.25f, true);

            IInputAction resetAction = GameEngine.CreateInputAction("reset");
            resetAction.AddKeyboardTrigger(KeyboardScanCode.Return, 0f);
            resetAction.AddGamePadButtonTrigger(GamePadButtonScanCode.Y, 0, 0f);
            resetAction.OnDigitalDeactivate += action =>
            {
                _pitch = 0f;
                _yaw = 0f;
                _cameraPersp.Transform.Position = new Vector3(0f, 2f, 8f);
            };

            IInputAction selectGoAction = GameEngine.CreateInputAction("selectGo");
            selectGoAction.AddKeyboardTrigger(KeyboardScanCode.Space, 0f);
            selectGoAction.AddGamePadButtonTrigger(GamePadButtonScanCode.A, 0, 0f);
            selectGoAction.OnDigitalDeactivate += action =>
            {
                _demoObjects[_selectedObject].Active = false;
                if (_selectedObject + 1 > _demoObjects.Count - 1)
                {
                    _selectedObject = 0;
                }
                else
                {
                    _selectedObject++;
                }
                _demoObjects[_selectedObject].Active = true;
            };

            IInputAction selectLightAction = GameEngine.CreateInputAction("selectLight");
            selectLightAction.AddKeyboardTrigger(KeyboardScanCode.Tab, 0f);
            selectLightAction.AddGamePadButtonTrigger(GamePadButtonScanCode.X, 0, 0f);
            selectLightAction.OnDigitalDeactivate += action =>
            {
                _lights[_selectedLight].Active = false;
                if (_selectedLight + 1 > _lights.Count - 1)
                {
                    _selectedLight = 0;
                }
                else
                {
                    _selectedLight++;
                }
                _lights[_selectedLight].Active = true;
            };

            IInputAction toggleDebugAction = GameEngine.CreateInputAction("toggleDebug");
            toggleDebugAction.AddKeyboardTrigger(KeyboardScanCode.F5, 0f);
            toggleDebugAction.AddGamePadButtonTrigger(GamePadButtonScanCode.Start, 0, 0f);
            toggleDebugAction.OnDigitalDeactivate += action =>
            {
                GameEngine.RenderEngine.DrawDebug = !GameEngine.RenderEngine.DrawDebug;
            };

            IInputAction toggleMusicAction = GameEngine.CreateInputAction("toggleMusic");
            toggleMusicAction.AddKeyboardTrigger(KeyboardScanCode.F1, 0f);
            toggleMusicAction.OnDigitalDeactivate += action =>
            {
                switch (GameEngine.SoundEngine.MarshalPlaybackStatus(_music.PlaybackStatus))
                {
                    case SoundPlaybackStatus.Playing:
                        _music.Stop(true);
                        break;
                    case SoundPlaybackStatus.Stopped:
                        _music.Play();
                        break;
                }
            };

            _demoObjects[_selectedObject].Active = true;

            _music.Play();
            GameEngine.CreateCoroutine(DrawDebug, CoroutineContext.Render).Run();
            GameEngine.RenderEngine.EngineThread.OnUpdate += RenderUpdate;
        }

        private static void RenderUpdate(double deltaTime)
        {
            const float rotationSpeedFactor = 0.05f;
            _pitch += _pitchAction.AnalogValue * rotationSpeedFactor;
            _yaw += _yawAction.AnalogValue * rotationSpeedFactor;

            Quaternion rotation =
                Quaternion.Identity *
                Quaternion.FromAxisAngle(Vector3.UnitY, _yaw) *
                Quaternion.FromAxisAngle(Vector3.UnitX, _pitch);
//            _testTransform.Rotation = rotation;
            _cameraPersp.Transform.Rotation = rotation;

            const float translationSpeedFactor = 0.5f;
            _cameraPersp.Transform.Position += _cameraPersp.Transform.RotationMatrix.Forward * _forwardAction.AnalogValue * translationSpeedFactor;
            _cameraPersp.Transform.Position += _cameraPersp.Transform.RotationMatrix.Right * _rightAction.AnalogValue * translationSpeedFactor;
            _cameraPersp.Transform.Position += _cameraPersp.Transform.RotationMatrix.Up * _upAction.AnalogValue * translationSpeedFactor;
        }

        private static IEnumerator<IYieldResult> DrawDebug()
        {
            while (_textDrawer != null && _cameraPersp != null)
            {
                IGameObject selectedObject = _demoObjects[_selectedObject];
                ILight selectedLight = _lights[_selectedLight];

                string debugText =
                    $"Selected Object: {selectedObject.Name}" +
                    "\n" +
                    $"Selected Light: {selectedLight.Name}" +
                    "\n" +
                    $"Camera Position: (X: {_cameraPersp.Transform.Position.X:F2}, Y: {_cameraPersp.Transform.Position.Y:F2}, Z: {_cameraPersp.Transform.Position.Z:F2})" +
                    "\n" +
                    $"Camera Rotation: (X: {_cameraPersp.Transform.Rotation.X:F2}, Y: {_cameraPersp.Transform.Rotation.Y:F2}, Z: {_cameraPersp.Transform.Rotation.Z:F2}, W: {_cameraPersp.Transform.Rotation.W:F2})";
                _textDrawer.Text = debugText;

                yield return new WaitForNextTick();
            }

            yield return null;
        }
    }
}