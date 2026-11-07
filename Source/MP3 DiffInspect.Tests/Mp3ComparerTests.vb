
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports MP3DiffInspect.Tests.Infrastructure

#End Region

#Region " Mp3ComparerTests "

''' <summary>
''' Integration tests for <see cref="Mp3Comparer.Compare"/>, using pairs of MP3 files generated on the fly.
''' </summary>
<TestClass>
<TestCategory("Integration")>
Public NotInheritable Class Mp3ComparerTests : Inherits Mp3FileTestBase

#Region " Helpers "

    ''' <summary>
    ''' Writes two MP3 files and compares them.
    ''' </summary>
    '''
    ''' <param name="first">
    ''' The builder of the first file.
    ''' </param>
    '''
    ''' <param name="second">
    ''' The builder of the second file.
    ''' </param>
    '''
    ''' <returns>
    ''' The comparison result.
    ''' </returns>
    Private Function CompareFiles(first As Mp3TestFileBuilder, second As Mp3TestFileBuilder) As Mp3ComparisonResult

        Dim firstPath As String = Me.CreateMp3File("first.mp3", first)
        Dim secondPath As String = Me.CreateMp3File("second.mp3", second)

        Using a As New Mp3Info(firstPath), b As New Mp3Info(secondPath)
            Return Mp3Comparer.Compare(a, b)
        End Using
    End Function

    ''' <summary>
    ''' Creates a builder for a fully tagged file with the default audio stream.
    ''' </summary>
    '''
    ''' <param name="coverPng">
    ''' Optional PNG data of the cover.
    ''' </param>
    '''
    ''' <returns>
    ''' The configured builder.
    ''' </returns>
    Private Shared Function CreateTaggedBuilder(Optional coverPng As Byte() = Nothing) As Mp3TestFileBuilder

        Return New Mp3TestFileBuilder().
            WithFrameCount(40).
            WithPayloadByte(&H11).
            WithId3v2("Artist", "Title", "Album", "2001", coverPng)
    End Function

#End Region

#Region " Argument Validation "

    ''' <summary>
    ''' Verifies that a <see langword="Nothing"/> argument throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_NullArgument_ThrowsArgumentNullException()

        Using info As New Mp3Info(Me.GetTempFilePath("unused.mp3"))
            Dim ex1 As ArgumentNullException = Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.Compare(Nothing, info))
            Dim ex2 As ArgumentNullException = Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.Compare(info, Nothing))

            Assert.AreEqual("first", ex1.ParamName)
            Assert.AreEqual("second", ex2.ParamName)
        End Using
    End Sub

#End Region

#Region " Identical Files "

    ''' <summary>
    ''' Verifies that two byte-identical files have no differences at all.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_IdenticalFiles_NoDifferences()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(16, 16, Color.Red)
        Dim result As Mp3ComparisonResult = Me.CompareFiles(Mp3ComparerTests.CreateTaggedBuilder(png),
                                                            Mp3ComparerTests.CreateTaggedBuilder(png))

        Assert.IsTrue(result.AudioStreamIdentical)
        Assert.IsTrue(result.MetadataTagsEqual)
        Assert.IsTrue(result.CoverEqual)
        Assert.IsFalse(result.HasOtherDifferences)
        Assert.IsEmpty(result.DifferentPropertyDisplayNames)
    End Sub

    ''' <summary>
    ''' Verifies that comparing an instance with itself reports no differences.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_SameInstance_NoDifferences()

        Dim filePath As String = Me.CreateMp3File("self.mp3", Mp3ComparerTests.CreateTaggedBuilder())

        Using info As New Mp3Info(filePath)
            Dim result As Mp3ComparisonResult = Mp3Comparer.Compare(info, info)

            Assert.IsTrue(result.AudioStreamIdentical)
            Assert.IsFalse(result.HasOtherDifferences)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that two untagged files with the same audio stream are identical.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_UntaggedIdenticalFiles_NoDifferences()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder(), New Mp3TestFileBuilder())

        Assert.IsTrue(result.AudioStreamIdentical)
        Assert.IsTrue(result.CoverEqual)
        Assert.IsFalse(result.HasOtherDifferences)
    End Sub

#End Region

#Region " Metadata Differences "

    ''' <summary>
    ''' Verifies that files with the same audio but different tags have an identical audio stream.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_SameAudioDifferentTags_AudioStreamIdentical()

        Dim first As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithPayloadByte(&H22).WithId3v2("Artist A", "Title A", "Album A", "2001")
        Dim second As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithPayloadByte(&H22).WithId3v2("Artist B", "Title B", "Album B", "2002")

        Dim result As Mp3ComparisonResult = Me.CompareFiles(first, second)

        Assert.IsTrue(result.AudioStreamIdentical)
        Assert.IsFalse(result.MetadataTagsEqual)
        Assert.AreEqual("Metadata tags", result.OtherDifferencesSummary)
        CollectionAssert.AreEqual({"Artist", "Title", "Album", "Year"}, result.DifferentPropertyDisplayNames.ToArray())
    End Sub

    ''' <summary>
    ''' Verifies that each individual tag difference is detected.
    ''' </summary>
    '''
    ''' <param name="tagName">
    ''' The tag that differs.
    ''' </param>
    <TestMethod>
    <DataRow("Artist")>
    <DataRow("Title")>
    <DataRow("Album")>
    <DataRow("Year")>
    Public Sub Compare_SingleTagDifferent_OnlyThatTagIsReported(tagName As String)

        Dim first As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v2("Artist", "Title", "Album", "2001")
        Dim second As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v2(
            If(tagName = "Artist", "Other", "Artist"),
            If(tagName = "Title", "Other", "Title"),
            If(tagName = "Album", "Other", "Album"),
            If(tagName = "Year", "1999", "2001"))

        Dim result As Mp3ComparisonResult = Me.CompareFiles(first, second)

        Assert.IsTrue(result.AudioStreamIdentical)
        CollectionAssert.AreEqual({tagName}, result.DifferentPropertyDisplayNames.ToArray())
        Assert.AreEqual("Metadata tags", result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that the tag comparison is case-sensitive.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_TagsDifferOnlyInCase_AreDifferent()

        Dim first As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v2(artist:="ARTIST")
        Dim second As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v2(artist:="artist")

        Dim result As Mp3ComparisonResult = Me.CompareFiles(first, second)

        Assert.IsFalse(result.ArtistEqual)
    End Sub

    ''' <summary>
    ''' Verifies that the same tag values stored as ID3v2 in one file and as ID3v1 in the other are considered equal.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_SameTagValuesInDifferentTagVersions_AreEqual()

        Dim first As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v2("Artist", "Title", "Album", "2001")
        Dim second As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithId3v1("Artist", "Title", "Album", "2001")

        Dim result As Mp3ComparisonResult = Me.CompareFiles(first, second)

        Assert.IsTrue(result.MetadataTagsEqual)
        Assert.IsTrue(result.AudioStreamIdentical)
    End Sub

#End Region

#Region " Cover Differences "

    ''' <summary>
    ''' Verifies that different cover images are detected.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_DifferentCover_CoverIsReported()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(
            Mp3ComparerTests.CreateTaggedBuilder(TestImageFactory.CreateSolidPng(16, 16, Color.Red)),
            Mp3ComparerTests.CreateTaggedBuilder(TestImageFactory.CreateSolidPng(16, 16, Color.Blue)))

        Assert.IsTrue(result.AudioStreamIdentical)
        Assert.IsFalse(result.CoverEqual)
        Assert.AreEqual("Cover", result.OtherDifferencesSummary)
        CollectionAssert.AreEqual({"Cover"}, result.DifferentPropertyDisplayNames.ToArray())
    End Sub

    ''' <summary>
    ''' Verifies that a cover present in only one file is detected, in both orders.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_CoverOnlyInOneFile_CoverIsReported()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(16, 16, Color.Red)

        Dim resultA As Mp3ComparisonResult = Me.CompareFiles(Mp3ComparerTests.CreateTaggedBuilder(png),
                                                             Mp3ComparerTests.CreateTaggedBuilder())
        Assert.IsFalse(resultA.CoverEqual)

        Dim resultB As Mp3ComparisonResult = Me.CompareFiles(Mp3ComparerTests.CreateTaggedBuilder(),
                                                             Mp3ComparerTests.CreateTaggedBuilder(png))
        Assert.IsFalse(resultB.CoverEqual)
    End Sub

#End Region

#Region " Audio Stream Differences "

    ''' <summary>
    ''' Verifies that a different bitrate (same frame count and sample rate) is reported,
    ''' while the duration stays equal.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_DifferentBitrate_BitrateIsReported()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder().WithBitrate(128),
                                                            New Mp3TestFileBuilder().WithBitrate(160))

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.IsFalse(result.BitrateEqual)
        Assert.IsTrue(result.DurationEqual)
        Assert.AreEqual("Bitrate", result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that a different number of channels is reported.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_DifferentChannels_ChannelsIsReported()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder().WithMono(False),
                                                            New Mp3TestFileBuilder().WithMono(True))

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.IsFalse(result.ChannelsEqual)
        Assert.AreEqual("Channels", result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that a different sample rate (same frame count) is reported together with the duration.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_DifferentSampleRate_SampleRateAndDurationAreReported()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder().WithSampleRate(44100),
                                                            New Mp3TestFileBuilder().WithSampleRate(48000))

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.IsFalse(result.SampleRateEqual)
        Assert.IsFalse(result.DurationEqual)
        Assert.AreEqual("Sample Rate, Duration", result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that a different number of frames is reported as a different duration.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_DifferentFrameCount_DurationIsReported()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder().WithFrameCount(40),
                                                            New Mp3TestFileBuilder().WithFrameCount(41))

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.IsFalse(result.DurationEqual)
        Assert.AreEqual("Duration", result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that files with the same stream properties but different frame data
    ''' are reported as having a different audio stream, with no other differences.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_SamePropertiesDifferentFrameData_AudioStreamDifferentOnly()

        Dim result As Mp3ComparisonResult = Me.CompareFiles(New Mp3TestFileBuilder().WithPayloadByte(&H1),
                                                            New Mp3TestFileBuilder().WithPayloadByte(&H2))

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.IsTrue(result.ChannelsEqual)
        Assert.IsTrue(result.SampleRateEqual)
        Assert.IsTrue(result.BitrateEqual)
        Assert.IsTrue(result.DurationEqual)
        Assert.IsFalse(result.HasOtherDifferences)
    End Sub

    ''' <summary>
    ''' Verifies that the comparison is symmetric (swapping the files gives the same result).
    ''' </summary>
    <TestMethod>
    Public Sub Compare_IsSymmetric()

        Dim firstPath As String = Me.CreateMp3File("a.mp3", New Mp3TestFileBuilder().WithBitrate(128).WithId3v2(artist:="A"))
        Dim secondPath As String = Me.CreateMp3File("b.mp3", New Mp3TestFileBuilder().WithBitrate(160).WithId3v2(artist:="B"))

        Using a As New Mp3Info(firstPath), b As New Mp3Info(secondPath)
            Dim ab As Mp3ComparisonResult = Mp3Comparer.Compare(a, b)
            Dim ba As Mp3ComparisonResult = Mp3Comparer.Compare(b, a)

            Assert.AreEqual(ab.AudioStreamIdentical, ba.AudioStreamIdentical)
            Assert.AreEqual(ab.OtherDifferencesSummary, ba.OtherDifferencesSummary)
            CollectionAssert.AreEqual(ab.DifferentPropertyDisplayNames.ToArray(), ba.DifferentPropertyDisplayNames.ToArray())
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that audio and metadata differences are all reported together.
    ''' </summary>
    <TestMethod>
    Public Sub Compare_EverythingDifferent_AllDifferencesAreReported()

        Dim first As Mp3TestFileBuilder = New Mp3TestFileBuilder().
            WithBitrate(128).WithSampleRate(44100).WithMono(False).
            WithId3v2("A", "T", "B", "2001", TestImageFactory.CreateSolidPng(8, 8, Color.Red))

        Dim second As Mp3TestFileBuilder = New Mp3TestFileBuilder().
            WithBitrate(160).WithSampleRate(48000).WithMono(True).
            WithId3v2("X", "Y", "Z", "1999", TestImageFactory.CreateSolidPng(8, 8, Color.Blue))

        Dim result As Mp3ComparisonResult = Me.CompareFiles(first, second)

        Assert.IsFalse(result.AudioStreamIdentical)
        Assert.AreEqual("Channels, Sample Rate, Bitrate, Duration, Cover, Metadata tags", result.OtherDifferencesSummary)
    End Sub

#End Region

End Class

#End Region
