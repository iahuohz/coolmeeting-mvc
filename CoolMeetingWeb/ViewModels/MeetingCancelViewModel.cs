using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoolMeetingWeb.ViewModels
{
    public class MeetingCancelViewModel
    {
        public int MeetingID { get; set; }
        public string MeetingName { get; set; }

        [Required(ErrorMessage="*")]
        [StringLength(400)]
        public string Reason { get; set; }

        public bool CanCancel { get; set; }
    }
}