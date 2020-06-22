# Unity-StyleCop

A StyleCop integration package for Unity.

## About StyleCop

StyleCop is an open source static code analysis tool that checks for code style
violations. The default rules were established by consolidating the preferences
of various teams across Microsoft and are designed to emphasize readability and
maintainability of C# code, but users can also customize their own rules

The version of StyleCop used by this system is based on the new [StyleCop.Analyzer
package.](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) vs using the old
StyleCop convention so that its portable to Rider as well as Visual Studio for
Windows & Mac OS. **It does not work on VS Code as it doesn't support it as**
**far as I am aware**

## Installing this Package

- Open your preferred command line window and navigate to the root of your unity
project.
- Navigate to the `Packages` folder.
- Clone this repository as follows:

```bash
git clone https://github.com/ImLp/Unity-StyleCop
```

The package is now installed and ready to go.

## Using This Package

After integrating this package to your project, it will automatically add itself
to all detected `*.csproj` in your project ensuring all code is covered.

This package includes a `UnityStyleCop.ruleset` file which drives all the rules
being applied. This can be edited to match the team stylistic preference.
All StyleCop rules have been annotated for convenience.

## Why Coding Style Matters - An opinion

Good quality code is not based just in its execution. Good quality code is a readable,
maintainable source that any engineer could understand and modify without requiring
arcane knowledge about its functionality to fix / improve something.

There is no magic bullet when it comes to a package that would make code be good
quality code. Nonetheless if proper standards are followed you can get very close
to a source base where you can develop with ease. StyleCop is a heavy handed approach,
but it does push you towards embracing:

- Correct documentation of Public APIs between classes ensuring readability and
better maintainability.
- Enforcement of correct encapsulation by discouraging use of `public` members.
- Consistent naming conventions for readability.
- Implicit enforcement of single responsibility principles by discouraging multiple
classes in a single file (default rules).

Unity encourages c# anti-patterns that simply lead to problematic code. Let's do
better.