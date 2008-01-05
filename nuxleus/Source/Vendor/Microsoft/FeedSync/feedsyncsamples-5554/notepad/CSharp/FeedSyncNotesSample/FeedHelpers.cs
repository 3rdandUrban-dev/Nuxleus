using System;
using System.Collections.Generic;
using System.Text;

namespace FeedSyncNotesDemo
{
    class FeedHelpers
    {
        public static Microsoft.Samples.FeedSync.Feed CreateFeedFromURL(string i_Url)
        {
            // Get the feed from URL:
            string strFeed = Microsoft.Samples.FeedSyncService.FeedManager.ReadFeedContents(i_Url);

            // Create a new XML document instance for the feed
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(strFeed);

            // Create a Feed object to read and manipulate feed items
            return Microsoft.Samples.FeedSync.Feed.Create(xmlDoc);
        }
    }
}
