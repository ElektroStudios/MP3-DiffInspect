
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Mp3InfoFileNameTests "

''' <summary>
''' Unit tests for the file-related properties of <see cref="Mp3Info"/>
''' (<see cref="Mp3Info.FileInfo"/>, <see cref="Mp3Info.Directory"/>, <see cref="Mp3Info.FileName"/>
''' and <see cref="Mp3Info.FileNameNormalized"/>).
''' <para></para>
''' These properties do not access the file, so no file is created on disk.
''' </summary>
<TestClass>
<TestCategory("Unit")>
Public NotInheritable Class Mp3InfoFileNameTests

    ''' <summary>
    ''' A rooted directory used to build file paths that do not need to exist.
    ''' </summary>
    Private Shared ReadOnly FakeDirectory As String = Path.Combine(Path.GetTempPath(), "MP3 DiffInspect.Tests", "Fake")

#Region " Constructors "

    ''' <summary>
    ''' Verifies that the string constructor builds the expected <see cref="FileInfo"/>.
    ''' </summary>
    <TestMethod>
    Public Sub Constructor_WithFilePath_SetsFileInfo()

        Dim filePath As String = Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "Song.mp3")

        Using info As New Mp3Info(filePath)
            Assert.IsNotNull(info.FileInfo)
            Assert.AreEqual(filePath, info.FileInfo.FullName)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the <see cref="FileInfo"/> constructor keeps the same instance.
    ''' </summary>
    <TestMethod>
    Public Sub Constructor_WithFileInfo_KeepsSameInstance()

        Dim fi As New FileInfo(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "Song.mp3"))

        Using info As New Mp3Info(fi)
            Assert.AreSame(fi, info.FileInfo)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that constructing an instance does not access the file (properties are lazily loaded).
    ''' </summary>
    <TestMethod>
    Public Sub Constructor_WithNonExistingFile_DoesNotThrow()

        Dim filePath As String = Path.Combine(Mp3InfoFileNameTests.FakeDirectory, $"{Guid.NewGuid():N}.mp3")

        Using info As New Mp3Info(filePath)
            Assert.IsFalse(info.FileInfo.Exists)
        End Using
    End Sub

#End Region

#Region " Directory / FileName "

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.Directory"/> returns the parent directory of the file.
    ''' </summary>
    <TestMethod>
    Public Sub Directory_ReturnsParentDirectory()

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "Song.mp3"))
            Assert.AreEqual(Mp3InfoFileNameTests.FakeDirectory, info.Directory)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.FileName"/> returns the file name including its extension.
    ''' </summary>
    <TestMethod>
    Public Sub FileName_ReturnsNameWithExtension()

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "01. My Song.mp3"))
            Assert.AreEqual("01. My Song.mp3", info.FileName)
        End Using
    End Sub

#End Region

#Region " FileNameNormalized "

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.FileNameNormalized"/> removes the track index prefix,
    ''' the file extension and any character that is not a letter or a digit.
    ''' </summary>
    '''
    ''' <param name="fileName">
    ''' The source file name.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected normalized name.
    ''' </param>
    <TestMethod>
    <DataRow("01. Song Title.mp3", "SongTitle", DisplayName:="Dot + space separator")>
    <DataRow("01 - Song Title.mp3", "SongTitle", DisplayName:="Space + hyphen + space separator")>
    <DataRow("01- Song Title.mp3", "SongTitle", DisplayName:="Hyphen + space separator")>
    <DataRow("01-Song Title.mp3", "SongTitle", DisplayName:="Hyphen separator")>
    <DataRow("01 -- Song.mp3", "Song", DisplayName:="Double hyphen separator")>
    <DataRow("05.-Song.mp3", "Song", DisplayName:="Dot + hyphen separator")>
    <DataRow("01_Song.mp3", "Song", DisplayName:="Underscore separator")>
    <DataRow("1-Song.mp3", "Song", DisplayName:="Single digit index")>
    <DataRow("123. Song.mp3", "Song", DisplayName:="Three digit index")>
    <DataRow("01. (Remix) Song!.mp3", "RemixSong", DisplayName:="Symbols are removed")>
    Public Sub FileNameNormalized_RemovesTrackIndexPrefix(fileName As String, expected As String)

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, fileName))
            Assert.AreEqual(expected, info.FileNameNormalized)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.FileNameNormalized"/> keeps leading numbers
    ''' that are not followed by a track index separator.
    ''' </summary>
    '''
    ''' <param name="fileName">
    ''' The source file name.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected normalized name.
    ''' </param>
    <TestMethod>
    <DataRow("99 Luftballons.mp3", "99Luftballons", DisplayName:="Number + space is part of the title")>
    <DataRow("2001. A Space Odyssey.mp3", "2001ASpaceOdyssey", DisplayName:="Four digit number is not an index")>
    <DataRow("1999 - Prince.mp3", "1999Prince", DisplayName:="Separator beyond the 5-char prefix window")>
    <DataRow("01.mp3", "01", DisplayName:="Index only, no separator")>
    Public Sub FileNameNormalized_KeepsNonIndexLeadingNumbers(fileName As String, expected As String)

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, fileName))
            Assert.AreEqual(expected, info.FileNameNormalized)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.FileNameNormalized"/> does not alter names without a track index prefix,
    ''' apart from removing non-alphanumeric characters.
    ''' </summary>
    '''
    ''' <param name="fileName">
    ''' The source file name.
    ''' </param>
    '''
    ''' <param name="expected">
    ''' The expected normalized name.
    ''' </param>
    <TestMethod>
    <DataRow("Artist - Title.mp3", "ArtistTitle", DisplayName:="Artist - Title")>
    <DataRow("A-ha - Take On Me.mp3", "AhaTakeOnMe", DisplayName:="Hyphen inside prefix window but not an index")>
    <DataRow("Ab.mp3", "Ab", DisplayName:="Name shorter than the prefix window")>
    <DataRow("Ñandú Canción.mp3", "ÑandúCanción", DisplayName:="Non-ASCII letters are kept")>
    <DataRow("Song.Name.With.Dots.mp3", "SongNameWithDots", DisplayName:="Only the last extension is removed")>
    Public Sub FileNameNormalized_WithoutIndexPrefix_OnlyRemovesSymbols(fileName As String, expected As String)

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, fileName))
            Assert.AreEqual(expected, info.FileNameNormalized)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.FileNameNormalized"/> is computed once and cached.
    ''' </summary>
    <TestMethod>
    Public Sub FileNameNormalized_IsCached()

        Using info As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "01. Song.mp3"))
            Dim first As String = info.FileNameNormalized
            Dim second As String = info.FileNameNormalized

            Assert.AreSame(first, second)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that two files that only differ by their track index prefix have the same normalized name.
    ''' </summary>
    <TestMethod>
    Public Sub FileNameNormalized_DifferentIndexFormats_ProduceSameValue()

        Using a As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "01. Song Title.mp3")),
              b As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "07 - Song Title.mp3")),
              c As New Mp3Info(Path.Combine(Mp3InfoFileNameTests.FakeDirectory, "Song Title.mp3"))

            Assert.AreEqual(a.FileNameNormalized, b.FileNameNormalized)
            Assert.AreEqual(a.FileNameNormalized, c.FileNameNormalized)
        End Using
    End Sub

#End Region

End Class

#End Region
