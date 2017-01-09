---
Title: "Less in Wyam"
Published: 01/08/2017 13:25:42
Tags: ["Wyam", "CSS", "LESS"]
---

# Less in Wyam

In the [last post](/posts/alerts-in-wyam) I showed how to use Bootstrap alerts from
within a Wyam based blog site. I included some CSS to restyle the alerts to include
glyphicons for the various alert types. The CSS I used had a lot of repetitive code.
The [Less](http://lesscs.org) CSS preprocessor makes it easy to use mixins to eliminate
duplicate code in CSS files. Wyam has a module for using Less, so I updated my site
to use it.

The first thing I had to do was modify my config.wyam file.

```csharp
#n -p Wyam.Less
#n -p Wyam.Minification

Settings.Host = "digitaltapestry.net";
GlobalMetadata["Title"] = "Digital Tapestry";
GlobalMetadata["Description"] = "Weaving a Digital Tapestry";
GlobalMetadata["Intro"] = "The musings of a .NET developer.";

Pipelines.Add("Less",
    ReadFiles("**/*.less"),
    Less(),
    MinifyCss(),
    WriteFiles(".css")
);
```

Obviously this is specific to my site. You'll need the ```#n -p Wyam.Less```, ```#n -p Wyam.Minification```
and the call to ```Pipelines.Add``` while the rest is site specific. ```MinifyCss``` is optional. it
minifies the output CSS, which is a good idea for "production", but you may not want to do it while
working on the CSS.

With that in place I created a ```input\assets\css\override.less``` with the following
code.

```css
.alert {
    margin-left: 10px;
}
.alert > p {
    margin-top: 0px;
}
.alert-glyphicon {
    position: relative;
    top: 1px;
    float: left;
    margin-right: 8px;
    font-family: 'Glyphicons Halflings';
    font-style: normal;
    font-weight: normal;
    line-height: 1;
    -webkit-font-smoothing: antialiased;
}
.alert-success:before {
    content: "\e125";
    .alert-glyphicon;
}
.alert-info:before {
    content: "\270f";
    .alert-glyphicon;
}
.alert-warning:before {
    content: "\e107";
    .alert-glyphicon;
}
.alert-danger:before {
    content: "\e074";
    .alert-glyphicon;
}
```

This is simpler than the original CSS thanks to the ```.alert-glyphicon``` mixin.

If you've already created a ```input\assets\css\override.css``` you'll have to delete
it. Now when you build the site, ```override.less``` gets compiled to ```override.css```.
This does still copy the ```override.less``` file to the output as well. ~~~If I figure
out how to remove that, I'll update this post.~~~

Update: Dave Glick, the awesome author of Wyam, solved the issue of the LESS file being copied for me on
[Gitter](https://gitter.im/Wyamio/Wyam). He answers inquiries on there so quickly that I suspect he's
actually an AI robot. ;)

Here's the code to add to remove LESS files from being copied to the output.

```csharp
Pipelines[BlogPipelines.Resources].Clear();
Pipelines[BlogPipelines.Resources].Add(
    CopyFiles("**/*{!.cshtml,!.md,!.less,}")
);
```