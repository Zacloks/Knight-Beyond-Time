/*
Abstrae el ataque de un enemigo para que MovementEnemy y Enemy no dependan de
si es cuerpo a cuerpo (AttackEnemy) o a distancia (AttackEnemyRanged).
*/
public interface IEnemyAttack
{
    float AttackReach { get; }
    int AttackDamage { get; }
}
