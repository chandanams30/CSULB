using ThoughtFocus.Common.WorkFlowDataAccess;

namespace ThoughtFocus.Common.Workflow.DbPersistance
{
    public abstract class DbProvider
    {
        protected string ConnectionString { get; set; }

        public DbProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public WorkFlowContext CreateContext()
        {
            WorkFlowContext context = null;
            try
            {
                 context =  new WorkFlowContext(ConnectionString);
                return context;
            }
            finally
            {
               
            }
        }
    }
}
