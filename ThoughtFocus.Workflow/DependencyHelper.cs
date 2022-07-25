//using ThoughtFocus.Business.Interfaces.EmailTemplate;
//using ThoughtFocus.DataAccess;
using ThoughtFocus.Domain.Common;
//using ThoughtFocus.Repository.Interfaces.Admin;
//using ThoughtFocus.Repository.Interfaces.Application;
//using ThoughtFocus.Repository.Interfaces.FundingSource;
//using ThoughtFocus.Repository.Interfaces.Master;

namespace ThoughtFocus.Workflow
{
    public class DependencyHelper
    {
        public static WorkflowInit WorkflowInit { get;set; }
        public static WorkflowRole WorkflowRole { get; set; }
        public static WorkflowRule WorkflowRule { get; set; }
        public static WorkflowActions WorkflowActions { get; set; }
        //public static IEmailTemplateManager EmailTemplateManager { get; set; }
        //public static ILoanApplicationRepository LoanApplicationRepository { get; set; }
        //public static IApplicationStatusRepository ApplicationStatusRepository { get; set; }
        //public static IProgramInvitationRepository ProgramInvitationRepository { get; set; }
        public static SqlConnectionStrings SqlConnectionStrings { get; set; }
        //public static IFundUtilizationRepository FundUtilizationRepository { get; set; }
        //public static ILoanApplicationCommentRepository LoanApplicationCommentRepository { get; set; }
       // public static ILoanApplicationAgreementDetailRepository LoanApplicationAgreementDetailRepository { get; set; }
        
    }
}
