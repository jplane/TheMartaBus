
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.EventHubListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for Orleans Silo to start. Press Enter to proceed...");
            Console.ReadLine(); 
            
            GrainClient.Initialize("ClientConfiguration.xml");

            var ehConnectionString = CloudConfigurationManager.GetSetting("ehConnectionString");
            var ehName = CloudConfigurationManager.GetSetting("ehName");
            var storageConnectionString = CloudConfigurationManager.GetSetting("azureTableConnection");

            //var storageProcHost = new EventProcessorHost(Guid.NewGuid().ToString(), ehName, "forstorage", ehConnectionString, storageConnectionString);
            //storageProcHost.RegisterEventProcessorAsync<StorageProcessor>().Wait();

            var entityProcHost = new EventProcessorHost(Guid.NewGuid().ToString(), ehName, "fororleans", ehConnectionString, storageConnectionString);
            entityProcHost.RegisterEventProcessorAsync<EntityProcessor>().Wait();

            Console.ReadLine();
        }
    }
}
