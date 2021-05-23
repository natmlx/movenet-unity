/* 
*   Pose Cam
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using NatSuite.Devices;
    using NatSuite.ML;
    using NatSuite.ML.Vision;
    using NatSuite.ML.Visualizers;
    using NatSuite.ML.Extensions;

    public class PoseCam : MonoBehaviour {

        [Header("ML")]
        public MLModelData modelData;

        [Header("UI")]
        public MLBodyPoseVisualizer visualizer;

        private MLModel model;
        private MLAsyncPredictor<MLBodyPose> predictor;
        private CameraDevice cameraDevice;
        private Texture2D preview;

        async void Start () {
            // Request camera permissions
            if (!await MediaDeviceQuery.RequestPermissions<CameraDevice>()) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Create predictor
            model = modelData.Deserialize();
            predictor = new MoveNetPredictor(model).ToAsync();
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            preview = await cameraDevice.StartRunning();
            // Display preview
            visualizer.Render(preview, null);
        }

        async void Update () {
            // Check that the camera is running
            if (!preview)
                return;
            // Check that the predictor is ready for more predictions
            if (!predictor.readyForPrediction)
                return;
            // Detect
            var pose = await predictor.Predict(preview);
            // Visualize
            visualizer.Render(preview, pose);
        }

        void OnDisable () {
            // Stop preview
            if (cameraDevice?.running ?? false)
                cameraDevice.StopRunning();
            // Dispose model
            model?.Dispose();
        }
    }
}