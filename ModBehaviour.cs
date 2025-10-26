using System;
using UnityEngine;

// ⚠️ 修改这个命名空间为你的 Mod 名称（必须与 info.ini 中的 name 字段一致）
namespace AirdropBox
{
    /// <summary>
    /// Mod 主类
    /// 继承自 Duckov.Modding.ModBehaviour
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        /// <summary>
        /// Mod 初始化
        /// 在 Mod 加载时调用一次
        /// </summary>
        void Awake()
        {
            Debug.Log("[AirdropBox] Mod Loaded!");  // 修改日志前缀

            // 在这里进行初始化操作
            // 例如：加载配置、注册事件等
        }

        /// <summary>
        /// 每帧调用
        /// 用于处理按键输入、更新 UI 等
        /// </summary>
        void Update()
        {
            // 示例：按 H 键显示消息
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("[YourModName] Hello, World!");
            }

            // 在这里添加你的逻辑
        }

        /// <summary>
        /// Mod 卸载
        /// 在 Mod 被卸载或游戏退出时调用
        /// </summary>
        void OnDestroy()
        {
            Debug.Log("[YourModName] Mod Unloaded");

            // 在这里清理资源
        }
    }
}
