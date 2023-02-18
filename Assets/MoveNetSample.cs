/* 
*   MoveNet
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.VideoKit;
    using NatML.Vision;
    using Visualizers;

    public class MoveNetSample : MonoBehaviour {

        #region --Inspector--
        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public MoveNetVisualizer visualizer;
        #endregion
        

        #region --Operations--
        private MoveNetPredictor predictor;

        private async void Start () {
            // Create the MoveNet predictor
            predictor = await MoveNetPredictor.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict            
            var pose = predictor.Predict(frame);
            // Visualize
            visualizer.Render(pose);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the model
            predictor?.Dispose();
        }
        #endregion
    }
}