
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Mp3FileTestBase "

Namespace Infrastructure

    ''' <summary>
    ''' Base class for tests that need real MP3 files on disk.
    ''' <para></para>
    ''' Each test gets its own temporary directory, which is deleted after the test runs.
    ''' </summary>
    Public MustInherit Class Mp3FileTestBase

#Region " Constants "

        ''' <summary>
        ''' The maximum difference, in milliseconds, tolerated when comparing durations.
        ''' <para></para>
        ''' On .NET Framework, <see cref="TimeSpan.FromSeconds(Double)"/> (used internally by NAudio)
        ''' rounds to the nearest millisecond.
        ''' </summary>
        Protected Const DurationToleranceMs As Double = 2.0

#End Region

#Region " Private Fields "

        Private tempDir As TempDirectory

#End Region

#Region " Test Lifecycle "

        ''' <summary>
        ''' Creates the temporary directory for the current test.
        ''' </summary>
        <TestInitialize>
        Public Sub BaseTestInitialize()

            Me.tempDir = New TempDirectory()
        End Sub

        ''' <summary>
        ''' Deletes the temporary directory of the current test.
        ''' </summary>
        <TestCleanup>
        Public Sub BaseTestCleanup()

            Me.tempDir?.Dispose()
            Me.tempDir = Nothing
        End Sub

#End Region

#Region " Protected Methods "

        ''' <summary>
        ''' Writes an MP3 file built by the specified builder into the temporary directory of the current test.
        ''' </summary>
        '''
        ''' <param name="fileName">
        ''' The file name.
        ''' </param>
        '''
        ''' <param name="builder">
        ''' The configured builder.
        ''' </param>
        '''
        ''' <returns>
        ''' The full path of the written file.
        ''' </returns>
        Friend Function CreateMp3File(fileName As String, builder As Mp3TestFileBuilder) As String

            If builder Is Nothing Then
                Throw New ArgumentNullException(NameOf(builder))
            End If

            Return builder.Build(Me.tempDir.GetFilePath(fileName))
        End Function

        ''' <summary>
        ''' Gets a path inside the temporary directory of the current test, without creating any file.
        ''' </summary>
        '''
        ''' <param name="fileName">
        ''' The file name.
        ''' </param>
        '''
        ''' <returns>
        ''' The full file path.
        ''' </returns>
        Protected Function GetTempFilePath(fileName As String) As String

            Return Me.tempDir.GetFilePath(fileName)
        End Function

        ''' <summary>
        ''' Asserts that two durations are equal within <see cref="DurationToleranceMs"/>.
        ''' </summary>
        '''
        ''' <param name="expected">
        ''' The expected duration.
        ''' </param>
        '''
        ''' <param name="actual">
        ''' The actual duration.
        ''' </param>
        Protected Shared Sub AssertDurationsAreClose(expected As TimeSpan, actual As TimeSpan)

            Dim deltaMs As Double = Math.Abs((expected - actual).TotalMilliseconds)
            Assert.IsLessThanOrEqualTo(Mp3FileTestBase.DurationToleranceMs, deltaMs,
                                       $"Expected duration {expected} but was {actual} (delta: {deltaMs} ms).")
        End Sub

#End Region

    End Class

End Namespace

#End Region
