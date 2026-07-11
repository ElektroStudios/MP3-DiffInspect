Imports System.Runtime.InteropServices

Friend Module NativeMethods

    <DllImport("dwmapi.dll")>
    Friend Function DwmSetWindowAttribute(
        hWnd As IntPtr,
        attribute As UInteger,
        ByRef refAttributeValue As Integer,
        attributeSize As UInteger
    ) As Integer
    End Function

End Module
