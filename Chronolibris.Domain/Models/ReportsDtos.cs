using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{

    public class GetReportsRequest
    {
        public long? LastTargetId {  get; set; }
        public long? LastTargetTypeId { get; set; }
        public long? LastReportTypeId { get; set; }
        public int Count { get; set; }
        public long? ReportStatusId { get; set; }
        public bool TargetTypeFilter { get; set; }
        public bool ReportTypeFilter { get; set; }
        public bool ReportStatusFilter { get; set; }
        public DateTime? LastDate { get; set; }
    }
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
        public string Comment { get; set; } = string.Empty;
    }

    //public class GetTargetInfoRequest
    //{
    //    public long TargetId { get; set; }
    //    public long TargetTypeId { get; set; }
    //}

    public class GetTargetInfoResponse
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public string? Text { get; set; }
        public long? ReaderId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookDescription { get; set; }
        public required long BookId { get; set; }
        public string? ParentCommentText { get; set; }
    }

    public class GetTargetReportsRequest
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public long ReasonTypeId { get; set; }
        public int Count { get; set; }
        public long? LastReportId { get; set; }

    }

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
    public class CreateModerationTaskRequest
    {
        public long TargetId { get; set; }
        public long TargetTypeId { get; set; }
        public long ReportTypeId { get; set; }
    }
    public class CreateModerationTaskResponse
    {
        public long? Id { get; set; }
        public DateTime? TaskCreatedAt { get; set; }
        public long TaskStatusId { get; set; }
    }

    public class TaskResolutionRequest
    {
        //public long Id { get; set; }
        public bool Resolution { get; set; }
        [MinLength(20)]
        [MaxLength(2000)]
        public required string Comment { get; set; }
    }
    public class TaskResolutionResponse
    {
        public bool Success { get; set; }
        public DateTime? TaskResolvedAt { get; set; }
        public long? TaskStatusId { get; set; }
    }

}
