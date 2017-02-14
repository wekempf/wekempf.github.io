---
Title: "Pragmatic Languages"
Published: 02/09/2017 07:07:49
Tags: ["Programming", "Languages"]
---

# Pragmatic Languages

I was influenced heavily early on in my career by the book *The Pragmatic Programmer: From Journeyman to Master*[^1]. One of the
best pieces of advice given in that book was to learn new languages. That's a continuous process. You should never stop learning
new languages. Most developers basically learn a single language and spend their career programming in it, but the pragmatic
programmer knows that this is a mistake. There's the obvious problem: languages come and go, and thus so can your career.
This even happens to programmers that use languages "that will never go away." For instance, COBOL is still used pretty heavily,
and I'm not about to predict when it will ever truly disappear. Despite that, I saw many a COBOL developer who's career ended
in the late 80s and early 90s as the demand for the language waned. That's a silly position to be in, since demand for programmers
was on the rise.

The idea behind learning new languages is deeper than that, though. Yes, you don't want to be caught in the position of the only
language you know losing demand. That said, though, the reality is that most of us will spend most of our career writing code
in a single language. Maybe you'll switch languages once or twice, but that's the extent of it. So, why continually learn new
languages? The answer to that is because it makes you a better programmer. Every language has a "style" that's pretty unique.
We've labeled certain groupings of these styles: procedural, functional, Object-Oriented. Even with these labels though
you'll find that many languages mostly fall into one category but have features of others, and no two languages really
approach a problem the same way. Learning these other styles can actually open up your mind to other ways of attacking a
problem, even when using another language... say the language you actually use in your career. If a language sees enough use,
you see this within the evolution of the language itself. For instance, C# started life pretty much as an Object-Oriented
language. There were little hints of procedural features (static methods) and functional concepts (delegates), but There
was no way you'd put it in either of those categories. Over time, though, features were added to the language to the point
where we're on the cusp of actually being able to call it a functional language as well. That said, it's still no F# or
Haskell, both of which use very different styles for solving problems.

I have no idea if that super brief explanation swayed you or not. If not, I strongly recommend reading *The Pragmatic Programmer*.
Assuming you are convinced, what languages should you learn? Well, all of them ;). That's not realistic, though. You're
probably looking for guidance as you start picking up this habit as to what languages to start with. Well, opinions will
differ greatly hear, but I'm going to tell you which languages I consider to have been important to me as I did this, and
why I felt that way.

## Assembly Language

Note that I'm not telling you what Assembly language to learn. I learned a couple of them, but I don't think it matters too
much which one(s) you learn.

This "language" almost didn't make my list here. "When I was a youngun'..." this was a very important "language" to learn.
It's importance is diminished, somewhat, in this day and age. It's still important because it's the best way to understand
how the hardware you're righting code for works. Some other languages are "low level" enough to help you learn some of this,
which is why this almost didn't make the list, but there's no doubt learning Assembly is the best way to understand the
hardware. Depending on your language, this might also be important for day to day debugging tasks, and it will always
be important for the really hard debugging cases.

## C/C++

OK, some will hate that I've grouped the two together. They are separate languages, and maybe I should have made them separate
entries. If you're cognizant of what you should learn, however, you could learn C++ without ever learning C. I do think you
should learn C++ regardless.

These are low level languages, and in a pinch could stand in for learning Assembly. You'll learn concepts like the stack and
the heap, pointers, memory management, etc. This are all concepts that are extremely important to understand even if your
language of choice doesn't expose them or at least hides them from most of your code. The higher level abstractions are all
built on these concepts, and understanding them will help you to write more efficient code in any language. C++ is singularly
unique. Over time it's gained all sort of features from other languages, but has done so in ways that usually more directly
expose you to how such features work. For instance, C++ started out with function pointers and over time wrapped those
in high level concepts like lambdas. When doing so, however, you are still very much cognizant of the lower level concepts
used to build the higher level features. Templates are a great examle here. Most modern languages have the concept of
"generic types" in one form or another, but none match the low level implementation of C++ templates and it's expressibility.
Of course, this means that C++ is probably the most complex language in existence. There's a reason there's a big divide
among those that love the language and those that hate it, with very few people falling in between. Don't ignore this
important language because of the complexity, however. Avoiding it as your day to day language may make sense (do note that
I said "may"), but avoiding learning it to make you a better programmer is, IMHO, a grave error. I'd almost label this
language as *the* most important language to learn, even though it's probably the last language I'd use for most tasks
today (again, note that I said *most*).

## Smalltalk

OK, my mentioning this one is a little hypocritical, as while I've studied it a little, it's the language on this list
that I've used the least, by a very wide margin. That said, I've learned enough from it to be confident in saying you
can learn a lot from this language. You can't really talk about OO programming without having some knowledge of Smalltalk.
Many things we consider to just be part of our essential tools have their roots in Smalltalk: unit testing, patterns,
messaging... these are all concepts that at least in part came from Smalltalk and the developers that used it.

## Lisp

There are other functional languages, including more "pure" languages that are probably more important today. That said,
Lisp is to functional what Smalltalk was to OO. Lisp syntax may make your eyes bleed, but the simplicity of it's design
and the incredible power that design gives you are a thing of beauty. If you truly learn Lisp I guarantee this language
will impact how you think about problem solving in pretty much every other language you ever use. In particular,
the concept of Macros in Lisp is something I whished more languages had.

## Haskell

There's no doubt that functional languages are seeing a resurgence today. There's also no doubt that functional concepts
are becoming more important regardless of the language you use. Functional approaches to problems are so often the
simplest and most ellagant approaches. Haskell, among all of the languages in this category, stands out from most
in a list of languages for a pragmatic programmer to learn, because it's got one of the richest type systems of any
other language. Where Lisp is powerful due to it's simplicity, Haskell is powerful on a different level, due to its
type abstractions. Learning about Monads and Higher Kinded Types my hurt your brain a bit, they really allow you to
approach problems in a different manner.

## Eiffel

I have a special place in my heart for this one, even though I've never been able to really use it. The book
*Object Oriented Software Construction*[^2] is on my top 5 list of books every developer should read. There were
concepts born out of Eiffel, such as Design By Contract (DbC), that should influence how every developer writes
programs. It's one of the more unique languages I've ever worked with, and it's ideas certainly still influence
me today.

## Conclusion

There's all sorts of other languages that I've learned and love, or that have taught me new tricks even if I didn't
much care for the language. Ruby, Python, Scala, C#, Java, F#. Some of those came very close to making my list here
(looking at you Ruby and C#), but this list isn't a list of my favorite languages. It's also not a list of languages
I'd like to learn better (Rust, Swift, Go, Smalltalk). It's a list of the languages I believe most influenced me and is the most
likely to influence you as well. These should be the start of your journey, and not the entirety of it.

[^1]: Hunt, Andrew and David Thomas. [The Pragmatic Programmer: From Journeyman to Master](https://www.amazon.com/Pragmatic-Programmer-Journeyman-Master/dp/020161622X/ref=sr_1_1?ie=UTF8&qid=1486642301&sr=8-1&keywords=the+pragmatic+programmer).
[^2]: Meyer, Bertrand. [Object Oriented Software Construction](https://www.amazon.com/Object-Oriented-Software-Construction-Book-CD-ROM/dp/0136291554/ref=sr_1_1?s=books&ie=UTF8&qid=1486647602&sr=1-1&keywords=object+oriented+software+construction).