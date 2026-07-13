namespace POS_System.ViewModels
{
    public class RoleUser
    {
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        //Properti Navigasi/Relasi one to many
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
