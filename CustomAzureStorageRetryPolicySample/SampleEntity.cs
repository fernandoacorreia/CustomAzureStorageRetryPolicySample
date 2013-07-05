using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace CustomAzureStorageRetryPolicySample
{
    /// <summary>
    /// Just a sample Table Storage entity.
    /// </summary>
    public class SampleEntity : TableEntity
    {
        public SampleEntity(Guid id)
        {
            this.PartitionKey = id.ToString().Substring(0, 4);
            this.RowKey = id.ToString();
        }

        public SampleEntity() { }

        public string CreatedAt { get; set; }

        public int Number { get; set; }
    }
}
