
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.CompilerServices

#End Region

#Region " ContainerControl Extensions "

' ReSharper disable once CheckNamespace

Namespace DevCase.Extensions.ContainerControlExtensions

    ''' <summary>
    ''' Provides extension methods to use with <see cref="ContainerControl"/>.
    ''' </summary>
    <HideModuleName>
    Public Module ContainerControlExtensions

#Region " Public Extension Methods "

        ''' <summary>
        ''' Iterates through all controls within a parent <see cref="ContainerControl"/>, 
        ''' optionally recursively, and performs the specified action on each control.
        ''' </summary>
        '''
        ''' <param name="container">
        ''' The parent <see cref="ContainerControl"/> whose child controls are to be iterated.
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
        <Extension>
        <DebuggerStepThrough>
        Public Sub ForEachControl(container As ContainerControl, recursive As Boolean, action As Action(Of Control))

            ControlExtensions.ForEachControl(Of Control)(container, recursive, action)
        End Sub

        ''' <summary>
        ''' Iterates through all controls of the specified type within a parent <see cref="ContainerControl"/>, 
        ''' optionally recursively, and performs the specified action on each control.
        ''' </summary>
        '''
        ''' <typeparam name="T">
        ''' The type of child controls to iterate through.
        ''' </typeparam>
        ''' 
        ''' <param name="container">
        ''' The parent <see cref="ContainerControl"/> whose child controls are to be iterated.
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
        <Extension>
        <DebuggerStepThrough>
        Public Sub ForEachControl(Of T As Control)(container As ContainerControl, recursive As Boolean, action As Action(Of T))

            ControlExtensions.ForEachControl(container, recursive, action)
        End Sub

#End Region

    End Module

End Namespace

#End Region
