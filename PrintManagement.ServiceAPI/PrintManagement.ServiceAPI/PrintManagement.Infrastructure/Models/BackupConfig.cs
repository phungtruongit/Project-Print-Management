﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PrintManagement.Infrastructure
{
    public partial class BackupConfig
    {
        public BackupConfig()
        {
            SystemConfigOptions = new HashSet<SystemConfigOption>();
        }

        public Guid Oid { get; set; }
        public string BackupLocation { get; set; }
        public int? BackupSchedule { get; set; }
        public int? DeleteLogSchedule { get; set; }

        public virtual ICollection<SystemConfigOption> SystemConfigOptions { get; set; }
    }
}