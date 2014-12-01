using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Table;
using AnimalWorker.Models;

namespace AnimalWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        string qConnectionString = CloudConfigurationManager.GetSetting("animalqueu");
        string qName = "animalqueu";

        string fuladjurstorageConnectionString = CloudConfigurationManager.GetSetting("fuladjurstorage"); 

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("AnimalWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("AnimalWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("AnimalWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("AnimalWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working like an animal");

                //Skapa ny Queueclient 
                QueueClient qc = QueueClient.CreateFromConnectionString(qConnectionString, qName);
                //Ta emot det meddelande som kommer fr�n web role.                 
                BrokeredMessage msg = qc.Receive();
                if (msg != null)
                {
                    try
                    {
                        Trace.WriteLine("Comment Received");
                        msg.Complete();

                        var name = (string)msg.Properties["Name"];
                        var text = (string)msg.Properties["Text"];
                        var animalid = (int)msg.Properties["AnimalId"];

                        SaveCommentToStorage(name, text, animalid); 
                    }
                    catch (Exception)
                    {
                        // Problem, l�s upp message i queue 
                        msg.Abandon();
                    }
                } 

                await Task.Delay(1000);
            }
        }

        private void SaveCommentToStorage(string name, string text, int animalid)
        {
            //det namn v�r table ska ha 
            string tableName = "UglyComments";
            //Connection till table storage account 
            CloudStorageAccount account = CloudStorageAccount.Parse(fuladjurstorageConnectionString);
            //Klient f�r table storage 
            CloudTableClient tableStorage = account.CreateCloudTableClient();
            //H�mta en reference till tablen, om inte finns, skapa table 
            CloudTable table = tableStorage.GetTableReference(tableName);
            table.CreateIfNotExists();

            var key = Guid.NewGuid().ToString();

            var comment = new UglyComment(key)
            {
                Name = name,
                Text = text,
                AnimalId = animalid
            };

            //Sparar personen i signups table 
            TableOperation insertOperation = TableOperation.Insert(comment);
            table.Execute(insertOperation);

        } 
    }
}
