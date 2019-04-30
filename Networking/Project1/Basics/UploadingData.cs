using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.IO;

namespace Project1.Basics
{
    static class UploadingData
    {
        public static void Run()
        {
            //UploadFormDataUsingWebRequest();  // Successful
            UploadFormDataUsingHttpClient();
            //Login();
        }

        static void UploadFormDataUsingHttpClient()
        {
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie
            {
                Name = ".AspNet.ApplicationCookie",
                Value = "2tTrI1oLWDx4ZVLO4Iz819sz1TxfwmTR_91-msA74_DEg50Xl48LWPX9HfrROpMEG2JYJLDKJElqOPsqMSWWkIAR6p3yG3jWAO-ZcDFmSRlIbGmTEX4h1-PTPCdJCEF2D65aXiQc-VQEfyxgLmwPcxuoLM0Z6eGwrOVbJT7DMNrf74JblljhZJa3HuNtMpJWAE9FrLHlgmoncHyts-olf_t6pi70pazFXJ2rP8qxTLbVCmtMPyWAg2MDOl4G5xGXW_HtWA-zvTOQHOaViIRG9kWMTC05uxLzPoQAilTO-RgJSGKbmnvQW9OGMtLFZi17H639DFAHsHrXcKvxIT1nELmrbkeO4Wv_2NfOhJuRblhtSNljT7Phof9IzYlnFjnR9Ecri7UV72VkUBWSd7LHW_BI21lZ5EalklDh0MH5udFSW8HQ8lXeDbVWgDveiIc-3xHJ2ubw0J07gvKlic4pz3r28TE0Y2zrbZfuyvoed64RozFetbfmZjGGCX08kKnY",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });
            cookieContainer.Add(new Cookie
            {
                Name = "__RequestVerificationToken",
                //Value = "CcFoBNhb0D7FOKjUW_TTj8A594XFXm6XYMRa0fznRPN7pCbpUh0PODoNxZ8U4oI_t9kDyZXofVzemWlQtu9DjrtAivl4r5EPdgPjfDperv41",
                Value = "i_SdxXhZGH4BHlTSH8OtHGdXZwE0XKnlKWkZJvvQPi0stvvd5JwWB",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });

            HttpClientHandler handler = new HttpClientHandler
            {
                //AllowAutoRedirect = false, // allow to capture cookies
                CookieContainer = cookieContainer,
                Proxy = null,
                UseCookies = true,
            };

            HttpClient client = new HttpClient(handler);

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", "Some token....."},
                { "RegistrationNo", "LEV-9249" },
                { "VehicleMake", "Honda" },
                { "VehicleColor", "Black" },
                { "AvailableToDouble", "false" },
                { "RentPerWeek", "3000" },
                { "VehicleConditionId", "2" },
                { "VehicleTypeId", "2" },
                { "IsLivery", "true" },
                { "ProprietorId", "6" }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(data);

            HttpResponseMessage response = client.PostAsync("http://vehiclerenting.apphb.com/Account/Login", content).Result;
        }

        static void UploadFormDataUsingWebRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://vehiclerenting.apphb.com/Admin/CreateVehicle");
            request.AllowAutoRedirect = false;  // this allows to capture cookies

            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            cookieContainer.Add(new Cookie
            {
                Name = ".AspNet.ApplicationCookie",
                Value = "DbClGd0E7S1ZlhppKspHSeUFg1s9cGAzm95ng5KRuBpDa4OfKz5MVXTHTKQLrB4SET5BYFd-Ekfde0KIzLn1YB4AYbRGISwXenc60XEn1s6-0jNXe_-MnL-tuxtIeJR2BxWe000819O2DXwtiIurAP3FDsQ3pq_pmcooDwDSAzzU8cBgcaFArG_ulM52MSQnymcrQuID-aiiKcjN2PFPYCHQ8eOYu1xWPkTg70408YPwuRoCHe8N26MavJIH94aeTWsyMwwqycytsvLAyjxVgw1KCTWVgb_hexywvDtEMPexff-W8WPpGcP1s7cS7gAf2QBwe2Qr3Oru0EIA1et5RhUjRMGRiXhoWCOfjv-40sTGG6HK-Geo7HFd4XVyaqgaByPLm0cleTp0X70sRQrROZc5wo2sVrqpT1TobqlzoSh1PrBhjkbFoB8gnExWZZcM8J5hOWNtcZvZwxZ3Y6tMPedbn7MlM5qno3qi_CUh_ZCWT8BScJhLX7GSwtY2Jt_f",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });
            cookieContainer.Add(new Cookie
            {
                Name = "__RequestVerificationToken",
                //Value = "CcFoBNhb0D7FOKjUW_TTj8A594XFXm6XYMRa0fznRPN7pCbpUh0PODoNxZ8U4oI_t9kDyZXofVzemWlQtu9DjrtAivl4r5EPdgPjfDperv41",
                Value = "i_SdxXhZGH4BHlTSH8OtHGdXZwE0XKnlKWkZJvvQPi0stvvd5JwWB",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });

