using System.ComponentModel;
namespace POS_System.ViewModels
{
    public class Permission
    {
        public int PermissionID { get; set; }
        [DisplayName("Hak Akses/Izin")]
        public string? PermissionName { get; set; }
    }
}
