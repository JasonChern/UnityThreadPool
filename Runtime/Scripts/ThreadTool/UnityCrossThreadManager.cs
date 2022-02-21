using System;
using System.Collections.Generic;
using com.FunJimChee.CommonTool;

namespace com.FunJimChee.ThreadTool
{
    public class UnityCrossThreadManager : MonoSingletonBase<UnityCrossThreadManager>
    {
        private readonly Queue<Action> _actions = new Queue<Action>();

        private void Update()
        {
            if(_actions.Count == 0) return;
            
            _actions.Dequeue()?.Invoke();
        }

        public void AddTask(Action task)
        {
            _actions.Enqueue(task);
        }
    }
}