using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnityObjects;

namespace MBUretim.Mvc.Models
{
    public class GlobalParameters
    {
        private static UnityApplication _unityApp;
        public static UnityApplication UnityApp
        {
            get
            {
                return GlobalParameters._unityApp;
            }
            set
            {
                GlobalParameters._unityApp = value;
            }
        }
    }
}