﻿> [!NOTE]
> Cannot include library for licensing reasons.  

Place `SDL2Droid-CS-Java.jar` here.

----

## JARs

This directory is for Android .jars.
There are 2 types of jars that are supported:

### Input Jar

This is the jar that bindings should be generated for.

For example, if you were binding the Google Maps library, this would be Google's `maps.jar`.  
Set the build action for these jars in the properties page to `InputJar`.


### Reference Jars

These are jars that are referenced by the input jar.  
C# bindings will not be created for these jars.  
These jars will be used to resolve types used by the input jar.  

> [!NOTE]
> Do not add `android.jar` as a reference jar.  
> It will be added automatically based on the Target Framework selected.

Set the build action for these jars in the properties page to `ReferenceJar`.
