
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports MP3DiffInspect.Tests.Infrastructure

#End Region

#Region " Mp3InfoTagPropertiesTests "

''' <summary>
''' Integration tests for the metadata tag properties of <see cref="Mp3Info"/>
''' (<see cref="Mp3Info.Artist"/>, <see cref="Mp3Info.Title"/>, <see cref="Mp3Info.Album"/>,
''' <see cref="Mp3Info.Year"/> and <see cref="Mp3Info.Cover"/>), and of its disposal.
''' </summary>
<TestClass>
<TestCategory("Integration")>
Public NotInheritable Class Mp3InfoTagPropertiesTests : Inherits Mp3FileTestBase

#Region " ID3v2 / ID3v1 "

    ''' <summary>
    ''' Verifies that the ID3v2 tag values are returned.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_FromId3v2_AreReturned()

        Dim filePath As String = Me.CreateMp3File("v2.mp3", New Mp3TestFileBuilder().
                                                                WithId3v2("The Artist", "The Title", "The Album", "2001"))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("The Artist", info.Artist)
            Assert.AreEqual("The Title", info.Title)
            Assert.AreEqual("The Album", info.Album)
            Assert.AreEqual(2001, info.Year)
            Assert.AreEqual("2001", info.YearFormated)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the ID3v1 tag values are returned when there is no ID3v2 tag.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_OnlyId3v1_AreReturned()

        Dim filePath As String = Me.CreateMp3File("v1.mp3", New Mp3TestFileBuilder().
                                                                WithId3v1("V1 Artist", "V1 Title", "V1 Album", "1999"))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("V1 Artist", info.Artist)
            Assert.AreEqual("V1 Title", info.Title)
            Assert.AreEqual("V1 Album", info.Album)
            Assert.AreEqual(1999, info.Year)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the ID3v2 tag takes precedence over the ID3v1 tag.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_Id3v2AndId3v1_Id3v2TakesPrecedence()

        Dim filePath As String = Me.CreateMp3File("both.mp3", New Mp3TestFileBuilder().
                                                                  WithId3v2("V2 Artist", "V2 Title", "V2 Album", "2010").
                                                                  WithId3v1("V1 Artist", "V1 Title", "V1 Album", "1990"))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("V2 Artist", info.Artist)
            Assert.AreEqual("V2 Title", info.Title)
            Assert.AreEqual("V2 Album", info.Album)
            Assert.AreEqual(2010, info.Year)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the fields missing in the ID3v2 tag are filled from the ID3v1 tag.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_PartialId3v2_MissingFieldsFallBackToId3v1()

        Dim filePath As String = Me.CreateMp3File("partial.mp3", New Mp3TestFileBuilder().
                                                                     WithId3v2(title:="V2 Title").
                                                                     WithId3v1("V1 Artist", "V1 Title", "V1 Album", "1985"))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("V1 Artist", info.Artist)
            Assert.AreEqual("V2 Title", info.Title)
            Assert.AreEqual("V1 Album", info.Album)
            Assert.AreEqual(1985, info.Year)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies the "N/A" placeholders and default values when the file has no tags at all.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_NoTags_ReturnPlaceholders()

        Dim filePath As String = Me.CreateMp3File("notags.mp3", New Mp3TestFileBuilder())

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("N/A", info.Artist)
            Assert.AreEqual("N/A", info.Title)
            Assert.AreEqual("N/A", info.Album)
            Assert.AreEqual(0, info.Year)
            Assert.AreEqual("N/A", info.YearFormated)
            Assert.IsNull(info.Cover)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that non-ASCII (ISO-8859-1) characters are read correctly.
    ''' </summary>
    <TestMethod>
    Public Sub Tags_Latin1Characters_AreReadCorrectly()

        Dim filePath As String = Me.CreateMp3File("latin1.mp3", New Mp3TestFileBuilder().
                                                                    WithId3v2("Mägo de Oz", "Fiesta Pagana", "Finisterra", "2000"))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("Mägo de Oz", info.Artist)
        End Using
    End Sub

