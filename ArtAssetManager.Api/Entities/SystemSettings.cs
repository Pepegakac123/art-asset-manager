using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtAssetManager.Api.Entities
{
    public class SystemSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Wpisywanie klucza ręcznie
        public string Key { get; set; } = null!;
        public string Value { get; set; } = string.Empty;
    }
}