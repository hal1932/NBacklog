using Newtonsoft.Json.Linq;

namespace NBacklog.DataTypes
{
    public class Link : BacklogItem
    {
        public int KeyId { get; }
        public string Title { get; }

        internal Link(JObject data)
            : base(data.Value<int>("id"))
        {
            KeyId = data.Value<int>("key_id");
            Title = data.Value<string>("title");
        }
    }
}
