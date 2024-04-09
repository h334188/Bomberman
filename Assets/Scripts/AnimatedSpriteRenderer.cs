using UnityEngine;

public class AnimatedSpriteRenderer : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite[] animationSprites;

    public float animationTime = .25f;
    private int animationFrame;

    public bool loop = true;
    public bool idle = true;// 空闲

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        spriteRenderer.enabled = true;
    }

    private void OnDisable() {
        spriteRenderer.enabled = false;
    }

    private void Start() {
        // 每0.25s会前进到下一帧
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
    }

    private void NextFrame() {
        animationFrame++;

        if (loop && animationFrame >= animationSprites.Length) {
            animationFrame = 0;
        }

        if (idle) {
            spriteRenderer.sprite = idleSprite;
        } else if (animationFrame >= 0 && animationFrame < animationSprites.Length) {
            spriteRenderer.sprite = animationSprites[animationFrame];
        }
    }
}