#End Region

#Region " Cover "

    ''' <summary>
    ''' Verifies that the embedded front cover is loaded as an image with the same size and pixels.
    ''' </summary>
    <TestMethod>
    Public Sub Cover_EmbeddedPicture_IsLoaded()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(24, 12, Color.Blue)
        Dim filePath As String = Me.CreateMp3File("cover.mp3", New Mp3TestFileBuilder().WithId3v2(title:="Cover", coverPng:=png))

        Using info As New Mp3Info(filePath)
            Dim cover As Image = info.Cover

            Assert.IsNotNull(cover)
            Assert.AreEqual(24, cover.Width)
            Assert.AreEqual(12, cover.Height)

            Dim pixel As Color = DirectCast(cover, Bitmap).GetPixel(5, 5)
            Assert.AreEqual(Color.Blue.ToArgb(), pixel.ToArgb())
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.Cover"/> is loaded once and cached.
    ''' </summary>
    <TestMethod>
    Public Sub Cover_IsCached()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(8, 8, Color.Green)
        Dim filePath As String = Me.CreateMp3File("cover.mp3", New Mp3TestFileBuilder().WithId3v2(title:="Cover", coverPng:=png))

        Using info As New Mp3Info(filePath)
            Dim first As Image = info.Cover
            Dim second As Image = info.Cover

            Assert.IsNotNull(first)
            Assert.AreSame(first, second)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the cover image does not depend on the source stream (it remains usable afterwards).
    ''' </summary>
    <TestMethod>
    Public Sub Cover_RemainsUsableAfterLoading()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(10, 10, Color.Orange)
        Dim filePath As String = Me.CreateMp3File("cover.mp3", New Mp3TestFileBuilder().WithId3v2(title:="Cover", coverPng:=png))

        Using info As New Mp3Info(filePath)
            Dim cover As Image = info.Cover

            Using ms As New MemoryStream()
                cover.Save(ms, Imaging.ImageFormat.Png)
                Assert.IsGreaterThan(0L, ms.Length)
            End Using
        End Using
    End Sub

#End Region

#Region " Dispose "

    ''' <summary>
    ''' Verifies that disposing the instance disposes the cover image.
    ''' </summary>
    <TestMethod>
    Public Sub Dispose_DisposesCoverImage()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(8, 8, Color.Red)
        Dim filePath As String = Me.CreateMp3File("cover.mp3", New Mp3TestFileBuilder().WithId3v2(title:="Cover", coverPng:=png))

        Dim info As New Mp3Info(filePath)
        Dim cover As Image = info.Cover
        Assert.IsNotNull(cover)

        info.Dispose()

        ' Accessing the size of a disposed GDI+ image throws ArgumentException.
        Assert.ThrowsExactly(Of ArgumentException)(Function() cover.Width)
    End Sub

    ''' <summary>
    ''' Verifies that calling <see cref="Mp3Info.Dispose()"/> more than once does not throw.
    ''' </summary>
    <TestMethod>
    Public Sub Dispose_CalledTwice_DoesNotThrow()

        Dim filePath As String = Me.CreateMp3File("twice.mp3", New Mp3TestFileBuilder().WithId3v2("A", "T", "B", "2000"))

        Dim info As New Mp3Info(filePath)
        Dim artist As String = info.Artist
        Assert.AreEqual("A", artist)

        info.Dispose()
        info.Dispose()
    End Sub

    ''' <summary>
    ''' Verifies that disposing an instance whose properties were never read does not access the file.
    ''' </summary>
    <TestMethod>
    Public Sub Dispose_WithoutReadingProperties_DoesNotThrow()

        Dim info As New Mp3Info(Me.GetTempFilePath("never-created.mp3"))
        info.Dispose()
    End Sub

#End Region

End Class

#End Region
