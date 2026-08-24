namespace Ecs.Mjj;

public class MjjStatus
{
    public string QuNo { get; set; } = "error";
    public string Temp { get; set; } = "error";
    public string Hum { get; set; } = "error";
    public string Pm2_5 { get; set; } = "error";
    public string Pm10 { get; set; } = "error";
    public string Tvoc { get; set; } = "error";
    public string Co2 { get; set; } = "error";
    public string ColNo { get; set; } = "error";
    public string MjjZTLX { get; set; } = "error";
    public string MjjZTLXName { get; set; } = "error";
    public string ColumnDWZT_changed { get; set; } = "error";
    public string Data { get; set; } = "error";
    public string IsBJ { get; set; } = "error";
    public string IsLock { get; set; } = "error";
    public string IsVent { get; set; } = "error";
    public string IsPower { get; set; } = "error";
    public string IsZDKJ { get; set; } = "error";
    public string ColumnStatus { get; set; } = "error";

    public static bool operator==(MjjStatus one, MjjStatus two)
    {
        if(ReferenceEquals(one, two))
            return true;
        if(ReferenceEquals(one, null)) //one是null，那么two就不是null，肯定不相等
            return false;
        if(ReferenceEquals(two, null)) //two是null，那么one就不是null，肯定不相等
            return false;
        bool bEqual = 
            one.QuNo == two.QuNo &&
            one.Temp == two.Temp &&
            one.Hum == two.Hum &&
            one.Pm2_5 == two.Pm2_5 &&
            one.Pm10 == two.Pm10 &&
            one.Tvoc == two.Tvoc &&
            one.Co2 == two.Co2 &&
            one.ColNo == two.ColNo &&
            one.MjjZTLX == two.MjjZTLX &&
            one.MjjZTLXName == two.MjjZTLXName &&
            one.ColumnDWZT_changed == two.ColumnDWZT_changed &&
            one.Data == two.Data &&
            one.IsBJ == two.IsBJ &&
            one.IsLock == two.IsLock &&
            one.IsVent == two.IsVent &&
            one.IsPower == two.IsPower &&
            one.IsZDKJ == two.IsZDKJ &&
            one.ColumnStatus == two.ColumnStatus;
        return bEqual;
    }

    public static bool operator!=(MjjStatus one, MjjStatus two) => !(one == two);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        if(obj.GetType() != typeof(MjjStatus))
        {
            return false;
        }

        return this == (MjjStatus)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}