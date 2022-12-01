using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QUT4Net.Models;

namespace QUT4Net.Parallel
{
    /// <summary>
    /// A tool to collect full unit info on parallel
    /// </summary>
    public class UnitInfoCollector : IDisposable
    {
        /// <summary>
        /// The max number of concurrent requests for unit outlines
        /// </summary>
        public int MaxWorkers { get; }

        /// <summary>
        /// The current number of workers processing unit outlines
        /// </summary>
        public int CurrentWorkers { get; private set; } = 0;

        /// <summary>
        /// Currently processed units.
        /// Call <seealso cref="FinishAsync"/> to wait for the collection to be completed
        /// </summary>
        public List<QUTUnit> Units { get; } = new List<QUTUnit>();


        private HiQClient m_Client { get; }
        private ConcurrentQueue<UnitInfo> m_Ingest { get; } = new ConcurrentQueue<UnitInfo>();
        private SemaphoreSlim m_IngestSemaphore { get; } = new SemaphoreSlim(0);

        private CancellationTokenSource m_TokenSource { get; } = new CancellationTokenSource();

        private TaskCompletionSource<List<QUTUnit>> m_CompletionSource { get; } = new TaskCompletionSource<List<QUTUnit>>();
        private TaskCompletionSource<object> m_QueueEmpty { get; } = new TaskCompletionSource<object>();

        private bool Finishing { get; set; } = false;

        public UnitInfoCollector(HiQClient client, int workerCount)
        {
            m_Client = client;
            MaxWorkers = workerCount;
        }

        /// <summary>
        /// Starts the worker tasks to begin processing requests
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < MaxWorkers; i++)
            {
                var workerTask = Task.Run(Worker);
                workerTask.ContinueWith(WorkerExit);
            }
        }

        /// <summary>
        /// Signal that there are no more requests, and to start exiting workers as the queue is cleared.
        /// </summary>
        /// <returns>The final list of <seealso cref="QUTUnit"/>, with unit info and outline</returns>
        public async Task<List<QUTUnit>> FinishAsync()
        {
            Finishing = true;
            await m_QueueEmpty.Task;
            m_TokenSource.Cancel();
            return await m_CompletionSource.Task;
        }

        /// <summary>
        /// Enqueues a unit to be processed.
        /// </summary>
        public void Enqueue(UnitInfo info)
        {
            m_Ingest.Enqueue(info);
            m_IngestSemaphore.Release();
        }

        private void WorkerExit(Task _)
        {
            CurrentWorkers--;

            if (CurrentWorkers <= 0)
            {
                m_CompletionSource.SetResult(Units);
            }
        }

        private bool TryDequeue(out UnitInfo info)
        {
            var result = m_Ingest.TryDequeue(out info);

            if (m_Ingest.IsEmpty && Finishing)
            {
                m_QueueEmpty.SetResult(null);
            }

            return result;
        }

        private async Task Worker()
        {
            while (!m_TokenSource.IsCancellationRequested)
            {
                await m_IngestSemaphore.WaitAsync(m_TokenSource.Token);
                if (TryDequeue(out var unit))
                {
                    try
                    {
                        var outline = await m_Client.UnitOutlines.GetUnitOutlineAsync(unit.OutlineURL);
                        lock (Units)
                        {
                            Units.Add(new QUTUnit(unit, outline));
                        }
                    }
                    catch (Exception)
                    {
                        lock (Units)
                        {
                            Units.Add(new QUTUnit(unit, null));
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            m_IngestSemaphore.Dispose();
            m_TokenSource.Dispose();
        }
    }
}