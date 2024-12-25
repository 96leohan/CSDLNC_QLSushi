public static List<ThongKeDoanhThu> ThongKeDoanhThuTheoNhanVien(DateTime tuNgay, DateTime denNgay)
{
    string query = @"
        SELECT nv.MaNhanVien, nv.HoTen, SUM(hd.TongTien) AS TongDoanhThu
        FROM HoaDon hd
        JOIN NhanVien nv ON hd.MaNhanVien = nv.MaNhanVien
        WHERE hd.NgayLap BETWEEN @TuNgay AND @DenNgay
        GROUP BY nv.MaNhanVien, nv.HoTen";

    Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@TuNgay", tuNgay },
        { "@DenNgay", denNgay }
    };

    DataTable dataTable = DataProvider.ExecuteSelectQuery(query, parameters);

    List<ThongKeDoanhThu> result = new List<ThongKeDoanhThu>();

    if (dataTable != null)
    {
        foreach (DataRow row in dataTable.Rows)
        {
            result.Add(new ThongKeDoanhThu
            {
                MaNhanVien = row["MaNhanVien"].ToString(),
                HoTen = row["HoTen"].ToString(),
                TongDoanhThu = Convert.ToDecimal(row["TongDoanhThu"])
            });
        }
    }

    return result;
}
