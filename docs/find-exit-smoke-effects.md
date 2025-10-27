# 查找撤离点的烟雾特效

## 概述

在《逃离鸭科夫》中，撤离点通常有烟雾特效标记位置。本指南将介绍如何找到这些特效和位置，以及如何在自己的 Mod 中使用。

## 撤离点相关组件

游戏中撤离点主要涉及以下组件：

### 1. SimpleTeleporter
- 实际的传送/撤离交互对象
- 位置：撤离点的具体位置

### 2. FowSmoke
- 烟雾特效组件
- 包含粒子系统（ParticleSystem）
- 通常附加在撤离点对象上

### 3. ExitCreator
- 负责在地图上动态生成撤离点
- 由 LevelManager 管理

## 查找撤离点位置

### 方法1：查找所有撤离点对象

```csharp
using UnityEngine;
using System.Collections.Generic;

public class ExitFinder
{
    // 查找场景中所有的撤离点
    public static List<SimpleTeleporter> FindAllExits()
    {
        List<SimpleTeleporter> exits = new List<SimpleTeleporter>();
        
        // 查找所有 SimpleTeleporter 组件
        SimpleTeleporter[] allTeleporters = GameObject.FindObjectsOfType<SimpleTeleporter>();
        
        foreach (SimpleTeleporter teleporter in allTeleporters)
        {
            exits.Add(teleporter);
            Debug.Log($"找到撤离点: {teleporter.name} at {teleporter.transform.position}");
        }
        
        return exits;
    }
    
    // 获取撤离点的位置列表
    public static List<Vector3> GetExitPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        
        SimpleTeleporter[] exits = GameObject.FindObjectsOfType<SimpleTeleporter>();
        foreach (SimpleTeleporter exit in exits)
        {
            positions.Add(exit.transform.position);
        }
        
        return positions;
    }
    
    // 打印所有撤离点信息
    public static void LogAllExits()
    {
        SimpleTeleporter[] exits = GameObject.FindObjectsOfType<SimpleTeleporter>();
        
        Debug.Log($"共找到 {exits.Length} 个撤离点:");
        
        for (int i = 0; i < exits.Length; i++)
        {
            SimpleTeleporter exit = exits[i];
            Debug.Log($"[{i}] 位置: {exit.transform.position}");
            Debug.Log($"    传送目标: {(exit.target != null ? exit.target.position.ToString() : "无")}");
        }
    }
}

// 使用示例
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
    void Update()
    {
        // 按 F11 键查找所有撤离点
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ExitFinder.LogAllExits();
        }
    }
}
```

### 方法2：通过 LevelManager 获取

```csharp
public class ExitHelper
{
    // 通过 LevelManager 的 ExitCreator 访问
    public static ExitCreator GetExitCreator()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager 不存在");
            return null;
        }
        
        return LevelManager.Instance.ExitCreator;
    }
    
    // 检查是否有撤离点
    public static bool HasExits()
    {
        SimpleTeleporter[] exits = GameObject.FindObjectsOfType<SimpleTeleporter>();
        return exits.Length > 0;
    }
    
    // 获取最近的撤离点
    public static SimpleTeleporter GetNearestExit(Vector3 fromPosition)
    {
        SimpleTeleporter[] exits = GameObject.FindObjectsOfType<SimpleTeleporter>();
        
        if (exits.Length == 0)
            return null;
        
        SimpleTeleporter nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (SimpleTeleporter exit in exits)
        {
            float distance = Vector3.Distance(fromPosition, exit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = exit;
            }
        }
        
        return nearest;
    }
}
```

## 查找烟雾特效

### 方法1：通过撤离点找烟雾

```csharp
using UnityEngine;

public class SmokeFinder
{
    // 从撤离点获取烟雾特效
    public static FowSmoke GetSmokeFromExit(SimpleTeleporter exit)
    {
        if (exit == null)
            return null;
        
        // 在撤离点对象及其子对象中查找
        FowSmoke smoke = exit.GetComponentInChildren<FowSmoke>();
        
        return smoke;
    }
    
    // 获取所有烟雾特效
    public static List<FowSmoke> FindAllSmokes()
    {
        List<FowSmoke> smokes = new List<FowSmoke>();
        
        FowSmoke[] allSmokes = GameObject.FindObjectsOfType<FowSmoke>();
        smokes.AddRange(allSmokes);
        
        return smokes;
    }
    
    // 获取烟雾的粒子系统
    public static ParticleSystem[] GetSmokeParticles(FowSmoke smoke)
    {
        if (smoke == null)
            return null;
        
        return smoke.particles;
    }
}
```

