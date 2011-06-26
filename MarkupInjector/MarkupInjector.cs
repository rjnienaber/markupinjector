using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MarkupInjector
{
    public static class InjectMarkup
    {
        public static void InHead(Func<string> markupFunction)
        {
            Settings.HeadListeners.Add(markupFunction);
        }

        public static void InBody(Func<string> markupFunction)
        {
            Settings.BodyListeners.Add(markupFunction);
        }
    }
}
