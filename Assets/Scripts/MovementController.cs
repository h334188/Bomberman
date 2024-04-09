using System.Collections;
using UnityEditor;
using UnityEngine;

public class MovementController : MonoBehaviour {

    public new Rigidbody2D rigidbody {  get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;

    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    // 可以简单作为初始化
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;
    }

    // 更新频率取决于游戏帧率
    private void Update() {
        // GetKey检测是否一直按住，GetKeyDown只会在按下的第一帧检测并返回bool
        if (Input.GetKey(inputUp)) {
            SetDirection(Vector2.up, spriteRendererUp);
        } else if (Input.GetKey(inputDown)) {
            SetDirection(Vector2.down, spriteRendererDown);
        } else if (Input.GetKey(inputLeft)) {
            SetDirection(Vector2.left, spriteRendererLeft);
        } else if (Input.GetKey(inputRight)) {
            SetDirection(Vector2.right, spriteRendererRight);
        } else {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }
    }

    private void FixedUpdate() {
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;

        rigidbody.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer) {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion")) {
            DeathSequence();
        }
    }

    private void DeathSequence() {
        enabled = false;
        StartCoroutine(DisableAfterAllExplode(GetComponent<BombController>()));

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        StartCoroutine(OnDeathSequenceEnded(GetComponent<BombController>()));
    }

    private IEnumerator DisableAfterAllExplode(BombController bombController) {
        yield return new WaitUntil(() => bombController.bombAmount == bombController.bombsRemaining);
        bombController.enabled = false;
    }

    private IEnumerator OnDeathSequenceEnded(BombController bombController) {
        yield return new WaitForSeconds(1.25f);

        transform.localScale = Vector3.zero;// 隐藏人物
        FindObjectOfType<GameManager>().CheckWinState();

        yield return new WaitUntil(() => bombController.bombAmount == bombController.bombsRemaining);
        // 最后调用，否则炸弹无法消失
        gameObject.SetActive(false);
    }
}
