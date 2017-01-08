---
Title:  "Alerts In Wyam"
Published: 2017-01-08 11:02:00
Tags: ["Markdown", "Markdig", "Wyam", "CSS"]
---

# Alerts in Wyam

Bootstrap has the concept of "alerts", which are specially styled divs used to call out notes.
Here's some examples:

:::{.alert .alert-success}
A success alert.
:::

:::{.alert .alert-info}
An info alert.
:::

:::{.alert .alert-warning}
A warning alert.
:::

:::{.alert .alert-danger}
A danger alert.
:::

I wanted to be able to add these alerts to a Markdown document processed by [Wyam](https://wyam.io).
It took me a while to figure this out. Wyam uses [Markdig](https://github.com/lunet-io/markdig/)
for the Markdown processor, which includes several extensions. Which extensions are used is
controllable via the global metadata `MarkdownExtensions` property, which defaults to "advanced+bootstrap".
The "advanced" part of that includes many extensions, but the important one here is the extension
for custom containers and special attributes. Combined, they let you code an alert like this:

```markdown
:::{.alert .alert-warning}
A warning alert.
:::
```

The `:::` is similar to a code fence, but defines a custom container (a div), while ```{.alert .alert-warning}```
specifies the CSS classes to add to the element.

I wanted to modify the styling for the alerts a bit, in order to include a glyphicon in every such alert.
So, I created a ```input\assets\css\override.css``` file to replace the file in the theme with the following
contents:

```css
.alert {
    margin-left: 10px;
}
.alert-success:before {
    position: relative;
    top: 1px;
    float: left;
    margin-right: 8px;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: normal;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
    content: "\e125";
}
.alert-info:before {
    position: relative;
    top: 1px;
    float: left;
    margin-right: 8px;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: normal;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
    content: "\270f";
}
.alert-warning:before {
    position: relative;
    top: 1px;
    float: left;
    margin-right: 8px;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: normal;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
    content: "\e107";
}
.alert-danger:before {
    position: relative;
    top: 1px;
    float: left;
    margin-right: 8px;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: normal;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
    content: "\e074";
}
.alert > p {
    margin-top: 0px;
}
```

The examples I showed at the top of this post were created using the Markdown from the example and styled
with the above CSS.