public interface ICanDamage
{
    public int DamagePoints { get; set; }
    public void InflictDamage(int damageVal, IHaveHealth target);
    public float AttackRate { get; set; }
}