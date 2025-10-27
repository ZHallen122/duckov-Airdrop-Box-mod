# 在地图上生成和放置对象

## 概述

在《逃离鸭科夫》中，你可以通过 Mod 在地图上动态生成和放置各种对象，包括箱子、物品、敌人、传送点等。本指南将介绍相关的 API 和使用方法。

## 在地图上放置箱子（LootBox）

### 核心 API：InteractableLootbox.CreateFromItem

游戏提供了一个强大的静态方法来从物品创建可交互的箱子：

```csharp
public static InteractableLootbox CreateFromItem(
    Item item, 
    Vector3 position, 
    Quaternion rotation, 
    bool moveToMainScene = true, 
    InteractableLootbox prefab = null, 
    bool filterDontDropOnDead = false
)
```

#### 参数说明

- `item` - 源物品，箱子会包含这个物品的所有内容（插槽物品和背包物品）
- `position` - 箱子在世界中的位置
- `rotation` - 箱子的旋转角度
- `moveToMainScene` - 是否将箱子移动到主场景（默认 true）
- `prefab` - 使用的箱子预制体（默认使用游戏内置的）
- `filterDontDropOnDead` - 是否过滤掉不应该掉落的物品（默认 false）

### 示例1：创建简单的战利品箱

```csharp
using ItemStatsSystem;
using UnityEngine;

public class LootboxSpawner
{
    public static async void SpawnLootbox(Vector3 position)
    {
        // 1. 创建一个容器物品
        Item containerItem = new GameObject("LootContainer").AddComponent<Item>();
        Inventory inventory = containerItem.gameObject.AddComponent<Inventory>();
        inventory.SetCapacity(20);
        
        // 2. 向容器中添加一些物品
        Item weapon = await ItemAssetsCollection.InstantiateAsync(254); // 示例物品ID
        Item ammo = await ItemAssetsCollection.InstantiateAsync(100);
        
        inventory.AddItem(weapon);
        inventory.AddItem(ammo);
        
        // 3. 在地图上创建箱子
        InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
            item: containerItem,
            position: position,
            rotation: Quaternion.identity,
            moveToMainScene: true
        );
        
        if (lootbox != null)
        {
            Debug.Log($"箱子已生成在: {position}");
        }
        
        // 4. 清理临时容器物品
        GameObject.Destroy(containerItem.gameObject);
    }
}
```

### 示例2：在玩家附近生成宝箱

```csharp
using UnityEngine;

public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void Update()
    {
        // 按 F8 键在玩家前方生成箱子
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SpawnLootboxNearPlayer();
        }
    }
    
    private async void SpawnLootboxNearPlayer()
    {
        CharacterMainControl player = LevelManager.Instance.MainCharacter;
        if (player == null) return;
        
        // 在玩家前方2米处生成
        Vector3 spawnPos = player.transform.position + player.transform.forward * 2f;
        
        // 创建容器并添加随机物品
        Item containerItem = new GameObject("RandomLoot").AddComponent<Item>();
        Inventory inv = containerItem.gameObject.AddComponent<Inventory>();
        inv.SetCapacity(15);
        
        // 添加3-5个随机物品
        int itemCount = Random.Range(3, 6);
        ItemFilter filter = new ItemFilter();
        int[] allItemIds = ItemAssetsCollection.Search(filter);
        
        for (int i = 0; i < itemCount; i++)
        {
            int randomId = allItemIds[Random.Range(0, allItemIds.Length)];
            Item randomItem = await ItemAssetsCollection.InstantiateAsync(randomId);
            if (randomItem != null)
            {
                inv.AddItem(randomItem);
            }
        }
        
        // 创建箱子
        InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
            containerItem,
            spawnPos,
            Quaternion.identity
        );
        
        if (lootbox != null)
        {
            player.PopText("宝箱已生成！", 2f);
        }
        
        GameObject.Destroy(containerItem.gameObject);
    }
}
```

### 示例3：从角色尸体创建战利品箱

