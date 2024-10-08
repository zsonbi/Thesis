using Game;
using UnityEngine;

/// <summary>
/// Script which follows the player's car
/// </summary>
public class CameraScript : ThreadSafeMonoBehaviour
{
    /// <summary>
    /// Reference to the game's controller
    /// </summary>
    [SerializeField]
    private GameController gameController;

    /// <summary>
    /// Where should the camera be
    /// </summary>
    private Vector3 CameraOffset;

    //Runs before first frame
    private void Start()
    {
        //Set the offset
        this.CameraOffset = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        //Move the camera
        if (this.gameController.Running)
            this.transform.position = new Vector3(CameraOffset.x + gameController.PlayerPos.x, CameraOffset.y, CameraOffset.z + gameController.PlayerPos.z);
    }
}