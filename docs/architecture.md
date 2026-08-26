# 架构说明

```text
浏览器
   │
   ▼
GamePulse.Web
   │  Blazor Server 数据看板与导入交互
   ▼
HTTP 请求
   │
   ▼
GamePulse.Api
   │  组合依赖、端点与序列化
   ▼
GamePulse.Application
   │  导入校验、指标口径与业务服务
   ▼
GamePulse.Domain
   │  匿名事件、事件类型与领域校验
   ▼
GamePulse.Infrastructure
      EF Core、SQLite 与仓储实现
```

## 依赖方向

- `Domain` 不依赖其他项目。
- `Application` 只依赖 `Domain`，并通过接口描述数据访问需求。
- `Infrastructure` 实现 `Application` 定义的仓储接口。
- `Api` 负责组装依赖并暴露 HTTP 端点。
- `Web` 通过类型化 `HttpClient` 调用 API，不直接访问数据库。
- 单元测试直接验证纯指标函数；集成测试验证真实 HTTP 管线。

## 数据口径

- 玩家数：事件中不同 `PlayerId` 的数量。
- 对局数：`GameStart` 事件中不同 `GameId` 的数量。
- 平均时长与平均得分：仅统计包含对应值的 `GameOver` 事件。
- 每日趋势：按事件发生日期聚合对局数，并计算当天结束对局的平均得分。
- 次日留存：玩家首次活跃日后的下一自然日仍有事件；数据集最后一天首次出现的玩家不进入分母。
- 漏斗转化率：各阶段会话数除以已开始会话数。

## 后续扩展

后续将增加 CSV 导入、查询筛选、玩家分群、权限和审计日志。当前 Blazor 看板的视觉基线与响应式规则记录在 `docs/design/dashboard-design-spec.md`。
