using System.Net.Mail;

namespace Shared.Components.Pages
{
    public partial class Index
    {
        IEnumerable<CItems> _selectedItems = new List<CItems>();
        IEnumerable<CItems> selectedItems { get; set; } = new HashSet<CItems>();

        List<CItems> items = new List<CItems>
        {
            new CItems { Id = 1, Name = "Oleg" },
            new CItems { Id = 2, Name = "Dima" }
        };

        void OnAdded()
        {
            //try
            //{
            //    SmtpClient mySmtpClient = new SmtpClient("mail.yandex.ru");

            //    // set smtp-client with basicAuthentication
            //    mySmtpClient.UseDefaultCredentials = false;
            //    System.Net.NetworkCredential basicAuthenticationInfo = new
            //       System.Net.NetworkCredential("rusfaq02@yandex.ru", "");
            //    mySmtpClient.Credentials = basicAuthenticationInfo;

            //    // add from,to mailaddresses
            //    MailAddress from = new MailAddress("rusfaq02@yandex.ru", "RusFAQ.ru");
            //    MailAddress to = new MailAddress("adm@rfpro.ru", "TestToName");
            //    MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            //    // add ReplyTo
            //    MailAddress replyTo = new MailAddress("rusfaq02@yandex.ru");
            //    myMail.ReplyToList.Add(replyTo);

            //    // set subject and encoding
            //    myMail.Subject = "Test message";
            //    myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            //    // set body-message and encoding
            //    myMail.Body = "<b>Test Mail</b><br>using <b>HTML</b>.";
            //    myMail.BodyEncoding = System.Text.Encoding.UTF8;
            //    // text or html
            //    myMail.IsBodyHtml = true;

            //    mySmtpClient.Send(myMail);
            //}

            //catch (SmtpException ex)
            //{
            //    throw new ApplicationException
            //      ("SmtpException has occured: " + ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            //var item = new CItems { Id = 3, Name = "Sasha" };
            //items.Add(item);

            //item.Id = 4;
            //items.Add(item);
        }

        void OnDeleted()
        {
            items[0].IsDeleted = true;

            //((HashSet<CItems>)selectedItems).Remove(items[0]);
            //items.Remove(items[0]);
            //selectedItems = null;
        }

        void OnChanged()
        {
            items[0].IsDeleted = false;
            //items[0].Name = "Oleg_changed";
        }

    }

    class CItems
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        
        public override string ToString()
        {
            return Name;
        }
    }

}
