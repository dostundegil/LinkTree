namespace LinkTree
{
    public interface IEmailSender
    {
        public int SendEmailAsync(string receiverAddress, string subject, string htmlBody);
    }
}
