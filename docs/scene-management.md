# 场景和地图管理

## 概述

在《逃离鸭科夫》中，游戏使用 Unity 的场景系统来管理不同的地图。了解场景系统对于在正确的地图上生成对象、监听场景切换事件至关重要。

## 场景类型

游戏中主要有两种类型的场景：

### 1. 基地场景（Base Level）
- **特征**：`LevelConfig.IsBaseLevel == true`
- **用途**：玩家的基地/避难所
- **ID**：`"Base"`
- **特点**：
  - 安全区域，通常不会有敌人
  - 可以存放物品、休息、制作
  - 通常不会生成战利品箱

### 2. 突袭地图（Raid Map）
- **特征**：`LevelConfig.IsRaidMap == true`
- **用途**：战斗、搜刮物资的地图
- **特点**：
  - 有敌人生成
  - 有战利品箱
  - 有撤离点
  - 时间限制（某些地图）

## 检查当前场景

### 获取场景信息

```csharp
using UnityEngine.SceneManagement;

public static class SceneHelper
{
    // 获取当前场景
    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
    
    // 获取当前场景名称
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    // 获取当前场景的 Build Index
    public static int GetCurrentSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    
    // 检查是否在基地
    public static bool IsInBase()
    {
        return LevelConfig.IsBaseLevel;
    }
    
    // 检查是否在突袭地图
    public static bool IsInRaidMap()
    {
        return LevelConfig.IsRaidMap;
    }
    
    // 获取场景ID
    public static string GetCurrentSceneID()
    {
        int buildIndex = GetCurrentSceneBuildIndex();
        return SceneInfoCollection.GetSceneID(buildIndex);
    }
}
```

### 使用示例

```csharp
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void Start()
    {
        // 检查当前场景
        string sceneName = SceneHelper.GetCurrentSceneName();
        string sceneID = SceneHelper.GetCurrentSceneID();
        
        Debug.Log($"当前场景: {sceneName}");
        Debug.Log($"场景ID: {sceneID}");
        
        if (SceneHelper.IsInBase())
        {
            Debug.Log("当前在基地");
            // 只在基地执行的逻辑
        }
        else if (SceneHelper.IsInRaidMap())
        {
            Debug.Log("当前在突袭地图");
            // 只在突袭地图执行的逻辑
        }
    }
}
```

## 在特定场景生成对象

### 根据场景类型生成

```csharp
public class ConditionalSpawner
{
    public static void SpawnLootboxIfInRaid(Vector3 position)
    {
        // 只在突袭地图生成箱子
        if (!LevelConfig.IsRaidMap)
        {
            Debug.Log("不在突袭地图，取消生成");
            return;
        }
        
        // 生成箱子的代码
        SpawnLootbox(position);
    }
    
    public static void SpawnStorageBoxInBase(Vector3 position)
    {
        // 只在基地生成储物箱
        if (!LevelConfig.IsBaseLevel)
        {
            Debug.Log("不在基地，取消生成");
            return;
        }
        
        // 生成储物箱的代码
        SpawnStorageBox(position);
    }
    
    private static async void SpawnLootbox(Vector3 position)
    {
        Item containerItem = new GameObject("RaidLoot").AddComponent<Item>();
        Inventory inv = containerItem.gameObject.AddComponent<Inventory>();
        inv.SetCapacity(20);
        
        // 添加物品...
        
        InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
            containerItem,
            position,
            Quaternion.identity
        );
        
        GameObject.Destroy(containerItem.gameObject);
    }
    
    private static void SpawnStorageBox(Vector3 position)
    {
        // 储物箱逻辑
    }
}
```

### 检查特定场景

```csharp
public class SpecificSceneSpawner
{
    // 游戏中的场景ID示例（实际ID需要查询游戏数据）
    private const string SCENE_ID_BASE = "Base";
    // 其他场景ID需要通过游戏数据获取
    
    public static void SpawnIfInSpecificScene(string requiredSceneID, Vector3 position)
    {
        string currentSceneID = SceneHelper.GetCurrentSceneID();
        
        if (currentSceneID != requiredSceneID)
        {
            Debug.Log($"当前场景 {currentSceneID} 不是目标场景 {requiredSceneID}");
            return;
        }
        
        Debug.Log($"在目标场景 {requiredSceneID} 中，开始生成");
        // 生成逻辑...
    }
}
```

