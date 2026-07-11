
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Imports WinForms = System.Windows.Forms

Imports DevCase.Core.Application.Forms
Imports DevCase.Win32.Enums

#End Region

#Region " Form Extensions "

' ReSharper disable once CheckNamespace

Namespace DevCase.Extensions.FormExtensions

    ''' <summary>
    ''' Provides extension methods to use with <see cref="Form"/>.
    ''' </summary>
    <HideModuleName>
    Public Module FormExtensions

#Region " Public Extension Methods "

        ''' <summary>
        ''' Iterates through all controls within a parent <see cref="Form"/>, 
        ''' optionally recursively, and performs the specified action on each control.
        ''' </summary>
        '''
        ''' <param name="form">
        ''' The parent <see cref="Form"/> whose child controls are to be iterated.
        ''' </param>
        ''' 
        ''' <param name="recursive">
        ''' <see langword="True"/> to iterate recursively through all child controls 
        ''' (i.e., iterate the child controls of child controls); otherwise, <see langword="False"/>.
        ''' </param>
        ''' 
        ''' <param name="action">
        ''' The action to perform on each control.
        ''' </param>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Public Sub ForEachControl(form As Form, recursive As Boolean, action As Action(Of Control))

            ContainerControlExtensions.ForEachControl(Of Control)(form, recursive, action)
        End Sub

        ''' <summary>
        ''' Changes the color appearance of the source <see cref="Form"/> using the specified theme.
        ''' </summary>
        '''
        ''' <param name="f">
        ''' The source <see cref="Form"/>.
        ''' </param>
        ''' 
        ''' <param name="theme">
        ''' The visual theme.
        ''' </param>
        ''' 
        ''' <param name="childControls">
        ''' <see langword="True"/> to change the color appearance of child controls too.
        ''' </param>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Public Sub SetVisualTheme(f As Form, theme As VisualTheme, childControls As Boolean)

            Dim useDarkMode As Integer

            Select Case theme

                Case VisualTheme.Default
                    f.BackColor = WinForms.Form.DefaultBackColor
                    f.ForeColor = WinForms.Form.DefaultForeColor
                    useDarkMode = 0

                Case VisualTheme.VisualStudioDark
                    f.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                    f.ForeColor = System.Drawing.Color.Gainsboro
                    useDarkMode = 1

                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(theme), theme, GetType(VisualTheme))

            End Select

            If childControls Then
                FormExtensions.ForEachControl(f, True,
                                              Sub(ctrl As Control)
                                                  If ctrl.GetType().IsPublic Then
                                                      ControlExtensions.SetVisualTheme(ctrl, theme)
                                                  End If
                                              End Sub)
            End If

            Dim useDarkModeResult As Integer =
                NativeMethods.DwmSetWindowAttribute(f.Handle, DwmWindowAttribute.UseImmersiveDarkMode,
                                                    useDarkMode, CUInt(Marshal.SizeOf(useDarkMode)))

            ' If the window is in normal state, force it to redraw by
            ' resizing it by 1 pixel and then resizing it back to the original size.
            ' This is required because the caption color change (DwmWindowAttribute.UseImmersiveDarkMode).
            If (useDarkModeResult = 0) AndAlso (f.WindowState = FormWindowState.Normal) Then
                Dim originalSize As Size = f.Size

                ' --- Determine a safe delta (+1 or -1) ---
                Dim delta As Integer = 0

                ' Check if we can grow by 1 pixel in height
                Dim canGrow As Boolean =
                    (f.MaximumSize.Height = 0 OrElse originalSize.Height + 1 <= f.MaximumSize.Height) AndAlso
                    (f.MaximumSize.Width = 0 OrElse originalSize.Width <= f.MaximumSize.Width)

                ' Check if we can shrink by 1 pixel in height
                Dim canShrink As Boolean =
                    (originalSize.Height - 1 >= f.MinimumSize.Height) AndAlso
                    (originalSize.Height - 1 >= SystemInformation.MinimumWindowSize.Height)

                If canGrow Then
                    delta = 1
                ElseIf canShrink Then
                    delta = -1
                End If

                ' Only attempt to resize if we found a delta.
                If delta <> 0 Then
                    f.Size = New Size(originalSize.Width, originalSize.Height + delta)
                    f.Size = originalSize
                End If
            End If
        End Sub

#End Region

    End Module

End Namespace

#End Region
