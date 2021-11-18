using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanlyBanSach
{
	public partial class BillForm : Form
	{
		public BillForm()
		{
			InitializeComponent();
		}

		private void lvBill_DoubleClick(object sender, EventArgs e)
		{
			BillDetailsForm bdt = new BillDetailsForm();
			bdt.ShowDialog();
		}
	}
}
