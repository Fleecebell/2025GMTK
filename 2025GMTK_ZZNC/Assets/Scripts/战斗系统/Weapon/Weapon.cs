using UnityEngine;
using System.Collections;
using InventorySystem.Items;
using InventorySystem.Data;
using System.Collections.Generic;

/// <summary>
/// 武器脚本 - 挂载在武器预制体上
/// 负责执行武器的功能：攻击、朝向、特效等
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("武器组件")]
    [SerializeField] private Transform firePoint;          // 攻击发射点
    [SerializeField] private SpriteRenderer weaponSprite;  // 武器精灵渲染器
    [SerializeField] private AudioSource audioSource;      // 音频源
    [SerializeField] private Animator weaponAnimator;      // 武器动画器

    [SerializeField] private GameObject bulletPrefab;//子弹

    [Header("武器设置")]
    [SerializeField] private LayerMask targetLayerMask = -1;    // 目标图层
    [SerializeField] private float rotationSpeed = 10f;        // 旋转速度
    [SerializeField] private bool flipSpriteWhenFacingLeft = true; // 面向左时翻转精灵
    [SerializeField] private Transform attackPosition;        // 旋转中心
    [SerializeField] private Transform weapon;          // 武器

    [Header("调试设置")]
    [SerializeField] private bool enableDebugLogs = true;      // 启用调试日志
    [SerializeField] private bool showAttackRange = true;      // 显示攻击范围

    // 武器数据
    private WeaponData weaponData;
    private float damage;
    private float attackSpeed;
    private AttackType attackType;

    // 攻击状态
    [SerializeField] private bool canAttack = true;
    private float lastAttackTime;
    private bool isAttacking = false;

    // 组件引用
    private Camera currentCamera;
    private Transform playerTransform;

    // 事件
    public System.Action<float> OnAttack;           // 攻击事件（传递伤害值）
    public System.Action<Vector2> OnAttackDirection; // 攻击方向事件

    // 属性访问器
    public WeaponData WeaponData => weaponData;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
    public AttackType AttackType => attackType;
    public bool IsAttacking => isAttacking;
    public bool CanAttack => canAttack && Time.time >= lastAttackTime + (1f / attackSpeed);
    [Header("方向翻转")]
    private bool isFlipped = false;   // true = 翻转 180°

    [Header("挥砍攻击设置")]
    [SerializeField] private float slashRotationAngle = 90f;        // 挥砍旋转角度
    [SerializeField] private float slashScaleMultiplier = 1.5f;     // 挥砍放大倍数
    [SerializeField] private float slashDuration = 0.1f;           // 挥砍动画持续时间
    [SerializeField] private AnimationCurve slashSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 旋转速度曲线

    private bool isPerformingSlash = false; // 防止重复执行挥砍动画
    [Header("刺击攻击设置")]
    [SerializeField] private float thrustDistance = 1.5f;          // 突刺前进距离
    [SerializeField] private float thrustScaleMultiplier = 2.0f;   // 突刺过程中的最大缩放倍数
    [SerializeField] private float thrustDuration = 0.2f;          // 突刺动画持续时间
    [SerializeField] private AnimationCurve thrustSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 突刺速度曲线

    private bool isPerformingThrust = false; // 防止重复执行突刺动画

    // 记录初始状态
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    [Header("触发器伤害")]
    [SerializeField] private Collider2D attackCollider; // 拖武器上的触发器
    private List<GameObject> hitThisSwing = new List<GameObject>(); // 防止重复伤害


    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[Weapon-{gameObject.name}] {message}");
        }
    }

    private void DebugLogWarning(string message)
    {
        if (enableDebugLogs)
        {
            Debug.LogWarning($"[Weapon-{gameObject.name}] {message}");
        }
    }

    /// <summary>
    /// 初始化武器
    /// </summary>
    public void Initialize(WeaponData data, Transform player = null)
    {
        DebugLog("开始初始化武器...");

        if (data == null)
        {
            Debug.LogError("武器数据不能为空");
            return;
        }

        weaponData = data;
        damage = data.Damage;
        attackSpeed = data.AttackSpeed;
        attackType = data.AttackType;
        playerTransform = player;

        DebugLog($"武器数据设置完成 - 名称:{data.ItemName}, 伤害:{damage}, 攻速:{attackSpeed}, 类型:{attackType}");

        // 初始化组件
        InitializeComponents();

        // 设置武器外观
        SetupWeaponAppearance();

        DebugLog($"武器初始化完成: {data.ItemName}");

        originalPosition = weapon.position;
        originalScale = weapon.localScale;
        originalRotation = weapon.rotation;
    }

    private void InitializeComponents()
    {
        DebugLog("初始化组件中...");

        // 获取当前激活的相机
        FindActiveCamera();

        // 自动获取组件
        if (weaponSprite == null)
        {
            weaponSprite = GetComponent<SpriteRenderer>();
            DebugLog($"自动获取SpriteRenderer: {weaponSprite != null}");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            DebugLog($"自动获取AudioSource: {audioSource != null}");
        }

        if (weaponAnimator == null)
        {
            weaponAnimator = GetComponent<Animator>();
            DebugLog($"自动获取Animator: {weaponAnimator != null}");
        }

        // 如果没有设置发射点，创建一个
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = Vector3.right;
            firePoint = firePointObj.transform;
            DebugLog("自动创建FirePoint");
        }

        // 设置音频源
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        DebugLog("组件初始化完成");

    }

    private void FindActiveCamera()
    {
        DebugLog("查找激活的相机...");

        // 方法1：查找所有相机中激活的那个
        Camera[] allCameras = FindObjectsOfType<Camera>();
        DebugLog($"找到 {allCameras.Length} 个相机");

        foreach (Camera cam in allCameras)
        {
            if (cam.gameObject.activeInHierarchy && cam.enabled)
            {
                currentCamera = cam;
                DebugLog($"使用相机: {cam.name}");
                break;
            }
        }

        // 如果没找到激活的相机，尝试使用主相机作为备选
        if (currentCamera == null)
        {
            currentCamera = Camera.main;
            if (currentCamera != null)
                DebugLog($"使用主相机: {currentCamera.name}");
        }

        // 如果还是没有，尝试查找带有特定标签的相机
        if (currentCamera == null)
        {
            GameObject cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj != null)
            {
                currentCamera = cameraObj.GetComponent<Camera>();
                if (currentCamera != null)
                    DebugLog($"使用MainCamera标签的相机: {currentCamera.name}");
            }
        }

        if (currentCamera == null)
        {
            DebugLogWarning("未找到可用的相机，武器朝向功能可能无法正常工作");
        }
    }

    public void SetCamera(Camera camera)
    {
        currentCamera = camera;
        DebugLog($"手动设置相机: {camera?.name ?? "null"}");
    }

    public void RefreshCamera()
    {
        DebugLog("刷新相机引用");
        FindActiveCamera();
    }

    private void SetupWeaponAppearance()
    {
        if (weaponData?.Icon != null && weaponSprite != null)
        {
            weaponSprite.sprite = weaponData.Icon;
            DebugLog("武器外观设置完成");
        }
        else
        {
            DebugLogWarning($"武器外观设置失败 - Icon:{weaponData?.Icon != null}, Sprite:{weaponSprite != null}");
        }
    }
    private void Awake()
    {
        if (attackCollider == null)
            attackCollider = GetComponentInChildren<Collider2D>();

        if (attackCollider != null)
        {
            var rb = attackCollider.GetComponent<Rigidbody2D>();
            if (rb == null) rb = attackCollider.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void Update()
    {
        // 如果相机无效，尝试重新查找
        if (currentCamera == null || !currentCamera.gameObject.activeInHierarchy || !currentCamera.enabled)
        {
            FindActiveCamera();
        }

        UpdateWeaponRotation();
        HandleInput();
    }

    private void UpdateWeaponRotation()
    {
        if (currentCamera == null) return;

        Vector3 mouseWorldPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 翻转标记
        if (isFlipped) angle += 180f;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        HandleSpriteFlip(direction.x < 0);
    }


    private void HandleSpriteFlip(bool facingLeft)
    {
        if (weaponSprite != null && flipSpriteWhenFacingLeft)
        {
            weaponSprite.flipY = facingLeft;
        }
    }

    private void HandleInput()
    {
        // 检测攻击输入（鼠标左键或空格键）
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            DebugLog("检测到攻击输入");
            TryAttack();
        }
        // 2. 新增：监听 A / D 键翻转武器
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.localScale = new Vector3(-1.2f, transform.localScale.y, transform.localScale.z);
            isFlipped = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.localScale = new Vector3(1.2f, transform.localScale.y, transform.localScale.z);
            isFlipped = false;
        }
    }

    public void TryAttack()
    {
        DebugLog($"尝试攻击 - CanAttack: {CanAttack}");
        DebugLog($"攻击状态检查 - canAttack: {canAttack}, Time.time: {Time.time}, lastAttackTime: {lastAttackTime}, 冷却时间: {1f / attackSpeed}");
        DebugLog($"攻击冷却剩余: {(lastAttackTime + (1f / attackSpeed)) - Time.time}");

        if (!CanAttack)
        {
            Debug.Log($"攻击冷却剩余: {(lastAttackTime + (1f / attackSpeed)) - Time.time}");
            DebugLogWarning("无法攻击 - 冷却中或被禁用");
            return;
        }

        DebugLog("开始执行攻击");
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;  // 只设置时间，不修改canAttack

        // 获取攻击方向
        Vector2 attackDirection = GetAttackDirection();

        // 播放攻击动画等...
        PlayAttackAnimation();
        PlayAttackSound();
        SpawnAttackEffect();
        ExecuteAttackLogic(attackDirection);

        // 触发攻击事件
        OnAttack?.Invoke(damage);
        OnAttackDirection?.Invoke(attackDirection);

        // 等待攻击动画完成
        yield return new WaitForSeconds(0.1f);

        isAttacking = false;
        // 移除额外的冷却等待和canAttack设置
    }

    private Vector2 GetAttackDirection()
    {
        if (currentCamera == null)
        {
            DebugLogWarning("相机为空，使用默认攻击方向");
            return Vector2.right;
        }

        Vector3 mouseWorldPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        DebugLog($"鼠标世界坐标: {mouseWorldPos}, 武器位置: {transform.position}, 攻击方向: {direction}");

        return direction;
    }

    private void PlayAttackAnimation()
    {
        DebugLog("播放攻击动画");

        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("Attack");
            DebugLog("触发Attack动画");
        }
        else
        {
            DebugLog("没有动画器，播放简单缩放动画");
            StartCoroutine(SimpleAttackAnimation());
        }
    }

    private IEnumerator SimpleAttackAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // 放大
        float elapsed = 0f;
        float duration = 0.05f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // 缩小
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        transform.localScale = originalScale;
        DebugLog("简单攻击动画完成");
    }

    private void PlayAttackSound()
    {
        if (audioSource != null && weaponData?.AttackSound != null)
        {
            audioSource.PlayOneShot(weaponData.AttackSound);
            DebugLog("播放攻击音效");
        }
        else
        {
            DebugLog($"无法播放音效 - AudioSource: {audioSource != null}, AttackSound: {weaponData?.AttackSound != null}");
        }
    }

    private void SpawnAttackEffect()
    {
        if (weaponData?.AttackEffect != null && firePoint != null)
        {
            GameObject effect = Instantiate(weaponData.AttackEffect, firePoint.position, firePoint.rotation);
            Destroy(effect, 2f);
            DebugLog($"生成攻击特效: {weaponData.AttackEffect.name}");
        }
        else
        {
            DebugLog($"无法生成特效 - AttackEffect: {weaponData?.AttackEffect != null}, FirePoint: {firePoint != null}");
        }
    }

    private void ExecuteAttackLogic(Vector2 direction)
    {
        DebugLog($"执行攻击逻辑 - 攻击类型: {attackType}");

        switch (attackType)
        {
            case AttackType.Slash:
                PerformSlashAttack(direction);
                break;
            case AttackType.Thrust:
                PerformThrustAttack(direction);
                break;
            case AttackType.Firearm:
                PerformFirearmAttack(direction);
                break;
            default:
                DebugLogWarning($"未知的攻击类型: {attackType}");
                break;
        }
    }

    private void PerformSlashAttack(Vector2 direction)
    {
        DebugLog("执行挥砍攻击");
        // 添加挥砍动画效果
        if (weapon != null && attackPosition != null && !isPerformingSlash)
        {
            StartCoroutine(PerformAdvancedSlashAnimation(direction));
        }
    }
    /// <summary>
    /// 执行挥砍动画 - weapon绕attackPosition旋转并放大
    /// </summary>
    private IEnumerator PerformSlashAnimation(Vector2 attackDirection)
    {
        if (isPerformingSlash) yield break;

        isPerformingSlash = true;
        DebugLog("开始挥砍动画");

        // 记录初始状态
        Vector3 originalPosition = weapon.position;
        Vector3 originalScale = weapon.localScale;
        Quaternion originalRotation = weapon.rotation;

        // 计算旋转参数
        Vector3 pivotPoint = attackPosition.position;

        // 确定旋转方向（根据攻击方向）
        float rotationDirection = attackDirection.x >= 0 ? 1f : -1f; // 右侧攻击顺时针，左侧逆时针
        float totalRotationAngle = slashRotationAngle * rotationDirection;

        // 计算weapon相对于旋转中心的初始角度和距离
        Vector3 initialOffset = originalPosition - pivotPoint;
        float initialAngle = Mathf.Atan2(initialOffset.y, initialOffset.x) * Mathf.Rad2Deg;
        float rotationRadius = initialOffset.magnitude;

        DebugLog($"挥砍参数 - 旋转中心: {pivotPoint}, 旋转角度: {totalRotationAngle}, 半径: {rotationRadius}");
        DebugLog($"初始角度: {initialAngle}, 攻击方向: {attackDirection}, 旋转方向: {rotationDirection}");

        float elapsedTime = 0f;

        while (elapsedTime < slashDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / slashDuration;

            // 使用曲线控制旋转速度
            float curveValue = slashSpeedCurve.Evaluate(normalizedTime);

            // 计算当前旋转角度
            float currentRotationAngle = totalRotationAngle * curveValue;
            float currentAngle = initialAngle + currentRotationAngle;

            // 计算新位置（绕点旋转）
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 newOffset = new Vector3(
                Mathf.Cos(radians) * rotationRadius,
                Mathf.Sin(radians) * rotationRadius,
                0f
            );
            weapon.position = pivotPoint + newOffset;

            // 计算缩放（使用曲线让放大效果更自然）
            float scaleProgress = slashSpeedCurve.Evaluate(normalizedTime);
            float currentScale = Mathf.Lerp(1f, slashScaleMultiplier, scaleProgress);

            // 在动画后半段开始缩小
            if (normalizedTime > 0.6f)
            {
                float shrinkProgress = (normalizedTime - 0.6f) / 0.4f;
                currentScale = Mathf.Lerp(slashScaleMultiplier, 1f, shrinkProgress);
            }

            weapon.localScale = originalScale * currentScale;

            // 让weapon朝向旋转方向（可选）
            float weaponRotation = currentAngle + (rotationDirection > 0 ? 0f : 180f);
            weapon.rotation = Quaternion.AngleAxis(weaponRotation, Vector3.forward);

            yield return null;
        }

        // 恢复到初始状态
        weapon.position = originalPosition;
        weapon.localScale = originalScale;
        weapon.rotation = originalRotation;

        isPerformingSlash = false;
        DebugLog("挥砍动画完成");
    }

    /// <summary>
    /// 高级版本：支持更复杂的挥砍动画
    /// </summary>
    private IEnumerator PerformAdvancedSlashAnimation(Vector2 attackDirection)
    {
        if (isPerformingSlash) yield break;

        isPerformingSlash = true;
        DebugLog("开始高级挥砍动画");

        // 记录初始状态
        Vector3 originalPosition = weapon.position;
        Vector3 originalScale = weapon.localScale;
        Quaternion originalRotation = weapon.rotation;

        Vector3 pivotPoint = attackPosition.position;

        // 根据攻击方向计算挥砍轨迹
        float attackAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        // 挥砍分为三个阶段：蓄力、挥砍、收招
        float prepareTime = slashDuration * 0.2f;    // 蓄力时间
        float executeTime = slashDuration * 0.6f;    // 执行时间
        float recoverTime = slashDuration * 0.2f;    // 收招时间

        float elapsedTime = 0f;

        // 第一阶段：蓄力（向后拉）
        DebugLog("蓄力阶段");
        float prepareRotation = -20f; // 向后拉20度

        while (elapsedTime < prepareTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / prepareTime;

            // 蓄力动画（轻微后拉和缩小）
            float currentRotation = Mathf.Lerp(0f, prepareRotation, progress);
            float currentScale = Mathf.Lerp(1f, 0.9f, progress);

            // 应用变换
            ApplySlashTransform(pivotPoint, originalPosition, currentRotation, originalScale * currentScale, attackAngle);

            yield return null;
        }

        // 第二阶段：执行挥砍
        DebugLog("执行阶段");
        float executeStartTime = elapsedTime;

        while (elapsedTime < executeStartTime + executeTime)
        {
            float phaseTime = elapsedTime - executeStartTime;
            float progress = phaseTime / executeTime;

            // 使用曲线控制挥砍速度
            float curveValue = slashSpeedCurve.Evaluate(progress);

            // 从蓄力位置快速挥砍到目标位置
            float currentRotation = Mathf.Lerp(prepareRotation, slashRotationAngle, curveValue);
            float currentScale = Mathf.Lerp(0.9f, slashScaleMultiplier, curveValue);

            // 应用变换
            ApplySlashTransform(pivotPoint, originalPosition, currentRotation, originalScale * currentScale, attackAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 第三阶段：收招
        DebugLog("收招阶段");
        float recoverStartTime = elapsedTime;

        while (elapsedTime < recoverStartTime + recoverTime)
        {
            float phaseTime = elapsedTime - recoverStartTime;
            float progress = phaseTime / recoverTime;

            // 缓慢恢复到初始状态
            float currentRotation = Mathf.Lerp(slashRotationAngle, 0f, progress);
            float currentScale = Mathf.Lerp(slashScaleMultiplier, 1f, progress);

            // 应用变换
            ApplySlashTransform(pivotPoint, originalPosition, currentRotation, originalScale * currentScale, attackAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 恢复到初始状态
        weapon.position = originalPosition;
        weapon.localScale = originalScale;
        weapon.rotation = originalRotation;

        isPerformingSlash = false;
        DebugLog("高级挥砍动画完成");
    }


    /// <summary>
    /// 应用挥砍变换
    /// </summary>
    private void ApplySlashTransform(Vector3 pivotPoint, Vector3 originalPosition, float rotationAngle, Vector3 scale, float baseAngle)
    {
        // 计算绕点旋转后的位置
        Vector3 offset = originalPosition - pivotPoint;
        float currentAngle = (baseAngle + rotationAngle) * Mathf.Deg2Rad;
        float radius = offset.magnitude;

        Vector3 newOffset = new Vector3(
            Mathf.Cos(currentAngle) * radius,
            Mathf.Sin(currentAngle) * radius,
            0f
        );

        weapon.position = pivotPoint + newOffset;
        weapon.localScale = scale;
        weapon.rotation = Quaternion.AngleAxis(baseAngle + rotationAngle, Vector3.forward);
    }



    private void PerformThrustAttack(Vector2 direction)
    {
        DebugLog("执行刺击攻击");
        // 添加突刺动画效果
        if (weapon != null && !isPerformingThrust)
        {
            StartCoroutine(PerformAdvancedThrustAnimation(direction));
        }
    }
    /// <summary>
    /// 执行突刺动画 - 武器向前突刺并放大
    /// </summary>
    private IEnumerator PerformThrustAnimation(Vector2 thrustDirection)
    {
        if (isPerformingThrust) yield break;

        isPerformingThrust = true;
        DebugLog("开始突刺动画");

        // 记录初始状态
        Vector3 originalPosition = weapon.position;
        Vector3 originalScale = weapon.localScale;

        // 计算目标位置（向前突刺）
        Vector3 thrustTargetPosition = originalPosition + (Vector3)thrustDirection * thrustDistance;

        DebugLog($"突刺参数 - 初始位置: {originalPosition}, 目标位置: {thrustTargetPosition}, 突刺距离: {thrustDistance}");
        DebugLog($"缩放参数 - 初始缩放: {originalScale}, 最大缩放倍数: {thrustScaleMultiplier}");

        float elapsedTime = 0f;

        // 突刺分为两个阶段：向前突刺（同时放大）、回到原位（同时缩小）
        float thrustOutTime = thrustDuration * 0.6f;  // 向前突刺时间（占总时间的60%）
        float thrustBackTime = thrustDuration * 0.4f; // 回到原位时间（占总时间的40%）

        // 第一阶段：向前突刺并放大
        DebugLog("突刺阶段：向前突刺并放大");
        float thrustOutStartTime = elapsedTime;

        while (elapsedTime < thrustOutStartTime + thrustOutTime)
        {
            float phaseTime = elapsedTime - thrustOutStartTime;
            float progress = phaseTime / thrustOutTime;

            // 使用曲线控制突刺速度
            float curveValue = thrustSpeedCurve.Evaluate(progress);

            // 计算当前位置（向前突刺）
            Vector3 currentPosition = Vector3.Lerp(originalPosition, thrustTargetPosition, curveValue);

            // 计算当前缩放（逐渐放大）
            float currentScaleMultiplier = Mathf.Lerp(1f, thrustScaleMultiplier, curveValue);
            Vector3 currentScale = originalScale * currentScaleMultiplier;

            // 应用变换
            weapon.position = currentPosition;
            weapon.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 第二阶段：回到原位并恢复大小
        DebugLog("突刺阶段：回到原位并恢复大小");
        float thrustBackStartTime = elapsedTime;

        while (elapsedTime < thrustBackStartTime + thrustBackTime)
        {
            float phaseTime = elapsedTime - thrustBackStartTime;
            float progress = phaseTime / thrustBackTime;

            // 使用曲线控制回退速度
            float curveValue = thrustSpeedCurve.Evaluate(progress);

            // 计算当前位置（从目标位置回到原位）
            Vector3 currentPosition = Vector3.Lerp(thrustTargetPosition, originalPosition, curveValue);

            // 计算当前缩放（从最大缩放回到原始大小）
            float currentScaleMultiplier = Mathf.Lerp(thrustScaleMultiplier, 1f, curveValue);
            Vector3 currentScale = originalScale * currentScaleMultiplier;

            // 应用变换
            weapon.position = currentPosition;
            weapon.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保恢复到精确的初始状态
        weapon.position = originalPosition;
        weapon.localScale = originalScale;

        isPerformingThrust = false;
        DebugLog("突刺动画完成");
    }

    /// <summary>
    /// 高级突刺动画 - 支持更复杂的突刺效果
    /// </summary>
    private IEnumerator PerformAdvancedThrustAnimation(Vector2 thrustDirection)
    {
        if (isPerformingThrust) yield break;

        isPerformingThrust = true;
        DebugLog("开始高级突刺动画");

        // 计算目标位置
        Vector3 thrustTargetPosition = originalPosition + (Vector3)thrustDirection * thrustDistance;

        // 突刺分为三个阶段：蓄力、突刺、回退
        float prepareTime = thrustDuration * 0.2f;    // 蓄力时间
        float executeTime = thrustDuration * 0.5f;    // 突刺时间
        float recoverTime = thrustDuration * 0.3f;    // 回退时间

        float elapsedTime = 0f;

        // 第一阶段：蓄力（轻微后退）
        DebugLog("蓄力阶段");
        Vector3 preparePosition = originalPosition - (Vector3)thrustDirection * (thrustDistance * 0.2f);

        while (elapsedTime < prepareTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / prepareTime;

            // 蓄力动画（轻微后退和轻微缩小）
            Vector3 currentPosition = Vector3.Lerp(originalPosition, preparePosition, progress);
            Vector3 currentScale = Vector3.Lerp(originalScale, originalScale * 0.9f, progress);

            weapon.position = currentPosition;
            weapon.localScale = currentScale;

            yield return null;
        }

        // 第二阶段：快速突刺并放大
        DebugLog("突刺阶段");
        float executeStartTime = elapsedTime;

        while (elapsedTime < executeStartTime + executeTime)
        {
            float phaseTime = elapsedTime - executeStartTime;
            float progress = phaseTime / executeTime;

            // 使用曲线控制突刺速度
            float curveValue = thrustSpeedCurve.Evaluate(progress);

            // 从蓄力位置快速突刺到目标位置
            Vector3 currentPosition = Vector3.Lerp(preparePosition, thrustTargetPosition, curveValue);
            float currentScaleMultiplier = Mathf.Lerp(0.9f, thrustScaleMultiplier, curveValue);
            Vector3 currentScale = originalScale * currentScaleMultiplier;

            weapon.position = currentPosition;
            weapon.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 第三阶段：回到原位
        DebugLog("回退阶段");
        float recoverStartTime = elapsedTime;

        while (elapsedTime < recoverStartTime + recoverTime)
        {
            float phaseTime = elapsedTime - recoverStartTime;
            float progress = phaseTime / recoverTime;

            // 缓慢恢复到初始状态
            Vector3 currentPosition = Vector3.Lerp(thrustTargetPosition, originalPosition, progress);
            float currentScaleMultiplier = Mathf.Lerp(thrustScaleMultiplier, 1f, progress);
            Vector3 currentScale = originalScale * currentScaleMultiplier;

            weapon.position = currentPosition;
            weapon.localScale = currentScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保恢复到精确的初始状态
        weapon.position = originalPosition;
        weapon.localScale = originalScale;
        weapon.rotation = originalRotation;

        isPerformingThrust = false;
        DebugLog("高级突刺动画完成");
    }

    private void PerformFirearmAttack(Vector2 direction)
    {
        DebugLog("执行枪械攻击");

        // 发射子弹
        if (weaponData?.attackInstance != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
            DebugLog($"生成子弹: {weaponData.attackInstance.name}");

            // 设置子弹属性
            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(damage, direction, 10f);
                DebugLog($"子弹初始化完成 - 伤害: {damage}, 方向: {direction}, 速度: 10");
            }
            else
            {
                DebugLogWarning("子弹预制体上没有找到Bullet脚本");
            }
        }
        else
        {
            DebugLogWarning("weaponData.attackInstance 为空，无法生成子弹");
        }
    }

    private void DealDamageToTarget(GameObject target, float damageAmount)
    {
        DebugLog($"尝试对 {target.name} 造成 {damageAmount} 伤害");

        // 查找目标的生命值组件
        var health = target.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            DebugLog($"成功对 {target.name} 造成 {damageAmount} 点伤害");
        }
        else
        {
            DebugLogWarning($"目标 {target.name} 没有实现IHealth接口，无法造成伤害");
        }
    }

    public void SetWeaponStats(float newDamage, float newAttackSpeed)
    {
        damage = newDamage;
        attackSpeed = newAttackSpeed;
        DebugLog($"武器属性已更新: 伤害:{damage} 攻速:{attackSpeed}");
    }

    public string GetWeaponStatus()
    {
        return $"武器: {weaponData?.ItemName ?? "未知"}\n" +
               $"伤害: {damage}\n" +
               $"攻速: {attackSpeed}\n" +
               $"类型: {attackType}\n" +
               $"可攻击: {CanAttack}\n" +
               $"攻击中: {isAttacking}\n" +
               $"当前相机: {(currentCamera != null ? currentCamera.name : "无")}\n" +
               $"FirePoint: {(firePoint != null ? firePoint.name : "无")}\n" +
               $"目标层级: {targetLayerMask}";
    }

    private void OnDrawGizmosSelected()
    {
        if (!showAttackRange || firePoint == null) return;

        Gizmos.color = Color.red;

        switch (attackType)
        {
            case AttackType.Slash:
                // 绘制扇形攻击范围
                Gizmos.DrawWireSphere(firePoint.position, 2f);
                break;
            case AttackType.Thrust:
                // 绘制直线攻击范围
                Vector2 direction = GetAttackDirection();
                Gizmos.DrawRay(firePoint.position, direction * 3f);
                break;
            case AttackType.Firearm:
                // 绘制射击方向
                Vector2 shootDirection = GetAttackDirection();
                Gizmos.DrawRay(firePoint.position, shootDirection * 5f);
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        if (targetLayerMask != (targetLayerMask | (1 << other.gameObject.layer))) return;
        if (hitThisSwing.Contains(other.gameObject)) return;

        var health = other.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            hitThisSwing.Add(other.gameObject);
        }
    }
}
