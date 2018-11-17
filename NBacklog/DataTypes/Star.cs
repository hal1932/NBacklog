using System;

namespace NBacklog.DataTypes
{
    public class Star : BacklogItem
    {
        public object Comment { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public User Creator { get; set; }
        public DateTime Created { get; set; }

        internal Star(_Star data, BacklogClient client)
            : base(data.id)
        {
            Comment = data.comment;
            Url = data.url;
            Title = data.title;
            Creator = client.ItemsCache.Get(data.presenter.id, () => new User(data.presenter, client));
            Created = data.created;
        }
    }
}
