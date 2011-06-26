using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkupInjector
{
    public static class Settings
    {
        static Settings() {
            HeadListeners = new List<Func<string>>();
            BodyListeners = new List<Func<string>>();
        }

        public static List<Func<string>> HeadListeners { get; set; }
        public static List<Func<string>> BodyListeners { get; set; }

        public static bool ShouldInterceptResponse
        {
            get
            {
                return (HeadListeners.Count + BodyListeners.Count) != 0;
            }
        }
    }
}
