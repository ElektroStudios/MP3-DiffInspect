
#Region " Option Statements "

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks

#End Region

#Region " DevFloatingPicture "

' ReSharper disable once CheckNamespace

Namespace DevCase.UI.Components

    ''' <summary>
    ''' A control that displays a picture in a stylish way that is similar to Telegram client.
    ''' </summary>
    '''
    ''' <example> This is a code example.
    ''' <code language="VB.NET">
    ''' Dim imagePath As String = "C:\Image.jpg"
    ''' Dim img As Image = Image.FromFile(imagePath)
    ''' 
    ''' Using fpic As New DevFloatingPicture(img) With {
    '''         .ImageLayout = ImageLayout.Zoom,
    '''         .BackgroundColor = Color.Black,
    '''         .BackgroundOpacity = 0.75,
    '''         .FitBoundsToWorkingArea = True,
    '''         .TopMost = True,
    '''         .TitleBar = False,
    '''         .ImageBorder = True,
    '''         .ImageBorderColor = Color.FromArgb(255, 30, 30, 30),
    '''         .ImageBorderSize = 5,
    '''         .CloseOnEscapeKey = True,
    '''         .CloseOnLeftMouseClick = False
    '''     }
    ''' 
    '''     fpic.ShowDialog()
    ''' End Using
    ''' </code>
    ''' </example>
    <ToolboxItem(True)>
    <DesignerCategory(NameOf(DesignerCategoryAttribute.Component))>
    <DisplayName(NameOf(DevFloatingPicture))>
    <Description("A control that displays a picture in a way that is similar to Telegram client.")>
    <DesignTimeVisible(True)>
    <ToolboxBitmap(GetType(PictureBox), "PictureBox.bmp")>
    <ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Allow)>
    <ComVisible(True)>
    <DefaultBindingProperty(NameOf(DevFloatingPicture.Image))>
    <DefaultProperty(NameOf(DevFloatingPicture.Image))>
    Public Class DevFloatingPicture : Inherits Component

#Region " Private Fields "

        ''' <summary>
        ''' The background <see cref="Form"/> where <see cref="DevFloatingPicture.FrontForm"/> is hosted.
        ''' </summary>
        Protected WithEvents BackForm As Form

        ''' <summary>
        ''' The <see cref="Form"/> where the picture is shown.
        ''' </summary>
        Protected WithEvents FrontForm As Form

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the image to show in this control.
        ''' </summary>
        Public Property Image As Image
            Get
                Return Me.image_
            End Get
            Set(value As Image)
                Me.image_ = value
                Me.FrontForm.BackgroundImage = value
            End Set
        End Property

        ''' <summary>
        ''' ( Backing Field )
        ''' <para></para>
        ''' The image to show in this control.
        ''' </summary>
        Private image_ As Image

        ''' <summary>
        ''' Gets or sets the image layout.
        ''' <para></para>
        ''' Default value is <see cref="ImageLayout.Zoom"/>
        ''' </summary>
        Public Property ImageLayout As ImageLayout
            Get
                Return Me.FrontForm.BackgroundImageLayout
            End Get
            Set(value As ImageLayout)
                Me.FrontForm.BackgroundImageLayout = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether to fit the control bounds to the desktop working area.
        ''' <para></para>
        ''' if this value is False, the control can overlap the desktop taskbar bounds. 
        ''' <para></para>
        ''' Default value is False.
        ''' </summary>
        Public Property FitBoundsToWorkingArea As Boolean
            Get
                Return Me.fitBoundsToWorkingArea_
            End Get
            Set(value As Boolean)
                If Me.fitBoundsToWorkingArea_ = value Then
                    Exit Property
                End If
                Me.fitBoundsToWorkingArea_ = value
                Me.AdjustFormSizes()
            End Set
        End Property

        ''' <summary>
        ''' ( Backing Field )
        ''' <para></para>
        ''' A value indicating whether to fit the control bounds to the desktop working area.
        ''' </summary>
        Private fitBoundsToWorkingArea_ As Boolean

        ''' <summary>
        ''' Gets or sets the context menu for this control.
        ''' <para></para>
        ''' Note: the default context menu has a single command named "Close Picture".
        ''' </summary>
        Public Property ContextMenuStrip As ContextMenuStrip
            Get
                Return Me.contextMenuStrip_
            End Get
            Set(value As ContextMenuStrip)
                If Not Me.contextMenuStrip_.Equals(value) Then
                    Me.contextMenuStrip_?.Dispose()
                    Me.contextMenuStrip_ = value
                    Me.BackForm.ContextMenuStrip = value
                    Me.FrontForm.ContextMenuStrip = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' ( Backing Field )
        ''' <para></para>
        ''' The context menu for this control.
        ''' </summary>
        Private contextMenuStrip_ As ContextMenuStrip

        ''' <summary>
        ''' Gets or sets a value indicating whether the control has a title bar.
        ''' <para></para>
        ''' Default value is False.
        ''' </summary>
        Public Property TitleBar As Boolean
            Get
                Return Me.FrontForm.FormBorderStyle = FormBorderStyle.FixedSingle
            End Get
            Set(value As Boolean)
                Me.FrontForm.FormBorderStyle = If(value, FormBorderStyle.FixedSingle, FormBorderStyle.None)
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the control should be displayed as a topmost cotrol.
        ''' <para></para>
        ''' Default value is False.
        ''' </summary>
        Public Property TopMost As Boolean
            Get
                Return Me.FrontForm.TopMost
            End Get
            Set(value As Boolean)
                Me.BackForm.TopMost = value
                Me.FrontForm.TopMost = value
                If value Then
                    Me.FrontForm.BringToFront()
                    Me.FrontForm.Activate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the background color of the control.
        ''' <para></para>
        ''' Default value is <see cref="Color.Black"/>
        ''' </summary>
        Public Property BackgroundColor As Color
            Get
                Return Me.BackForm.BackColor
            End Get
            Set(value As Color)
                Me.BackForm.BackColor = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the background opacity of the control. This value does not affect the image opacity.
        ''' <para></para>
        ''' Default value is 0.75
        ''' </summary>
        Public Property BackgroundOpacity As Double
            Get
                Return Me.BackForm.Opacity
            End Get
            Set(value As Double)
                Me.BackForm.Opacity = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the color that will represent transparent areas of the control 
        ''' (see: <see cref="Form.TransparencyKey"/>).
        ''' <para></para>
        ''' Default value is <see cref="Color.Fuchsia"/>
        ''' </summary>
        Public Property TransparencyKey As Color
            Get
                Return Me.FrontForm.TransparencyKey
            End Get
            Set(value As Color)
                Me.FrontForm.TransparencyKey = value
                Me.FrontForm.BackColor = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the control should be closed 
        ''' when pressing Escape key.
        ''' <para></para>
        ''' Default value is True.
        ''' </summary>
        Public Property CloseOnEscapeKey As Boolean = True

        ''' <summary>
        ''' Gets or sets a value indicating whether the control should be closed 
        ''' when doing a left mouse button click on it.
        ''' <para></para>
        ''' Default value is True.
        ''' </summary>
        Public Property CloseOnLeftMouseClick As Boolean = True

        ''' <summary>
        ''' Gets or sets a value indicating whether to draw a border around the image.
        ''' <para></para>
        ''' Default value is False.
        ''' </summary>
        Public Property ImageBorder As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating the size of the border drawn around the image.
        ''' <para></para>
        ''' Default value is 2.
        ''' </summary>
        Public Property ImageBorderSize As Integer = 2

        ''' <summary>
        ''' Gets or sets a value indicating the color of the border drawn around the image.
        ''' <para></para>
        ''' Default value is <see cref="Color.Black"/>.
        ''' </summary>
        Public Property ImageBorderColor As Color = Color.Black

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DevFloatingPicture"/> class.
        ''' </summary>
        <DebuggerStepThrough>
        Public Sub New()
            Me.New(img:=Nothing)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DevFloatingPicture"/> class.
        ''' </summary>
        '''
        ''' <param name="img">
        ''' The image to show in this control.
        ''' </param>
        Public Sub New(img As Image)

            Me.image_ = img

            Me.BackForm = New Form With {
                .Visible = False,
                .IsMdiContainer = False,
                .FormBorderStyle = FormBorderStyle.None,
                .MaximizeBox = False,
                .MinimizeBox = False,
                .BackColor = Color.Black,
                .Opacity = 0.75,
                .WindowState = FormWindowState.Normal,
                .ShowInTaskbar = False,
                .TabStop = False,
                .TopMost = False,
                .CausesValidation = False,
                .ControlBox = False,
                .Margin = Padding.Empty,
                .Padding = Padding.Empty,
                .SizeGripStyle = SizeGripStyle.Hide,
                .StartPosition = FormStartPosition.CenterScreen,
                .AutoScaleMode = AutoScaleMode.None,
                .BackgroundImageLayout = ImageLayout.None,
                .ShowIcon = False,
                .ClientSize = New Size(0, 0),
                .Size = New Size(0, 0)
            }

            Me.FrontForm = New Form With {
                .Visible = False,
                .Owner = Me.BackForm,
                .Dock = DockStyle.Fill,
                .FormBorderStyle = FormBorderStyle.None,
                .MaximizeBox = False,
                .MinimizeBox = False,
                .Opacity = 1,
                .ShowIcon = False,
                .WindowState = FormWindowState.Normal,
                .BackgroundImage = Me.image_,
                .BackgroundImageLayout = ImageLayout.Zoom,
                .BackColor = Color.Fuchsia,
                .TransparencyKey = Color.Fuchsia,
                .ShowInTaskbar = False,
                .TabStop = False,
                .TopMost = False,
                .CausesValidation = False,
                .ControlBox = True,
                .Margin = Padding.Empty,
                .Padding = Padding.Empty,
                .SizeGripStyle = SizeGripStyle.Hide,
                .StartPosition = FormStartPosition.CenterScreen,
                .ClientSize = New Size(0, 0),
                .Size = New Size(0, 0)
            }

            Me.contextMenuStrip_ = New ContextMenuStrip()
            Me.contextMenuStrip_.Items.Add("&Close Picture", SystemIcons.Error.ToBitmap(), Sub() Me.FrontForm?.Close())

            Me.BackForm.ContextMenuStrip = Me.contextMenuStrip_
            Me.FrontForm.ContextMenuStrip = Me.contextMenuStrip_

            GC.KeepAlive(Me.FrontForm.Handle)
            GC.KeepAlive(Me.BackForm.Handle)

            Dim doubleBufferProperty As PropertyInfo = GetType(Form).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            doubleBufferProperty.SetValue(Me.FrontForm, True)

            'Application.DoEvents()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Diplays the picture.
        ''' </summary>
        Public Sub Show(Optional owner As IWin32Window = Nothing)

            Me.ShowWithoutFlickering(dialogModal:=False, owner)

            ' Previous Methodology:
            ' ---------------------
            'Me.AdjustFormSizes()
            'Me.BackForm.Show()
            'Me.FrontForm.Show()
        End Sub

        ''' <summary>
        ''' Diplays the picture as a modal dialog box (i.e. it blocks execution until the control is closed).
        ''' </summary>
        Public Function ShowDialog(Optional owner As IWin32Window = Nothing) As DialogResult

            Return Me.ShowWithoutFlickering(dialogModal:=True, owner)

            ' Previous Methodology:
            ' ---------------------
            'Me.AdjustFormSizes()
            'Me.BackForm.Show()
            'Me.FrontForm.ShowDialog()

            ' Previous Methodology (older):
            ' -----------------------------
            'Dim formClosed As Boolean
            'AddHandler Me.BackForm.FormClosed, Sub() formClosed = True
            'AddHandler Me.FrontForm.FormClosed, Sub() formClosed = True
            'Me.Show()

            'While Not formClosed
            '    Thread.Sleep(100)
            '    Application.DoEvents()
            '    ' DevCase.Core.Application.Forms.UtilForms.DoEventsSafe()
            'End While
        End Function

        ''' <summary>
        ''' Close the control.
        ''' </summary>
        Public Sub Close()
            Me.FrontForm.Close()
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Adjusts the <see cref="BackForm"/> and <see cref="FrontForm"/> sizes to the screen bounds.
        ''' </summary>
        Protected Overridable Sub AdjustFormSizes()

            Dim screen As Screen = Screen.FromControl(Me.FrontForm)
            Dim workingArea As Rectangle = If(Me.fitBoundsToWorkingArea_, screen.WorkingArea, screen.Bounds)

            Me.BackForm.Location = workingArea.Location
            Me.BackForm.Size = workingArea.Size

            Me.FrontForm.Location = workingArea.Location
            Me.FrontForm.Size = workingArea.Size

            Application.DoEvents()

        End Sub

#End Region

#Region " Event-Handlers "

        ''' <summary>
        ''' Handles the <see cref="Form.FormClosed"/> event of 
        ''' the <see cref="DevFloatingPicture.BackForm"/> 
        ''' and <see cref="DevFloatingPicture.FrontForm"/> controls.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="FormClosedEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub Forms_FormClosed(sender As Object, e As FormClosedEventArgs) Handles BackForm.FormClosed,
                                                                                                       FrontForm.FormClosed

            Me.BackForm?.Dispose()
            Me.FrontForm?.Dispose()

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.KeyPress"/> event of 
        ''' the <see cref="DevFloatingPicture.BackForm"/> 
        ''' and <see cref="DevFloatingPicture.FrontForm"/> controls.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="KeyPressEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub Forms_KeyPress(sender As Object, e As KeyPressEventArgs) Handles BackForm.KeyPress,
                                                                                                   FrontForm.KeyPress

            If e.KeyChar = Convert.ToChar(Keys.Escape) AndAlso Me.CloseOnEscapeKey Then
                DirectCast(sender, Form).Close()
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.MouseClick"/> event of 
        ''' the <see cref="DevFloatingPicture.BackForm"/> 
        ''' and <see cref="DevFloatingPicture.FrontForm"/> controls.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="MouseEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub Forms_MouseClick(sender As Object, e As MouseEventArgs) Handles BackForm.MouseClick,
                                                                                                  FrontForm.MouseClick

            If e.Button = MouseButtons.Left AndAlso Me.CloseOnLeftMouseClick Then
                DirectCast(sender, Form).Close()
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.GotFocus"/> event of the <see cref="DevFloatingPicture.BackForm"/> control.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub BackForm_GotFocus(sender As Object, e As EventArgs) Handles BackForm.GotFocus

            If Not Me.CloseOnLeftMouseClick Then
                Me.FrontForm?.Focus()
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.MouseDown"/> event of the <see cref="DevFloatingPicture.BackForm"/> control.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="MouseEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub BackForm_MouseDown(sender As Object, e As MouseEventArgs) Handles BackForm.MouseDown

            If Not Me.CloseOnLeftMouseClick Then
                Me.FrontForm?.Focus()
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.MouseHover"/> event of the <see cref="DevFloatingPicture.BackForm"/> control.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub BackForm_MouseHover(sender As Object, e As EventArgs) Handles BackForm.MouseHover

            If Not Me.CloseOnLeftMouseClick Then
                Me.FrontForm?.Focus()
            End If

        End Sub

        ''' <summary>
        ''' Handles the <see cref="Form.Paint"/> event of the <see cref="DevFloatingPicture.FrontForm"/> control.
        ''' </summary>
        '''
        ''' <param name="sender">
        ''' The source of the event.
        ''' </param>
        ''' 
        ''' <param name="e">
        ''' The <see cref="PaintEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub FrontForm_Paint(sender As Object, e As PaintEventArgs) Handles FrontForm.Paint

            If Not Me.ImageBorder OrElse Me.ImageBorderSize <= 0 Then
                Exit Sub
            End If

            If Me.DesignMode Then
                Exit Sub
            End If

            Dim imageRect As Rectangle

            Select Case Me.FrontForm.BackgroundImageLayout

                Case ImageLayout.Stretch, ImageLayout.Tile
                    imageRect = Me.FrontForm.ClientRectangle

                Case ImageLayout.Center
                    imageRect = New Rectangle(
                        CInt((Me.FrontForm.ClientSize.Width - Me.Image.Width) / 2),
                        CInt((Me.FrontForm.ClientSize.Height - Me.Image.Height) / 2),
                        Me.Image.Width, Me.Image.Height
                    )

                Case ImageLayout.Zoom
                    Dim imageSize As Size = Me.Image.Size
                    Dim clientSize As Size = Me.FrontForm.ClientSize
                    Dim aspectRatio As Double = imageSize.Width / imageSize.Height

                    If clientSize.Width / aspectRatio <= clientSize.Height Then
                        ' La imagen se ajusta en anchura
                        imageRect.Width = clientSize.Width
                        imageRect.Height = CInt(clientSize.Width / aspectRatio)
                    Else
                        ' La imagen se ajusta en altura
                        imageRect.Width = CInt(clientSize.Height * aspectRatio)
                        imageRect.Height = clientSize.Height
                    End If

                    imageRect.X = CInt((clientSize.Width - imageRect.Width) / 2)
                    imageRect.Y = CInt((clientSize.Height - imageRect.Height) / 2)

                Case Else ' ImageLayout.None
                    imageRect = New Rectangle(0, 0, Me.Image.Width, Me.Image.Height)

            End Select

            Using pen As New Pen(Me.ImageBorderColor, Me.ImageBorderSize)
                e.Graphics.DrawRectangle(pen, imageRect)
            End Using

        End Sub

#End Region

#Region " Private Methods "

        Private Function ShowWithoutFlickering(dialogModal As Boolean, Optional owner As IWin32Window = Nothing) As DialogResult

            Me.AdjustFormSizes()

            Dim previousBackFormOpacity As Double = Me.BackForm.Opacity
            Me.BackForm.Opacity = 0
            Me.FrontForm.Opacity = 0

            Dim previousBackFormLocation As Point = Me.BackForm.Location
            Dim previousFrontFormLocation As Point = Me.FrontForm.Location
            Me.BackForm.Location = New Point(-10000, -10000)
            Me.FrontForm.Location = New Point(-10000, -10000)

            Me.BackForm.Show()

            Dim antiFlickerHandler As EventHandler =
                Sub(sender As Object, e As EventArgs)
                    RemoveHandler Me.FrontForm.Shown, antiFlickerHandler

                    Task.Factory.StartNew(
                        Sub()
                            Thread.Sleep(20)
                            Me.BackForm.Invoke(Sub() Me.BackForm.Opacity = previousBackFormOpacity)
                            Me.FrontForm.Invoke(Sub()
                                                    Me.FrontForm.Opacity = 1
                                                    Me.FrontForm.BringToFront()
                                                    Me.FrontForm.Focus()
                                                End Sub)

                            Me.BackForm.Invoke(Sub() Me.BackForm.Location = previousBackFormLocation)
                            Me.FrontForm.Invoke(Sub() Me.FrontForm.Location = previousFrontFormLocation)
                        End Sub)
                End Sub

            AddHandler Me.FrontForm.Shown, antiFlickerHandler

            If dialogModal Then
                Return Me.FrontForm.ShowDialog(owner)
            Else
                Me.FrontForm.Show(owner)
                Return DialogResult.None
            End If
        End Function

#End Region

#Region " Dispose Method "

        ''' <summary>
        ''' Releases unmanaged and optionally managed resources.
        ''' </summary>
        '''
        ''' <param name="disposing">
        ''' true to release both managed and unmanaged resources; false to release only unmanaged resources.
        ''' </param>
        Protected Overrides Sub Dispose(disposing As Boolean)

            If disposing Then
                Me.FrontForm?.Dispose()
                Me.BackForm?.Dispose()
            End If

            MyBase.Dispose(disposing)

        End Sub

#End Region

    End Class

End Namespace

#End Region
