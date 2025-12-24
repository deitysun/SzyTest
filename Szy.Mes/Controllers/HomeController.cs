using Szy.Mes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Szy.Mes.Data;

namespace Szy.Mes.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 首页（加载菜单）
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // 获取所有菜单，按级别和父级ID排序
            var menus = await _dbContext.Menus
                .OrderBy(m => m.Level)
                .ThenBy(m => m.ParentId)
                .ThenBy(m => m.Id)
                .ToListAsync();

            // 按级别分组
            ViewBag.Level1Menus = menus.Where(m => m.Level == 1).ToList();
            ViewBag.AllMenus = menus;

            return View();
        }
    }
}