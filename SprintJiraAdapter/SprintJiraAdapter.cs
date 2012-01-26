using System;
using System.Collections.Generic;
using System.Configuration;
using Jira;
using SprintJiraAdapter.com.iqmetrix.pm;

namespace SprintJiraAdapter
{
    class SprintJiraAdapter
    {
        static void Main(string[] args)
        {
            
            var jiraSoapService = new JiraSoapServiceService();

            var userName = ConfigurationManager.AppSettings["JiraUserName"];
            var passWord = ConfigurationManager.AppSettings["JiraPassword"];
            string token = jiraSoapService.login(userName, passWord);
            var projectStr = ConfigurationManager.AppSettings["JiraProject"];
            RemoteProject project = jiraSoapService.getProjectByKey(token, projectStr);
            RemoteUser user = jiraSoapService.getUser(token, userName);
            
            string path = @"C:\Users\Public\IssuesFinal.txt"; //<---------------------------------------------------------------------------insert path here
            var allIssues = CSVParse.ParseCsvFile(path);

            foreach (var issue in allIssues)
            {
                var issueClass = InitializeIssue(issue);
                JiraAdapter.CreateIssueFromCsv(issueClass, token, user, project, jiraSoapService);
            }
            
            Console.WriteLine(Environment.NewLine + "All Issue Successfully Uploaded to Jira");
            Console.WriteLine(Environment.NewLine + "Done");
            Console.ReadKey();
        }


        private static JiraIssue InitializeIssue(List<string> issue)
        {

            JiraIssue newIssue = new JiraIssue(issue[0], issue[1], issue[2], issue[3], issue[4], issue[5], issue[6], issue[7], issue[8]);
            return newIssue;
        }
    }
}
