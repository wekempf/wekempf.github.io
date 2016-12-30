---
Title:  "Logging - Part I"
Published:   2016-12-28 09:00:00
Tags: ["C#", ".NET", "Logging", "Microsoft.Extensions.Logging"]
---

# Logging

## Part I

We've all dealt with logging in our projects before. This is a mostly known commodity for every developer.
So, why am I blogging about it? Well, I think there's something to be learned about the finer points of
logging, and there's a new kid on the block: [Microsoft.Extensions.Logging](https://github.com/aspnet/Logging).

Why another logging framework? Really, aren't there enough choices already? Well, I think MEL (hey, I just
coined that, and kinda like it) is a little different. See, first and foremost, MEL is a logging abstraction,
not a logging framework. It's main intent is to provide interfaces that act as a facade over any other
logging framework. Technically, any logging framework that exposes interfaces could make this claim, since
pretty much all of them provide a way to plug in "listeners", "providers" or some other conceptual name
for a type that controls how and where to log. By creating one of these you can pretty much bridge from
one logging framework to another, and MEL isn't much different in this regard. Where MEL differs, however
is in the fact that the design was **intended** to be used in this fashion. Its logging abstractions are
generic and fully intended to allow adapting to any other framework. Other projects, such as
[common-logging](https://github.com/net-commons/common-logging) have attempted to do the same thing,
but I really think MEL has done the best job here. It's simple, elegant, and provides all the features
you'd want in a way that doesn't require the underlying frameworks to have to be "bent" to work with it.

That's a pretty bold claim. I'm going to spend a few posts here going over MEL's design to try and back
up that claim. A caveat, though. I'm new to MEL, and not a contributor to it. MEL is also, at the moment,
underdocumented and not talked much about, so this will actually be one of the few blog posts on the
topic available at the time I post it. This means these posts are written by someone that doesn't know much more
about the subject than you probably do. Expect some mistakes to be made. Share with me in the comments
any mistakes I've made or differences of opinion you have, and I'll likely address it either with corrections
to individual posts, or with entirely new posts on the subject.

### ILogger

So, I'm writing about this subject from the point of view of the design, not as a tutorial or reference.
You'll learn some things about using MEL, I hope, but it may take a bit longer to get there because of
this.

Given that, the first place I'm going to start is with talking about the design of the main abstraction:
the `ILogger` interface. Here it is, in it's full glory.

```csharp
/// <summary>
/// Represents a type used to perform logging.
/// </summary>
/// <remarks>Aggregates most logging patterns to a single method.</remarks>
public interface ILogger
{
    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state"/> and <paramref name="exception"/>.</param>
    void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);

    /// <summary>
    /// Checks if the given <paramref name="logLevel"/> is enabled.
    /// </summary>
    /// <param name="logLevel">level to be checked.</param>
    /// <returns><c>true</c> if enabled.</returns>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    IDisposable BeginScope<TState>(TState state);
}
```

The first thing I'd like to point out about the design here is the minimalism of it. Three little
methods, no more, no less. Many of the other abstractions and logging frameworks include a half
dozen or more methods for logging at different levels. MEL just provides a single `Log` method.

Wait, that's a good thing? That `Log` method looks a little complicated to call, doesn't it?

Actually, yes, that method does look a little complicated, but I still contend this makes for an
elegant design. If you're trying to adapt this abstraction to some other framework you have far
less work to do with a single `Log` method than you'd have with a half dozen of them. Most
other frameworks actually include a similar method, and all of the others are basically just
simpler overloads that should delegate to this one. By keeping the abstraction to this singular
method it's actually easier to work with when writing an adapter.

OK, but isn't it harder to use in regular logging code, then? Actually, no, it's not. See, there
**are** overloads provided, they are just provided through extension methods rather than polluting
the interface. To make you feel better, here's the extension methods.

```csharp
/// <summary>
/// ILogger extension methods for common scenarios.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogDebug(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogDebug(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogDebug(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogTrace(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogTrace(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogTrace(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogInformation(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogInformation(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogInformation(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogWarning(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogWarning(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogWarning(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogError(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogError(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogError(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogCritical(this ILogger logger, EventId eventId, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void LogCritical(this ILogger logger, string message, params object[] args)
    {
        // ...
    }

    /// <summary>
    /// Formats the message and creates a scope.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to create the scope in.</param>
    /// <param name="messageFormat">Format string of the scope message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>A disposable scope object. Can be null.</returns>
    public static IDisposable BeginScope(
        this ILogger logger,
        string messageFormat,
        params object[] args)
    {
        // ...
    }
}
```

So, all the overloads you'd expect are there, and as a consumer you can write code that looks very
familiar.

```csharp
logger.LogInformation("Something happened.");
```

You can, but I would not recommend writing code like that. I think there's a better way to use MEL,
which we'll slowly cover throughout this series. Bear with me. :)

Let's pick apart this `Log` method a bit, to get a deeper understanding of MEL.

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
```

That first parameter should be familiar. All logging frameworks have the concept of a logging level,
and MEL is no different. In fact, the levels it provides are pretty much the same levels every framework
uses, though maybe the names and values differ a little.

```csharp
/// <summary>
/// Defines logging severity levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
    /// These messages are disabled by default and should never be enabled in a production environment.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Logs that are used for interactive investigation during development.  These logs should primarily contain
    /// information useful for debugging and have no long-term value.
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Logs that track the general flow of the application. These logs should have long-term value.
    /// </summary>
    Information = 2,

    /// <summary>
    /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
    /// application execution to stop.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
    /// failure in the current activity, not an application-wide failure.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
    /// immediate attention.
    /// </summary>
    Critical = 5,

    /// <summary>
    /// Not used for writing log messages. Specifies that a logging category should not write any messages.
    /// </summary>
    None = 6,
}
```

That last value, `None`, should stick out at you a bit. This is one of few places where I think MEL's design
is deficient. There's two problems I see here. One, they've chosen to use a single enumeration where I think
two should have been. The enumeration you use when logging really should not be the same enumeration you use
to configure how to filter. As the comment says, `None` makes perfect sense when filtering, but it makes
no sense at all when logging. By combining the concepts into a single type we lose some compiler enforcement,
and a developer **could** use `None` while logging, as much as that makes no sense to do. The bigger issue here,
though, is actually the value assigned. `None` has an integral value of 6, while the Framework design
Guidelines tell you that such "no value" enumeration tags should always have a value of 0. Worse, 0
is assigned as the value for the tag `Trace`. This means `default(LogLevel)` equals `Trace`, not `None`
as one would expect. Oh well, no code is perfect.

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
```

