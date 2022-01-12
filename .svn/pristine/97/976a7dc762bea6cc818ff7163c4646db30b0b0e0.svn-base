using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABankAdmin.ViewModels
{
    public class GetImageJsonStringRequest
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public string ClientVersion { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }

        //get
        public string ImagePath { get; set; }
    }

    public class SaveImageJsonStringRequest
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public string ClientVersion { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceModel { get; set; }

        //save
        public string Base64Image { get; set; }

        public string ImageName { get; set; }
        public string ImageFileType { get; set; }
    }

    public class ImageRequestModel
    {
        public string JsonStringRequest { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public string DeviceID { get; set; }
        public string IV { get; set; }
        public string SessionID { get; set; }
    }

    public class GetImageJsonStringResponse
    {
        //get
        public string Base64Image { get; set; }

        public string RespCode { get; set; }
        public string RespDescription { get; set; }
    }

    public class SaveImageJsonStringResponse
    {
        //save
        public string ImagePath { get; set; }

        public string RespCode { get; set; }
        public string RespDescription { get; set; }
    }

    public class ImageResponseModel
    {
        public string JsonStringResponse { get; set; }
        public string SessionID { get; set; }
        public string RespCode { get; set; }
        public string RespDescription { get; set; }
    }
}