# What is this
The anagram game Anagramarama.  
Works with Windows, MacOS (only intel CPU tested so far), linux  

The original Anagramarama (v0.7/0.8), but rewritten in C# (.net8.0+) and object oriented  
Released under GPL2 as the original authors of the application in C did  

Added more comprehensive __command line__ switches:  
```
-h/--help: help (eg shortcut keys) 
-v/--version: version number
-l/--locale <locale-code>: for specifying a language 
    (as long as the dictionary is there) 
    (locale-code such as en-GB or fr-FR)
```

Keyboard usage:  
```
LETTER A to Z: Move Available Letter to the Guess
BACKSPACE: remove the rightmost letter from the Guess
ESCAPE: remove all the letters from the Guess
SPACE: shuffle the letters in the Available Letters

F1: switch between full screen or windowed

Some keys I added:
F2: start a new game
F5: quit the game (not available yet in release v1.0.0)
```

# Just run it?
Download the [release](https://github.com/Squizzy/anagramarama-cs-oop/releases/tag/v1.0.0) version for the desired OS, unzip and then run the executable.  
### NOTE
There is no installer yet, and the MacOS version is not yet available as a package... all on the todo list but the executables already run fine (normally)  

# How to compile and run
### Code editor  
  - [VSCode](https://code.visualstudio.com/download)  
### Development platform: C# + Dotnet  
  - [DotNet 8.0 or above](https://dotnet.microsoft.com/en-us/download)  
  - and the relevant extensions for vscode if desired  
### Logging (two options)
 1. Microsoft: MS Extension logging, MS output to Console (terminal), MS output to vscode Debug Console  
  ```
    dotnet add package Microsoft.Extensions.Logging  
    dotnet add package Microsoft.Extensions.Logging.Consoled
    dotnet add package Microsoft.Extensions.Logging.Debug
  ```

2. Serilog: Serilog logging, Serilog output to Console, Serilog output to file, Serilog output to vscode Debug Console, Serilog Console themes:  
```
  dotnet add package Serilog.Extensions.Logging
  dotnet add package Serilog.Sinks.Console
  dotnet add package Serilog.Sinks.File
  dotnet add package Serilog.Sinks.Debug
  dotnet add package Serilog.Sinks.SystemConsole.Themes
```

### SDL2 development librairies
- Jeremy Sayers' [SDL2 adaptation for C#](https://jsayers.dev/tutorials/)  
- This application was developed using only this set of librairies, and not tested with any others SDL2 bindings  

### Clone this repository
- ```
  git clone https://github.com/Squizzy/anagramarama-cs-oop.git
  ```

### Note on final installation
  - The "i18n" and the "audio" folders needs to be copied into the correct folder for the OS (to be improved!)  
  - Jim Rich listed the [path variable for multiple OS here](https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/)  
  - The variable needed is under the name: ```Environment.SpecialFolder.LocalApplicationData```  
      eg in windows: ```C:\Users\\<username\>\AppData\Roaming\anagramarama\```

### Running from the source code
 from inside the folder AgOop, execute:  
- ```
  dotnet run
  ```

### Creating the executables from the source code
The command is similar for Windows, MacOs or Linux:  
1. If ok to modify AgOop.csproj content:  
Uncomment the OS desired wanted in AgOop.csproj by removing the ```<!-- -->```
    - eg to compile for windows x64  
      ```
      <RuntimeIdentifier>win-x64</RuntimeIdentifier>
      <!-- <RuntimeIdentifier>linux-x64</RuntimeIdentifier> -->
      <!-- <RuntimeIdentifier>osx-x64</RuntimeIdentifier> -->
      ```

    Create the executable using either:  
    - dotnet from the command line  
    ```
    dotnet publish -c release
    ```  
    - a ```Solution Explorer``` vscode extension:  
    I use [fernandoescolar's vscode-solution-explorer](vscode:extension/fernandoescolar.vscode-solution-explorer)  
    right clicking the AgOop solution brings up a menu, where "publish" appears  

2. If modifying AgOop.csproj is not considered:  
from the command line:  
    Windows:  
    ``` 
    dotnet publish --runtime win-x64
    ``` 
    MacOs (intel version):  
    ```
    dotnet publish --runtime osx-x64
    ```
    Linux:  
    ```
    dotnet publish --runtime linux-x64
    ``` 

# History
25 Jun 2025: Release v1.0.0

# Credit
All who developed the original application  
Latest version they maintain: https://identicalsoftware.com/anagramarama/