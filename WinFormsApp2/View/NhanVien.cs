private void btnThemNhanVien_Click(object sender, EventArgs e)
{
    NhanVien nhanVien = new NhanVien
    {
        MaNhanVien = txtMaNhanVien.Text,
        HoTen = txtHoTen.Text,
        TaiKhoan = txtTaiKhoan.Text,
        MatKhau = txtMatKhau.Text,
        MaBoPhan = cboMaBoPhan.SelectedItem.ToString(),
        MaChiNhanh = cboMaChiNhanh.SelectedItem.ToString(),
        NgayBatDau = dtpNgayBatDau.Value,
        NgayKetThuc = chkNgayKetThuc.Checked ? (DateTime?)dtpNgayKetThuc.Value : null
    };

    if (NhanVienDAO.InsertNhanVien(nhanVien))
    {
        MessageBox.Show("Thêm nhân viên thành công!");
        LoadDanhSachNhanVien();
    }
    else
    {
        MessageBox.Show("Thêm nhân viên thất bại!");
    }
}

private void btnCapNhatNhanVien_Click(object sender, EventArgs e)
{
    NhanVien nhanVien = new NhanVien
    {
        MaNhanVien = txtMaNhanVien.Text,
        HoTen = txtHoTen.Text,
        TaiKhoan = txtTaiKhoan.Text,
        MatKhau = txtMatKhau.Text,
        MaBoPhan = cboMaBoPhan.SelectedItem.ToString(),
        MaChiNhanh = cboMaChiNhanh.SelectedItem.ToString(),
        NgayBatDau = dtpNgayBatDau.Value,
        NgayKetThuc = chkNgayKetThuc.Checked ? (DateTime?)dtpNgayKetThuc.Value : null
    };

    if (NhanVienDAO.UpdateNhanVien(nhanVien))
    {
        MessageBox.Show("Cập nhật nhân viên thành công!");
        LoadDanhSachNhanVien();
    }
    else
    {
        MessageBox.Show("Cập nhật nhân viên thất bại!");
    }
}

private void btnXoaNhanVien_Click(object sender, EventArgs e)
{
    string maNhanVien = txtMaNhanVien.Text;

    if (NhanVienDAO.DeleteNhanVien(maNhanVien))
    {
        MessageBox.Show("Xóa nhân viên thành công!");
        LoadDanhSachNhanVien();
    }
    else
    {
        MessageBox.Show("Xóa nhân viên thất bại!");
    }
}

private void LoadDanhSachNhanVien()
{
    dgvNhanVien.DataSource = NhanVienDAO.GetAllNhanVien();
}
