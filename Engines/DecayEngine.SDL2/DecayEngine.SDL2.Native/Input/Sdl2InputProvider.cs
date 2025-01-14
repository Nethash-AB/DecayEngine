using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;
using DecayEngine.SDL2.Native.Sdl2Interop;

namespace DecayEngine.SDL2.Native.Input
{
    public class Sdl2InputProvider : IInputProvider<Sdl2InputProviderOptions>
    {
        private readonly SDL.SDL_EventFilter _sdlEventFilterCallbackDelegate;
        private readonly IntPtr[] _joystickHandles;

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public Sdl2InputProvider()
        {
            _sdlEventFilterCallbackDelegate = SdlEventTriggered;

            _joystickHandles = new IntPtr[99];

            ComponentFactories = new List<IComponentFactory>();
            ScriptExports = new ScriptExports();
        }

        public Task Init(Sdl2InputProviderOptions options)
        {
            if (SDL.SDL_WasInit(SDL.SDL_INIT_GAMECONTROLLER) == 0 ||
                SDL.SDL_WasInit(SDL.SDL_INIT_JOYSTICK) == 0 ||
                SDL.SDL_WasInit(SDL.SDL_INIT_HAPTIC) == 0)
            {
                throw new Exception("Tried to use SDL Input Provider but SDL is not initialized.");
            }

            for (int i = 0; i < SDL.SDL_NumJoysticks(); i++)
            {
                if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE)
                {
                    _joystickHandles[i] = SDL.SDL_GameControllerOpen(i);
                }
                else
                {
                    _joystickHandles[i] = SDL.SDL_JoystickOpen(i);
                }
            }

            SDL.SDL_AddEventWatch(_sdlEventFilterCallbackDelegate, IntPtr.Zero);

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            try
            {
                SDL.SDL_DelEventWatch(_sdlEventFilterCallbackDelegate, IntPtr.Zero);
            }
            catch
            {
                //
            }

            return Task.CompletedTask;
        }

        private int SdlEventTriggered(IntPtr userdata, IntPtr sdlevent)
        {
            SDL.SDL_Event e = Marshal.PtrToStructure<SDL.SDL_Event>(sdlevent);

            return e.type switch
            {
                SDL.SDL_EventType.SDL_KEYDOWN => HandleKeyboard(e.key),
                SDL.SDL_EventType.SDL_KEYUP => HandleKeyboard(e.key),
                SDL.SDL_EventType.SDL_FINGERDOWN => 0,
                SDL.SDL_EventType.SDL_FINGERUP => 0,
                SDL.SDL_EventType.SDL_FINGERMOTION => 0,
                SDL.SDL_EventType.SDL_MOUSEWHEEL => 0,
                SDL.SDL_EventType.SDL_JOYBUTTONDOWN => 0,
                SDL.SDL_EventType.SDL_JOYBUTTONUP => 0,
                SDL.SDL_EventType.SDL_JOYHATMOTION => 0,
                SDL.SDL_EventType.SDL_JOYAXISMOTION => 0,
                SDL.SDL_EventType.SDL_JOYBALLMOTION => 0,
                SDL.SDL_EventType.SDL_JOYDEVICEADDED => 0,
                SDL.SDL_EventType.SDL_JOYDEVICEREMOVED => 0,
                SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN => HandleGamePadButton(e.cbutton),
                SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP => HandleGamePadButton(e.cbutton),
                SDL.SDL_EventType.SDL_CONTROLLERAXISMOTION => HandleGamePadAxis(e.caxis),
                SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED => 0,
                SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED => 0,
                SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED => 0,
                SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN => 0,
                SDL.SDL_EventType.SDL_MOUSEBUTTONUP => 0,
                SDL.SDL_EventType.SDL_MOUSEMOTION => 0,
                _ => 1
            };
        }

