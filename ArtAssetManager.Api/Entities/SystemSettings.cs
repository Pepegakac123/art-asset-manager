using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ArtAssetManager.Api.Entities
{
    // Tabela konfiguracyjna Key-Value dla ustawień globalnych aplikacji
    // Np. klucz "Scanner_AllowedExtensions" przechowuje listę ".jpg;.png;.obj"
    public class SystemSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Klucz ustawiany ręcznie (string), brak auto-increment
        public string Key { get; set; } = null!;
        public string Value { get; set; } = string.Empty;
    }
}
