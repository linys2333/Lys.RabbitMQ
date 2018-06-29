using Lys.Service.Common;
using Microsoft.AspNetCore.Identity;
using System;

namespace Lys.Service.Models.Passport
{
    public class TestUser : IdentityUser
    {
        public TestUser()
        {
            Id = SequentialGuid.NewGuidString();
            CreatorId = Guid.Empty.ToString();
            UpdaterId = Guid.Empty.ToString();
            Created = Updated = DateTime.Now;
        }
        
        public string FirmId { get; set; }
        
        public string CreatorId { get; set; }

        public DateTime Created { get; set; }
        
        public string UpdaterId { get; set; }

        public DateTime Updated { get; set; }
    }
}
