/* 
*   MoveNet
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Threading.Tasks;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MoveNet body pose predictor.
    /// </summary>
    public sealed partial class MoveNetPredictor : IMLPredictor<MoveNetPredictor.Pose> {

        #region --Client API--
        /// <summary>
        /// Predictor tag.
        /// </summary>
        public const string Tag = "@natsuite/movenet";

        /// <summary>
        /// Detect the body pose in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected body pose.</returns>
        public Pose Predict (params MLFeature[] inputs) {          
            // Preprocess
            var input = inputs[0];
            if (input is MLImageFeature image) {
                image.aspectMode = model.aspectMode;
                (image.mean, image.std) = model.normalization;
            }
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(model.inputs[0]);
            using var outputFeatures = model.Predict(inputFeature);
            // Filter
            var landmarks = new MLArrayFeature<float>(outputFeatures[0]).ToArray();
            landmarks = filter?.Filter(landmarks) ?? landmarks;
            // Return
            var pose = new Pose(landmarks);
            return pose;
        }

        /// <summary>
        /// Dispose the predictor and release resources.
        /// </summary>
        public void Dispose () => model.Dispose();

        /// <summary>
        /// Create the MoveNet predictor.
        /// </summary>
        /// <param name="smoothing">Apply smoothing filter to detected points.</param>
        /// <param name="configuration">Edge model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<MoveNetPredictor> Create (
            bool smoothing = true,
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var model = await MLEdgeModel.Create(Tag, configuration, accessKey);
            var filter = smoothing ? new OneEuroFilter(0.5f, 3f, 1f) : null;
            var predictor = new MoveNetPredictor(model, filter);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly OneEuroFilter filter;

        private MoveNetPredictor (MLEdgeModel model, OneEuroFilter filter) {
            this.model = model;
            this.filter = filter;
        }
        #endregion
    }
}