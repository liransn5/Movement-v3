using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataRetriever.Services.Repositories;

[Table("DataItems")]
public class DataItem
{
    [Key]
    public string Id { get; set; }

    public string Value { get; set; }
}
