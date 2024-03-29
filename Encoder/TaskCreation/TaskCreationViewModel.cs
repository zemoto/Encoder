using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Encoder.Encoding;
using Encoder.Encoding.Tasks;
using Encoder.Filters.Audio;
using Encoder.Filters.Audio.Copy;
using Encoder.Filters.Video;
using Encoder.Filters.Video.Copy;
using Microsoft.Win32;
using ZemotoCommon.UI;
using ZemotoCommon;

namespace Encoder.TaskCreation
{
   internal sealed class TaskCreationViewModel : ViewModelBase
   {
      private readonly EncodingManager _encodingManager;

      public TaskCreationViewModel( EncodingManager encodingManager )
      {
         _encodingManager = encodingManager;
      }

      private void SelectFiles()
      {
         var dlg = new OpenFileDialog
         {
            Filter = "Video Files (*.mp4;*.wmv;*.webm;*.swf;*.mkv;*.avi)|*.mp4;*.wmv;*.webm;*.swf;*.mkv;*.avi|All files (*.*)|*.*",
            Multiselect = true
         };

         if ( dlg.ShowDialog( Application.Current.MainWindow ) == true )
         {
            foreach ( var file in dlg.FileNames )
            {
               if ( !SelectedFiles.Contains( file ) )
               {
                  SelectedFiles.Add( file );
               }
            }
         }
      }

      private void CreateAndStartNewTasks()
      {
         if ( Tasks.Count == 0 )
         {
            AddTask();
         }

         var encodingTasks = GetTasksForSelectedFiles();

         Tasks.Clear();
         SelectedFiles.Clear();
         Reset();

         if ( !encodingTasks.Any( x => x == null ) )
         {
            _encodingManager.EnqueueTasks( encodingTasks );
         }
      }

      private List<EncodingTaskBase> GetTasksForSelectedFiles()
      {
         var tasksForSelectedFiles = new List<EncodingTaskBase>();
         foreach ( var file in SelectedFiles )
         {
            var taskCopies = Tasks.Select( x => DeepCopy.Copy( x ) ).ToArray();
            tasksForSelectedFiles.Add( ConvertToAssemblyLineIfNeeded( taskCopies, file ) );
         }

         return tasksForSelectedFiles;
      }

      private static EncodingTaskBase ConvertToAssemblyLineIfNeeded( EncodingTask[] encodingSteps, string file )
      {
         var filePathProvider = new FilePathProvider( file );
         if ( encodingSteps.Length == 1 )
         {
            // Only a single task so we don't need an assembly line. Just return the task.
            var task = encodingSteps[0];
            task.FileProvider = filePathProvider;
            return task;
         }

         return new AssemblyLine( filePathProvider, encodingSteps );
      }

      private void AddTask()
      {
         EncodingTask encodingTask = EncodingType switch
         {
            EncodingType.Filters => new EncodeWithFilters( VideoFilter, AudioFilter, OverrideBitrate, CustomBitrate * 1000 ),
            EncodingType.Custom => new EncodeWithCustomParams( CustomParams, CustomExtension ),
            _ => throw new ArgumentOutOfRangeException(),
         };

         Tasks.Add( encodingTask );
         Reset();
      }

      private void Reset()
      {
         EncodingType = EncodingType.Filters;
         VideoFilterType = VideoFilterType.Copy;
         AudioFilterType = AudioFilterType.Copy;
         CustomParams = string.Empty;
         CustomExtension = string.Empty;
      }

      public ObservableCollection<string> SelectedFiles { get; } = new ObservableCollection<string>();
      public ObservableCollection<EncodingTask> Tasks { get; } = new ObservableCollection<EncodingTask>();

      private EncodingType _encodingType;
      public EncodingType EncodingType
      {
         get => _encodingType;
         set => SetProperty( ref _encodingType, value );
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

      private string _customParams;
      public string CustomParams
      {
         get => _customParams;
         set => SetProperty( ref _customParams, value );
      }

      private string _customExtension;
      public string CustomExtension
      {
         get => _customExtension;
         set => SetProperty( ref _customExtension, value );
      }

      private bool _useCustomBitrate;
      public bool OverrideBitrate
      {
         get => _useCustomBitrate;
         set => SetProperty( ref _useCustomBitrate, value );
      }

      private uint _customBitrate;
      public uint CustomBitrate
      {
         get => _customBitrate;
         set => SetProperty( ref _customBitrate, value );
      }

      private RelayCommand _selectFileCommand;
      public RelayCommand SelectFilesCommand => _selectFileCommand ??= new RelayCommand( SelectFiles );

      private RelayCommand<string> _removeFileCommand;
      public RelayCommand<string> RemoveFileCommand => _removeFileCommand ??= new RelayCommand<string>( file => SelectedFiles.Remove( file ) );

      private RelayCommand _createTasksCommand;
      public RelayCommand CreateTasksCommand => _createTasksCommand ??= new RelayCommand( CreateAndStartNewTasks, SelectedFiles.Any );

      private RelayCommand _addTaskCommand;
      public RelayCommand AddTaskCommand => _addTaskCommand ??= new RelayCommand( AddTask );
   }
}
