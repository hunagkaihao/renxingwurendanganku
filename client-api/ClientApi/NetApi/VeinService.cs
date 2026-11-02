using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApi.NetApi
{
    /// <summary>
    /// 指静脉接口
    /// </summary>
    public class VeinService
    {
        /// <summary>
        /// 获取用户的指纹数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
    //    public static List<UserVein> GetUserVeinsData(int userId)
    //    {
    //        try
    //        {
    //            Logger.Singleton.Debug("GetUserVeinsData  UserId" + userId.ToString());
    //            List<UserVein> userVeins = new List<UserVein>();
    //            Dictionary<string, Object> dic = new Dictionary<string, Object>();
    //            dic.Add("userId", userId);
    //            var req = new RestApi<ResultDto>().Get(ServerUrl.ApiUrl,
    //"/api/TokenAuth/GetVeinsByUserId", dic);
    //            if (req.success == false)
    //            {
    //                Logger.Singleton.Debug("GetUserVeinsData失败");
    //                return null;
    //            }
    //            else
    //            {
    //                Logger.Singleton.Debug("GetUserVeinsData成功");
    //                var result =JsonConvert.DeserializeObject(req.result.ToString()) as JArray;
    //                //var result = req.result as JArray;
    //                //var items = result1["items"] as JArray;
    //                for (int i = 0; i < result.Count; i++)
    //                {
    //                    UserVein userVein = new UserVein();
    //                    userVein.VeinId = Convert.ToInt32(result[i]);
    //                    userVein.UserId = userId;
    //                    userVeins.Add(userVein);
    //                }

    //                return userVeins;
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.Singleton.Debug("GetUserVeinsData异常" + ex.Message);
    //            //_log.Error(String.Format("response {0},", responseText));               
    //            return null;
    //        }


    //    }
        /// <summary>
        /// 上传指静脉数据
        /// </summary>
        /// <param name="featureData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int UploadUserVeinData(byte[] featureData, int userId)
        {
            try
            {
                var contact = new
                {
                    userId = userId,
                    featureData = Convert.ToBase64String(featureData),
                    featureCnt = 6
                };
                var req = new RestApi<ResultDto>().Post(ServerUrl.AMSApiUrl,
                    "/api/services/app/User/SaveUserVeinData", contact);
                if (req.success == false)
                {
                    Logger.Singleton.Debug($"UploadUserVeinData失败{userId}");
                    Logger.Singleton.Debug(Convert.ToBase64String(featureData));
                    //var error = req["error"] as JObject;
                    Logger.Singleton.Debug(req.error.message);
                    return 0;
                }
                else
                {
                    Logger.Singleton.Debug($"UploadUserVeinData成功{userId}-{req.result.ToString()}");
                    return Convert.ToInt16(req.result.ToString());
                }
            }
            catch (Exception ex)
            {

                Logger.Singleton.Debug("UploadUserVeinData异常" + ex.Message);
                return 0;
            }

        }
        /// <summary>
        /// 删除指静脉数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool DeleteVeinDataById(int id)
        {
            try
            {
                Dictionary<string, Object> dic = new Dictionary<string, Object>();
                dic.Add("id", id);
                var req = new RestApi<ResultDto>().Delete(ServerUrl.AMSApiUrl,
                    "/api/services/app/User/DeleteVeinDataById", dic);
                if (req.success == false)
                {
                    Logger.Singleton.Debug($"DeleteVeinDataById失败 {id} {req.error.message}");
                    return false;
                }
                else
                {
                    Logger.Singleton.Debug("DeleteVeinDataById成功");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error($"调用DeleteVeinDataById失败{id}{ex.Message}");
                return false;
            }

        }
    }
}
