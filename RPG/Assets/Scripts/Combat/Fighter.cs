using UnityEngine;
using RPG.Move;
using RPG.Core;
using RPG.Animation;

namespace RPG.Combat 
{
    public class Fighter : MonoBehaviour, IAction
    {
        [Header("Attack")]
        [SerializeField] float attackingRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 20f;

        //[SerializeField] GameObject weaponPrefab = null;
        //[SerializeField] Transform handTransform = null;

        float timeSinceLastAttack = Mathf.Infinity;

        [Header("Scripts")]
        Movement movementScript;
        Health healthScriptOfAttacker;
        UpdateAnimator updateAnimatorScript;
        Health healthScriptOfTarget;

        bool isInRangeOfTarget;

        void Awake()
        {
            movementScript = GetComponent<Movement>();
            updateAnimatorScript = GetComponentInChildren<UpdateAnimator>();
            healthScriptOfAttacker = GetComponent<Health>();
        }

        void Start()
        {

        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            UpdateMoveToAttack();
        }

        //void EquipSword()
        //{
        //    Instantiate(weaponPrefab, handTransform);
        //}

        public void StartAttackAction(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            healthScriptOfTarget = combatTarget.GetComponent<Health>();
        }

        void UpdateMoveToAttack()
        {
            if (healthScriptOfTarget == null) return;
            if (healthScriptOfTarget == healthScriptOfAttacker) return;
            if (healthScriptOfAttacker.GetIsDeadBool()) return;
            if (healthScriptOfTarget.GetIsDeadBool()) return;

            MoveToTargetBehaviour();
            AttackBehaviour();
        }

        void MoveToTargetBehaviour()
        {
            if (gameObject.tag == "Player")
            {
                movementScript.MoveToTarget(healthScriptOfTarget.gameObject, 1f);
            }
            else if (gameObject.tag == "Enemy")
            {
                movementScript.MoveToTarget(healthScriptOfTarget.gameObject, 0.8f);
            }
            isInRangeOfTarget = Vector3.Distance(transform.position, healthScriptOfTarget.transform.position) < attackingRange;
            if (!isInRangeOfTarget) return;
            movementScript.StopAttack();
        }

        void AttackBehaviour()
        {
            if (!isInRangeOfTarget) return;
            transform.LookAt(healthScriptOfTarget.transform);
            if (timeSinceLastAttack < timeBetweenAttacks) return;
            healthScriptOfTarget.TakeDamage(GetWeaponDamage());
            updateAnimatorScript.AttackAnimation();
            timeSinceLastAttack = 0;
        }

        public void StopAttack()
        {
            updateAnimatorScript.StopAttackIfInProcess();
            healthScriptOfTarget = null;
            GetComponent<Movement>().StopAttack();
        }

        public Health GetSelectedTarget() => healthScriptOfTarget;
        public float GetWeaponDamage() => weaponDamage;
    }
}