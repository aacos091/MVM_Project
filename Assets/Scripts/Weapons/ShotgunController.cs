using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotgunController : MonoBehaviour
{
    [Header("Shotgun Fire Rate")]
    public float fireRate;
    private float _nextFire;
    private bool _isFiring;
    
    [Header("Shotgun Reload Rate")]
    public float reloadRate;
    private float _nextReload;
    private bool _isReloading;

    [Header("Shotgun Check Time")]
    public float checkTime;
    private bool _isChecking;

    [Header("Shotgun Aiming Angles")]
    public float upAimingAngle;
    public float downAimingAngle;
    private bool _isAiming;

    private const float MinimumHeldDuration = 0.25f;
    private float _reloadPressedTime = 0;
    private bool _reloadHeld = false;

    [Header("Other")]
    public Transform gunBarrel;
    public LayerMask targetLayer;
    public Image weaponImage;

    private RaycastHit2D hit;

    private AmmoManager ammo;

    [Header("shotgun Sounds")]
    public AudioSource weaponAudio;
    public AudioClip shotgunEmpty, shotgunFire, shotgunInsertBullet, shotgunRemove, shotgunReload;

    // This is only used for drawing the lines in debugging
    [Header("Debug")]
    public Material lineMaterial;
    public Ray r2d;
    
    private void Start()
    {
        ammo = GetComponentInParent<AmmoManager>();
        
        Debug.Log("Amount of shells: " + ammo.shotgunShells);
        Debug.Log("Amount in barrel: " + ammo.currentShellCount);
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.currentShellCount + " shells in the barrel");
    }
    
    private void OnEnable()
    {
        weaponImage.gameObject.SetActive(true);
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    private void OnDisable()
    {
        weaponImage.gameObject.SetActive(false);
    }
    
    void shotgunRemoveSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(shotgunRemove);
            soundPlayed = true;
        }
    }

    void shotgunReloadSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(shotgunReload);
            soundPlayed = true;
        }
    }
    
    // See https://youtu.be/FgI8cgYAewM for reference
    // This is for aiming with the mouse and/or thumbstick
    void weaponRotationAiming()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 gunPosition = Camera.main.WorldToScreenPoint(transform.position);
        
        mousePos.x = mousePos.x - gunPosition.x;
        mousePos.y = mousePos.y - gunPosition.y;

        float gunAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(180f, 0f, -gunAngle));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, gunAngle));
        }
    }

    // Use this to aim in pre-defined directions
    void weaponDirectionalAiming()
    {
        if (Input.GetAxisRaw("Vertical") > 0.2f)
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, upAimingAngle);
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2f)
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, downAimingAngle);
        }
        else
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        
    }
    
    // See https://answers.unity.com/questions/8338/how-to-draw-a-line-using-script.html for reference
    // Only used for debugging
    void DrawLine(Vector2 start, Vector2 end, Color color, float duration = 3f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.useWorldSpace = false;
        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
