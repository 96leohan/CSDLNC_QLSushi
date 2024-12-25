public class NhanVienDTO
{
    public string MaNV { get; set; }
    public string HoTen { get; set; }
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; }
    public decimal Luong { get; set; }
    public string BoPhan { get; set; }
    public string ChiNhanh { get; set; }

    public NhanVienDTO(string maNV, string hoTen, DateTime ngaySinh, string gioiTinh, decimal luong, string boPhan, string chiNhanh)
    {
        MaNV = maNV;
        HoTen = hoTen;
        NgaySinh = ngaySinh;
        GioiTinh = gioiTinh;
        Luong = luong;
        BoPhan = boPhan;
        ChiNhanh = chiNhanh;
    }
}