### 方法2：直接查找所有烟雾

```csharp
public class SmokeEffectExplorer
{
    // 查找并显示所有烟雾特效信息
    public static void LogAllSmokes()
    {
        FowSmoke[] smokes = GameObject.FindObjectsOfType<FowSmoke>();
        
        Debug.Log($"共找到 {smokes.Length} 个烟雾特效:");
        
        for (int i = 0; i < smokes.Length; i++)
        {
            FowSmoke smoke = smokes[i];
            
            Debug.Log($"[{i}] 烟雾位置: {smoke.transform.position}");
            
            if (smoke.particles != null && smoke.particles.Length > 0)
            {
                Debug.Log($"    粒子系统数量: {smoke.particles.Length}");
                
                foreach (ParticleSystem ps in smoke.particles)
                {
                    if (ps != null)
                    {
                        Debug.Log($"    - 粒子: {ps.name}, 发射: {ps.isEmitting}");
                    }
                }
            }
        }
    }
    
    // 获取烟雾的详细信息
    public static void LogSmokeDetails(FowSmoke smoke)
    {
        if (smoke == null)
        {
            Debug.Log("烟雾对象为空");
            return;
        }
        
        Debug.Log($"烟雾特效信息:");
        Debug.Log($"  位置: {smoke.transform.position}");
        Debug.Log($"  开始时间: {smoke.startTime}");
        Debug.Log($"  生命周期: {smoke.lifeTime}");
        Debug.Log($"  粒子淡出时间: {smoke.particleFadeTime}");
        
        if (smoke.particles != null)
        {
            Debug.Log($"  粒子系统数量: {smoke.particles.Length}");
        }
        
        if (smoke.colParent != null)
        {
            Debug.Log($"  碰撞父物体: {smoke.colParent.name}");
        }
    }
}
```

## 复制烟雾特效到其他位置

### 复制整个撤离点（包括烟雾）

```csharp
using UnityEngine;

public class ExitCloner
{
    // 复制一个撤离点到新位置
    public static SimpleTeleporter CloneExit(SimpleTeleporter original, Vector3 newPosition)
    {
        if (original == null)
        {
            Debug.LogError("原始撤离点为空");
            return null;
        }
        
        // 实例化撤离点
        GameObject cloned = GameObject.Instantiate(original.gameObject);
        cloned.transform.position = newPosition;
        cloned.transform.rotation = original.transform.rotation;
        
        SimpleTeleporter clonedExit = cloned.GetComponent<SimpleTeleporter>();
        
        Debug.Log($"已复制撤离点到: {newPosition}");
        
        return clonedExit;
    }
    
    // 只复制烟雾特效
    public static FowSmoke CloneSmokeEffect(FowSmoke original, Vector3 newPosition)
    {
        if (original == null)
        {
            Debug.LogError("原始烟雾为空");
            return null;
        }
        
        GameObject cloned = GameObject.Instantiate(original.gameObject);
        cloned.transform.position = newPosition;
        cloned.transform.rotation = original.transform.rotation;
        
        FowSmoke clonedSmoke = cloned.GetComponent<FowSmoke>();
        
        Debug.Log($"已复制烟雾特效到: {newPosition}");
        
        return clonedSmoke;
    }
}
```

### 只复制烟雾特效（不含交互）

```csharp
public class SmokeOnlyCloner
{
    // 在指定位置创建烟雾特效（仅视觉）
    public static GameObject CreateSmokeAtPosition(Vector3 position)
    {
        // 查找现有的烟雾作为模板
        FowSmoke templateSmoke = GameObject.FindObjectOfType<FowSmoke>();
        
        if (templateSmoke == null)
        {
            Debug.LogError("找不到烟雾特效模板");
            return null;
        }
        
        // 复制烟雾
        GameObject smokeObject = GameObject.Instantiate(templateSmoke.gameObject);
        smokeObject.transform.position = position;
        smokeObject.name = "CustomSmoke";
        
        // 移除不需要的组件（如果只要视觉效果）
        // 保留 FowSmoke 和 ParticleSystem
        
        return smokeObject;
    }
    
    // 创建简化版烟雾（只有粒子系统）
    public static ParticleSystem CreateSimpleSmoke(Vector3 position)
    {
        // 查找模板
        FowSmoke templateSmoke = GameObject.FindObjectOfType<FowSmoke>();
        
        if (templateSmoke == null || templateSmoke.particles == null || templateSmoke.particles.Length == 0)
        {
            Debug.LogError("找不到烟雾粒子系统");
            return null;
        }
        
        // 复制第一个粒子系统
        ParticleSystem templatePS = templateSmoke.particles[0];
        ParticleSystem newPS = GameObject.Instantiate(templatePS);
        newPS.transform.position = position;
        newPS.transform.rotation = templatePS.transform.rotation;
        newPS.name = "CustomSmokeParticle";
        
        // 开始播放
        if (!newPS.isPlaying)
        {
            newPS.Play();
        }
        
        return newPS;
    }
}
```

