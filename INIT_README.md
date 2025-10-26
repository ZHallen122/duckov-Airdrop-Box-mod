# Duckov Mod 项目初始化指南

这是一个基于 PresetLoadout 的 Mod 项目模板。按照以下步骤快速开始你的 Mod 开发。

---

## 第一步：复制模板

```bash
# 复制模板到新项目
cp -r /Volumes/ssd/i/duckov/ModTemplate /Volumes/ssd/i/duckov/YourModName

cd /Volumes/ssd/i/duckov/YourModName
```

---

## 第二步：必须修改的文件

### 1. `YourModName.csproj` (项目配置)

**修改内容**:
```xml
<!-- 第 5 行：修改项目名称 -->
<AssemblyName>YourModName</AssemblyName>

<!-- 第 10 行：修改游戏路径（如果不同） -->
<DuckovPath>/Users/jacksonc/Library/Application Support/Steam/steamapps/common/Escape from Duckov/Duckov.app/Contents/Resources/Data</DuckovPath>
```

**检查方法**:
```bash
# 确认游戏路径是否正确
ls "$DUCKOV_PATH/Managed/TeamSoda.*.dll"
# 应该看到多个 TeamSoda DLL 文件
```

---

### 2. `info.ini` (Mod 信息)

**修改内容**:
```ini
name = YourModName           # 与项目名称一致
displayName = 你的 Mod 显示名称   # 在游戏中显示的名称
description = Mod 功能描述      # 简短描述
```

**⚠️ 重要**: `name` 字段必须与：
- 项目文件名 (`YourModName.csproj`)
- 命名空间 (`namespace YourModName`)
- DLL 文件名 (`YourModName.dll`)
- 完全一致

---

### 3. `ModBehaviour.cs` (主代码)

**修改内容**:
```csharp
// 第 1 行：修改命名空间
namespace YourModName  // 改为你的 Mod 名称
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        void Awake()
        {
            Debug.Log("YourModName Loaded!");  // 修改日志名称
        }
    }
}
```

---

### 4. `scripts/deploy.sh` (部署脚本)

**修改内容**:
```bash
# 第 23 行：修改 Workshop ID（如果使用临时方案）
WORKSHOP_PATH="/Users/jacksonc/Library/Application Support/Steam/steamapps/workshop/content/3167020/YOUR_WORKSHOP_ID"

# 第 25 行：修改 DLL 文件名
cp bin/Release/netstandard2.1/YourModName.dll "$WORKSHOP_PATH/"
```

**如果使用正式 Mods 文件夹**:
```bash
MODS_PATH="/Users/jacksonc/Library/Application Support/Steam/steamapps/common/Escape from Duckov/Duckov.app/Contents/Resources/Data/Mods/YourModName"
```

---

### 5. `README.md` (文档)

修改以下内容：
- Mod 名称
- 功能描述
- 使用方法
- 快捷键
- 作者信息

---

## 第三步：初次编译测试

```bash
# 1. 清理旧文件
dotnet clean

# 2. 编译项目
dotnet build -c Release

# 3. 检查输出
ls bin/Release/netstandard2.1/YourModName.dll
# 应该看到 DLL 文件

# 4. 部署到游戏
./scripts/deploy.sh
```

---

## 第四步：验证 Mod 加载

```bash
# 1. 启动日志监控
./scripts/watch-log.sh

# 2. 启动游戏
# 打开《逃离鸭科夫》

# 3. 查看日志
# 应该看到: "YourModName Loaded!"
```

---

## 可选配置

### 修改 `.gitignore`

模板已包含标准的 .gitignore，通常不需要修改。

### 添加依赖 DLL

如果需要引用额外的游戏 DLL：

```xml
<!-- 在 YourModName.csproj 中添加 -->
<ItemGroup>
  <Reference Include="$(DuckovPath)/Managed/YourDependency.dll" />
</ItemGroup>
```

### 修改 `preview.png`

替换 `preview.png` 为你的 Mod 预览图（256×256 像素）。

---

## 快速检查清单

在开始开发前，确认以下都已修改：

- [ ] `YourModName.csproj` - AssemblyName 和 DuckovPath
- [ ] `info.ini` - name, displayName, description
- [ ] `ModBehaviour.cs` - namespace 和日志
- [ ] `scripts/deploy.sh` - WORKSHOP_PATH 和 DLL 文件名
- [ ] `README.md` - Mod 信息
- [ ] 项目文件夹名称已重命名
- [ ] 项目文件 `.csproj` 已重命名

---

## 常见问题

### Q: 编译失败 "找不到 TeamSoda.*.dll"
**A**: 检查 `<DuckovPath>` 是否正确，路径应该指向 `.app/Contents/Resources/Data`

### Q: 游戏中看不到 Mod
**A**:
1. 检查 `info.ini` 中的 `name` 字段是否与 DLL 文件名一致
2. macOS 上检查是否部署到 Workshop 目录
3. 查看日志确认 Mod 是否加载

### Q: Mod 加载但没有任何反应
**A**:
1. 检查命名空间是否与 `info.ini` 的 `name` 一致
2. 确认 `ModBehaviour` 类继承自 `Duckov.Modding.ModBehaviour`
3. 查看日志中的错误信息

---

## 下一步

初始化完成后，开始开发你的 Mod！

**推荐阅读**:
- [NotableAPIs_CN.md](/Users/jacksonc/i/duckov/duckov_modding/Documents/NotableAPIs_CN.md) - 游戏 API 文档
- [scripts/README.md](scripts/README.md) - 开发脚本说明
- [PresetLoadout](../PresetLoadout) - 完整示例项目

**开发流程**:
1. 修改代码
2. `./scripts/deploy.sh` 编译部署
3. 重启游戏测试
4. 查看 `./scripts/watch-log.sh` 调试

---

祝你开发顺利！🎮
