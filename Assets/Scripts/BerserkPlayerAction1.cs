using System.Collections;
using System.Linq;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class BerserkPlayerAction1 : IPlayerAction
{

    public float activeDuration = 1.5f;
    GameObject sword;
    //for the sheathing
    private BerserkModifier mod;
    private Rigidbody2D rb;
    public BerserkPlayerAction1(BerserkModifier modSource)
    {
        this.mod = modSource;
    }


  public IEnumerator ActiveCycle()
  {
    mod.DrawSword();
    yield return new WaitForSeconds(activeDuration);
    mod.SheatheSword();
    yield break;

  }
  public bool OnCoolDown()
  {
    return false;
  }
    void IPlayerAction.Run()
    {
        //To allow for restarting in case of double jump
      
        if (mod.player.GetJumpsAvailable() <= 0)
        {
            return;
        }
          mod.player.StopCoroutine(ActiveCycle());
        mod.player.StartCoroutine(ActiveCycle());
        // rb = sword.GetComponent<Rigidbody2D>();
        mod.player.animator.SetTrigger("Jump");
        mod.player.jumpsUsed += 1;
        sword = ((GameObject)mod.sword);
       
        //freezes the player rotation so the sword can be aimed with modifers that affect rotation (beach ball, etc.)
        mod.player.rb.freezeRotation = true;
        
        //creates a motion blur-like effect
        mod.player.StartCoroutine( mod.player.GenerateTrail(4, 1));
        Vector2 camDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mod.player.transform.position).normalized;
        float angle = Mathf.Atan2(camDir.y, camDir.x) * Mathf.Rad2Deg - mod.player.transform.rotation.eulerAngles.z;
      

        var inbet = ((mod.player.transform.InverseTransformVector(camDir)));

      
       
        //angle sum theorm go brrr
        //maybe overkill but meh
        float cursorRadians = Mathf.Atan2(inbet.y, inbet.x);
#region trig stuff
       var firstAngle = cursorRadians;
        var secondAngle = mod.player.rb.rotation * Mathf.Deg2Rad;
        var finalCosine = Mathf.Cos(firstAngle) * Mathf.Cos(secondAngle) - (Mathf.Sin(firstAngle) * Mathf.Sin(secondAngle));
        var finalSine = Mathf.Sin(firstAngle) * Mathf.Cos(secondAngle) + Mathf.Sin(secondAngle) * Mathf.Cos(firstAngle);
        var finalAngle = new Vector2(finalCosine, finalSine).normalized;

#endregion

     
        int consta = 18;//:change this for the good changes?
        mod.player.StopCoroutine(nameof(MoveSwordInDir));
        mod.player.StartCoroutine(MoveSwordInDir(finalAngle * consta));

        sword.transform.GetChild(0).GetComponentInChildren<Animator>().SetTrigger("Swing");
        //  (camDir);



        mod.player.rb.AddForce(6500 * camDir);
        mod.player.entityBase.AS.PlayOneShot(mod.player.entityBase.soundManager.GetClip(SFXManagerSO.Sound.whoosh));
    }

    private IEnumerator MoveSwordInDir(Vector3 end)
    {

        sword.transform.localPosition = -end*3/5;
        Vector3 mainVector = new Vector3(0, 0, (float)(
                            ((Mathf.Atan2(end.y, end.x) + Mathf.PI * .5)
                            * Mathf.Rad2Deg)));
        var endNormalized = end.normalized;
        var endInDeg = Mathf.Atan2(endNormalized.y, endNormalized.x) * Mathf.Rad2Deg;
        // sword.transform.rotation.SetFromToRotation(Vector3.forward, Vector3.forward * endInDeg);
        mod.SetRotation((int)endInDeg -90);//oh my god we might have it



        for (int i = 0; i < 5; i++)
        {
            sword.transform.position += end / 5;
          
            sword.GetComponent<BoxCollider2D>().enabled = true;
            yield return new WaitForSeconds(.0125f * 1.5f);
        }
       
            yield return new WaitForSeconds(3f);
            //seeing if the player has a freeroation modifier applied
           if (mod.player.mods.Any(item => item.GetType().GetInterfaces().Contains(typeof(IFreeRotationModifier))))
            {
        mod.player.rb.freezeRotation = false;
        
                
            }

        yield break;

    }

    public Sprite GetIcon()

    {
        return mod.GetIcon();
    }

    public void DecrementCD()
    {
     return;
    }
}