namespace Jira
{
    public class JiraIssue
    {
        public string SprintIssueId { get; set; }
        public string IssueType { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Priority { get; set; }
        public string EnvironmentFoundIn { get; set; }
        public string DateCreated { get; set; }
        public string Log { get; set; }

        public JiraIssue(string id, string type, string summary, string description, string severity, string priority, string environment, string date, string log)
        {
            SprintIssueId = id;
            IssueType = type;
            Summary = summary;
            Description = description;
            Severity = severity;
            Priority = priority;
            EnvironmentFoundIn = "UAT";
            DateCreated = date;
            Log = log;
        }
    
    }
}