```csharp
public class DeathLootSpawner
{
    public static void CreateLootFromCharacter(CharacterMainControl character)
    {
        if (character == null || character.Equipment == null) return;
        
        Vector3 deathPosition = character.transform.position;
        
        // 使用角色的装备物品创建箱子
        Item characterEquipment = character.Equipment;
        
        // 创建箱子，过滤掉不应该掉落的物品
        InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
            item: characterEquipment,
            position: deathPosition,
            rotation: Quaternion.identity,
            moveToMainScene: true,
            prefab: null,
            filterDontDropOnDead: true  // 过滤粘性物品和标记为不掉落的物品
        );
        
        if (lootbox != null)
        {
            Debug.Log($"战利品箱已生成在: {deathPosition}");
        }
    }
}
```

## 在地图上放置可拾取物品

### 方法1：直接使用 Unity 实例化

```csharp
using UnityEngine;
using ItemStatsSystem;

public class ItemDropper
{
    public static async void DropItemAtPosition(int itemTypeId, Vector3 position)
    {
        // 创建物品实例
        Item item = await ItemAssetsCollection.InstantiateAsync(itemTypeId);
        if (item == null) return;
        
        // 设置位置
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;
        
        // 创建物品代理（用于地图上显示）
        ItemAgent agent = item.AgentUtilities.CreateAgent(ItemAgent.AgentTypes.pickUp);
        
        if (agent != null)
        {
            agent.transform.position = position;
            Debug.Log($"物品 {item.DisplayName} 已掉落到: {position}");
        }
    }
}
```

### 方法2：使用 InteractablePickup

```csharp
public class PickupSpawner
{
    public static async void SpawnPickupItem(int itemTypeId, Vector3 position)
    {
        // 获取可拾取物品的预制体（需要从游戏配置中获取）
        InteractablePickup pickupPrefab = GetPickupPrefab();
        if (pickupPrefab == null)
        {
            Debug.LogError("找不到 InteractablePickup 预制体");
            return;
        }
        
        // 创建物品
        Item item = await ItemAssetsCollection.InstantiateAsync(itemTypeId);
        if (item == null) return;
        
        // 实例化可拾取对象
        InteractablePickup pickup = UnityEngine.Object.Instantiate(
            pickupPrefab,
            position,
            Quaternion.identity
        );
        
        // 设置物品
        DuckovItemAgent itemAgent = item.AgentUtilities.CreateAgent(
            ItemAgent.AgentTypes.pickUp
        ) as DuckovItemAgent;
        
        if (itemAgent != null)
        {
            // 将代理附加到 pickup
            itemAgent.transform.SetParent(pickup.transform);
            itemAgent.transform.localPosition = Vector3.zero;
        }
    }
    
    private static InteractablePickup GetPickupPrefab()
    {
        // 从游戏配置或资源中获取预制体
        // 这需要根据实际情况实现
        return null;
    }
}
```

## 在地图上放置自定义对象

### 创建自定义可交互对象

```csharp
using UnityEngine;

public class CustomInteractable : InteractableBase
{
    protected override bool IsInteractable()
    {
        return true;
    }
    
    protected override void OnInteractStart(CharacterMainControl character)
    {
        // 交互逻辑
        character.PopText("你与自定义对象交互了！", 2f);
        base.StopInteract();
    }
}

// 使用示例
public class CustomObjectSpawner
{
    public static void SpawnCustomObject(Vector3 position)
    {
        // 创建 GameObject
        GameObject obj = new GameObject("CustomInteractable");
        obj.transform.position = position;
        
        // 添加组件
        CustomInteractable interactable = obj.AddComponent<CustomInteractable>();
        interactable.InteractTime = 1.0f;
        interactable.InteractName = "CustomObject";
        
        // 添加视觉效果
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(obj.transform);
        cube.transform.localPosition = Vector3.zero;
        
        Debug.Log($"自定义对象已生成在: {position}");
    }
}
```

## 在地图上生成敌人/NPC

