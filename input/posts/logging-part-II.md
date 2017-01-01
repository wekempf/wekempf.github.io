---
Title:  "Logging - Part II"
Published: 2017-01-01 00:00:00
Tags: ["C#", ".NET", "Logging", "Microsoft.Extensions.Logging"]
---

# Logging

## Part II

In [Part I](/posts/logging-part-I) we were introduced to [Microsoft.Extensions.Logging](https://github.com/aspnet/Logging)
which I amusingly abbreviate to just MEL. In particular, we explored the design of the `ILogger` interface.
In this post we'll continue to explore MEL by looking next at the design of the `ILoggerFactory` interface.

### ILoggerFactory

The `ILoggerFactory` type is used to create `ILogger` instances. Here it is:

```csharp
/// <summary>
/// Represents a type used to configure the logging system and create instances of <see cref="ILogger"/> from
/// the registered <see cref="ILoggerProvider"/>s.
/// </summary>
public interface ILoggerFactory : IDisposable
{
    /// <summary>
    /// Creates a new <see cref="ILogger"/> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The <see cref="ILogger"/>.</returns>
    ILogger CreateLogger(string categoryName);

    /// <summary>
    /// Adds an <see cref="ILoggerProvider"/> to the logging system.
    /// </summary>
    /// <param name="provider">The <see cref="ILoggerProvider"/>.</param>
    void AddProvider(ILoggerProvider provider);
}
```

This one is even simpler than `ILogger`, with only two methods. The `AddProvider` method is what you use to
control where log events go, but I'm not going to cover that in this post. I'll probably cover it in a future
post. For now, let's focus on `CreateLogger` instead.

The `CreateLogger` method is called, not surprisingly, to create an `ILogger` instance. Depending on the
logging framework you're used to using, you may be surprised at the `categoryName` parameter it requires,
however. Some, but not all, logging frameworks allow you to create loggers that log with different
"categories" to allow you to control filtring at a very fine grained level. For instance, if you know
you have a bug in `SomeClass` you could turn logging up to include `Debug` level events only for the
"category" that `SomeClass` logs to. Often these categories are based on a `Type` name (the full name,
including the namespace). MEL makes it easy to do the same thing by providing some extension methods.

```cscript
/// <summary>
/// ILoggerFactory extension methods for common scenarios.
/// </summary>
public static class LoggerFactoryExtensions
{
    /// <summary>
    /// Creates a new ILogger instance using the full name of the given type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="factory">The factory.</param>
    public static ILogger<T> CreateLogger<T>(this ILoggerFactory factory)
    {
        // ..
    }

    /// <summary>
    /// Creates a new ILogger instance using the full name of the given type.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="type">The type.</param>
    public static ILogger CreateLogger(this ILoggerFactory factory, Type type)
    {
        // ..
    }
}
```

There's a concrete `LoggerFactory` instance that implements `ILoggerFactory`. So, putting everything
we've learned so far together, here's a simple console application that logs a single message
to standard output.

```csharp
using Microsoft.Extensions.Logging;
using System;

namespace LoggingPlay
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new LoggerFactory()
                .AddConsole();
            var logger = factory.CreateLogger<Program>();
            logger.LogInformation("Started at {Time}.", DateTime.Now);
        }
    }
}
```

> Note: To use this code you'll also have to include the NuGet packages for `Microsoft.Extensions.Logging`
> and `Microsoft.Extensions.Logging.Console`.

Most of this should be familiar to you at this point. The `AddConsole` method may not make any sense to
you yet. This is actually an extension method found in `Microsoft.Extensions.Logging.Console`. it
simply creates a "provider" that writes log events to standard output and adds it by calling
`ILoggerFactory.AddProvider`.

When you run this, the following is produced.

```text
info: LoggingPlay.Program[0]
      Started at 01/01/2017 12:00:25.
```

The output is based on the following:

* "info" is the log level.
* "LoggingPlay.Program" is the category name, derived from the generic type parameter used in the
  `CreateLogger` call.
* "[0]" is the event ID (we didn't specify one when we called `LogInformation` so it defaulted to 0).
* "Started at 01/01/2017 12:00:25" is the fomratted message generated from the template and parameters
  we passed to `LogInformation`.

Some providers allow you to configure the output format, but as far as I know the console provider does not.
Each provider will be unique and you'll have to learn how to use it on your own. I'll cover at
least the basics of some of them in the future. Remember, I'm not giving you a tutorial here,
but rather I'm exploring the API design.

While you're not forced into any specific usage pattern here, typically your application should follow
a simple pattern. You'll have a single `LoggerFactory` instance for the entire application, which
you add providers to at startup. Then, in every class that will do some logging you'll include an
`ILogger` instance created by calling `CreateInstance<T>`. It's a really good idea to do all of this
through Dependency Injection. We know enough now to add the `LoggerFactory` to the DI container of
your choice and inject `ILoggerFactory` into the classes that will create log events. This will
work, but it does seem strange and a bit of a chore to inject an `ILoggerFactory` instead of an
`ILogger`. Some DI containers would allow you to do some clever configuration to allow injecting
an `ILogger` created with the Type that the logger is being injected into, but not all containers
can accomplish this. MEL's design actually considers this, which is something we'll explore
in Part III.