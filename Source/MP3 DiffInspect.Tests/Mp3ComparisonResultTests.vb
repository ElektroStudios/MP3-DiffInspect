
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Reflection

#End Region

#Region " Mp3ComparisonResultTests "

''' <summary>
''' Unit tests for <see cref="Mp3ComparisonResult"/>.
''' </summary>
<TestClass>
<TestCategory("Unit")>
Public NotInheritable Class Mp3ComparisonResultTests

#Region " Helpers "

    ''' <summary>
    ''' Creates a <see cref="Mp3ComparisonResult"/> where everything is equal except the specified fields.
    ''' </summary>
    Private Shared Function CreateResult(Optional audioStreamIdentical As Boolean = True,
                                         Optional channelsEqual As Boolean = True,
                                         Optional sampleRateEqual As Boolean = True,
                                         Optional bitsPerSampleEqual As Boolean = True,
                                         Optional bitrateEqual As Boolean = True,
                                         Optional durationEqual As Boolean = True,
                                         Optional coverEqual As Boolean = True,
                                         Optional artistEqual As Boolean = True,
                                         Optional titleEqual As Boolean = True,
                                         Optional albumEqual As Boolean = True,
                                         Optional yearEqual As Boolean = True) As Mp3ComparisonResult

        Return New Mp3ComparisonResult(audioStreamIdentical,
                                       channelsEqual, sampleRateEqual, bitsPerSampleEqual, bitrateEqual, durationEqual,
                                       coverEqual,
                                       artistEqual, titleEqual, albumEqual, yearEqual)
    End Function

    ''' <summary>
    ''' Creates a <see cref="Mp3ComparisonResult"/> where everything is different.
    ''' </summary>
    Private Shared Function CreateAllDifferentResult() As Mp3ComparisonResult

        Return New Mp3ComparisonResult(False, False, False, False, False, False, False, False, False, False, False)
    End Function

#End Region

#Region " Constructor "

    ''' <summary>
    ''' Verifies that every constructor argument is assigned to its matching property.
    ''' </summary>
    <TestMethod>
    Public Sub Constructor_AssignsEveryProperty()

        Dim result As New Mp3ComparisonResult(audioStreamIdentical:=True,
                                              channelsEqual:=False,
                                              sampleRateEqual:=True,
                                              bitsPerSampleEqual:=False,
                                              bitrateEqual:=True,
                                              durationEqual:=False,
                                              coverEqual:=True,
                                              artistEqual:=False,
                                              titleEqual:=True,
                                              albumEqual:=False,
                                              yearEqual:=True)

        Assert.IsTrue(result.AudioStreamIdentical)
        Assert.IsFalse(result.ChannelsEqual)
        Assert.IsTrue(result.SampleRateEqual)
        Assert.IsFalse(result.BitsPerSampleEqual)
        Assert.IsTrue(result.BitrateEqual)
        Assert.IsFalse(result.DurationEqual)
        Assert.IsTrue(result.CoverEqual)
        Assert.IsFalse(result.ArtistEqual)
        Assert.IsTrue(result.TitleEqual)
        Assert.IsFalse(result.AlbumEqual)
        Assert.IsTrue(result.YearEqual)
    End Sub

#End Region

#Region " MetadataTagsEqual "

    ''' <summary>
    ''' Verifies that <see cref="Mp3ComparisonResult.MetadataTagsEqual"/> is <see langword="True"/> when all tags are equal.
    ''' </summary>
    <TestMethod>
    Public Sub MetadataTagsEqual_AllTagsEqual_ReturnsTrue()

        Assert.IsTrue(Mp3ComparisonResultTests.CreateResult().MetadataTagsEqual)
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3ComparisonResult.MetadataTagsEqual"/> is <see langword="False"/> when any tag differs.
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
    Public Sub MetadataTagsEqual_AnyTagDifferent_ReturnsFalse(tagName As String)

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult(artistEqual:=tagName <> "Artist",
                                                                                  titleEqual:=tagName <> "Title",
                                                                                  albumEqual:=tagName <> "Album",
                                                                                  yearEqual:=tagName <> "Year")
        Assert.IsFalse(result.MetadataTagsEqual)
    End Sub

    ''' <summary>
    ''' Verifies that the cover is not considered a metadata tag by <see cref="Mp3ComparisonResult.MetadataTagsEqual"/>.
    ''' </summary>
    <TestMethod>
    Public Sub MetadataTagsEqual_OnlyCoverDifferent_ReturnsTrue()

        Assert.IsTrue(Mp3ComparisonResultTests.CreateResult(coverEqual:=False).MetadataTagsEqual)
    End Sub

