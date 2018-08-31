using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FairyGUI
{
	/// <summary>
	/// 
	/// </summary>
	class EventBridge
	{
		public EventDispatcher owner;

		public EventCallback0 _callback0;
		public EventCallback1 _callback1;
		public EventCallback1 _captureCallback;
		internal bool _dispatching;

        List<System.Delegate> luaDelegateSet = new List<System.Delegate>();

		public EventBridge(EventDispatcher owner)
		{
			this.owner = owner;
		}

		public void AddCapture(EventCallback1 callback)
		{
            var luad = callback.Target as LuaInterface.LuaDelegate;
            if (luad != null)
            {
                if (!luaDelegateSet.Contains(callback))
                {
                    luaDelegateSet.Add(callback);
                }
            }
            _captureCallback -= callback;
			_captureCallback += callback;
		}

		public void RemoveCapture(EventCallback1 callback)
		{
			_captureCallback -= callback;
		}

		public void Add(EventCallback1 callback)
		{
            AddLuaD(callback);
            
            _callback1 -= callback;
			_callback1 += callback;

            
		}

		public void Remove(EventCallback1 callback)
		{

            RemoveLuaD(callback);

            _callback1 -= callback;

 
        }


        private void AddLuaD(System.Delegate callback)
        {
            var luad = callback.Target as LuaInterface.LuaDelegate;
            if (luad != null)
            {
                if (!luaDelegateSet.Contains(callback))
                {
                    luaDelegateSet.Add(callback);
                }
            }
        }

        private void RemoveLuaD(System.Delegate callback)
        {
            var luad = callback.Target as LuaInterface.LuaDelegate;
            if (luad != null)
            {
                if (luaDelegateSet.Contains(callback))
                {
                    luaDelegateSet.Remove(callback);
                    LuaFramework.LuaDelegateManager ld = AppFacade.Instance.GetManager<LuaFramework.LuaDelegateManager>();
                    ld.ClearDelegate(callback);
                }
            }
        }

        private void ClearLuaD()
        {
            LuaFramework.LuaDelegateManager ld = AppFacade.Instance.GetManager<LuaFramework.LuaDelegateManager>();
            foreach (var item in luaDelegateSet)
            {
                ld.ClearDelegate(item);
            }
            luaDelegateSet.Clear();
        }

		public void Add(EventCallback0 callback)
		{
            AddLuaD(callback);
            _callback0 -= callback;
			_callback0 += callback;
		}

		public void Remove(EventCallback0 callback)
		{
            _callback0 -= callback;
            RemoveLuaD(callback);

        }

		public bool isEmpty
		{
			get { return _callback1 == null && _callback0 == null && _captureCallback == null; }
		}


		public void Clear()
		{

            //只清理 注册的lua代理    
            ClearLuaD();

			_callback1 = null;
			_callback0 = null;
			_captureCallback = null;
		}

		public void CallInternal(EventContext context)
		{
			_dispatching = true;
			context.sender = owner;
			try
			{
				if (_callback1 != null)
					_callback1(context);
				if (_callback0 != null)
					_callback0();
			}
			finally
			{
				_dispatching = false;
			}
		}

		public void CallCaptureInternal(EventContext context)
		{
			if (_captureCallback == null)
				return;

			_dispatching = true;
			context.sender = owner;
			try
			{
				_captureCallback(context);
			}
			finally
			{
				_dispatching = false;
			}
		}
	}
}
