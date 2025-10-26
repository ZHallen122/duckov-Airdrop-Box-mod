# YourModName

<!-- ⚠️ 修改以下所有内容为你的 Mod 信息 -->

一个用于《逃离鸭科夫》的 Mod。

## 功能特性

- ✅ 功能 1
- ✅ 功能 2
- ✅ 功能 3

## 使用方法

### 基本操作
1. 步骤 1
2. 步骤 2
3. 步骤 3

## 快捷键一览

| 快捷键 | 功能 |
|--------|------|
| H | 示例功能 |

## 安装方法

### 方法 1: 使用编译好的版本
1. 下载发布的 `YourModName` 文件夹
2. 将文件夹复制到游戏目录: `<游戏安装路径>/Duckov_Data/Mods/`
3. 启动游戏，在 Mods 界面启用此 Mod

### 方法 2: 从源码编译
1. 打开 `YourModName.csproj`
2. 修改 `<DuckovPath>` 为你的游戏安装路径
3. 运行快速部署脚本:
   ```bash
   ./scripts/deploy.sh
   ```

## 开发工具

详细的开发脚本和工作流，请参考 [scripts/README.md](scripts/README.md)。

**快速开发命令**:
- `./scripts/deploy.sh` - 编译并部署
- `./scripts/watch-log.sh` - 实时查看游戏日志

## 技术细节

### 项目结构
```
YourModName/
├── ModBehaviour.cs       # 主逻辑
├── scripts/              # 开发脚本
│   ├── deploy.sh         # 快速编译部署
│   └── watch-log.sh      # 实时日志监控
├── YourModName.csproj    # 项目配置
└── README.md             # 本文件
```

## 注意事项

- ⚠️ 注意事项 1
- ⚠️ 注意事项 2

## 许可证

本 Mod 遵循《鸭科夫社区准则》。

## 反馈与支持

如遇问题或有建议，请在 GitHub 上提 Issue。
