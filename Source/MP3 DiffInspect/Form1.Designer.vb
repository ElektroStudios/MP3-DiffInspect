<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.PropertyGridTop = New System.Windows.Forms.PropertyGrid()
        Me.ButtonLoadFileTop = New System.Windows.Forms.Button()
        Me.PictureBoxTop = New System.Windows.Forms.PictureBox()
        Me.ButtonLoadFileBottom = New System.Windows.Forms.Button()
        Me.PictureBoxBottom = New System.Windows.Forms.PictureBox()
        Me.PropertyGridBottom = New System.Windows.Forms.PropertyGrid()
        Me.LabelOtherDifferences = New System.Windows.Forms.Label()
        Me.LabelOtherDifferencesResult = New System.Windows.Forms.Label()
        Me.OpenFileDialogTop = New Ookii.Dialogs.WinForms.VistaOpenFileDialog()
        Me.OpenFileDialogBottom = New Ookii.Dialogs.WinForms.VistaOpenFileDialog()
        Me.LabelAudioStreamDiff = New System.Windows.Forms.Label()
        Me.TableLayoutPanel_TopPict = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel_BottomPict = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel_FullBottomArea = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel_FullTopArea = New System.Windows.Forms.TableLayoutPanel()
        CType(Me.PictureBoxTop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxBottom, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel_TopPict.SuspendLayout()
        Me.TableLayoutPanel_BottomPict.SuspendLayout()
        Me.TableLayoutPanel_FullBottomArea.SuspendLayout()
        Me.TableLayoutPanel_FullTopArea.SuspendLayout()
        Me.SuspendLayout()
        '
        'PropertyGridTop
        '
        Me.PropertyGridTop.AllowDrop = True
        Me.PropertyGridTop.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.PropertyGridTop.CanShowVisualStyleGlyphs = False
        Me.PropertyGridTop.CommandsVisibleIfAvailable = False
        Me.PropertyGridTop.DisabledItemForeColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.PropertyGridTop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridTop.Font = New System.Drawing.Font("Segoe UI", 12.25!)
        Me.PropertyGridTop.HelpVisible = False
        Me.PropertyGridTop.LineColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.PropertyGridTop.Location = New System.Drawing.Point(210, 0)
        Me.PropertyGridTop.Margin = New System.Windows.Forms.Padding(0)
        Me.PropertyGridTop.Name = "PropertyGridTop"
        Me.PropertyGridTop.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGridTop.Size = New System.Drawing.Size(826, 253)
        Me.PropertyGridTop.TabIndex = 1
        Me.PropertyGridTop.ToolbarVisible = False
        Me.PropertyGridTop.ViewBackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.PropertyGridTop.ViewBorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.PropertyGridTop.ViewForeColor = System.Drawing.Color.WhiteSmoke
        '
        'ButtonLoadFileTop
        '
        Me.ButtonLoadFileTop.AllowDrop = True
        Me.ButtonLoadFileTop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonLoadFileTop.Font = New System.Drawing.Font("Segoe UI", 12.25!)
        Me.ButtonLoadFileTop.Location = New System.Drawing.Point(0, 0)
        Me.ButtonLoadFileTop.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonLoadFileTop.Name = "ButtonLoadFileTop"
        Me.ButtonLoadFileTop.Size = New System.Drawing.Size(210, 43)
        Me.ButtonLoadFileTop.TabIndex = 0
        Me.ButtonLoadFileTop.Text = "Open first MP3 file..."
        Me.ButtonLoadFileTop.UseVisualStyleBackColor = True
        '
        'PictureBoxTop
        '
        Me.PictureBoxTop.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.PictureBoxTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBoxTop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBoxTop.Location = New System.Drawing.Point(0, 43)
        Me.PictureBoxTop.Margin = New System.Windows.Forms.Padding(0)
        Me.PictureBoxTop.Name = "PictureBoxTop"
        Me.PictureBoxTop.Size = New System.Drawing.Size(210, 210)
        Me.PictureBoxTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxTop.TabIndex = 2
        Me.PictureBoxTop.TabStop = False
        '
        'ButtonLoadFileBottom
        '
        Me.ButtonLoadFileBottom.AllowDrop = True
        Me.ButtonLoadFileBottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonLoadFileBottom.Font = New System.Drawing.Font("Segoe UI", 12.25!)
        Me.ButtonLoadFileBottom.Location = New System.Drawing.Point(0, 0)
        Me.ButtonLoadFileBottom.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonLoadFileBottom.Name = "ButtonLoadFileBottom"
        Me.ButtonLoadFileBottom.Size = New System.Drawing.Size(210, 43)
        Me.ButtonLoadFileBottom.TabIndex = 3
        Me.ButtonLoadFileBottom.Text = "Open second MP3 file..."
        Me.ButtonLoadFileBottom.UseVisualStyleBackColor = True
        '
        'PictureBoxBottom
        '
        Me.PictureBoxBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.PictureBoxBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBoxBottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBoxBottom.Location = New System.Drawing.Point(0, 43)
        Me.PictureBoxBottom.Margin = New System.Windows.Forms.Padding(0)
        Me.PictureBoxBottom.Name = "PictureBoxBottom"
        Me.PictureBoxBottom.Size = New System.Drawing.Size(210, 210)
        Me.PictureBoxBottom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxBottom.TabIndex = 4
        Me.PictureBoxBottom.TabStop = False
        '
        'PropertyGridBottom
        '
        Me.PropertyGridBottom.AllowDrop = True
        Me.PropertyGridBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.PropertyGridBottom.CanShowVisualStyleGlyphs = False
        Me.PropertyGridBottom.CommandsBackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.PropertyGridBottom.CommandsVisibleIfAvailable = False
        Me.PropertyGridBottom.DisabledItemForeColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.PropertyGridBottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridBottom.Font = New System.Drawing.Font("Segoe UI", 12.25!)
        Me.PropertyGridBottom.HelpVisible = False
        Me.PropertyGridBottom.LineColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(46, Byte), Integer))
        Me.PropertyGridBottom.Location = New System.Drawing.Point(210, 0)
        Me.PropertyGridBottom.Margin = New System.Windows.Forms.Padding(0)
        Me.PropertyGridBottom.Name = "PropertyGridBottom"
        Me.PropertyGridBottom.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGridBottom.Size = New System.Drawing.Size(827, 253)
        Me.PropertyGridBottom.TabIndex = 5
        Me.PropertyGridBottom.ToolbarVisible = False
        Me.PropertyGridBottom.ViewBackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.PropertyGridBottom.ViewBorderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.PropertyGridBottom.ViewForeColor = System.Drawing.Color.WhiteSmoke
        '
        'LabelOtherDifferences
        '
        Me.LabelOtherDifferences.AutoSize = True
        Me.LabelOtherDifferences.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.LabelOtherDifferences.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.LabelOtherDifferences.Location = New System.Drawing.Point(237, 539)
        Me.LabelOtherDifferences.Name = "LabelOtherDifferences"
        Me.LabelOtherDifferences.Size = New System.Drawing.Size(164, 25)
        Me.LabelOtherDifferences.TabIndex = 6
        Me.LabelOtherDifferences.Text = "Other Differences:"
        Me.LabelOtherDifferences.Visible = False
        '
        'LabelOtherDifferencesResult
        '
        Me.LabelOtherDifferencesResult.AutoSize = True
        Me.LabelOtherDifferencesResult.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.LabelOtherDifferencesResult.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.LabelOtherDifferencesResult.Location = New System.Drawing.Point(401, 539)
        Me.LabelOtherDifferencesResult.Name = "LabelOtherDifferencesResult"
        Me.LabelOtherDifferencesResult.Size = New System.Drawing.Size(24, 25)
        Me.LabelOtherDifferencesResult.TabIndex = 7
        Me.LabelOtherDifferencesResult.Text = "..."
        Me.LabelOtherDifferencesResult.Visible = False
        '
        'OpenFileDialogTop
        '
        Me.OpenFileDialogTop.Filter = "MP3 Files (*.mp3)|*.mp3"
        Me.OpenFileDialogTop.ReadOnlyChecked = True
        Me.OpenFileDialogTop.ShowReadOnly = True
        Me.OpenFileDialogTop.SupportMultiDottedExtensions = True
        Me.OpenFileDialogTop.Title = "Seelct an MP3 file to load on the top area..."
        '
        'OpenFileDialogBottom
        '
        Me.OpenFileDialogBottom.Filter = "MP3 Files (*.mp3)|*.mp3"
        Me.OpenFileDialogBottom.ReadOnlyChecked = True
        Me.OpenFileDialogBottom.ShowReadOnly = True
        Me.OpenFileDialogBottom.SupportMultiDottedExtensions = True
        Me.OpenFileDialogBottom.Title = "Seelct an MP3 file to load on the bottom area..."
        '
        'LabelAudioStreamDiff
        '
        Me.LabelAudioStreamDiff.AutoSize = True
        Me.LabelAudioStreamDiff.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.LabelAudioStreamDiff.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.LabelAudioStreamDiff.Location = New System.Drawing.Point(8, 539)
        Me.LabelAudioStreamDiff.Name = "LabelAudioStreamDiff"
        Me.LabelAudioStreamDiff.Size = New System.Drawing.Size(155, 25)
        Me.LabelAudioStreamDiff.TabIndex = 8
        Me.LabelAudioStreamDiff.Text = "Audio Stream is..."
        Me.LabelAudioStreamDiff.Visible = False
        '
        'TableLayoutPanel_TopPict
        '
        Me.TableLayoutPanel_TopPict.ColumnCount = 1
        Me.TableLayoutPanel_TopPict.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel_TopPict.Controls.Add(Me.ButtonLoadFileTop, 0, 0)
        Me.TableLayoutPanel_TopPict.Controls.Add(Me.PictureBoxTop, 0, 1)
        Me.TableLayoutPanel_TopPict.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel_TopPict.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel_TopPict.Name = "TableLayoutPanel_TopPict"
        Me.TableLayoutPanel_TopPict.RowCount = 2
        Me.TableLayoutPanel_TopPict.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel_TopPict.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.TableLayoutPanel_TopPict.Size = New System.Drawing.Size(210, 253)
        Me.TableLayoutPanel_TopPict.TabIndex = 9
        '
        'TableLayoutPanel_BottomPict
        '
        Me.TableLayoutPanel_BottomPict.ColumnCount = 1
        Me.TableLayoutPanel_BottomPict.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel_BottomPict.Controls.Add(Me.ButtonLoadFileBottom, 0, 0)
        Me.TableLayoutPanel_BottomPict.Controls.Add(Me.PictureBoxBottom, 0, 1)
        Me.TableLayoutPanel_BottomPict.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel_BottomPict.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel_BottomPict.Name = "TableLayoutPanel_BottomPict"
        Me.TableLayoutPanel_BottomPict.RowCount = 2
        Me.TableLayoutPanel_BottomPict.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel_BottomPict.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.TableLayoutPanel_BottomPict.Size = New System.Drawing.Size(210, 253)
        Me.TableLayoutPanel_BottomPict.TabIndex = 10
        '
        'TableLayoutPanel_FullBottomArea
        '
        Me.TableLayoutPanel_FullBottomArea.ColumnCount = 2
        Me.TableLayoutPanel_FullBottomArea.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.TableLayoutPanel_FullBottomArea.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel_FullBottomArea.Controls.Add(Me.TableLayoutPanel_BottomPict, 0, 0)
        Me.TableLayoutPanel_FullBottomArea.Controls.Add(Me.PropertyGridBottom, 1, 0)
        Me.TableLayoutPanel_FullBottomArea.Location = New System.Drawing.Point(12, 275)
        Me.TableLayoutPanel_FullBottomArea.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel_FullBottomArea.Name = "TableLayoutPanel_FullBottomArea"
        Me.TableLayoutPanel_FullBottomArea.RowCount = 1
        Me.TableLayoutPanel_FullBottomArea.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel_FullBottomArea.Size = New System.Drawing.Size(1036, 253)
        Me.TableLayoutPanel_FullBottomArea.TabIndex = 11
        '
        'TableLayoutPanel_FullTopArea
        '
        Me.TableLayoutPanel_FullTopArea.ColumnCount = 2
        Me.TableLayoutPanel_FullTopArea.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.TableLayoutPanel_FullTopArea.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel_FullTopArea.Controls.Add(Me.TableLayoutPanel_TopPict, 0, 0)
        Me.TableLayoutPanel_FullTopArea.Controls.Add(Me.PropertyGridTop, 1, 0)
        Me.TableLayoutPanel_FullTopArea.Location = New System.Drawing.Point(13, 12)
        Me.TableLayoutPanel_FullTopArea.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel_FullTopArea.Name = "TableLayoutPanel_FullTopArea"
        Me.TableLayoutPanel_FullTopArea.RowCount = 1
        Me.TableLayoutPanel_FullTopArea.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel_FullTopArea.Size = New System.Drawing.Size(1036, 253)
        Me.TableLayoutPanel_FullTopArea.TabIndex = 12
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(26, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1064, 571)
        Me.Controls.Add(Me.TableLayoutPanel_FullTopArea)
        Me.Controls.Add(Me.TableLayoutPanel_FullBottomArea)
        Me.Controls.Add(Me.LabelOtherDifferences)
        Me.Controls.Add(Me.LabelAudioStreamDiff)
        Me.Controls.Add(Me.LabelOtherDifferencesResult)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MP3 Comparer"
        CType(Me.PictureBoxTop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxBottom, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel_TopPict.ResumeLayout(False)
        Me.TableLayoutPanel_BottomPict.ResumeLayout(False)
        Me.TableLayoutPanel_FullBottomArea.ResumeLayout(False)
        Me.TableLayoutPanel_FullTopArea.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PropertyGridTop As PropertyGrid
    Friend WithEvents ButtonLoadFileTop As Button
    Friend WithEvents PictureBoxTop As PictureBox
    Friend WithEvents ButtonLoadFileBottom As Button
    Friend WithEvents PictureBoxBottom As PictureBox
    Friend WithEvents PropertyGridBottom As PropertyGrid
    Friend WithEvents LabelOtherDifferences As Label
    Friend WithEvents LabelOtherDifferencesResult As Label
    Friend WithEvents OpenFileDialogTop As Ookii.Dialogs.WinForms.VistaOpenFileDialog
    Friend WithEvents OpenFileDialogBottom As Ookii.Dialogs.WinForms.VistaOpenFileDialog
    Friend WithEvents LabelAudioStreamDiff As Label
    Friend WithEvents TableLayoutPanel_TopPict As TableLayoutPanel
    Friend WithEvents TableLayoutPanel_BottomPict As TableLayoutPanel
    Friend WithEvents TableLayoutPanel_FullBottomArea As TableLayoutPanel
    Friend WithEvents TableLayoutPanel_FullTopArea As TableLayoutPanel
End Class
