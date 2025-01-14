using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.Tween.Exports.Tween
{
    [ScriptExportEnum("TweenEaseType", "Represents the type of ease the tween will update.")]
    public enum TweenEaseType
    {
        [ScriptExportField("Alias for `Linear`.")] None = 0,
        [ScriptExportField("Linear easing.")] Linear = 0,
        [ScriptExportField("Quadratic easing.")] QuadraticIn = 1,
        [ScriptExportField("Quadratic easing.")] QuadraticOut = 2,
        [ScriptExportField("Quadratic easing.")] QuadraticInOut = 3,
        [ScriptExportField("Cubic easing.")] CubicIn = 4,
        [ScriptExportField("Cubic easing.")] CubicOut = 5,
        [ScriptExportField("Cubic easing.")] CubicInOut = 6,
        [ScriptExportField("Quartic easing.")] QuarticIn = 7,
        [ScriptExportField("Quartic easing.")] QuarticOut = 8,
        [ScriptExportField("Quartic easing.")] QuarticInOut = 9,
        [ScriptExportField("Quintic easing.")] QuinticIn = 10,
        [ScriptExportField("Quintic easing.")] QuinticOut = 11,
        [ScriptExportField("Quintic easing.")] QuinticInOut = 12,
        [ScriptExportField("Sinusoidal easing.")] SinusoidalIn = 13,
        [ScriptExportField("Sinusoidal easing.")] SinusoidalOut = 14,
        [ScriptExportField("Sinusoidal easing.")] SinusoidalInOut = 15,
        [ScriptExportField("Exponential easing.")] ExponentialIn = 16,
        [ScriptExportField("Exponential easing.")] ExponentialOut = 17,
        [ScriptExportField("Exponential easing.")] ExponentialInOut = 18,
        [ScriptExportField("Circular easing.")] CircularIn = 19,
        [ScriptExportField("Circular easing.")] CircularOut = 20,
        [ScriptExportField("Circular easing.")] CircularInOut = 21,
        [ScriptExportField("Elastic easing.")] ElasticIn = 22,
        [ScriptExportField("Elastic easing.")] ElasticOut = 23,
        [ScriptExportField("Elastic easing.")] ElasticInOut = 24,
        [ScriptExportField("Back easing.")] BackIn = 25,
        [ScriptExportField("Back easing.")] BackOut = 26,
        [ScriptExportField("Back easing.")] BackInOut = 27,
        [ScriptExportField("Bounce easing.")] BounceIn = 28,
        [ScriptExportField("Bounce easing.")] BounceOut = 29,
        [ScriptExportField("Bounce easing.")] BounceInOut = 30,
        [ScriptExportField("Alias for `Linear`.")] Power0 = 0,
        [ScriptExportField("Alias for `QuadraticOut`.")] Power1 = 2,
        [ScriptExportField("Alias for `CubicOut`.")] Power2 = 5,
        [ScriptExportField("Alias for `QuarticOut`.")] Power3 = 8,
        [ScriptExportField("Alias for `QuinticOut`.")] Power4 = 11
    }
}