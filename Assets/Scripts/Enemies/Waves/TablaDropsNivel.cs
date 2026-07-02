using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TablaDropsNivel", menuName = "Enemigos/Tabla de Drops de Nivel")]
public class TablaDropsNivel : ScriptableObject
{
    [Tooltip("Items que pueden soltar los enemigos de este nivel. Cada uno con su probabilidad (0-100%).")]
    public List<LifeEnemy.PosibleDrop> drops = new List<LifeEnemy.PosibleDrop>();

    [Tooltip("Máximo de items que puede soltar un enemigo al morir. 0 = sin límite (cada item se tira por separado).")]
    public int maxItemsPorMuerte = 1;

    [Tooltip("Radio (en unidades) alrededor del enemigo donde aparecen los items soltados.")]
    public float radioDrop = 0.6f;
}
