using UnityEngine;
using UnityEngine.EventSystems;

public class DefenseActivator : MonoBehaviour, IPointerClickHandler
{
    public GameObject ShieldPrefab;
    public float CooldownRemaining { get { return m_coolDownRemaining; } }

    private float m_coolDownRemaining = 0;

    void Start ()
    {
        GameManager.Instance.NotifyStateChange += OnStateChange;
	}

    void Update()
    {
        if (m_coolDownRemaining > 0)
        {
            m_coolDownRemaining = Mathf.Max(m_coolDownRemaining - Time.deltaTime, 0);
        }
    }

    private void OnStateChange(GameManager.GameState old_state, GameManager.GameState new_state)
    {
        //Make sure this object is only active during the movement phase.
        gameObject.SetActive(new_state == GameManager.GameState.Moving);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_coolDownRemaining <= 0)
        {
            //Instantiate a new shield object, and attach it to the player object.
            GameObject shield = Instantiate(ShieldPrefab, transform.position, Quaternion.identity);
            shield.transform.parent = transform.parent;

            shield.GetComponent<Shield>().ProtectedObject = transform.parent.GetComponent<Destructible>();

            m_coolDownRemaining = shield.GetComponent<Shield>().Cooldown;
        }
        
    }
}
