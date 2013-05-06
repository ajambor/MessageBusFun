namespace MessageBusFun
{
    public interface IProvider
    {
        string Channel { get; set; }
        string Name { get; set; }
    }
}