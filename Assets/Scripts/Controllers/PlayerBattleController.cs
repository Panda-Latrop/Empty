using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleController : PlayerController
{
    [SerializeField]
    protected float sensitivity = 1.5f;
    protected Character character;
    protected float timeDeathPause = 1.0f;
    protected float nextDeathPause;

    protected new CameraPlayerFPSActor camera;
    protected GameEmptyState state;
    protected bool hasUI;
    protected UIContainer uIContainer;
    protected HUDUI hUDUI;
    protected MenuUI menuUI;
    protected bool gameOver;
    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject uic = GameObject.FindGameObjectWithTag("UI");
        if (uic != null)
        {
            uIContainer = uic.GetComponent <UIContainer>();

            SimpleUI sui = default;
            uIContainer.GetScreen("HUD",ref sui);
            hUDUI = sui as HUDUI;

            sui = default;
            uIContainer.GetScreen("Menu", ref sui);
            menuUI = sui as MenuUI;
            


            hasUI = true;
        }
        else
        {
            hasUI = false;
        }
        state = GameInstance.Instance.GameState as GameEmptyState;
    }

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as Character;
        gameInput = true;
        camera = character.GetComponentInChildren<CameraPlayerFPSActor>();
    }
    public override void Unpossess()
    {
        base.Unpossess();
        character = null;
        gameInput = false;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        base.OnDeath(ds, raycastHit);
        GameOver();
    }
    public override void GameOver()
    {
        if (!gameOver)
        {
            hUDUI.Hide();
            gameInput = false;
            gameOver = true;
            nextDeathPause = Time.time + timeDeathPause;
        }
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        if (hasUI)
        {
            hUDUI.StartPain();
        }
    }
    protected void Update()
    {



        if (!gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = !Cursor.visible;
                if (Cursor.visible)
                    Cursor.lockState = CursorLockMode.Confined;
                else
                    Cursor.lockState = CursorLockMode.Locked;

                if (gameInput)
                {
                    menuUI.Show();
                    Time.timeScale = 0;
                    gameInput = false;
                }
                else
                {
                    menuUI.Hide();
                    Time.timeScale = 1;
                    gameInput = true;
                }

            }

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    var c = Physics.OverlapSphere(character.Center, 1.0f, (1 << 7) | (1 << 12), QueryTriggerInteraction.Collide);

            //    IInteractive interactive;
            //    for (int i = 0; i < c.Length; i++)
            //    {
            //        if ((interactive = c[i].GetComponent<IInteractive>()) != null)
            //        {
            //            interactive.Intercat(character);
            //            return;
            //        }
            //    }
            //}
            if (gameInput)
            {

                Vector3 look = new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, Input.GetAxis("Mouse X") * sensitivity, 0.0f);
                character.LookRotate(look * Time.fixedDeltaTime);


                #region Weapon Left
                if (Input.GetMouseButtonDown(0))
                    character.SetFire(true);
                else
                {
                    if (Input.GetMouseButton(0))
                        character.Shoot(Vector3.zero, Vector3.zero);
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            character.Shoot(false, Vector3.zero, Vector3.zero);
                        }

                    }
                }
                #endregion

                #region Reload
                if (Input.GetKeyDown(KeyCode.R))
                    character.Reload();
                else
                {
                    if (Input.GetKey(KeyCode.R))
                        character.Reload();
                    else
                    {
                        if (Input.GetKeyUp(KeyCode.R))
                            character.Reload(false);
                    }
                }
                #endregion

                if (character.Sprinting.IsSprint && (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)))// || (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)))
                {
                    character.Slide(true);
                }
                else
                {

                    Vector3 direction = Vector3.zero;
                    direction.Set(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
                    //Debug.Log(direction);
                    character.Move(direction.normalized);
                    if (Input.GetKey(KeyCode.Space))
                        character.Jump();

                    if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
                    {
                        character.Crouch(true);
                    }
                    else if ((Input.GetKeyUp(KeyCode.C) && !Input.GetKey(KeyCode.LeftControl)) || (Input.GetKeyUp(KeyCode.LeftControl) && !Input.GetKey(KeyCode.C)))
                    {
                        character.Crouch(false);
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        character.Sprint(true);
                    }
                    else if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        character.Sprint(false);
                    }
                }

                for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha9; i++)
                {
                    if (Input.GetKeyDown(i))
                    {
                        character.WeaponHolder.Change(((int)i)-49);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    character.WeaponHolder.Change();
                }
            }


            if (hasUI)
            {
                hUDUI.SetHealth(character.Health.Health / character.Health.MaxHealth);
                hUDUI.SetStamina(character.Sprinting.Stamina / character.Sprinting.MaxStamina);
                hUDUI.SetTimer(state.GetTimer());
            }
        }
        else
        {
            if (Time.time >= nextDeathPause)
            {
                camera.Fade.Out();
                if (Input.GetMouseButtonDown(0) || (Time.time - nextDeathPause) >= 2.0f)
                {
                    GameInstance.Instance.LoadScene("level1",0, 0);
                }
            }
               
        }



    }
}
