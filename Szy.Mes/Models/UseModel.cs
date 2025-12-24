namespace Szy.Mes.Models
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class SysUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } // 用户名
        public string Password { get; set; } // 密码
        public string RealName { get; set; } // 真实姓名
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
    }
}