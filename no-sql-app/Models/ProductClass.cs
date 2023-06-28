using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class Product : ProductInputModel
{
    [JsonProperty("id")]
    public string Id { get; set; }
}

public class ProductInputModel
{
    [Required]
    [JsonProperty("name")]
    public string Name { get; set; }

    [Required]
    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}