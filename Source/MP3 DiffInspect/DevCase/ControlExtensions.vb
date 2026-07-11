
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports DevCase.Core.Application.Forms

Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Runtime.CompilerServices

#End Region

#Region " Control Extensions "

' ReSharper disable once CheckNamespace

Namespace DevCase.Extensions.ControlExtensions

    ''' <summary>
    ''' Provides extension methods to use with <see cref="Control"/>.
    ''' </summary>
    <HideModuleName>
    Public Module ControlExtensions

#Region " Public Extension Methods "

        ''' <summary>
        ''' Iterates through all controls within a parent <see cref="Control"/>, 
        ''' optionally recursively, and performs the specified action on each control.
        ''' </summary>
        '''
        ''' <param name="parentControl">
        ''' The parent <see cref="Control"/> whose child controls are to be iterated.
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
        Public Sub ForEachControl(parentControl As Control, recursive As Boolean, action As Action(Of Control))

            ControlExtensions.ForEachControl(Of Control)(parentControl, recursive, action)
        End Sub

        ''' <summary>
        ''' Iterates through all controls of the specified type within a parent <see cref="Control"/>, 
        ''' optionally recursively, and performs the specified action on each control.
        ''' </summary>
        '''
        ''' <typeparam name="T">
        ''' The type of child controls to iterate through.
        ''' </typeparam>
        ''' 
        ''' <param name="parentControl">
        ''' The parent <see cref="Control"/> whose child controls are to be iterated.
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
        Public Sub ForEachControl(Of T As Control)(parentControl As Control, recursive As Boolean, action As Action(Of T))

            If TypeOf parentControl Is ToolStrip Then
                ' Throw New InvalidOperationException($"Not allowed. Please use method {NameOf(ToolStripExtensions.ForEachItem)} to iterate items of a {NameOf(ToolStrip)}, {NameOf(StatusStrip)}, {NameOf(MenuStrip)} or {NameOf(Control.ContextMenuStrip)} controls.")
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'ToolStrip'")
            End If

            If action Is Nothing Then
                Throw New ArgumentNullException(paramName:=NameOf(action), "Action cannot be null.")
            End If

            Dim queue As New Queue(Of Control)

            ' First level items iteration.
            For Each control As Control In parentControl.Controls
                If recursive Then
                    queue.Enqueue(control)
                Else
                    If TypeOf control Is T Then
                        action.Invoke(DirectCast(control, T))
                    End If
                End If
            Next control

            ' Recursive items iteration.
            While queue.Count <> 0
                Dim currentControl As Control = queue.Dequeue()
                If TypeOf currentControl Is T Then
                    action.Invoke(DirectCast(currentControl, T))
                End If

                For Each childControl As Control In currentControl.Controls
                    queue.Enqueue(childControl)
                Next childControl
            End While
        End Sub

        ''' <summary>
        ''' Changes the color appearance of the source <see cref="Control"/> using the specified theme.
        ''' </summary>
        '''
        ''' <param name="ctrl">
        ''' The source <see cref="Control"/>.
        ''' </param>
        '''
        ''' <param name="theme">
        ''' The visual theme.
        ''' </param>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Public Sub SetVisualTheme(ctrl As Control, theme As VisualTheme)

            Select Case theme

                Case VisualTheme.Default
                    ControlExtensions.Internal_SetThemeDefault(ctrl)

                Case VisualTheme.VisualStudioDark
                    ControlExtensions.Internal_SetThemeVisualStudioDark(ctrl)

                Case Else
                    Throw New InvalidEnumArgumentException(NameOf(theme), theme, GetType(VisualTheme))

            End Select

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Changes the color appearance of the source <see cref="Control"/> to its default appearance.
        ''' </summary>
        '''
        ''' <param name="ctrl">
        ''' The source <see cref="Control"/>.
        ''' </param>
        <DebuggerStepThrough>
        Private Sub Internal_SetThemeDefault(ctrl As Control)

            If ctrl.GetType() = GetType(Button) Then
                With DirectCast(ctrl, Button)
                    .ResetBackColor()
                    .ResetForeColor()
                    .FlatAppearance.BorderColor = Color.Empty
                    .FlatAppearance.BorderSize = 1
                    .UseVisualStyleBackColor = True
                    .UseCompatibleTextRendering = False
                    .FlatStyle = FlatStyle.Standard
                End With

            ElseIf ctrl.GetType() = GetType(ByteViewer) Then
                With DirectCast(ctrl, ByteViewer)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(CheckBox) Then
                With DirectCast(ctrl, CheckBox)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(CheckedListBox) Then
                With DirectCast(ctrl, CheckedListBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.Fixed3D
                End With

            ElseIf ctrl.GetType() = GetType(ComboBox) Then
                With DirectCast(ctrl, ComboBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .FlatStyle = FlatStyle.Standard
                End With

            ElseIf ctrl.GetType() = GetType(DateTimePicker) Then
                With DirectCast(ctrl, DateTimePicker)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(DataGridView) Then
                'With DirectCast(ctrl, DataGridView)
                '    .BorderStyle = BorderStyle.FixedSingle
                '    .RowTemplate.DefaultCellStyle.BackColor = WinForms.DataGridView.DefaultBackColor
                '    .RowTemplate.DefaultCellStyle.ForeColor = WinForms.DataGridView.DefaultForeColor
                '    .ResetBackColor()
                '    .ResetForeColor()
                '    .Rows.ForEach(Sub(row As DataGridViewRow) row.DefaultCellStyle.ApplyStyle(.RowTemplate.DefaultCellStyle))
                '    '.Rows.ForEach(Sub(row As DataGridViewRow) row.Cells.ForEach(Sub(cell As DataGridViewCell) cell.Style.ApplyStyle(.RowTemplate.DefaultCellStyle)))
                'End With
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'DataGridView'")

            ElseIf ctrl.GetType() = GetType(FlowLayoutPanel) Then
                With DirectCast(ctrl, FlowLayoutPanel)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(Form) Then
                With DirectCast(ctrl, Form)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(GroupBox) Then
                With DirectCast(ctrl, GroupBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .FlatStyle = FlatStyle.Standard
                End With

            ElseIf ctrl.GetType() = GetType(HScrollBar) Then
                With DirectCast(ctrl, HScrollBar)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(Label) Then
                With DirectCast(ctrl, Label)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(LinkLabel) Then
                With DirectCast(ctrl, LinkLabel)
                    .ActiveLinkColor = System.Drawing.Color.Red
                    .DisabledLinkColor = System.Drawing.Color.FromArgb(133, 133, 133)
                    .LinkColor = System.Drawing.Color.Blue
                    .VisitedLinkColor = System.Drawing.Color.FromArgb(255, 128, 0, 128)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(ListBox) Then
                With DirectCast(ctrl, ListBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(ListView) Then
                With DirectCast(ctrl, ListView)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.Fixed3D
                End With

            ElseIf ctrl.GetType() = GetType(MaskedTextBox) Then
                With DirectCast(ctrl, MaskedTextBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.Fixed3D
                End With

            ElseIf ctrl.GetType() = GetType(MonthCalendar) Then
                With DirectCast(ctrl, MonthCalendar)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(NumericUpDown) Then
                With DirectCast(ctrl, NumericUpDown)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(Panel) Then
                With DirectCast(ctrl, Panel)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(PictureBox) Then
                With DirectCast(ctrl, PictureBox)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(ProgressBar) Then
                'With DirectCast(ctrl, ProgressBar)
                '    .ResetBackColor()
                '    .ResetForeColor()
                '    RemoveHandler .Paint, AddressOf ControlExtensions.ProgressBar_Paint_VisualStudioDark
                'End With
                'ControlExtensions.SetControlStyle(ctrl, ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, False)
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'ProgressBar'")

            ElseIf ctrl.GetType() = GetType(PropertyGrid) Then
                With DirectCast(ctrl, PropertyGrid)
                    .CategoryForeColor = SystemColors.ControlText
                    .CommandsActiveLinkColor = System.Drawing.Color.Red
                    .CommandsBackColor = SystemColors.Control
                    .CommandsDisabledLinkColor = System.Drawing.Color.FromArgb(133, 133, 133)
                    .CommandsForeColor = SystemColors.ControlText
                    .CommandsLinkColor = System.Drawing.Color.FromArgb(0, 0, 255)
                    .HelpBackColor = SystemColors.Control
                    .HelpForeColor = SystemColors.ControlText
                    .LineColor = SystemColors.InactiveBorder
                    .ViewBackColor = SystemColors.Window
                    .ViewForeColor = SystemColors.WindowText
                    .CategorySplitterColor = SystemColors.Control
                    .CommandsBorderColor = SystemColors.ControlDark
                    .DisabledItemForeColor = SystemColors.GrayText
                    .HelpBorderColor = SystemColors.ControlDark
                    .SelectedItemWithFocusBackColor = SystemColors.Highlight
                    .SelectedItemWithFocusForeColor = SystemColors.HighlightText
                    .ViewBorderColor = SystemColors.ControlDark
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(RadioButton) Then
                With DirectCast(ctrl, RadioButton)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(RichTextBox) Then
                With DirectCast(ctrl, RichTextBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.Fixed3D
                End With

            ElseIf ctrl.GetType() = GetType(SplitContainer) Then
                With DirectCast(ctrl, SplitContainer)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(Splitter) Then
                With DirectCast(ctrl, Splitter)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(SplitterPanel) Then
                With DirectCast(ctrl, SplitterPanel)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(TabControl) Then
                With DirectCast(ctrl, TabControl)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(TabPage) Then
                With DirectCast(ctrl, TabPage)
                    '.ResetBackColor() ' Calling ResetBackColor method does not apply the proper default color.
                    .BackColor = SystemColors.Window
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(TableLayoutPanel) Then
                With DirectCast(ctrl, TableLayoutPanel)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(TextBox) Then
                With DirectCast(ctrl, TextBox)
                    .ResetBackColor()
                    .ResetForeColor()
                    .BorderStyle = BorderStyle.Fixed3D
                End With

            ElseIf ctrl.GetType() = GetType(ToolStrip) Then
                'Dim strip As ToolStrip = DirectCast(ctrl, ToolStrip)
                'strip.ResetBackColor()
                'strip.ResetForeColor()
                'ToolStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.ResetBackColor()
                '        item.ResetForeColor()
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.ManagerRenderMode
                'strip.Renderer = Nothing
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'ToolStrip'")

            ElseIf ctrl.GetType() = GetType(MenuStrip) Then
                'Dim strip As MenuStrip = DirectCast(ctrl, MenuStrip)
                'strip.ResetBackColor()
                'strip.ResetForeColor()
                'MenuStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.ResetBackColor()
                '        item.ResetForeColor()
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.ManagerRenderMode
                'strip.Renderer = Nothing
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'MenuStrip'")

            ElseIf ctrl.GetType() = GetType(StatusStrip) Then
                'Dim strip As StatusStrip = DirectCast(ctrl, StatusStrip)
                'strip.ResetBackColor()
                'strip.ResetForeColor()
                'StatusStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.ResetBackColor()
                '        item.ResetForeColor()
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.ManagerRenderMode
                'strip.Renderer = Nothing
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'StatusStrip'")

            ElseIf ctrl.GetType() = GetType(TrackBar) Then
                With DirectCast(ctrl, TrackBar)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(TreeView) Then
                With DirectCast(ctrl, TreeView)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(VScrollBar) Then
                With DirectCast(ctrl, VScrollBar)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(WebBrowser) Then
                With DirectCast(ctrl, WebBrowser)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            ElseIf ctrl.GetType() = GetType(WebBrowserBase) Then
                With DirectCast(ctrl, WebBrowserBase)
                    .ResetBackColor()
                    .ResetForeColor()
                End With

            Else
                ctrl.ResetBackColor()
                ctrl.ResetForeColor()
                ' Throw New NotImplementedException($"A visual style for the specified control type is not implemented: '{ctrl.GetType().FullName}'")

            End If

        End Sub

        ''' <summary>
        ''' Changes the color appearance of the source <see cref="Control"/> to Visual Studio Dark Theme appearance.
        ''' </summary>
        '''
        ''' <param name="ctrl">
        ''' The source <see cref="Control"/>.
        ''' </param>
        <DebuggerStepThrough>
        Private Sub Internal_SetThemeVisualStudioDark(ctrl As Control)

            If ctrl.GetType() = GetType(Button) Then
                With DirectCast(ctrl, Button)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .FlatAppearance.BorderColor = Color.DimGray
                    .FlatAppearance.BorderSize = 1
                    .UseVisualStyleBackColor = False
                    .UseCompatibleTextRendering = True
                    .FlatStyle = FlatStyle.Flat
                End With

            ElseIf ctrl.GetType() = GetType(ByteViewer) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(CheckBox) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(CheckedListBox) Then
                With DirectCast(ctrl, CheckedListBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(ComboBox) Then
                With DirectCast(ctrl, ComboBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    If .DropDownStyle <> ComboBoxStyle.Simple Then
                        .FlatStyle = FlatStyle.Flat

                        ' Toogling DropDownStyle value forces to recreate the ComboBox handle
                        ' to properly reflect the new BackColor.
                        Dim originalDropDownStyle As ComboBoxStyle = .DropDownStyle
                        .DropDownStyle = ComboBoxStyle.Simple
                        .DropDownStyle = originalDropDownStyle
                    End If
                End With

            ElseIf ctrl.GetType() = GetType(DateTimePicker) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(DataGridView) Then
                'ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                'ctrl.ForeColor = System.Drawing.Color.Gainsboro
                'With DirectCast(ctrl, DataGridView)
                '    .BorderStyle = BorderStyle.FixedSingle
                '    .RowTemplate.DefaultCellStyle.BackColor = Color.FromArgb(255, 33, 32, 33)
                '    .RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Gainsboro
                '    .Rows.ForEach(Sub(row As DataGridViewRow) row.DefaultCellStyle.ApplyStyle(.RowTemplate.DefaultCellStyle))
                '    ' .Rows.ForEach(Sub(row As DataGridViewRow) row.Cells.ForEach(Sub(cell As DataGridViewCell) cell.Style.ApplyStyle(.RowTemplate.DefaultCellStyle)))
                'End With
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'DataGridView'")

            ElseIf ctrl.GetType() = GetType(FlowLayoutPanel) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(Form) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(GroupBox) Then
                With DirectCast(ctrl, GroupBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .FlatStyle = FlatStyle.Flat
                End With

            ElseIf ctrl.GetType() = GetType(HScrollBar) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(Label) Then
                ' ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.BackColor = System.Drawing.Color.Transparent
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(LinkLabel) Then
                With DirectCast(ctrl, LinkLabel)
                    .ActiveLinkColor = System.Drawing.Color.IndianRed
                    '  .BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                    .BackColor = System.Drawing.Color.Transparent
                    .DisabledLinkColor = System.Drawing.Color.FromArgb(133, 133, 133)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .LinkColor = System.Drawing.Color.FromArgb(255, 0, 122, 204)
                    .VisitedLinkColor = System.Drawing.Color.FromArgb(255, 128, 0, 128)
                End With

            ElseIf ctrl.GetType() = GetType(ListBox) Then
                With DirectCast(ctrl, ListBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(ListView) Then
                With DirectCast(ctrl, ListView)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(MaskedTextBox) Then
                With DirectCast(ctrl, MaskedTextBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(MonthCalendar) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(ToolStrip) Then
                'Dim strip As ToolStrip = DirectCast(ctrl, ToolStrip)
                'strip.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                'strip.ForeColor = System.Drawing.Color.Gainsboro
                'ToolStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                '        item.ForeColor = System.Drawing.Color.Gainsboro
                '        AddHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        AddHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            AddHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.System
                'strip.Renderer = New ToolStripDarkSystemRenderer()
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'ToolStrip'")

            ElseIf ctrl.GetType() = GetType(MenuStrip) Then
                'Dim strip As MenuStrip = DirectCast(ctrl, MenuStrip)
                'strip.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                'strip.ForeColor = System.Drawing.Color.Gainsboro
                'MenuStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                '        item.ForeColor = System.Drawing.Color.Gainsboro
                '        AddHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        AddHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            AddHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.System
                'strip.Renderer = New ToolStripDarkSystemRenderer()
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'MenuStrip'")

            ElseIf ctrl.GetType() = GetType(StatusStrip) Then
                'Dim strip As StatusStrip = DirectCast(ctrl, StatusStrip)
                'strip.BackColor = System.Drawing.Color.FromArgb(255, 0, 122, 204)
                'strip.ForeColor = System.Drawing.Color.Gainsboro
                'StatusStripExtensions.ForEachItem(
                '    strip, recursive:=True,
                '    Sub(item As ToolStripItem)
                '        RemoveHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        RemoveHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            RemoveHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '        item.BackColor = System.Drawing.Color.FromArgb(255, 0, 122, 204)
                '        item.ForeColor = System.Drawing.Color.Gainsboro
                '        AddHandler item.MouseEnter, AddressOf ControlExtensions.ToolStripItem_MouseEnter_VisualStudioDark
                '        AddHandler item.MouseLeave, AddressOf ControlExtensions.ToolStripItem_MouseLeave_VisualStudioDark
                '        If TypeOf item Is ToolStripMenuItem Then
                '            AddHandler DirectCast(item, ToolStripMenuItem).DropDownClosed, AddressOf ControlExtensions.ToolStripMenuItem_DropDownClosed_VisualStudioDark
                '        End If
                '    End Sub)
                'strip.RenderMode = ToolStripRenderMode.System
                'strip.Renderer = New ToolStripDarkSystemRenderer()
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'StatusStrip'")

            ElseIf ctrl.GetType() = GetType(NumericUpDown) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(Panel) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(PictureBox) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(ProgressBar) Then
                'Dim pb As ProgressBar = DirectCast(ctrl, ProgressBar)
                'ctrl.BackColor = Color.FromArgb(255, 30, 30, 30) 'System.Drawing.Color.FromArgb(255, 45, 45, 48)
                'ctrl.ForeColor = Color.Black 'System.Drawing.Color.Gainsboro
                'ControlExtensions.SetControlStyle(pb, ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
                'AddHandler pb.Paint, AddressOf ControlExtensions.ProgressBar_Paint_VisualStudioDark
                Throw New NotImplementedException("A visual style for the specified control type on this project is not implemented: 'ProgressBar'")

            ElseIf ctrl.GetType() = GetType(PropertyGrid) Then
                With DirectCast(ctrl, PropertyGrid)
                    .BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .CategoryForeColor = System.Drawing.Color.Silver
                    .CommandsActiveLinkColor = System.Drawing.Color.Red
                    .CommandsBackColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .CommandsDisabledLinkColor = System.Drawing.Color.FromArgb(133, 133, 133)
                    .CommandsForeColor = System.Drawing.Color.Gainsboro
                    .CommandsLinkColor = System.Drawing.Color.FromArgb(0, 0, 255)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .HelpBackColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .HelpForeColor = System.Drawing.Color.Gainsboro
                    .LineColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .ViewBackColor = System.Drawing.Color.FromArgb(37, 37, 38)
                    .ViewForeColor = System.Drawing.Color.Gainsboro
                    .CategorySplitterColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .CommandsBorderColor = System.Drawing.Color.Silver
                    .DisabledItemForeColor = System.Drawing.Color.FromArgb(127, 245, 245, 245)
                    .HelpBorderColor = System.Drawing.Color.FromArgb(45, 45, 48)
                    .SelectedItemWithFocusBackColor = SystemColors.Highlight
                    .SelectedItemWithFocusForeColor = SystemColors.HighlightText
                    .ViewBorderColor = System.Drawing.Color.FromArgb(45, 45, 48)
                End With

            ElseIf ctrl.GetType() = GetType(RadioButton) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(RichTextBox) Then
                With DirectCast(ctrl, RichTextBox)
                    .BackColor = System.Drawing.Color.FromArgb(37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.None
                End With

            ElseIf ctrl.GetType() = GetType(SplitContainer) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(Splitter) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(SplitterPanel) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TabControl) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TabPage) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TableLayoutPanel) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TextBox) Then
                With DirectCast(ctrl, TextBox)
                    .BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                    .ForeColor = System.Drawing.Color.Gainsboro
                    .BorderStyle = BorderStyle.FixedSingle
                End With

            ElseIf ctrl.GetType() = GetType(ToolStrip) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TrackBar) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 45, 45, 48)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(TreeView) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(VScrollBar) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(WebBrowser) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            ElseIf ctrl.GetType() = GetType(WebBrowserBase) Then
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro

            Else
                ctrl.BackColor = System.Drawing.Color.FromArgb(255, 37, 37, 38)
                ctrl.ForeColor = System.Drawing.Color.Gainsboro
                ' Throw New NotImplementedException($"A visual style For the specified control type Is Not implemented: '{ctrl.GetType().FullName}'")

            End If

        End Sub

#End Region

    End Module

End Namespace

#End Region
