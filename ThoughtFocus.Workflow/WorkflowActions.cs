using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.Runtime;
using ThoughtFocus.Common.Exceptions;
//using ThoughtFocus.DataAccess;
using Microsoft.EntityFrameworkCore;
//using ThoughtFocus.Repository.Interfaces.Application;
//using ThoughtFocus.Repository.Impl.Application;
//using ThoughtFocus.DataAccess.Models.Application;
using Microsoft.Extensions.Logging;
using log4net;
//using ThoughtFocus.Repository.Interfaces.Master;
//using ThoughtFocus.Repository.Impl.Master;
//using ThoughtFocus.DataAccess.Models.Master;
//using ThoughtFocus.Domain.Enumeration;
//using ThoughtFocus.Business.Interfaces.EmailTemplate;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using ThoughtFocus.Domain.Common;
//using ThoughtFocus.Repository.Interfaces.Admin;
//using ThoughtFocus.DataAccess.Models.Admin;
//using ThoughtFocus.Repository.Interfaces.FundingSource;
//using ThoughtFocus.Repository.Impl.FundingSource;
//using ThoughtFocus.DataAccess.Models.FundingSource;
//using ThoughtFocus.Domain.Params;
using ThoughtFocus.Domain.User;
//using ThoughtFocus.DataAccess.Models;
using System.Net.Sockets;

namespace ThoughtFocus.Workflow
{
    public class WorkflowActions : IWorkflowActionProvider
    {
        //public ILoanApplicationRepository _loanApplicationRepository;

        public SqlConnectionStrings _sqlConnectionString;
        //public IApplicationStatusRepository _applicationStatusRepository;
        //public IProgramInvitationRepository _programInvitationRepository;
        //public IEmailTemplateManager _emailTemplateManager;

        //public IFundUtilizationRepository _fundUtilizationRepository;
        //public ILoanApplicationCommentRepository _loanApplicationCommentRepository;

        //public ILoanApplicationAgreementDetailRepository _loanApplicationAgreementDetailRepository;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkflowActions));

        //TODO: Define rules and business conditions

        public WorkflowActions()
        {
            // _context = DependencyHelper.AccreditationContext;
            // _auditService = DependencyHelper.AuditPersistance;
            // _emailSender = DependencyHelper.EmailSender;
            // _siteVisitStateRepository = DependencyHelper.SiteVisitStateRepository;
            // _siteVisitRepository = DependencyHelper.SiteVisitRepository;
            //_emailTemplateManager = DependencyHelper.EmailTemplateManager;
            //_loanApplicationRepository = DependencyHelper.LoanApplicationRepository;
            //_applicationStatusRepository = DependencyHelper.ApplicationStatusRepository;
            _sqlConnectionString = DependencyHelper.SqlConnectionStrings;
            //_programInvitationRepository = DependencyHelper.ProgramInvitationRepository;
            //_loanApplicationCommentRepository = DependencyHelper.LoanApplicationCommentRepository;
            //_loanApplicationAgreementDetailRepository = DependencyHelper.LoanApplicationAgreementDetailRepository;
        }
       
        private static Dictionary<string, Action<ProcessInstance, string>> _actions = new Dictionary
            <string, Action<ProcessInstance, string>>
        {
            //TODO: Add actions
        };
        private static Dictionary<string, Func<ProcessInstance, string, bool>> _conditions = new Dictionary<string, Func<ProcessInstance, string, bool>>
        {
            //TODO: Add conditions
        };
        public void ExecuteAction(string name, Common.Workflow.Core.Model.ProcessInstance processInstance, string actionParameter)
        {
            if (_actions.ContainsKey(name))
            {
                _actions[name].Invoke(processInstance, actionParameter);
                return;
            }

            throw new NotImplementedException(string.Format("Action with name {0} not implemented", name));
        }
        public bool ExecuteCondition(string name, Common.Workflow.Core.Model.ProcessInstance processInstance, string actionParameter)
        {
            if (_conditions.ContainsKey(name))
            {
                return _conditions[name].Invoke(processInstance, actionParameter);
            }

            throw new NotImplementedException(string.Format("Action condition with name {0} not implemented", name));
        }

        public List<string> GetActions()
        {
            return _actions.Keys.Concat(_conditions.Keys).ToList();
        }

        //public void UpdateLoanApplicationStatus(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered UpdateLoanApplicationStatus for ProcessID-{0}", processInstance.ProcessId));
        //    try
        //    {
        //        //Get The current Command
        //        string currentCommand = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command") == null ? String.Empty : processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value.ToString();
        //        if (String.IsNullOrEmpty(currentCommand))
        //        {
        //            Logger.Debug(String.Format("currentCommand is null for LoanApplicationID {0}", processInstance.ProcessId));
        //        }
        //        ActivityDefinition activityDefinition = processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault() == null ? null : processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault().To;
        //        string executedState = activityDefinition == null ? string.Empty : activityDefinition.Name;

