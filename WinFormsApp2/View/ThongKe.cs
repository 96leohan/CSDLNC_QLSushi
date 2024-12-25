private void btnThongKeDoanhThu_Click(object sender, EventArgs e)
{
    DateTime tuNgay = dtpTuNgay.Value;
    DateTime denNgay = dtpDenNgay.Value;

    List<ThongKeDoanhThu> thongKeDoanhThu = NhanVienDAO.ThongKeDoanhThuTheoNhanVien(tuNgay, denNgay);

    dgvThongKeDoanhThu.DataSource = thongKeDoanhThu;
}
private void btnThongKeKhachHang_Click(object sender, EventArgs e)
{
    DateTime fromDate = DateTime.Parse(txtFromDate.Text);
    DateTime toDate = DateTime.Parse(txtToDate.Text);

    List<KhachHang> khachHangs = NhanVienDAO.GetThongKeKhachHang(fromDate, toDate);

    dgvKhachHang.DataSource = khachHangs;
}

private void btnThongKeMonAn_Click(object sender, EventArgs e)
{
    DateTime tuNgay = dtpTuNgay.Value;
    DateTime denNgay = dtpDenNgay.Value;

    List<ThongKeMonAn> thongKeMonAn = NhanVienDAO.ThongKeMonAnBanChay(tuNgay, denNgay);

    dgvThongKeMonAn.DataSource = thongKeMonAn;
}
