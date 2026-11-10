
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.ObjectModel
Imports System.Text

#End Region

#Region " Mp3ComparisonResult "

''' <summary>
''' Represents the immutable result of comparing two <see cref="Mp3Info"/> objects.
''' </summary>
Public NotInheritable Class Mp3ComparisonResult ' NOSONAR

#Region " Constants "

    ''' <summary>
    ''' The separator used to join the entries of <see cref="Mp3ComparisonResult.OtherDifferencesSummary"/>.
    ''' </summary>
    Public Const SummarySeparator As String = ", "

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets a value indicating whether both audio streams are identical
    ''' (same stream properties and same raw frame data).
    ''' </summary>
    Public ReadOnly Property AudioStreamIdentical As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same number of channels.
    ''' </summary>
    Public ReadOnly Property ChannelsEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same sample rate.
    ''' </summary>
    Public ReadOnly Property SampleRateEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same bits per sample.
    ''' </summary>
    Public ReadOnly Property BitsPerSampleEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same average bitrate.
    ''' </summary>
    Public ReadOnly Property BitrateEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same duration.
    ''' </summary>
    Public ReadOnly Property DurationEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same embedded cover image.
    ''' </summary>
    Public ReadOnly Property CoverEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same artist tag.
    ''' </summary>
    Public ReadOnly Property ArtistEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same title tag.
    ''' </summary>
    Public ReadOnly Property TitleEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same album tag.
    ''' </summary>
    Public ReadOnly Property AlbumEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether both files have the same year tag.
    ''' </summary>
    Public ReadOnly Property YearEqual As Boolean

    ''' <summary>
    ''' Gets a value indicating whether all the compared metadata tags (artist, title, album and year) are equal.
    ''' </summary>
    Public ReadOnly Property MetadataTagsEqual As Boolean
        Get
            Return Me.ArtistEqual AndAlso Me.TitleEqual AndAlso Me.AlbumEqual AndAlso Me.YearEqual
        End Get
    End Property

    ''' <summary>
    ''' Gets a human-readable, comma-separated summary of the differences that are not
    ''' the audio stream itself (for example: "Bitrate, Cover, Metadata tags").
    ''' <para></para>
    ''' Returns <see cref="String.Empty"/> when there are no such differences.
    ''' </summary>
    Public ReadOnly Property OtherDifferencesSummary As String
        Get
            Dim parts As New List(Of String)

            If Not Me.ChannelsEqual Then parts.Add("Channels")
            If Not Me.SampleRateEqual Then parts.Add("Sample Rate")
            If Not Me.BitsPerSampleEqual Then parts.Add("Bits Per Sample")
            If Not Me.BitrateEqual Then parts.Add("Bitrate")
            If Not Me.DurationEqual Then parts.Add("Duration")
            If Not Me.CoverEqual Then parts.Add("Cover")
            If Not Me.MetadataTagsEqual Then parts.Add("Metadata tags")

            Return String.Join(Mp3ComparisonResult.SummarySeparator, parts)
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether <see cref="Mp3ComparisonResult.OtherDifferencesSummary"/> contains any difference.
    ''' </summary>
    Public ReadOnly Property HasOtherDifferences As Boolean
        Get
            Return Not String.IsNullOrEmpty(Me.OtherDifferencesSummary)
        End Get
    End Property

    ''' <summary>
    ''' Gets the display names of the properties that differ, in the order they are highlighted in the UI.
    ''' <para></para>
    ''' These names match the <see cref="System.ComponentModel.DisplayNameAttribute"/> of
    ''' the corresponding <see cref="Mp3Info"/> properties shown in the PropertyGrid.
    ''' </summary>
    Public ReadOnly Property DifferentPropertyDisplayNames As ReadOnlyCollection(Of String)
        Get
            Dim names As New List(Of String)

            If Not Me.ArtistEqual Then names.Add("Artist")
            If Not Me.TitleEqual Then names.Add("Title")
            If Not Me.AlbumEqual Then names.Add("Album")
            If Not Me.YearEqual Then names.Add("Year")
            If Not Me.ChannelsEqual Then names.Add("Channels")
            If Not Me.SampleRateEqual Then names.Add("Sample Rate")
            If Not Me.BitsPerSampleEqual Then names.Add("Bits Per Sample")
            If Not Me.BitrateEqual Then names.Add("Bitrate")
            If Not Me.DurationEqual Then names.Add("Duration")
            If Not Me.CoverEqual Then names.Add("Cover")

            Return names.AsReadOnly()
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Mp3ComparisonResult"/> class.
    ''' </summary>
    '''
    ''' <param name="audioStreamIdentical">
    ''' <see langword="True"/> if both audio streams are identical.
    ''' </param>
    '''
    ''' <param name="channelsEqual">
    ''' <see langword="True"/> if the number of channels is equal.
    ''' </param>
    '''
    ''' <param name="sampleRateEqual">
    ''' <see langword="True"/> if the sample rate is equal.
    ''' </param>
    '''
    ''' <param name="bitsPerSampleEqual">
    ''' <see langword="True"/> if the bits per sample are equal.
    ''' </param>
    '''
    ''' <param name="bitrateEqual">
    ''' <see langword="True"/> if the average bitrate is equal.
    ''' </param>
    '''
    ''' <param name="durationEqual">
    ''' <see langword="True"/> if the duration is equal.
    ''' </param>
    '''
    ''' <param name="coverEqual">
    ''' <see langword="True"/> if the embedded cover image is equal.
    ''' </param>
    '''
    ''' <param name="artistEqual">
    ''' <see langword="True"/> if the artist tag is equal.
    ''' </param>
    '''
    ''' <param name="titleEqual">
    ''' <see langword="True"/> if the title tag is equal.
    ''' </param>
    '''
    ''' <param name="albumEqual">
    ''' <see langword="True"/> if the album tag is equal.
    ''' </param>
    '''
    ''' <param name="yearEqual">
    ''' <see langword="True"/> if the year tag is equal.
    ''' </param>
    Public Sub New(audioStreamIdentical As Boolean,
                   channelsEqual As Boolean,
                   sampleRateEqual As Boolean,
                   bitsPerSampleEqual As Boolean,
                   bitrateEqual As Boolean,
                   durationEqual As Boolean,
                   coverEqual As Boolean,
                   artistEqual As Boolean,
                   titleEqual As Boolean,
                   albumEqual As Boolean,
                   yearEqual As Boolean)

        Me.AudioStreamIdentical = audioStreamIdentical
        Me.ChannelsEqual = channelsEqual
        Me.SampleRateEqual = sampleRateEqual
        Me.BitsPerSampleEqual = bitsPerSampleEqual
        Me.BitrateEqual = bitrateEqual
        Me.DurationEqual = durationEqual
        Me.CoverEqual = coverEqual
        Me.ArtistEqual = artistEqual
        Me.TitleEqual = titleEqual
        Me.AlbumEqual = albumEqual
        Me.YearEqual = yearEqual
    End Sub

#End Region

End Class

#End Region
