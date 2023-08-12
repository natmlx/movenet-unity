/*
*   MoveNet
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using NatML.Vision;
    using VideoKit.UI;

    /// <summary>
    /// MoveNet body pose visualizer.
    /// This visualizer uses visualizes the pose keypoints using a UI image.
    /// </summary>
    [RequireComponent(typeof(VideoKitCameraView))]
    public sealed class MoveNetVisualizer : MonoBehaviour {

        #region --Inspector--
        [SerializeField]
        public RectTransform keypoint;
        #endregion


        #region --Client API--
        /// <summary>
        /// Render a body pose.
        /// </summary>
        /// <param name="pose">Body pose to render.</param>
        /// <param name="confidenceThreshold">Keypoints with confidence lower than this value are not rendered.</param>
        public void Render (MoveNetPredictor.Pose pose) {
            // Delete current
            foreach (var point in currentPoints)
                GameObject.Destroy(point.gameObject);
            currentPoints.Clear();            
            // Render keypoints
            var imageTransform = transform as RectTransform;
            foreach (var point in pose) {
                // Instantiate
                var anchor = Instantiate(keypoint, transform);
                anchor.gameObject.SetActive(true);
                // Position
                anchor.anchorMin = 0.5f * Vector2.one;
                anchor.anchorMax = 0.5f * Vector2.one;
                anchor.pivot = 0.5f * Vector2.one;
                anchor.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
                // Add
                currentPoints.Add(anchor);
            }
        }
        #endregion


        #region --Operations--
        private readonly List<RectTransform> currentPoints = new List<RectTransform>();
        #endregion
    }
}