        //        if (!String.IsNullOrEmpty(executedState))
        //        { 
        //            DbContextOptionsBuilder<ApplicationDBContext> optionsBuilder =  SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ApplicationDBContext>(), _sqlConnectionString.AppDBConnection);
        //            optionsBuilder.UseLazyLoadingProxies();
        //            var options = optionsBuilder.Options;
                    
        //            using (var context = new ApplicationDBContext(options))
        //            {
        //                _loanApplicationRepository = new LoanApplicationRepository(context);
        //                _applicationStatusRepository = new ApplicationStatusRepositoryImpl(context);

        //                LoanApplication loanApplication = this._loanApplicationRepository.FirstOrDefault(a => a.LoanApplicationID == processInstance.ProcessId);
        //                if (loanApplication != null)
        //                {
        //                   ApplicationStatus applicationStatus = this._applicationStatusRepository.FirstOrDefault(a => a.ApplicationStatusName == executedState);
        //                    if (applicationStatus != null)
        //                    {
        //                        loanApplication.ApplicationStatusID = applicationStatus.ApplicationStatusID;
        //                        loanApplication.LastModifiedDateTime = DateTime.Now;
        //                        this._loanApplicationRepository.SaveLoanApplicationDetails(loanApplication, null);
        //                    }
        //                }
        //            }
        //        }
        //        Logger.Debug(String.Format("Successfully exited UpdateLoanApplicationStatus for ProcessID-{0}", processInstance.ProcessId));
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        //LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in UpdateLoanApplicationStatus ",null);
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateLoanApplicationStatus for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        //LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in UpdateLoanApplicationStatus ",null);
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateLoanApplicationStatus for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //}

        ////update the status of the ProgramInvitation to Applied 
        //public void UpdateProgramInvitationStatus(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered UpdateProgramInvitationStatus for ProcessID-{0}", processInstance.ProcessId));
        //    try
        //    {
        //        //Get The current Command
        //        string currentCommand = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command") == null ? String.Empty : processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value.ToString();
        //        if (String.IsNullOrEmpty(currentCommand))
        //        {
        //            Logger.Debug(String.Format("currentCommand is null for LoanApplicationID {0}", processInstance.ProcessId));
        //        }

        //        DbContextOptionsBuilder<ApplicationDBContext> optionsBuilder =  SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ApplicationDBContext>(), _sqlConnectionString.AppDBConnection);
        //        optionsBuilder.UseLazyLoadingProxies();
        //        var options = optionsBuilder.Options;

        //        using (var context = new ApplicationDBContext(options))
        //        {
        //             _loanApplicationRepository = new LoanApplicationRepository(context);    
        //            LoanApplication loanApplication = this._loanApplicationRepository.FirstOrDefault(a => a.LoanApplicationID == processInstance.ProcessId);

        //            if (loanApplication != null)
        //            {
        //               ProgramInvitation programInvitation = this._programInvitationRepository.FirstOrDefault(a => a.ProgramInvitationID == loanApplication.ProgramInvitationID);
        //                if (programInvitation != null)
        //                {
        //                    programInvitation.ProgramStatusID = (long)ProgramStatusEnumeration.Applied;
        //                    programInvitation.LastModifiedDateTime = DateTime.Now;
        //                    this._programInvitationRepository.SaveOrUpdateProgramInvitation(programInvitation, null);
        //                }
        //            }
        //        }

        //        Logger.Debug(String.Format("Successfully exited UpdateProgramInvitationStatus for ProcessID-{0}", processInstance.ProcessId));
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateProgramInvitationStatus for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateProgramInvitationStatus for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //}

        //public void SendNotification(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered SendInvitation for ProcessID-{0}", processInstance.ProcessId));
        //    try
        //    {
        //        //Get The current Command
        //        string currentCommand = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command") == null ? String.Empty : processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value.ToString();

        //        if (String.IsNullOrEmpty(currentCommand))
        //        {
        //            Logger.Debug(String.Format("currentCommand is null for Application {0}", processInstance.ProcessId));
        //        }

        //        processInstance.ExecutedActivityState = processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault() == null ? String.Empty : processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault().To.Name;

        //        ActivityDefinition activityDefinition = processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault() == null ? null : processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault().To;

        //        long activityID = activityDefinition == null ? 0 : activityDefinition.ID;

        //        _emailTemplateManager.SendWorkflowEmail(processInstance, activityID);

