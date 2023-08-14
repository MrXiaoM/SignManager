<div align="center">
  <img width="256" src="icon.png" alt="logo"></br>

# SignManager

[![Stars](https://img.shields.io/github/stars/MrXiaoM/SignManager?label=%E6%A0%87%E6%98%9F&logo=github)](https://github.com/MrXiaoM/SignManager/stargazers) [![MiraiForum](https://img.shields.io/badge/%E5%B8%96%E5%AD%90-%E6%9D%A5%E8%87%AA%20MiraiForum-5195E5)](https://mirai.mamoe.net/topic/2421) [![Releases](https://img.shields.io/github/downloads/MrXiaoM/SignManager/total?label=%E4%B8%8B%E8%BD%BD%E9%87%8F&logo=github)](https://github.com/MrXiaoM/SignManager/releases)

SignManager 是适用于 [mirai](https://github.com/mamoe/mirai) 平台的签名服务管理器

图标由画师[人间工作](https://www.pixiv.net/artworks/110690575)绘制

</div>

> 本程序用于配置 [cssxsh/fix-protocol-version](https://github.com/cssxsh/fix-protocol-version) 对接的签名服务。  
> 如果你想使用**即装即用**的内嵌签名服务，另请参见 [MrXiaoM/qsign](https://github.com/MrXiaoM/qsign)

# 使用方法

本软件仅支持在 Windows 下运行，你可以在 Windows 下配置好签名服务再迁移至其他系统使用。

欲运行本程序，需要安装 [.NET Core 6.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0) 桌面运行时环境。

本程序不支持 32 位操作系统，请使用 Windows 7 或以上的 64 位操作系统。

到 [Releases](https://github.com/MrXiaoM/SignManager/releases) 下载 `SignManager-x.x.x-win-x64.exe`，放入 mirai 所在目录并打开，即可安装并配置签名服务。

**如果你觉得有用，不妨给本帖`点赞`或到项目地址点一个 `Star`。**

# 入门级教程

如果无法加载版本列表，请关闭代理、重启程序后再试。  
如果还不行，可能是你请求次数过多，请至少等待1小时后再试。

目前推荐使用 `8.9.63` (较稳定)，下文以 `8.9.63` 举例，其它版本的安装方法雷同。

1. **确保 mirai 已经彻底关闭**。
2. 将本程序放到 mirai 所在目录 (那里可以看到 config、data、plugins 等文件夹)，然后打开本程序。
3. 点击 `下载/更新签名服务`，选择一个版本，点击 `下载`，等待安装完成。
4. 在 `签名服务相关配置` 中选择版本 `8.9.63`，点击 `生成该版本启动脚本`。
5. 若 `签名对接插件` 显示 `未安装`，点击 `安装/更新插件`，选择一个版本，点击 `下载`，等待安装完成。
6. 点击 `签名服务连接配置`，点击 `8.9.63` 版本，如果没有就点 `新建`，输入`8.9.63`新建一个配置并选中它。
7. 在 `读取配置` 选中 `8.9.63`，点击 `从签名服务配置中读取`，点击 `保存`，关闭窗口。
8. 点击 `下载协议信息`，`刷新版本列表`，选择 `8.9.63`，下载它的 `ANDROID_PHONE`，完成后关闭窗口。

如果 `检查` 处3个状态灯**都是绿灯**，则代表配置完成。

配置完成后，打开签名服务脚本，  
Windows 是 `start_unidbg-fetch-qsign.cmd`  
Linux/macOS 是 `start_unidbg-fetch-qsign.sh`

在浏览器打开 `检查` 里提到的签名服务 `服务地址`，如果出现 `IAA 云天明 章北海` 之类的字样则代表**签名服务开启成功**。  
如果签名服务开启失败，点击 `更改该版本配置`，将服务地址那行**第二个输入框**的数字改成 `1024-65535` 之间任意一个数，保存之后再重新打开脚本，重新验证签名服务运行是否正常。

签名服务成功开启后，再打开 mirai，用以下命令登录即可
```
/login 账号 密码
```


# 引用项目

- [mamoe/mirai](https://github.com/mamoe/mirai)
- [cssxsh/fix-protocol-version](https://github.com/cssxsh/fix-protocol-version)
- [fuqiuluo/unidbg-fetch-qsign](https://github.com/fuqiuluo/unidbg-fetch-qsign)
- [NingShenTian/CsharpJson](https://github.com/NingShenTian/CsharpJson)
