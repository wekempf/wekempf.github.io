---
Title: "PoshWyam"
Published: 01/23/2017 17:18:00
Tags: ["Wyam", "PowerShell"]
---

# PoshWyam

Over on the [Wyam Gitter](https://gitter.im/Wyamio/Wyam) [Jamie Phillips (@phillipsj)](https://github.com/phillipsj)
made a casual remark about using my PowerShell script I use in this blog's source for creating new blog posts, and
how he was trying to modify it for draft usage. That blossomed into a whole new project under the Wyamio organization
called [PoshWyam](https://github.com/Wyamio/PoshWyam) that I've been hacking at for a few days now. It's in preliminary
(read "alpha", not even "beta") state right now, but it is ready enough for people to use and provide feedback.
Once we are ready this will go up on [PowerShell Gallery](https://www.powershellgallery.com/), but until then
if you want to use it you'll need to clone the repo.

Here's a whirlwind tour for usage, prior to release.

```PowerShell
import-module PoshWyam\PoshWyam.psd1
```

You need to import the module from the location in which you cloned the repo (alternatively, you could copy the
PoshWyam directory from the repo into your PowerShell module path somewhere).

```PowerShell
Get-Command -Module PoshWyam
```

View all of the fabulous commands exported by this new module.

```PowerShell
Get-Help New-blog
```

View the help for any of these new commands. Keep in mind that we're alpha at best here. I know I'm missing
example documentation for all the commands, for instance. That said, every command is documented. If you find
anything missing or confusing provide feedback, please.

```PowerShell
New-Blog "My Blog" http://www.myblog.com
```

Create a new blog. Note that the example above is the minimal syntax for doing so. There's other parameters
you can supply, all documented with `Get-Help`. This new blog includes a `Cake` build script, so you're ready
to go right out of the box, no installation of any tools necessary. The `Cake` bootstrapper, `build.ps1` installs
it locally to this project and the `wyam.cake` script will install `Wyam` locally.

```PowerShell
Invoke-Wyam
```

If you have some need to run `wyam.exe` you can use the `Invoke-Wyam` cmdlet to do so. This bootstraps the local
installation of `Wyam` if it's not yet been done and then invokes it with the arguments you pass.

```PowerShell
New-BlogPost "My Second Post" Tag1,Tag2
```

This creates a new blog post with the specified Title and Tags. You can use the `-Draft` switch to specify the
creation of a draft post instead. You can also get creative and pipe this command to `Invoke-Item` to open
the markdown file in the associated editor on your system, just to go crazy with it.

```PowerShell
Get-BlogPost
```

This gets all the blog posts, sorted by the Published date.

```PowerShell
Get-BlogPost *Second*
```

You can supply a `-Title` parameter, including wild cards, to limit the search to just those posts that match.

```PowerShell
Get-BlogPost *Second* | Get-Content
```

The output objects for `Get-BlogPost` are designed specifically for piping to other PowerShell cmdlets that
take a `-Path` parameter by property name. This means most of the cmdlets you'd want to use it with will
"just work" as you expect. Don't be fooled by the default output of this command. We use a type formatting
file to make the output from `Get-BlogPost` look nice and clean, but there are properties, specifically
a `Path` property, that are present but not displayed by default.

```PowerShell
Publish-BlogDraft Title
```

Doesn't do you much good to create draft posts if you can't easily publish them later. Do note that
currently Wyam doesn't understand the concept of drafts, so the drafts created here cannot be viewed
even by using the preview server until they are published. This is a feature that
[Dave Glick (@daveaglick)](https://github.com/daveaglick) might add to `Wyam` in the future.

Those are the main commands, at least as of right now. Have other command ideas you'd like to see?
Found a bug? Want to participate in the development? Let us know on [GitHub](https://github.com/Wyamio/PoshWyam)
or [Gitter](https://gitter.im/Wyamio/PoshWyam).