## Assets

Any raw assets you want to be deployed with your application can be placed in this directory (and child directories) and given a Build Action of `AndroidAsset`.

These files will be deployed with you package and will be accessible using Android's `AssetManager`, like this:

```c#
public class ReadAsset : Activity
{
    protected override void OnCreate (Bundle bundle)
    {
        base.OnCreate (bundle);

        InputStream input = Assets.Open ("my_asset.txt");
    }
}
```

Additionally, some Android functions will automatically load asset files:

```c#
Typeface tf = Typeface.CreateFromAsset (Context.Assets, "fonts/samplefont.ttf");
```
