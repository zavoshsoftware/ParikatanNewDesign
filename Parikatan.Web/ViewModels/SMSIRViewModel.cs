using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parikatan.Web.ViewModels
{
    public class SMSIRViewModel
    {
        public string mobile { get; set; }
        public int templateId { get; set; }
        public List<UserParameter> parameters { get; set; }
    }

    public class UserParameter
    {
        public string name { get; set; }
        public string value { get; set; }
    }


    public class SmsIrResult
    {
        public int status { get; set; }
        public string message { get; set; }
        public SmsIrResultData data { get; set; }
    }

    public class SmsIrResultData
    {
        public int messageId { get; set; }
        public float cost { get; set; }
    }


    public class CommonSmsInput
    {
        public long lineNumber { get; set; }
        public string messageText { get; set; }
        public string[] mobiles { get; set; }
    }


    public class Rootobject
    {
        public int status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string packId { get; set; }
        public int[] messageIds { get; set; }
        public float cost { get; set; }
    }

}