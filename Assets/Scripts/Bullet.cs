using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float damage;
    float penetration;
    GameObject origin;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyBullet",5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDamage(float damage_)
    {
        damage=damage_;
    }
    public void SetPenetration(float penetration_)
    {
        penetration=penetration_;
    }
    public void SetOrigin(GameObject origin_)
    {
        origin=origin_;
    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision other)
    {
    Vector3 objectToOther = other.transform.forward;
    Vector3 objectForward = transform.forward;

    float angle = Vector3.Angle(objectToOther, objectForward);
    if(angle>90)
    {
        angle-=90;
    }
    //Debug.Log("Угол соприкосновения: " + angle);
    Armor armor;
    if(other.gameObject.TryGetComponent<Armor>(out armor))
    {
        float thickness=armor.GetThickness();
        float way=thickness/Mathf.Cos(angle*Mathf.Deg2Rad);
       // Debug.Log("Cos"+Mathf.Cos(angle*Mathf.Deg2Rad));
        if(penetration>=way)
        {
            Debug.Log("Penetrated");

        }
        else
        {
             Debug.Log("Not Penetrated");
        }
    }


    DestroyBullet();
    }
}
