# Cake.Prompt
Cake AddIn that extends Cake with interactive user prompts.

[![Build status](https://megakid.visualstudio.com/Cake.Prompt/_apis/build/status/Cake.Prompt)](https://megakid.visualstudio.com/Cake.Prompt/_build/latest?definitionId=1)

## Usage

```c#
NuGetPush(packagePath, new NuGetPushSettings {
    ApiKey = Prompt("Enter api key for NuGetPush:")
});
```

```c#
var continue = Prompt("Do you want to continue? [Y/N]", "N").Trim().ToUpperInvariant() == "Y";
if (continue)
    ...
```

The `Prompt()` method throws a `TimeoutException` after the supplied timeout duration (Defaults to 30 seconds).
