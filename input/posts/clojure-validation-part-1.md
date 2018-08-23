---
Title: "Clojure Validation - Part I"
Published: 08/21/2018 08:43:11
Tags: ["Clojure", "ClojureScript", "Reagent"]
---

# Clojure Validation - Part I

### Clojure Spec

> You know my method. It is founded upon the observation of trifles.  
> ^^ Sherlock Holmes

Our goal is to use ClojureScript and Reagent to create a component that validates its input. Along the way we'll learn about a few other things, including Clojure Specs, the focus of this post.

Full series:

* Part I - Clojure Spec (this post)
* [Part II - UI Validation](/posts/clojure-validation-part-2)
* [Part III - Better Messages with Phrase](/posts/clojure-validation-part-3)
* [Part IV - I18N](/posts/clojure-validation-part-4)

## Creating a Project

This isn't a beginners tutorial, but I'm going to start with how to create our project just to ensure if you're following along yourself you can start from the same place. I'll create a new project using Leiningen like this.

:::{.alert .alert-info}
I use PowerShell on Windows, so all command examples given will reflect this. If you're using a different platform or shell things may be slightly different for you, but it shouldn't be too hard to figure out.
:::

```PowerShell
PS> lein new reagent validation-experiment +test
```

The code at this point is pushed to https://github.com/wekempf/validation-experiment on the master branch. I'll create other branches for the code as we progress here.

## Our SSN "Type"

