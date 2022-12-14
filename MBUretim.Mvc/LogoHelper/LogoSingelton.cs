using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MBUretim.Mvc.LogoHelper
{
    public class LogoSingelton
    {
        private static UnityObjects.UnityApplication _unityInstance;
        public static UnityObjects.UnityApplication UnityApp
        {
            get
            {
                if (_unityInstance == null)
                {
                    _unityInstance = new UnityObjects.UnityApplication();
                }
                return _unityInstance;
            }
        }
    }
}