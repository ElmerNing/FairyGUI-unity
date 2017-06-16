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

		EventCallback0 _callback0;
		EventCallback1 _callback1;
		EventCallback1 _captureCallback;
		internal bool _dispatching;

		public EventBridge(EventDispatcher owner)
		{
			this.owner = owner;
		}

		public void AddCapture(EventCallback1 callback)
		{
			_captureCallback -= callback;
			_captureCallback += callback;
		}

		public void RemoveCapture(EventCallback1 callback)
		{
			_captureCallback -= callback;
		}

		public void Add(EventCallback1 callback)
		{
			_callback1 -= callback;
			_callback1 += callback;
		}

		public void Remove(EventCallback1 callback)
		{
			_callback1 -= callback;

			LuaFramework.LuaDelegateManager ld = AppFacade.Instance.GetManager<LuaFramework.LuaDelegateManager>(LuaFramework.ManagerName.LuaDelegateManager);
			ld.ClearDelegate(callback);
		}

		public void Add(EventCallback0 callback)
		{
			_callback0 -= callback;
			_callback0 += callback;
		}

		public void Remove(EventCallback0 callback)
		{
			_callback0 -= callback;

			LuaFramework.LuaDelegateManager ld = AppFacade.Instance.GetManager<LuaFramework.LuaDelegateManager>(LuaFramework.ManagerName.LuaDelegateManager);
			ld.ClearDelegate(callback);
		}

		public bool isEmpty
		{
			get { return _callback1 == null && _callback0 == null && _captureCallback == null; }
		}


		public void Clear()
		{

			//只清理 注册的lua代理    
			LuaFramework.LuaDelegateManager ld = AppFacade.Instance.GetManager<LuaFramework.LuaDelegateManager>(LuaFramework.ManagerName.LuaDelegateManager);
			ld.ClearDelegate(_callback1);
			ld.ClearDelegate(_callback0);
			ld.ClearDelegate(_captureCallback);

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
