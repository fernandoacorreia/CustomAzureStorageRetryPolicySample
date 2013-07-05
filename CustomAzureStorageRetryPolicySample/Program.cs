using System;

namespace CustomAzureStorageRetryPolicySample
{
    /// <summary>
    /// Sample console application that shows how to use a custom retry policy with Windows Azure Storage.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EntityCreator creator = new EntityCreator();
                creator.CreateEntities(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error: {0}", ex));
                throw;
            }
        }
    }
}
