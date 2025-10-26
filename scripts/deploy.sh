#!/bin/bash
# YourModName - 快速部署脚本
# ⚠️ 修改下面的 WORKSHOP_PATH 和 DLL 文件名

set -e  # 遇到错误立即退出

# 切换到项目根目录（脚本在 scripts/ 子目录中）
cd "$(dirname "$0")/.."

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔨 编译 YourModName..."  # 修改 Mod 名称
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

dotnet build -c Release

if [ $? -eq 0 ]; then
    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "📦 部署到游戏目录..."
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

    # ⚠️ 修改这个路径为你的部署目标
    # 临时方案：使用 Workshop ID
    WORKSHOP_PATH="/Users/jacksonc/Library/Application Support/Steam/steamapps/workshop/content/3167020/YOUR_WORKSHOP_ID"

    # 正式方案：使用 Mods 文件夹（如果可用）
    # MODS_PATH="/Users/jacksonc/Library/Application Support/Steam/steamapps/common/Escape from Duckov/Duckov.app/Contents/Resources/Data/Mods/YourModName"

    # ⚠️ 修改 DLL 文件名
    cp bin/Release/netstandard2.1/YourModName.dll "$WORKSHOP_PATH/"

    echo ""
    echo "✅ 部署完成!"
    echo ""
    echo "📍 Mod 位置: $WORKSHOP_PATH"
    echo "🎮 下一步: 重启游戏测试"
    echo ""
else
    echo ""
    echo "❌ 编译失败! 请检查错误信息"
    exit 1
fi
