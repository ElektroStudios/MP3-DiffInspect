Option Strict On
Option Explicit On
Option Infer Off

Imports System.Reflection
Imports System.Threading

Public Class PropertyGridHighlightOverlay : Inherits Control

    Private Const WM_NCHITTEST As Integer = &H84
    Private Const HTTRANSPARENT As Integer = -1

    Private ReadOnly _parentGrid As PropertyGrid
    Private ReadOnly _targetLabels As List(Of String)
    Private ReadOnly _gridView As Control

    Private highlightColor As Color = Color.FromArgb(40, Color.Yellow)

    Public Sub New(ByVal pg As PropertyGrid, ByVal labels As List(Of String))
        If pg Is Nothing Then
            Throw New ArgumentNullException(NameOf(pg))
        End If
        If labels Is Nothing Then
            Throw New ArgumentNullException(NameOf(labels))
        End If

        _parentGrid = pg
        _targetLabels = labels

        Dim field As FieldInfo = pg.GetType().GetField("gridView", BindingFlags.NonPublic Or BindingFlags.Instance)
        If field Is Nothing Then
            Throw New MissingMemberException("Field 'gridView' not found.")
        End If

        _gridView = DirectCast(field.GetValue(pg), Control)

        Me.SetStyle(ControlStyles.SupportsTransparentBackColor Or
                ControlStyles.OptimizedDoubleBuffer Or
                ControlStyles.AllPaintingInWmPaint Or
                ControlStyles.UserPaint, True)
        Me.BackColor = Color.Transparent
        Me.Bounds = _gridView.ClientRectangle
        _gridView.Controls.Add(Me)
        Me.BringToFront()

        AddHandler _gridView.SizeChanged, AddressOf Me.OnGridViewBoundsChanged
        AddHandler _gridView.Layout, AddressOf Me.OnGridViewLayout
        AddHandler _parentGrid.SizeChanged, AddressOf Me.OnParentGridSizeChanged
        AddHandler _gridView.Paint, AddressOf Me.OnGridViewPaint
    End Sub

    Private Sub OnGridViewBoundsChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.SyncBounds()
    End Sub

    Private Sub OnGridViewLayout(ByVal sender As Object, ByVal e As LayoutEventArgs)
        Me.SyncBounds()
    End Sub

    Private Sub OnParentGridSizeChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.SyncBounds()
    End Sub

    Private Sub SyncBounds()
        Dim target As Rectangle = _gridView.ClientRectangle
        If Me.Bounds <> target Then
            Me.Bounds = target
        End If
        Me.Invalidate()
    End Sub

    Private Sub OnGridViewResize(ByVal sender As Object, ByVal e As EventArgs)
        Me.Bounds = _gridView.ClientRectangle
        Me.Invalidate()
    End Sub

    Private Sub OnGridViewPaint(ByVal sender As Object, ByVal e As PaintEventArgs)
        ' When the gridView repaints (selection change, scroll, etc.) the overlay
        ' must repaint too, so the highlight rectangles stay aligned with rows
        ' whose position may have shifted.
        Me.Invalidate()
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_NCHITTEST Then
            ' Let every mouse message pass through to the gridView underneath,
            ' so clicks, hover, and native tooltips work as if the overlay did not exist.
            m.Result = New IntPtr(HTTRANSPARENT)
            Return
        End If

        MyBase.WndProc(m)
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Const WS_EX_TRANSPARENT As Integer = &H20
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or WS_EX_TRANSPARENT
            Return cp
        End Get
    End Property

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        Me.PaintHighlights(e.Graphics)
    End Sub

    Private Sub PaintHighlights(ByVal g As Graphics)
        Dim gvType As Type = _gridView.GetType()

        Dim entriesMethod As MethodInfo = gvType.GetMethod("GetAllGridEntries",
                                                           BindingFlags.NonPublic Or BindingFlags.Instance,
                                                           Nothing,
                                                           Type.EmptyTypes,
                                                           Nothing)

        Dim rectMethod As MethodInfo = gvType.GetMethod("AccessibilityGetGridEntryBounds", BindingFlags.NonPublic Or BindingFlags.Instance)

        Dim indentMethod As MethodInfo = gvType.GetMethod("GetIPELabelIndent", BindingFlags.NonPublic Or BindingFlags.Instance)

        Dim allEntries As IEnumerable = DirectCast(entriesMethod.Invoke(_gridView, Nothing), IEnumerable)

        For Each entry As Object In allEntries
            Thread.CurrentThread.Join(0)

            Dim labelProp As PropertyInfo = If(entry.GetType().GetProperty("Label", BindingFlags.Public Or BindingFlags.Instance), entry.GetType().GetProperty("PropertyLabel", BindingFlags.NonPublic Or BindingFlags.Instance))

            Dim currentLabel As String = DirectCast(labelProp.GetValue(entry), String)
            If Not _targetLabels.Contains(currentLabel) Then Continue For

            Dim rectScreen As Rectangle = DirectCast(rectMethod.Invoke(_gridView, New Object() {entry}), Rectangle)
            Dim localRect As Rectangle = _gridView.RectangleToClient(rectScreen)
            If localRect.Height <= 0 Then Continue For

            Dim indent As Integer = If(indentMethod IsNot Nothing, DirectCast(indentMethod.Invoke(_gridView, New Object() {entry}), Integer), 18)

            localRect.X += indent
            localRect.Width -= indent

            Using blendBrush As New SolidBrush(highlightColor)
                g.FillRectangle(blendBrush, localRect)
            End Using
        Next
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            RemoveHandler _gridView.SizeChanged, AddressOf Me.OnGridViewBoundsChanged
            RemoveHandler _gridView.Layout, AddressOf Me.OnGridViewLayout
            RemoveHandler _parentGrid.SizeChanged, AddressOf Me.OnParentGridSizeChanged
            RemoveHandler _gridView.Paint, AddressOf Me.OnGridViewPaint
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class