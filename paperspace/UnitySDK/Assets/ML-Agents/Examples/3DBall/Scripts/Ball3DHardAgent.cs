using UnityEngine;
using MLAgents;

public class Ball3DHardAgent : Agent
{
    [Header("Specific to Ball3DHard")]
    public GameObject ball;
    Rigidbody m_BallRb;
    IFloatProperties m_ResetParams;

    public float energy = 1f;
    public AnimationCurve curve;
    public float ballDiameter = 0.6f;
    public float reward;
    public float distance;
    public Vector3 diff;

    public override void InitializeAgent()
    {
        m_BallRb = ball.GetComponent<Rigidbody>();
        var academy = FindObjectOfType<Academy>();
        m_ResetParams = academy.FloatProperties;
        SetResetParameters();
    }

    public override void CollectObservations()
    {
        var pos = ball.transform.position;
        pos.y -= ballDiameter;

        diff = pos - gameObject.transform.position;

        Vector3 normalized = gameObject.transform.rotation.eulerAngles / 180.0f - Vector3.one;  // [-1,1]

        //AddVectorObs(gameObject.transform.rotation.z);
        //AddVectorObs(gameObject.transform.rotation.x);
        AddVectorObs(normalized.z);
        AddVectorObs(normalized.x);
        AddVectorObs(diff);
        AddVectorObs(energy);
    }

    public override void AgentAction(float[] vectorAction)
    {
        float r = 0;

        var pos = new Vector2(ball.transform.position.x, ball.transform.position.z);

        var target = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        distance = Vector2.Distance(pos, target);

        var actionZ = Mathf.Clamp(vectorAction[0], -1f, 1f);
        var actionX = Mathf.Clamp(vectorAction[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) || (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
            //energy -= Mathf.Abs(actionZ) / 100;
            energy -= 0.0001f;

            if (energy < 0.8f)
            {
                r -= Mathf.Abs(actionZ) / 5;
            }
        }

        if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) || (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
            //energy -= Mathf.Abs(actionX) / 100;
            energy -= 0.0001f;

            if (energy < 0.8f)
            {
                r -= Mathf.Abs(actionX) / 5;
            }
        }

        if ((ball.transform.position.y - gameObject.transform.position.y) < -2f)
        {
            Done();
            SetReward(-10f);
        }
        else
        {
            if (distance <= 1)
            {
                //r += (curve.Evaluate(distance)) / 3;
                r++;
            }
            //else
            //{
            //    r -= Mathf.Abs(actionZ);
            //    r -= Mathf.Abs(actionX);
            //}

            SetReward(r);
            //SetReward(0.1f);
        }

        reward = r;
    }

    public override void AgentReset()
    {
        energy = 1f;

        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        m_BallRb.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
            + gameObject.transform.position;
    }

    public void SetBall()
    {

        //Set the attributes of the ball by fetching the information from the academy
        m_BallRb.mass = m_ResetParams.GetPropertyWithDefault("mass", 1.0f);
        var scale = m_ResetParams.GetPropertyWithDefault("scale", 1.0f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetBall();
    }
}
