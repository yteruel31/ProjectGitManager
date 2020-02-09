using System;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class GitlabAssigneeVO : ObservableObject
    {
        public int Id { get; set; }

        public string State { get; set; }

        public string AvatarUrl { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }
    }
}