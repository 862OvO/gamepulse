# GamePulse 游戏数据分析与运营平台

GamePulse 是一个使用 C# 与 .NET 10 构建的游戏数据分析项目。它读取匿名模拟玩家事件，计算核心指标、次日留存、每日趋势和行为漏斗，并通过 Blazor 仪表盘呈现结果，目标是把数据分析、产品运营与游戏策划场景连接到一套可复现的工程实现中。

> 当前版本使用离线匿名样例数据，不采集真实玩家信息，也不包含可识别个人身份的数据。

## 已实现

- ASP.NET Core Web API 与 OpenAPI 文档。
- JSON 匿名事件批量导入、字段校验和 EventId 去重。
- SQLite + Entity Framework Core 持久化。
- 玩家数、对局数、平均时长与平均得分摘要。
- 每日趋势、次日留存率与目标分数行为漏斗。
- Blazor Server 响应式数据看板、侧栏导航和 JSON 导入交互。
- 桌面端与移动端自适应布局及错误、加载、导入反馈状态。
- xUnit 单元测试、HTTP 集成测试和 GitHub Actions CI。

## 技术栈

- C# / .NET 10 LTS
- ASP.NET Core Minimal API
- Blazor Server
- Entity Framework Core / SQLite
- xUnit / Microsoft.AspNetCore.Mvc.Testing
- OpenAPI / GitHub Actions

## 项目结构

```text
GamePulse/
├─ src/
│  ├─ GamePulse.Api/
│  ├─ GamePulse.Application/
│  ├─ GamePulse.Domain/
│  ├─ GamePulse.Infrastructure/
│  └─ GamePulse.Web/
├─ tests/
│  ├─ GamePulse.UnitTests/
│  └─ GamePulse.IntegrationTests/
├─ samples/
├─ docs/
└─ .github/workflows/
```

## 本地运行

需要安装 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)。

```powershell
dotnet restore GamePulse.sln
dotnet build GamePulse.sln --configuration Release
dotnet test GamePulse.sln --configuration Release
```

随后分别打开两个 PowerShell 窗口运行 API 与看板：

```powershell
dotnet run --project src/GamePulse.Api --urls http://localhost:5080
```

```powershell
dotnet run --project src/GamePulse.Web --urls http://localhost:5075
```

运行后可访问：

- 数据看板：`http://localhost:5075`
- 健康检查：`http://localhost:5080/health`
- OpenAPI：`http://localhost:5080/openapi/v1.json`

导入仓库中的匿名样例数据：

```powershell
Invoke-RestMethod `
  -Uri http://localhost:5080/api/imports/json `
  -Method Post `
  -ContentType 'application/json' `
  -InFile samples/game-events.sample.json
```

随后访问：

- `GET /api/dashboard/summary`
- `GET /api/analytics/trend`
- `GET /api/analytics/retention`
- `GET /api/analytics/funnel?targetScore=512`

## 文档

- [第一阶段需求](docs/requirements.md)
- [架构与指标口径](docs/architecture.md)
- [匿名事件数据字典](docs/data-dictionary.md)
- [看板视觉设计规范](docs/design/dashboard-design-spec.md)

## 后续计划

- CSV 导入、异常记录下载与模拟数据生成器。
- 查询筛选、玩家分群、难度曲线和报表导出。
- 运营活动配置与 A/B 方案对比。
- 权限、审计日志与可配置运营活动。

## 隐私说明

本项目仅使用离线匿名模拟数据。仓库不应包含真实玩家姓名、联系方式、微信标识、精确位置、IP 地址、线上 AppId、访问令牌或数据库文件。

## 作者

GitHub：[@862OvO](https://github.com/862OvO)