### 使用 CharacterSpawner

```csharp
public class EnemySpawner
{
    public static void SpawnEnemy(Vector3 position)
    {
        // 使用 CharacterCreator 创建角色
        CharacterCreator creator = LevelManager.Instance.CharacterCreator;
        if (creator == null)
        {
            Debug.LogError("CharacterCreator 不可用");
            return;
        }
        
        // 获取敌人预设
        CharacterRandomPreset enemyPreset = GetEnemyPreset();
        if (enemyPreset == null) return;
        
        // 生成角色
        CharacterMainControl enemy = creator.CreateCharacter(
            preset: enemyPreset,
            position: position,
            rotation: Quaternion.identity
        );
        
        if (enemy != null)
        {
            // 设置为敌对
            enemy.SetTeam(Teams.Enemy);
            Debug.Log($"敌人已生成在: {position}");
        }
    }
    
    private static CharacterRandomPreset GetEnemyPreset()
    {
        // 从游戏资源获取敌人预设
        // 需要根据实际情况实现
        return null;
    }
}
```

## 管理生成的对象

### 跟踪生成的对象

```csharp
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObjectManager
{
    private static List<GameObject> spawnedObjects = new List<GameObject>();
    
    public static void RegisterSpawnedObject(GameObject obj)
    {
        if (obj != null && !spawnedObjects.Contains(obj))
        {
            spawnedObjects.Add(obj);
        }
    }
    
    public static void CleanupSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        Debug.Log("所有生成的对象已清理");
    }
    
    public static int GetSpawnedObjectCount()
    {
        // 移除已销毁的对象
        spawnedObjects.RemoveAll(obj => obj == null);
        return spawnedObjects.Count;
    }
}

// 使用示例
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void OnDisable()
    {
        // Mod 卸载时清理所有生成的对象
        SpawnedObjectManager.CleanupSpawnedObjects();
    }
}
```

### 保存和加载生成对象的位置

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using Saves;

[Serializable]
public class SpawnedObjectData
{
    public Vector3 position;
    public Quaternion rotation;
    public string objectType;
    public int itemTypeId; // 如果是箱子或物品
}

public class SpawnDataManager
{
    private const string SAVE_KEY = "ModSpawnedObjects";
    private static List<SpawnedObjectData> spawnedData = new List<SpawnedObjectData>();
    
    public static void SaveSpawnedObject(GameObject obj, string objectType, int itemTypeId = -1)
    {
        SpawnedObjectData data = new SpawnedObjectData
        {
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            objectType = objectType,
            itemTypeId = itemTypeId
        };
        
        spawnedData.Add(data);
        SaveToFile();
    }
    
    private static void SaveToFile()
    {
        string json = JsonUtility.ToJson(new { objects = spawnedData });
        // 使用游戏的存档系统保存
        // 具体实现根据游戏版本可能不同
        SaveUtility.SetString(SAVE_KEY, json);
    }
    
    public static async void LoadAndRestoreObjects()
    {
        string json = SaveUtility.GetString(SAVE_KEY, "");
        if (string.IsNullOrEmpty(json)) return;
        
        // 解析数据并重新生成对象
        // 实现根据具体需求而定
    }
}
```

## 位置选择工具

### 获取玩家位置

```csharp
public static class PositionHelper
{
    public static Vector3 GetPlayerPosition()
    {
        CharacterMainControl player = LevelManager.Instance?.MainCharacter;
        return player != null ? player.transform.position : Vector3.zero;
    }
    
    public static Vector3 GetPlayerForwardPosition(float distance = 2f)
    {
        CharacterMainControl player = LevelManager.Instance?.MainCharacter;
        if (player == null) return Vector3.zero;
        
        return player.transform.position + player.transform.forward * distance;
    }
    
