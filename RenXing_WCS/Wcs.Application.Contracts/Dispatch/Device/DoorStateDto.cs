namespace Wcs.Dispatch.Device
{
    public class DoorStateDto
    {
        public bool success { get; set; }

        public string message { get; set; }

        /// <summary>
        /// 取档口状态：true：闭合，false：打开
        /// </summary>
        public bool doorState { get; set; }
    }
}
