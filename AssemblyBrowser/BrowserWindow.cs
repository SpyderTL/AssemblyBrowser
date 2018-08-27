using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyBrowser
{
	public partial class BrowserWindow : Form
	{
		private string LastFindText = null;

		public BrowserWindow()
		{
			InitializeComponent();

			//var node = new AssemblyFile { Path = @"C:\Users\joshu\source\repos\TestCore\TestCore\bin\Debug\netcoreapp2.1\TestCore.dll" };

			//treeView.Nodes.Add(TreeNode(node));
		}

		private void Open()
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				var node = new AssemblyFile { Path = openFileDialog.FileName };

				treeView.Nodes.Add(TreeNode(node));
			}
		}

		private TreeNode TreeNode(object item)
		{
			var node = new TreeNode
			{
				Text = item.ToString(),
				Tag = item
			};

			if (item is IFolder)
				node.Nodes.Add("Loading...");

			return node;
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is IFolder)
			{
				e.Node.Nodes.Clear();

				e.Node.Nodes.AddRange(((IFolder)e.Node.Tag).Items.Select(TreeNode).ToArray());
			}
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid.SelectedObject = e.Node.Tag is IProperties ? ((IProperties)e.Node.Tag).Properties : null;
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Find();
		}

		private void Find()
		{
			var result = Microsoft.VisualBasic.Interaction.InputBox("Search Text", "Find");

			LastFindText = result;

			FindNext();
		}

		private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FindNext();
		}

		private void FindNext()
		{
			if(LastFindText == null)
				return;

			foreach (var node in treeView.Nodes.Cast<TreeNode>())
			{
				if (node.Text.Contains(LastFindText))
				{
					treeView.SelectedNode = node;
					return;
				}

				if (Find(node.Nodes))
					return;
			}

			MessageBox.Show("No more instances found.");
		}

		private bool Find(TreeNodeCollection nodes)
		{
			foreach (var node in nodes.Cast<TreeNode>())
			{
				if (node.Text.Contains(LastFindText))
				{
					treeView.SelectedNode = node;
					return true;
				}

				if (Find(node.Nodes))
					return true;
			}

			return false;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open();
		}
	}
}
