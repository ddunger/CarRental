namespace CarRental.Web.Dtos.Manufacturers
{
    public record ManufacturerResponse(int Id, string Name);
    public record CreateManufacturerRequest(string Name);
    public record UpdateManufacturerRequest(string Name);
}
