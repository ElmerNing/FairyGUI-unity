using System;
using System.Collections.Generic;
using FairyGUI.Utils;
using LuaInterface;

namespace FairyGUI
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class LuaUIHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="luaClass"></param>
		public static void SetExtension(string url, System.Type baseType, LuaFunction extendFunction)
		{
			UIObjectFactory.SetPackageItemExtension(url, () => {
				GComponent gcom = (GComponent)Activator.CreateInstance(baseType);
				gcom.data = extendFunction;
				return gcom;
			});
		}

        static public Action<GComponent, string> gcomponentCreator;
        public static void SetDefaultExtension(LuaFunction extendFunction)
        {
            gcomponentCreator = ((gcom, name) =>
            {
                extendFunction.BeginPCall();
                extendFunction.Push(gcom);
                if (name != null && name != "")
                {
                    extendFunction.Push(name);
                }
                extendFunction.PCall();
                LuaTable _peerTable = extendFunction.CheckLuaTable();
                extendFunction.EndPCall();
                gcom.SetLuaPeer(_peerTable);
            });
        }

		
	}
   
}

