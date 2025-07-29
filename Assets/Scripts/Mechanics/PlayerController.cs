using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.InputSystem;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {

        Rigidbody2D rb;
        float originalGravity;

        private InputAction m_AttackAction;
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        private Vector3 attackPointOriginalLocalPos;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;
        
        public Transform attackPoint; // Empty GameObject as attack origin
        public float attackRange = 0.5f;
        public LayerMask enemyLayers;
        public int attackDamage = 1;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {

            rb = GetComponent<Rigidbody2D>();
            originalGravity = rb.gravityScale;

            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            
            attackPointOriginalLocalPos = attackPoint.localPosition;

            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            m_AttackAction = InputSystem.actions.FindAction("Player/Attack");
            
            m_AttackAction.Enable();
            m_MoveAction.Enable();
            m_JumpAction.Enable();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                if (!isAttack)
                    move.x = m_MoveAction.ReadValue<Vector2>().x;
                else
                    move = Vector2.zero;

                // Jump logic
                if (!isAttack && jumpState == JumpState.Grounded && m_JumpAction.WasPressedThisFrame())
                    jumpState = JumpState.PrepareToJump;
                else if (!isAttack && m_JumpAction.WasReleasedThisFrame())
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move = Vector2.zero;
            }
            UpdateJumpState();
            base.Update();
            
            if (controlEnabled && m_AttackAction.WasPressedThisFrame())
            {
                if (!isAttack && health != null && health.IsAlive)
                {
                    Attack();
                }
            }
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }
        
        bool isAttack = false;

        void Attack()
        {
            StartCoroutine(PerformAttack()); 
        }

        IEnumerator PerformAttack()
        {

            // Wait for animation length or fixed time
            animator.SetTrigger("attack");
            isAttack = true;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (var hitCollider in hitEnemies)
            {
                // Only damage if tagged "Enemy"
                if (!hitCollider.CompareTag("Enemy"))
                    continue;

                var enemyHealth = hitCollider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();

                    if (!enemyHealth.IsAlive)
                    {
                        var enemyController = hitCollider.GetComponent<EnemyController>();
                        if (enemyController != null)
                            Platformer.Core.Simulation.Schedule<Platformer.Gameplay.EnemyDeath>().enemy = enemyController;
                    }
                }
            }


            yield return new WaitForSeconds(0.01f);


            isAttack = false;
            
            rb.gravityScale = originalGravity;


        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
            

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            
            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                attackPoint.localPosition = attackPointOriginalLocalPos;
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                // Flip the x position to mirror the hitbox on the other side
                attackPoint.localPosition = new Vector3(
                    -attackPointOriginalLocalPos.x,
                    attackPointOriginalLocalPos.y,
                    attackPointOriginalLocalPos.z
                );
            }


            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;

        }


        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}