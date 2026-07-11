
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " VisualTheme "

' ReSharper disable once CheckNamespace

Namespace DevCase.Core.Application.Forms

    ''' <summary>
    ''' Specifies a visual theme for controls.
    ''' </summary>
    Public Enum VisualTheme As Integer

        ''' <summary>
        ''' Default theme.
        ''' </summary>
        [Default] = 0

        ''' <summary>
        ''' Visual Studio's dark theme.
        ''' </summary>
        VisualStudioDark = 1

    End Enum

End Namespace

#End Region
