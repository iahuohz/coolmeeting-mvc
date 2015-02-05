using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CoolMeetingWeb.Models;

namespace CoolMeetingWeb.ViewModels
{
    public class MeetingsInRoomViewModel
    {
        public Room MeetingRoom { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
}