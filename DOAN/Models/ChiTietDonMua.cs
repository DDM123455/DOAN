//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DOAN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChiTietDonMua
    {
        public int MaDM { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<int> Thue { get; set; }
        public Nullable<int> TongCong { get; set; }
        public Nullable<int> MaKH { get; set; }
    
        public virtual DonMua DonMua { get; set; }
        public virtual KhachHang KhachHang { get; set; }
    }
}
