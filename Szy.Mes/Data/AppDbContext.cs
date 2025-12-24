using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Szy.Mes.Models;
namespace Szy.Mes.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 菜单表
        public DbSet<MenuModel> Menus { get; set; }
        // 用户表
        public DbSet<SysUser> SysUsers { get; set; }

        /// <summary>
        /// 初始化菜单数据
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. 优先执行 Identity 基础配置（必须放在最前）
            base.OnModelCreating(modelBuilder);

            // 2. 核心修复：动态移除 IdentityPasskeyData 实体（彻底规避主键报错）
            // 适配 .NET 10：无论该实体是否存在，都不会报错
            var passkeyEntityType = modelBuilder.Model.GetEntityTypes()
                .FirstOrDefault(t => t.ClrType.Name == "IdentityPasskeyData");
            if (passkeyEntityType != null)
            {
                modelBuilder.Model.RemoveEntityType(passkeyEntityType);
            }

            // 3. 显式配置 Identity 核心实体（兜底，确保 EF Core 识别）
            modelBuilder.Entity<IdentityUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.ToTable("AspNetUsers");
                b.Property(u => u.Id).HasMaxLength(450);
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.HasKey(r => r.Id);
                b.ToTable("AspNetRoles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });
                b.ToTable("AspNetUserRoles");
            });
            // 一级菜单
            modelBuilder.Entity<MenuModel>().HasData(
                new MenuModel { Id = 1, MenuName = "供应链管理", ParentId = 0, Level = 1, Url = "" },
                new MenuModel { Id = 2, MenuName = "生产管理", ParentId = 0, Level = 1, Url = "" },
                new MenuModel { Id = 3, MenuName = "基础数据", ParentId = 0, Level = 1, Url = "" },
                new MenuModel { Id = 4, MenuName = "系统管理", ParentId = 0, Level = 1, Url = "" }
            );

            // 二级菜单
            modelBuilder.Entity<MenuModel>().HasData(
                // 供应链管理二级
                new MenuModel { Id = 11, MenuName = "采购管理", ParentId = 1, Level = 2, Url = "" },
                new MenuModel { Id = 12, MenuName = "销售管理", ParentId = 1, Level = 2, Url = "" },
                new MenuModel { Id = 13, MenuName = "库存管理", ParentId = 1, Level = 2, Url = "" },
                new MenuModel { Id = 14, MenuName = "委外管理", ParentId = 1, Level = 2, Url = "" },
                // 生产管理二级
                new MenuModel { Id = 21, MenuName = "工程数据", ParentId = 2, Level = 2, Url = "" },
                new MenuModel { Id = 22, MenuName = "生产订单", ParentId = 2, Level = 2, Url = "" },
                new MenuModel { Id = 23, MenuName = "物料需求", ParentId = 2, Level = 2, Url = "" },
                // 基础数据二级
                new MenuModel { Id = 31, MenuName = "物料数据", ParentId = 3, Level = 2, Url = "" },
                new MenuModel { Id = 32, MenuName = "财务数据", ParentId = 3, Level = 2, Url = "" },
                new MenuModel { Id = 33, MenuName = "采购数据", ParentId = 3, Level = 2, Url = "" },
                new MenuModel { Id = 34, MenuName = "销售数据", ParentId = 3, Level = 2, Url = "" },
                new MenuModel { Id = 35, MenuName = "基本数据", ParentId = 3, Level = 2, Url = "" },
                // 系统管理二级
                new MenuModel { Id = 41, MenuName = "权限管理", ParentId = 4, Level = 2, Url = "" },
                new MenuModel { Id = 42, MenuName = "安全管理", ParentId = 4, Level = 2, Url = "" }
            );

            // 三级菜单（权限管理下）
            modelBuilder.Entity<MenuModel>().HasData(
                new MenuModel { Id = 411, MenuName = "用户", ParentId = 41, Level = 3, Url = "" },
                new MenuModel { Id = 412, MenuName = "角色", ParentId = 41, Level = 3, Url = "" }
            );

            // 四级菜单（末级）
            modelBuilder.Entity<MenuModel>().HasData(
                new MenuModel { Id = 4111, MenuName = "用户新增", ParentId = 411, Level = 4, Url = "/SysUser/Add" },
                new MenuModel { Id = 4112, MenuName = "用户列表", ParentId = 411, Level = 4, Url = "/SysUser/List" }
            );
        }


    }
}