
using Marta.Common;
using Newtonsoft.Json;
using System;

namespace Marta.DataFeed
{
    public class ConsoleSink : ISnapshotSink
    {
        public ConsoleSink()
        {
        }

        public void HandleSnapshot(BusStatus snapshot)
        {
            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
