using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CoolMeetingWeb.Models
{
    public class TermDescription
    {
        private static Dictionary<int, string> genderTypes;
        private static Dictionary<int, string> roomStatusTypes;

        public static Dictionary<int, string> GetDataFor(string typeName)
        {
            Type t = typeof(TermDescription);
            object result = t.InvokeMember(typeName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty,
                null, null, null);
            return result as Dictionary<int, string>;
        }

        public static Dictionary<int, string> Gender
        {
            get
            {
                if (genderTypes == null)
                {
                    genderTypes = new Dictionary<int, string>();
                    genderTypes.Add((int)GenderType.Female, "女");
                    genderTypes.Add((int)GenderType.Male, "男");
                }
                return genderTypes;
            }
        }

        public static Dictionary<int, string> RoomStatus
        {
            get
            {
                if (roomStatusTypes == null)
                {
                    roomStatusTypes = new Dictionary<int, string>();
                    roomStatusTypes.Add((int)RoomStatusType.Retired, "已删除");
                    roomStatusTypes.Add((int)RoomStatusType.Maintained, "维护中");
                    roomStatusTypes.Add((int)RoomStatusType.Active, "可用");
                }
                return roomStatusTypes;
            }
        }
    }
}