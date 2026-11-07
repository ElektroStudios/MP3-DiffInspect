
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Text

#End Region

#Region " Mp3TestFileBuilder "

Namespace Infrastructure

    ''' <summary>
    ''' Builds small, deterministic and valid MP3 files for testing purposes.
    ''' <para></para>
    ''' The audio stream is made of MPEG-1 Layer III frames (without CRC) whose payload is filled with a configurable byte,
    ''' optionally preceded by an ID3v2.3 tag and/or followed by an ID3v1 tag.
    ''' </summary>
    '''
    ''' <remarks>
    ''' The frames are never decoded by the tests (only parsed), so a synthetic payload is enough.
    ''' </remarks>
    Friend NotInheritable Class Mp3TestFileBuilder

#Region " Constants "

        ''' <summary>
        ''' The number of PCM samples contained in a single MPEG-1 Layer III frame.
        ''' </summary>
        Friend Const SamplesPerFrame As Integer = 1152

#End Region

#Region " Private Fields "

        Private frameCount As Integer = 40

        Private bitrateKbps As Integer = 128

        Private sampleRate As Integer = 44100

        Private isMono As Boolean

        Private payloadByte As Byte

        Private id3v2Artist As String

        Private id3v2Title As String

        Private id3v2Album As String

        Private id3v2Year As String

        Private id3v2CoverPng As Byte()

        Private hasId3v1 As Boolean

        Private id3v1Artist As String

        Private id3v1Title As String

        Private id3v1Album As String

        Private id3v1Year As String

#End Region

#Region " Public Methods (Fluent configuration) "

        ''' <summary>
        ''' Sets the number of audio frames to write.
        ''' </summary>
        '''
        ''' <param name="count">
        ''' The number of frames. Must be greater than zero.
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithFrameCount(count As Integer) As Mp3TestFileBuilder

            If count <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(count), count, "Frame count must be greater than zero.")
            End If

            Me.frameCount = count
            Return Me
        End Function

        ''' <summary>
        ''' Sets the bitrate of every frame.
        ''' </summary>
        '''
        ''' <param name="kbps">
        ''' The bitrate, in kbps. Supported values: 128, 160, 192.
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithBitrate(kbps As Integer) As Mp3TestFileBuilder

            Mp3TestFileBuilder.GetBitrateIndex(kbps) ' Validates the value.
            Me.bitrateKbps = kbps
            Return Me
        End Function

        ''' <summary>
        ''' Sets the sample rate of every frame.
        ''' </summary>
        '''
        ''' <param name="hz">
        ''' The sample rate, in Hz. Supported values: 44100, 48000, 32000.
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithSampleRate(hz As Integer) As Mp3TestFileBuilder

            Mp3TestFileBuilder.GetSampleRateIndex(hz) ' Validates the value.
            Me.sampleRate = hz
            Return Me
        End Function

        ''' <summary>
        ''' Sets whether the frames are mono (single channel) instead of stereo.
        ''' </summary>
        '''
        ''' <param name="mono">
        ''' <see langword="True"/> for mono, <see langword="False"/> for stereo.
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithMono(mono As Boolean) As Mp3TestFileBuilder

            Me.isMono = mono
            Return Me
        End Function

        ''' <summary>
        ''' Sets the byte used to fill the payload of every frame.
        ''' <para></para>
        ''' Two files with the same stream properties but a different payload byte have different audio data.
        ''' </summary>
        '''
        ''' <param name="value">
        ''' The payload byte.
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithPayloadByte(value As Byte) As Mp3TestFileBuilder

            Me.payloadByte = value
            Return Me
        End Function

        ''' <summary>
        ''' Adds an ID3v2.3 tag. <see langword="Nothing"/> values are not written.
        ''' </summary>
        '''
        ''' <param name="artist">
        ''' The artist (TPE1 frame).
        ''' </param>
        '''
        ''' <param name="title">
        ''' The title (TIT2 frame).
        ''' </param>
        '''
        ''' <param name="album">
        ''' The album (TALB frame).
        ''' </param>
        '''
        ''' <param name="year">
        ''' The year (TYER frame).
        ''' </param>
        '''
        ''' <param name="coverPng">
        ''' The PNG data of the front cover (APIC frame).
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithId3v2(Optional artist As String = Nothing,
                                  Optional title As String = Nothing,
                                  Optional album As String = Nothing,
                                  Optional year As String = Nothing,
                                  Optional coverPng As Byte() = Nothing) As Mp3TestFileBuilder

            Me.id3v2Artist = artist
            Me.id3v2Title = title
            Me.id3v2Album = album
            Me.id3v2Year = year
            Me.id3v2CoverPng = coverPng
            Return Me
        End Function

        ''' <summary>
        ''' Adds an ID3v1 tag. <see langword="Nothing"/> values are written as empty fields.
        ''' </summary>
        '''
        ''' <param name="artist">
        ''' The artist (max. 30 characters).
        ''' </param>
        '''
        ''' <param name="title">
        ''' The title (max. 30 characters).
        ''' </param>
        '''
        ''' <param name="album">
        ''' The album (max. 30 characters).
        ''' </param>
        '''
        ''' <param name="year">
        ''' The year (4 characters).
        ''' </param>
        '''
        ''' <returns>
        ''' This builder instance.
        ''' </returns>
        Friend Function WithId3v1(Optional artist As String = Nothing,
                                  Optional title As String = Nothing,
                                  Optional album As String = Nothing,
                                  Optional year As String = Nothing) As Mp3TestFileBuilder

            Me.hasId3v1 = True
            Me.id3v1Artist = artist
            Me.id3v1Title = title
            Me.id3v1Album = album
            Me.id3v1Year = year
            Return Me
        End Function