Clojure is a dynamic language, so I'm using "type" rather loosely here. What we're going to work with, and validate, however is an SSN value. This is going to be a string that conforms to certain properties... which is what we'll want to validate. We're going to do our validation through [Clojure Spec](https://clojure.org/guides/spec). If you read that guide you may find it to be confusing (or at the very least, rather dry reading). While some of the details of how you use it may take some time to master, it really should not be too difficult to grasp at a high level.

*[FP]: Functional Programming

Basically, Clojure Spec is a library used to validate that data conforms to certain rules specified as predicates (functions that take the data and return true if the data conforms and false if it does not). At the 1000 foot view, that's all it is. Dial in a little closer and you'll see that Clojure Spec provides a few "higher order functions" that allow you to compose predicates so that you can take simple predicates and build more complex ones. This composability is an important aspect of functional programming (FP), and there's lots of power in this that may not be evident to those new to FP. Clojure Spec provides methods for "running" these predicates and producing rich information about the results. Finally, Clojure Spec provides facilities for runtime assertions and testing facilities such as generative testing. As we work on this experiment we'll touch on most of these features, but this isn't an in depth focus on Clojure Spec. We're focused on how we can use it to provide UI validation.

Let's get started. Create an ssn.cljc file in the src\cljc\validation_experiment directory. In that file, add the following code.

```Clojure
(ns validation-experiment.ssn
    (:require [clojure.spec.alpha :as s]))

(defn- valid-format? [ssn]
  (boolean (re-matches #"\d{9}|\d{3}-\d{2}-\d{4}" ssn)))

(s/def ::ssn valid-format?)
```

We've created our namespace and required clojure.spec.alpha. The `s/def` call defines our predicate (so far) for validating SSN values. You may not be familiar with the `::ssn` syntax used here... that's a shortcut for specifying a keyword that's fully qualified with the current namespace. So, what we've really done here is defined the predicate for  `:validation-experiment.ssn/ssn`. Because Clojure Spec defines predicates in a global registry it's a best practice to define them using namespaces to avoid name collisions.

I've chosen to use a (private) named predicate function outside of the `s/def`. This is a good practice for several reasons. First, it makes it easier to compose predicates as we'll see shortly. Just as importantly, though, it provides richer information that will be reported when there's validation errors.

The `valid-format?` predicate validates that an SSN consists of 9 digits, with or without the '-' segment separators (i.e. 123456789 and 123-45-6789 are both valid). I'm accepting both formats to make it easier for a user to enter an SSN.

With this in place, fire up a REPL with the following command.

```PowerShell
PS> lein repl
```

At the REPL enter the following.

```
validation-experiment.repl=> (require '[clojure.spec.alpha :as s])
nil
validation-experiment.repl=> (require '[validation-experiment.ssn :as ssn])
nil
validation-experiment.repl=> (s/valid? :validation-experiment.ssn/ssn "123456789")
true
```

Great! Clojure Spec is able to validate "123456789" as a valid SSN using our predicate. Let's try validating an invalid value.

```
validation-experiment.repl=> (s/valid? :validation-experiment.ssn/ssn "12345678")
false
```

That works as well!

Honestly, though, this isn't that exciting yet. If we'd made `valid-format?` public we could have just called that directly and gotten the same result. Let's try something else to start to see the benefit of using Clojure Spec.

```
validation-experiment.repl=> (s/explain :validation-experiment.ssn/ssn "12345678")
val: "12345678" fails spec: :validation-experiment.ssn/ssn predicate: valid-format?
nil
```

The `s/explain` function writes out information that tells how the data fails to conform to the specification. In this case it tells us the specification name (`:validation-experiment.ssn/ssn`) that failed. It also tells what predicate (`valid-format?`) within that specification failed. See why I wanted to use a named predicat function?

Clojure Spec does more. There's nothing wrong with what we have so far but there's several invalid SSN values that will still be accepted right now. So, let's write another predicate to ensure the area segment (the first 3 digits) is not "000" which is not valid in SSNs.

```Clojure
(defn- valid-area? [ssn]
  (not= (subs ssn 0 3) "000"))
```

We also have to update the `::ssn` definition to include this new predicate.

```Clojure
(s/def ::ssn (s/and valid-format? valid-area?))
```

We use `s/and` here to compose our two predicates into a single predicate that requires both to pass. Using composition like this allows Clojure Spec to give us pretty detailed information about why validation fails.

Now let's see this in action.

```
validation-experiment.repl=> (use 'validation-experiment.ssn :reload)
nil
validation-experiment.repl=> (s/explain :validation-experiment.ssn/ssn "000456789")
val: "000456789" fails spec: :validation-experiment.ssn/ssn predicate: valid-area?
nil
```

The "000" value isn't the only invalid value for the area segment (see http://usrecordsearch.com/ssn.htm): any value in the range 700-799 is also invalid. Let's update our predicate to reflect this.

```Clojure
(defn- parseInt [s]
  #?(:clj (Integer/parseInt s))
  #?(:cljs (js/parseInt s)))

(defn- valid-area? [ssn]
  (let [area (subs ssn 0 3)]
    (and
     (not= area "000")
     (not (some #{(parseInt area)} (range 700 799))))))
```

We've defined a helper `parseInt` here and used [reader conditionals](https://clojure.org/guides/reader_conditionals) to "do the right thing" whether we're compiling to Clojure or ClojureScript. We then use that helper to test to see if the area is in the range 700-799 in the updated predicate. Test this out in the REPL.

```
validation-experiment.repl=> (use 'validation-experiment.ssn :reload)
nil
validation-experiment.repl=> (s/explain :validation-experiment.ssn/ssn "777456789")
val: "777456789" fails spec: :validation-experiment.ssn/ssn predicate: valid-area?
nil
```

Like the area, the group segment in an SSN cannot be all zeros. We should add a predicate for that as well, but there's a slight complication here. We're accepting two forms for the SSN, with or without the '-' segment separator character. This means we need to check the digits either at position 3 or position 4, depending on the form used. Let's simplify this by creating a method that will strip out the separator characters so that it will always be in position 3 that we check. To do this we'll need to use `clojure.string/replace` so let's require that namespace.

```Clojure
(ns validation-experiment.ssn
  (:require [clojure.spec.alpha :as s]
            [clojure.string :as string]))
```

With that in place, here's the updated code we need to add.

```Clojure
(defn digits [ssn]
  (string/replace ssn "-" ""))

(defn- valid-group? [ssn]
  (let [group (subs (digits ssn) 3 5)]
    (not= group "00")))

(s/def ::ssn (s/and valid-format? valid-area? valid-group?))
```

Note that we've kept `digits` public (`defn` vs `defn-`) as this could be a useful method to use outside of this namespace.

With that in place we can test it in the REPL.

```
validation-experiment.repl=> (use 'validation-experiment.ssn :reload)
nil
validation-experiment.repl=> (s/explain :validation-experiment.ssn/ssn "123006789")
val: "1230
06789" fails spec: :validation-experiment.ssn/ssn predicate: valid-group?
nil
```

There's one last check to make: SSN serial numbers can also not be all zeros. In addition, we'll improve the `::ssn` predicate a bit and ensure the data is a string before we test the other predicates. Here's the final code in it's entirety.

```Clojure
(ns validation-experiment.ssn
  (:require [clojure.spec.alpha :as s]
            [clojure.string :as string]))

(defn- valid-format? [ssn]
  (boolean (re-matches #"\d{9}|\d{3}-\d{2}-\d{4}" ssn)))

(defn- parseInt [s]
  #?(:clj (Integer/parseInt s))
  #?(:cljs (js/parseInt s)))

(defn- valid-area? [ssn]
  (let [area (subs ssn 0 3)]
    (and
     (not= area "000")
     (not (some #{(parseInt area)} (range 700 799))))))

(defn digits [ssn]
  (string/replace ssn "-" ""))

(defn- valid-group? [ssn]
  (let [group (subs (digits ssn) 3 5)]
    (not= group "00")))

(defn- valid-serial-number? [ssn]
  (let [serial-number (subs (digits ssn) 5 9)]
    (not= serial-number "0000")))

(s/def ::ssn (s/and string? valid-format? valid-area? valid-group? valid-serial-number?))
```

That's it for now. [Next time](/posts/clojure-validation-part-2) we'll actually tie this into our Reagent code to validate the input in a text box.

:::{.alert .alert-success}
The code for this part of the series can be found in the branch `part1` in the associated repo at https://github.com/wekempf/validation-experiment.
:::