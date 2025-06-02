using UnityEngine;

public class InputManager : NghiaMono
{
    [SerializeField] protected GemBoardCtr gemBoardCtr;
    protected Vector2 firstPressPos;
    protected Vector2 finalPressPos;
    protected float swipeAngle;
    private Vector2 touchStart;
    private float swipeThreshold = 50f;
    private GemCtr currentGem;

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemBoardCtr();
    }
    protected virtual void LoadGemBoardCtr()
    {
        if (this.gemBoardCtr != null) return;
        this.gemBoardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemBoardCtr", gameObject);
    }
    
     private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<GemCtr>())
            {
                if (gemBoardCtr.GemSwaper.IsProccessingMove) return;
                currentGem = hit.collider.gameObject.GetComponent<GemCtr>();
                Debug.Log("da cham vao" + currentGem.name);
                this.gemBoardCtr.GemSwaper.SelectGem(currentGem);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - touchStart;
            
            if (swipeDelta.magnitude > swipeThreshold)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchStart);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.gameObject.GetComponent<GemCtr>())
                {
                    
                    
                    // Determine swipe direction
                    float angle = Mathf.Atan2(swipeDelta.y, swipeDelta.x) * Mathf.Rad2Deg;
                    
                    // Find adjacent gem based on swipe direction
                    Vector2Int direction = GetSwipeDirection(angle);
                    int targetX = currentGem.xIndex + direction.x;
                    int targetY = currentGem.yIndex + direction.y;
                    
                    // Check if target position is valid and gem exists
                    if (targetX >= 0 && targetX < gemBoardCtr.Gemboard.width && 
                        targetY >= 0 && targetY < gemBoardCtr.Gemboard.height &&
                        gemBoardCtr.Gemboard.gemBoardNode[targetX, targetY].Gem != null)
                    {
                      GemCtr targetGem = gemBoardCtr.Gemboard.gemBoardNode[targetX, targetY].Gem.GetComponent<GemCtr>();
                      this.gemBoardCtr.GemSwaper.SelectGem(targetGem);
                    }
                }
            }
        }
    }

    private Vector2Int GetSwipeDirection(float angle)
    {
        // Convert angle to 0-360 range
        if (angle < 0) angle += 360;
        
        // Define direction based on angle
        if (angle >= 315 || angle < 45) return new Vector2Int(1, 0);  // Right
        if (angle >= 45 && angle < 135) return new Vector2Int(0, 1);  // Up
        if (angle >= 135 && angle < 225) return new Vector2Int(-1, 0); // Left
        if (angle >= 225 && angle < 315) return new Vector2Int(0, -1); // Down
        return new Vector2Int(0, 0);
    }
}
