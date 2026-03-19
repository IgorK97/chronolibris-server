using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{

    //Запрос для списка жалоб в панели модерации - 1

    public class GetReportsRequest
    {
        public long? LastTargetId {  get; set; }
        public long? LastTargetTypeId { get; set; }
        public long? LastReportTypeId { get; set; }
        public int Count { get; set; }
        public long? ReportStatusId { get; set; } //moderatorId если что подцепится в контроллере
        //для получения только своих взятых в обработку или обработанных
        public bool TargetTypeFilter { get; set; }
        public bool ReportTypeFilter { get; set; }
        public bool ReportStatusFilter { get; set; }
        public DateTime? LastDate { get; set; }
    }

    //Ответ на запрос списка жалоб в панели модерации
    public class GetReportsResponse
    {
        public List<ReportShortDto> Reports { get; set; }
        public bool HasNext { get; set; }
        public int Count { get; set; }
        public long LastTargetId { get; set; }
        public long LastTargetTypeId { get; set; }
        public long LastReportTypeId { get; set; }
    }

    public class ReportShortDto
    {
        public long TargetId {  get; set; }
        public long TargetTypeId { get; set; }
        public long ReasonTypeId { get; set; }
        public int Count { get; set; }
        public DateTime FirstReportDate { get; set; }
        public DateTime LastReportDate { get; set; }
        public long? ModerationTaskId { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public DateTime? TaskResolvedAt { get; set; }
        public long? TaskStatusId { get; set; }
    }

    //Запрос информации о таргете - 2

    public class GetTargetInfoRequest
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
    }

    //Ответ на запрос информации о таргете

    public class GetTargetInfoResponse
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public string? Text { get; set; }
        public long? ReaderId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookDescription { get; set; }
    }

    //Запрос списка жалоб конкретного типа на конкретный таргет - 3

    public class GetTargetReportsRequest
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public long ReasonTypeId { get; set; }
        public int Count { get; set; }
        public long? LastReportId { get; set; }

    }
    //Ответ на запрос получения списка жалоб конкретного типа на конкретный таргет

    public class GetTargetReportsResponse
    {
        public List<ReportDto> Reports { get; set; }
        public bool HasNext { get; set; }
        public int Count { get; set; }
        public long LastReportId { get; set; }
    }

    public class ReportDto
    {
        public long Id { get; set; }
        public long ReporterId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    //Запрос на взятие в обработку таска - 4

    public class CreateModerationTaskRequest
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public long ReportTypeId { get; set; }
        //Модератора контроллер подцепит из куков или токена
    }

    //Ответ на запрос на взятие таска в обработку

    public class CreateModerationTaskResponse
    {
        public long? Id { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public long TaskStatusId { get; set; }
    }

    //Запрос на принятие жалобы - 5

    public class TaskResolutionRequest
    {
        //public long Id { get; set; }
        public bool Resolution { get; set; }
        //Модератора контроллер сам подцепит из куков или токена
    }

    //Ответ на запрос по принятию жалобы
    public class TaskResolutionResponse
    {
        public bool Success { get; set; }
        public DateTime? TaskResolvedAt { get; set; }
        public long? TaskStatusId { get; set; }
    }

    //Запрос на отклонение жалобы - 6
    //То же, что и выше, только резолюшин = фалсе

    //Ответ на запрос по отклонению жалобы
    //То же, что и выше, только значения полей другие

}
