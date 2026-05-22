using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    private Movement movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        animator.SetBool("IsJumping", movement.IsJumping);
        animator.SetBool("IsRolling", movement.IsRolling);
        animator.SetBool("IsFlying", movement.IsFlying);
    }
}
