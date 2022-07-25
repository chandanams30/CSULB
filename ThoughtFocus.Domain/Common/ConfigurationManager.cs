namespace ThoughtFocus.Domain.Common
{
    public class ConfigurationManager
    {
        public EmailSettings EmailSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }


    public class EmailSettings
    {
        public string SmtpServerAddress { get; set; }
        public int SmtpServerPort { get; set; }
        public bool SmtpServerEnableSSL { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassWord { get; set; }
        public string EmailFromUserName { get; set; }
        public bool IsSendMail { get; set; }
        public string ApprovedEmailAttachmentPath { get; set; }
        public string NUL_Logo_Standalone_Logo { get; set; }
        public string Urban_Empowerment_Fund_Logo { get; set; }
        public string Pepsico_Logo { get; set; }
        public string BRA_Logo { get; set; }

    }

    public class ConnectionStrings
    {
        public string SmtpServerAddress { get; set; }

    }

    public class SqlConnectionStrings
    {
        public string AppDBConnection { get; set; }
    }
    public class DocumentStorageConfiguration
    {
        public string BlobStorageConnectionString { get; set; }
        public string ContainerName { get; set; }

    }


}