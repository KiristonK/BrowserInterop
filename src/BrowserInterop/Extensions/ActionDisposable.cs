using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserInterop.Extensions
{
    internal class ActionDisposable(Action todoOnDispose) : IDisposable
    {
        public void Dispose()
        {
            todoOnDispose();
        }
    }
}
