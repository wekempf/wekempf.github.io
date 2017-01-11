---
Title: "Wyam Anchors and Code Copy"
Published: 01/11/2017 07:34:28
Tags: ["Wyam", "CSS", "JavaScript", "AnchorJS", "clipboard.js"]
---

# Wyam Anchors and Code Copy

On the [Wyam](https://wyam.io) [Gitter](https://gitter.im/Wyamio/Wyam) discussion Gary Ewan Park (@gep13 [^1])
asked the following:

> If you look at a GitHub wiki page, say this one...

> when you hover over the headers, you get this:

> ![Anchor Button](https://files.gitter.im/Wyamio/Wyam/ZknP/blob "Anchor Button")

> which allows you to click the link to the header. Markdig is already generating the link to the header, but is there an easy way to
> add the showing of the link on hover, and to allow copying from the address bar?

Dave Glick (@daveglick [^2]), the author of Wyam, responded with:

> There is - in fact strongly considering adding this and code block copy buttons to the themes (it's all frontend JS stuff) - here's
> the two libraries I like the best: [https://github.com/bryanbraun/anchorjs](https://github.com/bryanbraun/anchorjs) (for anchor links)
> and [https://github.com/zenorocha/clipboard.js](https://github.com/zenorocha/clipboard.js) (for clipboard copying)

> Adding them to your own site would just be a matter of putting the appropriate JS in the _Scripts.cshtml file

So, the three of us set off independently to see if we could figure out how to get these working. @daveglick [^2] intends to add
support for this to the existing themes, I believe, so most of what I write here won't be relevant to anyone in the future,
but I'm going to go ahead and record how I got it working for posterity. There's still some tips you can glean from what I found
when you're trying to extend an existing Wyam theme on your own.

## AnchorJS

This was the easy one. In fact, @gep13 [^1] and @daveglick [^2] had this one done before I started looking into it. Anyway,
this is strictly a JavaScript bit of magic. Here's how you can get it to work.

If you don't already have a ```_Scripts.cshtml``` override file created in your input folder, do so. This is a "hook" into
the theme that allows you to add your own JavaScript to every page. For AnchorJS, this is what I added.

```JavaScript
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/anchor-js/3.2.2/anchor.min.js"></script>
<script type="text/javascript">
    anchors.options.placement = 'left';
    anchors.add();
</script>
```

That's it. Nothing else needs to be done. I will point out there's one impact to doing this that I don't care for. This
will add the anchor button to the ```h1``` tag in the header of the page. The one inside the blog's banner. This one
serves no purpose, really, and I think it looks bad. Some CSS or JavaScript magic could probably fix this easy enough,
but I've not done that work.

## clipboard.js

This one was more difficult. The idea is we want a button to appear with every code block that when clicked copies
the code in that block. Since clipboard.js is really just about adding clipboard support through JavaScript, and not
explicitly about adding copy buttons to code blocks, the [instructions](https://clipboardjs.com/) on the site only gets
you so far. However, that page does do exactly what we want. So, I figured I could just "steal" the code from the
page source.

I used Edge to do this.

:::{.alert .alert-info}
Yeah, I'm that guy. The lone man daring to use the browser no one else will touch. I'm going to describe how **I**
did this, using Edge. You'll have to figure out how to do the same thing in your browser of choice... they all
have similar capabilities.
:::

I right clicked on one of the code blocks and selected "Inspect Element" to get into the developer tools at
the proper location. By inspecting the HTML for this element I was able to find the CSS classes to search for.
Edge's developer tools allow you to search all the source files, including linked JavaScript and CSS files.
So, using this I was able to find both the JavaScript file(s) involved here as well as the CSS file(s).
Unfortunately for us humans, all of these files were minified, making the entirely unreadable. I figured
there must be some online tools for "unmininfying" both, so I went to Bing (don't shame me) and typed
both "format JavaScript" and "format CSS" into the search. On Bing, the top entry for both is a tool
to format the JavaScript/CSS right there in the search results. Handy to know.

:::{.alert .alert-info}
In case you're just figuring it out, I'm not exactly a web developer. I've done web development professionally,
but that was 12 years ago. Or, if you count XUL based desktop development, 10 years ago. I'm not up
on the latest web development technologies and tools.
:::

The JavaScript was spread across a few files, so it took a while to locate everything, but it wasn't really
so bad. That site uses generic CSS class names that I'm concerned could clash, so I modified the code
slightly for my own purposes. Here's what I came up with for the JavaScript.

```JavaScript
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/clipboard.js/1.5.16/clipboard.min.js"></script>
<script type="text/javascript">
    var snippets = document.querySelectorAll("pre > code");
    [].forEach.call(snippets, function(snippet) {
        snippet.insertAdjacentHTML("beforebegin", "<button class='btn-copy' data-clipboard-snippet><img class='clippy' width=13 src='/assets/images/clippy.svg' alt='Copy to clipboard'></button>");
    });
    var clipboardSnippets = new Clipboard('[data-clipboard-snippet]', {
        target: function(trigger) {
            return trigger.nextElementSibling;
        }
    });
    clipboardSnippets.on('success', function(e) {
        e.clearSelection();
        showTooltip(e.trigger, "Copied!");
    });
    clipboardSnippets.on('error', function(e) {
        showTooltip(e.trigger, fallbackMessage(e.action));
    });

    var btns = document.querySelectorAll('.btn-copy');
    for (var i = 0; i < btns.length; i++) {
        btns[i].addEventListener('mouseleave', function(e) {
            e.currentTarget.setAttribute('class', 'btn-copy');
            e.currentTarget.removeAttribute('aria-label');
        });
    }

    function showTooltip(elem, msg) {
        elem.setAttribute('class', 'btn-copy tooltipped tooltipped-s');
        elem.setAttribute('aria-label', msg);
    }

    function fallbackMessage(action) {
        var actionMsg = '';
        var actionKey = (action === 'cut' ? 'X' : 'C');
        if (/iPhone|iPad/i.test(navigator.userAgent)) {
            actionMsg = 'No support :(';
        } else if (/Mac/i.test(navigator.userAgent)) {
            actionMsg = 'Press ⌘-' + actionKey + ' to ' + action;
        } else {
            actionMsg = 'Press Ctrl-' + actionKey + ' to ' + action;
        }
        return actionMsg;
    }
    hljs.initHighlightingOnLoad();
</script>
```

I'll be honest here, I don't know what the purpose of the ```hljs.initHighlightingOnLoad``` is. I've
kept it from the code I stole, but I should really figure out why it's there at some point.

The CSS was trickier. Like the JavaScript, it was spread out through multiple files. The bigger issue,
though, was that much of it was specific to this site, and not necessarily something we'd need ourselves.
So, I tried to eliminate everything that's not necessary. My CSS skills are a bit lacking, though,
so I may have eliminated too much and/or kept things that are unecessary. However, as you can see
it does work. Long term, it may be that it would benefit from some tweaking. In particular, I'll point
out this all relies on an SVG image for the copy button. I'd like to make this use a glyphicon instead.
Anyway, here's the CSS I've used.

```CSS
.btn-copy[disabled] .clippy {
    opacity: .3;
}
pre .btn-copy {
    -webkit-transition: opacity 0.3s ease-in-out;
    -o-transition: opacity 0.3s ease-in-out;
    transition: opacity 0.3s ease-in-out;
    opacity: 0;
    padding: 2px 6px;
    float: right;
}
pre:hover .btn-copy {
    opacity: 1;
}
.tooltipped {
    position: relative
}
.tooltipped:after {
    position: absolute;
    z-index: 1000000;
    display: none;
    padding: 5px 8px;
    font: normal normal 11px/1.5 Helvetica, arial, nimbussansl, liberationsans, freesans, clean, sans-serif, "Segoe UI Emoji", "Segoe UI Symbol";
    color: #fff;
    text-align: center;
    text-decoration: none;
    text-shadow: none;
    text-transform: none;
    letter-spacing: normal;
    word-wrap: break-word;
    white-space: pre;
    pointer-events: none;
    content: attr(aria-label);
    background: rgba(0, 0, 0, 0.8);
    border-radius: 3px;
    -webkit-font-smoothing: subpixel-antialiased
}
.tooltipped:before {
    position: absolute;
    z-index: 1000001;
    display: none;
    width: 0;
    height: 0;
    color: rgba(0, 0, 0, 0.8);
    pointer-events: none;
    content: "";
    border: 5px solid transparent
}
.tooltipped:hover:before, .tooltipped:hover:after, .tooltipped:active:before, .tooltipped:active:after, .tooltipped:focus:before, .tooltipped:focus:after {
    display: inline-block;
    text-decoration: none
}
.tooltipped-s:after, .tooltipped-se:after, .tooltipped-sw:after {
    top: 100%;
    right: 50%;
    margin-top: 5px
}
.tooltipped-s:before, .tooltipped-se:before, .tooltipped-sw:before {
    top: auto;
    right: 50%;
    bottom: -5px;
    margin-right: -5px;
    border-bottom-color: rgba(0, 0, 0, 0.8)
}
```

This goes in ```override.css``` in your input directory (create one if you haven't already). Or, like me, you can get creative
and use [Less](http://lesscss.org), which I've [blogged about](/posts/less-in-wyam.md).

That's it. If @daveglick [^2] hasn't added this to the themes by the time you read this, that's how you can make this work.

[^1]: [Gary Ewan Park (@gep13)](https://github.com/gep13)
[^2]: [Dave Glick (@daveglick)](https://github.com/daveaglick)