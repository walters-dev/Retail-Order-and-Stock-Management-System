using RetailOrderStockManagementSystem.Backend.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RetailOrderStockManagementSystem.Backend.DTOs
{
    public class LogInRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Users.UserTypes Type { get; set; }
    }
}
