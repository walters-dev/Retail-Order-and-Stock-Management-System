using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class ApiErrorResponseDto
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public ModelStateDictionary Details { get; set; }
    }
}
