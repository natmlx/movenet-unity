# MoveNet
[MoveNet](https://blog.tensorflow.org/2021/05/next-generation-pose-detection-with-movenet-and-tensorflowjs.html) single body pose detection from Google MediaPipe.

## Installing MoveNet
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.movenet": "1.0.6"
  }
}
```

## Predicting Pose in an Image
First, create the MoveNet predictor:
```csharp
// Create the MoveNet predictor
var predictor = await MoveNetPredictor.Create();
```

Then detect the body pose in an image:
```csharp
// Given an image
Texture2D image = ...;
// Detect the body pose
MoveNetPredictor.Pose pose = predictor.Predict(image);
```
___

## Requirements
- Unity 2021.2+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://natml.ai/community).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!