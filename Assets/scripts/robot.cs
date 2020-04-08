using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class robot : Agent
{
    [Header("速度"), Range(1, 50)]
    public float speed = 10;

    /// <summary>
    /// 機器人鋼體
    /// </summary>
    private Rigidbody rigrobot;
    /// <summary>
    /// 足球鋼體
    /// </summary>
    private Rigidbody rigball;

    private void Start()
    {
        rigrobot = GetComponent<Rigidbody>();
        rigball = GameObject.Find("足球").GetComponent<Rigidbody>();

    }
    /// <summary>
    /// 事件開始時:重新設定機器人與足球位置
    /// </summary>
    public override void OnEpisodeBegin()
    {
        ///重設鋼體加速度
        rigrobot.velocity = Vector3.zero;
        rigrobot.angularVelocity = Vector3.zero;
        rigball.velocity = Vector3.zero;
        rigball.angularVelocity = Vector3.zero;

        //隨機機器人位置
        Vector3 posrobot = new Vector3(Random.Range(-2f, 2f), 0.1f, Random.Range(-2f, 0f));
        transform.position = posrobot;
        
        //隨機足球位置
        Vector3 posball = new Vector3(Random.Range(-2f, 2f), 0.1f, Random.Range(1f, 2f));
        rigball.position = posball;

        //足球尚未進入球門
        ball.complete = false;
    }
    /// <summary>
    /// 收集觀測資料
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 加入觀測資料 : 機器人 足球座標 機器人加速度 X Z
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rigball.position);
        sensor.AddObservation(rigrobot.velocity.x);
        sensor.AddObservation(rigrobot.velocity.z);
    }

    /// <summary>
    /// 動作 : 控制機器人與回饋
    /// </summary>
    public override void OnActionReceived(float[] vectorAction)
    {
        //使用參數控制機器人
        Vector3 control = Vector3.zero;
        control.x = vectorAction[0];
        control.z = vectorAction[1];
        rigrobot.AddForce(control * speed);

       // 球進入球門 成功 : 加一分並結束
        if (ball.complete)
        {
            SetReward(1);
            EndEpisode();
        }

        // 機器人或足球掉到地板下 失敗: 扣一分並結束
        if (transform.position.y < 0 || rigball.position.y < 0)
        {
            SetReward(1);
            EndEpisode();
        }
    }

    /// <summary>
    /// 探索: 讓開發者測試環境
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        // 提供開發者控制的方式
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}

