/* 
*   MoveNet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MoveNet body pose predictor.
    /// </summary>
    public sealed partial class MoveNetPredictor : IMLPredictor<MoveNetPredictor.Pose> {

        #region --Client API--
        /// <summary>
        /// Create the MoveNet predictor.
        /// </summary>
        /// <param name="model">MoveNet ML model.</param>
        /// <param name="smoothing">Apply smoothing filter to detected points.</param>
        public MoveNetPredictor (MLModel model, bool smoothing = true) {
            this.model = model as MLEdgeModel;
            this.filter = smoothing ? new OneEuroFilter(0.5f, 3f, 1f) : null;
        }

        /// <summary>
        /// Detect the body pose in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected body pose.</returns>
        public Pose Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"MoveNet predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"MoveNet predictor expects an an array or image feature", nameof(inputs));     
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
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly OneEuroFilter filter;

        void IDisposable.Dispose () { } // Not used
        #endregion
    }
}