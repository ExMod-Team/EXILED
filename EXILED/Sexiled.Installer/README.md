### TL;DR

Sexiled.Installer - SEXILED online installer. Downloads the latest release from the GitHub repository and installs it.

#### Usage

```
Usage:
  Sexiled.Installer [options] [[--] <additional arguments>...]]

Options:
  -p, --path <path> (REQUIRED)         Path to the folder with the SL server [default: YourWorkingFolder]
  --appdata <appdata> (REQUIRED)       Forces the folder to be the AppData folder (useful for containers when pterodactyl runs as root) [default: YourAppDataPath]
  --sexiled <sexiled> (REQUIRED)         Indicates the Sexiled root folder [default: YourAppDataPath]
  --pre-releases                       Includes pre-releases [default: False]
  --target-version <target-version>    Target version for installation
  --github--token <github--token>      Uses a token for auth in case the rate limit is exceeded (no permissions required)
  --exit                               Automatically exits the application anyway
  --get-versions                       Gets all possible versions for installation
  --version                            Show version information
  -?, -h, --help                       Show help and usage information

Additional Arguments:
  Arguments passed to the application that is being run.
```

-----

#### Examples

- ##### Basic installation in the folder you are in

```
user@user:~/SCP# ./Sexiled.Installer-Linux --pre-releases
Sexiled.Installer-Linux-3.2.3.0
AppData folder: YourAppDataPath
Sexiled folder: YourAppDataPath
Receiving releases...
Prereleases included - True
Target release version - (null)
Searching for the latest release that matches the parameters...
Trying to find release..
Release found!
PRE: True | ID: 87710626 | TAG: 6.0.0-beta.18
Asset found!
ID: 90263995 | NAME: Sexiled.tar.gz | SIZE: 1027928 | URL: https://api.github.com/repos/ExMod-Team/Sexiled-EA/releases/assets/90263995 | DownloadURL: https://github.com/ExMod-Team/Sexiled-EA/releases/download/6.0.0-beta.18/Sexiled.tar.gz
Processing 'SEXILED/Plugins/dependencies/0Harmony.dll'
Extracting '0Harmony.dll' into 'YourAppDataPath/SEXILED/Plugins/dependencies/0Harmony.dll'...
Processing 'SEXILED/Plugins/dependencies/Sexiled.API.dll'
Extracting 'Sexiled.API.dll' into 'YourAppDataPath/SEXILED/Plugins/dependencies/Sexiled.API.dll'...
Processing 'SEXILED/Plugins/dependencies/SemanticVersioning.dll'
Extracting 'SemanticVersioning.dll' into 'YourAppDataPath/SEXILED/Plugins/dependencies/SemanticVersioning.dll'...
Processing 'SEXILED/Plugins/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into 'YourAppDataPath/SEXILED/Plugins/dependencies/YamlDotNet.dll'...
Processing 'SEXILED/Plugins/Sexiled.CreditTags.dll'
Extracting 'Sexiled.CreditTags.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.CreditTags.dll'...
Processing 'SEXILED/Plugins/Sexiled.CustomItems.dll'
Extracting 'Sexiled.CustomItems.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.CustomItems.dll'...
Processing 'SEXILED/Plugins/Sexiled.CustomRoles.dll'
Extracting 'Sexiled.CustomRoles.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.CustomRoles.dll'...
Processing 'SEXILED/Plugins/Sexiled.Events.dll'
Extracting 'Sexiled.Events.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.Events.dll'...
Processing 'SEXILED/Plugins/Sexiled.Permissions.dll'
Extracting 'Sexiled.Permissions.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.Permissions.dll'...
Processing 'SEXILED/Plugins/Sexiled.Updater.dll'
Extracting 'Sexiled.Updater.dll' into 'YourAppDataPath/SEXILED/Plugins/Sexiled.Updater.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Sexiled.API.dll'
Extracting 'Sexiled.API.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Sexiled.API.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/Sexiled.Loader.dll'
Extracting 'Sexiled.Loader.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/Sexiled.Loader.dll'...
Installation complete
```

- ##### Installation in a specific folder, specific version and specific appdata folder

```
user@user:~/SCP# ./Sexiled.Installer-Linux --appdata /user/SCP --sexiled /user/SCP
Sexiled.Installer-Linux-3.2.3.0
AppData folder: /user/SCP
Sexiled folder: /user/SCP
Receiving releases...
Prereleases included - False
Target release version - (null)
Searching for the latest release that matches the parameters...
Trying to find release..
Release found!
PRE: False | ID: 87710626 | TAG: 6.0.0-beta.18
Asset found!
ID: 90263995 | NAME: Sexiled.tar.gz | SIZE: 1027928 | URL: https://api.github.com/repos/ExMod-Team/Sexiled-EA/releases/assets/90263995 | DownloadURL: https://github.com/ExMod-Team/Sexiled-EA/releases/download/6.0.0-beta.18/Sexiled.tar.gz
Processing 'SEXILED/Plugins/dependencies/0Harmony.dll'
Extracting '0Harmony.dll' into '/user/SCP/SEXILED/Plugins/dependencies/0Harmony.dll'...
Processing 'SEXILED/Plugins/dependencies/Sexiled.API.dll'
Extracting 'Sexiled.API.dll' into '/user/SCP/SEXILED/Plugins/dependencies/Sexiled.API.dll'...
Processing 'SEXILED/Plugins/dependencies/SemanticVersioning.dll'
Extracting 'SemanticVersioning.dll' into '/user/SCP/SEXILED/Plugins/dependencies/SemanticVersioning.dll'...
Processing 'SEXILED/Plugins/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into '/user/SCP/SEXILED/Plugins/dependencies/YamlDotNet.dll'...
Processing 'SEXILED/Plugins/Sexiled.CreditTags.dll'
Extracting 'Sexiled.CreditTags.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.CreditTags.dll'...
Processing 'SEXILED/Plugins/Sexiled.CustomItems.dll'
Extracting 'Sexiled.CustomItems.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.CustomItems.dll'...
Processing 'SEXILED/Plugins/Sexiled.CustomRoles.dll'
Extracting 'Sexiled.CustomRoles.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.CustomRoles.dll'...
Processing 'SEXILED/Plugins/Sexiled.Events.dll'
Extracting 'Sexiled.Events.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.Events.dll'...
Processing 'SEXILED/Plugins/Sexiled.Permissions.dll'
Extracting 'Sexiled.Permissions.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.Permissions.dll'...
Processing 'SEXILED/Plugins/Sexiled.Updater.dll'
Extracting 'Sexiled.Updater.dll' into '/user/SCP/SEXILED/Plugins/Sexiled.Updater.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Sexiled.API.dll'
Extracting 'Sexiled.API.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Sexiled.API.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/Sexiled.Loader.dll'
Extracting 'Sexiled.Loader.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/Sexiled.Loader.dll'...
Installation complete
```
