using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
using System.Net;
using System.Text;
//using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ABankAdmin.Core.Utils
{
    public class NotificationHelper
    {
        public bool PushNotificationByFireBase(SendNotificationRequestModel fireNotificationRequestModel)
        {
            var RespDescription = "";
            //string errMsg = "";
            try
            {
                var requestModel = new PushNotificationRequestModel();
                requestModel.to = fireNotificationRequestModel.FireBaseToken;
                requestModel.priority = "high";
                var notification = new Notification();
                var datanew = new Data();

                var bodymsg = fireNotificationRequestModel.MessageBody;
                notification.body = bodymsg;
                datanew.body = bodymsg;

                notification.title = fireNotificationRequestModel.Title;
                datanew.title = notification.title;

                notification.sound = "default";
                notification.badge = "0";
                requestModel.notification = notification;


                datanew.type = "NOTI";

                requestModel.data = datanew;

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = requestModel;
                var jsonstri = JsonConvert.SerializeObject(requestModel);

                //var serializer = new JavaScriptSerializer();
                //var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(jsonstri);
                tRequest.Headers.Add(string.Format("Authorization: key={0}",
                        @"AAAAdrb6NIU:APA91bHWo8D1dT9CJmcZKeUavcxKwn4qS-ncA2Xk6neS7pAvuH0w7yoZlpboUUkg9Tayv7hpGKYRF6ikP37ymEcvnyPh7NQoPKwubKEnHmy1sx1521cO5B-g23OCfdZiR0X8eH31VjHh"));


                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            RespDescription = sResponseFromServer;

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            finally
            {
                //log

            }
        }

        public class SendNotificationRequestModel
        {
            public string UserType { get; set; }
            public string FireBaseToken { get; set; }
            public string Title { get; set; }
            public string MessageBody { get; set; }
            public string NotiType { get; set; }
        }

        public class PushNotificationRequestModel
        {
            public string to { get; set; }
            public string priority { get; set; }
            public Notification notification { get; set; }
            public Data data { get; set; }
        }

        public class Notification
        {
            public string body { get; set; }
            public string title { get; set; }
            public string sound { get; set; }
            public string badge { get; set; }
        }

        public class Data
        {
            public string type { get; set; }
            public string receivedAmount { get; set; }
            public string body { get; set; }
            public string title { get; set; }
            public string invoiceNo { get; set; }
            public string merchantUserID { get; set; }
            public string referenceIntegrationID { get; set; }
            public string merchantName { get; set; }

        }
    }
}
