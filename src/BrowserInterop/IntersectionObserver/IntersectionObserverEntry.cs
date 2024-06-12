using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BrowserInterop.Extensions;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BrowserInterop.IntersectionObserver
{
    public class IntersectionObserverEntry
    {
        public RectangleF BoundingClientRect { get; set; }  
        public float IntersectionRatio { get; set; }
        public RectangleF IntersectionRect {  get; set; }
        public bool IsIntersecting { get; set; }
        public RectangleF? RootBounds { get; set; }
        public IJSObjectReference Target { get; set; }
        public long Time { get; set; }
    }
}
