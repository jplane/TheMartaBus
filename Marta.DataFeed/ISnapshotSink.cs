
using Marta.Common;
using System;

namespace Marta.DataFeed
{
    public interface ISnapshotSink
    {
        void HandleSnapshot(BusStatus snapshot);
    }
}