## 监听场景切换事件

### 使用 SceneLoader 事件

```csharp
using Duckov.Scenes;

public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void OnEnable()
    {
        // 监听场景开始加载
        SceneLoader.onStartedLoadingScene += OnSceneLoadStart;
        
        // 监听场景加载完成
        SceneLoader.onFinishedLoadingScene += OnSceneLoadFinish;
        
        // 监听场景激活前
        SceneLoader.onBeforeSetSceneActive += OnBeforeSceneActive;
        
        // 监听场景初始化后
        SceneLoader.onAfterSceneInitialize += OnAfterSceneInitialize;
    }
    
    void OnDisable()
    {
        SceneLoader.onStartedLoadingScene -= OnSceneLoadStart;
        SceneLoader.onFinishedLoadingScene -= OnSceneLoadFinish;
        SceneLoader.onBeforeSetSceneActive -= OnBeforeSceneActive;
        SceneLoader.onAfterSceneInitialize -= OnAfterSceneInitialize;
    }
    
    private void OnSceneLoadStart(SceneLoadingContext context)
    {
        Debug.Log($"开始加载场景");
        // 清理当前场景的对象
        CleanupCurrentSceneObjects();
    }
    
    private void OnSceneLoadFinish(SceneLoadingContext context)
    {
        Debug.Log($"场景加载完成");
    }
    
    private void OnBeforeSceneActive(SceneLoadingContext context)
    {
        Debug.Log($"场景即将激活");
    }
    
    private void OnAfterSceneInitialize(SceneLoadingContext context)
    {
        Debug.Log($"场景初始化完成");
        
        // 在新场景中生成对象
        string sceneID = SceneHelper.GetCurrentSceneID();
        Debug.Log($"进入场景: {sceneID}");
        
        if (LevelConfig.IsRaidMap)
        {
            // 在突袭地图生成额外的战利品
            SpawnExtraLoot();
        }
    }
    
    private void CleanupCurrentSceneObjects()
    {
        // 清理逻辑
    }
    
    private void SpawnExtraLoot()
    {
        // 生成逻辑
    }
}
```

### 使用 Unity 场景管理事件

```csharp
using UnityEngine.SceneManagement;

public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景已加载: {scene.name}");
        
        // 等待一帧后再操作，确保场景完全初始化
        StartCoroutine(DelayedSceneSetup(scene));
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"场景已卸载: {scene.name}");
    }
    
    private IEnumerator DelayedSceneSetup(Scene scene)
    {
        yield return null; // 等待一帧
        
        // 现在可以安全地在场景中生成对象
        SetupSceneObjects();
    }
    
    private void SetupSceneObjects()
    {
        // 设置逻辑
    }
}
```

## 场景持久化对象

### 跨场景保持对象

```csharp
using UnityEngine;

public class PersistentObjectManager
{
    private static GameObject persistentRoot;
    
    // 创建跨场景持久化的对象
    public static GameObject CreatePersistentObject(string name)
    {
        if (persistentRoot == null)
        {
            persistentRoot = new GameObject("ModPersistentRoot");
            GameObject.DontDestroyOnLoad(persistentRoot);
        }
        
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(persistentRoot.transform);
        
        return obj;
    }
    
    // 清理所有持久化对象
    public static void CleanupPersistentObjects()
    {
        if (persistentRoot != null)
        {
            GameObject.Destroy(persistentRoot);
            persistentRoot = null;
        }
    }
}

// 使用示例
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    private GameObject persistentManager;
    
    void Start()
    {
        // 创建一个跨场景的管理器
        persistentManager = PersistentObjectManager.CreatePersistentObject("MyManager");
        persistentManager.AddComponent<CustomManager>();
    }
    
    void OnDisable()
    {
        // Mod卸载时清理
        PersistentObjectManager.CleanupPersistentObjects();
    }
}
```

