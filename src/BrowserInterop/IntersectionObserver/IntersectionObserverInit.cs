using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace BrowserInterop.IntersectionObserver
{
    public class IntersectionObserverInit
    {
        /// <summary>
        /// An Element or Document object which is an ancestor of the intended target, whose bounding rectangle will be considered the viewport. Any part of the target not visible in the visible area of the root is not considered visible. If not specified, the observer uses the document's viewport as the root, with no margin, and a 0% threshold (meaning that even a one-pixel change is enough to trigger a callback).
        /// </summary>
        public ElementReference Root { get; set; }
        /// <summary>
        /// A string which specifies a set of offsets to add to the root's bounding_box when calculating intersections, effectively shrinking or growing the root for calculation purposes. The syntax is approximately the same as that for the CSS margin property; see The intersection root and root margin for more information on how the margin works and the syntax. The default is "0px 0px 0px 0px".
        /// </summary>
        public string RootMargin { get; set; } = "0px 0px 0px 0px";
        /// <summary>
        /// Either a single number or an array of numbers between 0.0 and 1.0, specifying a ratio of intersection area to total bounding box area for the observed target. A value of 0.0 means that even a single visible pixel counts as the target being visible. 1.0 means that the entire target element is visible. See Thresholds for a more in-depth description of how thresholds are used. The default is a threshold of 0.0.
        /// </summary>
        public float[] Threshold { get; set; } = [ 0 ];
    }
}
