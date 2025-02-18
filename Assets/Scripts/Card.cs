using UnityEngine;

public class CircularScroll : MonoBehaviour
{
    [SerializeField, Header("カード")]
    private GameObject[] cards;
    [SerializeField, Header("半径")]
    private float radius = 5f;
    [SerializeField, Header("カーブの方向")]
    private bool isConvex = true;
    [SerializeField, Header("無限スクロール")]
    private bool infiniteScroll = false;
    [SerializeField, Header("回転速度")]
    private float rotationSpeed = 50f;

    private float currentAngle = 0f;

    private void Start()
    {
        ArrangeCards();
    }

    private void Update()
    {
        if (infiniteScroll)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            ArrangeCards();
        }
    }

    // カードを円形に配置
    private void ArrangeCards()
    {
        float angleStep = 360f / cards.Length;
        for (int i = 0; i < cards.Length; i++)
        {
            float angle = currentAngle + i * angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;

            float zPosition = isConvex ? Mathf.Cos(angleInRadians) * radius : -Mathf.Cos(angleInRadians) * radius;
            float xPosition = Mathf.Sin(angleInRadians) * radius;

            Vector3 newPosition = new Vector3(xPosition, 0, zPosition);
            cards[i].transform.localPosition = newPosition;

            

            cards[i].transform.LookAt(transform.position);

            cards[i].transform.Rotate(0, 0, 180);
        }
    }
}
