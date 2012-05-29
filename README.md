SimpleWebServer
===============

## A very simple and lightweight web server to be used in .NET applications.

In one of my recent projects, I used an HTML/Javascript web interface for debugging and simulating various aspects of the application via an HTTP protocol.
Instead of setting up a full blown web server such as Apache I decided to use the `HttpListener` class of .NET to setup my own, small web server. That way, if a certain debug flag in my application is set, the web interface can simply be reached using a specified address and port.

Be aware, this is NOT a production quality web server. There is only one asynchronous task (using the Task Parallel Library) to serve all incoming requests, which are queued up. MIME types of files are simply determined by their file extension.

Very useful though, if you only need to serve a number of files to a small number of clients, e.g. local help files for your application.

### Requirements ###

.NET 4, Visual Studio 2010.

### Usage ###

    var server = new SimpleWebServer("http://localhost:8080/", "files/");
    server.Start();