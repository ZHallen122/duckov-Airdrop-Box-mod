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

            // 在地图上创建箱子
            InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
                item: containerItem,
                position: spawnPos,
                rotation: Quaternion.identity,
                moveToMainScene: true
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
