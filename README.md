# nodeWrapper
A wrapper around LibGit2Sharp for NodeJS/Edge, cause no matter how hard I try, I cannot seem to get a native node module to build on windows.

### About
This started as an attempt to wrap only LibGit2Sharp in a way where it could be used from NodeJS via the `edge` module. At this point,
the code is actually capable of converting anything into an edge usable form. You can see it's still focused on LibGit2 if you look at
`Startup.Invoke` where it makes and returns a repo. However, it can easily be modified to provide any entry point into any C# API.

The dictionary is produce such that the keys are in the format `camelCaseMethodName_arg1Type_arg2Type_argXType` so that you can tell which one
you're calling from the JS side and what you need to provide it.

It will also find and provide access to Extension Methods, provided they are in the same assembly as the class it's converting. Since extension
methods are actually Static methods of a different class that takes the class as an argument, these can be identified in JS by the first argument
being the calling type. You do not need to pass that first argument (just skip it completely, don't even pass `null`). It will be provided in the
Invokable proxy method on the C# side.
