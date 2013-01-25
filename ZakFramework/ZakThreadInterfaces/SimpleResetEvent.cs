
using System;
using System.Diagnostics;
using System.Threading;

namespace ZakThread.Threading
{
	public class SimpleResetEvent 
	{
		private volatile int m_eventState;
		private ManualResetEventSlim m_event;

    private Thread m_worker;

    private const int EVENT_SET     = 1;
    private const int EVENT_NOT_SET = 2;
    private const int EVENT_ON_WAIT = 3;

		public SimpleResetEvent(bool initialState, Thread workerThread = null)
    {
			m_event = new ManualResetEventSlim(initialState);
        m_eventState = initialState ? EVENT_SET : EVENT_NOT_SET;

        m_worker = workerThread==null?Thread.CurrentThread:workerThread;
    }

		public bool WaitOne(int ms)
    {
        verifyCaller();

        if (m_eventState == EVENT_SET && Interlocked.CompareExchange(
            ref m_eventState, EVENT_NOT_SET, EVENT_SET) == EVENT_SET)
        {
            return true;
        }

        if (m_eventState == EVENT_NOT_SET && Interlocked.CompareExchange(
            ref m_eventState, EVENT_ON_WAIT, EVENT_NOT_SET) == EVENT_NOT_SET)
        {
            return m_event.Wait(ms);
        }
			return false;
    }

    public void Set()
    {
        if (m_eventState == EVENT_NOT_SET && Interlocked.CompareExchange(
            ref m_eventState, EVENT_SET, EVENT_NOT_SET) == EVENT_NOT_SET)
        {
            return;
        }

        if (m_eventState == EVENT_ON_WAIT && Interlocked.CompareExchange(
            ref m_eventState, EVENT_NOT_SET, EVENT_ON_WAIT) == EVENT_ON_WAIT)
        {
            m_event.Set();
        }
    }

    // [Conditional("DEBUG")]
    private void verifyCaller()
    {
        if (m_worker != Thread.CurrentThread)
        {
            string errMsg = string.Format("Only the pre-defined Worker thread may call WaitOne (Current: {0}, Worker: {1})", Thread.CurrentThread, m_worker);

            throw new SynchronizationLockException(errMsg);
        }
    }
	}
}
