# AccessControl.Extension.AspNetCore

​	基于 ASP.NET Core 框架，使用特性标签 `AccessControlAttribute` 等控制 Action 方法的权限，使用 TagHelper `AccessControlTagHelper` 来控制页面上元素的显示权限，同时支持通过中间件 `AccessControlMiddleware`实现对静态资源的访问。

## 快速使用

### 1. 安装权限控制显示组件 

* [AccessControl.Extension.AspNetCore](https://www.nuget.org/packages/AccessControl.Extension.AspNetCore)

``` bash
dotnet add package AccessControl.Extension.AspNetCore
```

### 2. 实现自定义权限控制策略

- 实现页面元素显示策略接口 `IControlAccessStrategy`
- 实现 `Action` 访问显示策略接口 `IResourceAccessStrategy`

示例代码：

1. [ResourceAccessStrategy.cs](https://github.com/Run2948/AccessControl/blob/master/samples/AccessControlDemo/Services/ActionAccessStrategy.cs)

1. [ControlAccessStrategy.cs](https://github.com/Run2948/AccessControl.Extension.AspNetCore/blob/master/samples/AccessControlDemo/Services/ControlAccessStrategy.cs)

### 3. 注册自定义权限控制策略

​	在 `Startup` 文件中注册显示策略，参考 [Startup.cs](https://github.com/Run2948/AccessControl.Extension.AspNetCore/blob/master/samples/AccessControlDemo/Startup.cs)

``` csharp
// ConfigureServices
services.AddAccessControl<ResourceAccessStrategy, ControlAccessStrategy>();

// 自己注册服务，如果只用到资源访问，比如只有 API 可以只注册 IResourceAccessStrategy 策略
//services.TryAddScoped<IResourceAccessStrategy, ActionAccessStrategy>();
// 反之如果只用到视图上的权限控制可以只注册 IControlAccessStrategy 策略
//services.TryAddSingleton<IControlAccessStrategy, ControlAccessStrategy>();
// 最后注册权限控制组件
//services.AddAccessControl();

// 自定义服务生命周期
// services.AddAccessControl<ActionAccessStrategy, ControlAccessStrategy>(ServiceLifetime.Scoped, ServiceLifetime.Singleton);

// ASP.NET Core【推荐用法1】
services.AddAccessControl(options =>
{
    options.UseAsDefaultPolicy = true;
    options.AccessKeyResolver = context => context.RequestServices
                        .GetRequiredService<AccessKeyResolver>()
                        .GetAccessKey(context.Request.Path);
})
    .AddResourceAccessStrategy<ResourceAccessStrategy>(ServiceLifetime.Scoped)
    .AddControlAccessStrategy<ControlAccessStrategy>();

// ASP.NET Core【推荐用法2】
services.AddAccessControl<ResourceAccessStrategy, ControlAccessStrategy>(options =>
{
    options.UseAsDefaultPolicy = true;
    options.AccessKeyResolver = context => context.RequestServices
                        .GetRequiredService<AccessKeyResolver>()
                        .GetAccessKey(context.Request.Path);
});

services.TryAddSingleton<AccessKeyResolver>();

// 全局权限控制的使用（会忽略控制器的 [AllowAnonymous] 特性）
// app.UseAccessControl();
```

### 4. 控制 `Action` 的方法权限

​	通过 `AccessControl` 和 `NoAccessControl` 标签特性来控制 `Action` 的访问权限，如果Action上定义了 `NoAccessControl` 可以忽略上级定义的 `AccessControl`，另外可以设置 Action 对应的 `AccessKey`：

``` csharp
[NoAccessControl]
public IActionResult Index()
{
    return View();
}

[AccessControl]
public IActionResult About()
{
    ViewData["Message"] = "Your application description page.";
    return View();
}

[AccessControl(AccessKey = "Contact")]
public IActionResult Contact()
{
    ViewData["Message"] = "Your contact page.";
    return View();
}
```

也可以设置 `Policy` 和直接使用 `[AccessControl]` 方法一致：

``` csharp
// [Authorize(AccessControlConstants.PolicyName)]
[Authorize("AccessControl")]
public IActionResult Contact()
{
    ViewData["Message"] = "Your contact page.";
    return View();
}
```

### 5. 控制页面元素的显示

​	为了使用比较方便，建议在页面上导入命名空间，具体方法如下，详见  [Samples](https://github.com/Run2948/AccessControl.Extension.AspNetCore/blob/master/samples/AccessControlDemo)：

#### HtmlHelper 扩展

1. 添加命名空间引用

    在  **_ViewImports.cshtml** 中引用命名空间 `AccessControl.Extension.AspNetCore`

    ``` csharp
    @using AccessControlDemo
    // add AccessControl.Extension.AspNetCore
    @using AccessControl.Extension.AspNetCore
    @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
    ```

2. 在 Razor 页面上使用

      - `SparkContainer` 使用

          ``` csharp
          @using(Html.SparkContainer("div",new { @class="container", custom-attribute = "abcd" }))
          {
              @Html.Raw("1234")
          }
          
          @using (Html.SparkContainer("span",new { @class = "role" }, "user:role:view"))
          {
              @:12344
          }
          
          @using (Html.SparkContainer("button",new { @type="button", @class= "btn btn-primary" }, "user:role:add"))
          {
              @:12344
          }
          ```

          没有权限访问就不会渲染到页面上，有权限访问的时候渲染得到的 Html 如下：

          ``` html
          <div class="container" custom-attribute="abcd">1234</div>
          
          <span class="role">12344</span>
          
          <button class="btn btn-primary" type="button">12234</button>
          ```


      - `SparkActionLink`

          ``` csharp
          @Html.SparkActionLink("Learn about me &raquo;", "About", "Home", new { @class = "btn btn-default", "user:detail:show" })
          ```

          有权限访问时渲染出来的 html 如下：

          ``` html
          <a class="btn btn-default" href="http://localhost:5000/Home/About">Learn about me »</a>
          ```


#### TagHelper  注册

1. 添加 TagHelper 引用

    在 **_ViewImports.cshtml** 中引用 `AccessControl.Extension.AspNetCore` TagHelper

    ``` csharp
    @using AccessControlDemo
    @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
    // add AccessControl.Extension.AspNetCore TagHelper
    @addTagHelper *, AccessControl.Extension.AspNetCore
    ```

2. 在 Razor 页面上使用

    在需要权限控制的元素上增加 `asp-access` 即可，如果需要配置 access-key 通过 `asp-accesss-key` 来配置，示例：`<ul class="list-group" asp-access asp-access-key="user:list:view">...</ul>`

    这样有权限的时候就会输出这个 `ul` 的内容，如果没有权限就不会输出，而且出于安全考虑，如果有配置 `asp-access-key` 的话也会把 `asp-access-key` 给移除，不会输出到浏览器中。
