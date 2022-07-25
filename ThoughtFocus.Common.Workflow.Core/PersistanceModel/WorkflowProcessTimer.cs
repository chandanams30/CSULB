using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.PersistanceModel
{
    [Table("WorkflowProcessTimer", Schema = "WorkFlow")]
    public class WorkflowProcessTimer
    {
         [Key]
         public Guid  WorkflowProcessTimerID{get;set;}
         public string Name{get;set;}
         public DateTime NextExecutionDateTime{get;set;}
         public long  ProcessInstanceID { get; set; }
         public long WorkFlowDefinationID { get; set; }
         public bool Ignore { get; set; }
                       
    }
}
