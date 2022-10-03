/* 
*   MoveNet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Features;
    using NatML.Vision;
    using Visualizers;

    public class MoveNetSample : MonoBehaviour {

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
        public MoveNetVisualizer visualizer;

        private CameraDevice cameraDevice;
        private TextureOutput previewTextureOutput;

        private MLModelData modelData;
        private MLModel model;
        private MoveNetPredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            // Display the preview texture
            var previewTexture = await previewTextureOutput;
            visualizer.image = previewTexture;
            // Create the MoveNet predictor
            modelData = await MLModelData.FromHub("@natsuite/movenet");
            model = modelData.Deserialize();
            predictor = new MoveNetPredictor(model);
        }

        void Update () {
            // Check that the predictor has been created
            if (predictor == null)
                return;
            // Create the image feature
            var imageFeature = new MLImageFeature(previewTextureOutput.texture);
            (imageFeature.mean, imageFeature.std) = modelData.normalization;
            // Detect
            var pose = predictor.Predict(imageFeature);
            // Visualize
            visualizer.Render(pose);
        }

        void OnDisable () {
            // Dispose model
            model?.Dispose();
        }
    }
}