using System.Collections.Generic;
using UnityEngine;

namespace UXF
{
    /// <summary>
    /// Attach this component to any gameobject and assign it in the trackedObjects field in an ExperimentSession to automatically record the position of the mouse in screen coordinates. 
    /// Note (0, 0) is the bottom-left of the window displaying the game.
    /// </summary>
    public class MouseScreenTracker : Tracker
    {
        public override string MeasurementDescriptor => "mouse_screen";
        public override IEnumerable<string> CustomHeader => new string[] { "pix_x", "pix_y" };

        /// <summary>
        /// Returns current mouse position in screen coordinates
        /// </summary>
        /// <returns></returns>
        protected override void GetCurrentValues(UXFDataRow row)
        {
            Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            row.Add(("pix_x", p.x));
            row.Add(("pix_y", p.y));
        }
    }
}