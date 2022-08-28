/*
*   MoveNet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Diagnostics;
    using UnityEngine;

    /// <summary>
    /// Implementation of the 1€ filter.
    /// https://hal.inria.fr/hal-00670496/document
    /// </summary>
    internal sealed class OneEuroFilter {

        #region --Client API--

        public OneEuroFilter (float cutoffFrequency, float beta, float derivativeCutoff) {
            this.cutoffFrequency = cutoffFrequency;
            this.beta = beta;
            this.derivativeCutoff = derivativeCutoff;
            this.watch = new Stopwatch();
        }

        public float[] Filter (float[] x) {
            // Shortcut
            if (xPrev == null) {
                xPrev = x;
                dxPrev = new float[x.Length];
                watch.Start();
                tPrev = (float)watch.Elapsed.TotalSeconds;
                return x;
            }
            // Compute factors
            var t = (float)watch.Elapsed.TotalSeconds;
            var deltaTime = t - tPrev;
            var alphaD = ComputeAlpha(deltaTime, derivativeCutoff);
            for (var i = 0; i < x.Length; i++) {
                // Filter first derivative
                var dx = (x[i] - xPrev[i]) / deltaTime;
                dxPrev[i] = Mathf.Lerp(dxPrev[i], dx, alphaD);
                // Filter signal
                var cutoff = Mathf.Abs(dxPrev[i]) * beta + cutoffFrequency;
                var alpha = ComputeAlpha(deltaTime, cutoff);
                xPrev[i] = Mathf.Lerp(xPrev[i], x[i], alpha);
            }
            tPrev = t;
            return xPrev;
        }
        #endregion


        #region --Operations--
        public readonly float cutoffFrequency;
        public readonly float beta;
        public readonly float derivativeCutoff;

        private readonly Stopwatch watch;
        private float[] xPrev, dxPrev;
        private float tPrev;

        private static float ComputeAlpha (float deltaTime, float cutoffFrequency) {
            var r = 2f * Mathf.PI * cutoffFrequency * deltaTime;
            return r / (r + 1f);
        }
        #endregion
    }
}