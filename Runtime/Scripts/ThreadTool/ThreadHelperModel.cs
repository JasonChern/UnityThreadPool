using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.FunJimChee.ThreadTool
{
    public class ThreadHelperModel
    {
        public event Action<ThreadTaskMessage> UpdateEvent;

        public event Action FinishEvent;

        private readonly Queue<Action> _tasks = new Queue<Action>();

        private readonly object _objectLock = new object();

        private int _treadCount = 0;
        
        public async void Run(int threadCount = 10)
        {
            var loopCount = _tasks.Count > threadCount ? threadCount : _tasks.Count;

            UnityCrossThreadManager.Instance.AddTask(()=> Debug.Log("ThreadHelperModel => Start"));

            for (var i = 0; i < loopCount; i++)
            {
                RunTask(_tasks.Dequeue());
            }

            await Task.Run(() =>
            {
                while (_tasks.Count > 0)
                {
                    Task.Delay(100).Wait();

                    var runCount = loopCount - _treadCount;
                    
                    if (runCount > 0)
                    {
                        for (var i = 0; i < runCount; i++)
                        {
                            RunTask(_tasks.Dequeue());
                        }
                    }
                }

                while (_treadCount > 0)
                {
                    Task.Delay(100).Wait();
                }
            
                UnityCrossThreadManager.Instance.AddTask(() => FinishEvent?.Invoke());
            });
            
            UnityCrossThreadManager.Instance.AddTask(()=> Debug.Log("ThreadHelperModel => End!"));
        }

        private void RunTask(Action task)
        {
            _treadCount++;
            
            Task.Run(() =>
            {
                var handle = new ThreadTaskMessage();

                try
                {
                    task?.Invoke();

                    handle.Result = true;

                    handle.Message = "Success";
                }
                catch (Exception e)
                {
                    handle.Result = false;

                    handle.Message = e.Message;
                }
                
                lock (_objectLock)
                {
                    UnityCrossThreadManager.Instance.AddTask(() => UpdateEvent?.Invoke(handle));
                    
                    _treadCount--;
                }
            });
        }

        public void AddTask(Action task)
        {
            _tasks.Enqueue(task);
        }
    }
}