using System;
using System.IO;
using System.Net;
using Mono.WebServer;

public class WebServer
{
    // Fields
    private string App = "/:.";
    private string Path = "/";
    private int Port = 0x270f;
    private string RootDirectory = @".\public_web";
    private bool Verbose = true;
    private ApplicationServer WebAppServer;
    private XSPWebSource websource;

    // Methods
    public void AddApp(string app, string path)
    {
        this.WebAppServer.AddApplication(app, this.Port, path, Directory.GetCurrentDirectory());
    }

    public void GetCurDir()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
    }

    public void SetApp(string app)
    {
        this.App = app;
    }

    public void SetPath(string path)
    {
        this.Path = path;
    }

    public void SetPort(int port)
    {
        this.Port = port;
    }

    public void SetRoot(string root)
    {
        this.RootDirectory = root;
    }

    public void SetSystemRoot(string sys)
    {
        Environment.CurrentDirectory = sys;
    }

    public void SetVerbose(bool verbose)
    {
        this.Verbose = verbose;
    }

    public void Start()
    {
        Environment.CurrentDirectory = this.RootDirectory;
        this.websource = new XSPWebSource(IPAddress.Any, this.Port);
        this.WebAppServer = new ApplicationServer(this.websource);
        this.WebAppServer.AddApplication(this.App, this.Port, this.Path, Directory.GetCurrentDirectory());
        this.WebAppServer.Verbose = this.Verbose;
        this.WebAppServer.Start(false);
    }

    public void Stop()
    {
        this.WebAppServer.UnloadAll();
        this.WebAppServer.Stop();
    }
}