            string dataString = "__RequestVerificationToken=mytoken&RegistrationNo=LEK-8888&VehicleMake=Honda&VehicleColor=Red&AvailableToDouble=true&RentPerWeek=20000&VehicleConditionId=2&VehicleTypeId=2&IsLivery=false&ProprietorId=6";
            byte[] rawData = Encoding.UTF8.GetBytes(dataString);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = rawData.Length;
            request.Method = "POST";

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(rawData, 0, rawData.Length);
            }

            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (Stream outStream = File.Create("search.html"))
            {
                var httpresponse = response as HttpWebResponse;
                stream.CopyTo(outStream);
                outStream.Flush();
            }
        }

        static void Login()
        {
            string uri = "http://vehiclerenting.apphb.com/Account/Login";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string authInfo = "__RequestVerificationToken=OBD7VcOPn9SC5n3u_GmYaD64Y0snjNB6epbHtMT6l9VpjjMP6F9_gKwNkNDKqPUSStaVwy8wpu852IT2VcAcMSvLcNmGHPoWdjF7W0X4c5w1&Username=admin&Password=updating";
            byte[] rawAuthInfo = Encoding.UTF8.GetBytes(authInfo);
            request.ContentLength = rawAuthInfo.Length;

            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            cookieContainer.Add(new Cookie
            {
                Name = ".AspNet.ApplicationCookie",
                Value = "DbClGd0E7S1ZlhppKspHSeUFg1s9cGAzm95ng5KRuBpDa4OfKz5MVXTHTKQLrB4SET5BYFd-Ekfde0KIzLn1YB4AYbRGISwXenc60XEn1s6-0jNXe_-MnL-tuxtIeJR2BxWe000819O2DXwtiIurAP3FDsQ3pq_pmcooDwDSAzzU8cBgcaFArG_ulM52MSQnymcrQuID-aiiKcjN2PFPYCHQ8eOYu1xWPkTg70408YPwuRoCHe8N26MavJIH94aeTWsyMwwqycytsvLAyjxVgw1KCTWVgb_hexywvDtEMPexff-W8WPpGcP1s7cS7gAf2QBwe2Qr3Oru0EIA1et5RhUjRMGRiXhoWCOfjv-40sTGG6HK-Geo7HFd4XVyaqgaByPLm0cleTp0X70sRQrROZc5wo2sVrqpT1TobqlzoSh1PrBhjkbFoB8gnExWZZcM8J5hOWNtcZvZwxZ3Y6tMPedbn7MlM5qno3qi_CUh_ZCWT8BScJhLX7GSwtY2Jt_f",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });
            cookieContainer.Add(new Cookie
            {
                Name = "__RequestVerificationToken",
                //Value = "CcFoBNhb0D7FOKjUW_TTj8A594XFXm6XYMRa0fznRPN7pCbpUh0PODoNxZ8U4oI_t9kDyZXofVzemWlQtu9DjrtAivl4r5EPdgPjfDperv41",
                Value = "i_SdxXhZGH4BHlTSH8OtHGdXZwE0XKnlKWkZJvvQPi0stvvd5JwWB",
                Domain = "vehiclerenting.apphb.com",
                Path = "/",
                HttpOnly = true
            });

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(rawAuthInfo, 0, rawAuthInfo.Length);
                requestStream.Flush();
            }

            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (Stream outStream = File.Create("search.html"))
            {
                var httpresponse = response as HttpWebResponse;
                stream.CopyTo(outStream);
                outStream.Flush();
            }

        }
    }
}
