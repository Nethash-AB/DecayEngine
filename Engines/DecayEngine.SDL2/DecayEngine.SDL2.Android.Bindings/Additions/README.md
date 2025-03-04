﻿## Additions

Additions allow you to add arbitrary C# to the generated classes
before they are compiled.  This can be helpful for providing convenience
methods or adding pure C# classes.

### Adding Methods to Generated Classes

Let's say the library being bound has a `Rectangle` class with a constructor that takes an x and y position, and a width and length size.  
It will look like this:

```c#
public partial class Rectangle
{
    public Rectangle (int x, int y, int width, int height)
    {
        // JNI bindings
    }
}
```

Imagine we want to add a constructor to this class that takes a `Point` and `Size` structure instead of 4 ints.  
We can add a new file called `Rectangle.cs` with a partial class containing our new method:

```c#
public partial class Rectangle
{
    public Rectangle (Point location, Size size) :
        this (location.X, location.Y, size.Width, size.Height)
    {
    }
}
```

At compile time, the additions class will be added to the generated class and the final assembly will a `Rectangle` class with both constructors.


### Adding C# Classes

Another thing that can be done is adding fully C# managed classes to the generated library.  
In the above example, let's assume that there isn't a `Point` class available in Java or our library.  
The one we create doesn't need to interact with Java, so we'll create it like a normal class in C#.

By adding a `Point.cs` file with this class, it will end up in the binding library:

```c#
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
```
