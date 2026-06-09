using System.ComponentModel.DataAnnotations;

namespace EHandelApi.Models.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}