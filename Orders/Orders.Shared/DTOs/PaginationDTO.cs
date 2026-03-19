namespace Orders.Shared.DTOs;

public class PaginationDTO
{
    public int Id { get; set; } //Id de la entidad a consultar
    public int Page { get; set; } = 1; //Pagina que se muestra los registros
    public int RecordsNumber { get; set; } = 10; //Cantidad de registros x página
    public string? Filter { get; set; } //Filtro por nombre
}