using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class TagDetails
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required long TagTypeId { get; set; }
        public string? TagTypeName { get; set; }

        public long? ParentTagId { get; set; }
        public string? ParentTagName { get; set; }
        public long? RelationTypeId { get; set; }
        public string? RelationTypeName { get; set; }
        public bool HasChildren { get; set; }
    }

    //public class TagRelationTypeDto
    //{
    //    public long Id { get; set; }
    //    public required string Name { get; set; }
    //    public string? Description { get; set; }
    //}
}
