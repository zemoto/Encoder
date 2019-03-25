using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Audio.Copy;
using Encoder.Filters.Video;
using Encoder.Filters.Video.Copy;
using Encoder.Operations;
using ZemotoCommon.UI;

namespace Encoder.AssemblyLineCreation
{
   internal sealed class AssemblyLineCreationViewModel : ViewModelBase
   {
      public IEnumerable<AssemblyLine> GetAssemblyLines()
      {
         var operation = GetOperation();
         var assemblyLines = SelectedFiles.SelectMany( x => operation.GetAssemblyLines( x ) ).ToList();
         
         SelectedFiles.Clear();

         return assemblyLines.Any( x => x == null ) ? null : assemblyLines;
      }

      private Operation GetOperation()
      {
         Operation operation;
         switch ( OperationType )
         {
            case OperationType.Filters:
               operation = new ApplyFiltersOperation( VideoFilter, AudioFilter );
               break;
            case OperationType.Separate:
               operation = new SeparateOperation();
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }

         return Async ? operation.ToAsync() : operation;
      }

      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();

      private OperationType _operationType;
      public OperationType OperationType
      {
         get => _operationType;
         set => SetProperty( ref _operationType, value );
      }

      private bool _async;
      public bool Async
      {
         get => _async;
         set => SetProperty( ref _async, value );
      }

      private VideoFilterType _videoFilterType = VideoFilterType.Copy;
      public VideoFilterType VideoFilterType
      {
         get => _videoFilterType;
         set
         {
            if ( SetProperty( ref _videoFilterType, value ) )
            {
               VideoFilter = VideoFilter.GetFilterForType( value );
            }
         }
      }

      private VideoFilter _videoFilter = new CopyVideoFilter();
      public VideoFilter VideoFilter
      {
         get => _videoFilter;
         private set => SetProperty( ref _videoFilter, value );
      }

      private AudioFilterType _audioFilterType = AudioFilterType.Copy;
      public AudioFilterType AudioFilterType
      {
         get => _audioFilterType;
         set
         {
            if ( SetProperty( ref _audioFilterType, value ) )
            {
               AudioFilter = AudioFilter.GetFilterForType( value );
            }
         }
      }

      private AudioFilter _audioFilter = new CopyAudioFilter();
      public AudioFilter AudioFilter
      {
         get => _audioFilter;
         private set => SetProperty( ref _audioFilter, value );
      }

      public RelayCommand SelectFilesCommand { get; set; }
      public RelayCommand<string> RemoveFileCommand { get; set; }
      public RelayCommand CreateTasksCommand { get; set; }
   }
}
