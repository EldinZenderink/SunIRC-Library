# SunIRC Library

SunIRCLibrary is .NET C# Library which provides a JSON interface for a IRC Client based on SimpleIRC Library. It's quit basic but it supports simple stuff such as multiple Channels support, DCC Downloading, etcetra. It includes a fully interfacable json file browser as well. It doesn't allow much tinkering as of now, it's basically a fully featured IRC client which you could put in a console to act as a server, or within a winform with a browser element.

Browser element you say? Why? Well... duh. It's a json interface, so you need to build the frontend in atleast HTML & Javascript, and talk to the library through websockets.

Be aware that this application is still in development and might still have some issues!

**This is part of a few applications of mine, and the mindset while creating this library was solely set to making it work with MY applications, this means features which probably should be added, arent there, but you can request them, not sure if and when I will implement them though.**

**Be aware: the default download directory for the IRC client is the "Executable Directory".**

**This will be changable through code whithin the near future, or upon heavy demand.**

**Some settings are changable through the JSON API (such as the download directory).**

# (Basic) Usage

### [The All Important Wiki](https://github.com/EldinZenderink/SunIRC-Library/wiki)



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
