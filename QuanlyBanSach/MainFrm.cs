using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace QuanlyBanSach
{
	public partial class MainFrm : Form
	{
		
		private Form currentChildForm;
		public MainFrm()
		{
			InitializeComponent();
		}

		private void MainFrm_Load(object sender, EventArgs e)
		{
			LoginFrm login = new LoginFrm();
			if(login.ShowDialog()!=DialogResult.OK)
			{
				Application.Exit();
			}
			this.Text = string.Empty;
			this.ControlBox = false;
			this.DoubleBuffered = true;
			this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
		}

		private void btnMinimum_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void btnMaxSize_Click(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Normal)
				WindowState = FormWindowState.Maximized;
			else
				WindowState = FormWindowState.Normal;
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		[DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.DLL", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

		private void guna2Panel2_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}

		private void OpenChildForm(Form childform)
		{
			if(currentChildForm !=null)
			{
				currentChildForm.Close();
			}
			currentChildForm = childform;
			childform.TopLevel = false;
			childform.FormBorderStyle = FormBorderStyle.None;
			childform.Dock = DockStyle.Fill;
			PanelMainFrm.Controls.Add(childform);
			PanelMainFrm.Tag = childform;
			childform.BringToFront();
			childform.Show();
		}

		private void btnFrmTinhTien_Click(object sender, EventArgs e)
		{
			OpenChildForm(new FormTinhTien());
		}

		private void btnLogOut_Click(object sender, EventArgs e)
		{
			this.Hide();
			LoginFrm login = new LoginFrm();
			login.Show();
			
		}

		private void btnBill_Click(object sender, EventArgs e)
		{
			OpenChildForm(new BillForm());
		}

		private void btnStorage_Click(object sender, EventArgs e)
		{
			OpenChildForm(new StorageForm());
		}

		private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
