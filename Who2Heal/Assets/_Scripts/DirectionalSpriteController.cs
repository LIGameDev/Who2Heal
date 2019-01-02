using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DirectionalSprite
{
    public Sprite sprite;
    public float yAngle; 
}

public class DirectionalSpriteController : MonoBehaviour {

    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] bool castShadows = true;
    [SerializeField] bool receiveShadows = true;

    [SerializeField] UnitMover unitMover;
    [SerializeField] GameSceneManager gameSceneManager; // get camera rotation from here for now

    [SerializeField] List<DirectionalSprite> sprites;

    Quaternion initialRotation;
    float lastUnitMoverRotationY; 

    private void Start()
    {
        if (castShadows)
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        spriteRenderer.receiveShadows = receiveShadows;

        initialRotation = transform.rotation;
        lastUnitMoverRotationY = 0f; 

        unitMover.OnRotationChanged += UnitMover_OnRotationChanged;
        gameSceneManager.OnCameraRotationChanged += GameSceneManager_OnCameraRotationChanged;
    }
    
    protected virtual void OnEnable()
    {
        if (unitMover != null)
            unitMover.OnRotationChanged += UnitMover_OnRotationChanged;
    }

    protected virtual void OnDisable()
    {
        if (unitMover != null)
            unitMover.OnRotationChanged -= UnitMover_OnRotationChanged;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(gameSceneManager.CameraYAngle, Vector3.up) * initialRotation; 
    }

    private void UnitMover_OnRotationChanged(Quaternion newRot)
    {
        SetDirection(newRot);
    }

    private void GameSceneManager_OnCameraRotationChanged()
    {
        SetDirection(Quaternion.AngleAxis(lastUnitMoverRotationY, Vector3.up)); 
    }

    public void SetDirection(Quaternion rot)
    {
        lastUnitMoverRotationY = rot.eulerAngles.y; 

        if (sprites.Count == 0)
            return;

        // Find directional sprite with angle closest to target
        float targetYAngle = rot.eulerAngles.y - gameSceneManager.CameraYAngle;
        DirectionalSprite closestDirSpr = null;
        float closestDiff = -1f; 

        for (int i = 0; i < sprites.Count; i++)
        {
            DirectionalSprite spr = sprites[i];
            float diff = Mathf.DeltaAngle(spr.yAngle, targetYAngle); 

            if (i == 0 || Mathf.Abs(diff) < Mathf.Abs(closestDiff))
            {
                closestDirSpr = spr;
                closestDiff = diff; 
            }
        }

        // Set sprite accordingly
        spriteRenderer.sprite = closestDirSpr.sprite;

        //Debug.Log($"[DirectionalSpriteController] target = {targetYAngle}, closest = {closestDirSpr.yAngle}, diff = {closestDiff}"); 
    }
	
}
