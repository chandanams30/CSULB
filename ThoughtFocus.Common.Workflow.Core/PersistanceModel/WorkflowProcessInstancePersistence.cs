using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.PersistanceModel
{
    [Table("WorkflowProcessInstancePersistence", Schema = "WorkFlow")]
    public class WorkflowProcessInstancePersistence
    {
          [Key]
          public Guid WorkflowProcessInstancePersistenceID { get; set; }

          public string ParameterName { get; set; }

          public long ProcessInstanceID { get; set; }

          public long WorkFlowDefinationID { get; set; }

          public string Value { get; set; }

    }
}