#End Region

#Region " Public Methods (Output) "

        ''' <summary>
        ''' Gets the expected duration of the audio stream that this builder produces.
        ''' </summary>
        '''
        ''' <returns>
        ''' The expected duration.
        ''' </returns>
        Friend Function GetExpectedDuration() As TimeSpan

            Dim totalSamples As Long = CLng(Me.frameCount) * Mp3TestFileBuilder.SamplesPerFrame
            Return TimeSpan.FromTicks(CLng(Math.Round(totalSamples * TimeSpan.TicksPerSecond / CDbl(Me.sampleRate))))
        End Function

        ''' <summary>
        ''' Builds the raw bytes of the audio stream only (no tags).
        ''' </summary>
        '''
        ''' <returns>
        ''' The raw audio stream bytes.
        ''' </returns>
        Friend Function BuildAudioStream() As Byte()

            Using ms As New MemoryStream()
                Dim frame As Byte() = Me.BuildFrame()
                For i As Integer = 1 To Me.frameCount
                    ms.Write(frame, 0, frame.Length)
                Next
                Return ms.ToArray()
            End Using
        End Function

        ''' <summary>
        ''' Builds the complete MP3 file and writes it to disk.
        ''' </summary>
        '''
        ''' <param name="filePath">
        ''' The destination file path.
        ''' </param>
        '''
        ''' <returns>
        ''' The destination file path.
        ''' </returns>
        Friend Function Build(filePath As String) As String

            If String.IsNullOrEmpty(filePath) Then
                Throw New ArgumentNullException(NameOf(filePath))
            End If

            Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)
                Dim id3v2 As Byte() = Me.BuildId3v2Tag()
                fs.Write(id3v2, 0, id3v2.Length)

                Dim audio As Byte() = Me.BuildAudioStream()
                fs.Write(audio, 0, audio.Length)

                Dim id3v1 As Byte() = Me.BuildId3v1Tag()
                fs.Write(id3v1, 0, id3v1.Length)
            End Using

            Return filePath
        End Function

#End Region

#Region " Private Methods (Audio) "

        ''' <summary>
        ''' Builds a single MPEG-1 Layer III frame.
        ''' </summary>
        '''
        ''' <returns>
        ''' The raw frame bytes.
        ''' </returns>
        Private Function BuildFrame() As Byte()

            ' Frame length (no padding) = 144 * bitrate / sampleRate
            Dim frameLength As Integer = (144 * Me.bitrateKbps * 1000) \ Me.sampleRate
            Dim frame(frameLength - 1) As Byte

            ' Header:
            ' AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM
            ' A = sync (11 bits set), B = 11 (MPEG-1), C = 01 (Layer III), D = 1 (no CRC)
            ' E = bitrate index, F = sample rate index, G = padding (0), H = private (0)
            ' I = channel mode (00 = stereo, 11 = mono), J = mode extension, K = copyright, L = original, M = emphasis
            frame(0) = &HFF
            frame(1) = &HFB
            frame(2) = CByte((Mp3TestFileBuilder.GetBitrateIndex(Me.bitrateKbps) << 4) Or
                             (Mp3TestFileBuilder.GetSampleRateIndex(Me.sampleRate) << 2))
            frame(3) = If(Me.isMono, CByte(&HC0), CByte(&H0))

            ' Side information is left zeroed; only the main data area is filled with the payload byte.
            Dim sideInfoLength As Integer = If(Me.isMono, 17, 32)
            For i As Integer = 4 + sideInfoLength To frameLength - 1
                frame(i) = Me.payloadByte
            Next

            Return frame
        End Function

        ''' <summary>
        ''' Gets the MPEG-1 Layer III bitrate index for the specified bitrate.
        ''' </summary>
        '''
        ''' <param name="kbps">
        ''' The bitrate, in kbps.
        ''' </param>
        '''
        ''' <returns>
        ''' The bitrate index.
        ''' </returns>
        Private Shared Function GetBitrateIndex(kbps As Integer) As Integer

            Select Case kbps
                Case 128 : Return 9
                Case 160 : Return 10
                Case 192 : Return 11
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(kbps), kbps, "Unsupported bitrate.")
            End Select
        End Function

        ''' <summary>
        ''' Gets the MPEG-1 sample rate index for the specified sample rate.
        ''' </summary>
        '''
        ''' <param name="hz">
        ''' The sample rate, in Hz.
        ''' </param>
        '''
        ''' <returns>
        ''' The sample rate index.
        ''' </returns>
        Private Shared Function GetSampleRateIndex(hz As Integer) As Integer

            Select Case hz
                Case 44100 : Return 0
                Case 48000 : Return 1
                Case 32000 : Return 2
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(hz), hz, "Unsupported sample rate.")
            End Select
        End Function

