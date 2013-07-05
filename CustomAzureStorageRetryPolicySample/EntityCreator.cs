using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CustomAzureStorageRetryPolicySample
{
    /// <summary>
    /// Sample code that shows how to use a custom retry policy with Windows Azure Storage by capturing retry events.
    /// </summary>
    public class EntityCreator
    {
        private CloudTable table;

        /// <summary>
        /// Initializes the sample class, setting up the custom retry policy and its event handler.
        /// </summary>
        public EntityCreator()
        {
            // Retrieve the storage account from the connection string.
            Console.WriteLine("Retrieving storage account");
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Prepare the retry policy.
            EventExponentialRetry retryPolicy = new EventExponentialRetry(TimeSpan.FromSeconds(2), 10);
            retryPolicy.RaiseRetryEvent += HandleRetryEvent;

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.RetryPolicy = retryPolicy;

            // Create the table if it doesn't exist.
            Console.WriteLine("Preparing table.");
            const string tableName = "Sample";
            table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Creates several Table Storage entities.
        /// </summary>
        /// <param name="count"></param>
        public void CreateEntities(int count)
        {
            // Create several entities.
            for (int i = 1; i <= count; i++)
            {
                // Create a new entity.
                Random rnd = new Random();
                SampleEntity entity = new SampleEntity(Guid.NewGuid())
                {
                    CreatedAt = DateTime.UtcNow.ToString("o"),
                    Number = rnd.Next(0, int.MaxValue)
                };

                // Create the TableOperation that inserts the entity.
                Console.WriteLine(String.Format("Inserting entity {0}.", i));
                TableOperation insertOperation = TableOperation.Insert(entity);

                // Execute the insert operation.
                table.Execute(insertOperation);
                Console.WriteLine(String.Format("Entity '{0}' inserted.", entity.RowKey));
            }
        }

        /// <summary>
        /// Handles retry events.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data.</param>
        void HandleRetryEvent(object sender, RetryEventArgs e)
        {
            Console.WriteLine(String.Format("Retry {0}: {1}", e.CurrentRetryCount, e.LastException));
        }
    }
}