        //        Logger.Debug(String.Format("Successfully exited SendNotification for ProcessID-{0}", processInstance.ProcessId));
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        //LoggerExtensions.LogMessage(Logger, ex);
        //        Logger.Error(String.Format("Unsuccessfully exited SendNotification for ProcessID-{0}", processInstance.ProcessId));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        //LoggerExtensions.LogMessage(Logger, ex);
        //        Logger.Error(String.Format("Unsuccessfully exited SendNotification for ProcessID-{0}", processInstance.ProcessId));
        //        return;
        //    }
        //}

        //public void UpdateFundUtilization(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered UpdateFundUtilization for ProcessID-{0}", processInstance.ProcessId));
        //    try
        //    {
        //       FundUtilizationParam fundUtilizationParam = (FundUtilizationParam)processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "FundUtilization").Value;

        //        if(fundUtilizationParam != null)
        //        {
        //            UserSessionEntity userEntity = (UserSessionEntity)processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "UserSessionModel").Value;
                    
        //            DbContextOptionsBuilder<ApplicationDBContext> optionsBuilder =  SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ApplicationDBContext>(), _sqlConnectionString.AppDBConnection);
        //            optionsBuilder.UseLazyLoadingProxies();
        //            var options = optionsBuilder.Options;

        //            using (var context = new ApplicationDBContext(options))
        //            {
        //                _loanApplicationRepository = new LoanApplicationRepository(context);    
        //                LoanApplication loanApplication = this._loanApplicationRepository.FirstOrDefault(a => a.LoanApplicationID == processInstance.ProcessId);
                        
        //                 _fundUtilizationRepository = new FundUtilizationRepository(context);    
        //                FundUtilization fundUtilization = this._fundUtilizationRepository.FirstOrDefault(a => a.ApplicationID == processInstance.ProcessId);

        //                if(fundUtilization ==null)
        //                fundUtilization = new FundUtilization();

        //                fundUtilization.ApplicationID = processInstance.ProcessId;
        //                fundUtilization.Comment = fundUtilizationParam.Comment;
        //                fundUtilization.DateofDisbursement = fundUtilizationParam.DateofDisbursement.Value;
        //                fundUtilization.OriginatingBankAccount = fundUtilizationParam.OriginatingBankAccount;
        //                fundUtilization.TransactionAmount = fundUtilizationParam.TransactionAmount.Value;
        //                fundUtilization.TransactionDate = System.DateTime.Now;
        //                fundUtilization.TransactionTypeID = (int)TransactionTypeEnumeration.Allocated;
        //                fundUtilization.CreatedDateTime = System.DateTime.Now;
        //                fundUtilization.CreatedByUserID = userEntity.UserID;
        //                fundUtilization.IsActive = true;
        //                fundUtilization.LastModifiedDateTime = System.DateTime.Now;
        //                fundUtilization.LastModifiedByUserID = userEntity.UserID;
        //                fundUtilization.FundingSourceID = loanApplication.FundingApplication.ProgramID;
        //                //Add document 
                        
        //                FundTransactionDocument fundTransactionDocument = new FundTransactionDocument();
        //                fundTransactionDocument.DocumentGUID = fundUtilizationParam.ApplicationDocument.DocumentGUID;
        //                fundTransactionDocument.DocumentName = fundUtilizationParam.ApplicationDocument.DocumentName;
        //                fundTransactionDocument.DocumentTypeID = fundUtilizationParam.ApplicationDocument.DocumentTypeID;
        //                fundTransactionDocument.FileName = fundUtilizationParam.ApplicationDocument.FileName;
        //                fundTransactionDocument.FileSize = fundUtilizationParam.ApplicationDocument.FileSize;
        //                fundTransactionDocument.IsActive = true;
        //                fundTransactionDocument.CreatedByUserID = userEntity.UserID;
        //                fundTransactionDocument.CreatedDateTime = System.DateTime.Now;
        //                fundTransactionDocument.LastModifiedByUserID = userEntity.UserID;
        //                fundTransactionDocument.LastModifiedDateTime = System.DateTime.Now;
        //                fundTransactionDocument.PhysicalFileStorageKey = fundUtilizationParam.ApplicationDocument.PhysicalFileStorageKey;

        //                fundUtilization.FundTransactionDocuments.Add(fundTransactionDocument);
        //                 this._fundUtilizationRepository.SaveOrUpdateFundUtilization(fundUtilization, userEntity.UserID);
        //            }

        //            Logger.Debug(String.Format("Successfully exited UpdateFundUtilization for ProcessID-{0}", processInstance.ProcessId));
        //        }
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateFundUtilization for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited UpdateFundUtilization for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }

        //}

