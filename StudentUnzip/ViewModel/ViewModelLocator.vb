Imports GalaSoft.MvvmLight
Imports GalaSoft.MvvmLight.Ioc
Imports Microsoft.Practices.ServiceLocation

Public Class ViewModelLocator
    Sub New()
        'Inversion of control.
        ServiceLocator.SetLocatorProvider(Function() SimpleIoc.Default)
        SimpleIoc.Default.Register(Of MainViewModel)()
    End Sub

    Public ReadOnly Property Main() As MainViewModel
        Get
            Return ServiceLocator.Current.GetInstance(Of MainViewModel)()
        End Get
    End Property


End Class
