using System;
using UnityEngine;
using ItemStatsSystem;

// ⚠️ 修改这个命名空间为你的 Mod 名称（必须与 info.ini 中的 name 字段一致）
namespace AirdropBox
{
    /// <summary>
    /// Mod 主类
    /// 继承自 Duckov.Modding.ModBehaviour
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {   
        /// </summary>
        private bool hasSpawnedBox = false;
        private InteractableLootbox[] availablePrefabs = null;
        private int currentPrefabIndex = -1; // -1 表示使用默认样式
        private string targetPrefabName = "Loot_TechnicalSuppliesBox"; // 指定的箱子模型名称

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
            // 检查游戏是否开始且玩家存在
            if (!hasSpawnedBox && LevelManager.Instance != null && LevelManager.Instance.MainCharacter != null)
            {
                SpawnEmptyLootbox();
                hasSpawnedBox = true;
            }

            // 示例：按 F8 键手动生成箱子
            if (Input.GetKeyDown(KeyCode.F8))
            {
                SpawnEmptyLootbox();
            }

            // 按 F9 键切换箱子样式
            if (Input.GetKeyDown(KeyCode.F9))
            {
                CycleBoxStyle();
            }

            // 按 F7 键列出所有可用样式
            if (Input.GetKeyDown(KeyCode.F7))
            {
                ListAllBoxStyles();
            }
        }

        /// <summary>
        /// 在玩家附近生成空箱子
        /// </summary>
        private void SpawnEmptyLootbox()
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null)
            {
                Debug.LogWarning("[AirdropBox] 玩家不存在，无法生成箱子");
                return;
            }

            // 在玩家前方2米处生成
            Vector3 spawnPos = player.transform.position + player.transform.forward * 2f;

            // 创建一个容器物品作为箱子
            Item containerItem = new GameObject("EmptyAirdropBox").AddComponent<Item>();
            Inventory inventory = containerItem.gameObject.AddComponent<Inventory>();
            
            // 设置箱子容量为20格（空箱子）
            inventory.SetCapacity(20);

            // 确定要使用的预制体
            InteractableLootbox prefabToUse = null;
            
            // 首先尝试查找指定的箱子模型
            if (!string.IsNullOrEmpty(targetPrefabName))
            {
                InteractableLootbox[] allBoxes = Resources.FindObjectsOfTypeAll<InteractableLootbox>();
                foreach (var box in allBoxes)
                {
                    if (box.name == targetPrefabName)
                    {
                        prefabToUse = box;
                        Debug.Log($"[AirdropBox] 找到并使用指定样式: {prefabToUse.name}");
                        break;
                    }
                }
                
                if (prefabToUse == null)
                {
                    Debug.LogWarning($"[AirdropBox] 未找到指定样式 '{targetPrefabName}'，尝试使用选择的样式");
                }
            }
            
            // 如果没有找到指定样式，使用用户选择的样式
            if (prefabToUse == null && currentPrefabIndex >= 0 && availablePrefabs != null && currentPrefabIndex < availablePrefabs.Length)
            {
                prefabToUse = availablePrefabs[currentPrefabIndex];
                Debug.Log($"[AirdropBox] 使用选定样式: {prefabToUse.name} (索引: {currentPrefabIndex})");
            }
            
            if (prefabToUse == null)
            {
                Debug.Log("[AirdropBox] 使用默认样式");
            }

            // 在地图上创建箱子
            InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
                item: containerItem,
                position: spawnPos,
                rotation: Quaternion.identity,
                moveToMainScene: true,
                prefab: prefabToUse  // 使用找到的预制体
            );

            if (lootbox != null)
            {
                Debug.Log($"[AirdropBox] 空箱子已生成在玩家附近: {spawnPos}");
                player.PopText("空投箱已生成！", 2f);
            }
            else
            {
                Debug.LogError("[AirdropBox] 箱子生成失败");
            }

            // 清理临时容器物品
            GameObject.Destroy(containerItem.gameObject);
        }

        /// <summary>
        /// 列出所有可用的箱子样式
        /// </summary>
        private void ListAllBoxStyles()
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null) return;

            // 查找所有箱子预制体
            InteractableLootbox[] allLootboxes = Resources.FindObjectsOfTypeAll<InteractableLootbox>();
            
            Debug.Log($"[AirdropBox] ===== 找到 {allLootboxes.Length} 个箱子样式 =====");
            player.PopText($"找到 {allLootboxes.Length} 个箱子样式，查看日志", 3f);
            
            for (int i = 0; i < allLootboxes.Length; i++)
            {
                string sceneName = allLootboxes[i].gameObject.scene.name;
                string info = $"[{i}] {allLootboxes[i].name} (场景: {sceneName})";
                Debug.Log($"[AirdropBox] {info}");
            }
            
            Debug.Log($"[AirdropBox] 当前使用样式索引: {currentPrefabIndex} (-1 = 默认样式)");
        }

        /// <summary>
        /// 切换箱子样式
        /// </summary>
        private void CycleBoxStyle()
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null) return;

            // 首次调用时加载所有预制体
            if (availablePrefabs == null)
            {
                availablePrefabs = Resources.FindObjectsOfTypeAll<InteractableLootbox>();
                Debug.Log($"[AirdropBox] 加载了 {availablePrefabs.Length} 个箱子预制体");
            }

            if (availablePrefabs.Length == 0)
            {
                player.PopText("没有找到可用的箱子样式", 2f);
                return;
            }

            // 循环到下一个样式
            currentPrefabIndex++;
            if (currentPrefabIndex >= availablePrefabs.Length)
            {
                currentPrefabIndex = -1; // 回到默认样式
            }

            string styleName = currentPrefabIndex == -1 ? "默认样式" : availablePrefabs[currentPrefabIndex].name;
            Debug.Log($"[AirdropBox] 切换到箱子样式: {styleName} (索引: {currentPrefabIndex})");
            player.PopText($"箱子样式: {styleName}", 2f);
        }

        /// <summary>
        /// Mod 卸载
        /// 在 Mod 被卸载或游戏退出时调用
        /// </summary>
        void OnDestroy()
        {
            Debug.Log("[AirdropBox] Mod Unloaded");

            // 在这里清理资源
        }
    }
}
