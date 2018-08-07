using System.ComponentModel;

namespace Interpolator.Filters.Interpolate
{
   internal enum InterpolationMode
   {
      [FilterParameterValue( "dup" )]
      Duplicate,

      [FilterParameterValue( "blend" )]
      Blend,

      [FilterParameterValue( "mci" )]
      MotionCompensated
   }

   internal enum MotionCompensationMode
   {
      [FilterParameterValue( "obmc" )]
      OverlappedBlock,

      [FilterParameterValue( "aobmc" )]
      Adaptive
   }

   internal enum MotionEstimationMode
   {
      [FilterParameterValue( "bidir" )]
      Bidirectional,

      [FilterParameterValue( "bilat" )]
      Bilateral
   }

   internal enum MotionEstimationAlgorithm
   {
      [FilterParameterValue( "esa" )]
      Exhaustive,

      [FilterParameterValue( "tss" )]
      ThreeStep,

      [FilterParameterValue( "tdls" )]
      TwoDimensional,

      [FilterParameterValue( "ntss" )]
      NewThreeStep,

      [FilterParameterValue( "fss" )]
      FourStep,

      [FilterParameterValue( "ds" )]
      Diamond,

      [FilterParameterValue( "hexbs" )]
      Hexagon,

      [FilterParameterValue( "epzs" )]
      Predictive,

      [FilterParameterValue( "umh" )]
      UnevenMultiHexagon
   }

   internal enum SceneChangeDetectionAlgorithm
   {
      [FilterParameterValue( "none" )]
      None,

      [FilterParameterValue( "fdiff" )]
      FrameDifference
   }
}
