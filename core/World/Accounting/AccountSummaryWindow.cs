#region LICENSE
/*
 * Copyright (C) 2007 - 2008 FreeTrain Team (http://freetrain.sourceforge.net)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
#endregion LICENSE

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using FreeTrain.Framework;
using FreeTrain.Framework.Plugin;
using FreeTrain.Util;

namespace FreeTrain.World.Accounting
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountSummaryWindow : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public AccountSummaryWindow()
        {
            InitializeComponent();

            // register an event handler
            AccountManager.onAccountChange += new AccountListener(onAccountChanged);

            WorldDefinition.onNewWorld += new EventHandler(onNewWorld);

            populateListView();
            onAccountChanged();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            // initialize the font
            if (options.font != null) setFont(options.font.createFont());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(System.EventArgs e)
        {
            AccountManager.onAccountChange -= new AccountListener(onAccountChanged);
            options.save();
        }





        /// <summary>
        /// Short-cut to the AccountingManager.
        /// </summary>
        private AccountManager manager { get { return AccountManager.theInstance; } }

        /// <summary>
        /// Persistent setting.
        /// </summary>
        private Options options = new Options().load();

        /// <summary>
        /// Select a summary from a history by using the currently selected mode.
        /// </summary>
        internal delegate TransactionSummary Selector(TransactionHistory history);
        private Selector select = new Selector(selectDay);

        /// <summary>
        /// Update the displayed data of list view items.
        /// </summary>
        private AccountListener updateItems;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Set up items in the list view according to <code>options.genres</code>.
        /// </summary>
        private void populateListView()
        {
            detailView.BeginUpdate();
            detailView.Items.Clear();
            foreach (AccountGenre g in options.genres)
            {
                ListViewItem lvi = new GenreListItem(this, g);
                detailView.Items.Add(lvi);
            }
            updateItems();	// fill in the sub texts
            detailView.EndUpdate();
        }

        private void onAccountChanged()
        {
            liquidAsset.Text = CurrencyUtil.format(manager.liquidAssets);
            debts.Text = CurrencyUtil.format(manager.totalDebts);
        }

        private void onNewWorld(object sender, EventArgs e)
        {
            // update all the entiries
            onAccountChanged();
            populateListView();
        }


        private void setFont(Font f)
        {
            this.Font = f;
            detailView.Font = f;
            fontDialog.Font = f;
            options.font = new FontInfo(f);
            options.save();
        }

        //
        // selector methods
        //
        private static TransactionSummary selectDay(TransactionHistory history)
        {
            return history.day;
        }
        private static TransactionSummary selectMonth(TransactionHistory history)
        {
            return history.month;
        }
        private static TransactionSummary selectYear(TransactionHistory history)
        {
            return history.year;
        }


        /// <summary> Change font of the dialog. </summary>
        private void onChangeFont(object sender, System.EventArgs e)
        {
            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                setFont(fontDialog.Font);
            }
        }

        private void onSelectorChanged(object sender, System.EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0: select = new Selector(selectDay); break;
                case 1: select = new Selector(selectMonth); break;
                case 2: select = new Selector(selectYear); break;
            }
            updateItems();	// update the data on the screen
        }

        private void onCustomizeGenres(object sender, System.EventArgs e)
        {
            using (GenreSelectorDialog dialog = new GenreSelectorDialog(options.genres))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    // update this window
                    options.genres = dialog.selected;
                    populateListView();
                }
            }
        }

        #region GUI components
        private System.Windows.Forms.MenuItem menuItem_Graph;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label liquidAsset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader colCategory;
        private System.Windows.Forms.ColumnHeader colSales;
        private System.Windows.Forms.ColumnHeader colCost;
        private System.Windows.Forms.ColumnHeader colBalance;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        //private Crownwood.Magic.Controls.TabPage tabPage1;
        //private Crownwood.Magic.Controls.TabPage tabPage2;
        //private Crownwood.Magic.Controls.TabPage tabPage3;
        TabPage tabPage1;
        TabPage tabPage2;
        TabPage tabPage3;
        private System.Windows.Forms.Label debts;
        private System.Windows.Forms.ListView detailView;
        //private Crownwood.Magic.Controls.TabControl tabControl;
        TabControl tabControl;
        private System.ComponentModel.Container components = null;
        #endregion
        #region Windows Form Designer generated code
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rail",
            "TODO"}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128))));
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_Graph = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.liquidAsset = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.debts = new System.Windows.Forms.Label();
            this.detailView = new System.Windows.Forms.ListView();
            this.colCategory = new System.Windows.Forms.ColumnHeader();
            this.colSales = new System.Windows.Forms.ColumnHeader();
            this.colCost = new System.Windows.Forms.ColumnHeader();
            this.colBalance = new System.Windows.Forms.ColumnHeader();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.ContextMenu = this.contextMenu;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Funds:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem_Graph,
            this.menuItem4,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Change &Font...";
            this.menuItem1.Click += new System.EventHandler(this.onChangeFont);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "&Edit Displayed Items...";
            this.menuItem2.Click += new System.EventHandler(this.onCustomizeGenres);
            // 
            // menuItem_Graph
            // 
            this.menuItem_Graph.Index = 2;
            this.menuItem_Graph.Text = "Display &Graph";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.Text = "-";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 4;
            this.menuItem3.Text = "&Close";
            // 
            // liquidAsset
            // 
            this.liquidAsset.ContextMenu = this.contextMenu;
            this.liquidAsset.Location = new System.Drawing.Point(56, 0);
            this.liquidAsset.Name = "liquidAsset";
            this.liquidAsset.Size = new System.Drawing.Size(112, 26);
            this.liquidAsset.TabIndex = 1;
            this.liquidAsset.Text = "100,000,000";
            this.liquidAsset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.ContextMenu = this.contextMenu;
            this.label2.Location = new System.Drawing.Point(168, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Loans:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // debts
            // 
            this.debts.ContextMenu = this.contextMenu;
            this.debts.Location = new System.Drawing.Point(224, 0);
            this.debts.Name = "debts";
            this.debts.Size = new System.Drawing.Size(112, 26);
            this.debts.TabIndex = 3;
            this.debts.Text = "100,000,000";
            this.debts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // detailView
            // 
            this.detailView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.detailView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCategory,
            this.colSales,
            this.colCost,
            this.colBalance});
            this.detailView.ContextMenu = this.contextMenu;
            this.detailView.FullRowSelect = true;
            this.detailView.GridLines = true;
            this.detailView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.detailView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.detailView.Location = new System.Drawing.Point(0, 23);
            this.detailView.MultiSelect = false;
            this.detailView.Name = "detailView";
            this.detailView.Size = new System.Drawing.Size(338, 79);
            this.detailView.TabIndex = 4;
            this.detailView.UseCompatibleStateImageBehavior = false;
            this.detailView.View = System.Windows.Forms.View.Details;
            // 
            // colCategory
            // 
            this.colCategory.Text = "Category";
            this.colCategory.Width = 80;
            // 
            // colSales
            // 
            this.colSales.Text = "Sales";
            this.colSales.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSales.Width = 84;
            // 
            // colCost
            // 
            this.colCost.Text = "Cost";
            this.colCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colCost.Width = 84;
            // 
            // colBalance
            // 
            this.colBalance.Text = "Balance";
            this.colBalance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colBalance.Width = 84;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.ContextMenu = this.contextMenu;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 102);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(322, 26);
            this.tabControl.TabIndex = 5;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.onSelectorChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(314, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Today";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(314, 0);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "This month";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(314, 0);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "This year";
            // 
            // AccountSummaryWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(340, 126);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.detailView);
            this.Controls.Add(this.debts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.liquidAsset);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(348, 52);
            this.Name = "AccountSummaryWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Sales Report";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion




        /// <summary>
        /// Manage ListViewItem and display information about an account genre.
        /// </summary>
        private class GenreListItem : ListViewItem, IDisposable
        {
            private readonly AccountGenre genre;
            private readonly AccountSummaryWindow parent;

            /// <summary> History object that whose value we are displaying. </summary>
            private TransactionHistory history { get { return genre.History; } }

            internal GenreListItem(AccountSummaryWindow _parent, AccountGenre genre)
            {
                this.parent = _parent;
                this.genre = genre;
                this.Text = genre.name;

                this.SubItems.Add("0");
                this.SubItems.Add("0");
                this.SubItems.Add("0");
                onUpdate();

                genre.onUpdate += new AccountListener(onUpdate);
                parent.updateItems += new AccountListener(onUpdate);
            }
            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                // disconnect
                genre.onUpdate -= new AccountListener(onUpdate);
                parent.updateItems -= new AccountListener(onUpdate);
            }

            /// <summary> Update data on the screen. </summary>
            private void onUpdate()
            {
                TransactionSummary s = parent.select(history);
                this.SubItems[1].Text = CurrencyUtil.format(s.sales);
                this.SubItems[2].Text = CurrencyUtil.format(s.expenditures);
                this.SubItems[3].Text = CurrencyUtil.format(s.balance);
            }
        }



        /// <summary>
        /// Persistent information of this dialog.
        /// </summary>
        public class Options : PersistentOptions
        {
            /// <summary> display font. </summary>
            public FontInfo font;

            /// <summary>
            /// List of displayed genre ids.
            /// Public only for XmlSerializer.
            /// </summary>
            /// 
            string[] genre;

            /// <summary>
            /// 
            /// </summary>
            [XmlElement("genre")]
            public string[] Genre
            {
                get { return genre; }
                set { genre = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            [XmlIgnore()]
            public AccountGenre[] genres
            {
                get
                {
                    try
                    {
                        AccountGenre[] r = new AccountGenre[genre.Length];
                        for (int i = 0; i < r.Length; i++)
                            r[i] = (AccountGenre)PluginManager.GetContribution(genre[i]);
                        return r;
                    }
                    catch (Exception e)
                    {
                        // recover from missing plug-in error by returning a default list.
                        Debug.WriteLine(e.StackTrace);
                        return new AccountGenre[] {
							AccountGenre.RailService,
							AccountGenre.RoadService,
							AccountGenre.Subsidiaries
						};
                    }
                }
                set
                {
                    genre = new string[value.Length];
                    for (int i = 0; i < value.Length; i++)
                        genre[i] = value[i].Id;
                    save();
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public new Options load()
            {
                return (Options)base.load();
            }
        }



    }
}