#End Region

#Region " OtherDifferencesSummary "

    ''' <summary>
    ''' Verifies that the summary is empty when there are no differences.
    ''' </summary>
    <TestMethod>
    Public Sub OtherDifferencesSummary_NoDifferences_IsEmpty()

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult()

        Assert.AreEqual(String.Empty, result.OtherDifferencesSummary)
        Assert.IsFalse(result.HasOtherDifferences)
    End Sub

    ''' <summary>
    ''' Verifies that the audio stream frame data difference alone is not part of the summary.
    ''' </summary>
    <TestMethod>
    Public Sub OtherDifferencesSummary_OnlyAudioStreamDifferent_IsEmpty()

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult(audioStreamIdentical:=False)

        Assert.AreEqual(String.Empty, result.OtherDifferencesSummary)
        Assert.IsFalse(result.HasOtherDifferences)
    End Sub

    ''' <summary>
    ''' Verifies the summary entry produced by each individual difference.
    ''' </summary>
    '''
    ''' <param name="field">
    ''' The field that differs.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected summary.
    ''' </param>
    <TestMethod>
    <DataRow("Channels", "Channels")>
    <DataRow("SampleRate", "Sample Rate")>
    <DataRow("BitsPerSample", "Bits Per Sample")>
    <DataRow("Bitrate", "Bitrate")>
    <DataRow("Duration", "Duration")>
    <DataRow("Cover", "Cover")>
    <DataRow("Artist", "Metadata tags")>
    <DataRow("Title", "Metadata tags")>
    <DataRow("Album", "Metadata tags")>
    <DataRow("Year", "Metadata tags")>
    Public Sub OtherDifferencesSummary_SingleDifference_ReturnsExpectedEntry(field As String, expected As String)

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult(channelsEqual:=field <> "Channels",
                                                                                  sampleRateEqual:=field <> "SampleRate",
                                                                                  bitsPerSampleEqual:=field <> "BitsPerSample",
                                                                                  bitrateEqual:=field <> "Bitrate",
                                                                                  durationEqual:=field <> "Duration",
                                                                                  coverEqual:=field <> "Cover",
                                                                                  artistEqual:=field <> "Artist",
                                                                                  titleEqual:=field <> "Title",
                                                                                  albumEqual:=field <> "Album",
                                                                                  yearEqual:=field <> "Year")
        Assert.AreEqual(expected, result.OtherDifferencesSummary)
        Assert.IsTrue(result.HasOtherDifferences)
    End Sub

    ''' <summary>
    ''' Verifies the order of the entries and that multiple tag differences produce a single "Metadata tags" entry.
    ''' </summary>
    <TestMethod>
    Public Sub OtherDifferencesSummary_AllDifferent_ReturnsEntriesInOrder()

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateAllDifferentResult()

        Assert.AreEqual("Channels, Sample Rate, Bits Per Sample, Bitrate, Duration, Cover, Metadata tags",
                        result.OtherDifferencesSummary)
    End Sub

    ''' <summary>
    ''' Verifies that the summary has no leading or trailing separators.
    ''' </summary>
    <TestMethod>
    Public Sub OtherDifferencesSummary_HasNoDanglingSeparators()

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult(bitrateEqual:=False, coverEqual:=False)
        Dim summary As String = result.OtherDifferencesSummary

        Assert.AreEqual("Bitrate, Cover", summary)
        Assert.IsFalse(summary.StartsWith(Mp3ComparisonResult.SummarySeparator, StringComparison.Ordinal))
        Assert.IsFalse(summary.EndsWith(Mp3ComparisonResult.SummarySeparator, StringComparison.Ordinal))
    End Sub

#End Region

#Region " DifferentPropertyDisplayNames "

    ''' <summary>
    ''' Verifies that no property name is returned when there are no differences.
    ''' </summary>
    <TestMethod>
    Public Sub DifferentPropertyDisplayNames_NoDifferences_IsEmpty()

        Assert.IsEmpty(Mp3ComparisonResultTests.CreateResult().DifferentPropertyDisplayNames)
    End Sub

    ''' <summary>
    ''' Verifies the order of the property names when everything differs.
    ''' </summary>
    <TestMethod>
    Public Sub DifferentPropertyDisplayNames_AllDifferent_ReturnsNamesInOrder()

        Dim expected As String() = {"Artist", "Title", "Album", "Year",
                                    "Channels", "Sample Rate", "Bits Per Sample", "Bitrate", "Duration",
                                    "Cover"}

        CollectionAssert.AreEqual(expected, Mp3ComparisonResultTests.CreateAllDifferentResult().DifferentPropertyDisplayNames.ToArray())
    End Sub

    ''' <summary>
    ''' Verifies that only the property names that differ are returned.
    ''' </summary>
    <TestMethod>
    Public Sub DifferentPropertyDisplayNames_SomeDifferent_ReturnsOnlyThose()

        Dim result As Mp3ComparisonResult = Mp3ComparisonResultTests.CreateResult(titleEqual:=False, durationEqual:=False)

        CollectionAssert.AreEqual({"Title", "Duration"}, result.DifferentPropertyDisplayNames.ToArray())
    End Sub

    ''' <summary>
    ''' Verifies that every property name highlighted in the PropertyGrid matches the
    ''' <see cref="DisplayNameAttribute"/> of a browsable <see cref="Mp3Info"/> property.
    ''' <para></para>
    ''' This guards against renaming a <see cref="DisplayNameAttribute"/> without updating the highlight logic.
    ''' </summary>
    <TestMethod>
    Public Sub DifferentPropertyDisplayNames_MatchBrowsableMp3InfoDisplayNames()

        ' These names are reported as differences but are not shown in the PropertyGrid:
        ' "Bits Per Sample" (its property is commented out) and "Cover" (shown in a PictureBox).
        Dim notInPropertyGrid As String() = {"Bits Per Sample", "Cover"}

        Dim browsableDisplayNames As New HashSet(Of String)(StringComparer.Ordinal)
        For Each prop As PropertyInfo In GetType(Mp3Info).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim browsable As BrowsableAttribute = prop.GetCustomAttribute(Of BrowsableAttribute)()
            If browsable IsNot Nothing AndAlso Not browsable.Browsable Then
                Continue For
            End If

            Dim displayName As DisplayNameAttribute = prop.GetCustomAttribute(Of DisplayNameAttribute)()
            browsableDisplayNames.Add(If(displayName IsNot Nothing, displayName.DisplayName, prop.Name))
        Next

        For Each name As String In Mp3ComparisonResultTests.CreateAllDifferentResult().DifferentPropertyDisplayNames
            If notInPropertyGrid.Contains(name) Then
                Continue For
            End If

            Assert.IsTrue(browsableDisplayNames.Contains(name),
                          $"No browsable Mp3Info property has DisplayName ""{name}"".")
        Next
    End Sub

    ''' <summary>
    ''' Verifies that the returned collection is read-only.
    ''' </summary>
    <TestMethod>
    Public Sub DifferentPropertyDisplayNames_IsReadOnly()

        Dim names As IList(Of String) = Mp3ComparisonResultTests.CreateAllDifferentResult().DifferentPropertyDisplayNames

        Assert.IsTrue(names.IsReadOnly)
        Assert.ThrowsExactly(Of NotSupportedException)(Sub() names.Add("Extra"))
    End Sub

#End Region

End Class

#End Region
