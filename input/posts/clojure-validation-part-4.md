---
Title: "Clojure Validation - Part IV"
Published: 08/22/2018 11:29:16
Tags: ["Clojure", "ClojureScript", "Reagent"]
Scripts: ["/assets/scripts/clojure-validation-part-4/app.js"]
---

# Clojure Validation - Part IV

### I18N

> I confess that I have been blind as a mole, but it is better to learn wisdom late than never to learn it at all.
> ^^ Sherlock Holmes

Last time we created really nice validation messages to display for the various input errors we detect. Over my many years as a UI developer I've learned that no matter what the product owners say today, sooner or later they will demand you localize the application. So this time we're going to get a bonus post and learn how we can localize these messages to various languages.

* [Part I - Clojure Spec](/posts/clojure-validation-part-1)
* [Part II - UI Validation](/posts/clojure-validation-part-2)
* [Part III - Better Messages with Phrase](/posts/clojure-validation-part-3)
* Part IV - I18N (this post)

## Final Result

Like last time, here's an embedded example of what we'll accomplish this time around.

:::{#app}
Enable JavaScript to see the example.
:::

:::{.alert .alert-info}
OK, we still have some aesthetic issues, but once again this is purely an example. That's my excuse and I'm sticking to it.
:::

## What the Heck is I18N?

I18N is an abbreviation for "internationalization". It's a clever abbreviation at that. "Internationalization" is I followed by 18 other letters followed by N. No, I cannot (will not) take credit for that, it really is the abbreviation used by the industry.

Internationalization is the design and development of a product, application or document content that enables easy localization for target audiences that vary in culture, region, or language. A related topic is L10N ("localization"... you can guess how the abbreviation came to be now): the adaptation of a product, application or document content to meet the language, cultural and other requirements of a specific target market (a locale). Technically we're going to do both in this post, but the focus is on internationalization not localization as we're only going to localize to two locales, en-US and de-DE, and even then we're going to do a lousy job of it by using Google Translate.

## Tempura

We'll internationalize our "application" using [Tempura](https://github.com/ptaoussanis/tempura), which makes this very easy (especially in comparison to other programming languages in which I've done localization).

We'll start by adding Tempura as a dependency in `project.clj`.

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
                 [phrase "0.3-alpha4"]
                 [com.taoensso/tempura "1.2.1"]]
```

:::{.alert .alert-warning}
Don't forget to restart Figwheel as project changes aren't auto applied the same way that source changes are.
:::

Next we'll create a `src/cljc/validation_experiment/i18n.cljc` file with the following contents.

```Clojure
(ns validation-experiment.i18n
  #?(:clj  (:require [taoensso.tempura :as tempura]))
  #?(:cljs (:require [taoensso.tempura :as tempura]
                     [reagent.core :as reagent :refer [atom]])))

(defonce lang (atom :en))

(def ^:private translations
  {:en
   {:missing "en missing text"}})

(defn tr [resource-ids]
  (tempura/tr {:dict translations} [@lang :en] resource-ids))
```

We've used [Reader Conditionals](https://clojure.org/guides/reader_conditionals) to require differnt namespaces depending on whether we're compiling for Clojure or ClojureScript. Technically we only need ClojureScript for this series, so we could have created a `cljs` file instead of a `cljc` file, but we're showing how we could support both client and server side here. If we were compiling with Clojure the `atom` used to define `lang` would be a `clojure.core/atom`, but in ClojureScript it will be a `reagent.core/atom`.

Tempura's documentation recommends using a `partial` to simplify the call to `tr`, the function that gives us our translated messages. However, we want to control the language used through that `lang` atom we defined and using `partial` would create a closure that would not recognize changes to it. So, instead we declare our own `tr` that doesn't create a closure and passes the dictionary and language to `taoensso.tempura/tr`.

This is all we need to start localizing our "application".

## Internationalizing the UI

This is really simple. First, we'll add our i18n namespace to the namespace declaration in `core.cljs`.

```Clojure
(ns validation-experiment.core
  (:require [reagent.core :as reagent :refer [atom]]
            [validation-experiment.ssn :as ssn]
            [clojure.spec.alpha :as s]
            [phrase.alpha :refer [phrase-first defphraser]]
            [validation-experiment.i18n :refer [tr]]))
