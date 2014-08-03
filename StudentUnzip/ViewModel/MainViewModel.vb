Imports GalaSoft.MvvmLight
Imports GalaSoft.MvvmLight.Command

Public Class MainViewModel
    Inherits ViewModelBase

    Sub New()
        _sourceFile = "Source file..."
        _destinationPath = "Destination path..."
        _statusLog = "Status log..."
        _goCommand = New RelayCommand(Sub() Go())
    End Sub

    Private _sourceFile As String
    Public Property SourceFile() As String
        Get
            Return _sourceFile
        End Get
        Set(ByVal value As String)
            If Not value = _sourceFile Then
                _sourceFile = value
                RaisePropertyChanged(Function() Me.SourceFile)
            End If
        End Set
    End Property

    Private _destinationPath As String
    Public Property DestinationPath() As String
        Get
            Return _destinationPath
        End Get
        Set(ByVal value As String)
            If Not value = _destinationPath Then
                _destinationPath = value
                RaisePropertyChanged(Function() Me.DestinationPath)
            End If
        End Set
    End Property

    Private _statusLog As String
    Public Property StatusLog() As String
        Get
            Return _statusLog
        End Get
        Set(ByVal value As String)
            If Not value = _statusLog Then
                _statusLog = value
                RaisePropertyChanged(Function() Me.StatusLog)
            End If
        End Set
    End Property

    Private _goCommand As RelayCommand
    Public Property GoCommand() As RelayCommand
        Get
            Return _goCommand
        End Get
        Set(ByVal value As RelayCommand)
            _goCommand = value
        End Set
    End Property

    Private Sub Go()
        Dim gotSourceFile = GetSourceFile()
        Dim gotDestinationFolder = GetDestinationFolder()
        Dim isDestinationEmpty As Boolean = True

        If Not gotSourceFile Then
            WriteStatusLine("You did not select a source file.")
        End If

        If gotDestinationFolder Then
            Dim hasFiles = System.IO.Directory.GetFiles(Me.DestinationPath).Any()
            Dim hasFolders = System.IO.Directory.GetDirectories(Me.DestinationPath).Any()

            If hasFiles Or hasFolders Then
                WriteStatusLine("Destination not empty.  Please try again.")
                isDestinationEmpty = False
            End If
        Else
            WriteStatusLine("You did not select a destination folder.")

        End If

        If gotDestinationFolder And gotSourceFile And isDestinationEmpty Then
            Decompress()

        End If
    End Sub

    Private Sub WriteStatusLine(line As String)
        Me.StatusLog = line & Environment.NewLine & Me.StatusLog
    End Sub
    Private Function GetSourceFile() As Boolean
        ' Configure open file dialog box 
        Dim dlg As New Microsoft.Win32.OpenFileDialog()
        dlg.FileName = "bulk_download.zip" ' Default file name
        dlg.DefaultExt = ".zip" ' Default file extension
        dlg.Filter = "Zip files (.zip)|*.zip" ' Filter files by extension

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result = True Then
            ' Open document 
            Me.SourceFile = dlg.FileName
            Return True
        End If
        Return False
    End Function

    Private Function GetDestinationFolder() As Boolean
        ' Configure open file dialog box 
        Dim dlg As New System.Windows.Forms.FolderBrowserDialog()

        ' Show open file dialog box 
        Dim result = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result = System.Windows.Forms.DialogResult.OK Then
            ' Open document 
            Me.DestinationPath = dlg.SelectedPath
            Return True
        End If
        Return False
    End Function

    Private Sub Decompress()
        Throw New NotImplementedException
    End Sub


End Class
