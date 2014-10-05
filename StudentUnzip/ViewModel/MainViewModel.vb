Imports GalaSoft.MvvmLight
Imports GalaSoft.MvvmLight.Command
Imports System.IO.Compression
Imports GalaSoft.MvvmLight.Threading

Public Class MainViewModel
    Inherits ViewModelBase

    Sub New()
        _sourceFile = "Source file..."
        _destinationPath = "Destination path..."
        _statusLog = "Status log..."
        _goCommand = New RelayCommand(Sub() Go())

        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            Dim dep = System.Deployment.Application.ApplicationDeployment.CurrentDeployment
            _windowTitle = String.Format("Student Unzip - {0}", dep.CurrentVersion)
        Else
            _windowTitle = "Student Unzip Local!"
        End If
    End Sub

    Private _windowTitle As String
    Public Property WindowTitle() As String
        Get
            Return _windowTitle
        End Get
        Set(ByVal value As String)
            If Not value = _windowTitle Then
                _windowTitle = value
                RaisePropertyChanged(Function() Me.WindowTitle)
            End If
        End Set
    End Property

    Private _sourceFile As String
    Public Property SourceFile() As String
        Get
            Return _sourceFile
        End Get
        Set(ByVal value As String)
            If Not value = _sourceFile Then
                _sourceFile = value
                RaisePropertyChanged(Function() Me.SourceFile)
                Me.PathOrig = IO.Path.GetFileNameWithoutExtension(Me.SourceFile)
            End If
        End Set
    End Property

    Private _pathAlias As String
    Public Property PathAlias() As String
        Get
            Return _pathAlias
        End Get
        Set(ByVal value As String)
            If Not value = _pathAlias Then
                _pathAlias = value
                RaisePropertyChanged(Function() Me.PathAlias)
                RaisePropertyChanged(Function() Me.HasPathAlias)
            End If
        End Set
    End Property

    Private _pathOrig As String
    Public Property PathOrig() As String
        Get
            Return _pathOrig
        End Get
        Set(ByVal value As String)
            If Not value = _pathOrig Then
                _pathOrig = value
                RaisePropertyChanged(Function() Me.PathOrig)
            End If
        End Set
    End Property

    Public ReadOnly Property HasPathAlias As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(Me.PathAlias)
        End Get
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

    Private Async Sub Go()
        Dim gotSourceFile = GetSourceFile()
        Dim gotDestinationFolder = GetDestinationFolder()

        StatusLog = String.Empty

        If Not gotSourceFile Then
            WriteStatusLine("You did not select a source file.")
        ElseIf Me.SourceFile.Length > 25 And Not Me.HasPathAlias Then
            Me.PathAlias = "UnzippedFiles"
        End If

        If gotDestinationFolder Then
            Dim hasFiles = IO.Directory.GetFiles(Me.DestinationPath).Any()
            Dim hasFolders = IO.Directory.GetDirectories(Me.DestinationPath).Any()

            If hasFiles Or hasFolders Then
                Dim folderIndex As Integer = 0
                Dim subFolder As String = "UnzippedFiles"
                Dim subBaseFolder As String = "UnzippedFiles"

                While IO.Directory.Exists(IO.Path.Combine(Me.DestinationPath, subFolder))
                    folderIndex += 1
                    subFolder = String.Format("{0}{1:000}", subBaseFolder, folderIndex)
                End While
                Me.DestinationPath = IO.Path.Combine(Me.DestinationPath, subFolder)
                IO.Directory.CreateDirectory(Me.DestinationPath)
            End If
        Else
            WriteStatusLine("You did not select a destination folder.")

        End If

        If gotDestinationFolder And gotSourceFile Then
            Await DecompressAsync()
            WriteStatusLine("All Done!")

            If Me.PathAlias = "UnzippedFiles" Then
                WriteStatusLine("The path provided by Sakai was too long.  Look in the UnzippedFiles folder.")
            End If
            System.Diagnostics.Process.Start(Me.DestinationPath)
        End If


    End Sub

    Private Sub WriteStatusLine(line As String)
        'DispatcherHelper.CheckBeginInvokeOnUI(Sub()
        Me.StatusLog = line & Environment.NewLine & Me.StatusLog
        'End Sub)
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

    Private Async Function DecompressAsync() As Task
        Using archive As ZipArchive = IO.Compression.ZipFile.OpenRead(Me.SourceFile)
            For Each entry As ZipArchiveEntry In archive.Entries
                Dim fullPath = IO.Path.Combine(Me.DestinationPath, ScrubPath(entry.FullName))
                If Not String.IsNullOrEmpty(entry.Name) Then
                    WriteStatusLine(entry.FullName)
                    Dim newPath = IO.Path.Combine(Me.DestinationPath, ScrubPath(entry.FullName.Substring(0, entry.FullName.Length - entry.Name.Length)))
                    IO.Directory.CreateDirectory(newPath)
                    If entry.Name.EndsWith("zip", StringComparison.CurrentCultureIgnoreCase) Then
                        fullPath &= ".temp"
                        entry.ExtractToFile(fullPath)
                        Await Task.Factory.StartNew(Sub()
                                                        'System.Threading.Thread.Sleep(5000)
                                                        ZipFile.ExtractToDirectory(fullPath, newPath)
                                                        IO.File.Delete(fullPath)
                                                        WriteStatusLine(newPath)
                                                    End Sub)
                    Else
                        entry.ExtractToFile(fullPath)
                    End If
                End If
            Next
        End Using

    End Function

    Private Function ScrubPath(path As String) As String
        If Me.HasPathAlias Then
            Return path.Replace(Me.PathOrig, Me.PathAlias)
        Else
            Return path
        End If
    End Function

End Class
