public interface IHealth
{
    void TakeDamage(float damage);
    float CurrentHealth { get; }
    float MaxHealth { get; }
}