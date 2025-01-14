using Android.App;
using Android.Content;

namespace DecayEngine.Fmod.Android
{
    public partial class FmodAndroid : Java.Lang.Object
    {
        public static void LoadRelease(Context context)
        {
            Java.Lang.JavaSystem.LoadLibrary("fmod");
            Java.Lang.JavaSystem.LoadLibrary("fmodstudio");

            Init(context);
        }

        public static void LoadDebug(Context context)
        {
            Java.Lang.JavaSystem.LoadLibrary("fmodL");
            Java.Lang.JavaSystem.LoadLibrary("fmodstudioL");

            Init(context);
        }

        public static void Close()
        {
            CloseInternal();
        }
    }
}