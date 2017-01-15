---
Title: "Playing With Equality"
Published: 01/14/2017 13:10:36
Tags: ["C#", ".NET"]
---

# Playing With Equality

Most developers think they understand equality, but it's actually a pretty tricky subject.
Implementing equality correctly is much more complicated than most realize. In this post
I'm going to play around with equality, showing you some things you may not be aware of,
and eventually show you an interesting trick you can use when implementing equality.

## Value Equality

Did you know that in .NET you get value equality for free? Value types automatically
implement `object.Equals` and `object.GetHashCode` to "do the right thing".
Here's a simple `Point` struct.

```csharp
public struct Point
{
    public int X { get; set; }

    public int Y { get; set; }
}
```

And here's some simple code to illustrate the equality behavior you get for free.

```csharp
var p1 = new Point { X = 10, Y = 20 };
var p2 = new Point { X = 10, Y = 30 };
var p3 = new Point { X = 10, Y = 20 };

WriteLine(p1.Equals(p1));
WriteLine(p1.Equals(p2));
WriteLine(p1.Equals(p3));
WriteLine(p1.GetHashCode());
WriteLine(p2.GetHashCode());
WriteLine(p3.GetHashCode());
```

This produces the following results.

```text
True
False
True
346948930
346948936
346948930
```

So `object.Equals` behaves as you'd expect, as does `object.GetHashCode`. What you
don't get is `IEquatable<Point>`, unfortunately, but for value types that's fairly
easy to implement.

```csharp
public struct Point : IEquatable<Point>
{
    public int X { get; set; }

    public int Y { get; set; }

        public bool Equals(Point other) => object.Equals(this, other);
}
```

## Reference Equality

It's not so simple for reference types. Reference types don't provide value equality,
they provide reference equality. Let's illustrate.

```csharp
public class RefPoint
{
    public int X { get; set; }

    public int Y { get; set; }
}
```

This is identical to our original `Point`, except it's a class instead of a struct.
Let's use it like we did the struct.

```csharp
var p4 = new RefPoint { X = 10, Y = 20 };
var p5 = new RefPoint { X = 10, Y = 30 };
var p6 = new RefPoint { X = 10, Y = 20 };

WriteLine(p4.Equals(p4));
WriteLine(p4.Equals(p5));
WriteLine(p4.Equals(p6));
WriteLine(p4.GetHashCode());
WriteLine(p5.GetHashCode());
WriteLine(p6.GetHashCode());
```

Here's the result.

```text
True
False
False
21083178
55530882
30015890
```

So, every instance has it's own unique hash code and an instance is only equal to itself.
This is reference equality. We can override the behavior to get back value equality, but
it's non-trivial.

```csharp
public class RefPoint : IEquatable<RefPoint>
{
    public int X { get; set; }

    public int Y { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as RefPoint);

    public override int GetHashCode() =>
        (this.X.GetHashCode() * 486187739) + this.Y.GetHashCode();

    public bool Equals(RefPoint other) =>
        other == null
            ? false
            : this.X == other.X && this.Y == other.Y;
}
```

Handling `null` is a bit tricky, but most of us should be able to handle that corectly if
we're careful. `GetHashCode` is another matter. Not many people know how to implement a good
hashing algorithm. With some web searching you can come across the basic algorithm I've used
here: you multiply each "part" (each field/property) except the last by some large prime
number and add all of the results together. You could memorize this, but it's still a lot
of fiddly code to have to write for such an "easy" concept as equality.

## ValueTuple trick

I promised a trick. Well, we know that value types implement equality by default. There's
a trick you can take advantage of when implementing equality for reference types.

In C# 7 they've introduced tuples into the language. Under the cover these tuples are
instances of a new type: `ValueTuple`. This type is, as it's name implies, a value type.
Since value types implement `object.Equals` and `object.GetHashCode`, we can take
advantage of that when implementing them for reference types.

```csharp
public class RefPoint : IEquatable<RefPoint>
{
    public int X { get; set; }

    public int Y { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as RefPoint);

    public override int GetHashCode() =>
        this.AsTuple().GetHashCode();

    public bool Equals(RefPoint other) =>
        other == null
            ? false
            : this.AsTuple().Equals(other.AsTuple());

    private (int X, int Y) AsTuple() => (this.X, this.Y);
}
```

We stil have to handle `null`, unfortunately, but the rest has become much easier.
BTW, in older versions of C# you can do the same thing with an anonymous type
or a custom value type, but neither is as nice as the `ValueTuple`.

## Wishful Thinking

The Ruby language has this nice little concept of the "spaceship operator". The
`<=>` operator is basically equivelent to `IComparable.Compare`, returning a
negative value if the left value is less than the right value, a positive value
if the left value is greater than the right value and zero if the left value is
equal to the right value. The real beauty here is if you define this one operator,
everything else is defined for you. Wouldn't it be awesome if we had the same
concept in C#? In C# we split `IEquatable` and `IComparable` concepts up, rightfully,
so we might need two operators. I'll call them `===` and `<=>`. If you defined
`<=>` you'd get `IEqutable`, `IComparable`, `object.Equals`, `object.GetHashCode`,
`==`, `!=`, `<`, `<=`, `>` and `>=` all implemented for you by the compiler.
If you implemented `===` instead you'd get `IEquatable`, `object.Equals`,
`object.GetHashCode`, `==` and `!=`. Oh, and as long as we're wishing,
let's make it really simple to implement these two operators.

```csharp
public class RefPoint
{
    public int X { get; set; }

    public int Y { get; set; }

    public operator ===() => this.X, this.Y;
}
```

In the above example we implemented this hypothetical operator not by declaring
how to compare but by only declaring what to compare. This allows the definition
to automatically implement everything else for us, basically giving us code equivalent
to what I showed for the `ValueTuple` trick (plus the operators).

Yeah, I'm not entirely sold on the syntax here either. After all, there's no `===` or
`<=>` operators you can use anywhere other than in declaring things here. Maybe it would
make more sense with syntax like this.

```csharp
public class RefPoint
    equatable : X, Y
{
    public int X { get; set; }

    public int Y { get; set; }
}
```

Still, the idea of no longer having to deal with the intricacies of implementing equality
(or ordering) by hand, the source of far too many bugs, is enticing, isn't it?