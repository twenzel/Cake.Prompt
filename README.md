# Cake.Prompt
Cake AddIn that extends Cake with interactive user prompts.

[![.NET](https://github.com/cake-contrib/Cake.Prompt/actions/workflows/dotnet.yml/badge.svg)](https://github.com/cake-contrib/Cake.Prompt/actions/workflows/dotnet.yml) 
[![NuGet](https://img.shields.io/nuget/v/Cake.Prompt?style=flat-square)](https://www.nuget.org/packages/Cake.prompt)

## Definition

```c#
/// <summary>
/// Prompts the user for input.
/// </summary>
/// <param name="context">The context.</param>
/// <param name="message">The message which is shown to the user.</param>
/// <param name="defaultResult">Value supplied if the user simply returns with no input</param>
/// <param name="timeout">Timeout time, defaults to 30 seconds</param>
/// <returns>The user input.</returns>
string Prompt(this ICakeContext context, string message, string defaultResult = default, TimeSpan timeout = default);
```

## Usage

```c#
NuGetPush(packagePath, new NuGetPushSettings {
    ApiKey = Prompt("Enter api key for NuGetPush:")
});
```

```c#
var shouldContinue = Prompt("Do you want to continue? [Y/N]", "N").Trim().ToUpperInvariant() == "Y";
if (shouldContinue)
{
    ...
}
```

The `Prompt()` method throws a `TimeoutException` after the supplied timeout duration (Defaults to 30 seconds).
