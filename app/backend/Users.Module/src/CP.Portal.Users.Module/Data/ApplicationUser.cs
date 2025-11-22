using Microsoft.AspNetCore.Identity;

namespace CP.Portal.Users.Module.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    private readonly List<CartMovie> _cartItems = new();
    public IReadOnlyCollection<CartMovie> CartItems => _cartItems.AsReadOnly();

    public void AddItemToCart(CartMovie item)
    {
      
        var existingBook = _cartItems.SingleOrDefault(c => c.MovieId == item.MovieId);
        if (existingBook != null)
        {
            existingBook.UpdateQuantity(existingBook.Quantity + item.Quantity);
            existingBook.UpdateDescription(item.Description);
            existingBook.UpdateUnitPrice(item.UnitPrice);
            return;
        }
        _cartItems.Add(item);
    }

}
