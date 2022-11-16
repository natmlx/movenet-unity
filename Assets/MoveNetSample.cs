/* 
*   MoveNet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML;
    using NatML.VideoKit;
    using NatML.Vision;
    using Visualizers;

    public class MoveNetSample : MonoBehaviour {

        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public MoveNetVisualizer visualizer;

        private MLModelData modelData;
        private MLModel model;
        private MoveNetPredictor predictor;

        private async void Start () {
            // Fetch the MoveNet model data
            modelData = await MLModelData.FromHub("@natsuite/movenet");
            // Create the model
            model = new MLEdgeModel(modelData);
            // Create the MoveNet predictor
            predictor = new MoveNetPredictor(model);
            // Listen for camera frames
            cameraManager.OnFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var feature = frame.feature;
            (feature.mean, feature.std) = modelData.normalization;
            var pose = predictor.Predict(feature);
            // Visualize
            visualizer.Render(pose);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnFrame.RemoveListener(OnCameraFrame);
            // Dispose the model
            model?.Dispose();
        }
    }
}