namespace Encoder.Filters.Video.Interpolate
{
   [Filter( "minterpolate" )]
   internal sealed class InterpolateVideoFilterViewModel : VideoFilterViewModel
   {
      private double _targetFrameRate = 60;
      [FilterParameter( "Target Frame Rate", "fps" )]
      public double TargetFrameRate
      {
         get => _targetFrameRate;
         set => SetProperty( ref _targetFrameRate, value );
      }

      private SceneChangeDetectionAlgorithm _sceneChangeDetectionAlgorithm = SceneChangeDetectionAlgorithm.FrameDifference;
      [FilterParameter( "Scene Change Algorithm", "scd" )]
      public SceneChangeDetectionAlgorithm SceneChangeDetectionAlgorithm
      {
         get => _sceneChangeDetectionAlgorithm;
         set => SetProperty( ref _sceneChangeDetectionAlgorithm, value );
      }

      private double _sceneChangeThreshold = 5.0;
      [FilterParameter( "Scene Change Threshold", "scd_threshold", nameof( SceneChangeDetectionAlgorithm ), Interpolate.SceneChangeDetectionAlgorithm.FrameDifference )]
      public double SceneChangeThreshold
      {
         get => _sceneChangeThreshold;
         set => SetProperty( ref _sceneChangeThreshold, value );
      }

      private InterpolationMode _interpolationMode = InterpolationMode.MotionCompensated;
      [FilterParameter( "Interpolation Mode", "mi_mode" )]
      public InterpolationMode InterpolationMode
      {
         get => _interpolationMode;
         set => SetProperty( ref _interpolationMode, value );
      }

      private MotionCompensationMode _motionCompensationMode = MotionCompensationMode.Adaptive;
      [FilterParameter( "Motion Compensation", "mc_mode", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public MotionCompensationMode MotionCompensationMode
      {
         get => _motionCompensationMode;
         set => SetProperty( ref _motionCompensationMode, value );
      }

      private MotionEstimationMode _motionEstimationMode = MotionEstimationMode.Bidirectional;
      [FilterParameter( "Motion Estimation", "me_mode", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public MotionEstimationMode MotionEstimationMode
      {
         get => _motionEstimationMode;
         set => SetProperty( ref _motionEstimationMode, value );
      }

      private MotionEstimationAlgorithm _motionEstimationAlgorithm = MotionEstimationAlgorithm.Predictive;
      [FilterParameter( "Estimation Algorithm", "me", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public MotionEstimationAlgorithm MotionEstimationAlgorithm
      {
         get => _motionEstimationAlgorithm;
         set => SetProperty( ref _motionEstimationAlgorithm, value );
      }

      private int _macroblockSize = 16;
      [FilterParameter( "Macroblock Size", "mb_size", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public int MacroblockSize
      {
         get => _macroblockSize;
         set => SetProperty( ref _macroblockSize, value );
      }

      private int _searchParameter = 32;
      [FilterParameter( "Algorithm Parameter", "search_param", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public int SearchParameter
      {
         get => _searchParameter;
         set => SetProperty( ref _searchParameter, value );
      }

      private bool _variableSizeCompensation;
      [FilterParameter( "Variable Block Size", "vsbmc", nameof( InterpolationMode ), Interpolate.InterpolationMode.MotionCompensated )]
      public bool VariableSizeCompensation
      {
         get => _variableSizeCompensation;
         set => SetProperty( ref _variableSizeCompensation, value );
      }
   }
}