        private static int HandleKeyboard(SDL.SDL_KeyboardEvent keyboardEvent)
        {
            KeyboardScanCode scancode = Sdl2ScanCodeParser.ParseKey(keyboardEvent.keysym.scancode);
            if (scancode == KeyboardScanCode.Unsupported) return 1;

            bool newState = keyboardEvent.state == SDL.SDL_PRESSED;

            int outVal = 1;
            foreach (KeyValuePair<string, IInputAction> keyValuePair in GameEngine.ActionMap)
            {
                foreach (IKeyboardActionTrigger inputActionTrigger in keyValuePair.Value.Triggers.OfType<IKeyboardActionTrigger>())
                {
                    if (inputActionTrigger.ScanCode != scancode) continue;

                    inputActionTrigger.Value = newState;
                    outVal = 0;
                }
                keyValuePair.Value.Update();
            }

            return outVal;
        }

        private int HandleGamePadButton(SDL.SDL_ControllerButtonEvent controllerButtonEvent)
        {
            GamePadButtonScanCode scancode =
                Sdl2ScanCodeParser.ParseGamePadButton((SDL.SDL_GameControllerButton) controllerButtonEvent.button);
            if (scancode == GamePadButtonScanCode.Unsupported) return 1;

            int gamePadIndex = -1;
            for (int i = 0; i < _joystickHandles.Length; i++)
            {
                IntPtr joystickHandle = _joystickHandles[i];
                if (SDL.SDL_IsGameController(i) != SDL.SDL_bool.SDL_TRUE) continue;
                if (SDL.SDL_GameControllerFromInstanceID(controllerButtonEvent.which) != joystickHandle) continue;

                gamePadIndex = i;
                break;
            }

            if (gamePadIndex < 0)
            {
                return 1;
            }

            bool newState = controllerButtonEvent.state == SDL.SDL_PRESSED;

            int outVal = 1;
            foreach (KeyValuePair<string, IInputAction> keyValuePair in GameEngine.ActionMap)
            {
                foreach (IGamePadButtonActionTrigger inputActionTrigger in keyValuePair.Value.Triggers.OfType<IGamePadButtonActionTrigger>())
                {
                    if (inputActionTrigger.ScanCode != scancode || inputActionTrigger.GamePadIndex != gamePadIndex) continue;

                    inputActionTrigger.Value = newState;
                    outVal = 0;
                }
                keyValuePair.Value.Update();
            }

            return outVal;
        }

        private int HandleGamePadAxis(SDL.SDL_ControllerAxisEvent controllerAxisEvent)
        {
            GamePadAxisScanCode scanCode =
                Sdl2ScanCodeParser.ParseGamePadAxis((SDL.SDL_GameControllerAxis) controllerAxisEvent.axis);
            if (scanCode == GamePadAxisScanCode.Unsupported) return 1;

            int gamePadIndex = -1;
            for (int i = 0; i < _joystickHandles.Length; i++)
            {
                IntPtr joystickHandle = _joystickHandles[i];
                if (SDL.SDL_IsGameController(i) != SDL.SDL_bool.SDL_TRUE) continue;
                if (SDL.SDL_GameControllerFromInstanceID(controllerAxisEvent.which) != joystickHandle) continue;

                gamePadIndex = i;
                break;
            }

            if (gamePadIndex < 0)
            {
                return 1;
            }

            short axisValue = controllerAxisEvent.axisValue;
            float newState = 2f * (axisValue - short.MinValue) / (short.MaxValue - short.MinValue) - 1f;

            int outVal = 1;
            foreach (KeyValuePair<string, IInputAction> keyValuePair in GameEngine.ActionMap)
            {
                foreach (IGamePadAxisActionTrigger inputActionTrigger in keyValuePair.Value.Triggers.OfType<IGamePadAxisActionTrigger>())
                {
                    if (inputActionTrigger.ScanCode != scanCode || inputActionTrigger.GamePadIndex != gamePadIndex) continue;

                    float inputState = newState;
                    if (inputActionTrigger.Invert)
                    {
                        inputState *= -1;
                    }

                    inputActionTrigger.Value = Math.Abs(inputState) <= inputActionTrigger.DeadZone ? 0f : inputState;
                    outVal = 0;
                }
                keyValuePair.Value.Update();
            }

            return outVal;
        }
    }
}