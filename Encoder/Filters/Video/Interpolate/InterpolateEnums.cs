using System.ComponentModel;

namespace Encoder.Filters.Video.Interpolate
{
   internal enum InterpolationMode
   {
      [FilterEnumValue( "dup" )]
      Duplicate,

      [FilterEnumValue( "blend" )]
      Blend,

      [FilterEnumValue( "mci" )]
      [Description( "Motion Compensated" )]
      MotionCompensated
   }

   internal enum MotionCompensationMode
   {
      [FilterEnumValue( "obmc" )]
      [Description( "Overlapped Block" )]
      OverlappedBlock,

      [FilterEnumValue( "aobmc" )]
      Adaptive
   }

   internal enum MotionEstimationMode
   {
      [FilterEnumValue( "bidir" )]
      Bidirectional,

      [FilterEnumValue( "bilat" )]
      Bilateral
   }

   internal enum MotionEstimationAlgorithm
   {
      [FilterEnumValue( "esa" )]
      Exhaustive,

      [FilterEnumValue( "tss" )]
      [Description( "Three Step" )]
      ThreeStep,

      [FilterEnumValue( "tdls" )]
      [Description( "Two Dimensional" )]
      TwoDimensional,

      [FilterEnumValue( "ntss" )]
      [Description( "New Three Step" )]
      NewThreeStep,

      [FilterEnumValue( "fss" )]
      [Description( "Four Step" )]
      FourStep,

      [FilterEnumValue( "ds" )]
      Diamond,

      [FilterEnumValue( "hexbs" )]
      Hexagon,

      [FilterEnumValue( "epzs" )]
      Predictive,

      [FilterEnumValue( "umh" )]
      [Description( "Uneven Multi-Hexagon" )]
      UnevenMultiHexagon
   }

   internal enum SceneChangeDetectionAlgorithm
   {
      [FilterEnumValue( "none" )]
      None,

      [FilterEnumValue( "fdiff" )]
      [Description( "Frame Difference" )]
      FrameDifference
   }
}
