using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CoolMeetingWeb.Models;
using CoolMeetingWeb.ViewModels;

namespace CoolMeetingWeb.ViewModels
{
    public enum MeetingListTypeDef
    {
        Canceled = -1,
        Participated = 0,
        Reserved = 1
    }

    public class MeetingListViewModel
    {
        public MeetingListTypeDef MeetingListType { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
}