namespace Encoder.Filters.Video.Interpolate
{
   [Filter( "minterpolate", typeof( InterpolateVideoFilterControl ) )]
   internal sealed class InterpolateVideoFilterViewModel : VideoFilterViewModel
   {
      private double _targetFrameRate = 60;
      [FilterParameter( "fps" )]
      public double TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      private InterpolationMode _interpolationMode = InterpolationMode.MotionCompensated;
      [FilterParameter( "mi_mode" )]
      public InterpolationMode InterpolationMode
      {
         get => _interpolationMode;
         set => SetProperty( ref _interpolationMode, value );
      }

      private MotionCompensationMode _motionCompensationMode = MotionCompensationMode.Adaptive;
      [FilterParameter( "mc_mode" )]
      public MotionCompensationMode MotionCompensationMode
      {
         get => _motionCompensationMode;
         set => SetProperty( ref _motionCompensationMode, value );
      }

      private MotionEstimationMode _motionEstimationMode = MotionEstimationMode.Bidirectional;
      [FilterParameter( "me_mode" )]
      public MotionEstimationMode MotionEstimationMode
      {
         get => _motionEstimationMode;
         set => SetProperty( ref _motionEstimationMode, value );
      }

      private MotionEstimationAlgorithm _motionEstimationAlgorithm = MotionEstimationAlgorithm.Predictive;
      [FilterParameter( "me" )]
      public MotionEstimationAlgorithm MotionEstimationAlgorithm
      {
         get => _motionEstimationAlgorithm;
         set => SetProperty( ref _motionEstimationAlgorithm, value );
      }

      private int _macroblockSize = 16;
      [FilterParameter( "mb_size" )]
      public int MacroblockSize
      {
         get => _macroblockSize;
         set => SetProperty( ref _macroblockSize, value );
      }

      private int _searchParameter = 32;
      [FilterParameter( "search_param" )]
      public int SearchParameter
      {
         get => _searchParameter;
         set => SetProperty( ref _searchParameter, value );
      }

      private bool _variableSizeCompensation = false;
      [FilterParameter( "vsbmc" )]
      public bool VariableSizeCompensation
      {
         get => _variableSizeCompensation;
         set => SetProperty( ref _variableSizeCompensation, value );
      }

      private SceneChangeDetectionAlgorithm _sceneChangeDetectionAlgorithm = SceneChangeDetectionAlgorithm.FrameDifference;
      [FilterParameter( "scd" )]
      public SceneChangeDetectionAlgorithm SceneChangeDetectionAlgorithm
      {
         get => _sceneChangeDetectionAlgorithm;
         set => SetProperty( ref _sceneChangeDetectionAlgorithm, value );
      }

      private double _sceneChangeThreshold = 5.0;
      [FilterParameter( "scd_threshold" )]
      public double SceneChangeThreshold
      {
         get => _sceneChangeThreshold;
         set => SetProperty( ref _sceneChangeThreshold, value );
      }
   }
}
