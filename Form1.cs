//Copyright 2011-2012 Melvyn Laily
//http://arcanesanctum.net

//This file is part of MovieBarCodeGenerator.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MovieBarCode
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.Text += Application.ProductVersion;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			if (ofd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtPathIn.Text = ofd1.FileName;
			}
		}


		private void btnBrowseOut_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(ofd1.FileName))
			{
				sfd1.FileName = System.IO.Path.GetFileNameWithoutExtension(ofd1.FileName) + ".png";
			}
			if (sfd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtPathOut.Text = sfd1.FileName;
			}
		}

		private void btnProcess_Click(object sender, EventArgs e)
		{
			if (System.IO.File.Exists(txtPathIn.Text))
			{
				int width;
				int height;
				int iterations;
				int barWidth;
				try
				{
					width = int.Parse(txtWidth.Text);
					if (width <= 0)
					{
						throw new ArgumentException();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Width not valid!");
					return;
				}
				try
				{
					height = int.Parse(txtHeight.Text);
					if (height <= 0)
					{
						throw new ArgumentException();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Height not valid!");
					return;
				}
				try
				{
					iterations = int.Parse(txtIterations.Text);
					if (iterations <= 0)
					{
						throw new ArgumentException();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Iterations value not valid!");
					return;
				}
				try
				{
					barWidth = int.Parse(txtBarWidth.Text);
					if (barWidth <= 0)
					{
						throw new ArgumentException();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("Bar-width value not valid!");
					return;
				}
				txtPathOut.Text.TrimEnd(".".ToCharArray());
				if (txtPathOut.Text.Length <= 0)
				{
					txtPathOut.Text = System.IO.Path.GetFileNameWithoutExtension(ofd1.FileName) + ".png";
				}
				if (System.IO.Path.GetExtension(txtPathOut.Text).Length <= 0)
				{
					txtPathOut.Text += ".png";
				}
				if (System.IO.File.Exists(txtPathOut.Text))
				{
					if (MessageBox.Show(string.Format(@"The file '{0}' already exists. are you sure you want to overwrite it?", txtPathOut.Text), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
					{
						return;
					}
				}
				if (txtPathOut.Text == null || txtPathOut.Text.Length <= 0 || txtPathOut.Text.Any(c => System.IO.Path.GetInvalidPathChars().Contains(c)))
				{
					MessageBox.Show("Output path value not valid!");
					return;
				}
				progressBar1.Maximum = iterations;
				SetChildrenReadOnly(true);
				ParallelGeneration generationObject = new ParallelGeneration(txtPathIn.Text, txtPathOut.Text, width, height, iterations, barWidth);
				generationObject.GenerationComplete += (o2, e2) =>
				{
					try
					{
						this.Invoke((Action)(() => SetChildrenReadOnly(false)));
					}
					catch (Exception)
					{
						//application exited ?
					}
				};
				generationObject.ProgressChanged += (o3, e3) =>
				{
					try
					{
						this.Invoke((Action)(() => progressBar1.Value = (int)(e3.Progression * iterations)));
					}
					catch (Exception)
					{
						//application exited ?
					}
				};
				new System.Threading.Thread(() => generationObject.GenerateMovieBarCode()).Start();
			}
			else
			{
				MessageBox.Show("File not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
		}

		private void SetChildrenReadOnly(bool readOnly)
		{
			foreach (Control item in this.Controls)
			{
				item.Enabled = !readOnly;
			}
		}

		private void link1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			new System.Threading.Thread(
				new System.Threading.ThreadStart(
					() => System.Diagnostics.Process.Start(@"http://arcanesanctum.net"))).Start();
		}

		private void txtIterations_KeyUp(object sender, KeyEventArgs e)
		{
			if (!chkAutoCorrect.Checked)
			{
				return;
			}
			//barwidth = width/iterations
			int width;
			int iterations;
			try
			{
				width = int.Parse(txtWidth.Text);
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				iterations = int.Parse(txtIterations.Text);
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				int barWidth = (int)Math.Round((double)width / (double)iterations);
				txtBarWidth.Text = barWidth.ToString();
			}
			catch (Exception)
			{
			}
		}

		private void txtBarWidth_KeyUp(object sender, KeyEventArgs e)
		{
			if (!chkAutoCorrect.Checked)
			{
				return;
			}
			//iterations = width/barwidth
			int width;
			int barWidth;
			try
			{
				width = int.Parse(txtWidth.Text);
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				barWidth = int.Parse(txtBarWidth.Text);
			}
			catch (Exception)
			{
				return;
			}

			try
			{
				int iterations = (int)Math.Round((double)width / (double)barWidth);
				txtIterations.Text = iterations.ToString();
			}
			catch (Exception)
			{
			}
		}

	}
}