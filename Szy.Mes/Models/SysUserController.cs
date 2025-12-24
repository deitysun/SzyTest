using Szy.Mes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Szy.Mes.Data;

namespace MenuUserSystem.Controllers
{
    public class SysUserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public SysUserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 用户新增页面
        /// </summary>
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 保存新增用户
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add(SysUser user)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.SysUsers.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return Json(new { success = true, msg = "新增成功" });
            }
            return Json(new { success = false, msg = "参数错误" });
        }

        /// <summary>
        /// 用户列表页面
        /// </summary>
        public async Task<IActionResult> List()
        {
            var users = await _dbContext.SysUsers.ToListAsync();
            return View(users);
        }

        /// <summary>
        /// 获取用户列表数据（供前端异步加载）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            var users = await _dbContext.SysUsers
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.RealName,
                    u.CreateTime
                })
                .ToListAsync();
            return Json(new { data = users });
        }
    }
}