using Encoder.UI;

namespace Encoder.Filters.Video.Interpolate
{
   [Filter( "minterpolate" )]
   internal sealed class InterpolateVideoFilterViewModel : FilterViewModel
   {
      private double _targetFrameRate = 60;
      [FilterPropertyDescription( "Target Frame Rate", "fps" )]
      public double TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      private SceneChangeDetectionAlgorithm _sceneChangeDetectionAlgorithm = SceneChangeDetectionAlgorithm.FrameDifference;
      [FilterPropertyDescription( "Scene Change Algorithm", "scd" )]
      public SceneChangeDetectionAlgorithm SceneChangeDetectionAlgorithm
      {
         get => _sceneChangeDetectionAlgorithm;
         set => SetProperty( ref _sceneChangeDetectionAlgorithm, value );
      }

      private double _sceneChangeThreshold = 5.0;
      [FilterPropertyDescription( "Scene Change Threshold", "scd_threshold" )]
      [PropertyDependency( nameof( SceneChangeDetectionAlgorithm ), SceneChangeDetectionAlgorithm.FrameDifference )]
      public double SceneChangeThreshold
      {
         get => _sceneChangeThreshold;
         set => SetProperty( ref _sceneChangeThreshold, value );
      }

      private InterpolationMode _interpolationMode = InterpolationMode.MotionCompensated;
      [FilterPropertyDescription( "Interpolation Mode", "mi_mode" )]
      public InterpolationMode InterpolationMode
      {
         get => _interpolationMode;
         set => SetProperty( ref _interpolationMode, value );
      }

      private MotionCompensationMode _motionCompensationMode = MotionCompensationMode.Adaptive;
      [FilterPropertyDescription( "Motion Compensation", "mc_mode" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public MotionCompensationMode MotionCompensationMode
      {
         get => _motionCompensationMode;
         set => SetProperty( ref _motionCompensationMode, value );
      }

      private MotionEstimationMode _motionEstimationMode = MotionEstimationMode.Bidirectional;
      [FilterPropertyDescription( "Motion Estimation", "me_mode" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public MotionEstimationMode MotionEstimationMode
      {
         get => _motionEstimationMode;
         set => SetProperty( ref _motionEstimationMode, value );
      }

      private MotionEstimationAlgorithm _motionEstimationAlgorithm = MotionEstimationAlgorithm.Predictive;
      [FilterPropertyDescription( "Estimation Algorithm", "me" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public MotionEstimationAlgorithm MotionEstimationAlgorithm
      {
         get => _motionEstimationAlgorithm;
         set => SetProperty( ref _motionEstimationAlgorithm, value );
      }

      private int _macroblockSize = 16;
      [FilterPropertyDescription( "Macroblock Size", "mb_size" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public int MacroblockSize
      {
         get => _macroblockSize;
         set => SetProperty( ref _macroblockSize, value );
      }

      private int _searchParameter = 32;
      [FilterPropertyDescription( "Algorithm Parameter", "search_param" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public int SearchParameter
      {
         get => _searchParameter;
         set => SetProperty( ref _searchParameter, value );
      }

      private bool _variableSizeCompensation;
      [FilterPropertyDescription( "Variable Block Size", "vsbmc" )]
      [PropertyDependency( nameof( InterpolationMode ), InterpolationMode.MotionCompensated )]
      public bool VariableSizeCompensation
      {
         get => _variableSizeCompensation;
         set => SetProperty( ref _variableSizeCompensation, value );
      }
   }
}
