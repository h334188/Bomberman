using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour {

    [Header("Bomb")]
    public KeyCode inputKey = KeyCode.Space;
    public Bomb bombPrefab;
    public LayerMask bombLayerMask;
    public float bombFuseTime = 3f;// 爆炸时间
    public int bombAmount = 1;// 可放下的炸弹数量
    public int bombsRemaining { get; private set; }// 剩余炸弹

    [Header("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    [Header("Destructible")]
    public Tilemap destructibleTilemaps;
    public Destructible destructiblePrefab;

    private void OnEnable() {
        bombsRemaining = bombAmount;
    }

    private void Update() {
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey)) {
            StartCoroutine(PlaceBomb());
        }
    }

    private IEnumerator PlaceBomb() {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        // 旋转约等于无
        Bomb bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        bombsRemaining--;

        //yield return new WaitForSeconds(bombFuseTime);
        StartCoroutine(BombTimeOut(bomb));
        yield return new WaitUntil(() => bomb.beBombed);

        position = bomb.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        Destroy(bomb.gameObject);
        bombsRemaining++;
    }

    private IEnumerator BombTimeOut(Bomb bomb) {
        yield return new WaitForSeconds(bombFuseTime);
        bomb.beBombed = true;
    }

    private void Explode(Vector2 position, Vector2 direction, int length) {
        if (length <= 0) {
            return;
        }

        position += direction;

        // 控制爆炸不会超出边界（并检测某图层 在当前位置的 某范围内 旋转0度，是否有物体）
        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask)) {
            ClearDestructible(position);
            return;
        }

        // 若检测到其他炸弹，则连续引爆
        if (Physics2D.OverlapBox(position, Vector2.one, 0f, bombLayerMask)) {
            Bomb bomb = Physics2D.OverlapBox(position, Vector2.one, 0f, bombLayerMask).GetComponent<Bomb>();
            bomb.beBombed = true;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, direction, length - 1);
    }

    private void ClearDestructible(Vector2 position) {
        // 把该位置的世界坐标转为cell坐标，如果包含在可破坏物体map内则处理
        Vector3Int cell = destructibleTilemaps.WorldToCell(position);
        TileBase tile = destructibleTilemaps.GetTile(cell);

        if (tile != null) {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTilemaps.SetTile(cell, null);
        }
    }

    public void AddBomb() {
        bombAmount++;
        bombsRemaining++;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other">被玩家碰到的炸弹</param>
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb")) {
            // 开启物理碰撞效果
            other.isTrigger = false;
        }
    }

}
