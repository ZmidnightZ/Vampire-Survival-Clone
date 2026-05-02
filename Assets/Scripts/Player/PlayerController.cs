using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }

    public float moveSpeed;

    public Animator anim;

    public float pickupRange = 1.5f;

    //public Weapon activeWeapon;

    public List<Weapon> unassignedWeapons, assignedWeapons;

    public int maxWeapons = 3;

    [HideInInspector]
    public List<Weapon> fullyLevelledWeapons = new List<Weapon>();

    void Start()
    {
        if (assignedWeapons.Count == 0 && unassignedWeapons.Count > 0)
        {
            int randIndex = Random.Range(0, Mathf.Min(5, unassignedWeapons.Count));

            Weapon startingWeapon = unassignedWeapons[randIndex];

            AddWeapon(startingWeapon);
            startingWeapon.weaponLevel = 1;
        }
        // Ensure purchased stats are applied (in case of load order differences)
        if (PlayerStatController.instance != null)
        {
            PlayerStatController.instance.ApplyPurchasedStats();
        }
    }

    void Update()
    {
        Vector3 moveInput = new Vector3(0f, 0f, 0f);
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        transform.position += moveInput * moveSpeed * Time.deltaTime;

        if(moveInput != Vector3.zero)
        {
            anim.SetBool("isMoving", true);
        } else
        {
            anim.SetBool("isMoving", false);
        }
    }

    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber < unassignedWeapons.Count)
        {
            Weapon weapon = unassignedWeapons[weaponNumber];

            weapon.weaponLevel = 1;
            weapon.statsUpdated = false;

            assignedWeapons.Add(weapon);

            weapon.gameObject.SetActive(true);
            unassignedWeapons.RemoveAt(weaponNumber);
        }
    }

    public void AddWeapon(Weapon weaponToAdd)
    {
        weaponToAdd.weaponLevel = 1;
        weaponToAdd.statsUpdated = false;

        weaponToAdd.gameObject.SetActive(true);

        assignedWeapons.Add(weaponToAdd);
        unassignedWeapons.Remove(weaponToAdd);
    }
}
