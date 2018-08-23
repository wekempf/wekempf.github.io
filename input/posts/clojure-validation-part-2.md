---
Title: "Clojure Validation - Part II"
Published: 08/21/2018 15:14:26
Tags: ["Clojure", "ClojureScript", "Reagent"]
Scripts: ["/assets/scripts/clojure-validation-part-2/app.js"]
---

# Clojure Validation - Part II

### UI Validation

> Nothing clears up a case so much as stating it to another person.  
> ^^ Sherlock Holmes

Last time we used Clojure Spec to build up some validation predicates to ensure we have a valid SSN. This time around we're going to tie this to some Reagent components to use in our Web UI, giving feedback to the user when they enter bad data.

Full series:

* [Part I - Clojure Spec](/posts/clojure-validation-part-1)
* Part II - UI Validation (this post)
* [Part III - Better Messages with Phrase](/posts/clojure-validation-part-3)
* [Part IV - I18N](/posts/clojure-validation-part-4)

## Final Result

Here's an embedded example of the final result we're going to get after this blog post.

:::{#app}
Enable JavaScript to see the example.
:::

:::{.alert .alert-info}
This is only the look and behavior at the end of this post, and not the final result we'll get working through this series!
:::

## A Much Simpler Core

The generated `core.cljs` file we have currently, while not really complex, is far more complex then we need for this experiment. It's a SPA page with navigation, and frankly we don't need that. In fact, I'm wanting to generate a script that I can use to embed in this blog post, so we really want something simple here. This is what we'll start with.

```Clojure
(ns validation-experiment.core
  (:require [reagent.core :as reagent :refer [atom]]))

(defn example []
  [:div [:label "Enter SSN: " [:input {:type :text}]]])

(defn mount-root []
  (reagent/render [example] (.getElementById js/document "app")))

(defn init! []
  (mount-root))
```

This produces (inside an existing DOM element with an "app" id) a simple label and a text input box. Oh the magnificence!

You can view this wonder using Figwheel by executing the following.

```PowerShell
PS> lein figwheel
```

Now open a browser and navigate to http://localhost:3449 to behold the results of our hard work.

If you're not familiar with [Figwheel](https://github.com/bhauman/lein-figwheel) it " builds your ClojureScript code and hot loads it into the browser as you are coding!" So, don't stop it or close that browser. As we change our code and save it your browser will magically update with the changes live, current state intact. This is an unbelievable bit of tech that makes coding stuff like this a dream.

## Adding State

So, we have a text input box, but there's no state backing it. Let's fix that by changing a bit of the code.

```Clojure
(defonce ssn (atom "123-45-6789"))

(defn example []
  [:div [:label "Enter SSN: " [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]])
```

We've defined an `ssn` variable as an `atom` with the value "123-45-6789". A [Clojure atom](https://clojure.org/reference/atoms) provides "a way to manage shared, synchronous, independent state." In this case, this is actually a [Reagent atom](https://reagent-project.github.io/). "It works exactly like the one in clojure.core, except that it keeps track of every time it is deref’ed. Any component that uses an atom is automagically re-rendered when its value changes." This is really the magic that makes Reagent work here, though with just what we have right now you will be forgiven for not seeing any magic.

## Adding Validation

Let's see that magic in use! We'll add validation to our control using Clojure Spec and the code we wrote last time. We're going to need access to a few more namespaces.

```Clojure
(ns validation-experiment.core
  (:require [reagent.core :as reagent :refer [atom]]
            [validation-experiment.ssn :as ssn]
            [clojure.spec.alpha :as s]))
```

Now, we can get automatic display of validation results with the following simple change.

```Clojure
(defn example []
  [:div [:label "Enter SSN: " [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]
   [:p {:style {:color "red"}} (str (s/explain-data :validation-experiment.ssn/ssn @ssn))]])
```

We've added a `p` tag, styled it to display in red, and populated it with the results of `s/explain-data` rendered as a string. Last time we used `s/explain`, so this shouldn't be too unfamilar. The difference between `s/explain` and `s/explain-data` is that the former just writes the results to the output (not very helpful in a web page) while the latter returns those results as a Clojure map, which we rendered as a string to include in our paragraph.

Play around a bit with the result, entering different values into the text box. See how the validation display changes as you type? Notice that it disappears entirely when you've entered a valid SSN? The live updates are the magic of Reagent and it's atom, while having the validation message dissapear is a result of how Reagent handles `nil`. When `s/explain-data` returns `nil` for valid input Reagent actually removes it from the child list.

Look at how super simple it was to get validation in Reagent using Clojure Spec!

Oh, but the validation message displayed is really ugly you say? Well, yes, yes it is. We'll fix that in the [next part](/posts/clojure-validation-part-3) of this series.

:::{.alert .alert-success}
The code for this part of the series can be found in the branch `part2` in the associated repo at https://github.com/wekempf/validation-experiment.
:::