using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Szy.Mes.Data;
using Szy.Mes.Models;

var builder = WebApplication.CreateBuilder(args);

// 添加DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 身份验证配置
builder.Services.AddDefaultIdentity<IdentityUser>(options => { options.SignIn.RequireConfirmedAccount = true;
    // .NET 10 禁用 Passkey（可选，避免实体冲突）
    options.SignIn.RequireConfirmedAccount = true;
    // .NET 10 密码策略（无需新增 RequiredUniqueChars）
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>();

// 添加MVC服务
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // 身份验证需要Razor Pages

var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // 应用迁移并创建数据库
}

// 中间件配置
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // 身份验证
app.UseAuthorization();  // 授权

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // 身份验证页面路由

app.Run();