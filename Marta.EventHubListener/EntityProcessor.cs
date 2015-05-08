
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.EventHubListener
{
    internal class EntityProcessor : IEventProcessor
    {
        public EntityProcessor()
        {
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("EntityProcessor opening.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));

            return Task.FromResult<object>(null);
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("EntityProcessor closing. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason.ToString()));

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var msg in messages)
            {
                var status = await JsonConvert.DeserializeObjectAsync<BusStatus>(Encoding.UTF8.GetString(msg.GetBytes()));

                var bus = GrainFactory.GetGrain<IBus>(status.VehicleId);

                await bus.UpdateStatus(status);
            }

            await context.CheckpointAsync();
        }
    }
}
