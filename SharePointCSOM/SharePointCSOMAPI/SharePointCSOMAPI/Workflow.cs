using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCSOMAPI
{
    class Workflow
    {
        public static void Test(ClientContext context)
        {
            var workflowServiceManager = new WorkflowServicesManager(context, context.Web);
            var workflowDeploymentService = workflowServiceManager.GetWorkflowDeploymentService();
            var workflowSubscriptionService = workflowServiceManager.GetWorkflowSubscriptionService();
            var subscriptions = workflowSubscriptionService.EnumerateSubscriptions();
            context.Load(workflowDeploymentService);
            context.Load(workflowSubscriptionService);
            context.Load(subscriptions);
            context.ExecuteQuery();
            var subscription = subscriptions.First(temp => temp.Name == "7777");
            var workflowDefination = workflowDeploymentService.GetDefinition(subscription.Id);
            context.Load(workflowDefination);
            context.ExecuteQuery();

            var folderUrl = workflowDefination.Properties["Definition.Path"];
            var folder = context.Web.GetFolderByServerRelativeUrl(folderUrl);
            context.Load(folder);
            context.Load(folder.Files);
            context.ExecuteQuery();
            foreach (var file in folder.Files)
            {
                context.Load(file.Properties);
                context.Load(file.ListItemAllFields);
                context.ExecuteQuery();
            }

        }
        public static void WFSVCListTest(ClientContext context)
        {
             var list = context.Web.GetListByTitle("wfsvc");
            context.Load(list);
            context.ExecuteQuery();
            list.Update();
            context.ExecuteQuery();
        }
        public static void Get13ModeWorkflow(ClientContext context)
        {
            //var info = context.GetFormDigestDirect();
            var workflowServicesManager = new WorkflowServicesManager(context, context.Web);
            var deploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var workflowDefinitions = deploymentService.EnumerateDefinitions(false);

            //only load what we need
            context.Load(workflowDefinitions, a => a.Include(
                                                    b => b.Description,
                                                    b => b.Id,
                                                    b => b.Published,
                                                    b => b.RestrictToScope,
                                                    b => b.RestrictToType,
                                                    b => b.DisplayName,
                                                    b => b.Properties));
            context.ExecuteQuery();
            var workflowId = new Guid("0f2cbcb4-5a0c-4c5a-b332-8f1e3c3aa04f");
            foreach (var wf in workflowDefinitions)
            {
                if (wf.Id == workflowId)
                {

                }
            }

        }
        public static void Load13ModeWorklfow(ClientContext context)
        {
            var file = context.Web.GetFileByUrl("/sites/Test7/wfsvc/892036153093485d847117338349817b/WorkflowAssociation_84713e587b7a464d8650bdce3b5f1b8c");
            context.Load(file);
            context.Load(file.Properties);
            context.Load(file.ListItemAllFields);
            context.ExecuteQuery();
            var workflowServiceManager = new WorkflowServicesManager(context, context.Web);
            context.Load(workflowServiceManager);
            context.ExecuteQuery();
            if (workflowServiceManager.IsConnected)
            {
                var subScriptionService = workflowServiceManager.GetWorkflowSubscriptionService();
                var subscription = subScriptionService.GetSubscription(new Guid("8ee0cf74-a2b5-4ab2-a192-ec38e4fe6577"));
                context.Load(subscription);
                context.ExecuteQuery();
                //Backup13ModeStartOption(context, cache, subScriptionService, subscriptions);
                context.ExecuteQuery();
            }
        }
    }
}
