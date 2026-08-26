# 匿名游戏事件数据字典

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `id` | UUID | 是 | 事件唯一标识，用于去重 |
| `playerId` | string | 是 | 不可反查真实身份的匿名玩家 ID |
| `sessionId` | string | 是 | 一次访问会话的匿名 ID |
| `gameId` | string | 是 | 一局游戏的匿名 ID |
| `eventType` | enum | 是 | `SessionStart`、`GameStart`、`Move`、`Merge`、`GameOver`、`SettingChange` |
| `occurredAt` | ISO 8601 | 是 | 事件发生时间，建议统一使用 UTC |
| `gameVersion` | string | 是 | 游戏版本，例如 `1.0.0` |
| `deviceType` | string | 是 | 匿名设备类别，不记录唯一设备标识 |
| `score` | integer | 否 | 当前或最终得分，不能为负数 |
| `stepCount` | integer | 否 | 当前或最终步数，不能为负数 |
| `durationSeconds` | integer | 否 | 对局时长，单位为秒 |
| `fruitLevel` | integer | 否 | 当前最高合成层级 |
| `direction` | string | 否 | `Move` 事件方向：`up`、`down`、`left`、`right` |

数据中禁止出现真实姓名、电话、邮箱、微信标识、精确位置、IP 地址或其他可识别个人身份的信息。
