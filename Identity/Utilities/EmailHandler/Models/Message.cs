﻿using MimeKit;

namespace Identity.Utilities.EmailHandler.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(email => new MailboxAddress(string.Empty, email))); 
            Subject = subject;
            Content = content;
        }
    }
}
