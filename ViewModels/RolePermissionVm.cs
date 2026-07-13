namespace POS_System.ViewModels
{
    public class RolePermission
    {
        public int RolePermissionID { get; set; }
        public int RoleID { get; set; }
        public RoleUser? Role { get; set; }
        public int PermissionID { get; set; }
        public Permission? Permission { get; set; }
    }
}
