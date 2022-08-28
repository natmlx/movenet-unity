/*
*   MoveNet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed partial class MoveNetPredictor {

        /// <summary>
        /// Detected body pose.
        /// The xy coordinates are the normalized position of the keypoint, in range [0, 1].
        /// The z coordinate is the confidence score of the keypoint, in range [0, 1].
        /// </summary>
        public readonly struct Pose : IReadOnlyList<Vector3> {

            #region --Client API--
            /// <summary>
            /// Number of keypoints in the pose.
            /// </summary>
            public readonly int Count               => data.Length / 3;

            /// <summary>
            /// Nose position.
            /// </summary>
            public readonly Vector3 nose            => this[0];

            /// <summary>
            /// Left eye position.
            /// </summary>
            public readonly Vector3 leftEye         => this[1];

            /// <summary>
            /// Right eye position.
            /// </summary>
            public readonly Vector3 rightEye        => this[2];

            /// <summary>
            /// Left ear position.
            /// </summary>
            public readonly Vector3 leftEar         => this[3];

            /// <summary>
            /// Right ear position.
            /// </summary>
            public readonly Vector3 rightEar        => this[4];

            /// <summary>
            /// Left shoulder position.
            /// </summary>
            public readonly Vector3 leftShoulder    => this[5];

            /// <summary>
            /// Right shoulder position.
            /// </summary>
            public readonly Vector3 rightShoulder   => this[6];

            /// <summary>
            /// Left elbow position.
            /// </summary>
            public readonly Vector3 leftElbow       => this[7];

            /// <summary>
            /// Right elbow position.
            /// </summary>
            public readonly Vector3 rightElbow      => this[8];

            /// <summary>
            /// Left wrist position.
            /// </summary>
            public readonly Vector3 leftWrist       => this[9];

            /// <summary>
            /// Right wrist position.
            /// </summary>
            public readonly Vector3 rightWrist      => this[10];

            /// <summary>
            /// Left hip position.
            /// </summary>
            public readonly Vector3 leftHip         => this[11];

            /// <summary>
            /// Right hip position.
            /// </summary>
            public readonly Vector3 rightHip        => this[12];

            /// <summary>
            /// Left knee position.
            /// </summary>
            public readonly Vector3 leftKnee        => this[13];

            /// <summary>
            /// Right knee position.
            /// </summary>
            public readonly Vector3 rightKnee       => this[14];

            /// <summary>
            /// Left ankle position.
            /// </summary>
            public readonly Vector3 leftAnkle       => this[15];

            /// <summary>
            /// Right ankle position.
            /// </summary>
            public readonly Vector3 rightAnkle      => this[16];

            /// <summary>
            /// Get a pose keypoint by index.
            /// </summary>
            /// <param name="idx">Keypoint index. Must be in range [0, 16].</param>
            public readonly Vector3 this [int idx] {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => new Vector3(data[3 * idx + 1], 1f - data[3 * idx + 0], data[3 * idx + 2]);
            }
            #endregion


            #region --Operations--
            private readonly float[] data;

            internal Pose (float[] data) => this.data = data;

            readonly IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator () {
                for (var i = 0; i < Count; ++i)
                    yield return this[i];
            }

            readonly IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector3>).GetEnumerator();
            #endregion
        }
    }
}