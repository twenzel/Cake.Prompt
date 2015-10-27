# Cake.Prompt
Cake AddIn that extends Cake with interactive user prompts.

## Usage

```c#
NuGetPush(packagePath, new NuGetPushSettings {
    ApiKey = Prompt("Enter api key for NuGetPush:")
});
```

The `Prompt()` method throws an Exception if `cake.exe` is not called with `-interactive=true`:
```
cake.exe build.cake -interactive=true
```
