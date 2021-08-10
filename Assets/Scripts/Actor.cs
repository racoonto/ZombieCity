using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int hp = 100;
    [HideInInspector] public int maxHp;
    public float bloodEffectYPosition = 1.3f;

    public GameObject bloodParticle;
    protected Animator animator;

    protected void Awake()
    {
        maxHp = hp;
    }

    protected void CreateBloodEffect()
    {
        var pos = transform.position;
        pos.y = bloodEffectYPosition;
        Instantiate(bloodParticle, pos, Quaternion.identity);
    }

    public static void CreateTextEffect(int number, Vector3 position, Color color)
    {
        CreateTextEffect(number.ToNumber(), "TextEffect", position, color);
    }

    public static void CreateTextEffect(string str, string prefabName, Vector3 position
        , Color color, Transform parent = null)
    {
        GameObject memoryGo = (GameObject)Resources.Load(prefabName);
        GameObject go = Instantiate(memoryGo, position, Camera.main.transform.rotation);
        if (parent)
            go.transform.parent = parent;
        TextMeshPro textMeshPro = go.GetComponentInChildren<TextMeshPro>();
        textMeshPro.text = str;
        textMeshPro.color = color;
    }

    public Color damageColor = Color.white;

    protected void TakeHit(int damage)
    {
        hp -= damage;
        CreateBloodEffect();
        CreateTextEffect(damage, transform.position, damageColor);
    }
}