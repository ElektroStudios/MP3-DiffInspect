
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " TempDirectory "

Namespace Infrastructure

    ''' <summary>
    ''' Represents a unique temporary directory that is deleted (with all its contents) when disposed.
    ''' </summary>
    Friend NotInheritable Class TempDirectory : Implements IDisposable

        Private disposedValue As Boolean

        ''' <summary>
        ''' Gets the full path of the temporary directory.
        ''' </summary>
        Friend ReadOnly Property FullPath As String

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TempDirectory"/> class, creating the directory on disk.
        ''' </summary>
        Friend Sub New()

            Me.FullPath = Path.Combine(Path.GetTempPath(), "MP3 DiffInspect.Tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(Me.FullPath)
        End Sub

        ''' <summary>
        ''' Combines the temporary directory path with the specified file name.
        ''' </summary>
        '''
        ''' <param name="fileName">
        ''' The file name.
        ''' </param>
        '''
        ''' <returns>
        ''' The full file path.
        ''' </returns>
        Friend Function GetFilePath(fileName As String) As String

            Return Path.Combine(Me.FullPath, fileName)
        End Function

        ''' <summary>
        ''' Deletes the temporary directory and all its contents.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If Me.disposedValue Then
                Exit Sub
            End If

            If Directory.Exists(Me.FullPath) Then
                Directory.Delete(Me.FullPath, recursive:=True)
            End If

            Me.disposedValue = True
        End Sub

    End Class

End Namespace

#End Region
