using System.ComponentModel;

namespace Encoder.Filters.Video.Interpolate
{
   internal enum InterpolationMode
   {
      [FilterEnumValue( "dup" )]
      Duplicate,

      [FilterEnumValue( "blend" )]
      Blend,

      [FilterEnumValue( "Motion Compensated", "mci" )]
      MotionCompensated
   }

   internal enum MotionCompensationMode
   {
      [FilterEnumValue( "Overlapped Block", "obmc" )]
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

      [FilterEnumValue( "Two Dimensional", "tdls" )]
      TwoDimensional,

      [FilterEnumValue( "New Three Step", "ntss" )]
      NewThreeStep,

      [FilterEnumValue( "Four Step", "fss" )]
      FourStep,

      [FilterEnumValue( "ds" )]
      Diamond,

      [FilterEnumValue( "hexbs" )]
      Hexagon,

      [FilterEnumValue( "epzs" )]
      Predictive,

      [FilterEnumValue( "Uneven Multi-Hexagon", "umh" )]
      UnevenMultiHexagon
   }

   internal enum SceneChangeDetectionAlgorithm
   {
      [FilterEnumValue( "none" )]
      None,

      [FilterEnumValue( "Frame Difference", "fdiff" )]
      FrameDifference
   }
}
