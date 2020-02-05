using UnityEngine;

public class StarDust : MonoBehaviour
{
    public ParticleSystemRenderer particle;
    public ParticleSystem particleSpeed;
    float maxSpeed = 2f;
    float lowSpeed = 0.5f;




    // Update is called once per frame
    void Update()
    {
        var main = particleSpeed.main;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward);
            particle.lengthScale = 5f;
            main.startSpeed = maxSpeed;
            //particleSpeed.startSpeed = 2f;

            //Debug.Log("is moving");
        }
        else
        {
            particle.lengthScale = 1f;
            main.startSpeed = lowSpeed;
            //particleSpeed.startSpeed = 0.5f;
        }
    }
}
