using QuanLySuShi.Controller.DAO;
using QuanLySuShi.Model.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLySuShi
{
    public partial class MainfmNhanvien : Form 
    {
        private Button _selected_table = null; // Lưu button đang chọn
        private string current_maphieu_tai_ban = null; // Lưu button đang chọn
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
            Loadtable();
            ThucDon.LoadThucdon(cbbthucdon, (Dangnhap.user as NhanVien).MaChiNhanh);
            PhanQuyen();
            LoadDonHang();
        }

        private void LoadTable()
        {
            flpTable.Controls.Clear();
            foreach( var table in Table.Tables)
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
            showPhieudat(current_maphieu_tai_ban);
        }
        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dangnhap loginForm = new Dangnhap();
            loginForm.Show();
            this.Close();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            adminfm adminfm = new adminfm();
            adminfm.Show();

        }



        private void btThem_Click(object sender, EventArgs e)
        {
            if (_selected_table == null)
            {
                MessageBox.Show("Vui lòng chọn Bàn.", "Thông báo");
                return;
            }


            if (cbbmonan.SelectedItem == null || string.IsNullOrEmpty(soluong.Text))
            {
                MessageBox.Show("Vui lòng chọn món ăn và nhập số lượng hợp lệ.", "Thông báo");
                return;
            }

            // Lấy thông tin món ăn từ combobox
            MonAn selectedMonAn = (MonAn)cbbmonan.SelectedItem;

            // Lấy số lượng từ textbox
            int soLuong;
            int.TryParse(soluong.Text, out soLuong);
            Table selectTable = (_selected_table.Tag as Table);



            bool isSuccess = false;
            string mamonan = selectedMonAn.MaMonAn;
            if (string.IsNullOrEmpty(current_maphieu_tai_ban))
            {
                string makhachhang = null;

                // Hỏi người dùng xem có phải khách hàng thân thiết không
                DialogResult result = MessageBox.Show("Khách hàng có phải là khách hàng thân thiết không?",
                                                      "Xác nhận khách hàng",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Mở input để nhập thông tin khách hàng thân thiết
                    string CCCD = Microsoft.VisualBasic.Interaction.InputBox(
                                          "Vui lòng nhập CCCD:",
                                          "Nhập mã khách hàng thân thiết",
                                          "");
                    TheKhachHang tkh = TheKhachHangDAO.GetTheKhachHang(cccd: CCCD);
                    if (tkh == null)
                    {
                        MessageBox.Show("Không tồn tại!", "Lỗi");
                        return;
                    }
                    makhachhang = tkh.MaKhachHang;
                    // Thêm logic xử lý khách hàng thân thiết ở đây (ví dụ: kiểm tra mã khách hàng)
                    MessageBox.Show($"Thành Công", "Thông báo");
                }
                else
                {
                    makhachhang = KhachHangDAO.GetNextMakhachhang();
                    KhachHang kh = new KhachHang() { MaDinhDanh = makhachhang };
                    KhachHangDAO.CreatKhachHang(kh);
                }


                string maPhieuMoi = PhieudatmonDAO.GeNextPhieuDatMon(); // Hoặc có thể là chuỗi tự tạo, tùy vào quy tắc trong hệ thống của bạn.
                string loaiphieudat = "Trực Tiếp";


                isSuccess = PhieudatmonDAO.CreatePhieuDatMon(Dangnhap.user.MaDinhDanh, makhachhang, (Dangnhap.user as NhanVien).MaChiNhanh, maPhieuMoi, loaiphieudat);
                if (isSuccess)
                {
                    current_maphieu_tai_ban = maPhieuMoi;
                    if (PhieudatmontructiepDAO.CreatePhieuDatMonTrucTiep(maPhieuMoi, selectTable.TableID) && ChitietphieuDAO.AddChitietPhieu(maPhieuMoi, mamonan, soLuong))
                        MessageBox.Show("Tạo Phiếu và Thêm món ăn vào phiếu thành công!", "Thông báo");
                    selectTable.Status = Table.GetTableStatus(selectTable.TableID);
                    Loadtable();
                    showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia, _mauudai_taiBan);

                }

            }
            else
            {
                // Thêm vào cơ sở dữ liệu
                isSuccess = ChitietphieuDAO.AddChitietPhieu(current_maphieu_tai_ban, mamonan, soLuong);

                // Cập nhật lại danh sách chi tiết phiếu
                showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia, _mauudai_taiBan);

            }
        }

        void Loadtable()
        {
            flpTable.Controls.Clear();
            foreach (var table in Table.Tables)
            {
                Button btn = new Button()
                {
                    Width = Table.btnWidth,
                    Height = Table.btnHeight
                };
                btn.Click += Btn_Click;
                btn.Tag = table;
                flpTable.Controls.Add(btn);
                btn.Text = table.TableName + Environment.NewLine + table.Status;
                switch (table.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default: btn.BackColor = Color.Red; break;
                }

            }

        }

        void showPhieudat(string maphieu, ListView lsView, TextBox? tongTien = null, TextBox? giamGia = null, string? MaUuDai = null)
        {
            lsView.View = View.Details;
            lsView.Items.Clear();
            lsView.Columns.Clear();
            lsView.Columns.Add("Mã Món Ăn", 150); // Cột 1: Mã món ăn.
            lsView.Columns.Add("Tên Món", 100); // Cột 2: Giá.
            lsView.Columns.Add("Giá", 100); // Cột 2: Giá.
            lsView.Columns.Add("Số lượng", 100); // Cột 2: Giá.


            decimal totalprice = 0;
            decimal tienGiam = 0;
            decimal chietkhau = 0;
            if (MaUuDai != null)
            {
                UuDai uuDai = UuDaiDAO.GetUuDais(maUuDai: MaUuDai)[0];
                tienGiam += (Decimal)uuDai.GiamGia;
                chietkhau += (Decimal)uuDai.UuDaiChietKhau / 100;

            }
            CultureInfo culture = new CultureInfo("vi-VN");


            if (!string.IsNullOrEmpty(maphieu))
            {
                List<Chitietphieudat> list = ChitietphieuDAO.GetChitietPhieuByMaPhieu(maphieu);
                foreach (Chitietphieudat item in list)
                {
                    ListViewItem lsitem = new ListViewItem(item.MaMonAn.ToString());
                    MonAn monan = MonAnDAO.GetMonAn(maMonAn: item.MaMonAn)[0];
                    lsitem.SubItems.Add(monan.TenMonAn.ToString());
                    lsitem.SubItems.Add(monan.GiaTien.ToString("c", culture));
                    lsitem.SubItems.Add(item.SoLuong.ToString());
                    lsitem.SubItems.Add((monan.GiaTien * item.SoLuong).ToString("c", culture));
                    lsView.Items.Add(lsitem);
                    totalprice += monan.GiaTien * item.SoLuong;
                }
            }
            tienGiam = tienGiam + totalprice * chietkhau;
            totalprice = totalprice - tienGiam;
            if (totalprice < 0)
            {
                totalprice = 0;
            }
            if (tongTien != null)
                tongTien.Text = totalprice.ToString("c", culture);
            if (giamGia != null)
                giamGia.Text = tienGiam.ToString("c", culture);

        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            _selected_table = (sender as Button);
            string id_table = ((sender as Button).Tag as Table).TableID;
            current_maphieu_tai_ban = PhieudatmonDAO.GetPhieuDatMonByTableId(id_table);
            _mauudai_taiBan = null;
            showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia);
        }

        private void cbbthucdon_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi người dùng thay đổi thực đơn, tải lại các mục theo thực đơn đã chọn
            Muc.LoadMucByThucdon(cbbmuc, cbbthucdon);
        }

        private void cbbMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Khi người dùng thay đổi mục, tải lại các món ăn theo mục đã chọn
            MonAn.LoadMonAnByMuc(cbbmuc, cbbmonan);
        }


        private void btXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn không
            if (listchitiet.SelectedItems.Count > 0)
            {
                // Lấy mục đã chọn
                ListViewItem selectedItem = listchitiet.SelectedItems[0];

                // Lấy các thông tin cần thiết từ mục đã chọn
                string maMonAn = selectedItem.SubItems[0].Text;

                // Thực hiện hành động xóa (ví dụ: xóa khỏi cơ sở dữ liệu hoặc danh sách)
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa món ăn {maMonAn}?", "Xóa món ăn", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    bool isDeleted = false;

                    // Thực hiện xóa trong cơ sở dữ liệu hoặc danh sách (nếu có)
                    isDeleted = ChitietphieuDAO.XoaMonAnTheoPhieu(maMonAn, current_maphieu_tai_ban); // Đây là ví dụ, thay đổi tùy theo DAO của bạn
                    if (isDeleted)
                    {
                        MessageBox.Show("Món ăn đã được xóa thành công.");
                        showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia, _mauudai_taiBan);

                    }

                    else
                        MessageBox.Show("Xoá thất bại.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món ăn để xóa.");
            }
        }
        private void PhanQuyen()
        {
            NhanVien nhanvien = Dangnhap.user as NhanVien;

            if (nhanvien.QuanlyChiNhanh)
            {
                btn_admin.Enabled = true;
            }
            else { btn_admin.Enabled = false; }
        }

        private void btnthanhtoan_Click(object sender, EventArgs e)
        {
            Table selectTable = (_selected_table.Tag as Table);


            if (current_maphieu_tai_ban != null)
            {

                string mahoadon = HoaDonDAO.GetNextHoaDon();
                HoaDonDAO.AddHoaDon(mahoadon, (Dangnhap.user as NhanVien).MaChiNhanh, current_maphieu_tai_ban, UuDaiDAO.GetUuDais(_mauudai_taiBan)[0]);


                selectTable.Status = Table.GetTableStatus(selectTable.TableID);
                Loadtable();

                MessageBox.Show($"Tổng hoá đơn của quý khách là {tbtongtien.Text}", "thông báo");
                TheKhachHangDAO.UpdateCardStatus(KhachHangDAO.MaKhachHangByMaPhieu(current_maphieu_tai_ban));
                current_maphieu_tai_ban = null;
                _selected_table = null;
                _mauudai_taiBan = null;
                listchitiet.Items.Clear();
                showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia, _mauudai_taiBan);


            }
            else
            {
                MessageBox.Show("vui lòng chọn bàn khác để thanh toán", "thông báo");

            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu trong các textbox
            if (string.IsNullOrWhiteSpace(tbHoVaTen_taothe.Text))
            {
                MessageBox.Show("Họ và tên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbHoVaTen_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbSDT_taothe.Text))
            {
                MessageBox.Show("Số điện thoại không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbSDT_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbEmail_taothe.Text))
            {
                MessageBox.Show("Email không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbEmail_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbCCCD_taothe.Text))
            {
                MessageBox.Show("CCCD không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbCCCD_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cbbGioitinh_taothe.Text))
            {
                MessageBox.Show("Giới tính không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbbGioitinh_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbTaiKhoan_taothe.Text))
            {
                MessageBox.Show("Tài khoản không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbTaiKhoan_taothe.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbMatKhau_taothe.Text))
            {
                MessageBox.Show("Mật khẩu không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbMatKhau_taothe.Focus();
                return;
            }

            try
            {
                // Lấy mã khách hàng mới (tự động tăng)
                string maKhachHang = KhachHangDAO.GetNextMakhachhang();

                // Tạo đối tượng KhachHang
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

                // Gọi DAO để thêm tài khoản
                bool result = KhachHangDAO.CreatKhachHang(newCustomer) && TheKhachHangDAO.CreateTheKhachHang(TheKhachHangDAO.GetNextTheKhachHang(), (Dangnhap.user as NhanVien).MaDinhDanh, maKhachHang);

                if (result)
                {
                    MessageBox.Show("Tạo tài khoản khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Reset form
                    tbHoVaTen_taothe.Clear();
                    tbSDT_taothe.Clear();
                    tbEmail_taothe.Clear();
                    tbCCCD_taothe.Clear();
                    cbbGioitinh_taothe.SelectedIndex = -1;
                    tbTaiKhoan_taothe.Clear();
                    tbMatKhau_taothe.Clear();
                }
                else
                {
                    MessageBox.Show("Tạo tài khoản thất bại. Vui lòng thử lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUuDai_Click(object sender, EventArgs e)
        {
            if (current_maphieu_tai_ban == null)
            {
                MessageBox.Show("Vui lòng chọn bàn", "Thông Báo");
                return;
            }
            string makh = KhachHangDAO.MaKhachHangByMaPhieu(current_maphieu_tai_ban);
            TheKhachHang tkh = TheKhachHangDAO.GetTheKhachHang(maKhachHang: makh);
            if (tkh == null)
            {
                MessageBox.Show("Vui lòng đăng ký để sử dụng dịch vụ ưu đãi", "Thông Báo");
                return;
            }
            List<UuDai> lsUuDai = UuDaiDAO.GetUuDais(loaiTheApDung: tkh.LoaiThe);

            // Mở form phụ để hiển thị danh sách ưu đãi
            fmUuDais frm = new fmUuDais(lsUuDai);
            frm.ShowDialog();
            if (frm.uuDai != null)
            {
                _mauudai_taiBan = frm.uuDai.Cells["MaUuDai"].Value?.ToString();

                showPhieudat(current_maphieu_tai_ban, listchitiet, tbtongtien, tbgiamGia, _mauudai_taiBan);

            }

        }

        private void dtgvDonHang_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                UuDai uuDai = null;
                selectDonHang = dtgvDonHang.Rows[e.RowIndex];
                string maphieudonhang = selectDonHang.Cells["maphieu"].Value?.ToString();
                HoaDon hd = HoaDonDAO.GetHoaDon(MaPhieu: maphieudonhang);
                if (hd != null)
                    uuDai = UuDaiDAO.GetUuDais(maUuDai: hd.MaUuDai)[0];
                showPhieudat(maphieudonhang, listchitiet_donhang, tbTongTien_donhang, tbGiamGia_donhang, uuDai?.MaUuDai);

            }
        }

        private void btnDuyet_donhang_Click(object sender, EventArgs e)
        {
            if (selectDonHang != null)
            {
                string maphieudonhang = selectDonHang.Cells["maphieu"].Value?.ToString();
                bool isSuccess = PhieudatmonDAO.UpdatePhieuDatMon(maphieudonhang, Dangnhap.user.MaDinhDanh);
                if (isSuccess)
                {
                    MessageBox.Show("Đơn đã được xác nhân", "Thông báo");

                }
                else
                {
                    MessageBox.Show("xác nhận thất bại", "Thông báo");

                }
                selectDonHang = null;
            }
        }

        private void MainfmNhanvien_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.dangnhapForm.Show();
        }
    }
}