## 完整示例：撤离点和烟雾查看器

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace ExitSmokeViewer
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private List<SimpleTeleporter> exits = new List<SimpleTeleporter>();
        private List<FowSmoke> smokes = new List<FowSmoke>();
        private bool showUI = false;
        private Vector2 scrollPosition;
        
        void Update()
        {
            // 按 F11 刷新查找
            if (Input.GetKeyDown(KeyCode.F11))
            {
                RefreshExitsAndSmokes();
            }
            
            // 按 F12 切换UI
            if (Input.GetKeyDown(KeyCode.F12))
            {
                showUI = !showUI;
            }
        }
        
        void OnGUI()
        {
            if (!showUI) return;
            
            GUILayout.BeginArea(new Rect(20, 20, 500, 600));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("撤离点和烟雾查看器 (F11刷新 / F12关闭)", GUILayout.Height(30));
            
            if (GUILayout.Button("刷新", GUILayout.Height(30)))
            {
                RefreshExitsAndSmokes();
            }
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
            
            // 显示撤离点
            GUILayout.Label($"=== 撤离点 ({exits.Count}) ===", GUILayout.Height(25));
            
            for (int i = 0; i < exits.Count; i++)
            {
                SimpleTeleporter exit = exits[i];
                if (exit == null) continue;
                
                GUILayout.BeginHorizontal("box");
                GUILayout.Label($"[{i}] {exit.transform.position}", GUILayout.Width(300));
                
                if (GUILayout.Button("传送到", GUILayout.Width(80)))
                {
                    TeleportPlayerTo(exit.transform.position);
                }
                
                if (GUILayout.Button("复制这里", GUILayout.Width(80)))
                {
                    CloneExitHere(exit);
                }
                
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10);
            
            // 显示烟雾
            GUILayout.Label($"=== 烟雾特效 ({smokes.Count}) ===", GUILayout.Height(25));
            
            for (int i = 0; i < smokes.Count; i++)
            {
                FowSmoke smoke = smokes[i];
                if (smoke == null) continue;
                
                GUILayout.BeginHorizontal("box");
                GUILayout.Label($"[{i}] {smoke.transform.position}", GUILayout.Width(300));
                
                if (GUILayout.Button("传送到", GUILayout.Width(80)))
                {
                    TeleportPlayerTo(smoke.transform.position);
                }
                
                if (GUILayout.Button("复制烟雾", GUILayout.Width(80)))
                {
                    CloneSmokeHere(smoke);
                }
                
                GUILayout.EndHorizontal();
                
                if (smoke.particles != null && smoke.particles.Length > 0)
                {
                    GUILayout.Label($"  粒子系统: {smoke.particles.Length} 个", GUILayout.Height(20));
                }
            }
            
            GUILayout.EndScrollView();
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void RefreshExitsAndSmokes()
        {
            exits.Clear();
            smokes.Clear();
            
            // 查找撤离点
            SimpleTeleporter[] foundExits = GameObject.FindObjectsOfType<SimpleTeleporter>();
            exits.AddRange(foundExits);
            
            // 查找烟雾
            FowSmoke[] foundSmokes = GameObject.FindObjectsOfType<FowSmoke>();
            smokes.AddRange(foundSmokes);
            
            Debug.Log($"找到 {exits.Count} 个撤离点, {smokes.Count} 个烟雾特效");
            
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player != null)
            {
                player.PopText($"找到 {exits.Count} 个撤离点", 2f);
            }
        }
        
        private void TeleportPlayerTo(Vector3 position)
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player != null)
            {
                player.transform.position = position + Vector3.up * 2f;
                player.PopText($"已传送到: {position}", 2f);
            }
        }
        
        private void CloneExitHere(SimpleTeleporter original)
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null) return;
            
            Vector3 spawnPos = player.transform.position + player.transform.forward * 3f;
            
            SimpleTeleporter cloned = ExitCloner.CloneExit(original, spawnPos);
            if (cloned != null)
            {
                player.PopText("已复制撤离点", 2f);
            }
        }
        
        private void CloneSmokeHere(FowSmoke original)
        {
            CharacterMainControl player = LevelManager.Instance?.MainCharacter;
            if (player == null) return;
            
            Vector3 spawnPos = player.transform.position + player.transform.forward * 3f;
            
            FowSmoke cloned = ExitCloner.CloneSmokeEffect(original, spawnPos);
            if (cloned != null)
            {
                player.PopText("已复制烟雾特效", 2f);
            }
        }
    }
}
```

## 自定义烟雾特效

### 创建自己的烟雾效果

```csharp
public class CustomSmokeCreator
{
    // 创建基于游戏内烟雾的自定义版本
    public static GameObject CreateCustomSmoke(Vector3 position, Color smokeColor)
    {
        // 获取模板
        FowSmoke template = GameObject.FindObjectOfType<FowSmoke>();
        if (template == null)
        {
            Debug.LogError("找不到烟雾模板");
            return null;
        }
        
        // 复制
        GameObject customSmoke = GameObject.Instantiate(template.gameObject);
        customSmoke.transform.position = position;
        customSmoke.name = "CustomColoredSmoke";
        
        // 修改粒子颜色
        FowSmoke smokeComponent = customSmoke.GetComponent<FowSmoke>();
        if (smokeComponent != null && smokeComponent.particles != null)
        {
            foreach (ParticleSystem ps in smokeComponent.particles)
            {
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = smokeColor;
                }
            }
        }
        
