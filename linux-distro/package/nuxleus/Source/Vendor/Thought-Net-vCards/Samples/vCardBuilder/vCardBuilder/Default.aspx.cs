
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Thought.Net.vCards;

public partial class DefaultPage : System.Web.UI.Page 
{

    /// <summary>
    ///     Executed when the page is loaded.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    ///     Executed when the Submit button is clicked.
    /// </summary>
    protected void SubmitButton_Click(object sender, EventArgs e)
    {

        vCard card = new vCard();

        // Simple properties

        card.AdditionalNames = AdditionalNames.Text;
        card.FamilyName = FamilyName.Text;
        card.GivenName = GivenName.Text;
        card.NamePrefix = NamePrefix.Text;
        card.NameSuffix = NameSuffix.Text;
        card.Organization = Organization.Text;
        card.Role = Role.Text;
        card.Title = Title.Text;

        // ---------------------------------------------------------------
        // Email Addresses
        // ---------------------------------------------------------------
        // A vCard supports any number of email addresses.

        if (!string.IsNullOrEmpty(WorkEmail.Text))
        {
            card.EmailAddresses.Add(
                new vCardEmailAddress(WorkEmail.Text));

        }

        // ---------------------------------------------------------------
        // Notes
        // ---------------------------------------------------------------
        // The vCard specification allows for multiple notes, although
        // most applications seem to support a maximum of one note.

        if (Note.Text.Length > 0)
        {
            card.Notes.Add(new vCardNote(Note.Text));
        }


        // ---------------------------------------------------------------
        // Phones
        // ---------------------------------------------------------------
        //
        // A vCard supports any number of telephones.  Each telephone can
        // have a different type (such as a video phone or a fax) and a
        // purpose (e.g. a home number or a work number).

        if (!string.IsNullOrEmpty(WorkPhone.Text))
        {
            card.Phones.Add(
                new vCardPhone(WorkPhone.Text, vCardPhoneType.WorkVoice));
        }

        if (!string.IsNullOrEmpty(WorkFax.Text))
        {
            card.Phones.Add(
                new vCardPhone(WorkFax.Text, vCardPhoneType.WorkFax));
        }


        // ---------------------------------------------------------------
        // Web Sites
        // ---------------------------------------------------------------

        if (WorkWebSite.Text.Length > 0)
        {
            card.WebSites.Add(
                new vCardWebSite(WorkWebSite.Text, vCardWebSiteType.Work));
        }

        // ---------------------------------------------------------------
        // Nicknames
        // ---------------------------------------------------------------

        string[] nicklist = Nicknames.Text.Split(new char[] { ',' });
        foreach (string nick in nicklist)
        {
            if (nick.Length > 0)
                card.Nicknames.Add(nick);
        }

        // The vCard object has been populated.  The rest of
        // the code generates the vCard file format and exports
        // it to the response stream.

        Response.ContentType = "text/x-vcard";

        // The "content-disposition" is a special HTTP header
        // that tells the web browser how to interpreted the
        // output.  In this case, the browser is informed the
        // content should be treated as an attachment with
        // a default filename.  This should cause the browser
        // to display a dialog box to save the vCard (instead
        // of displaying the vCard as inline text).

        Response.AppendHeader(
            "content-disposition", "attachment;filename=vCard.vcf");

        vCardStandardWriter writer = new vCardStandardWriter();

        writer.EmbedInternetImages = false;
        writer.EmbedLocalImages = true;
        writer.Options = vCardStandardWriterOptions.IgnoreCommas;

        writer.Write(card, Response.Output);
        Response.End();

    }

    /// <summary>
    ///     Executed when a menu item is clicked.
    /// </summary>
    protected void MainMenu_MenuItemClick(object sender, MenuEventArgs e)
    {

        switch (e.Item.Value)
        {

            case "Employment":
                MainView.SetActiveView(EmploymentView);
                break;

            case "Name":
                MainView.SetActiveView(NameView);
                break;

            case "Note":
                MainView.SetActiveView(NoteView);
                break;

            default:
                throw new NotImplementedException();

        }

    }
}
