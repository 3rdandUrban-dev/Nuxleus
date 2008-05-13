// Use lightweight F# syntax

#light

// Declare a specific namespace and module name

module MyNamespace.MyApplication

// Import managed assemblies

open System.Xml
open System.Collections
open System.Collections.Generic
open System.IO
open System.Net
open Microsoft.FSharp.Control.CommonExtensions

// The RSS feeds we wish to get. The first two values are
// only used if our code is not able to parse the feed's XML

let feeds =
  [ ("Through the Interface",
     "http://blogs.autodesk.com/through-the-interface",
     "http://through-the-interface.typepad.com/through_the_interface/rss.xml");
     
    ("Don Syme's F# blog",
     "http://blogs.msdn.com/dsyme/",
     "http://blogs.msdn.com/dsyme/rss.xml");
    
    ("Shaan Hurley's Between the Lines",
     "http://autodesk.blogs.com/between_the_lines",
     "http://autodesk.blogs.com/between_the_lines/rss.xml");
    
    ("Scott Sheppard's It's Alive in the Lab",
     "http://blogs.autodesk.com/labs",
     "http://labs.blogs.com/its_alive_in_the_lab/rss.xml");
    
    ("Lynn Allen's Blog",
     "http://blogs.autodesk.com/lynn",
     "http://lynn.blogs.com/lynn_allens_blog/index.rdf");

    ("Heidi Hewett's AutoCAD Insider",
     "http://blogs.autodesk.com/autocadinsider",
     "http://heidihewett.blogs.com/my_weblog/index.rdf") ]

// Fetch the contents of a web page, asynchronously

let httpAsync(url:string) = 
  async { let req = WebRequest.Create(url) 
          use! resp = req.GetResponseAsync()
          use stream = resp.GetResponseStream() 
          use reader = new StreamReader(stream) 
          return reader.ReadToEnd() }

// Load an RSS feed's contents into an XML document object
// and use it to extract the titles and their links
// Hopefully these always match (this could be coded more
// defensively)

let titlesAndLinks (name, url, xml) =
  let xdoc = new XmlDocument()
  xdoc.LoadXml(xml)

  let titles =
    [ for n in xdoc.SelectNodes("//*[name()='title']")
        -> n.InnerText ]
  let links =
    [ for n in xdoc.SelectNodes("//*[name()='link']") ->
        let inn = n.InnerText
        if  inn.Length > 0 then
          inn
        else
          let href = n.Attributes.GetNamedItem("href").Value
          let rel = n.Attributes.GetNamedItem("rel").Value
          if href.Contains("feedburner") then
              ""
          else
            href ]
          
  let descs =
    [ for n in xdoc.SelectNodes
        ("//*[name()='description' or name()='content' or name()='subtitle']")
          -> n.InnerText ]

  // A local function to filter out duplicate entries in
  // a list, maintaining their current order.
  // Another way would be to use:
  //    Set.of_list lst |> Set.to_list
  // but that results in a sorted (probably reordered) list.

  let rec nub lst =
    match lst with
    | a::[] -> [a]
    | a::b ->
      if a = List.hd b then
        nub b
      else
        a::nub b
    | [] -> []

  // Filter the links to get (hopefully) the same number
  // and order as the titles and descriptions

  let real = List.filter (fun (x:string) -> x.Length > 0)  
  let lnks = real links |> nub

  // Return a link to the overall blog, if we don't have
  // the same numbers of titles, links and descriptions

  let lnum = List.length lnks
  let tnum = List.length titles
  let dnum = List.length descs
  
  if tnum = 0 || lnum = 0 || lnum <> tnum || dnum <> tnum then
    [(name,url,url)]
  else
    List.zip3 titles lnks descs

// For a particular (name,url) pair,
// create an AutoCAD HyperLink object

let hyperlink (name,url,desc) =
  let hl = new HyperLink()
  hl.Name <- url
  hl.Description <- desc
  (name, hl)

// Use asynchronous workflows in F# to download
// an RSS feed and return AutoCAD HyperLinks
// corresponding to its posts

let hyperlinksAsync (name, url, feed) =
  async { let! xml = httpAsync feed
          let tl = titlesAndLinks (name, url, xml)
          return List.map hyperlink tl }

// Now we declare our command

[<CommandMethod("rss")>]
let createHyperlinksFromRss() =
  
  // Let's get the usual helpful AutoCAD objects
  
  let doc =
    Application.DocumentManager.MdiActiveDocument
  let db = doc.Database

  // "use" has the same effect as "using" in C#

  use tr =
    db.TransactionManager.StartTransaction()

  // Get appropriately-typed BlockTable and BTRs

  let bt =
    tr.GetObject
      (db.BlockTableId,OpenMode.ForRead)
    :?> BlockTable
  let ms =
    tr.GetObject
      (bt.[BlockTableRecord.ModelSpace],
       OpenMode.ForWrite)
    :?> BlockTableRecord
  
  // Add text objects linking to the provided list of
  // HyperLinks, starting at the specified location
  
  // Note the valid use of tr and ms, as they are in scope

  let addTextObjects pt lst =
    // Use a for loop, as we care about the index to
    // position the various text items

    let len = List.length lst
    for index = 0 to len - 1 do 
      let txt = new DBText()
      let (name:string,hl:HyperLink) = List.nth lst index
      txt.TextString <- name
      let offset =
        if index = 0 then
          0.0
        else
          1.0

      // This is where you can adjust:
      //  the initial outdent (x value)
      //  and the line spacing (y value)

      let vec =
        new Vector3d
          (1.0 * offset,
           -0.5 * (Int32.to_float index),
           0.0)
      let pt2 = pt + vec
      txt.Position <- pt2
      ms.AppendEntity(txt) |> ignore
      tr.AddNewlyCreatedDBObject(txt,true)
      txt.Hyperlinks.Add(hl) |> ignore

  // Here's where we do the real work, by firing
  // off - and coordinating - asynchronous tasks
  // to create HyperLink objects for all our posts

  let links =
    Async.Run
      (Async.Parallel
        [ for (name,url,feed) in feeds ->
          hyperlinksAsync (name,url,feed) ]) 

  // Add the resulting objects to the model-space  

  let len = Array.length links
  for index = 0 to len - 1 do

    // This is where you can adjust:
    //  the column spacing (x value)
    //  the vertical offset from origin (y axis)

    let pt =
      new Point3d
        (15.0 * (Int32.to_float index),
         30.0,
         0.0)
    addTextObjects pt (Array.get links index)
 
  tr.Commit()