## 场景特定的生成策略

### 根据场景类型调整生成

```csharp
public class AdaptiveSpawner
{
    public static async void SpawnAdaptiveLootbox(Vector3 position)
    {
        Item containerItem = new GameObject("AdaptiveLoot").AddComponent<Item>();
        Inventory inv = containerItem.gameObject.AddComponent<Inventory>();
        
        int capacity;
        int itemCount;
        
        if (LevelConfig.IsBaseLevel)
        {
            // 基地：小容量，工具和材料
            capacity = 10;
            itemCount = 3;
        }
        else if (LevelConfig.IsRaidMap)
        {
            // 突袭地图：大容量，武器弹药
            capacity = 20;
            itemCount = Random.Range(5, 10);
        }
        else
        {
            // 其他场景
            capacity = 15;
            itemCount = 5;
        }
        
        inv.SetCapacity(capacity);
        
        // 添加物品
        for (int i = 0; i < itemCount; i++)
        {
            Item item = await CreateRandomItemForScene();
            if (item != null)
            {
                inv.AddItem(item);
            }
        }
        
        InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
            containerItem,
            position,
            Quaternion.identity
        );
        
        GameObject.Destroy(containerItem.gameObject);
    }
    
    private static async UniTask<Item> CreateRandomItemForScene()
    {
        ItemFilter filter = new ItemFilter();
        
        if (LevelConfig.IsRaidMap)
        {
            // 突袭地图优先生成武器和弹药
            filter.requireTags = new string[] { "Weapon", "Ammo" };
        }
        else
        {
            // 其他场景生成材料和工具
            filter.requireTags = new string[] { "Material", "Tool" };
        }
        
        int[] ids = ItemAssetsCollection.Search(filter);
        if (ids.Length == 0)
        {
            // 如果没找到，就搜索所有物品
            ids = ItemAssetsCollection.Search(new ItemFilter());
        }
        
        if (ids.Length > 0)
        {
            int randomId = ids[Random.Range(0, ids.Length)];
            return await ItemAssetsCollection.InstantiateAsync(randomId);
        }
        
        return null;
    }
}
```

## 获取场景中的对象

### 查找场景中的特定对象

```csharp
public class SceneObjectFinder
{
    // 找到场景中所有的箱子
    public static List<InteractableLootbox> FindAllLootboxes()
    {
        List<InteractableLootbox> lootboxes = new List<InteractableLootbox>();
        
        InteractableLootbox[] all = GameObject.FindObjectsOfType<InteractableLootbox>();
        lootboxes.AddRange(all);
        
        return lootboxes;
    }
    
    // 找到场景中所有的敌人
    public static List<CharacterMainControl> FindAllEnemies()
    {
        List<CharacterMainControl> enemies = new List<CharacterMainControl>();
        
        CharacterMainControl[] all = GameObject.FindObjectsOfType<CharacterMainControl>();
        foreach (CharacterMainControl character in all)
        {
            if (character.Team == Teams.Enemy)
            {
                enemies.Add(character);
            }
        }
        
        return enemies;
    }
    
    // 找到玩家
    public static CharacterMainControl FindPlayer()
    {
        return LevelManager.Instance?.MainCharacter;
    }
}
```

## 完整示例：场景感知的动态生成系统

