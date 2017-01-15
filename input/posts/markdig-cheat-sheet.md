---
Title: "Markdig Cheat Sheet"
Published: 01/08/2017 17:17:42
Tags: ["Wyam", "Markdig"]
---

# Markdig Cheat Sheet

I mentioned in the post [Alerts in Wyam](/posts/alerts-in-wyam) that Wyam uses Markdig for Markdown
processing, and that Markdig supports extensions. Currently, Markdig lacks documentation. For this
reason, I'm going to try and create a "cheat sheet" for the extensions.

## Pipe Tables

tag                        | docs
---------------------------|----------------------------------------------------
pipetables --or-- advanced | http://pandoc.org/MANUAL.html#extension-pipe_tables

### Code

```text
Right | Left | Default | Center
-----:|:-----|---------|:-----:
12    | 12   | 12      | 12
123   | 123  | 123     | 123
1     | 1    | 1       | 1
```

### Result

Right | Left | Default | Center
-----:|:-----|---------|:-----:
12    | 12   | 12      | 12
123   | 123  | 123     | 123
1     | 1    | 1       | 1

---

## Grid Tables

tag                        | docs
---------------------------|----------------------------------------------------
gridtables --or-- advanced | http://pandoc.org/MANUAL.html#extension-grid_tables

### Code

```text
+-------+------+---------+--------+
| Right | Left | Default | Center |
+======:+:=====+=========+:======:+
| 12    | 12   | 12      | 12     |
+-------+------+---------+--------+
| 123   | 123  | 123     | 123    |
+-------+------+---------+--------+
| 1     | 1    | 1       | 1      |
+-------+------+---------+--------+
```

### Result [^1]

+-------+------+---------+--------+
| Right | Left | Default | Center |
+======:+:=====+=========+:======:+
| 12    | 12   | 12      | 12     |
+-------+------+---------+--------+
| 123   | 123  | 123     | 123    |
+-------+------+---------+--------+
| 1     | 1    | 1       | 1      |
+-------+------+---------+--------+

## Extra Emphasis

tag                            | docs
-------------------------------|----------------------------------------------------
emphasisextras --or-- advanced | http://pandoc.org/MANUAL.html#strikeout --and-- https://markdown-it.github.io/

### Code

```text
~~strike through~~ example

~subscript~ example

^superscript^ example

++inserted++ example

==marked== example
```

### Result

~~strike through~~ example

~subscript~ example

^superscript^ example

++inserted++ example

==marked== example

## Special Attributes

tag                        | docs
---------------------------|----------------------------------------------------
attributes --or-- advanced | https://michelf.ca/projects/php-markdown/extra/#spe-attr

### Code

```text
# Header 1 {#header1}

[Link](#header1) back to header1

# Header 2 {.my-class}

# Le Site {lang=fr}
```

### Result [^2]

# Header 1 {#header1}

