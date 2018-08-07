namespace Interpolator.Filters.Interpolate
{
   internal enum InterpolationMode
   {
      Duplicate,
      Blend,
      MotionCompensated
   }

   internal enum MotionCompensationMode
   {
      OverlappedBlock,
      Adaptive
   }

   internal enum MotionEstimationMode
   {
      Bidirectional,
      Bilateral
   }

   internal enum MotionEstimationAlgorithm
   {
      Exhaustive,
      ThreeStep,
      TwoDimensional,
      NewThreeStep,
      FourStep,
      Diamond,
      Hexagon,
      Predictive,
      UnevenMultiHexagon
   }

   internal enum SceneChangeDetectionAlgorithm
   {
      None,
      FrameDifference
   }
}
