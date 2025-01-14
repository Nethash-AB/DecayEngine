using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports
{
    [ScriptExportEnum("ManagedObjectType", "Represents the managed types wrapped by `IManagedWrapper`.")]
    public enum ManagedExportType
    {
        [ScriptExportField("Camera Component. Includes all projection types.")]
        Camera = 0,
        [ScriptExportField("Texture Component.")]
        Texture = 1,
        [ScriptExportField("Material Component.")]
        Material = 2,
        [ScriptExportField("Shader Component.")]
        Shader = 3,
        [ScriptExportField("ShaderProgram Component.")]
        ShaderProgram = 4,
        [ScriptExportField("SoundBank Component.")]
        SoundBank = 5,
        [ScriptExportField("Sound Component.")]
        Sound = 6,
        [ScriptExportField("Script Component.")]
        Script = 7,
        [ScriptExportField("Sprite2D Component.")]
        Sprite2D = 8,
        [ScriptExportField("TextSprite Component.")]
        TextSprite = 9,
        [ScriptExportField("RigidBody Component.")]
        RigidBody = 10,
        [ScriptExportField("Transform Object.")]
        Transform = 11,
        [ScriptExportField("GameObject Object.")]
        GameObject = 12,
        [ScriptExportField("Scene Object.")]
        Scene = 13,
        [ScriptExportField("EventHandler Object.")]
        EventHandler = 14,
        [ScriptExportField("Vector2 Object.")]
        Vector2 = 15,
        [ScriptExportField("Vector3 Object.")]
        Vector3 = 16,
        [ScriptExportField("Vector4 Object.")]
        Vector4 = 17,
        [ScriptExportField("Quaternion Object.")]
        Quaternion = 18,
        [ScriptExportField("Aabb Object.")]
        Aabb = 19,
        [ScriptExportField("CollisionData Object.")]
        CollisionData = 20,
        [ScriptExportField("Tween Component.")]
        Tween = 21,
        [ScriptExportField("Input Action.")]
        InputAction = 22,
        [ScriptExportField("Input Action Digital Trigger.")]
        InputActionTriggerDigital = 23,
        [ScriptExportField("Input Action Analog Trigger.")]
        InputActionTriggerAnalog = 24
    }
}