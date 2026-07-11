
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " DWM Window Attribute "

' ReSharper disable once CheckNamespace

Namespace DevCase.Win32.Enums

    ''' <summary>
    ''' Specifies the attribute to set when calling the <see cref="NativeMethods.DwmSetWindowAttribute"/> function.
    ''' </summary>
    '''
    ''' <remarks>
    ''' <see href="https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute"/>
    ''' </remarks>
    Public Enum DwmWindowAttribute As UInteger

        ' WARNING: THIS ENUM IS NOT COMPLETE. IT IS ADAPTED TO THE REQUIREMENTS OF THE PROJECT.
        ' IF YOU NEED TO USE OTHER ATTRIBUTES, PLEASE REFER TO THE DOCUMENTATION LINK ABOVE.

        ''' <summary>
        ''' Allows the window frame for this window to be drawn in dark mode colors when the dark mode system setting is enabled. 
        ''' <para></para>
        ''' For compatibility reasons, all windows default to light mode regardless of the system setting. 
        ''' <para></para>
        ''' This value is supported starting with Windows 11 Build 22000.
        ''' </summary>
        UseImmersiveDarkMode = 20

    End Enum

End Namespace

#End Region
