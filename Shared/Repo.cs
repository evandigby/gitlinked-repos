using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class Repo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Url { get; set; }
        public int StargazersCount { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
