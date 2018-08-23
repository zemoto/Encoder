using System.ComponentModel;

namespace Encoder.Filters.Interpolate
{
   internal enum InterpolationMode
   {
      [FilterParameterValue( "dup" )]
      Duplicate,

      [FilterParameterValue( "blend" )]
      Blend,

      [FilterParameterValue( "mci" )]
      [Description( "Motion Compensated" )]
      MotionCompensated
   }

   internal enum MotionCompensationMode
   {
      [FilterParameterValue( "obmc" )]
      [Description( "Overlapped Block" )]
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
      [Description( "Three Step" )]
      ThreeStep,

      [FilterParameterValue( "tdls" )]
      [Description( "Two Dimensional" )]
      TwoDimensional,

      [FilterParameterValue( "ntss" )]
      [Description( "New Three Step" )]
      NewThreeStep,

      [FilterParameterValue( "fss" )]
      [Description( "Four Step" )]
      FourStep,

      [FilterParameterValue( "ds" )]
      Diamond,

      [FilterParameterValue( "hexbs" )]
      Hexagon,

      [FilterParameterValue( "epzs" )]
      Predictive,

      [FilterParameterValue( "umh" )]
      [Description( "Uneven Multi-Hexagon" )]
      UnevenMultiHexagon
   }

   internal enum SceneChangeDetectionAlgorithm
   {
      [FilterParameterValue( "none" )]
      None,

      [FilterParameterValue( "fdiff" )]
      [Description( "Frame Difference" )]
      FrameDifference
   }
}
