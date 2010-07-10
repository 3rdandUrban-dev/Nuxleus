using System;
using System.Xml;
using System.IO;
using Xameleon.Amplee;
using Xameleon.Atom;
using Xameleon.Cryptography;

public class Test
{

    public static void Main()
    {

        StoreManager sm = new StoreManager();
        sm.MemberStorage = new FileSystemStorage(Directory.GetCurrentDirectory());

        AtomPubService s = new AtomPubService(sm);
        s.Lang = "en";
        s.Base = "http://amp.fm/";

        AtomPubWorkspace w = new AtomPubWorkspace(s);
        AtomPubCollection c = new AtomPubCollection(w, "col1");
        c.Uri = "pub/music/";
        c.Title = "Some music";
        c.Accepts.Add("text/plain");
        c.Categories.Fixed = true;

        AtomCategory ac = new AtomCategory();
        ac.Term = "audio";

        c.Categories.Categories.Add(ac);

        //s.Document.Save(Console.Out);

        AtomEntry e = new AtomEntry();
        e.Id = "urn:uuid:123f542";
        e.Title = " Hi there";
        e.Updated = DateTime.UtcNow;

        Link l = new Link();
        l.Mediatype = "text/plain";
        l.Rel = "alternate";
        l.Length = 12;
        l.Href = "http://blah.entry.txt";
        e.Links.Add(l);

        Author a = new Author();
        a.Name = "Sylvain";
        a.Uri = "http://www.defuze.org";
        a.Email = "sh@defuze.org";
        e.Authors.Add(a);

        e.Summary = new TextConstruct(TextElement.Summary);
        e.Summary.TextContent = "blah blah";
        e.Summary.Mediatype = "text";

        e.Content = new TextConstruct(TextElement.Content);

        string ct = "<div xmlns='http://www.w3.org/1999/xhtml'><b>hey hey</b></div>";
        e.Content.XmlContent = ct;
        e.Content.Mediatype = "xhtml";

        //e.Document.Save(Console.Out);

        AtomEntry e0 = new AtomEntry();
        e0.Document = e.Document;
        //e0.Document.Save(Console.Out);

        IMember mb = Member.FromEntry(c, e0.Document);
        Console.WriteLine(mb.MemberId);
        Console.WriteLine(mb.MediaType);

        MemoryStream ms = new MemoryStream();
        XmlWriter xw = XmlWriter.Create(ms);
        e0.Document.Save(xw);

        StreamReader sr = new StreamReader(ms);
        ms.Seek(0, SeekOrigin.Begin);

        c.Attach(mb, sr.ReadToEnd());

        ms.Close();

        c.CollectionFeed.Save(Console.Out);
    }
}
