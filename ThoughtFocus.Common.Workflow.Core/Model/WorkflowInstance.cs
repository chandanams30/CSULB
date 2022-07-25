using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ThoughtFocus.Common.Workflow.Core.Model
{

    public class ProcessInstance
    {
       
        public long ProcessId { get;  set; }
        public Guid SchemeId { get;  set; }
        public WorkflowDefinition Workflow { get; set; }
        public bool IsSchemeObsolete { get;  set; }
        public bool IsDeterminingParametersChanged { get;  set; }
        [NotMapped]
        public long WorkFlowID { get; set; }

        public IEnumerable<ParameterDefinitionWithValue> ProcessParameters
        {
            get { return _workflowParameters; }
        }

        private readonly List<ParameterDefinitionWithValue> _workflowParameters = new List<ParameterDefinitionWithValue>();


        public static ProcessInstance Create(Guid schemeId, long processId, WorkflowDefinition workflow, bool isSchemeObsolete, bool isDeterminingParametersChanged)
        {
            return new ProcessInstance() {SchemeId = schemeId, ProcessId = processId, Workflow = workflow, IsSchemeObsolete = isSchemeObsolete, IsDeterminingParametersChanged = isDeterminingParametersChanged};
        }

        public void AddParameter (ParameterDefinitionWithValue parameter)
        {
            _workflowParameters.RemoveAll(p => p.Name == parameter.Name);
            _workflowParameters.Add(parameter);
        }

        public void AddParameters(IEnumerable<ParameterDefinitionWithValue> parameters)
        {
            if (parameters != null)
            {
                _workflowParameters.RemoveAll(ep => parameters.Count(p => p.Name == ep.Name) > 0);
                _workflowParameters.AddRange(parameters);
            }
        }

        public ParameterDefinitionWithValue GetParameter(string name)
        {
            return _workflowParameters.SingleOrDefault(p => p.Name == name);
        }

        
        public string CurrentActivityName
        {
            get
            {
                var parameter = GetParameter(DefaultDefinitions.ParameterCurrentActivity.Name);
                return parameter == null ? null : (string) parameter.Value;
            }
        }

        public ActivityDefinition CurrentActivity
        {
            get { return Workflow.FindActivity(CurrentActivityName); }
        }

        public void SetProcessParameters(List<ParameterDefinitionWithValue> pd)
        {
            _workflowParameters.Clear();
            AddParameters(pd);
        }

        #region Localized
        public string GetLocalizedStateName (string stateName, CultureInfo culture)
        {
            return Workflow.GetLocalizedStateName(stateName, culture);
        }

        public string GetLocalizedCommandName(string commandName, CultureInfo culture)
        {
            return Workflow.GetLocalizedCommandName(commandName, culture);
        }
        #endregion

        #region Debug info

        public string ProcessParametersToString(ParameterPurposeEnumeration purpose)
        {
            StringBuilder res = new StringBuilder();
            foreach (var item in ProcessParameters.Where(p=>p.Purpose == purpose).OrderBy(p=>p.Name))
            {
                res.AppendLine(string.Format("{0}='{1}'", item.Name, item.Value));
            }
            return res.ToString();
        }
        #endregion

        public string CurrentState { get; set; }

        public string ExecutedActivityState { get; set; }

        public string CurrentCommand { get; set; }

        public string IdentityId { get; set; }

        public string ExecutedTimer { get; set; }
    }
}
