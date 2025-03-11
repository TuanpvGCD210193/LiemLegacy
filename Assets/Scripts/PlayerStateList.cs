using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX, recoilingY;
    public bool lookingRight;
    public bool invincible; //khong chi dinh khi player nhan dmg
    public bool healing;
    public bool casting;
    public bool cutscene = false;
}
