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

    public class PoseCam : MonoBehaviour {

        [Header(@"NatML Hub")]
        public string accessKey;

        [Header(@"UI")]
        public MoveNetVisualizer visualizer;

        private MLModelData modelData;
        private MLModel model;
        private MoveNetPredictor predictor;
        private CameraDevice cameraDevice;
        private Texture2D previewTexture;
        private byte[] pixelBuffer;

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
            previewTexture = await cameraDevice.StartRunning();
            pixelBuffer = previewTexture.GetRawTextureData<byte>().ToArray();
            // Display preview
            visualizer.Render(previewTexture, null);
            // Fetch the MoveNet model
            Debug.Log("Fetching model from NatML Hub");
            modelData = await MLModelData.FromHub("@natsuite/movenet", accessKey);
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the MoveNet predictor
            predictor = new MoveNetPredictor(model);
        }

        void Update () {
            // Check that the model has been loaded
            if (predictor == null)
                return;
            // Update the pixel buffer to avoid allocating memory
            previewTexture.GetRawTextureData<byte>().CopyTo(pixelBuffer);
            // Create the input feature
            var inputFeature = new MLImageFeature(pixelBuffer, previewTexture.width, previewTexture.height);
            (inputFeature.mean, inputFeature.std) = modelData.normalization;
            // Detect
            var pose = predictor.Predict(inputFeature);
            // Visualize
            visualizer.Render(previewTexture, pose);
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