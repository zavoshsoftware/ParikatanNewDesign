using SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Parikatan.Web.ViewModels;
using RestSharp;

namespace Helpers
{
    public class SendSms
    {
        static string apiKey = "hawWB4ASZdW4kmRXlZSrmEPbLrGq9nry0NojCMItTZgYo02apFontNvdYBIJcvYG";
        static string username = "alibabaei";
        static string line = "30007487129277";

        //Sms.ir V2
        public static void SendOtp(string cellNumber, string code)
        {
            var client = new RestClient("https://api.sms.ir/v1/send/verify");

            List<UserParameter> parameters = new List<UserParameter>()
            {
                new UserParameter()
                {
                    name = "VERIFYCODE",
                    value = code
                }
            };

            SMSIRViewModel res = new SMSIRViewModel()
            {
                mobile = cellNumber,
                templateId = 586082,
                parameters = parameters
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Accept", "text/plain");
            request.AddHeader("x-api-key", apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(res);
            IRestResponse response = client.Execute(request);

            var result = response.Content;
            SmsIrResult token = Newtonsoft.Json.JsonConvert.DeserializeObject<SmsIrResult>(result);
        }

        public  static void SendCommonSmsV2(string cellNumber, string message)
        {


            var client = new RestClient("https://api.sms.ir/v1/send");

            // ایجاد یک درخواست از نوع GET
            var request = new RestRequest(Method.GET);

            // اضافه کردن پارامترها به درخواست
            request.AddParameter("username", username);
            request.AddParameter("password", apiKey);
            request.AddParameter("mobile", cellNumber);
            request.AddParameter("line", line);
            request.AddParameter("text", message);

            // ارسال درخواست و دریافت پاسخ
            IRestResponse response = client.Execute(request);
        }

        public static void SendThanksSmsV2(string cellNumber, string name)
        {
            var client = new RestClient("https://api.sms.ir/v1/send/verify");

            List<UserParameter> parameters = new List<UserParameter>()
            {
                new UserParameter()
                {
                    name = "NAME",
                    value = name
                }
            };

            SMSIRViewModel res = new SMSIRViewModel()
            {
                mobile = cellNumber,
                templateId = 149399,
                parameters = parameters
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Accept", "text/plain");
            request.AddHeader("x-api-key", apiKey);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(res);
            IRestResponse response = client.Execute(request);

            var result = response.Content;
            SmsIrResult token = Newtonsoft.Json.JsonConvert.DeserializeObject<SmsIrResult>(result);
        }


        #region OldSMSIRMethods
        //public static void SendThanksSms(string cellNumber, string name)
        //{
        //    var token = new Token().GetToken("3857b1aa8e206d361e71b1aa", "123qwe!@#QWE");

        //    var ultraFastSend = new UltraFastSend()
        //    {
        //        Mobile = Convert.ToInt64(cellNumber),
        //        TemplateId = 80342,
        //        ParameterArray = new List<UltraFastParameters>()
        //        {
        //            new UltraFastParameters()
        //            {
        //                Parameter = "name" , ParameterValue = name
        //            }
        //        }.ToArray()

        //    };

        //    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

        //    if (ultraFastSendRespone.IsSuccessful)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        //public static void SendAds(string cellNumber, string name)
        //{
        //    var token = new Token().GetToken("3857b1aa8e206d361e71b1aa", "123qwe!@#QWE");

        //    var ultraFastSend = new UltraFastSend()
        //    {
        //        Mobile = Convert.ToInt64(cellNumber),
        //        TemplateId = 80407,
        //        ParameterArray = new List<UltraFastParameters>()
        //        {
        //            new UltraFastParameters()
        //            {
        //                Parameter = "name" , ParameterValue = name
        //            }
        //        }.ToArray()

        //    };

        //    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

        //    if (ultraFastSendRespone.IsSuccessful)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        //public static void SendCommonSms(string cellNumber, string message)
        //{
        //    var token = new Token().GetToken("3857b1aa8e206d361e71b1aa", "123qwe!@#QWE");

        //    SmsIrRestful.MessageSend messageSend = new SmsIrRestful.MessageSend();

        //    var res = messageSend.Send(token, new SmsIrRestful.MessageSendObject()
        //    {
        //        MobileNumbers = new List<string>() { cellNumber }.ToArray(),
        //        Messages = new List<string>() { message }.ToArray(),
        //        LineNumber = "30004747475709",
        //        SendDateTime = null,
        //        CanContinueInCaseOfError = false
        //    });

        //    SmsIrRestful.Credit credit = new SmsIrRestful.Credit();
        //    SmsIrRestful.CreditResponse creditResponse = new SmsIrRestful.CreditResponse();

        //}

        //public static void SendOtpSms(string cellNumber, string code)
        //{
        //    var token = new Token().GetToken("3857b1aa8e206d361e71b1aa", "123qwe!@#QWE");

        //    var ultraFastSend = new UltraFastSend()
        //    {
        //        Mobile = Convert.ToInt64(cellNumber),
        //        TemplateId = 80305,
        //        ParameterArray = new List<UltraFastParameters>()
        //        {
        //            new UltraFastParameters()
        //            {
        //                Parameter = "verifyCode" , ParameterValue = code
        //            }
        //        }.ToArray()

        //    };

        //    UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

        //    if (ultraFastSendRespone.IsSuccessful)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        #endregion
    }


}