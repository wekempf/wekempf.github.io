---
Title:  "Understanding Task - Part I"
Published: 2017-01-07 00:00:00
Tags: ["C#", ".NET", "Task", "TPL"]
---

# Understanding Task

## Part I

I've noticed that many developers I work and talk with have a pretty big misunderstanding of the `Task` and `Task<T>` types
from the Task Programming Library (TPL). In their minds a `Task` is a synonym for a `Thread`. With a bit of explanation
I can get them to almost understand that a `Task` is more often than not run on the thread pool, or represents asynchronous
I/O, but I don't get the feeling that I too often actually get them to understand this at a deep level. I think that
being able to effectively write programs with the TPL you do need this understanding, so I'm going to write a blog series
about asynchronous programming from a different point of view than most. I'm going to start from the bottom and work my
way up, while most other books/blogs/articles start at the top and work their way down, though usually not going to deep.
My hope is that by working from the bottom up, some of the mystery will just disappear.

I'll start by once again defining what a `Task` is at its most basic definition. A `Task` isn't something magical. Nor is
it really what the name implies: a running operation of some sort. No, a `Task` is just a simple data structure that
holds the result of something. That's it. At this point, don't even think in terms of that "something" being an asynchronous
operation. At it's simplest, that is not the case, and thinking in those terms right now will just confuse you and lead
you to some incorrect assumptions. Dispel this thought from your mind.

So, let's make this idea concrete instead of just asking you to comprehend this based solely on the claims somebody
on the Internet made. Let's actually start to build our own version of the TPL to understand this. Granted, what
we'll develop here and in future parts of this seriese will be naive implementations and a far cry from the actual
implementation in the TPL, but it will give us very deep understandings of what the TPL is all about. Remember, though,
we're building this from the bottom up. Don't try and think about where we'll be next or how we'll wind up at what
you're familiar with. Just stay with me at the stage in development that we're at.

So, let's create a `MyTask<T>`. For now we're going to ignore the non-generic version. We're also going to make the
most basic thing we can, ignoring many things that you know exist for the real `Task<T>`. Please, try not to think
about what you know, and just think about what we need at the simplest. From my description of what a `Task` is,
the simplest implementation for our `MyTask<T>` could look something like this:

```csharp
public class MyTask<TResult>
{
    public TResult Result { get; set; }
}
```

Really, it's as simple as that. This is a data structure that represents the result of "something". Most of what a
`Task` is all about is represented in the above code. Truly, I'm not trying to deceive you here, and if you're looking
for a deep understanding of the TPL, this isn't a gross oversimplification either. Yes, it is a simplification,
without a doubt, but it *does* give a very accurate representation of the concept of a `Task`. A `Task` *is*
just the result of *something*.

Obviously, where not done. I will contend this represents the largest portion of the concept of a `Task`, but that
doesn't mean we're done here. In fact, I've not done enough to make the light bulb click for you yet, I'm sure. At
this point you have more questions than you had before, and you're likely just thinking I'm a pretentious no-it-all
for distilling things down to this level. Bear with me. We're going to start fleshing things out now, and you'll
start to see the larger picture.

So, a `Task` just represents the result of *something*, but the above doesn't really go far enough to allow us to
represent the result of *anything*, or more accurately anything we'd model in a program. If nothing else, as
developers we realize most of the things we'd care to represent the result of has the potential to fail, so the
result is either successful or not. In .NET we represent failures with an `Exception`, so lets modify our code
to include this.

```csharp
public class MyTask<TResult>
{
    public Exception Error { get; set; }

    public TResult Result { get; set; }
}
```

This is starting to more closely resemble what we're used to, but there's serious problems with this code even
at this simple level. With it coded the way it is the `MyTask<T>` could have *both* a `Result` and an `Error`.
We need to constrain what it can represent. We'll do that by making the type immutable. While we're at it,
let's use factory methods instead of constructors to create instances of this.

```csharp
public class MyTask<TResult>
{
    private MyTask()
    {
    }

    public static MyTask<TResult> SetError(Exception e) => new MyTask<TResult> { Error = e };

    public static MyTask<TResult> SetResult(TResult result) => new MyTask<TResult> { Result = result };

    public Exception Error { get; private set; }

    public TResult Result { get; private set; }
}
```

There, that's it for now. Pretty simple, huh? Yeah, I know you still have questions. This code does a good
job representing my definition of a `Task`, but it's done *nothing* to turn the light bulb on yet, because
we're still missing some functionality that you know must exist. We'll come much closer to the mental model
you have next time, but in doing so I hope to also start to make you let go of that mental model and replace
it with a more accurate one.