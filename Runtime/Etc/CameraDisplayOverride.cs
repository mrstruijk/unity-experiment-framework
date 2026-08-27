using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UXF.EditorUtils
{
    /// <summary>
    /// Class which handles the cascading settings system.
    /// Only works in Default Render Pipeline, not in URP / HDRP
    /// </summary>
    public class CameraDisplayOverride : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;

        void OnValidate()
        {
            bool isUsingDefaultRenderPipeline = GraphicsSettings.defaultRenderPipeline == null;

            if (m_camera == null && isUsingDefaultRenderPipeline)
            {
                m_camera = GetComponent<Camera>();
                m_camera.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }
    }
}
