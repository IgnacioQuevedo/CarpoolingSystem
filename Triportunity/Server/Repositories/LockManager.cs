using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Repositories
{
    public static class LockManager
    {
        private static readonly object _lock = new object();
        private static int _readers = 0;
        private static bool _writerWaiting = false;
        private static bool _writeInProgress = false;

        public static void StartReading()
        {
            lock (_lock)
            {
                // Esperar mientras haya un escritor trabajando o esperando para evitar inanición de escritores.
                while (_writeInProgress || _writerWaiting)
                {
                    Monitor.Wait(_lock);
                }
                _readers++;
            }
        }

        public static void StopReading()
        {
            lock (_lock)
            {
                _readers--;
                if (_readers == 0)
                {
                    // Notificar a los escritores que pueden proceder si no hay lectores.
                    Monitor.PulseAll(_lock);
                }
            }
        }

        public static void StartWriting()
        {
            lock (_lock)
            {
                _writerWaiting = true;
                // Esperar mientras haya un escritor trabajando o lectores leyendo.
                while (_writeInProgress || _readers > 0)
                {
                    Monitor.Wait(_lock);
                }
                _writerWaiting = false;
                _writeInProgress = true;
            }
        }

        public static void StopWriting()
        {
            lock (_lock)
            {
                _writeInProgress = false;
                // Notificar a todos que el escritor ha terminado.
                Monitor.PulseAll(_lock);
            }
        }
    }
}
