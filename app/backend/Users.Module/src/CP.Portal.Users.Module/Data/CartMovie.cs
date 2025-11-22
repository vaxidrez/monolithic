namespace CP.Portal.Users.Module.Data;

public class CartMovie
{
    public CartMovie(Guid movieId, string description, int quantity, decimal unitPrice)
    {
        MovieId = movieId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public CartMovie()
    {
        // EF 
    }
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid MovieId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    internal void UpdateDescription(string description)
    {
        Description = description;
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
    }
}