```

With that in place we can internationalize the one and only string we have (the label text).

```Clojure
(defn example []
  [:div [:label (tr ["Enter SSN: "]) [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]
   [:p {:style {:color "red"}} (phrase-first {} :validation-experiment.ssn/ssn @ssn)]])
```

OK, not so exciting as all we've done is internationalized and not localized the label. Let's see how we could localize it. First, you'll need to add a translation in `i18n.cljc`.

```Clojure
(def ^:private translations
  {:en
   {:missing "en missing text"
    :ui {:ssn-label "Please enter an SSN: "}}})
```

:::{.alert .alert-info}
While still English, we've used slightly different wording here so that the change to the UI is observable.
:::

Now, we also have to tell `tr` which translation key to use, so modify `core.cljs` like this.

```Clojure
(defn example []
  [:div [:label (tr [:ui/ssn-label "Enter SSN: "]) [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]
   [:p {:style {:color "red"}} (phrase-first {} :validation-experiment.ssn/ssn @ssn)]])
```

We've left in the untranslated text which demonstrates how easy it is to localize without concern for whether or not translations exist yet. However, we added the key `:ui/ssn-label` which tells tempura how to find the translation in our dictionary. With this in place you should notice the change to the UI.

Let's do the same sort of changes for the validation messages. You can do them piece meal just like we did the UI, but we'll just show the final changes here. First, here's the changes to `i18n.cljc`.

```Clojure
(def ^:private translations
  {:en
   {:missing "en missing text"
    :ui {:ssn-label "Please enter an SSN: "}
    :ssn {:invalid-format "Not a valid SSN format."
          :invalid-area "\"%1\" is not a valid area segment."
          :invalid-group "\"%1\" is not a valid group segment."
          :invalid-serial-number "\"%1\" is not a valid serial number."}}})

(defn tr
    ([resource-ids] (tr resource-ids nil))
    ([resource-ids resource-args] (tempura/tr {:dict translations} [@lang :en] resource-ids resource-args)))
```

We've modified our `tr` to also accept `resource-args` so we can format messages with arguments. We've also added several new resource messages to our dictionary. Note that we use sub-dictionaries to isolate messages along the same sort of lines that we isolate our functions with namespaces. So all of the `validation-experiment.ssn` related messages are in a `:ssn` sub-dictionary.

Resource messages we want to format, such as `:invalid-area` above are declared with numbered replacement symbols such as `%1`.

Here's the changes to `ssn.cljc`.

```Clojure
(defphraser valid-format?
  [_ _]
  (tr [:ssn/invalid-format "Not a valid SSN format."]))

(defphraser valid-area?
  [_ {:keys [val]}]
  (tr [:ssn/invalid-area "\"%1\" is not a valid area segment."] [(subs val 0 3)]))

(defphraser valid-group?
  [_ {:keys [val]}]
  (tr [:ssn/invalid-group "\"%1\" is not a valid group segment."] [(subs (digits val) 3 5)]))

(defphraser valid-serial-number?
  [_ {:keys [val]}]
  (tr [:ssn/invalid-serial-number "\"%1\" is not a valid serial number."] [(subs (digits val) 5 9)]))
```

## Localizing to German

Let's add another language translation. Here's the changes to `i18n.cljc`.

:::{.alert .alert-info}
These translations were made using Google Translate, just so you know whom to blame if you speak German and find the translation to be horrible.
:::

```Clojure
(def ^:private translations
  {:en
   {:missing "en missing text"
    :ui {:ssn-label "Please enter an SSN: "}
    :ssn {:invalid-format "Not a valid SSN format."
          :invalid-area "\"%1\" is not a valid area segment."
          :invalid-group "\"%1\" is not a valid group segment."
          :invalid-serial-number "\"%1\" is not a valid serial number."}}
   :de-DE
   {:missing "de-DE missing text"
    :ui {:ssn-label "Bitte geben Sie eine SSN ein: "}
    :ssn {:invalid-format "Kein gültiges SSN-Format."
          :invalid-area "\"%1\" ist kein gültiges Flächensegment."
          :invalid-group "\"%1\" ist kein gültiges Gruppensegment."
          :invalid-serial-number "\"%1\" ist keine gültige Seriennummer."}}})
```

With this in place you can view the translation by switching the `lang` from `:en` to `:de-DE` and back.

:::{.alert .alert-info}
Due to our use of `defonce` changing the value of `lang` in the source won't cause Figwheel to automatically update the UI so you'll have to refresh the page to see changes
:::

Let's make a change to the UI to allow the user to change the language. First, we need to update the namespace in `core.cljs`.

```Clojure
(ns validation-experiment.core
  (:require [reagent.core :as reagent :refer [atom]]
            [validation-experiment.ssn :as ssn]
            [clojure.spec.alpha :as s]
            [phrase.alpha :refer [phrase-first defphraser]]
            [validation-experiment.i18n :refer [tr lang]]))
```

Then we need to modify the code for our component

```Clojure
(defn- swap-lang [cur-lang]
  (if (= cur-lang :en)
    :de-DE
    :en))

(defn example []
  [:div [:label (tr [:ui/ssn-label "Enter SSN: "]) [:input {:type :text :value @ssn :on-change #(reset! ssn (-> % .-target .-value))}]]
   [:p {:style {:color "red"}} (phrase-first {} :validation-experiment.ssn/ssn @ssn)]
   [:p [:input {:type :button :value (if (= @lang :en) "German" "English") :on-click #(swap! lang swap-lang)}]]])
```

That's it! There's our complete example.

:::{.alert .alert-success}
The code for this part of the series can be found in the branch `part4` in the associated repo at https://github.com/wekempf/validation-experiment.
:::