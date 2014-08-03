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
            _destinationPath = value
        End Set
    End Property

    Private _statusLog As String
    Public Property StatusLog() As String
        Get
            Return _statusLog
        End Get
        Set(ByVal value As String)
            _statusLog = value
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
        SourceFile = SourceFile & "."
    End Sub


End Class
