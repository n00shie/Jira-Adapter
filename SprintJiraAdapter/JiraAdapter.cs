using System;
using System.Collections.Generic;
using System.Configuration;
using Jira;
using SprintJiraAdapter.com.iqmetrix.pm;

namespace SprintJiraAdapter
{
    class JiraAdapter
    {
        
        public static void CreateIssueFromCsv(JiraIssue jiraIssue, string token,  RemoteUser user, RemoteProject project, JiraSoapServiceService jiraSoapService)
        {
            if (AreDuplicateIssues(jiraSoapService, token, project, jiraIssue) != null)
            {
                Console.WriteLine("Duplicate issue detected.");
                UpdateIssue(jiraSoapService, token, project, jiraIssue);
            }

            else
            {
                RemoteIssue issue = CreateIssue(project, jiraSoapService, token);

                //CreateComponents(project, jiraSoapService, token, issue);

                RemoteCustomFieldValue[] listOfCustomFields = InitCustomFields(issue, jiraIssue);

                InitIssue(issue, jiraIssue, listOfCustomFields, user);

                RemoteComment comment = CreateCommentLog(jiraIssue);

                PushToJira(jiraSoapService, token, issue, comment);
            }
            

           //DeleteAllIssues(jiraSoapService, token);
        }

        private static void UpdateIssue(JiraSoapServiceService jiraSoapService, string token, RemoteProject project, JiraIssue jiraIssue)
        {
            string updateMessage = "--- This is an automated message --- " + Environment.NewLine
                + "This issue and the following fields have been updated: " + Environment.NewLine;

            string searchTerms = AreDuplicateIssues(jiraSoapService, token, project, jiraIssue);
            string[] keys = {project.key};

            RemoteComment updateComment = new RemoteComment();
            var existingDuplicateIssue = jiraSoapService.getIssuesFromTextSearchWithProject(token, keys, searchTerms, 1);

            if (jiraIssue.Summary.Replace("\r", string.Empty) != existingDuplicateIssue[0].summary)
            {
                updateMessage += ("-----------------------------------" + Environment.NewLine
                                    +"Summary: " + Environment.NewLine 
                                    + jiraIssue.Summary + Environment.NewLine);
            }

            if (jiraIssue.Description.Replace("\r", string.Empty) != existingDuplicateIssue[0].description.Trim())
            {
                int length = jiraIssue.Description.Length;
                for (int i = 0; i < length; i++)
                {
                    if (jiraIssue.Description[i] != existingDuplicateIssue[0].description[i])
                        break;
                }

                int len = existingDuplicateIssue[0].description.Length;
                updateMessage += ("-----------------------------------" + Environment.NewLine
                                    +"Description: " + Environment.NewLine
                                    + jiraIssue.Description + Environment.NewLine);
            }

            else
            {
                updateMessage = "--- This is an automated message --- " + Environment.NewLine
                                + "Identical issue with no updated fields was attempted to be imported";
            }

            updateComment.body = updateMessage;
            jiraSoapService.addComment(token, existingDuplicateIssue[0].key, updateComment);

            System.Diagnostics.Debug.WriteLine("Successfully updated issue http://pm.iqmetrix.com/browse/" + existingDuplicateIssue[0].key);
            Console.WriteLine("Successfully updated issue http://pm.iqmetrix.com/browse/" + existingDuplicateIssue[0].key + Environment.NewLine);
        }

        static string AreDuplicateIssues(JiraSoapServiceService jiraSoapService, string token, RemoteProject project, JiraIssue uninitializedIssue)
        {
            List<string> listOfSprintIDs = GetListOfSprintIDs(project, jiraSoapService, token, "submitted");

            foreach (var SprintID in listOfSprintIDs)
            {
                if (uninitializedIssue.SprintIssueId == SprintID)
                {
                    return SprintID;
                }
            }
            return null;
        }

        private static void PushToJira(JiraSoapServiceService jiraSoapService, string token, RemoteIssue issue, RemoteComment comment)
        {
            RemoteIssue returnedIssue = jiraSoapService.createIssue(token, issue);
            jiraSoapService.addComment(token, returnedIssue.key, comment);

            System.Diagnostics.Debug.WriteLine("Successfully created issue http://pm.iqmetrix.com/browse/" + returnedIssue.key);
            Console.WriteLine("Successfully created issue http://pm.iqmetrix.com/browse/" + returnedIssue.key + Environment.NewLine);
        }

