using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockState : ICharacterState
{
    private bool isBlocking = false;

    public void Enter(Character character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
        isBlocking = true;
    }

    public void Update(Character character)
    {
        // Nếu có shield hoặc armor, giữ block tự động (không cần input)
        if (character.HasShield || character.HasArmor)
        {
            // Giữ trạng thái block
            Animator animator = character.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("IdleBlock", true);
            }
        }
        else
        {
            // Nếu không có shield/armor, kiểm tra input từ player (right mouse button)
            if (Input.GetMouseButtonUp(1))
            {
                character.GetComponent<Animator>().SetBool("IdleBlock", false);
                character.ChangeState(new IdleState());
            }
            else if (Input.GetMouseButton(1))
            {
                character.GetComponent<Animator>().SetBool("IdleBlock", true);
            }
        }
    }

    public void Exit(Character character)
    {
        isBlocking = false;
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IdleBlock", false);
        }
    }
}