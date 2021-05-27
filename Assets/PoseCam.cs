/* 
*   Pose Cam
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using NatSuite.Devices;
    using NatSuite.ML;
    using NatSuite.ML.Features;
    using NatSuite.ML.Vision;
    using NatSuite.ML.Visualizers;
    using NatSuite.ML.Extensions;

    public class PoseCam : MonoBehaviour {

        public MLBodyPoseVisualizer visualizer;

        private MLModelData modelData;
        private MLModel model;
        private MLAsyncPredictor<MoveNetPredictor.Pose> predictor;
        private CameraDevice cameraDevice;
        private Texture2D preview;

        async void Start () {
            // Request camera permissions
            if (!await MediaDeviceQuery.RequestPermissions<CameraDevice>()) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            preview = await cameraDevice.StartRunning();
            Debug.Log("Started camera preview");
            // Display preview
            visualizer.Render(preview, null);
            // Create predictor
            modelData = await MLModelData.FromHub("@natsuite/movenet-lightning");
            model = modelData.Deserialize();
            predictor = new MoveNetPredictor(model).ToAsync();
            Debug.Log("Created predictor");
        }

        async void Update () {
            // Check that the camera is running
            if (!preview)
                return;
            // Check that the predictor has been loaded
            if (predictor == null)
                return;
            // Check that the predictor is ready for more predictions
            if (!predictor.readyForPrediction)
                return;
            // Detect
            var input = new MLImageFeature(preview);
            (input.mean, input.std) = modelData.normalization;
            var pose = await predictor.Predict(input);
            // Visualize
            visualizer.Render(preview, pose);
        }

        void OnDisable () {
            // Stop preview
            if (cameraDevice?.running ?? false)
                cameraDevice.StopRunning();
            // Dispose model
            model?.Dispose();
            predictor?.Dispose();
        }
    }
}