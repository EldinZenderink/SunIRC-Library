# SunIRC Library

SunIRCLibrary is .NET C# Library which provides a JSON interface for a IRC Client based on SimpleIRC Library. It's quit basic but it supports simple stuff such as multiple Channels support, DCC Downloading, etcetra. It includes a fully interfacable json file browser as well. It doesn't allow much tinkering as of now, it's basically a fully featured IRC client which you could put in a console to act as a server, or within a winform with a browser element.

Browser element you say? Why? Well... duh. It's a json interface, so you need to build the frontend in atleast HTML & Javascript, and talk to the library through websockets.

Be aware that this application is still in development and might still have some issues!

**This is part of a few applications of mine, and the mindset while creating this library was solely set to making it work with MY applications, this means features which probably should be added, arent there, but you can request them, not sure if and when I will implement them though.**

# (Basic) Usage
The following is the fastest way to get things working, a full guide/wiki will follow in the future:

There are a few extra libraries which you need (Which for now can be found within the "All Necesary Libraries" directory on this github, or you may install them using the following nuget packages (Though the libraries within the "All Necesary Libraries" directory are sure to work (incase newer versions of these libraries break things))):
1. Get the following libraries:
* [JSON.net - Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
* [websocket-sharp](https://www.nuget.org/packages/WebSocketSharp)
* [SimpleIRCLib](https://www.nuget.org/packages/SimpleIRCLib/)

2. Create a C# Console project within Visual Studio
3. Add the following code to your main:
```` 
    //false for local desktop usage, true for server usage (on different pc within same network)
    SunIRCInit init = new SunIRCInit(false); 
    Console.WriteLine("Press a key to exit!");
    Console.ReadLine();
    init.Shutdown();
````
4. Done, you should now be able to access the websocket server at: "your ip address":1515 (example: 127.0.0.1:1515) or the http server at: http://"your ip address":6010 (example: http://127.0.0.1:6010) 
5. Read the Wiki on this github to learn about the JSON api, otherwise, using this is quit pointless.

**To use the http server:**
1. Create a directory within the executable directory called: GUI
2. For example, http://127.0.0.1:6010/index.html would point to: "Executable Directory"/GUI/index.html

**Be aware: the default download directory for the IRC client is the "Executable Directory".**

**This will be changable through code whithin the near future, or upon heavy demand.**

**Some settings are changable through the JSON API (such as the download directory).**

### Tech
SunIRCLibrary uses a number of open source projects to work properly:

* [JSON.net - Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)- A nice JSON library for C#.
* [websocket-sharp](http://sta.github.io/websocket-sharp/) - A nice websocket library for C#.
* [SimpleIRCLib](https://www.nuget.org/packages/SimpleIRCLib/) - Also one of my creations ;), the only one with DCC (download) support for now ;)


### Development

This application is still in development, but I guess worthy to be thrown into the world by now. There are still a few features missing which I will add in the future (see Todos). 


### Todos

 - Make time free to clean up the code, do some bug fixes and add some features that I suppose would make this library  more usable.

License
----

MIT

**Free Software, Hell Yeah!**
