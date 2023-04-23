using CustomTools;
using Models;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCamera;
    public Transform viewPoint;

    public GameObject player;

    private void LateUpdate()
    {
        if (player == null && User.Instance.CurrentCharacterObject != null) player = User.Instance.CurrentCharacterObject;

        if (player == null) return;

        transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
    }
}