        return customSmoke;
    }
    
    // 创建临时烟雾（几秒后自动销毁）
    public static void CreateTemporarySmoke(Vector3 position, float duration = 10f)
    {
        GameObject smoke = CreateCustomSmoke(position, Color.white);
        if (smoke != null)
        {
            GameObject.Destroy(smoke, duration);
        }
    }
}
```

## 实用工具：标记位置用烟雾

```csharp
public class SmokeMarker
{
    private static List<GameObject> markers = new List<GameObject>();
    
    // 在位置放置烟雾标记
    public static void MarkPosition(Vector3 position, string label = "")
    {
        GameObject smoke = SmokeOnlyCloner.CreateSmokeAtPosition(position);
        if (smoke != null)
        {
            smoke.name = string.IsNullOrEmpty(label) ? "Marker" : $"Marker_{label}";
            markers.Add(smoke);
            
            Debug.Log($"已在 {position} 放置标记: {label}");
        }
    }
    
    // 清除所有标记
    public static void ClearAllMarkers()
    {
        foreach (GameObject marker in markers)
        {
            if (marker != null)
            {
                GameObject.Destroy(marker);
            }
        }
        markers.Clear();
        Debug.Log("已清除所有烟雾标记");
    }
    
    // 标记玩家当前位置
    public static void MarkPlayerPosition()
    {
        CharacterMainControl player = LevelManager.Instance?.MainCharacter;
        if (player != null)
        {
            MarkPosition(player.transform.position, "PlayerPos");
            player.PopText("已标记当前位置", 2f);
        }
    }
}
```

## 注意事项

1. **撤离点数量** - 突袭地图会动态生成撤离点，数量由 `LevelConfig.MinExitCount` 和 `MaxExitCount` 控制

2. **场景切换** - 撤离点在场景切换后会重新生成，需要重新查找

3. **粒子系统** - 烟雾特效主要是 ParticleSystem，复制时确保粒子系统正常播放

4. **性能考虑** - 不要创建太多烟雾特效，会影响性能

5. **基地场景** - 基地通常没有撤离点，只在突袭地图查找

6. **时间控制** - FowSmoke 有生命周期（`lifeTime`），可能会自动消失

7. **碰撞体** - 某些烟雾有碰撞体（`colParent`），复制时注意

8. **等待初始化** - 撤离点在关卡初始化后才会生成，建议监听 `LevelManager.OnLevelInitialized`

## 相关 API 参考

- [在地图上生成对象](spawn-objects-in-map.md)
- [场景和地图管理](scene-management.md)
- [游戏核心系统](../modules/core.md)

