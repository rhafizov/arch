using System;
using System.Collections.Generic;
using System.Threading;

namespace Archiver.Conveyer
{
    class ProducerConsumerQueuesConveyer : IDisposable 
    {
        private EventWaitHandle _whTwo = new AutoResetEvent(false);
        private EventWaitHandle _whOne = new AutoResetEvent(false);

        private Thread _queueOneWorker;
        private Thread _QueueTwoWorker;

        private readonly object _queueOneLocker = new object();
        private readonly object _queueTwoLocker = new object();

        private Queue<byte[]> _queueOne = new Queue<byte[]>();
        private Queue<byte[]> _queueTwo = new Queue<byte[]>();

        private int _maxStoredItemsAtSameTime;

        public ProducerConsumerQueuesConveyer(Func<byte[], byte[]> firstQueueOperation, Action<byte[]> lastQueueOperation, int maxStoredItemsAtSameTime)
        {
            _maxStoredItemsAtSameTime = maxStoredItemsAtSameTime;
            _queueOneWorker = new Thread(() => { QueueOneWork(firstQueueOperation); });
            _QueueTwoWorker = new Thread(() => { QueueTwoWork(lastQueueOperation); });
            _queueOneWorker.Start();
            _QueueTwoWorker.Start();
        }

        private void QueueOneWork(Func<byte[], byte[]> func)
        {
            while (true)
            {
                byte[] chunk = null;
                lock (_queueOneLocker)
                {
                    if (_queueOne.Count > 0)
                    {                        
                        chunk = _queueOne.Dequeue();
                        if (chunk == null) 
                        {
                            EnqueueToQueueTwo(null);
                            return;
                        }
                    }
                }
                if (chunk != null)
                {
                    EnqueueToQueueTwo(func(chunk));
                }
                else
                {
                    _whOne.WaitOne();
                }
            }
        }

        private void QueueTwoWork(Action<byte[]> action)
        {
            while (true)
            {
                byte[] chunk = null;
                lock (_queueTwoLocker)
                {
                    if (_queueTwo.Count > 0)
                    {                        
                        chunk = _queueTwo.Dequeue();
                        if (chunk == null) return;
                    }
                }
                if (chunk != null)
                {
                    action(chunk);
                }
                else
                {
                    _whTwo.WaitOne();
                }
            }
        }
        
        public void EnqueueChunkToQueueOne(byte[] chunk)
        {
            while (_maxStoredItemsAtSameTime < (_queueOne.Count + _queueTwo.Count))
            {
                Thread.Sleep(1000);
            }

            lock (_queueOneLocker) _queueOne.Enqueue(chunk);
            _whOne.Set();
        }

        private void EnqueueToQueueTwo(byte[] chunk)
        {


            lock (_queueTwoLocker) _queueTwo.Enqueue(chunk);
            _whTwo.Set();
        }
        public void Dispose()
        {
            EnqueueChunkToQueueOne(null);

            _queueOneWorker.Join();
            _QueueTwoWorker.Join();

            _whOne.Close();
            _whTwo.Close();
        }
    }
}

