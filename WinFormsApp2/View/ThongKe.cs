private void btnThongKeDoanhThu_Click(object sender, EventArgs e)
{
    DateTime tuNgay = dtpTuNgay.Value;
    DateTime denNgay = dtpDenNgay.Value;

    List<ThongKeDoanhThu> thongKeDoanhThu = NhanVienDAO.ThongKeDoanhThuTheoNhanVien(tuNgay, denNgay);

    dgvThongKeDoanhThu.DataSource = thongKeDoanhThu;
}

private void btnThongKeMonAn_Click(object sender, EventArgs e)
{
    DateTime tuNgay = dtpTuNgay.Value;
    DateTime denNgay = dtpDenNgay.Value;

    List<ThongKeMonAn> thongKeMonAn = NhanVienDAO.ThongKeMonAnBanChay(tuNgay, denNgay);

    dgvThongKeMonAn.DataSource = thongKeMonAn;
}
