---
Title:  "Understanding Task - Part II"
Image: /assets/banner_web.jpg
Published: 2017-01-07 12:39:00
Tags: ["C#", ".NET", "Task", "TPL"]
---

# Understanding Task

## Part I

In [Part I](/posts/understanding-task-I) I explained that a `Task` is really **just** a datastructure
that represents the result of "something", and implemented a *very* basic `MyTask<T>` to illustrate
that. Of course, that isn't a very complete picture and I promised that this time around we'd
update `MyTask<T>` to more closely represent the mental model you have for `Task`.

Most developer's mental model is very firmly fixed around a `Task` representing a thread, or maybe
they have a slightly better understanding and think of it as something run on the thread pool. A
very few might actually think in terms of any asynchronous operation, which is much closer to the
truth. In the Task Programming Library (TPL) we call it a `Task`, but other languages and frameworks
call them a `Promise` or a `Future`. These are probably better names, to be honest, as they get you
closer to the correct mental model.

I said a `Task` is just the result of *something*, but let's refine that. It's a datastructure that
represents the result of *something once there is a result*. The result may exist now, or it may
exist in the future, and the `Task` represents that result *now*. That may be a complicated explanation
for you to understand, so once again we'll attempt to make it concrete.

Currently, our `MyTask<T>` represents a result that's already available. Let's modify it so it can
*also* represent a result that may be available only in the future. Here's our code so far.

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

If this is  going to represent a result that may only be available in the future, we need some way to
set the result in the future. This is a tricky thing to solve, and this is where the TPL gets really
clever. We need to create a `MyTask<T>` that doesn't have a result, and then have some way to set the
result in the future. However, we don't want just anyone to be able to set the result. We want only
the code/method that creates the `MyTask<T>` instance to be able to set its result. The TPL does this
by using another type as both a factory object and the only way to set the result of the `Task`.
In the TPL, this type is called the `TaskCompletionSource<T>`. Let's create our own `MyTaskCompletionSource<T>`.

```csharp
public class MyTask<TResult>
{
    internal MyTask()
    {
    }

    public Exception Error { get; internal set; }

    public TResult Result { get; internal set; }
}

public class MyTaskCompletionSource<TResult>
{
    public MyTask<TResult> Task { get; } = new MyTask<TResult>();

    public void SetError(Exception e) => Task.Error = e;

    public void SetResult(TResult result) => Task.Result = result;
}
```

The `MyTask<T>` constructor and the setters for `Error` and `Result` are all declared internal. Only
`MyTaskCompletionSource<T>` should call these. The only way for end user code to create a `MyTask<T>`
and to set its results is to use a `MyTaskCompletionSource<T>`.

So, we've split up the construction from the setting of the results, allowing us to set the results sometime
in the future. However, there's no way to know when the result is available, and so trying to get the
`Result` or the `Error` before it's been set won't behave the way we'd like. Things will get a bit more
complicated now, but lets solve these problems.

```csharp
public class MyTask<TResult>
{
    private readonly ManualResetEvent completedEvent = new ManualResetEvent(false);
    private Exception error;
    private TResult result;

    internal MyTask()
    {
    }

    public Exception Error
    {
        get
        {
            this.completedEvent.WaitOne();
            return this.error;
        }

        set
        {
            this.error = value;
            this.completedEvent.Set();
        }
    }

    public TResult Result
    {
        get
        {
            this.completedEvent.WaitOne();
            if (this.error != null)
            {
                throw new InvalidOperationException("Operation failed.", this.error);
            }

            return this.result;
        }

        set
        {
            this.result = value;
            this.completedEvent.WaitOne();
        }
    }
}

public class MyTaskCompletionSource<TResult>
{
    public MyTask<TResult> Task { get; } = new MyTask<TResult>();

    public void SetError(Exception e) => Task.Error = e;

    public void SetResult(TResult result) => Task.Result = result;
}
```

> WARNING: The above code is **NOT** thread-safe.

I told you I'd show you a naive implementation. This code illustrates the ideas well, but it is **NOT**
thread-safe and there are other issues (like the `MyTaskCompletionSource<T>` being able to set
both a `Result` and an `Error`). I'm not trying to write production code here, but instead to just
get across the concepts. Since `Task` already exists I feel safe in posting code like this, because no
one would ever use it, but heed my warning: don't use this code as a way to understand how to write
thread-safe or production code.

So, with the warnings out of the way, let's see what we did. Our `MyTask<T>` now has a `ManualResetEvent`
that helps us to know *when* the results have been set. We modified the setters for `Error` and
`Result` so that when the `MyTaskCompletionSource<T>` calls them the event is set as well. Meanwhile,
the getters wait for this event to be set, making them blocking calls that will wait for the values
to be set before returning them. In addition, I went ahead and made the getter for `Result` throw an
exception if the result is actually an `Error` instead.

I hope the lightbulb just turned on for many of you. While the implementation is horrid, it does
represent the bulk of what a real `Task` is. In fact, there's only two concepts we've left out
here: continuations and cancellation. I'm not going to cover those anytime soon.

Note that there's no `Thread` anywhere in this code. Nor have I used the thread pool. Despite that,
this *is* a complete representation of a `Task` (minus continuations and cancellation, of course).
I here some of you saying "but what about `Task.Run`?" Well, we'll talk about that and other
related concepts next time, but `Task` doesn't really need them. This is really the complete
concept of a `Task`, and if you shift your mental model to think in these terms you'll be
much better suited for using the TPL.