        private static List<string> GetListOfSprintIDs(RemoteProject project, JiraSoapServiceService jiraSoapService, string token, string searchTerms)
        {
            string[] keys = {project.key};
            string[] s = keys;
            var listOfIssues = jiraSoapService.getIssuesFromTextSearchWithProject(token, keys, searchTerms, 500);
            var n = listOfIssues.Length;

            var listOfSprintIDs = new List<string>();
            foreach (var oneissue in listOfIssues)
            {
                RemoteCustomFieldValue[] customfields = oneissue.customFieldValues;
               
                for (int i = customfields.Length - 1; i >= 0; i--)
                {
                    var temp = 0;
                    string[] fieldvalues = customfields[i].values;

                    for (int j = 0; j < fieldvalues.Length; j++)
                    {
                        if (int.TryParse(fieldvalues[j], out temp) && fieldvalues[j].ToString().Length == 6)
                        {
                            listOfSprintIDs.Add(fieldvalues[j]);
                        }

                    }
                }
            }
            return listOfSprintIDs;
        }

        private static void CreateComponents(RemoteProject project, JiraSoapServiceService jiraSoapService, string token, RemoteIssue issue)
        {
            List<RemoteComponent> components =
                new List<RemoteComponent>(jiraSoapService.getComponents(token, project.key));
            foreach (RemoteComponent component in components)
            {
                if (component.name.Equals("MM - Strategies"))
                {
                    issue.components = new RemoteComponent[] {component};
                }
            }
        }

        private static RemoteIssue CreateIssue(RemoteProject project, JiraSoapServiceService jiraSoapService, string token)
        {
            System.Diagnostics.Debug.WriteLine("Creating a new issue on http://jira/jira ...");
            Console.WriteLine("Creating a new issue on http://jira/jira ...");

            RemoteIssue issue = new RemoteIssue();

            issue.project = project.key;

            RemoteVersion Iteration = new RemoteVersion();
            Iteration.id = ConfigurationManager.AppSettings["JiraIteration"];
            RemoteVersion[] arrayOfVersions = {Iteration};

            issue.affectsVersions = arrayOfVersions;

            List<RemoteIssueType> issueTypes =
                new List<RemoteIssueType>(jiraSoapService.getIssueTypesForProject(token, project.id));
            foreach (RemoteIssueType issueType in issueTypes)
            {
                if (issueType.name.Equals("Bug"))
                {
                    issue.type = issueType.id;
                }
            }
            return issue;
        }

        public static void DeleteAllIssues(JiraSoapServiceService jiraSoapService, string token)
        {

            for (int i = 100; i < 200; i++)
            {
                string number = i.ToString();
                string IssueID = ConfigurationManager.AppSettings["JiraProject"] + "-" + number;
                try
                {
                    jiraSoapService.deleteIssue(token, IssueID); 
                }
                catch (Exception e)
                {
                    Console.WriteLine("Tried to delete issue: " + IssueID + " " + e.Message + Environment.NewLine);
                    continue;
                }
                Console.WriteLine("Deleted Issue: " + IssueID);
            }
        }

      
        private static RemoteCustomFieldValue[] InitCustomFields(RemoteIssue issue, JiraIssue jiraIssue)
        {
            string SprintIdFieldId = "customfield_10160";
            string SprintDateFieldId = "customfield_10563";
            string EnvironmentFieldId = "customfield_10062";
            string LabelId = "labels";

            string[] sprintIdNumber = { jiraIssue.SprintIssueId };
            string[] sprintDate = { jiraIssue.DateCreated };
            string[] environment = {jiraIssue.EnvironmentFoundIn};
            string[] labels = {ConfigurationManager.AppSettings["JiraIteration"]};

            RemoteCustomFieldValue sprintId = new RemoteCustomFieldValue();
            RemoteCustomFieldValue sprintIssueDate = new RemoteCustomFieldValue();
            RemoteCustomFieldValue environmentFoundIn = new RemoteCustomFieldValue();
            RemoteCustomFieldValue label = new RemoteCustomFieldValue();

            sprintId.customfieldId = SprintIdFieldId;
            sprintId.values = sprintIdNumber;

            sprintIssueDate.customfieldId = SprintDateFieldId;
            sprintIssueDate.values = sprintDate;

            environmentFoundIn.customfieldId = EnvironmentFieldId;
            environmentFoundIn.values = environment;

            label.customfieldId = LabelId;
            label.values = labels;

            RemoteCustomFieldValue[] listOfCustomFields = {sprintId, sprintIssueDate, environmentFoundIn};

            issue.customFieldValues = listOfCustomFields;
            return listOfCustomFields;
        }

        private static void InitIssue(RemoteIssue issue, JiraIssue jiraIssue, RemoteCustomFieldValue[] listOfCustomFields, RemoteUser user)
        {
            issue.reporter = user.name;
            issue.summary = jiraIssue.Summary;
            issue.description = jiraIssue.Description;
            issue.environment = jiraIssue.EnvironmentFoundIn;
            issue.customFieldValues = listOfCustomFields;
        }

        private static RemoteComment CreateCommentLog(JiraIssue jiraIssue)
        {
            RemoteComment comment = new RemoteComment();
            comment.body = jiraIssue.Log;
            return comment;
        }
    }
}
    

