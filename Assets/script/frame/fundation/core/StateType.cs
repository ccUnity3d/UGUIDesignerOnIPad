 public enum StateType
 {
     /// <summary>
     /// 攻击或使用技能
     /// </summary>
     Attack, //0

     /// <summary>
     /// 战斗待机状态
     /// </summary>
     AttackIdle,

     /// <summary>
     /// 被牵引状态
     /// </summary>
     Attracted,

     /// <summary>
     /// 休息待机
     /// </summary>
     Standby, //1

     /// <summary>
     /// 一段跳
     /// </summary>
     Jump, //

     Move, //

     /// <summary>
     /// 行走
     /// </summary>
     Run, //

     /// <summary>
     /// 受到伤害
     /// </summary>
     Hit, //

     /// <summary>
     /// 死亡动画
     /// </summary>
     Die, //

     /// <summary>
     /// 睡眠
     /// </summary>
     Sleep, //

     /// <summary>
     /// 眩晕
     /// </summary>
     Dizzy, //

     /// <summary>
     /// 下落
     /// </summary>
     Fall, //

     /// <summary>
     /// 踩踏
     /// </summary>
     Tread, //

     /// <summary>
     /// 下蹲
     /// </summary>
     Squat, //

     /// <summary>
     /// 弹跳
     /// </summary>
     Bounce, //

     /// <summary>
     /// 冰冻
     /// </summary>
     Freeze, //

    /// <summary>
    /// 怪物飞行
    /// </summary>
    Fly,

 }


/// <summary>
/// 怪物飞行方式
/// </summary>
public enum FlyType
{
    /// <summary>
    /// 线性飞行，不从外部获取方向事件，模拟巡逻
    /// </summary>
    line,
    /// <summary>
    /// 圆形飞行，不从外部获取方向事件，模拟巡逻
    /// </summary>
    circle,
    /// <summary>
    /// 追击飞行，需从外部获取方向事件
    /// </summary>
    chase
}

/// <summary>
/// 怪物除行为之外的附加状态，不属于状态机，可与状态机状态重叠或互斥，在每个状态处做应有的判断，该状态为短时间特殊行为，类似buff效果
/// </summary>
public enum BuffState
{
    /// <summary>
    /// 无特殊状态
    /// </summary>
    none,

    /// <summary>
    ///僵直，会使怪物停留在当前动画桢，并且不可进行所有行为
    /// </summary>
    rigidity

}