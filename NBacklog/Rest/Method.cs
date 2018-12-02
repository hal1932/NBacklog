using System.Net.Http;

namespace NBacklog.Rest
{
    public class Method
    {
        public static readonly Method GET = new Method(HttpMethod.Get.Method);
        public static readonly Method POST = new Method(HttpMethod.Post.Method);
        public static readonly Method PUT = new Method(HttpMethod.Put.Method);
        public static readonly Method PATCH = new Method("Patch");
        public static readonly Method DELETE = new Method(HttpMethod.Delete.Method);

        public HttpMethod HttpMethod { get; }

        public Method(string name)
        {
            HttpMethod = new HttpMethod(name);
        }
    }
}
