using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record TagsInputModel([Required]
        [CommaSeparatedLongs(MaxCount =100)]
        string Ids)
    {
        public List<long> ParsedIds => Ids.Split(',',
            StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse).ToList(); //либо можно написать StringToLongListConverter : TypeConverter
    }
}
