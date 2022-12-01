using System.Collections.Generic;
using System.Threading.Tasks;
using QUT4Net.Parallel;

namespace QUT4Net.Models
{
    public class QUTCollectionClient
    {
        private HiQClient Client { get; }

        public QUTCollectionClient(HiQClient client)
        {
            Client = client;
        }

        /// <summary>
        /// Scrapes all units, and then in parallel queries for thir outlines.
        /// </summary>
        /// <param name="workerCount">The max number of workers/concurrent requests querying unit outlines</param>
        /// <param name="limit">The max number of units to collect</param>
        /// <param name="skip">Skips a certain number of entires. This can be used to run a collection in multiple parts</param>
        /// <returns>A list of each unit with their respective outline info</returns>
        public async Task<List<QUTUnit>> CollectUnitInfoAsync(int workerCount = 1, int limit = -1, int skip = 0)
        {
            using (var collector = new UnitInfoCollector(Client, workerCount))
            {
                collector.Start();

                await foreach (var unitInfo in Client.Units.ReadUnitsAsync(limit, skip))
                {
                    collector.Enqueue(unitInfo);
                }

                return await collector.FinishAsync();
            }
        }
    }
}