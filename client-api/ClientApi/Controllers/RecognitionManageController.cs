using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using ClientApi.Model;

namespace ClientApi.Controllers
{
    /// <summary>旧版人脸识别兼容接口；当前未执行实际身份匹配。</summary>
    [ApiController]
    [Route("[controller]")]
    //[Route("api/student")]
    public class RecognitionManageController : ControllerBase
    {

        private readonly ILogger<RecognitionManageController> _logger;
        //private static double[] ia = new double[128] { -0.036260541528463364, 0.1375238448381424, 0.0673421323299408, -0.08039717376232147, -0.08465802669525146, -0.047540295869112015, -0.07322223484516144, -0.048198431730270386, 0.19035032391548157, -0.05380454659461975, 0.1912844479084015, -0.011881722137331963, -0.20624810457229614, -0.05887147784233093, -0.034033630043268204, 0.09228028357028961, -0.15393982827663422, -0.15142881870269775, -0.11919481307268143, -0.09755194187164307, 0.007619831711053848, 0.03810957074165344, -0.06966090947389603, 0.02295624278485775, -0.12724310159683228, -0.369438111782074, -0.09973740577697754, -0.02151789702475071, 0.14768758416175842, -0.15140078961849213, -0.028882550075650215, 0.11065269261598587, -0.1454824060201645, -0.0741802453994751, 0.08221173286437988, 0.06034582853317261, -0.0143441092222929, -0.09553509950637817, 0.19915486872196198, 0.12026753276586533, -0.0940946564078331, 0.07001975923776627, 0.03535224497318268, 0.3053401708602905, 0.20324108004570007, 0.02669748291373253, 0.05953730642795563, -0.11130905151367188, 0.13825388252735138, -0.24661053717136383, 0.006133418530225754, 0.2664661407470703, 0.10743191838264465, 0.09225836396217346, 0.145631805062294, -0.11289696395397186, 0.0075978003442287445, 0.08748878538608551, -0.22219139337539673, 0.14016790688037872, 0.04092670604586601, -0.06733758002519608, -0.09410157799720764, -0.08744936436414719, 0.10237274318933487, 0.08831136673688889, -0.09130633622407913, -0.11972428858280182, 0.19515712559223175, -0.15574103593826294, -0.06737177073955536, 0.02629697695374489, -0.07696172595024109, -0.13403654098510742, -0.23079228401184082, 0.03158879280090332, 0.39006391167640686, 0.20023532211780548, -0.20438659191131592, 0.037921078503131866, -0.06551908701658249, -0.10368738323450089, 0.007759069558233023, -0.0009490097872912884, -0.2023235708475113, -0.05134391039609909, -0.03692307323217392, 0.09922811388969421, 0.2381458729505539, 0.08751512318849564, 0.022600486874580383, 0.21142466366291046, 0.01994173228740692, 0.0508643314242363, 0.028769593685865402, 0.02105984091758728, -0.12129883468151093, -0.08021678030490875, -0.12355384975671768, -0.0628545880317688, 0.08997631072998047, -0.16667591035366058, 0.011929411441087723, 0.1257106214761734, -0.21494439244270325, 0.14425034821033478, 0.02631189115345478, -0.06482863426208496, -0.0042220186442136765, 0.05771908536553383, -0.11019976437091827, 0.01499576773494482, 0.20296990871429443, -0.2226320207118988, 0.22802363336086273, 0.2259662002325058, 0.07303407043218613, 0.06466325372457504, 0.08153272420167923, 0.012206127867102623, 0.009178819134831429, -0.0925404280424118, -0.021123213693499565, -0.1546286940574646, -0.054127052426338196, -0.025267265737056732, -0.007000967860221863, -0.008819528855383396 };
        //private static FaceEncoding  faceEncodingA = FaceRecognition.LoadFaceEncoding(RecognitionManageController.ia);
        //private static FaceRecognition fr = FaceRecognition.Create(Startup.HostEnv.ContentRootPath+ @"\bin\x64\Debug\netcoreapp3.1\models");
        ////模型对应的标记名
        //private static Dictionary<FaceEncoding, string> _faceDictionary= new Dictionary<FaceEncoding, string> {
        //    {faceEncodingA,"8"}
        //};
        public RecognitionManageController(ILogger<RecognitionManageController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 接收旧版 Base64 人脸图片请求。
        /// </summary>
        /// <param name="faceData">旧版识别请求，当前仅使用 image_content 字段。</param>
        /// <returns>JSON 文本格式的识别结果；当前实现返回失败，不返回匹配身份。</returns>
        /// <remarks>实际人脸识别请使用 /Face/FaceMatch。</remarks>
        [HttpPost]
        public IActionResult Post([FromBody] RecognitionInputDto faceData)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                byte[] imageBytes = Convert.FromBase64String(faceData.image_content);
                if (imageBytes.Length > 0)
                {
                    var bitmap = ToFormat24bpprgb(imageBytes);
                    return Content(JsonConvert.SerializeObject(new RecognitionOutputDto(1, "0", 0.0, "Bad Image Data")));

                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new RecognitionOutputDto(1, "0", 0.0, "Bad Image Data")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Content(JsonConvert.SerializeObject(new RecognitionOutputDto(1, "0", 0.0, "Exception"+e.Message)));
            }
        }


        private static Bitmap ToFormat24bpprgb(byte[] bs)
        {
            MemoryStream ms = new MemoryStream(bs);
            System.Drawing.Image imgr = System.Drawing.Image.FromStream(ms);
            Bitmap bmp = new Bitmap(imgr.Width, imgr.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(imgr, new System.Drawing.Point(0, 0));
            g.Dispose();
            imgr.Dispose();
            ms.Dispose();
            return bmp;
        }


    }

}
