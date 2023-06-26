using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject sphere;
    [SerializeField] GameObject cam;
    SphereCollider sphereCollider;
    [SerializeField] GameObject cannon;
    [SerializeField] float reloadTime;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletSpawner;
    [SerializeField] float damage;
    [SerializeField] float penetration;
    [SerializeField] float speed;
    [SerializeField] float rotSpeed;
    [SerializeField] float scatter=0;
    [SerializeField] float maxRightRotAgle=-1;
    [SerializeField] float maxLeftRotAgle=-1;
    [SerializeField] float maxUpRotAgle=90;
    [SerializeField] float maxDownRotAgle=-90;
    //[SerializeField] RectTransform circle;
    [SerializeField] Vector2 offSet;
    [SerializeField] Image circle;
    [SerializeField] float lowScatter;
    [SerializeField] float maxScatter;
    [SerializeField] RectTransform canvas;
    [SerializeField] Image willPenetateScope;
    [SerializeField] Image NotWillPenetrateScope;
    [SerializeField] Image UnknownPenetratescope;
    bool isReloading=false;
    RaycastHit hit;
    Vector3 pastFrameCoord;
    float circleStartScale;
    Coroutine coroutine=null;
    Image scope;

    void Start()
    {

    }
    private void FixedUpdate() {
        Ray ray;
        ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hit,500))
        {
            if(hit.point!=pastFrameCoord)
            {
                ChangeScatter();
                if(coroutine == null)
                {coroutine=StartCoroutine(LowScatter());}
            }
            pastFrameCoord=hit.point;

            Vector3 target=hit.point;
            Vector3 direct= Vector3.RotateTowards(transform.forward,target-transform.position,rotSpeed,0f);

            //Vector3 canonTarget=new Vector3(0,hit.point.y,0); 
            Vector3 CanonDirect= Vector3.RotateTowards(cannon.transform.forward,target-cannon.transform.position,rotSpeed,0f);

            Vector3 sphereDirect= Vector3.RotateTowards(transform.forward,target-transform.position,5f,0f);
            //Debug.Log(hit.point);
            //Vector3 cannnonDirect= Vector3.RotateTowards(cannon.transform.forward,target-cannon.transform.position,rotSpeed,0f);
            sphere.transform.rotation=Quaternion.LookRotation(sphereDirect);
            if(maxRightRotAgle>0 && maxLeftRotAgle>0)
            {
                if((sphere.transform.localEulerAngles.y<maxRightRotAgle)||(sphere.transform.localEulerAngles.y>maxLeftRotAgle))
                {
                    Vector3 temp=direct;
                    temp.y=0;
                    transform.rotation=Quaternion.LookRotation(temp);
                    
                }
            }
            else
            {
                //FIX THIS(without offset)!!!!!!
                Vector3 temp=direct;
                temp.y=0;
                transform.rotation=Quaternion.LookRotation(temp);
                RaycastHit hit2;
                LayerMask layerMask = ~LayerMask.GetMask("Bullet");
                if(Physics.Raycast(cannon.transform.position,cannon.transform.forward,out hit2,500,layerMask))
                {
                    //Vector2 screenPos = Camera.main.WorldToScreenPoint(hit2.point);
                   Vector3 screenPoint = Camera.main.WorldToScreenPoint(hit2.point);
                    screenPoint.z = 0;
                    
                    Vector2 screenPos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, Camera.main, out screenPos))
                    {
                        circle.transform.localPosition=screenPos-offSet;
                        Armor armor;
                        if(hit2.collider.gameObject.TryGetComponent<Armor>(out armor))
                        {
                            Vector3 objectToOther = hit2.collider.gameObject.transform.forward;
                            Vector3 objectForward = cannon.transform.forward;

                            float angle = Vector3.Angle(objectToOther, objectForward);
                            if(angle>90)
                            {
                                angle-=90;
                            }
                            float thickness=armor.GetThickness();
                            float way=thickness/Mathf.Cos(angle*Mathf.Deg2Rad);
                        // Debug.Log("Cos"+Mathf.Cos(angle*Mathf.Deg2Rad));
                            if(scope!=null)
                            {
                                Destroy(scope.gameObject);
                                scope=null;
                            }
                            if(penetration>=way)
                            {
                               if(penetration>way+penetration/10)
                                {
                                    scope=Instantiate(willPenetateScope,canvas.transform.position,canvas.transform.rotation);
                                    
                                    
                                }
                                else 
                                {
                                    scope=Instantiate(UnknownPenetratescope,canvas.transform.position,canvas.transform.rotation);
                                }
                                
                            }
                            else
                            {
                                scope=Instantiate(NotWillPenetrateScope,canvas.transform.position,canvas.transform.rotation);
                            }
                            scope.transform.parent=canvas.gameObject.transform;
                            scope.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
                        }
                        scope.transform.localPosition=screenPos-offSet;
                    }

                
                //Vector2 anchoredPos;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, null, out anchoredPos);
                //Debug.Log("anchoredPos "+anchoredPos);
                //circle.anchoredPosition = anchoredPos;
                
                }
                
                //Debug.DrawLine(Camera.main.transform.position,circle.transform.position+(Random.insideUnitSphere*circle.transform.localScale.x*0.1f),Color.red,1.0f);
                /*
                Vector2 screenPos = Camera.main.WorldToScreenPoint(hit.point);
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                screenPos.x -= screenWidth / 2f;
                screenPos.y -= screenHeight / 2f;

                Vector2 anchoredPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, null, out anchoredPos);
                circle.anchoredPosition = anchoredPos;
                Debug.DrawLine(Camera.main.transform.position,circle.position,Color.red,1.0f);*/
            }
            if(maxUpRotAgle>270 && maxDownRotAgle<90)
            {
                if((sphere.transform.localEulerAngles.x<maxDownRotAgle)||(sphere.transform.localEulerAngles.x>maxUpRotAgle))
                {
                    Quaternion temp=sphere.transform.rotation;
                    //cannon.transform.rotation=temp;
                    cannon.transform.rotation=Quaternion.LookRotation(CanonDirect);
                    
                }              
            }
            else
            {
                Debug.Log("Please difine max Up and Down Rot angles(they should be more than 270 and less than 90)");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1)&& !isReloading)
        {
            RaycastHit hit2;

            Vector3 pos=(circle.transform.position+(Random.insideUnitSphere*circle.transform.localScale.x*0.1f))-Camera.main.transform.position;
            //Debug.Log("POS "+ pos);
            if(Physics.Raycast(Camera.main.transform.position,pos,out hit2,500))
            {
                //Debug.Log("HIT "+ hit2.point);
                bulletSpawner.transform.LookAt(hit2.point);
            }
            GameObject bul= Instantiate(bullet,bulletSpawner.transform.position,bulletSpawner.transform.rotation);
            bul.SendMessage("SetDamage",damage);
            bul.SendMessage("SetPenetration",penetration);
            bul.SendMessage("SetOrigin",this.gameObject);
            Rigidbody rig;
            rig = bul.GetComponent<Rigidbody>();
            
            //bul.transform.LookAt(hit.point+GetRandomPointInSphere(scatter));
            //print();
            rig.AddRelativeForce(Vector3.forward*speed);
            StartCoroutine(reloading());
        }
    }
    private void Awake() {
       sphere=GameObject.CreatePrimitive(PrimitiveType.Sphere);
       sphere.transform.position=transform.position;
       sphere.transform.localScale=new Vector3(1f,1f,1f);
       sphere.transform.parent=this.gameObject.transform;
       sphereCollider=sphere.GetComponent<SphereCollider>();
       sphereCollider.isTrigger=true;
       MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
       sphereRenderer.enabled = false;
       circleStartScale=circle.transform.localScale.x;
    }
    IEnumerator reloading()
    {
        isReloading=true;
        yield return new WaitForSeconds(reloadTime);
        isReloading=false;
    }
     Vector3 GetRandomPointInSphere(float scatter_)
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        Vector3 randomPoint = randomDirection.normalized * scatter_;
        return randomPoint;     
    }
    void ChangeScatter()
    {
        float degree=(pastFrameCoord-hit.point).magnitude;
       // Debug.Log(degree);
        degree/=5;
        if(circle.transform.localScale.x+degree<maxScatter*circleStartScale)
        {
        circle.transform.localScale+=new Vector3(degree,degree,degree);
        }
        else
        {
            circle.transform.localScale=new Vector3(maxScatter*circleStartScale,maxScatter*circleStartScale,maxScatter*circleStartScale);
        }

    }
    IEnumerator LowScatter()
    {
        //Debug.Log("corot started");
        while(circle.transform.localScale.x!=scatter*circleStartScale)
        {
            if(circle.transform.localScale.x>scatter*circleStartScale)
            {
                if((circle.transform.localScale.x-scatter*circleStartScale)>lowScatter)
                {
                    circle.transform.localScale-=new Vector3(lowScatter,lowScatter,lowScatter);
                }
                else
                {
                    circle.transform.localScale=new Vector3(scatter*circleStartScale,scatter*circleStartScale,scatter*circleStartScale);
                }        
            }
            else
            {
                if((scatter*circleStartScale-circle.transform.localScale.x)>lowScatter)
                {
                    circle.transform.localScale+=new Vector3(lowScatter,lowScatter,lowScatter);
                }
                else
                {
                    circle.transform.localScale=new Vector3(scatter*circleStartScale,scatter*circleStartScale,scatter*circleStartScale);
                }  
            }
            yield return new WaitForEndOfFrame();
        }
        coroutine=null;
    }
}
