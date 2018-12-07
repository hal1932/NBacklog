using System;

namespace NBacklog
{
    public class BacklogClientConfig
    {
        /// <summary>
        /// HTTP ステータスコードが 4xx だったときに例外を投げるか？
        /// </summary>
        public bool ThrowOnClientError { get; set; } = false;

        /// <summary>
        /// HTTP ステータスコードが 5xx だったときにリトライするか？
        /// </summary>
        public bool RetryOnServerError { get; set; } = true;

        /// <summary>
        /// 最大リトライ回数
        /// </summary>
        public int MaxRetryCount { get; set; } = 2;

        /// <summary>
        /// リトライ間隔
        /// </summary>
        public TimeSpan RetrySpan { get; set; } = TimeSpan.FromMilliseconds(100);
    }
}
