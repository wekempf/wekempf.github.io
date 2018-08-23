---
Title: "Clojure Validation - Part III"
Published: 08/22/2018 09:52:09
Tags: ["Clojure", "ClojureScript", "Reagent"]
Scripts: ["/assets/scripts/clojure-validation-part-3/app.js"]
---

# Clojure Validation - Part III

### Better Messages with Phrase

> Data! Data! Data!” he cried impatiently. “I can’t make bricks without clay.
> ^^ Sherlock Holmes

Last time we added validation to our web UI, but the validation "messages" were something only a Clojurist would understand or care for. This time around we're going to fix that.

Full series:

* [Part I - Clojure Spec](/posts/clojure-validation-part-1)
* [Part II - UI Validation](/posts/clojure-validation-part-2)
* Part III - Better Messages with Phrase (this post)
* [Part IV - I18N](/posts/clojure-validation-part-4)

## Final Result

Like last time, here's an embedded example of what we'll accomplish this time around.

:::{#app}
Enable JavaScript to see the example.
:::

:::{.alert .alert-info}
This time our behavior is much better, but I make no excuses for the appearance, other than to say that this is only sample code and not something meant for a production website.
:::

## Humane Messages

Last time we built a UI that displayed validation errors using `s/explain_data`. That function returns data, not a message, and that data is meant for a Clojure developer, not a normal human. So, while it was great we could easily validate input, no one would argue that what we had last time was usable in any sort of way.

The nice thing about Clojure Spec and the data returned by `s/explain_data` though is that it gives us very rich data that we can use programmatically any way we choose to. One thing we could do is map that data, in particular which predicate failed, to a friendly message. That's precisely what [Phrase](https://github.com/alexanderkiel/phrase) does!

Let's do the simplest thing we can do to replace `s/explain_data` with `phrase/phrase-first`. First thing, you have to add `[phrase "0.3-alpha4"]` as a dependency to `project.clj`.

```Clojure
  :dependencies [[org.clojure/clojure "1.9.0"]
                 [ring-server "0.5.0"]
                 [reagent "0.8.1"]
                 [reagent-utils "0.3.1"]
                 [ring "1.6.3"]
                 [ring/ring-defaults "0.3.1"]
                 [compojure "1.6.1"]
                 [hiccup "1.0.5"]
                 [yogthos/config "1.1.1"]
                 [org.clojure/clojurescript "1.10.339"
                  :scope "provided"]
                 [secretary "1.2.3"]
                 [venantius/accountant "0.2.4"
                  :exclusions [org.clojure/tools.reader]]
                 [phrase "0.3-alpha4"]]
```

:::{.alert .alert-warning}
Don't forget to restart Figwheel as project changes aren't auto applied the same way that source changes are.
:::

Next you have to add the namespace in `core.cljs`.

```Clojure
(ns validation-experiment.core
  (:require [reagent.core :as reagent :refer [atom]]
            [validation-experiment.ssn :as ssn]
            [clojure.spec.alpha :as s]
            [phrase.alpha :refer [phrase-first defphraser]]))
```

With that out of the way, we can modify our component to use `phrase-first` and produce a more meaningful validation message.

```Clojure
(defphraser :default
  [_ _]
  "Invalid value!")

(defn example []
  [:div [:label "Enter SSN: " [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]
   [:p {:style {:color "red"}} (phrase-first {} :validation-experiment.ssn/ssn @ssn)]])
```

The call to `defphraser` registered a default message to display any time there's a validation error. Try it out. No more "meaningless" Clojure Spec data, and instead you see "Invalid value!" when validation fails.

## Bringing Detail Back

That's great and all, but we've lost information about _why_ the validation failed! Let's fix that. Since the "why" is specific to our Clojure Spec definitions, let's keep the code in the same namespace. So, modify the `ssn.cljc` file's namespace to include Phrase.

```Clojure
(ns validation-experiment.ssn
  (:require [clojure.spec.alpha :as s]
            [clojure.string :as string]
            [phrase.alpha :refer [defphraser]]))
```

Now at the end of that file define messages for each of our predicates.

```Clojure
(defphraser valid-format?
  [_ _]
  "Not a valid SSN format.")

(defphraser valid-area?
  [_ _]
  "Not a valid area segment.")

(defphraser valid-group?
  [_ _]
  "Not a valid group segment.")

(defphraser valid-serial-number?
  [_ _]
  "Not a valid serial number.")
```

Much better! We could do a little better yet, though. Not everyone knows what the "area", "group" and "serial number" segments are. Let's make this a little clearer by displaying the invalid portion of the input in the message.

```Clojure
(defphraser valid-area?
  [_ {:keys [val]}]
  (str "\"" (subs val 0 3) "\" is not a valid area segment."))

(defphraser valid-group?
  [_ {:keys [val]}]
  (str "\"" (subs (digits val) 3 5) "\" is not a valid group segment."))

(defphraser valid-serial-number?
  [_ {:keys [val]}]
  (str "\"" (subs (digits val) 5 9) "\" is not a valid serial number."))
```

Yet again, it was really easy to go from a validation message only a Clojurist would love, to rich and human readable messages.

[Next time](/posts/clojure-validation-part-4) we'll cover a bonus topic... localizing these messages.

:::{.alert .alert-success}
The code for this part of the series can be found in the branch `part3` in the associated repo at https://github.com/wekempf/validation-experiment.
:::