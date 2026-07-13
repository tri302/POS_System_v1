using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using POS_System.Models;
namespace POS_System.ViewModels
{
    public class PeranUserVm
    {
        public int PeranID { get; set; }
        [DisplayName("Role Name")]
        public string? RoleName { get; set; }
        [DisplayName("Nama Karyawan")]
        public int IdKaryawan { get; set; }
        [DisplayName("Hak Akses")]
        public List<string> Izin { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        [ValidateNever]
        public int RoleID { get; set; } // dipakai untuk binding form
        [ValidateNever]
        public IEnumerable<RoleUser>? RoleList { get; set; } // dropdown pilihan role
        public Karyawan? Karyawan { get; set; }
    }
}
