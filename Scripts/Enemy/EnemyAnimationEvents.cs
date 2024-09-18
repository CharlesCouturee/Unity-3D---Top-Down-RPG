using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void AnimationTrigger() => enemy.AnimationTrigger();

    private void StartManualMovement() => enemy.ActivateManualMovement(true);
    private void StopManualMovement() => enemy.ActivateManualMovement(false);
    private void StartManualRotation() => enemy.ActivateManualRotation(true);
    private void StopManualRotation() => enemy.ActivateManualRotation(false);
}
