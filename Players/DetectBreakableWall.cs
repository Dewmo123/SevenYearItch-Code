using UnityEngine;

namespace AKH.Scripts.Players
{
    public class DetectBreakableWall : MonoBehaviour, IPlayerComponent
    {
        [SerializeField] private Transform detectPos;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask breakableLayer;
        public void Init(Player player)
        {
        }
        public bool DetectWall()
        {
            Collider2D wall = Physics2D.OverlapCircle(detectPos.position, radius, breakableLayer);
            if (wall)
                Destroy(wall.gameObject);
            return wall;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectPos.position, radius);
        }
    }
}
