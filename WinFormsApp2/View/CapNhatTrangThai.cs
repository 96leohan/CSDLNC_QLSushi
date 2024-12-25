private void btnCapNhatTrangThai_Click(object sender, EventArgs e)
{
    string maMon = cboMaMon.SelectedItem.ToString();
    string trangThaiMoi = cboTrangThaiMoi.SelectedItem.ToString();

    if (NhanVienDAO.CapNhatTrangThaiMonAn(maMon, trangThaiMoi))
    {
        MessageBox.Show("Cập nhật trạng thái thành công!", "Thông báo");
    }
    else
    {
        MessageBox.Show("Cập nhật trạng thái thất bại!", "Lỗi");
    }
}
