
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Imaging

#End Region

#Region " TestImageFactory "

Namespace Infrastructure

    ''' <summary>
    ''' Creates small, deterministic images for testing purposes.
    ''' </summary>
    Friend NotInheritable Class TestImageFactory

        ''' <summary>
        ''' Prevents a default instance of the <see cref="TestImageFactory"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Creates a solid-color bitmap.
        ''' </summary>
        '''
        ''' <param name="width">
        ''' The bitmap width.
        ''' </param>
        '''
        ''' <param name="height">
        ''' The bitmap height.
        ''' </param>
        '''
        ''' <param name="color">
        ''' The fill color.
        ''' </param>
        '''
        ''' <returns>
        ''' The new bitmap. The caller is responsible for disposing it.
        ''' </returns>
        Friend Shared Function CreateSolidBitmap(width As Integer, height As Integer, color As Color) As Bitmap

            Dim bmp As New Bitmap(width, height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(color)
            End Using
            Return bmp
        End Function

        ''' <summary>
        ''' Creates the PNG data of a solid-color image.
        ''' </summary>
        '''
        ''' <param name="width">
        ''' The image width.
        ''' </param>
        '''
        ''' <param name="height">
        ''' The image height.
        ''' </param>
        '''
        ''' <param name="color">
        ''' The fill color.
        ''' </param>
        '''
        ''' <returns>
        ''' The PNG data.
        ''' </returns>
        Friend Shared Function CreateSolidPng(width As Integer, height As Integer, color As Color) As Byte()

            Using bmp As Bitmap = TestImageFactory.CreateSolidBitmap(width, height, color),
                  ms As New MemoryStream()

                bmp.Save(ms, ImageFormat.Png)
                Return ms.ToArray()
            End Using
        End Function

    End Class

End Namespace

#End Region
