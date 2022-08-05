using System;
using System.Diagnostics;

using NodaTime;
using NodaTime.Extensions;

namespace MEI.Core.Infrastructure
{
    public interface IStopwatch
    {
        TimeSpan Elapsed { get; }

        void Start();

        void Stop();

        void Reset();

        void Restart();

        Duration ElapsedDuration();
    }

    public class SystemStopwatch
        : IStopwatch
    {
        private readonly Stopwatch _stopwatch;

        public SystemStopwatch()
        {
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public Duration ElapsedDuration()
        {
            return _stopwatch.ElapsedDuration();
        }

        public TimeSpan Elapsed => _stopwatch.Elapsed;
    }
}