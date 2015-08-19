using Gin.Editors;

namespace Gin.Builder
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Первая группа", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Вторая группа", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOpenPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSavePackage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSavePackageAs = new System.Windows.Forms.ToolStripMenuItem();
            this.экспортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выполнитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.statusAppStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusFilePath = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.listCommands = new GinCommandsToolBox();
            this.imageListTree = new System.Windows.Forms.ImageList(this.components);
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.treeCommands = new System.Windows.Forms.TreeView();
            this.splitContainerArguments = new System.Windows.Forms.SplitContainer();
            this.panelCommandProperties = new System.Windows.Forms.FlowLayoutPanel();
            this.argumentHelp = new ArgumentHelpControl();
            this.menuMain.SuspendLayout();
            this.statusMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            this.splitContainerArguments.Panel1.SuspendLayout();
            this.splitContainerArguments.Panel2.SuspendLayout();
            this.splitContainerArguments.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(928, 24);
            this.menuMain.TabIndex = 1;
            this.menuMain.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewPackage,
            this.menuItemOpenPackage,
            this.menuItemSavePackage,
            this.menuItemSavePackageAs,
            this.экспортToolStripMenuItem,
            this.выполнитьToolStripMenuItem,
            this.menuItemExit});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // menuItemNewPackage
            // 
            this.menuItemNewPackage.Name = "menuItemNewPackage";
            this.menuItemNewPackage.Size = new System.Drawing.Size(173, 22);
            this.menuItemNewPackage.Text = "Создать";
            this.menuItemNewPackage.Click += new System.EventHandler(this.menuItemNewPackage_Click);
            // 
            // menuItemOpenPackage
            // 
            this.menuItemOpenPackage.Name = "menuItemOpenPackage";
            this.menuItemOpenPackage.Size = new System.Drawing.Size(173, 22);
            this.menuItemOpenPackage.Text = "Открыть...";
            this.menuItemOpenPackage.Click += new System.EventHandler(this.MenuItemOpenPackageClick);
            // 
            // menuItemSavePackage
            // 
            this.menuItemSavePackage.Name = "menuItemSavePackage";
            this.menuItemSavePackage.Size = new System.Drawing.Size(173, 22);
            this.menuItemSavePackage.Text = "Сохранить";
            this.menuItemSavePackage.Click += new System.EventHandler(this.menuItemSavePackage_Click);
            // 
            // menuItemSavePackageAs
            // 
            this.menuItemSavePackageAs.Name = "menuItemSavePackageAs";
            this.menuItemSavePackageAs.Size = new System.Drawing.Size(173, 22);
            this.menuItemSavePackageAs.Text = "Сохранить как...";
            this.menuItemSavePackageAs.Click += new System.EventHandler(this.menuItemSavePackageAs_Click);
            // 
            // экспортToolStripMenuItem
            // 
            this.экспортToolStripMenuItem.Name = "экспортToolStripMenuItem";
            this.экспортToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.экспортToolStripMenuItem.Text = "Экспорт...";
            this.экспортToolStripMenuItem.Click += new System.EventHandler(this.MenuItemExportClick);
            // 
            // выполнитьToolStripMenuItem
            // 
            this.выполнитьToolStripMenuItem.Name = "выполнитьToolStripMenuItem";
            this.выполнитьToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.выполнитьToolStripMenuItem.Text = "Выполнить";
            this.выполнитьToolStripMenuItem.Click += new System.EventHandler(this.executeToolStripMenuItem_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(173, 22);
            this.menuItemExit.Text = "Выход";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // statusMain
            // 
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusAppStatus,
            this.statusFilePath});
            this.statusMain.Location = new System.Drawing.Point(0, 562);
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(928, 22);
            this.statusMain.TabIndex = 3;
            this.statusMain.Text = "statusStrip1";
            // 
            // statusAppStatus
            // 
            this.statusAppStatus.Name = "statusAppStatus";
            this.statusAppStatus.Size = new System.Drawing.Size(103, 17);
            this.statusAppStatus.Text = "Старт приложения";
            // 
            // statusFilePath
            // 
            this.statusFilePath.Name = "statusFilePath";
            this.statusFilePath.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.ForeColor = System.Drawing.SystemColors.ControlText;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.listCommands);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerRight);
            this.splitContainerMain.Size = new System.Drawing.Size(928, 538);
            this.splitContainerMain.SplitterDistance = 267;
            this.splitContainerMain.SplitterWidth = 2;
            this.splitContainerMain.TabIndex = 4;
            // 
            // listCommands
            // 
            this.listCommands.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listCommands.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCommands.FullRowSelect = true;
            this.listCommands.GridLines = true;
            listViewGroup1.Header = "Первая группа";
            listViewGroup1.Name = "listViewGroup6";
            listViewGroup2.Header = "Вторая группа";
            listViewGroup2.Name = "listViewGroup1";
            this.listCommands.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listCommands.LargeImageList = this.imageListTree;
            this.listCommands.Location = new System.Drawing.Point(0, 0);
            this.listCommands.MultiSelect = false;
            this.listCommands.Name = "listCommands";
            this.listCommands.ShowItemToolTips = true;
            this.listCommands.Size = new System.Drawing.Size(265, 536);
            this.listCommands.SmallImageList = this.imageListTree;
            this.listCommands.StateImageList = this.imageListTree;
            this.listCommands.TabIndex = 0;
            this.listCommands.UseCompatibleStateImageBehavior = false;
            this.listCommands.View = System.Windows.Forms.View.Tile;
            // 
            // imageListTree
            // 
            this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
            this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTree.Images.SetKeyName(0, "command");
            this.imageListTree.Images.SetKeyName(1, "empty");
            this.imageListTree.Images.SetKeyName(2, "package");
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.treeCommands);
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.splitContainerArguments);
            this.splitContainerRight.Size = new System.Drawing.Size(657, 536);
            this.splitContainerRight.SplitterDistance = 388;
            this.splitContainerRight.TabIndex = 1;
            // 
            // treeCommands
            // 
            this.treeCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCommands.ImageIndex = 0;
            this.treeCommands.ImageList = this.imageListTree;
            this.treeCommands.Location = new System.Drawing.Point(0, 0);
            this.treeCommands.Name = "treeCommands";
            this.treeCommands.SelectedImageIndex = 0;
            this.treeCommands.ShowRootLines = false;
            this.treeCommands.Size = new System.Drawing.Size(388, 536);
            this.treeCommands.TabIndex = 0;
            // 
            // splitContainerArguments
            // 
            this.splitContainerArguments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerArguments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerArguments.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerArguments.Location = new System.Drawing.Point(0, 0);
            this.splitContainerArguments.Name = "splitContainerArguments";
            this.splitContainerArguments.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerArguments.Panel1
            // 
            this.splitContainerArguments.Panel1.Controls.Add(this.panelCommandProperties);
            // 
            // splitContainerArguments.Panel2
            // 
            this.splitContainerArguments.Panel2.Controls.Add(this.argumentHelp);
            this.splitContainerArguments.Size = new System.Drawing.Size(265, 536);
            this.splitContainerArguments.SplitterDistance = 264;
            this.splitContainerArguments.TabIndex = 1;
            // 
            // panelCommandProperties
            // 
            this.panelCommandProperties.AutoScroll = true;
            this.panelCommandProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCommandProperties.Location = new System.Drawing.Point(0, 0);
            this.panelCommandProperties.Name = "panelCommandProperties";
            this.panelCommandProperties.Size = new System.Drawing.Size(263, 262);
            this.panelCommandProperties.TabIndex = 0;
            this.panelCommandProperties.Resize += new System.EventHandler(this.panelCommandProperties_Resize);
            // 
            // argumentHelp
            // 
            this.argumentHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argumentHelp.Location = new System.Drawing.Point(0, 0);
            this.argumentHelp.Name = "argumentHelp";
            this.argumentHelp.Size = new System.Drawing.Size(263, 266);
            this.argumentHelp.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 584);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gin Builder";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            this.splitContainerRight.ResumeLayout(false);
            this.splitContainerArguments.Panel1.ResumeLayout(false);
            this.splitContainerArguments.Panel2.ResumeLayout(false);
            this.splitContainerArguments.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpenPackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemSavePackageAs;
        private System.Windows.Forms.ToolStripMenuItem menuItemSavePackage;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.ToolStripStatusLabel statusAppStatus;
        private GinCommandsToolBox listCommands;
        private System.Windows.Forms.TreeView treeCommands;
        private System.Windows.Forms.ToolStripMenuItem экспортToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel panelCommandProperties;
        private System.Windows.Forms.ToolStripMenuItem menuItemNewPackage;
        private System.Windows.Forms.SplitContainer splitContainerArguments;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.ImageList imageListTree;
        private System.Windows.Forms.ToolStripMenuItem выполнитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusFilePath;
        private ArgumentHelpControl argumentHelp;


    }
}

