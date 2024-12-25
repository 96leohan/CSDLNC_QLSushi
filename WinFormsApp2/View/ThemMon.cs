private void btnThemMon_Click(object sender, EventArgs e)
{
    MonAn monAn = new MonAn
    {
        MaMon = txtMaMon.Text,
        TenMon = txtTenMon.Text,
        DonGia = decimal.Parse(txtDonGia.Text),
        TrangThai = cboTrangThai.SelectedItem.ToString()
    };

    if (NhanVienDAO.ThemMonAn(monAn))
    {
        MessageBox.Show("Thêm món thành công!", "Thông báo");
    }
    else
    {
        MessageBox.Show("Thêm món thất bại!", "Lỗi");
    }
}