#End Region

#Region " Private Methods (Tags) "

        ''' <summary>
        ''' Builds the ID3v2.3 tag, or an empty array if no ID3v2 field was configured.
        ''' </summary>
        '''
        ''' <returns>
        ''' The raw tag bytes.
        ''' </returns>
        Private Function BuildId3v2Tag() As Byte()

            Using frames As New MemoryStream()
                Mp3TestFileBuilder.WriteTextFrame(frames, "TPE1", Me.id3v2Artist)
                Mp3TestFileBuilder.WriteTextFrame(frames, "TIT2", Me.id3v2Title)
                Mp3TestFileBuilder.WriteTextFrame(frames, "TALB", Me.id3v2Album)
                Mp3TestFileBuilder.WriteTextFrame(frames, "TYER", Me.id3v2Year)
                Mp3TestFileBuilder.WritePictureFrame(frames, Me.id3v2CoverPng)

                If frames.Length = 0 Then
                    Return Array.Empty(Of Byte)()
                End If

                Using tag As New MemoryStream()
                    tag.Write(Encoding.ASCII.GetBytes("ID3"), 0, 3)
                    tag.WriteByte(3) ' Major version (2.3)
                    tag.WriteByte(0) ' Revision
                    tag.WriteByte(0) ' Flags

                    Dim size As Byte() = Mp3TestFileBuilder.ToSynchsafe(CInt(frames.Length))
                    tag.Write(size, 0, size.Length)

                    frames.Position = 0
                    frames.CopyTo(tag)
                    Return tag.ToArray()
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Builds the 128-byte ID3v1 tag, or an empty array if no ID3v1 tag was configured.
        ''' </summary>
        '''
        ''' <returns>
        ''' The raw tag bytes.
        ''' </returns>
        Private Function BuildId3v1Tag() As Byte()

            If Not Me.hasId3v1 Then
                Return Array.Empty(Of Byte)()
            End If

            Dim tag(127) As Byte
            Encoding.ASCII.GetBytes("TAG").CopyTo(tag, 0)
            Mp3TestFileBuilder.WriteFixedField(tag, 3, 30, Me.id3v1Title)
            Mp3TestFileBuilder.WriteFixedField(tag, 33, 30, Me.id3v1Artist)
            Mp3TestFileBuilder.WriteFixedField(tag, 63, 30, Me.id3v1Album)
            Mp3TestFileBuilder.WriteFixedField(tag, 93, 4, Me.id3v1Year)
            ' Comment (97-126) left empty.
            tag(127) = 255 ' Genre: none.
            Return tag
        End Function

        ''' <summary>
        ''' Writes an ID3v2.3 text frame (ISO-8859-1 encoded) if <paramref name="value"/> is not <see langword="Nothing"/>.
        ''' </summary>
        '''
        ''' <param name="stream">
        ''' The destination stream.
        ''' </param>
        '''
        ''' <param name="frameId">
        ''' The 4-character frame identifier.
        ''' </param>
        '''
        ''' <param name="value">
        ''' The text value.
        ''' </param>
        Private Shared Sub WriteTextFrame(stream As Stream, frameId As String, value As String)

            If value Is Nothing Then
                Exit Sub
            End If

            Dim text As Byte() = Encoding.GetEncoding("ISO-8859-1").GetBytes(value)
            Dim content(text.Length) As Byte
            content(0) = 0 ' Text encoding: ISO-8859-1
            text.CopyTo(content, 1)

            Mp3TestFileBuilder.WriteFrame(stream, frameId, content)
        End Sub

        ''' <summary>
        ''' Writes an ID3v2.3 APIC (front cover) frame if <paramref name="png"/> is not <see langword="Nothing"/>.
        ''' </summary>
        '''
        ''' <param name="stream">
        ''' The destination stream.
        ''' </param>
        '''
        ''' <param name="png">
        ''' The PNG image data.
        ''' </param>
        Private Shared Sub WritePictureFrame(stream As Stream, png As Byte())

            If png Is Nothing Then
                Exit Sub
            End If

            Using content As New MemoryStream()
                content.WriteByte(0) ' Text encoding: ISO-8859-1
                Dim mime As Byte() = Encoding.ASCII.GetBytes("image/png")
                content.Write(mime, 0, mime.Length)
                content.WriteByte(0) ' MIME terminator
                content.WriteByte(3) ' Picture type: front cover
                content.WriteByte(0) ' Empty description terminator
                content.Write(png, 0, png.Length)

                Mp3TestFileBuilder.WriteFrame(stream, "APIC", content.ToArray())
            End Using
        End Sub

        ''' <summary>
        ''' Writes a raw ID3v2.3 frame (10-byte header followed by its content).
        ''' </summary>
        '''
        ''' <param name="stream">
        ''' The destination stream.
        ''' </param>
        '''
        ''' <param name="frameId">
        ''' The 4-character frame identifier.
        ''' </param>
        '''
        ''' <param name="content">
        ''' The frame content.
        ''' </param>
        Private Shared Sub WriteFrame(stream As Stream, frameId As String, content As Byte())

            stream.Write(Encoding.ASCII.GetBytes(frameId), 0, 4)

            ' ID3v2.3 frame sizes are plain 32-bit big-endian integers (not synchsafe).
            Dim size As Integer = content.Length
            stream.WriteByte(CByte((size >> 24) And &HFF))
            stream.WriteByte(CByte((size >> 16) And &HFF))
            stream.WriteByte(CByte((size >> 8) And &HFF))
            stream.WriteByte(CByte(size And &HFF))

            stream.WriteByte(0) ' Flags
            stream.WriteByte(0) ' Flags
            stream.Write(content, 0, content.Length)
        End Sub

        ''' <summary>
        ''' Writes a fixed-length, zero-padded ASCII field into an ID3v1 tag buffer.
        ''' </summary>
        '''
        ''' <param name="buffer">
        ''' The tag buffer.
        ''' </param>
        '''
        ''' <param name="offset">
        ''' The field offset.
        ''' </param>
        '''
        ''' <param name="length">
        ''' The field length.
        ''' </param>
        '''
        ''' <param name="value">
        ''' The field value.
        ''' </param>
        Private Shared Sub WriteFixedField(buffer As Byte(), offset As Integer, length As Integer, value As String)

            If String.IsNullOrEmpty(value) Then
                Exit Sub
            End If

            If value.Length > length Then
                Throw New ArgumentOutOfRangeException(NameOf(value), value, $"ID3v1 field exceeds {length} characters.")
            End If

            Encoding.ASCII.GetBytes(value).CopyTo(buffer, offset)
        End Sub

        ''' <summary>
        ''' Encodes an integer as a 4-byte ID3v2 synchsafe integer.
        ''' </summary>
        '''
        ''' <param name="value">
        ''' The value to encode.
        ''' </param>
        '''
        ''' <returns>
        ''' The synchsafe bytes.
        ''' </returns>
        Private Shared Function ToSynchsafe(value As Integer) As Byte()

            Return {CByte((value >> 21) And &H7F),
                    CByte((value >> 14) And &H7F),
                    CByte((value >> 7) And &H7F),
                    CByte(value And &H7F)}
        End Function

#End Region

    End Class

End Namespace

#End Region