```csharp
using UnityEngine;
using ItemStatsSystem;
using System.Collections.Generic;

namespace SceneAwareSpawner
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private List<GameObject> spawnedObjects = new List<GameObject>();
        private bool hasSpawnedInCurrentScene = false;
        
        void OnEnable()
        {
            SceneLoader.onAfterSceneInitialize += OnSceneInitialized;
            SceneLoader.onStartedLoadingScene += OnSceneUnloading;
        }
        
        void OnDisable()
        {
            SceneLoader.onAfterSceneInitialize -= OnSceneInitialized;
            SceneLoader.onStartedLoadingScene -= OnSceneUnloading;
            CleanupSpawnedObjects();
        }
        
        private void OnSceneInitialized(SceneLoadingContext context)
        {
            hasSpawnedInCurrentScene = false;
            
            string sceneID = SceneHelper.GetCurrentSceneID();
            Debug.Log($"场景 {sceneID} 已初始化");
            
            // 根据场景类型决定是否生成
            if (ShouldSpawnInCurrentScene())
            {
                StartCoroutine(DelayedSpawn());
            }
        }
        
        private void OnSceneUnloading(SceneLoadingContext context)
        {
            Debug.Log("场景即将切换，清理对象");
            CleanupSpawnedObjects();
        }
        
        private bool ShouldSpawnInCurrentScene()
        {
            // 在突袭地图生成
            if (LevelConfig.IsRaidMap)
            {
                return true;
            }
            
            // 在基地不生成
            if (LevelConfig.IsBaseLevel)
            {
                return false;
            }
            
            return false;
        }
        
        private IEnumerator DelayedSpawn()
        {
            // 等待场景完全加载
            yield return new WaitForSeconds(2f);
            
            if (hasSpawnedInCurrentScene)
                yield break;
                
            hasSpawnedInCurrentScene = true;
            
            // 生成多个箱子
            int spawnCount = Random.Range(3, 6);
            
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                SpawnLootbox(spawnPos);
                
                yield return new WaitForSeconds(0.5f);
            }
            
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player != null)
            {
                player.PopText($"已生成 {spawnCount} 个战利品箱！", 3f);
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null)
            {
                return Vector3.zero;
            }
            
            Vector3 playerPos = player.transform.position;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(15f, 40f);
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );
            
            return playerPos + offset;
        }
        
        private async void SpawnLootbox(Vector3 position)
        {
            Item containerItem = new GameObject("SceneLoot").AddComponent<Item>();
            Inventory inv = containerItem.gameObject.AddComponent<Inventory>();
            inv.SetCapacity(Random.Range(10, 20));
            
            // 添加随机物品
            int itemCount = Random.Range(4, 8);
            for (int i = 0; i < itemCount; i++)
            {
                Item randomItem = await CreateRandomItem();
                if (randomItem != null)
                {
                    inv.AddItem(randomItem);
                }
            }
            
            InteractableLootbox lootbox = InteractableLootbox.CreateFromItem(
                containerItem,
                position,
                Quaternion.identity
            );
            
            if (lootbox != null)
            {
                spawnedObjects.Add(lootbox.gameObject);
            }
            
            GameObject.Destroy(containerItem.gameObject);
        }
        
        private async UniTask<Item> CreateRandomItem()
        {
            ItemFilter filter = new ItemFilter();
            int[] allIds = ItemAssetsCollection.Search(filter);
            
            if (allIds.Length > 0)
            {
                int randomId = allIds[Random.Range(0, allIds.Length)];
                return await ItemAssetsCollection.InstantiateAsync(randomId);
            }
            
            return null;
        }
        
        private void CleanupSpawnedObjects()
        {
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedObjects.Clear();
            Debug.Log("已清理所有生成的对象");
        }
    }
}
```

## 重要注意事项

1. **场景切换清理** - 场景切换时，非持久化的对象会被销毁，务必监听场景切换事件并清理

2. **延迟生成** - 场景刚加载时某些系统可能还未完全初始化，建议延迟1-2秒后再生成对象

3. **检查 LevelManager** - 在访问 `LevelManager.Instance` 前先检查是否为 null

4. **场景ID** - 场景的具体ID需要从游戏数据中获取，不同版本可能不同

5. **多场景加载** - Unity 支持多场景加载，注意区分主场景和附加场景

6. **基地特殊性** - 基地场景通常不适合生成战利品箱，建议只在突袭地图生成

7. **性能考虑** - 不要在场景加载时立即生成大量对象，使用协程分帧生成

8. **保存兼容** - 动态生成的对象默认不会保存，需要自行实现保存系统

## 相关 API 参考

- [在地图上生成对象](spawn-objects-in-map.md)
- [容器物品使用指南](container-items.md)
- [游戏核心系统](../modules/core.md)