    public static Vector3 GetRandomPositionAroundPlayer(float minRadius = 2f, float maxRadius = 10f)
    {
        CharacterMainControl player = LevelManager.Instance?.MainCharacter;
        if (player == null) return Vector3.zero;
        
        Vector3 playerPos = player.transform.position;
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = UnityEngine.Random.Range(minRadius, maxRadius);
        
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            0,
            Mathf.Sin(angle) * distance
        );
        
        return playerPos + offset;
    }
    
    public static Vector3 GetGroundPosition(Vector3 position)
    {
        // 射线检测找到地面
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            return hit.point;
        }
        return position;
    }
}
```

## 完整示例：动态战利品生成系统

```csharp
using UnityEngine;
using ItemStatsSystem;
using System.Collections.Generic;

namespace DynamicLootSystem
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private List<GameObject> spawnedLootboxes = new List<GameObject>();
        private float spawnTimer = 0f;
        private float spawnInterval = 60f; // 每60秒生成一次
        
        void Start()
        {
            Debug.Log("动态战利品系统已启动");
        }
        
        void Update()
        {
            // 按 F10 手动生成
            if (Input.GetKeyDown(KeyCode.F10))
            {
                SpawnRandomLootbox();
            }
            
            // 自动定时生成
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnRandomLootbox();
            }
        }
        
        private async void SpawnRandomLootbox()
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null) return;
            
            // 在玩家周围随机位置生成
            Vector3 spawnPos = PositionHelper.GetRandomPositionAroundPlayer(10f, 30f);
            spawnPos = PositionHelper.GetGroundPosition(spawnPos);
            
            // 创建容器
            Item containerItem = new GameObject("DynamicLoot").AddComponent<Item>();
            Inventory inv = containerItem.gameObject.AddComponent<Inventory>();
            inv.SetCapacity(Random.Range(8, 20));
            
            // 添加随机物品
            int itemCount = Random.Range(3, 8);
            for (int i = 0; i < itemCount; i++)
            {
                Item randomItem = await CreateRandomItem();
                if (randomItem != null)
                {
                    inv.AddItem(randomItem);
                }
            }
            
            // 创建箱子
            InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
                containerItem,
                spawnPos,
                Quaternion.identity
            );
            
            if (lootbox != null)
            {
                spawnedLootboxes.Add(lootbox.gameObject);
                player.PopText("附近出现了战利品箱！", 3f);
                Debug.Log($"战利品箱已生成 (总数: {spawnedLootboxes.Count})");
            }
            
            GameObject.Destroy(containerItem.gameObject);
        }
        
        private async UniTask<Item> CreateRandomItem()
        {
            ItemFilter filter = new ItemFilter();
            int[] allIds = ItemAssetsCollection.Search(filter);
            
            if (allIds.Length == 0) return null;
            
            int randomId = allIds[Random.Range(0, allIds.Length)];
            return await ItemAssetsCollection.InstantiateAsync(randomId);
        }
        
        void OnDisable()
        {
            // 清理生成的箱子
            foreach (GameObject obj in spawnedLootboxes)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedLootboxes.Clear();
            Debug.Log("动态战利品系统已卸载");
        }
    }
}
```

## 注意事项

1. **性能考虑** - 不要一次生成太多对象，建议限制同时存在的对象数量

2. **位置验证** - 生成前应该检查位置是否合法（不在墙内、有地面支撑等）

3. **清理机制** - Mod 卸载时务必清理所有生成的对象，避免内存泄漏

4. **场景切换** - 场景切换时生成的对象可能会被销毁，需要重新生成或保存数据

5. **多人兼容** - 如果游戏支持多人模式，需要考虑同步问题

6. **存档兼容** - 动态生成的对象默认不会保存到存档，需要自行实现保存逻辑

7. **预制体获取** - 某些对象需要预制体，可能需要从游戏配置或资源中获取

8. **坐标系统** - 注意 Unity 的坐标系统，Y 轴通常是垂直方向

## 相关 API 参考

- [容器物品使用指南](container-items.md)
- [查找物品ID](find-item-ids.md)
- [物品系统](../systems/items.md)
- [角色系统](../systems/character.md)

