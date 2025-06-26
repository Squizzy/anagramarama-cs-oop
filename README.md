# What is this
Anagramarama 0.7 rewritten in C# (.net8.0) and object oriented.
Released under GPL2 as the original authors of the application in C

Added more comprehensive command line switches:
- -h/--help : for help  
- -v/--version :for version number
- -l/--locale locale-code : for specifying a language (as long as the dictionary is there) (locale-code such as en-GB or fr-FR)

added F2 to start a new game (F1 still here to switch between full screen or windowed)

# How to name it work
 - VSCode is sufficient  
 - Install DotNet (and the relevant extensions for vscode if wanted)  
 - logging, three options: 
 1. Microsoft:
 - install logging:  
```dotnet add package Microsoft.Extensions.Logging```
 - install logging output to console:  
 ```dotnet add package Microsoft.Extensions.Logging.Console```
2. Serilog:
 - install Serilog logging output to file:  
 ```dotnet add package serilog.Extensions.Logging```
 - install Serilog logging output to console:  
 ```dotnet add package serilog.Sinks.Console```
 - install Serilog logging output to file:  
 ```dotnet add package serilog.Sinks.File```

3. to VSCode
 - install debug output to vscode:  
```dotnet add package Microsoft.Extensions.Logging.Debug```

 - Install SDL2 (this was developed using [Jeremy Sayers](https://jsayers.dev/tutorials/)' SDL2 adaptation for C# - not tested with others)  
 - Clone this repository  
 - The "i18n" and the "audio" folders needs to be copied into the correct folder for the OS (to be improved!)  
    - Environment.SpecialFolder.LocalApplicationData  
    - check the above in https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/ for your OS  
    - eg in windows: C:\Users\__username__\AppData\Roaming\anagramarama\

 - Should work with just "dotnet run" (I haven't tested yet)
 - Should be able to create the executable for Windows, MacOs or Linux by:
   - commenting out all OS apart the one wanted in AgOop.csproj - eg to compile for windows x64
     ```
     <RuntimeIdentifier>win-x64</RuntimeIdentifier>
     <!-- <RuntimeIdentifier>linux-x64</RuntimeIdentifier> -->
     <!-- <RuntimeIdentifier>osx-x64</RuntimeIdentifier> -->
     ```
   - using "dotnet publish -c release" or use the Solution Explorer extensions (right click)
 