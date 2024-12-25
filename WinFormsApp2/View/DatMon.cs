private void btnDatMon_Click(object sender, EventArgs e)
{
    DatMon datMon = new DatMon
    {
        MaHoaDon = txtMaHoaDon.Text,
        MaMon = txtMaMonDat.Text,
        SoLuong = int.Parse(txtSoLuong.Text),
        DonGia = decimal.Parse(txtDonGiaDat.Text)
    };

    if (NhanVienDAO.DatMon(datMon))
    {
        MessageBox.Show("Đặt món thành công!", "Thông báo");
    }
    else
    {
        MessageBox.Show("Đặt món thất bại!", "Lỗi");
    }
}
