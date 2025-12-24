
namespace Szy.Mes.Models
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuModel
    {
        public int Id { get; set; }
        public  string MenuName { get; set; } // 菜单名称
        public int ParentId { get; set; }    // 父级ID（0为一级菜单）
        public int Level { get; set; }       // 菜单级别（1-4）
        public  string Url { get; set; }      // 菜单跳转地址
        //public  string Icon { get; set; }     // 菜单图标（可选）
    }

    /// <summary>
    /// 多页签模型
    /// </summary>
    public class TabItem
    {
        public required string Id { get; set; }       // 页签ID
        public  string Title { get; set; }    // 页签标题
        public  string Url { get; set; }      // 页签内容地址
        public bool Active { get; set; }     // 是否激活
    }
}