The next parameter is an `EventId`. Most logging frameworks have this concept as well... an integer identifier
for the log event. Many frameworks don't actually use this identifier in any meaningful way, so some
may not quite understand why this concept is so important. It's all about filtering, reporting and
parsing the logs. If a particular log event has a unique identifier it's easy to locate all such
events in the log when trying to diagnose issues or find trends. Some log sources, such as the Event
Viewer in Windows actually require an ID, and unfortunately many logging frameworks just use a generic
ID of 0 when you don't provide one and they log to such sources. We'll come back to this topic in later
parts of this series, but first I'd like to point out something a bit surprising.

```csharp
public struct EventId
{
    private int _id;
    private string _name;

    public EventId(int id, string name = null)
    {
        _id = id;
        _name = name;
    }

    public int Id
    {
        get
        {
            return _id;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
    }

    public static implicit operator EventId(int i)
    {
        return new EventId(i);
    }

    public override string ToString()
    {
        if (_name != null)
        {
            return _name;
        }
        else
        {
            return _id.ToString();
        }
    }
}
```

So, EventId isn't just an integer. It optionally also contains a `Name`. I'll admit, it's not entirely
clear to me why this is important. I will point out that with the implicit operator you *can* just
pass an integer where an `EventId` is expected and it will just work, meaning you can treat this parameter
exactly the same way you're probably used to with other logging frameworks. If you know why you'd
want to provide a richer identifier with both a numeric value and a name, please let me know. This
is one design area I'd like to explore more.

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
```

Next, we have a `TState` parameter. Many logging frameworks just log messages. Luckily, that's not
the case for MEL. MEL logs data, not messages. This is a concept known as "semantic logging", and
it makes the logging richer. By logging data instead of messages it makes your logs consumable by
more than just a human. Rich data mining, filtering and reporting becomes available to you, making
your logs useful for things other than manually diagnosing issues. For instance, it would be trivial
to determine which features your users actually use the most if you could query the logs for such
information. I may touch on this more deeply if I get into creating a provider for MEL, but you should
understand that typical usage of MEL doesn't directly use this TState. Rather, the extension methods
use a string "template" and a params array to create an internal `FormattedLogValues` to use a the
`TState`. An example of this would be the following:

```csharp
logger.Log("Operation {Operation} started at time {Time}", "SomeOperation", DateTime.Now);
```

The first string represents a "template", not an actual log message. This might appear to be an
interpolated string based on what looks like "holes" (`{Operation}` and `{Time}`), but note the
lack of a '$' prefix. This is just a string, not an interpolated string. The "holes" are named
placeholders where the params array members are filled in, in the order provided. So, in this
example the `{Operation}` placeholder will be filled in with "SomeOperation". Please note
that MEL doesn't do this formatting. Within MEL this remains a template and is not transformed
into a message. This means the rich data information is retained, and for example a log provider
could translate this into rich JSON.

```JavaScript
{
    "Message": "Operation SomeOperation started at 10:45 am",
    "Operation": "SomeOperation",
    "Time": "2017-01-01T10:45:00Z"
}
```

It's actually pretty important that MEL doesn't just simply use messages here. Semantic logging
is a very important concept that should not be lost when abstracting away logging. You can easily
transform rich semantic logging events into simple messages for log sources that work that way,
but you cannot go in the other direction. Given the benefits of semantic logging, I'd highly
suggest when using MEL that you treat your logging as such, even if your just going to log
to a traditional flat log file with no semantic logging features. By doing so you retain the
greatest flexibility.

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
```

Next up is an `Exception` parameter. This is pretty straightforward. The majority of log events
are typically generated in response to an exception being thrown, so including the exception
in the log event is natural. This information *could* just be included in the `TState` parameter.
I can only assume that MEL doesn't do that in order to make it easier for providers to deal with
the exception directly. If it were "buried" inside the `TState` it would be nearly impossible
to deal with the exception programatically.

```csharp
void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
```

Finally, we have a `Func<TState, Exception, string>` parameter that provides a way to format
the event into a more traditional log message. This design might make for an interesting extension
space for advanced usage, but most people that use MEL will never touch this or the `TState`
parameter directly, instead using the extension overloads that treat everything as a "template +
data" concept.

This has been a very lengthy post, and we haven't really learned much about how to use MEL yet. I
warned you this wasn't a tutorial seriese. :) In [Part II](/posts/logging-part-II) I'll look at
the design of `ILoggerFactory` which gets us a little closer into how to use MEL.