using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 动画器文件批量创建对应动画文件的编辑器工具
/// </summary>
public class AnimationCreatorTool : EditorWindow
{
    [Header("路径设置")]
    private string animatorFolderPath = "Assets/Resources/动画器";
    private string animationFolderPath = "Assets/Resources/动画";
    
    [Header("创建选项")]
    private bool overwriteExisting = false;
    private bool createSubfolders = true;
    
    [Header("状态显示")]
    private string statusMessage = "";
    private Vector2 scrollPosition;

    [MenuItem("Tools/Animation Creator Tool")]
    public static void ShowWindow()
    {
        GetWindow<AnimationCreatorTool>("动画创建工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("动画器文件批量创建动画工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 路径设置
        GUILayout.Label("路径设置", EditorStyles.boldLabel);
        animatorFolderPath = EditorGUILayout.TextField("动画器文件夹路径:", animatorFolderPath);
        animationFolderPath = EditorGUILayout.TextField("动画文件夹路径:", animationFolderPath);
        
        GUILayout.Space(10);

        // 创建选项
        GUILayout.Label("创建选项", EditorStyles.boldLabel);
        overwriteExisting = EditorGUILayout.Toggle("覆盖已存在的文件", overwriteExisting);
        createSubfolders = EditorGUILayout.Toggle("保持子文件夹结构", createSubfolders);
        
        GUILayout.Space(10);

        // 信息提示
        EditorGUILayout.HelpBox("每个Animator Controller将生成两个动画文件：\n? Idle_[文件名].anim\n? Attack_[文件名].anim", MessageType.Info);
        
        GUILayout.Space(10);

        // 操作按钮
        if (GUILayout.Button("预览将要创建的文件", GUILayout.Height(30)))
        {
            PreviewFiles();
        }

        if (GUILayout.Button("开始创建动画文件", GUILayout.Height(30)))
        {
            CreateAnimationFiles();
        }

        if (GUILayout.Button("清空状态信息", GUILayout.Height(25)))
        {
            statusMessage = "";
        }

        GUILayout.Space(10);

        // 状态显示区域
        GUILayout.Label("状态信息", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        EditorGUILayout.TextArea(statusMessage, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 预览将要创建的文件
    /// </summary>
    private void PreviewFiles()
    {
        statusMessage = "=== 预览模式 ===\n";
        
        if (!Directory.Exists(animatorFolderPath))
        {
            statusMessage += $"错误: 动画器文件夹不存在: {animatorFolderPath}\n";
            return;
        }

        // 获取所有动画器文件
        string[] animatorFiles = Directory.GetFiles(animatorFolderPath, "*.controller", SearchOption.AllDirectories);
        
        statusMessage += $"找到 {animatorFiles.Length} 个动画器文件:\n";
        statusMessage += $"将创建 {animatorFiles.Length * 2} 个动画文件\n\n";

        foreach (string animatorFile in animatorFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(animatorFile);
            string relativePath = GetRelativePath(animatorFile, animatorFolderPath);
            
            // 获取两个动画文件的路径
            string idleTargetPath = GetTargetAnimationPath(relativePath, $"Idle_{fileName}");
            string attackTargetPath = GetTargetAnimationPath(relativePath, $"Attack_{fileName}");
            
            statusMessage += $"动画器: {fileName}\n";
            statusMessage += $"  源路径: {animatorFile}\n";
            statusMessage += $"  Idle动画: {idleTargetPath} {(File.Exists(idleTargetPath) ? "[已存在]" : "[将创建]")}\n";
            statusMessage += $"  Attack动画: {attackTargetPath} {(File.Exists(attackTargetPath) ? "[已存在]" : "[将创建]")}\n\n";
        }
    }

    /// <summary>
    /// 创建动画文件
    /// </summary>
    private void CreateAnimationFiles()
    {
        statusMessage = "=== 开始创建动画文件 ===\n";
        
        // 检查文件夹是否存在
        if (!Directory.Exists(animatorFolderPath))
        {
            statusMessage += $"错误: 动画器文件夹不存在: {animatorFolderPath}\n";
            return;
        }

        // 确保目标文件夹存在
        if (!Directory.Exists(animationFolderPath))
        {
            Directory.CreateDirectory(animationFolderPath);
            statusMessage += $"创建目标文件夹: {animationFolderPath}\n";
        }

        // 获取所有动画器文件
        string[] animatorFiles = Directory.GetFiles(animatorFolderPath, "*.controller", SearchOption.AllDirectories);
        
        statusMessage += $"找到 {animatorFiles.Length} 个动画器文件\n";
        statusMessage += $"将尝试创建 {animatorFiles.Length * 2} 个动画文件\n\n";

        int createdCount = 0;
        int skippedCount = 0;
        int errorCount = 0;

        foreach (string animatorFile in animatorFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(animatorFile);
            string relativePath = GetRelativePath(animatorFile, animatorFolderPath);
            
            statusMessage += $"处理动画器: {fileName}\n";
            
            // 创建 Idle 动画
            bool idleResult = CreateSingleAnimation(relativePath, $"Idle_{fileName}", "Idle", ref createdCount, ref skippedCount, ref errorCount);
            
            // 创建 Attack 动画
            bool attackResult = CreateSingleAnimation(relativePath, $"Attack_{fileName}", "Attack", ref createdCount, ref skippedCount, ref errorCount);
            
            if (idleResult && attackResult)
            {
                statusMessage += $"  ? {fileName} 的两个动画文件创建完成\n";
            }
            else if (idleResult || attackResult)
            {
                statusMessage += $"  ? {fileName} 部分动画文件创建成功\n";
            }
            else
            {
                statusMessage += $"  ? {fileName} 的动画文件创建失败\n";
            }
            
            statusMessage += "\n";
        }

        // 刷新资源数据库
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        statusMessage += $"=== 创建完成 ===\n";
        statusMessage += $"成功创建: {createdCount} 个文件\n";
        statusMessage += $"跳过文件: {skippedCount} 个文件\n";
        statusMessage += $"创建失败: {errorCount} 个文件\n";
    }

    /// <summary>
    /// 创建单个动画文件
    /// </summary>
    private bool CreateSingleAnimation(string relativePath, string animationName, string animationType, 
                                     ref int createdCount, ref int skippedCount, ref int errorCount)
    {
        try
        {
            string targetPath = GetTargetAnimationPath(relativePath, animationName);
            
            // 检查文件是否已存在
            if (File.Exists(targetPath) && !overwriteExisting)
            {
                statusMessage += $"    跳过 {animationName} - 文件已存在\n";
                skippedCount++;
                return false;
            }

            // 确保目标文件夹存在
            string targetDir = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
                statusMessage += $"    创建文件夹: {targetDir}\n";
            }

            // 创建动画文件
            AnimationClip newClip = CreateAnimationClip(animationName, animationType);
            
            // 保存动画文件
            AssetDatabase.CreateAsset(newClip, targetPath);
            
            statusMessage += $"    ? 创建成功: {animationName}\n";
            createdCount++;
            return true;
        }
        catch (System.Exception ex)
        {
            statusMessage += $"    ? 创建失败: {animationName} - {ex.Message}\n";
            errorCount++;
            return false;
        }
    }

    /// <summary>
    /// 创建动画剪辑
    /// </summary>
    private AnimationClip CreateAnimationClip(string clipName, string animationType)
    {
        AnimationClip clip = new AnimationClip();
        clip.name = clipName;
        clip.frameRate = 60;

        // 根据动画类型创建不同的默认动画
        switch (animationType.ToLower())
        {
            case "idle":
                CreateIdleAnimation(clip);
                break;
            case "attack":
                CreateAttackAnimation(clip);
                break;
            default:
                // 创建一个空的动画
                break;
        }

        return clip;
    }

    /// <summary>
    /// 创建 Idle 动画（轻微浮动效果）
    /// </summary>
    private void CreateIdleAnimation(AnimationClip clip)
    {
        // 创建一个轻微的上下浮动效果
        AnimationCurve posY = new AnimationCurve();
        posY.AddKey(0f, 0f);
        posY.AddKey(1f, 0.05f);  // 向上移动0.05单位
        posY.AddKey(2f, 0f);     // 回到原位
        
        // 设置平滑插值
        for (int i = 0; i < posY.keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(posY, i, AnimationUtility.TangentMode.ClampedAuto);
            AnimationUtility.SetKeyRightTangentMode(posY, i, AnimationUtility.TangentMode.ClampedAuto);
        }
        
        clip.SetCurve("", typeof(Transform), "m_LocalPosition.y", posY);
        clip.wrapMode = WrapMode.Loop;
    }

    /// <summary>
    /// 创建 Attack 动画（缩放和旋转效果）
    /// </summary>
    private void CreateAttackAnimation(AnimationClip clip)
    {
        // 缩放动画
        AnimationCurve scaleX = new AnimationCurve();
        AnimationCurve scaleY = new AnimationCurve();
        
        scaleX.AddKey(0f, 1f);
        scaleX.AddKey(0.1f, 1.2f);  // 快速放大
        scaleX.AddKey(0.5f, 1f);    // 恢复原大小
        
        scaleY.AddKey(0f, 1f);
        scaleY.AddKey(0.1f, 1.2f);
        scaleY.AddKey(0.5f, 1f);
        
        // 旋转动画
        AnimationCurve rotation = new AnimationCurve();
        rotation.AddKey(0f, 0f);
        rotation.AddKey(0.2f, 10f);   // 轻微旋转
        rotation.AddKey(0.5f, 0f);    // 回到原角度
        
        // 设置插值
        SetCurveInterpolation(scaleX);
        SetCurveInterpolation(scaleY);
        SetCurveInterpolation(rotation);
        
        clip.SetCurve("", typeof(Transform), "m_LocalScale.x", scaleX);
        clip.SetCurve("", typeof(Transform), "m_LocalScale.y", scaleY);
        clip.SetCurve("", typeof(Transform), "localEulerAngles.z", rotation);
        
        clip.wrapMode = WrapMode.Once;
    }

    /// <summary>
    /// 设置曲线插值模式
    /// </summary>
    private void SetCurveInterpolation(AnimationCurve curve)
    {
        for (int i = 0; i < curve.keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
        }
    }

    /// <summary>
    /// 获取相对路径
    /// </summary>
    private string GetRelativePath(string fullPath, string basePath)
    {
        if (!createSubfolders)
            return "";
            
        string relativePath = Path.GetRelativePath(basePath, Path.GetDirectoryName(fullPath));
        return relativePath == "." ? "" : relativePath;
    }

    /// <summary>
    /// 获取目标动画文件路径
    /// </summary>
    private string GetTargetAnimationPath(string relativePath, string fileName)
    {
        string targetDir = animationFolderPath;
        
        if (createSubfolders && !string.IsNullOrEmpty(relativePath))
        {
            targetDir = Path.Combine(animationFolderPath, relativePath);
        }
        
        return Path.Combine(targetDir, fileName + ".anim").Replace("\\", "/");
    }
}