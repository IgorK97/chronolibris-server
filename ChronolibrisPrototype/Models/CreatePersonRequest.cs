namespace ChronolibrisPrototype.Models
{
    public class CreatePersonRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        // Само изображение в формате Base64
        //public string? ImageBase64 { get; set; }

        //// Имя файла (например, "avatar.jpg"), чтобы знать расширение
        //public string? FileName { get; set; }
    }
}
