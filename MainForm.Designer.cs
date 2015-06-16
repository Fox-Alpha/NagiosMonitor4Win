/*
 * Erstellt mit SharpDevelop.
 * Benutzer: buck
 * Datum: 08.06.2015
 * Zeit: 13:40
 * 
 */
namespace NagiosMonitor
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageStatus;
		private System.Windows.Forms.TabPage tabPageGroupSelection;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TreeView tvNagiosGroups;
		private System.Windows.Forms.Timer statusTimer;
		private System.Windows.Forms.ToolStripMenuItem programmToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nagiosToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verbindungsmanagerToolStripMenuItem;
		private System.Windows.Forms.ToolStripDropDownButton tsddButtConnections;
		private System.Windows.Forms.RichTextBox rtbNagiosDebug;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ListView lvGlobalNagiosStatus;
		private System.Windows.Forms.TabPage tabPageServerStatus;
		private System.Windows.Forms.SplitContainer splitContainer1;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Aktueller Status");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Host Gruppen");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Service Gruppen");
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsddButtConnections = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.programmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nagiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verbindungsmanagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageStatus = new System.Windows.Forms.TabPage();
			this.lvGlobalNagiosStatus = new System.Windows.Forms.ListView();
			this.rtbNagiosDebug = new System.Windows.Forms.RichTextBox();
			this.tabPageGroupSelection = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tvNagiosGroups = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabPageServerStatus = new System.Windows.Forms.TabPage();
			this.statusTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.tabControlMain.SuspendLayout();
			this.tabPageStatus.SuspendLayout();
			this.tabPageGroupSelection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsddButtConnections,
			this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 466);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(773, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tsddButtConnections
			// 
			this.tsddButtConnections.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsddButtConnections.Image = ((System.Drawing.Image)(resources.GetObject("tsddButtConnections.Image")));
			this.tsddButtConnections.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddButtConnections.Name = "tsddButtConnections";
			this.tsddButtConnections.Size = new System.Drawing.Size(29, 20);
			this.tsddButtConnections.Text = "toolStripDropDownButton1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(773, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.programmToolStripMenuItem,
			this.nagiosToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(773, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// programmToolStripMenuItem
			// 
			this.programmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.beendenToolStripMenuItem});
			this.programmToolStripMenuItem.Name = "programmToolStripMenuItem";
			this.programmToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
			this.programmToolStripMenuItem.Text = "Programm";
			// 
			// beendenToolStripMenuItem
			// 
			this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
			this.beendenToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
			this.beendenToolStripMenuItem.Text = "Beenden";
			// 
			// nagiosToolStripMenuItem
			// 
			this.nagiosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.verbindungsmanagerToolStripMenuItem});
			this.nagiosToolStripMenuItem.Name = "nagiosToolStripMenuItem";
			this.nagiosToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.nagiosToolStripMenuItem.Text = "Nagios";
			// 
			// verbindungsmanagerToolStripMenuItem
			// 
			this.verbindungsmanagerToolStripMenuItem.Name = "verbindungsmanagerToolStripMenuItem";
			this.verbindungsmanagerToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.verbindungsmanagerToolStripMenuItem.Text = "Verbindungsmanager";
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageStatus);
			this.tabControlMain.Controls.Add(this.tabPageGroupSelection);
			this.tabControlMain.Controls.Add(this.tabPageServerStatus);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 49);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(773, 417);
			this.tabControlMain.TabIndex = 3;
			// 
			// tabPageStatus
			// 
			this.tabPageStatus.Controls.Add(this.lvGlobalNagiosStatus);
			this.tabPageStatus.Controls.Add(this.rtbNagiosDebug);
			this.tabPageStatus.Location = new System.Drawing.Point(4, 22);
			this.tabPageStatus.Name = "tabPageStatus";
			this.tabPageStatus.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStatus.Size = new System.Drawing.Size(765, 391);
			this.tabPageStatus.TabIndex = 0;
			this.tabPageStatus.Text = "Status Übersicht";
			this.tabPageStatus.UseVisualStyleBackColor = true;
			// 
			// lvGlobalNagiosStatus
			// 
			this.lvGlobalNagiosStatus.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.lvGlobalNagiosStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvGlobalNagiosStatus.FullRowSelect = true;
			this.lvGlobalNagiosStatus.GridLines = true;
			this.lvGlobalNagiosStatus.HoverSelection = true;
			this.lvGlobalNagiosStatus.Location = new System.Drawing.Point(3, 3);
			this.lvGlobalNagiosStatus.MultiSelect = false;
			this.lvGlobalNagiosStatus.Name = "lvGlobalNagiosStatus";
			this.lvGlobalNagiosStatus.Size = new System.Drawing.Size(759, 287);
			this.lvGlobalNagiosStatus.TabIndex = 1;
			this.lvGlobalNagiosStatus.UseCompatibleStateImageBehavior = false;
			this.lvGlobalNagiosStatus.View = System.Windows.Forms.View.Details;
			// 
			// rtbNagiosDebug
			// 
			this.rtbNagiosDebug.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.rtbNagiosDebug.Location = new System.Drawing.Point(3, 290);
			this.rtbNagiosDebug.Name = "rtbNagiosDebug";
			this.rtbNagiosDebug.Size = new System.Drawing.Size(759, 98);
			this.rtbNagiosDebug.TabIndex = 0;
			this.rtbNagiosDebug.Text = "";
			// 
			// tabPageGroupSelection
			// 
			this.tabPageGroupSelection.Controls.Add(this.splitContainer1);
			this.tabPageGroupSelection.Location = new System.Drawing.Point(4, 22);
			this.tabPageGroupSelection.Name = "tabPageGroupSelection";
			this.tabPageGroupSelection.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageGroupSelection.Size = new System.Drawing.Size(765, 391);
			this.tabPageGroupSelection.TabIndex = 1;
			this.tabPageGroupSelection.Text = "Gruppen Übersicht";
			this.tabPageGroupSelection.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tvNagiosGroups);
			this.splitContainer1.Panel1MinSize = 150;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			this.splitContainer1.Size = new System.Drawing.Size(759, 385);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.SplitterWidth = 6;
			this.splitContainer1.TabIndex = 2;
			// 
			// tvNagiosGroups
			// 
			this.tvNagiosGroups.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvNagiosGroups.FullRowSelect = true;
			this.tvNagiosGroups.HideSelection = false;
			this.tvNagiosGroups.Location = new System.Drawing.Point(0, 0);
			this.tvNagiosGroups.Name = "tvNagiosGroups";
			treeNode1.Name = "currState";
			treeNode1.Tag = "1";
			treeNode1.Text = "Aktueller Status";
			treeNode1.ToolTipText = "Zeigt den aktuellen Stand vom Nagios Monitoring an";
			treeNode2.Name = "hostGroups";
			treeNode2.Tag = "200";
			treeNode2.Text = "Host Gruppen";
			treeNode2.ToolTipText = "Zeigt die konfigurierten Hostgruppen";
			treeNode3.Name = "serviceGroups";
			treeNode3.Tag = "500";
			treeNode3.Text = "Service Gruppen";
			this.tvNagiosGroups.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
			treeNode1,
			treeNode2,
			treeNode3});
			this.tvNagiosGroups.ShowNodeToolTips = true;
			this.tvNagiosGroups.Size = new System.Drawing.Size(200, 385);
			this.tvNagiosGroups.TabIndex = 1;
			this.tvNagiosGroups.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvNagiosGroups_BeforeExpand);
			this.tvNagiosGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvNagiosGroups_AfterSelect);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(553, 385);
			this.panel1.TabIndex = 2;
			// 
			// tabPageServerStatus
			// 
			this.tabPageServerStatus.Location = new System.Drawing.Point(4, 22);
			this.tabPageServerStatus.Name = "tabPageServerStatus";
			this.tabPageServerStatus.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageServerStatus.Size = new System.Drawing.Size(765, 391);
			this.tabPageServerStatus.TabIndex = 2;
			this.tabPageServerStatus.Text = "Nagios Server Status";
			this.tabPageServerStatus.UseVisualStyleBackColor = true;
			// 
			// statusTimer
			// 
			this.statusTimer.Interval = 30000;
			this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(773, 488);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "NagiosMonitor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControlMain.ResumeLayout(false);
			this.tabPageStatus.ResumeLayout(false);
			this.tabPageGroupSelection.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
