namespace Wcs.Dispatch
{
    public enum EnumDispatchNodeState
    {
        Idle = 0, //空闲，可使用
        Working,  //工作中，不可使用
        Disabled  //禁用
    }
}