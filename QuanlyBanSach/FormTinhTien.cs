using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QuanlyBanSach
{
	public partial class FormTinhTien : Form
	{
		string currentBillID = null;
		DataTable dt;
		DataTable table = new DataTable();
		////hàm kết nối SQL
		//SqlConnection connection;
		////hàm lệnh
		//SqlCommand command;
		//// chứa chuỗi kết nối
		//string str = @"Data Source=DESKTOP-ONTGILH\SQLEXPRESS;Initial Catalog=QLNhaSachN3;Integrated Security=True";
		//// hàm load dữ liệu lên DataGridView
		//SqlDataAdapter adapter = new SqlDataAdapter();
		//// nhận dữ liệu và đẩy xuống table
		//DataTable table = new DataTable();
		// Hiển thị dữ liệu sản phẩm
		//void LoadDataSP()
  //      {
		//	// load dữ liệu lên
		//	command = connection.CreateCommand();
		//	// tạo xử lý kết nối đến sản phẩm
		//	command.CommandText = "select * from dbo.SanPham";
		//	//thực thi truy vấn
		//	adapter.SelectCommand = command;
		//	// clear table
		//	table.Clear();
		//	// truyền dữ liệu adapter tới table
		//	adapter.Fill(table);
		//	dgvSanPham.DataSource = table;

  //      }
		public FormTinhTien()
		{
			InitializeComponent();
		}

        private void FormTinhTien_Load(object sender, EventArgs e)
        {
			LoadSanPham(dgvSanPham);	
		}

		public void LoadSanPham(DataGridView dgv)
		{
			DataGridView dgvCon = new DataGridView();		
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection sqlConnection = new SqlConnection(connectionString);

			// tạo đối tượng thực thi lệnh
			SqlCommand sqlCommand = sqlConnection.CreateCommand();

			sqlConnection.Open();

			sqlCommand.CommandText = "select idSanPham,TenSP,GiaSP,TenTacGia,NXB,HangSX,SoLuongTon from SanPham";

			//Tạo đối tượng DataAdapter
			SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

			// tạo datatable để chứa dữ liệu

			da.Fill(table);

			// Hiển thị danh sách món ăn lên form
			dgv.DataSource = table;

			dgv.Columns["idSanPham"].HeaderText = "Mã sản phẩm";
			dgv.Columns["TenSP"].HeaderText = "Tên sản phẩm";
			dgv.Columns["GiaSP"].HeaderText = "Giá sản phẩm";
			dgv.Columns["TenTacGia"].HeaderText = "Tên tác giả";
			dgv.Columns["NXB"].HeaderText = "Nhà xuất bản";
			dgv.Columns["HangSX"].HeaderText = "Hãng sản xuất";
			dgv.Columns["SoLuongTon"].HeaderText = "Số lượng còn";

			// đóng kết nối và giải phóng bộ nhớ
			sqlConnection.Close();
			sqlConnection.Dispose();
			da.Dispose();
		}
		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			if (table == null) return;
			string filterExpression = "TenSP like '%" + txtSearch.Text + "%'";
			string sortExpression = "GiaSP DESC";
			DataViewRowState rowStateFilter = DataViewRowState.OriginalRows;

			DataView foodView = new DataView(table, filterExpression, sortExpression, rowStateFilter);
			dgvSanPham.DataSource = foodView;
		}


		#region
		/// <summary>
		/// Hàm thêm một sản phẩm vào chi tiết hoá đơn
		/// </summary>
		/// <param name="idProduct"> Mã sản phẩm được thêm</param>
		/// <param name="donGia"> Giá của sản phẩm được thêm</param>
		public void AddProduct(string idProduct, float donGia)
		{
			if (currentBillID == null) MessageBox.Show("Hãy Thêm Hoá Đơn");
			else
			{

				dgvChitietdonHang.Columns.Clear();
				// Tạo đối tượng kết nối
				string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
				SqlConnection sqlConnection = new SqlConnection(connectionString);
				int idLimit = 7;
				string id = "CT" + ZeroAppend("000000" + AutoIncreatementIDBillDetail(), idLimit);
				// tạo đối tượng thực thi lệnh
				SqlCommand sqlCommand = sqlConnection.CreateCommand();

				sqlCommand.CommandText = "EXECUTE BillDetail_Insert @id, @soLuong, @donGia, @idHD,@idSP";

				sqlCommand.Parameters.Add("@id", SqlDbType.NVarChar, 20).Value = id;
				sqlCommand.Parameters.Add("@soLuong", SqlDbType.Float).Value = 1;
				sqlCommand.Parameters.Add("@donGia", SqlDbType.Float).Value = donGia;
				sqlCommand.Parameters.Add("@idHD", SqlDbType.NVarChar, 20).Value = currentBillID;
				sqlCommand.Parameters.Add("@idSP", SqlDbType.NVarChar, 20).Value = idProduct;

				sqlConnection.Open();
				sqlCommand.ExecuteNonQuery();

				AmmountUpdate();
				TongKetHoaDon();

				sqlCommand.CommandText = $"SELECT * from ChiTietHoaDon where idHoaDon = '{currentBillID}'";
				//Tạo đối tượng DataAdapter
				SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

				dt = new DataTable();
				// tạo datatable để chứa dữ liệu
				da.Fill(dt);

				// Hiển thị danh sách món ăn lên form
				dgvChitietdonHang.DataSource = dt;
				dgvChitietdonHang.Columns[0].ReadOnly = true;


				// đóng kết nối và giải phóng bộ nhớ
				sqlConnection.Close();
				sqlConnection.Dispose();
				da.Dispose();
			}
		}

		/// <summary>
		/// Hàm tổng kết hoá đơn, xuất thông tin ra cho người dùng
		/// </summary>
		private void TongKetHoaDon()
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = conn.CreateCommand();

			conn.Open();
			cmd.CommandText = $"select * from HoaDon where idHoaDon = '{currentBillID}'";
			SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
				dpkNgayBan.Value = DateTime.Parse(reader["NgayBan"].ToString());
				cbbTinhTrang.Text = reader["Status"].ToString() == "true" ? "Đã thanh toán" : "Chưa thanh toán";
				txtTongTien.Text = reader["TongTien"].ToString();
				txtGiamGia.Text = reader["GiamGia"].ToString();
				txtPhiGiaoHang.Text = reader["PhiGiaoHang"].ToString();
				txtThue.Text = reader["Thue"].ToString();

				int giamGia = int.Parse(reader["GiamGia"].ToString()) * int.Parse(reader["TongTien"].ToString());
				int ammount = int.Parse(reader["TongTien"].ToString()) - giamGia;
				txtAmountNumber.Text = ammount.ToString();
				txtAmountWorld.Text = Utils.NumberToText(ammount);

			}

			conn.Close();
		}

		/// <summary>
		/// Hàm tính tổng tiền của hoá đơn
		/// </summary>
		private void AmmountUpdate()
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = conn.CreateCommand();

			cmd.CommandText = "EXECUTE Amount_Update @id";
			cmd.Parameters.Add("@id", SqlDbType.NVarChar, 20).Value = currentBillID;
			conn.Open();

			cmd.ExecuteNonQuery();

			conn.Close();

		}

		/// <summary>
		/// Tạo một hoá đơn với status =  false để thêm sản phẩm.
		/// </summary>
		/// 
		private void InsertBill()
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			int idLimit = 7;
			string id = "HD" + ZeroAppend("000000" + AutoIncreatementID(), idLimit);
			currentBillID = id;
			string query = "EXECUTE Bill_Insert @id, @tenKH, @loaiHD, @sdt, @diaChi, @ngayBan, @nv,@nguoiNhan,@dcgh,@thue,@phiGH,@status,@giam,@tongTien";

			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;

			cmd.Parameters.Add("@id", SqlDbType.NVarChar,20).Value = id;
			cmd.Parameters.Add("@tenKH", SqlDbType.NVarChar, 100).Value = txtTen.Text;
			cmd.Parameters.Add("@loaiHD", SqlDbType.Int).Value = 0;
			cmd.Parameters.Add("@sdt", SqlDbType.NVarChar, 15).Value = txtSDT.Text;
			cmd.Parameters.Add("@diaChi", SqlDbType.NVarChar, 300).Value = txtDiaChi.Text;
			cmd.Parameters.Add("@ngayBan", SqlDbType.Date).Value = DateTime.Now;
			cmd.Parameters.Add("@nv", SqlDbType.NVarChar, 100).Value = "Phu";
			cmd.Parameters.Add("@nguoiNhan", SqlDbType.NVarChar, 100).Value = txtNguoiNhan.Text;
			cmd.Parameters.Add("@dcgh", SqlDbType.NVarChar, 300).Value = txtDiaChiGH.Text;
			cmd.Parameters.Add("@thue", SqlDbType.NVarChar, 100).Value = "";
			cmd.Parameters.Add("@phiGH", SqlDbType.Int).Value = 0;
			cmd.Parameters.Add("@status", SqlDbType.Bit).Value = false;
			cmd.Parameters.Add("@giam", SqlDbType.Float).Value = 0;
			cmd.Parameters.Add("@tongTien", SqlDbType.Float).Value = 0;

			conn.Open();

			cmd.ExecuteNonQuery();

			conn.Close();

		}

		/// <summary>
		/// Hàm tự tăng ID đơn hàng
		/// </summary>
		private string AutoIncreatementID()
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			conn.Open();
			string query = "SELECT ISNULL(MAX(stt),0) + 1 from HoaDon";
			string id = null;
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
				id = reader[0].ToString();         
			}
			conn.Close();
			return id;
        }

		/// <summary>
		/// Hàm tự tăng ID chi tiết hoá đơn
		/// </summary>
		/// <returns></returns>
		private string AutoIncreatementIDBillDetail()
		{
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			conn.Open();
			string query = "SELECT ISNULL(MAX(stt),0) + 1 from ChiTietHoaDon";
			string id = null;
			SqlCommand cmd = new SqlCommand(query, conn);
			SqlDataReader reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				id = reader[0].ToString();
			}
			conn.Close();
			return id;
		}

		/// <summary>
		/// Hàm thêm "0" vào sau tiền tố mã hoá đơn
		/// </summary>
		private string ZeroAppend(string data, int idLimit)
        {
			return data.Substring(data.Length - idLimit);
        }

		/// <summary>
		/// Hàm thanh toán hoá đơn
		/// </summary>
		private void UpdateStatusHoaDon()
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = conn.CreateCommand();

			cmd.CommandText = $"Update HoaDon set Status = 'true' where idHoaDon = '{currentBillID}'";
			conn.Open();

			int numEff = cmd.ExecuteNonQuery();
            if (numEff > 0)
            {
				MessageBox.Show("Thanh toán thành công!!");
				dgvChitietdonHang.Columns.Clear();
				gbBillDetail.Visible = false;
				currentBillID = null;
				ResetForm();
			}
			conn.Close();
		}

		/// <summary>
		/// Hàm xoá trắng thông tin của hoá đơn
		/// </summary>
		private void ResetForm()
        {
			dpkNgayBan.Value = DateTime.Now;
			cbbTinhTrang.Text = "";
			txtTongTien.Text = "";
			txtGiamGia.Text = "";
			txtPhiGiaoHang.Text = "";
			txtThue.Text = "";
			txtAmountNumber.Text = "";
			txtAmountWorld.Text = "";
			txtTen.Text = "";
			txtSDT.Text = "";
			txtDiaChi.Text = "";
		}
        #endregion

        private void dgvSanPham_DoubleClick(object sender, EventArgs e)
		{
			var RowIndex = dgvSanPham.SelectedRows[0].Cells[0].Value.ToString();
			float donGia = float.Parse(dgvSanPham.SelectedRows[0].Cells[2].Value.ToString());
			AddProduct(RowIndex,donGia);
		}

        private void btnAddBill_Click(object sender, EventArgs e)
        {
			gbBillDetail.Visible = true;
			InsertBill();
        }

        private void dgvChitietdonHang_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
			string connectionString = @"server= DESKTOP-ONTGILH\SQLEXPRESS; database = QLNhaSachN3; InteGrated Security = true; ";
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = conn.CreateCommand();

			var id = dgvChitietdonHang.SelectedRows[0].Cells[1].Value;
			var soLuong = dgvChitietdonHang.SelectedRows[0].Cells[2].Value;
			string query = $"UPDATE ChiTietHoaDon set SoLuong = {soLuong} where idChiTietHoaDon = '{id}'";
			conn.Open();

			cmd.CommandText = query;
			cmd.ExecuteNonQuery();
			AmmountUpdate();
			TongKetHoaDon();

			conn.Close();
		}

        private void btnLuu_Click(object sender, EventArgs e)
        {
			UpdateStatusHoaDon();
        }
    }
}