        //public void SaveTransitionComments(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered SaveTransitionComments for ProcessID-{0}", processInstance.ProcessId));
        //    DateTime currentDate = DateTime.Now;
        //    string comment = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Comment").Value.ToString();
        //    UserSessionEntity userEntity = (UserSessionEntity)processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "UserSessionModel").Value;
        //    try
        //    { 
        //        DbContextOptionsBuilder<ApplicationDBContext> optionsBuilder =  SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ApplicationDBContext>(), _sqlConnectionString.AppDBConnection);
        //        optionsBuilder.UseLazyLoadingProxies();
        //        var options = optionsBuilder.Options;

        //        using (var context = new ApplicationDBContext(options))
        //        {
        //            if (!String.IsNullOrEmpty(comment))
        //            {
        //                //Get The current Command
        //                string currentCommand = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value == null ? String.Empty : processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value.ToString();

                        
        //                LoanApplicationComment loanApplicationComment = new LoanApplicationComment();

        //                loanApplicationComment.Comments = comment;
        //                loanApplicationComment.ApplicationID = processInstance.ProcessId;
        //                loanApplicationComment.CreatedDateTime = DateTime.Now;
        //                loanApplicationComment.CreatedByUserID = userEntity.UserID;
        //                loanApplicationComment.IsActive = true;
        //                loanApplicationComment.LastModifiedDateTime = DateTime.Now;
        //                loanApplicationComment.LastModifiedByUserID = userEntity.UserID;
        //                //Check Whether you are getting right application
        //                loanApplicationComment.TransitionID = Convert.ToInt32(processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault() == null ? 0 : processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault().ID);
        //                _loanApplicationCommentRepository.SaveLoanApplicantComments(loanApplicationComment);
        //            }
        //            Logger.Debug(String.Format("Successfully exited SaveTransitionComments for ProcessID-{0}", processInstance.ProcessId));
        //        }
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited SaveTransitionComments for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited SaveTransitionComments for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }

        //}

        //public void SaveAgreementDetails(ProcessInstance processInstance)
        //{
        //    Logger.Debug(String.Format("Entered SaveAgreementDetails for ProcessID-{0}", processInstance.ProcessId));
        //    DateTime currentDate = DateTime.Now;
        //    UserSessionEntity userEntity = (UserSessionEntity)processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "UserSessionModel").Value;
        //    try
        //    {
        //        DbContextOptionsBuilder<ApplicationDBContext> optionsBuilder = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ApplicationDBContext>(), _sqlConnectionString.AppDBConnection);
        //        optionsBuilder.UseLazyLoadingProxies();
        //        var options = optionsBuilder.Options;

        //        using (var context = new ApplicationDBContext(options))
        //        {

        //            //Get The current Command
        //            string currentCommand = processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value == null ? String.Empty : processInstance.ProcessParameters.FirstOrDefault(a => a.Name == "Command").Value.ToString();
        //            string IPAdress = null;
        //            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        //            if (host != null)
        //            {
        //                if (host.AddressList.Count() > 0)
        //                {
        //                    foreach (var ip in host.AddressList)
        //                    {
        //                        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //                        {
        //                            IPAdress = ip.ToString();
        //                        }
        //                    }
        //                }
        //            }
        //            LoanApplicationAgreementDetail loanApplicationAgreementDetail = new LoanApplicationAgreementDetail();

        //            loanApplicationAgreementDetail.ApplicationID = processInstance.ProcessId;
        //            loanApplicationAgreementDetail.CreatedDateTime = DateTime.Now;
        //            loanApplicationAgreementDetail.CreatedByUserID = userEntity.UserID;
        //            loanApplicationAgreementDetail.IsActive = true;
        //            loanApplicationAgreementDetail.LastModifiedDateTime = DateTime.Now;
        //            loanApplicationAgreementDetail.LastModifiedByUserID = userEntity.UserID;
        //            loanApplicationAgreementDetail.IPAddress = IPAdress;
        //            //Check Whether you are getting right application
        //            loanApplicationAgreementDetail.TransitionID = Convert.ToInt32(processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault() == null ? 0 : processInstance.Workflow.Transitions.Where(a => a.From == processInstance.CurrentActivity && a.Trigger.Command.Name == currentCommand).FirstOrDefault().ID);
        //            this._loanApplicationAgreementDetailRepository.SaveLoanApplicationAgreementDetail(loanApplicationAgreementDetail);

        //            Logger.Debug(String.Format("Successfully exited SaveAgreementDetails for ProcessID-{0}", processInstance.ProcessId));
        //        }
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited SaveAgreementDetails for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(String.Format("Unsuccessfully exited SaveAgreementDetails for ProcessID-{0}", processInstance.ProcessId, ex));
        //        return;
        //    }

        //}
    }
}
