
namespace ShopSphere.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        //public ICollection<CartItem> Items { get; set; }
    }
}
