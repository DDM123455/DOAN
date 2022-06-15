using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN.Models
{
    public class DanhSachLoaiKH
    {
        public int MaKH { get; set; }
        public String HoTenKH { get; set; }
        public int SDT { get; set; }
        public String GioiTinh { get; set; }
        public String GiaChi { get; set; }
        public String GhiChu { get; set; }
        public String TenLoai { get; set; }
    }
}