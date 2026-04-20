using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace UXF
{
    /// <summary>
    /// Attach this component to a gameobject and assign it in the trackedObjects field in an ExperimentSession to automatically record position/rotation of the object at each frame.
    /// </summary>
    public class PositionRotationTracker : Tracker
    {
        public override string MeasurementDescriptor => "movement";
        public override IEnumerable<string> CustomHeader => new string[] { "pos_x", "pos_y", "pos_z", "rot_x", "rot_y", "rot_z" };

        /// <summary>
        /// Returns current position and rotation values
        /// </summary>
        /// <returns></returns>
        protected override void GetCurrentValues(UXFDataRow row)
        {
            Vector3 p = gameObject.transform.position;
            Vector3 r = gameObject.transform.eulerAngles;

            row.Add(("pos_x", p.x));
            row.Add(("pos_y", p.y));
            row.Add(("pos_z", p.z));
            row.Add(("rot_x", r.x));
            row.Add(("rot_y", r.y));
            row.Add(("rot_z", r.z));
        }
    }
}