[Link](#header1) back to header1

# Header 2 {.my-class}

# Le Site {lang=fr}

## Definition Lists

tag                             | docs
--------------------------------|----------------------------------------------------
definitionlists --or-- advanced | https://michelf.ca/projects/php-markdown/extra/#def-list

### Code

```text
Apple
: Pomaceous fruit of plants of the genus Malus in 
  the family Rosaceae.

Orange
: The fruit of an evergreen tree of the genus Citrus.
```

### Result [^2]

Apple
: Pomaceous fruit of plants of the genus Malus in 
  the family Rosaceae.

Orange
: The fruit of an evergreen tree of the genus Citrus.

## Footnotes

tag                       | docs
--------------------------|----------------------------------------------------
footnotes --or-- advanced | https://michelf.ca/projects/php-markdown/extra/#footnotes

### Code

```text
This is an example[^3] of footnote usage.

[^3]: An example footnote.
```

### Result

This is an example[^3] of footnote usage.

## Auto-identifiers

tag                             | docs
--------------------------------|----------------------------------------------------
autoidentifiers --or-- advanced | http://pandoc.org/MANUAL.html#extension-auto_identifiers

### Code

```text
# Automatic Header identifiers

[Link](#automatic-header-identifiers) to header.
```

### Result

# Automatic Header identifiers

[Link](#automatic-header-identifiers) to header.

## Auto-links

tag                       | docs
--------------------------|----------------------------------------------------
autolinks --or-- advanced | generates links if a text starts with ```http://``` or ```https://``` or ```ftp://``` or ```mailto:``` or ```www.xxx.yyy```

### Code

```text
http://www.digitaltapestry.net
```

### Result

http://www.digitaltapestry.net

## Task Lists

tag                       | docs
--------------------------|----------------------------------------------------
tasklists --or-- advanced | https://github.com/blog/1375-task-lists-in-gfm-issues-pulls-comments

### Code

```text
- [ ] a task list item
- [ ] list syntax required
- [ ] normal **formatting**
- [ ] incomplete
- [x] completed
```

### Result

- [ ] a task list item
- [ ] list syntax required
- [ ] normal **formatting**
- [ ] incomplete
- [x] completed

## Extra Bullet Lists

tag                        | docs
---------------------------|----------------------------------------------------
listextras --or-- advanced | supporting alpha bullet a. b. and roman bullet (i, ii...etc.)

### Code

```text
1. Level 1
    i. Level i
        a. Level a
```

### Result

1. Level 1
    i. Level i
        a. Level a

## Media Support

tag                        | docs
---------------------------|----------------------------------------------------
medialinks --or-- advanced | https://talk.commonmark.org/t/embedded-audio-and-video/441

### Code

```text
![Rick Roll](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
```

### Result

![Rick Roll](https://www.youtube.com/watch?v=dQw4w9WgXcQ)

## Abbreviations

tag                           | docs
------------------------------|----------------------------------------------------
abbreviations --or-- advanced | https://michelf.ca/projects/php-markdown/extra/#abbr

### Code

```text
*[HTML]: Hypertext Markup Language

Later in a text we are using HTML and it becomes an abbr tag HTML
```

### Result

*[HTML]: Hypertext Markup Language

Later in a text we are using HTML and it becomes an abbr tag HTML

## Citation

tag                       | docs
--------------------------|----------------------------------------------------
citations --or-- advanced | https://talk.commonmark.org/t/referencing-creative-works-with-cite/892

### Code

```text
""Tractatus Logico-Philosophicus"" was first published in 1921.
```

### Result

""Tractatus Logico-Philosophicus"" was first published in 1921.

## Custom Containers

tag                              | docs
---------------------------------|----------------------------------------------------
customcontainers --or-- advanced | https://talk.commonmark.org/t/custom-container-for-block-and-inline/2051

### Code

```text
:::{.alert .alert-info}
This is a Bootstrap alert.
:::
```

### Result

:::{.alert .alert-info}
This is a Bootstrap alert.
:::

## Figures

tag                     | docs
------------------------|----------------------------------------------------
figures --or-- advanced | https://talk.commonmark.org/t/image-tag-should-expand-to-figure-when-used-with-title/265/5

### Code

```text
![Kookburra](/kookaburra.jpg "Kookburra")
![Pelican](/pelican.jpg "Pelican")
![Cheeky looking Rainbow Lorikeet](/lorikeet.jpg "Rainbow Lorikeet")
```

### Result

![Kookburra](https://upload.wikimedia.org/wikipedia/commons/7/79/Kookaburra_melb.jpg "Kookburra"){ width=100 height=100 }
![Pelican](http://carolinabirds.org/Daniels/Florida/FLlg/Pelican,%20White%20Sanibel2.jpg "Pelican"){ width=100 height=100 }
![Cheeky looking Rainbow Lorikeet](http://images.fineartamerica.com/images-medium-large/female-rainbow-lorikeet--trichoglossus-haematodus-life-on-white.jpg "Rainbow Lorikeet"){ width=100 height=100 }

## Footers

tag                     | docs
------------------------|----------------------------------------------------
footers --or-- advanced | https://talk.commonmark.org/t/syntax-for-footer/2070

### Code

```text
> This is a blockquote
> ^^ This is a ""citation for name""
```

### Result

> This is a blockquote
> ^^ This is a ""citation for name""

## Mathematics

tag                         | docs
----------------------------|----------------------------------------------------
mathematics --or-- advanced | https://talk.commonmark.org/t/mathematics-extension/457/31

### Code

```text
This is a $math block$

$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
$$
```

### Result [^2] [^4]

This is a $math block$

$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
$$

## Emoji

tag    | docs
-------|----------------------------------------------------
emojis | https://markdown-it.github.io/

### Code

```text
It's easy to use emojis. ;) That's cool! 8-)
```

### Result

It's easy to use emojis. ;) That's cool! 8-)

## SmartyPants

tag         | docs
------------|----------------------------------------------------
smartypants | https://daringfireball.net/projects/smartypants/

### Code

```text
This is a "text"

This is a 'text'

This is a <<text>>

This is a "text

This is a "text 'with" a another text'

This is a -- text

This is a --- text

This is a en ellipsis...
```

### Result

This is a "text"

This is a 'text'

This is a <<text>>

This is a "text

This is a "text 'with" a another text'

This is a -- text

This is a --- text

This is a en ellipsis...

## Diagrams

tag                         | docs
----------------------------|----------------------------------------------------
diagrams --or-- advanced | https://knsv.github.io/mermaid/

### Code

```text
    ```mermaid
    graph TD;
        A-->B;
        A-->C;
        B-->D;
        C-->D;
    ```
```

### Result [^2] [^5]

```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```

---

[^1]: Alignment doesn't appear to be working, and I don't now why.
[^2]: Not all results are visibly evident with the current styling. View the HTML produced.
[^3]: An example footnote.
[^4]: I'll be honest, I don't understand the use case for this one.
[^5]: This needs mermaid.css and mermaid.js to be linked. I've done so here, but the way you do it will be
      improved the next release, so I'll wait to blog about that. Not sure why the generated diagram here
      is missing the arrow lines, but I don't believe it's got anything to do with Wyam or Markdig.