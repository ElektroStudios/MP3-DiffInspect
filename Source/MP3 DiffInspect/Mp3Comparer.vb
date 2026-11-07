
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Imaging
Imports System.IO

Imports NAudio.Wave

#End Region

#Region " Mp3Comparer "

''' <summary>
''' Provides the logic to compare two MP3 files, analyzing audio stream equality, cover image and metadata tags.
''' <para></para>
''' This logic is UI-independent so that it can be unit tested.
''' </summary>
Public NotInheritable Class Mp3Comparer

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="Mp3Comparer"/> class from being created.
    ''' </summary>
    Private Sub New()
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Compares two MP3 files.
    ''' </summary>
    '''
    ''' <param name="first">
    ''' The first MP3 file.
    ''' </param>
    '''
    ''' <param name="second">
    ''' The second MP3 file.
    ''' </param>
    '''
    ''' <returns>
    ''' A <see cref="Mp3ComparisonResult"/> describing the differences found.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="first"/> or <paramref name="second"/> is <see langword="Nothing"/>.
    ''' </exception>
    Public Shared Function Compare(first As Mp3Info, second As Mp3Info) As Mp3ComparisonResult

        If first Is Nothing Then
            Throw New ArgumentNullException(NameOf(first))
        End If

        If second Is Nothing Then
            Throw New ArgumentNullException(NameOf(second))
        End If

        Dim channelsEqual As Boolean = first.Channels = second.Channels
        Dim sampleRateEqual As Boolean = first.SampleRate = second.SampleRate
        Dim bitsPerSampleEqual As Boolean = first.BitsPerSample = second.BitsPerSample
        Dim bitrateEqual As Boolean = first.BitrateAverageKbps = second.BitrateAverageKbps
        Dim durationEqual As Boolean = first.Duration = second.Duration

        Dim streamPropertiesEqual As Boolean = channelsEqual AndAlso
                                               sampleRateEqual AndAlso
                                               bitsPerSampleEqual AndAlso
                                               bitrateEqual AndAlso
                                               durationEqual

        ' Frames are only read and compared when the stream properties are equal,
        ' because reading all the frames of a file is an expensive operation.
        Dim audioStreamIdentical As Boolean =
            streamPropertiesEqual AndAlso
            (first.FrameCount = second.FrameCount) AndAlso
            Mp3Comparer.AreFrameSequencesEqual(first.Frames, second.Frames)

        Dim artistEqual As Boolean = String.Equals(first.Artist, second.Artist, StringComparison.Ordinal)
        Dim titleEqual As Boolean = String.Equals(first.Title, second.Title, StringComparison.Ordinal)
        Dim albumEqual As Boolean = String.Equals(first.Album, second.Album, StringComparison.Ordinal)
        Dim yearEqual As Boolean = first.Year = second.Year
        Dim coverEqual As Boolean = Mp3Comparer.AreImagesEqual(first.Cover, second.Cover)

        Return New Mp3ComparisonResult(audioStreamIdentical,
                                       channelsEqual, sampleRateEqual, bitsPerSampleEqual, bitrateEqual, durationEqual,
                                       coverEqual,
                                       artistEqual, titleEqual, albumEqual, yearEqual)
    End Function

    ''' <summary>
    ''' Determines whether two sequences of MP3 frames contain the same raw data, frame by frame.
    ''' </summary>
    '''
    ''' <param name="first">
    ''' The first frame sequence.
    ''' </param>
    '''
    ''' <param name="second">
    ''' The second frame sequence.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if both sequences have the same number of frames
    ''' and every frame has the same raw data; otherwise, <see langword="False"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="first"/> or <paramref name="second"/> is <see langword="Nothing"/>.
    ''' </exception>
    Public Shared Function AreFrameSequencesEqual(first As IList(Of Mp3Frame), second As IList(Of Mp3Frame)) As Boolean

        If first Is Nothing Then
            Throw New ArgumentNullException(NameOf(first))
        End If

        If second Is Nothing Then
            Throw New ArgumentNullException(NameOf(second))
        End If

        If first.Count <> second.Count Then
            Return False
        End If

        ' NOTE: This iteration is a LOT faster than using the Mp3FrameSequenceComparer class.
        For i As Integer = 0 To first.Count - 1
            If Not Mp3Comparer.AreByteArraysEqual(first(i).RawData, second(i).RawData) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' Determines whether two byte arrays have the same length and content.
    ''' </summary>
    '''
    ''' <param name="first">
    ''' The first byte array.
    ''' </param>
    '''
    ''' <param name="second">
    ''' The second byte array.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if both arrays are equal; otherwise, <see langword="False"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="first"/> or <paramref name="second"/> is <see langword="Nothing"/>.
    ''' </exception>
    Public Shared Function AreByteArraysEqual(first As Byte(), second As Byte()) As Boolean

        If first Is Nothing Then
            Throw New ArgumentNullException(NameOf(first))
        End If

        If second Is Nothing Then
            Throw New ArgumentNullException(NameOf(second))
        End If

        If first.Length <> second.Length Then
            Return False
        End If

        For i As Integer = 0 To first.Length - 1
            If first(i) <> second(i) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' Determines whether two images are equal by comparing their bitmap (BMP) representation byte by byte.
    ''' </summary>
    '''
    ''' <param name="first">
    ''' The first image. Can be <see langword="Nothing"/> (no image).
    ''' </param>
    '''
    ''' <param name="second">
    ''' The second image. Can be <see langword="Nothing"/> (no image).
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if both images are <see langword="Nothing"/>,
    ''' or if both have the same bitmap representation; otherwise, <see langword="False"/>.
    ''' </returns>
    Public Shared Function AreImagesEqual(first As Image, second As Image) As Boolean

        If first Is Nothing AndAlso second Is Nothing Then
            Return True
        End If

        If first Is Nothing OrElse second Is Nothing Then
            Return False
        End If

        Using msFirst As New MemoryStream(), msSecond As New MemoryStream()
            first.Save(msFirst, ImageFormat.Bmp)
            second.Save(msSecond, ImageFormat.Bmp)

            If msFirst.Length <> msSecond.Length Then
                Return False
            End If

            msFirst.Position = 0
            msSecond.Position = 0

            Dim bufferFirst(UShort.MaxValue) As Byte
            Dim bufferSecond(UShort.MaxValue) As Byte

            While msFirst.Position < msFirst.Length
                Dim readFirst As Integer = msFirst.Read(bufferFirst, 0, bufferFirst.Length)
                Dim readSecond As Integer = msSecond.Read(bufferSecond, 0, bufferSecond.Length)

                If readFirst <> readSecond Then
                    Return False
                End If

                For i As Integer = 0 To readFirst - 1
                    If bufferFirst(i) <> bufferSecond(i) Then
                        Return False
                    End If
                Next
            End While
        End Using

        Return True
    End Function

#End Region

End Class

#End Region
