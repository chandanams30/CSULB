using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("TriggerDefinition", Schema = "WorkFlow")]
    public partial class TriggerDefinition
    {
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }
        [NotMapped]
        public int Type_Id { get; set; }    
        [NotMapped]   
        public long Command_ID { get; set; }
 
        public static TriggerDefinition Create(string type)
        {
            TriggerTypeEnumeration parsedType;
            Enum.TryParse(type, true, out parsedType);

            switch (parsedType)
            {
                case TriggerTypeEnumeration.Auto:
                    return Auto;
                    break;
                case TriggerTypeEnumeration.Command:
                    return new CommandTriggerDefinition();
                case TriggerTypeEnumeration.Timer:
                    return new TimerTriggerDefinition();
            }

            return null;
        }

        public string NameRef
        {
            get
            {
                string res;
                TriggerTypeEnumeration typeEnum = Type;
                switch (typeEnum)
                {
                    case TriggerTypeEnumeration.Command:
                        res = (Command == null ? string.Empty : Command.Name);
                        break;
                    case TriggerTypeEnumeration.Timer:
                        res = (Timer == null ? string.Empty : Timer.Name);
                        break;
                    default:
                        res = string.Empty;
                        break;
                }
                return res;
            }
        }
        public CommandDefinition CommandsLazyLoading;
        [ForeignKey("Command_ID")]
        public virtual CommandDefinition Command
        {
            get { return (CommandsLazyLoading); }
            set { }
        }
       
         [BackingField(nameof(CommandsLazyLoading))]
         public virtual CommandDefinition CommandList{get{return (this as CommandTriggerDefinition).Command;} set{}}
     
        [NotMapped]
        public TimerDefinition Timer
        {
            get { return (this as TimerTriggerDefinition).Timer; }
        }
        
        public static TriggerDefinition Auto
        {
            get { return new AutoTriggerDefinition(); }
        }

        [ForeignKey("Type_Id")]
        public virtual TriggerType Type { get; set; }        
      
    }

    public class CommandTriggerDefinition : TriggerDefinition
    {
        public override TriggerType Type
        {
            get { return TriggerTypeEnumeration.Command; }
            set { }
        }
        public override CommandDefinition Command { get; set; }

    }
    [NotMapped]
    public class TimerTriggerDefinition : TriggerDefinition
    {
        public override TriggerType Type
        {
            get { return TriggerTypeEnumeration.Timer; }
        }

        public TimerDefinition Timer { get; set; }
    }

    [NotMapped]
    public class AutoTriggerDefinition : TriggerDefinition
    {
        public override TriggerType Type
        {
            get { return TriggerTypeEnumeration.Auto; }
        }
    }



    public enum TriggerTypeEnumeration
    {
        Command = 1,
        Auto = 2,
        Timer = 3
    }

    [Table("ConditionDefinition", Schema = "WorkFlow")]
    public class ConditionDefinition
    {
        public ConditionDefinition()
        {
        }
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }

        public int ConditionTypeID { get; set; }

        public bool? ResultOnPreExecution { get; set; }
        public long? Action_ID { get; set; }

        [ForeignKey("ConditionTypeID")]
        public virtual ConditionType ConditionType { get; set; }

        [ForeignKey("Action_ID")]
        public virtual ActionDefinition Action { get; set; }



        public static ConditionDefinition Create(string type)
        {
            return Create(type, null, null);
        }

        public static ConditionDefinition Create(string type, string resultOnPreExecution)
        {
            return Create(type, null, resultOnPreExecution);
        }

        public static ConditionDefinition Create(string type, ActionDefinition action, string resultOnPreExecution)
        {
            ConditionTypeEnumeration parsedType;
            Enum.TryParse(type, true, out parsedType);

            return new ConditionDefinition() { Action = action, ConditionType = parsedType, ResultOnPreExecution = string.IsNullOrEmpty(resultOnPreExecution) ? (bool?)null : bool.Parse(resultOnPreExecution) };
        }

        public static ConditionDefinition Always
        {
            get { return new ConditionDefinition() { ConditionType = ConditionTypeEnumeration.Always }; }
        }

    }

    public enum ConditionTypeEnumeration
    {
        Action = 1,
        Always = 2,
        Otherwise = 3

    }


}
