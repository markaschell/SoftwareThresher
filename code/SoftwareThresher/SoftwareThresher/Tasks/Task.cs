﻿using System.Collections.Generic;
using SoftwareThresher.Observations;

namespace SoftwareThresher.Tasks
{
    public interface Task
    {
        string ReportTitleForErrors { get; }

        List<Observation> Execute(List<Observation> observations);
    }
}
