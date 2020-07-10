using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    #region "싱글턴"
    private static PlayerInput instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static PlayerInput Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion

    #region "점수 관련 변수"
    public int killCount;
    public float timeCount = 0.00f;
    private float timer = 0f;
    public int totalScore;
    public bool bossClear = false;
    public GameObject scorePanel;
    public TextMeshProUGUI textMeshPro;
    public GameObject newRecordTab;
    public GameObject clearTab;
    public GameObject escPanel;
    public GameObject youDiedTab;
    #endregion

    #region "마법 관련 변수
    public Image fireBallProgress;
    public Image fireWallProgress;
    public GameObject fireBallGenPoint;
    public GameObject fireWallGenPoint;
    public float fireBallCoolTime = 10.0f;
    public float fireWallCoolTime = 60.0f;
    #endregion

    #region "사운드 관련 변수
    public bool isMeetBoss = false;
    public GameObject caveMusic;
    public AudioClip[] playerSound;
    private AudioSource audioSource;
    #endregion

    #region "플레이어 스탯 변수"
    [HideInInspector]
    public int hpPotionCap;
    [HideInInspector]
    public int mpPotionCap;
    [HideInInspector]
    public float hp = 100f;
    [HideInInspector]
    public float stamina = 100f;
    [HideInInspector]
    public float mana = 10f;
    //스태미너 회복량
    private int staminaRecoverValue = 10;
    //남은 마나게이지
    [HideInInspector]
    public int fireCapacity;
    #endregion

    #region "기타 플레이어 변수"
    public bool isFireLit = false;
    public bool isSwordOn = true;
    public bool isPowerAttack = false;
    private PlayerMovement playerMovement;
    public GameObject sword;
    public GameObject trail;
    public GameObject enchantTrail;
    public GameObject normalTrail;
    public GameObject normalIcon;
    public GameObject enchantIcon;
    public GameObject enchant;//불 파티클

    public Slider hpSlider;
    public Slider staminaSlider;
    public Image manaImage;
    public Image manaBackGround;
    [HideInInspector]
    public bool isAttackEnd;
    [HideInInspector]
    public bool isRollEnd;
    private bool isBackButtonInput = false;
    [HideInInspector]
    public Vector3 dir;

    //칼에 불 붙였는지 판별
    public bool isEnchanted = false;

    //포션 파티클
    public GameObject HpPotionParticle;
    public GameObject MpPotionParticle;

    //칼 잔상
    private TrailRenderer trailRenderer;

    //콤보 관련
    private int comboCount;
    private bool isCanCombo = false;

    //구르기 후 돌아갈 회전값
    private Quaternion orgRotation;
    #endregion

    //플레이어 상태
    public enum PlayerState
    {
        IDLE,
        MOVE,
        ATTACK,
        ROLL,
        START_CASTING,
        END_CASTING,
        HIT,
        KNOCKBACK,
        DODGE,
        POWERATTACK,
        JUMP,
        DIE,
    }

    public Animator playerAnim { get; set; }
    public PlayerState state { get; set;}
    public float move { get; private set; }
    public float sideMove { get; private set; }
    public float mouseX { get; private set; }
    public float mouseY { get; private set; }

    public bool sideMoveTrigger { get; private set; }
    public bool attack { get; private set; }
    public bool turn { get; private set; }
    public bool roll { get; private set; }

    private void Start()
    {
        Cursor.visible = false;                     //마우스 커서가 보이지 않게 함
        Cursor.lockState = CursorLockMode.Locked;   //마우스 커서를 고정시킴
        audioSource = GetComponent<AudioSource>();
        trail = normalTrail;
        playerAnim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        state = PlayerState.IDLE;
        isAttackEnd = true;
        isRollEnd = true;
        fireBallProgress.transform.gameObject.SetActive(false);
        fireWallProgress.transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EscPanelSwitch();
        }

        //치트
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            fireCapacity++;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            killCount++;
        }

        //유 다이 메시지 띄우기 까지 텀
        if (state == PlayerState.DIE)
        {
            timer += Time.deltaTime * 1;
        }

        //보스 클리어하기 전까지의 시간을 잰다.
        if (!bossClear)
        {
            timeCount += Time.deltaTime * 1.00f;
        }
        else
        {
            timer += Time.deltaTime * 1;
        }
        
        if (timer > 3 && state == PlayerState.DIE)
        {
            escPanel.SetActive(true);
            youDiedTab.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //불 킨 수에 따라 마법 해제
        if(fireCapacity > 0 && fireBallProgress.gameObject.activeSelf==false)
        {
            fireBallProgress.gameObject.SetActive(true);
        }

        if (fireCapacity > 1 && fireWallProgress.gameObject.activeSelf == false)
        {
            fireWallProgress.gameObject.SetActive(true);
        }

        //클리어시 플레이어 점수 표시
        if (bossClear && scorePanel.activeSelf==false && timer > 7)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            scorePanel.SetActive(true);
            totalScore = (killCount * 1000) + (int)(10000.00f / timeCount);
            if(GameManager.instance.gameData.TotalScore < totalScore)
            {
                newRecordTab.SetActive(true);
                textMeshPro.text = "Clear Time - " + timeCount + "\r\n" +
                                   "Kill Count - " + killCount + "\r\n" +
                                   "New Top Score - " + totalScore + "\r\n" +
                                   "Pre Top Score - " + GameManager.instance.gameData.TotalScore + "\r\n";

                GameManager.instance.gameData.TotalScore = totalScore;
                GameManager.instance.gameData.TimeCount = (int)timeCount;
                GameManager.instance.gameData.killCount = killCount;
                GameManager.instance.SaveGameData();
            }
            else
            {
                clearTab.SetActive(true);
                textMeshPro.text = "Clear Time - " + timeCount + "\r\n" +
                                  "Kill Count - " + killCount + "\r\n" +
                                  "Total Score - " + totalScore + "\r\n" +
                                  "Top Record - " + GameManager.instance.gameData.TotalScore + "\r\n";
            }
        }
        //브금 재생
        if(isMeetBoss)
        {
            caveMusic.SetActive(false);
        }

        //이미지 필어마운트
        hpSlider.value = hp / 100;
        staminaSlider.value = stamina / 100;
        manaImage.fillAmount = mana / 400;
        manaBackGround.fillAmount = fireCapacity / 4;

        if(fireBallProgress.gameObject.activeSelf)
        {
            fireBallProgress.fillAmount = fireBallCoolTime / 10f;
        }
        if(fireWallProgress.gameObject.activeSelf)
        {
            fireWallProgress.fillAmount = fireWallCoolTime / 60f;
        }
       
        if(fireBallCoolTime < 10)
        {
            fireBallCoolTime += Time.deltaTime;
        }

        if(fireWallCoolTime < 60)
        {
            fireWallCoolTime += Time.deltaTime;
        }


        //인챈트 중일 때는 마나가 감소
        if(isEnchanted)
        {
            mana -= Time.deltaTime * 1;
            if (mana < 1)
            {
                EnchantOff();
            }
        }

        //입력 감지
        if (state != PlayerState.HIT && state != PlayerState.DIE && Time.timeScale == 1 && isPowerAttack == false)
        {
            move = Input.GetAxis("Vertical");
            sideMove = Input.GetAxis("Horizontal");
            attack = Input.GetButton("Attack");
            turn = Input.GetButton("Turn");
            roll = Input.GetButton("Roll");

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            //안맞을 때 실행
            if (move == 0 && sideMove == 0 && isAttackEnd == true && isRollEnd == true
                && state != PlayerState.START_CASTING && state != PlayerState.END_CASTING && state != PlayerState.ATTACK && state != PlayerState.JUMP)
            {
                //if(isBackButtonInput) transform.forward *= -1;
                isBackButtonInput = false;
                state = PlayerState.IDLE;
            }

            //공격 애니메이션이 끝난 상태일 때, 왼클릭 시 공격
            if (attack && isAttackEnd && stamina > 5 && state != PlayerState.ROLL)
            {
                stamina -= 5;
                Attack();
            }

            //콤보 공격
            if(state == PlayerState.ATTACK && !isAttackEnd && isCanCombo && attack)
            {
                comboCount++;
                isCanCombo = false;
                switch (comboCount)
                {
                    case 1:
                        if(stamina - 5 > 0)
                        {
                            stamina -= 5;
                            AttackCombo1();
                        }
                        else StateToIdle();

                        break;
                    case 2:
                        if (stamina - 10 > 0)
                        {
                            stamina -= 10;
                            AttackCombo2();
                        }
                        else StateToIdle();
                        break;
                    case 3:
                        StateToIdle();
                        break;
                }
            }

            //구르기 애니메이션이 끝난 상태일 때 , 스페이스 클릭시 구르기
            if (Input.GetKeyDown(KeyCode.LeftShift) && isRollEnd && stamina > 30 && state != PlayerState.ATTACK)
            {
                stamina -= 30;
                audioSource.PlayOneShot(playerSound[0], 0.6f);
                Roll();
            }

            //불켜기
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (mana == 0) mana += 5;
                playerAnim.SetTrigger("CASTING");
            }

            //불끄기
            if (Input.GetKeyDown(KeyCode.Tab) && isEnchanted)
            {
                playerAnim.SetTrigger("CASTINGOFF");
            }

            //체력회복
            if (Input.GetKeyDown(KeyCode.F1) && hp < 100 && hpPotionCap > 0)
            {
                HpRecover();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CheatRecover();
            }

            //마나회복
            if (Input.GetKeyDown(KeyCode.F2) && mana < fireCapacity * 100 && mpPotionCap > 0)
            {
                mpPotionCap--;
                ManaRefill();
            }

            //회피
            if (Input.GetKeyDown(KeyCode.LeftAlt)
                && state != PlayerState.ROLL
                && stamina > 15)
            {
                stamina -= 15;
                audioSource.PlayOneShot(playerSound[0], 0.6f);
                playerAnim.SetTrigger("DODGE");
                state = PlayerState.ROLL;
            }

            //점프
            if (Input.GetKeyDown(KeyCode.Space)
                && state != PlayerState.ROLL
                && stamina > 15)
            {
                stamina -= 15;
                audioSource.PlayOneShot(playerSound[0], 0.6f);
                playerAnim.SetTrigger("JUMP");
                state = PlayerState.JUMP;
            }

            //턴
            if (Input.GetKeyDown(KeyCode.V) && state != PlayerState.ATTACK
                && state != PlayerState.ROLL)
            {
                playerAnim.SetTrigger("TURN");
            }

            //강공격
            if (Input.GetMouseButtonDown(1) && isAttackEnd && stamina > 10 && state != PlayerState.ROLL)
            {
                stamina -= 10;
                state = PlayerState.ATTACK;
                isAttackEnd = false;
                isCanCombo = false;
                isPowerAttack = true;
                playerAnim.SetTrigger("POWERATTACK");
                PlayAirSound();
            }

            //파이어 볼
            if (Input.GetKeyDown(KeyCode.G) && isAttackEnd && mana > 10 && state != PlayerState.ROLL
                && fireBallCoolTime >= 10)
            {
                mana -= 10;
                fireBallCoolTime = 0;
                playerAnim.SetTrigger("FIREBALL");
            }

            //파이어 월
            if (Input.GetKeyDown(KeyCode.H) && isAttackEnd && mana > 20 && state != PlayerState.ROLL
                && fireWallCoolTime >= 20)
            {
                mana -= 20;
                fireWallCoolTime = 50;
                playerAnim.SetTrigger("CASTINGWALL");
            }

            //스태미너 회복
            if (state == PlayerState.MOVE || state == PlayerState.IDLE)
            {
                if (stamina > 100) stamina = 100;
                stamina += staminaRecoverValue * Time.deltaTime;
            }

            
        }
        //옆으로 이동 중인지 판별한다.
        if (sideMove != 0 && move == 0)
        {
            sideMoveTrigger = true;
        }
        else sideMoveTrigger = false;

    }

    private void AttackCombo1()
    {
        isSwordOn = false;
        sword.SetActive(false);
        playerAnim.SetInteger("ATTACKNUM", 1);
    }

    private void AttackCombo2()
    {
        isSwordOn = false;
        sword.SetActive(false);
        playerAnim.SetInteger("ATTACKNUM", 2);
    }

    private void Roll()
    {
        #region "구르기 로직"
        state = PlayerState.ROLL;
        isRollEnd = false;

        if (move < 0)
        {
            isBackButtonInput = true;
            transform.forward *= -1;
        }
        else if (move == 0)
        {
            if (sideMove > 0)
            {
                //오른쪽 구르기
                transform.forward = transform.right;
            }
            else if (sideMove < 0)
            {
                //왼 구르기
                transform.forward = -transform.right;
            }
        }
        else if (move > 0)
        {
            if (sideMove > 0)
            {
                //왼 대각 구르기
                transform.forward = transform.forward - (-transform.right);
                
            }
            else if (sideMove < 0)
            {
                //오른 대각 구르기
                transform.forward = transform.forward - transform.right;
            }
        } 
        //if (sideMove > 0 && move < 0)
        //{
        //    isBackButtonInput = true;
        //    transform.forward = -(-transform.forward -(transform.right));
        //}
        //if (sideMove < 0 && move < 0)
        //{
        //    isBackButtonInput = true;
        //    transform.forward = -(-transform.forward -(-transform.right));
        //}
        if (isBackButtonInput) playerAnim.SetBool("ISBACKTURN", true);
        playerAnim.SetTrigger("ROLL");
        #endregion
    }

    public void RollEnd()
    {
        if(!isBackButtonInput)
        {
            isRollEnd = true;
        }
        StateToIdle();
    }

    public void TurnEnd()
    {
        //transform.forward *= -1;
        isRollEnd = true;
        playerAnim.SetBool("ISBACKTURN", false);
        StateToIdle();
    }

    public void AttackEnd()
    {
        state = PlayerState.IDLE;
        isAttackEnd = true;
        isCanCombo = false;
        isSwordOn = false;
        sword.SetActive(false);
    }

    public void SwordColliderOn()
    {
        isSwordOn = true;
        sword.SetActive(true);
        print("현재 상태" + state);
    }

    private void Attack()
    {
        state = PlayerState.ATTACK;
        isAttackEnd = false;
        isCanCombo = false;
        playerAnim.SetTrigger("ATTACK");
        playerAnim.SetInteger("ATTACKNUM", 0);
    }

    public void SwordTrailOn()
    {
        audioSource.PlayOneShot(playerSound[Random.Range(3, 6)], 1.0f);
        trail.SetActive(true);
    }

    public void SwordTrailOff()
    {
        trail.SetActive(false);
        isCanCombo = true;
    }

    public void StateToIdle()
    {
        state = PlayerState.IDLE;
        transform.rotation = orgRotation;
        isAttackEnd = true;
        isCanCombo = false;
        isRollEnd = true;
        isPowerAttack = false;
        comboCount = 0;
        playerAnim.SetBool("ISBACKTURN", false);
        playerAnim.SetInteger("ATTACKNUM", 0);
        isSwordOn = false;
        sword.SetActive(false);
        SwordTrailOff();
        print("플레이어 상태 : 아이들");
    }

    //대미지 + 일반 피격 모션
    public void PlayerDamage(float value)
    {
        //구르기일땐 피격 X
        if (state != PlayerState.DIE && state != PlayerState.HIT && state != PlayerState.ROLL && state != PlayerState.DODGE)
        {
            if (hp - value <= 0)
            {
                hp -= value;
                state = PlayerState.DIE;
                playerAnim.SetTrigger("DIE");
                hpSlider.transform.gameObject.SetActive(false);
                return;
            }
            else
            {
                hp -= value;
                state = PlayerState.HIT;
                audioSource.PlayOneShot(playerSound[1], 1.2f);
                playerAnim.SetTrigger("HIT");
                print("플레이어의 현재 HP" + hp);
            }
        }
    }

    //대미지 + 넉백 모션
    public void PlayerDamage(float value, string key)
    {
        if (state != PlayerState.DIE && state != PlayerState.HIT && state != PlayerState.ROLL && state != PlayerState.DODGE)
        {
            //orgRotation = transform.rotation;
            //transform.rotation = rot;
            if (hp - value <= 0)
            {
                hp -= value;
                state = PlayerState.DIE;
                playerAnim.SetTrigger("DIE");
                hpSlider.transform.gameObject.SetActive(false);
                return;
            }
            else
            {
                hp -= value;
                state = PlayerState.HIT;
                playerAnim.SetTrigger("KNOCKBACK");
                audioSource.PlayOneShot(playerSound[2], 1f);
                print("플레이어의 현재 HP" + hp);
            }
        }
    }

    public void StartCasting()
    {
        state = PlayerState.START_CASTING;
    }

    public void EndCasting()
    {
        state = PlayerState.END_CASTING;
        StartCoroutine(IdleCoroutine());
    }

    IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        state = PlayerState.IDLE;
    }

    IEnumerator waitSecond()
    {
        yield return new WaitForSeconds(1f);
    }

    private void Enchant()
    {
        isEnchanted = true;
        trail = enchantTrail;
        enchant.SetActive(true);
        enchantIcon.SetActive(true);
        normalIcon.SetActive(false);
    }

    private void EnchantOff()
    {
        isEnchanted = false;
        trail = normalTrail;
        enchant.SetActive(false);
        enchantIcon.SetActive(false);
        normalIcon.SetActive(true);
    }

    public void PlusFireCap()
    {
        fireCapacity++;
    }

    public void ManaRefill()
    {
        MpPotionParticle.SetActive(true);
        print("마나 리필");
        mana = fireCapacity * 100;
    }

    private void HpRecover()
    {
        hpPotionCap--;
        HpPotionParticle.SetActive(true);
        print("체력 리필");
        if(hp + 30 <= 100)
        {
            hp += 30;
        }
        else
        {
            hp = 100;
        }
    }

    private void CheatRecover()
    {
        HpPotionParticle.SetActive(true);
        print("체력 리필");
        if (hp + 30 <= 100)
        {
            hp += 30;
        }
        else
        {
            hp = 100;
        }
    }

    public void PlayAirSound()
    {
        audioSource.PlayOneShot(playerSound[0], 0.8f);
    }

    public void PlayLandSound()
    {
        audioSource.PlayOneShot(playerSound[6], 0.8f);
    }

    public void EscPanelSwitch()
    {
        if (escPanel.activeSelf == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            escPanel.SetActive(true);
        }
        else
        {
            Cursor.visible = false;                     //마우스 커서가 보이지 않게 함
            Cursor.lockState = CursorLockMode.Locked;   //마우스 커서를 고정시킴
            escPanel.SetActive(false);
        }
    }

    public void GenarateFireball()
    {
        var _fireball = PoolingManager.Instance.GetFireball();
        if (_fireball != null)
        {
            _fireball.transform.rotation = Quaternion.identity;
            _fireball.transform.position = fireBallGenPoint.transform.position;
            _fireball.transform.forward = this.transform.forward;
            _fireball.SetActive(true);
        }
    }

    public void GenerateFireWall()
    {
        var _fireWall = PoolingManager.Instance.GetFireWall();
        if (_fireWall != null)
        {
            _fireWall.transform.position = this.transform.position;
            //_fireWall.transform.forward = this.transform.forward;
            _fireWall.SetActive(true);
        }
    }
}
