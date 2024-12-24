using QuanLySuShi.Controller.DAO;
using QuanLySuShi.Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QuanLySuShi
{
    public partial class MainfmNhanvien : Form
    {
        private Button _selected_table = null;
        private string current_maphieu_tai_ban = null;
        private string _mauudai_taiBan = null;
        DataGridViewRow selectDonHang = null;

        public MainfmNhanvien()
        {
            InitializeComponent();
            InitializeData();
        }

        #region Initialization

        private void InitializeData()
        {
            LoadTable();
            LoadThucDon();
            PhanQuyen();
            LoadDonHang();
        }

        private void LoadTable()
        {
            flpTable.Controls.Clear();
            foreach (var table in Table.Tables)
            {
                Button btn = new Button
                {
                    Width = Table.btnWidth,
                    Height = Table.btnHeight,
                    Text = table.TableName + Environment.NewLine + table.Status,
                    BackColor = table.Status == "Trống" ? Color.Aqua : Color.Red,
                    Tag = table
                };
                btn.Click += Btn_TableClick;
                flpTable.Controls.Add(btn);
            }
        }

        private void LoadThucDon()
        {
            ThucDon.LoadThucdon(cbbthucdon, (Dangnhap.user as NhanVien).MaChiNhanh);
        }

        private void LoadDonHang()
        {
            dtgvDonHang.DataSource = PhieudatmonDAO.GetPhieuDatMonChuaLap((Dangnhap.user as NhanVien).MaChiNhanh);
        }

        private void PhanQuyen()
        {
            btn_admin.Enabled = (Dangnhap.user as NhanVien).QuanlyChiNhanh;
        }

        #endregion

        #region Event Handlers

        private void Btn_TableClick(object sender, EventArgs e)
        {
            _selected_table = sender as Button;
            Table table = _selected_table.Tag as Table;
            current_maphieu_tai_ban = PhieudatmonDAO.GetPhieuDatMonByTableId(table.TableID);
            ShowPhieuDat(current_maphieu_tai_ban);
        }

        private void btnUuDai_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(current_maphieu_tai_ban))
            {
                MessageBox.Show("Vui lòng chọn bàn", "Thông Báo");
                return;
            }
            ApplyDiscount();
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            CreateCustomerCard();
        }

        private void btnthanhtoan_Click(object sender, EventArgs e)
        {
            ProcessPayment();
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            AddOrderDetail();
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            DeleteOrderDetail();
        }

        private void cbbthucdon_SelectedIndexChanged(object sender, EventArgs e)
        {
            Muc.LoadMucByThucdon(cbbmuc, cbbthucdon);
        }

        private void cbbMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonAn.LoadMonAnByMuc(cbbmuc, cbbmonan);
        }

        private void btnDuyet_donhang_Click(object sender, EventArgs e)
        {
            ApproveOrder();
        }

        private void MainfmNhanvien_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.dangnhapForm.Show();
        }

        #endregion

        #region Methods

        private void ShowPhieuDat(string maphieu)
        {
            listchitiet.Items.Clear();
            listchitiet.Columns.Clear();
            listchitiet.Columns.Add("Mã Món Ăn", 150);
            listchitiet.Columns.Add("Tên Món", 150);
            listchitiet.Columns.Add("Giá", 100);
            listchitiet.Columns.Add("Số Lượng", 100);

            if (string.IsNullOrEmpty(maphieu)) return;

            var details = ChitietphieuDAO.GetChitietPhieuByMaPhieu(maphieu);
            decimal totalPrice = 0;

            foreach (var detail in details)
            {
                MonAn monAn = MonAnDAO.GetMonAn(detail.MaMonAn).FirstOrDefault();
                if (monAn != null)
                {
                    ListViewItem item = new ListViewItem(detail.MaMonAn)
                    {
                        SubItems =
                        {
                            monAn.TenMonAn,
                            monAn.GiaTien.ToString("c", CultureInfo.CurrentCulture),
                            detail.SoLuong.ToString()
                        }
                    };
                    listchitiet.Items.Add(item);
                    totalPrice += monAn.GiaTien * detail.SoLuong;
                }
            }
            tbtongtien.Text = totalPrice.ToString("c", CultureInfo.CurrentCulture);
        }

        private void ApplyDiscount()
        {
            string makh = KhachHangDAO.MaKhachHangByMaPhieu(current_maphieu_tai_ban);
            TheKhachHang tkh = TheKhachHangDAO.GetTheKhachHang(maKhachHang: makh);
            if (tkh == null)
            {
                MessageBox.Show("Khách hàng chưa đăng ký thẻ.", "Thông Báo");
                return;
            }

            List<UuDai> dsUuDai = UuDaiDAO.GetUuDais(loaiTheApDung: tkh.LoaiThe);
            fmUuDais frm = new fmUuDais(dsUuDai);
            frm.ShowDialog();

            if (frm.uuDai != null)
            {
                _mauudai_taiBan = frm.uuDai.Cells["MaUuDai"].Value?.ToString();
                ShowPhieuDat(current_maphieu_tai_ban);
            }
        }

        private void CreateCustomerCard()
        {
            if (string.IsNullOrWhiteSpace(tbHoVaTen_taothe.Text) ||
                string.IsNullOrWhiteSpace(tbSDT_taothe.Text) ||
                string.IsNullOrWhiteSpace(tbEmail_taothe.Text) ||
                string.IsNullOrWhiteSpace(tbCCCD_taothe.Text) ||
                string.IsNullOrWhiteSpace(cbbGioitinh_taothe.Text) ||
                string.IsNullOrWhiteSpace(tbTaiKhoan_taothe.Text) ||
                string.IsNullOrWhiteSpace(tbMatKhau_taothe.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông Báo");
                return;
            }

            try
            {
                string maKhachHang = KhachHangDAO.GetNextMakhachhang();
                KhachHang newCustomer = new KhachHang
                {
                    MaDinhDanh = maKhachHang,
                    HoTen = tbHoVaTen_taothe.Text,
                    SoDienThoai = tbSDT_taothe.Text,
                    Email = tbEmail_taothe.Text,
                    CCCD = tbCCCD_taothe.Text,
                    GioiTinh = cbbGioitinh_taothe.Text,
                    TaiKhoan = tbTaiKhoan_taothe.Text,
                    MatKhau = tbMatKhau_taothe.Text
                };

                bool result = KhachHangDAO.CreatKhachHang(newCustomer) &&
                              TheKhachHangDAO.CreateTheKhachHang(TheKhachHangDAO.GetNextTheKhachHang(), (Dangnhap.user as NhanVien).MaDinhDanh, maKhachHang);

                if (result)
                {
                    MessageBox.Show("Tạo tài khoản khách hàng thành công!", "Thông Báo");
                    ResetCustomerForm();
                }
                else
                {
                    MessageBox.Show("Tạo tài khoản thất bại. Vui lòng thử lại.", "Thông Báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessPayment()
        {
            if (current_maphieu_tai_ban == null)
            {
                MessageBox.Show("Vui lòng chọn bàn để thanh toán.", "Thông Báo");
                return;
            }

            Table selectedTable = _selected_table.Tag as Table;
            string mahoadon = HoaDonDAO.GetNextHoaDon();

            bool result = HoaDonDAO.AddHoaDon(mahoadon, (Dangnhap.user as NhanVien).MaChiNhanh, current_maphieu_tai_ban, _mauudai_taiBan);
            if (result)
            {
                MessageBox.Show($"Tổng hóa đơn của quý khách là {tbtongtien.Text}", "Thông Báo");
                selectedTable.Status = Table.GetTableStatus(selectedTable.TableID);
                LoadTable();
                ResetOrder();
            }
            else
            {
                MessageBox.Show("Thanh toán thất bại.", "Thông Báo");
            }
        }

        private void AddOrderDetail()
        {
            if (_selected_table == null || cbbmonan.SelectedItem == null || string.IsNullOrEmpty(soluong.Text))
            {
                MessageBox.Show("Vui lòng chọn bàn, món ăn và nhập số lượng hợp lệ.", "Thông Báo");
                return;
            }

            MonAn selectedMonAn = cbbmonan.SelectedItem as MonAn;
            int soLuong = int.Parse(soluong.Text);

            if (string.IsNullOrEmpty(current_maphieu_tai_ban))
            {
                CreateNewOrder(selectedMonAn, soLuong);
            }
            else
            {
                ChitietphieuDAO.AddChitietPhieu(current_maphieu_tai_ban, selectedMonAn.MaMonAn, soLuong);
                ShowPhieuDat(current_maphieu_tai_ban);
            }
        }

        private void DeleteOrderDetail()
        {
            if (listchitiet.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn món ăn để xóa.", "Thông Báo");
                return;
            }

            ListViewItem selectedItem = listchitiet.SelectedItems[0];
            string maMonAn = selectedItem.SubItems[0].Text;

            bool isDeleted = ChitietphieuDAO.XoaMonAnTheoPhieu(maMonAn, current_maphieu_tai_ban);
            if (isDeleted)
            {
                MessageBox.Show("Món ăn đã được xóa thành công.", "Thông Báo");
                ShowPhieuDat(current_maphieu_tai_ban);
            }
            else
            {
                MessageBox.Show("Xóa thất bại.", "Thông Báo");
            }
        }

        private void ApproveOrder()
        {
            if (selectDonHang != null)
            {
                string maphieudonhang = selectDonHang.Cells["maphieu"].Value?.ToString();
                bool isSuccess = PhieudatmonDAO.UpdatePhieuDatMon(maphieudonhang, Dangnhap.user.MaDinhDanh);

                if (isSuccess)
                {
                    MessageBox.Show("Đơn hàng đã được duyệt.", "Thông Báo");
                    LoadDonHang();
                }
                else
                {
                    MessageBox.Show("Duyệt đơn hàng thất bại.", "Thông Báo");
                }
                selectDonHang = null;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng để duyệt.", "Thông Báo");
            }
        }

        private void ResetCustomerForm()
        {
            tbHoVaTen_taothe.Clear();
            tbSDT_taothe.Clear();
            tbEmail_taothe.Clear();
            tbCCCD_taothe.Clear();
            cbbGioitinh_taothe.SelectedIndex = -1;
            tbTaiKhoan_taothe.Clear();
            tbMatKhau_taothe.Clear();
        }

        private void ResetOrder()
        {
            current_maphieu_tai_ban = null;
            _selected_table = null;
            _mauudai_taiBan = null;
            listchitiet.Items.Clear();
            tbtongtien.Clear();
            tbgiamGia.Clear();
        }

        private void CreateNewOrder(MonAn selectedMonAn, int soLuong)
        {
            string makhachhang = GetCustomerInfo();
            if (string.IsNullOrEmpty(makhachhang)) return;

            string maPhieuMoi = PhieudatmonDAO.GeNextPhieuDatMon();
            string loaiphieudat = "Trực Tiếp";

            bool isSuccess = PhieudatmonDAO.CreatePhieuDatMon(Dangnhap.user.MaDinhDanh, makhachhang, (Dangnhap.user as NhanVien).MaChiNhanh, maPhieuMoi, loaiphieudat);
            if (isSuccess)
            {
                current_maphieu_tai_ban = maPhieuMoi;
                Table selectedTable = _selected_table.Tag as Table;
                PhieudatmontructiepDAO.CreatePhieuDatMonTrucTiep(maPhieuMoi, selectedTable.TableID);
                ChitietphieuDAO.AddChitietPhieu(maPhieuMoi, selectedMonAn.MaMonAn, soLuong);
                ShowPhieuDat(maPhieuMoi);
                LoadTable();
            }
            else
            {
                MessageBox.Show("Tạo phiếu đặt món thất bại.", "Thông Báo");
            }
        }

        private string GetCustomerInfo()
        {
            DialogResult result = MessageBox.Show("Khách hàng có phải là khách hàng thân thiết không?", "Xác nhận khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string cccd = Microsoft.VisualBasic.Interaction.InputBox("Vui lòng nhập CCCD:", "Nhập mã khách hàng thân thiết", "");
                TheKhachHang tkh = TheKhachHangDAO.GetTheKhachHang(cccd: cccd);
                if (tkh == null)
                {
                    MessageBox.Show("Không tồn tại khách hàng thân thiết!", "Lỗi");
                    return null;
                }
                return tkh.MaKhachHang;
            }
            else
            {
                string makhachhang = KhachHangDAO.GetNextMakhachhang();
                KhachHang kh = new KhachHang { MaDinhDanh = makhachhang };
                KhachHangDAO.CreatKhachHang(kh);
                return makhachhang;
            }
        }

        #endregion
    }
}
