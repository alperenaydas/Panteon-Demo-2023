public interface IHaveHealth
{
    public int HealthPoints { get; set; }
    public void TakeDamage(int damageVal);
    public void Die();
}